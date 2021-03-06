﻿using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.MGCL
{
    [MediusApp(MediusAppPacketIds.MediusServerConnectGamesRequest)]
    public class MediusServerConnectGamesRequest : BaseMGCLMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.MediusServerConnectGamesRequest;

        public string ServerIP; // MGCL_SERVERIP_MAXLEN
        public int ServerPort;
        public int GameWorldID;
        public int SpectatorWorldID;

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            ServerIP = reader.ReadString(MediusConstants.MGCL_SERVERIP_MAXLEN);
            reader.ReadBytes(3);
            ServerPort = reader.ReadInt32();
            GameWorldID = reader.ReadInt32();
            SpectatorWorldID = reader.ReadInt32();
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(ServerIP, MediusConstants.MGCL_SERVERIP_MAXLEN);
            writer.Write(new byte[3]);
            writer.Write(ServerPort);
            writer.Write(GameWorldID);
            writer.Write(SpectatorWorldID);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"ServerIP:{ServerIP}" + " " +
$"ServerPort:{ServerPort}" + " " +
$"GameWorldID:{GameWorldID}" + " " +
$"SpectatorWorldID:{SpectatorWorldID}";
        }
    }
}