﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebsocketServerRunner.cs" company="Chromely Projects">
//   Copyright (c) 2017-2018 Chromely Projects
// </copyright>
// <license>
//      See the LICENSE.md file in the project root for more information.
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace Chromely.CefGlue.Gtk
{
    using System;

    using Chromely.CefGlue.Gtk.Browser.ServerHandlers;
    using Chromely.Core;
    using Chromely.Core.Infrastructure;

    /// <summary>
    /// The websocket server runner.
    /// </summary>
    public static class WebsocketServerRunner
    {
        /// <summary>
        /// The m server handler.
        /// </summary>
        private static CefGlueServerHandler mServerHandler;

        /// <summary>
        /// Gets the server address.
        /// </summary>
        public static string Address { get; private set; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        public static int Port { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is server running.
        /// </summary>
        public static bool IsServerRunning
        {
            get
            {
                if (mServerHandler == null)
                {
                    return false;
                }

                return mServerHandler.IsServerRunning;
            }
        }

        /// <summary>
        /// The start server.
        /// </summary>
        public static void StartServer()
        {
            StartServer(0);
        }

        /// <summary>
        /// The start server.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        public static void StartServer(int port)
        {
            StartServer(string.Empty, port);
        }

        /// <summary>
        /// The start server.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="port">
        /// The port.
        /// </param>
        public static void StartServer(string address, int port)
        {
            try
            {
                Address = address;
                Port = port;

                if (port < 8000 || port > 65535)
                {
                    string errorMessage = $"Invalid port number: {port} specified. Port number must be between 8000 and 65535";
                    Log.Error(errorMessage);
                    return;
                }

                var sockeHandler = IoC.GetInstance<IChromelyWebsocketHandler>(typeof(IChromelyWebsocketHandler).FullName)
                                   ?? new CefGlueWebsocketHandler();

                ConnectionNameMapper.Clear();
                mServerHandler = new CefGlueServerHandler(sockeHandler);
                mServerHandler.StartServer(Address, Port, OnStartServerComplete);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        /// <summary>
        /// The stop server.
        /// </summary>
        public static void StopServer()
        {
            try
            {
                if (mServerHandler == null)
                {
                    Log.Info("Cannot stop server. Server is not currently running.");
                    return;
                }

                // Stop the server. OnComplete will be executed upon completion.
                mServerHandler.StopServer(OnStopServerComplete);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        /// <summary>
        /// The on start server complete.
        /// </summary>
        private static void OnStartServerComplete()
        {
            Log.Info($"Server started on {Address} and port {Port}.");
        }

        /// <summary>
        /// The on stop server complete.
        /// </summary>
        private static void OnStopServerComplete()
        {
            if (mServerHandler != null)
            {
                Log.Info($"Server on {Address} and port {Port} stopped.");
                mServerHandler.DisposeServer();
                mServerHandler = null;
            }
        }
    }
}