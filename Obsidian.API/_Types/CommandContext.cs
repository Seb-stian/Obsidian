﻿using Obsidian.API;
using Obsidian.API.Plugins;
using System;

namespace Obsidian.API
{
    public class CommandContext
    {
        internal string Message;

        public CommandContext(string message, IPlayer player, IServer server/*, IClient client*/)
        {
            this.Player = player;
            this.Server = server;
            this.Message = message;
            //this.Client = client;
        }

        public IPlayer Player { get; private set; }

        public IServer Server { get; private set; }

        public PluginBase Plugin { get; internal set; }

        // public IClient Client { get; private set; }
    }
}