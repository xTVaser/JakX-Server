﻿using JakX.Server.Messages;
using JakX.Server.Messages.RTIME;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace JakX.Server.Mods
{
    public class Gamemode
    {
        /// <summary>
        /// Whether or not the game mode is enabled.
        /// </summary>
        public bool Enabled = false;

        /// <summary>
        /// Application id the game mode is for.
        /// </summary>
        public int ApplicationId = 0;

        /// <summary>
        /// The fullname of the game mode.
        /// </summary>
        public string FullName = null;

        /// <summary>
        /// Strings that satisfy !gm <key> for this game mode.
        /// </summary>
        public string[] Keys = null;

        /// <summary>
        /// The address to inject the game mode to.
        /// </summary>
        public string Address = "0x00000000";

        /// <summary>
        /// Path to the game mode payload.
        /// </summary>
        public string BinPath { get; set; } = null;


        private uint address = 0;

        /// <summary>
        /// Whether or not the given game mode is valid.
        /// </summary>
        public bool IsValid(int appId)
        {
            return Enabled && appId == ApplicationId && address != 0 && File.Exists(BinPath);
        }

        /// <summary>
        /// Apply game mode to a given set of clients.
        /// </summary>
        public void Apply(IEnumerable<ClientObject> clients)
        {
            List<BaseMessage> messages = new List<BaseMessage>();

            // Add paylaod
            messages.AddRange(RT_MSG_SERVER_MEMORY_POKE.FromPayload(address, File.ReadAllBytes(BinPath)));

            // Add module
            byte[] moduleEntry = new byte[16];
            Array.Copy(BitConverter.GetBytes((int)1), 0, moduleEntry, 0, 4);
            Array.Copy(BitConverter.GetBytes(address), 0, moduleEntry, 4, 4);

            // 
            messages.Add(new RT_MSG_SERVER_MEMORY_POKE()
            {
                Address = 0x000CF000 + (0 * 16),
                Payload = moduleEntry
            });

            // Send each
            foreach (var target in clients)
                if (target != null && target.IsConnected)
                    target.AddLobbyMessages(messages);
        }

        /// <summary>
        /// Disables the gamemode module.
        /// </summary>
        public static void Disable(IEnumerable<ClientObject> clients)
        {
            // reset
            var modulePokes = RT_MSG_SERVER_MEMORY_POKE.FromPayload(0x000CF000 + (0 * 16), new byte[16]);
            foreach (var player in clients)
                player.AddLobbyMessages(modulePokes);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (Address.StartsWith("0x"))
                address = Convert.ToUInt32(Address, 16);
            else
                address = Convert.ToUInt32(Address);
        }
    }
}
