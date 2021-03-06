﻿using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.Lobby
{
    [MediusApp(MediusAppPacketIds.FindPlayer)]
    public class MediusFindPlayerRequest : BaseLobbyMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.FindPlayer;

        public string SessionKey; // SESSIONKEY_MAXLEN
        public MediusPlayerSearchType SearchType;
        public int ID;
        public string Name; // PLAYERNAME_MAXLEN

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            SessionKey = reader.ReadString(MediusConstants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            SearchType = reader.Read<MediusPlayerSearchType>();
            ID = reader.ReadInt32();
            Name = reader.ReadString(MediusConstants.PLAYERNAME_MAXLEN);
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(SessionKey, MediusConstants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(SearchType);
            writer.Write(ID);
            writer.Write(Name, MediusConstants.PLAYERNAME_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"SessionKey:{SessionKey}" + " " +
$"SearchType:{SearchType}" + " " +
$"ID:{ID}" + " " +
$"Name:{Name}";
        }
    }
}