﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.Lobby
{
    [MediusApp(MediusAppPacketIds.GetBuddyList_ExtraInfo)]
    public class MediusGetBuddyList_ExtraInfoRequest : BaseLobbyMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.GetBuddyList_ExtraInfo;

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);
        }


        public override string ToString()
        {
            return base.ToString();
        }
    }
}
