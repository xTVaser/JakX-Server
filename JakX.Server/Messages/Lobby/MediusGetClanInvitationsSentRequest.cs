﻿using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.Lobby
{
    [MediusApp(MediusAppPacketIds.GetClanInvitationsSent)]
    public class MediusGetClanInvitationsSentRequest : BaseLobbyMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.GetClanInvitationsSent;

        public string SessionKey; // SESSIONKEY_MAXLEN
        public int Start;
        public int PageSize;

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            SessionKey = reader.ReadString(MediusConstants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            Start = reader.ReadInt32();
            PageSize = reader.ReadInt32();
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(SessionKey, MediusConstants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(Start);
            writer.Write(PageSize);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"SessionKey:{SessionKey}" + " " +
$"Start:{Start}" + " " +
$"PageSize:{PageSize}";
        }
    }
}