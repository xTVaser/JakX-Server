﻿using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.Lobby
{
    [MediusApp(MediusAppPacketIds.ClearGameListFilter)]
    public class MediusClearGameListFilterRequest : BaseLobbyMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.ClearGameListFilter;

        public uint FilterID;

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            reader.ReadBytes(3);
            FilterID = reader.ReadUInt32();
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(new byte[3]);
            writer.Write(FilterID);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"FilterID:{FilterID}";
        }
    }
}