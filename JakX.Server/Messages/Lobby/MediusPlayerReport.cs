﻿using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.Lobby
{
    [MediusApp(MediusAppPacketIds.PlayerReport)]
    public class MediusPlayerReport : BaseAppMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.PlayerReport;

        public string SessionKey; // SESSIONKEY_MAXLEN
        public int MediusWorldID;
        public byte[] Stats = new byte[MediusConstants.ACCOUNTSTATS_MAXLEN]; 

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            SessionKey = reader.ReadString(MediusConstants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(3);
            MediusWorldID = reader.ReadInt32();
            Stats = reader.ReadBytes(MediusConstants.ACCOUNTSTATS_MAXLEN);
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(SessionKey, MediusConstants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[3]);
            writer.Write(MediusWorldID);
            writer.Write(Stats);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"SessionKey:{SessionKey}" + " " +
$"MediusWorldID:{MediusWorldID}" + " " +
$"Stats:{BitConverter.ToString(Stats)}";
        }
    }
}