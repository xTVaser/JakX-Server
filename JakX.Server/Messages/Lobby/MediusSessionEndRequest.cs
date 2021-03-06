﻿using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.Lobby
{
    [MediusApp(MediusAppPacketIds.SessionEnd)]
    public class MediusSessionEndRequest : BaseLobbyMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.SessionEnd;

        public string SessionKey; // SESSIONKEY_MAXLEN

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            SessionKey = reader.ReadString(MediusConstants.SESSIONKEY_MAXLEN);
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(SessionKey, MediusConstants.SESSIONKEY_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"SessionKey:{SessionKey}";
        }
    }
}