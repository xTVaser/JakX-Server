﻿using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.MGCL
{
    [MediusApp(MediusAppPacketIds.MediusServerDisconnectPlayerRequest)]
    public class MediusServerDisconnectPlayerRequest : BaseMGCLMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.MediusServerDisconnectPlayerRequest;

        public int DmeWorldID;
        public int DmeClientIndex;

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            reader.ReadBytes(3);
            DmeWorldID = reader.ReadInt32();
            DmeClientIndex = reader.ReadInt32();
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(new byte[3]);
            writer.Write(DmeWorldID);
            writer.Write(DmeClientIndex);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"DmeWorldID:{DmeWorldID}" + " " +
$"DmeClientIndex:{DmeClientIndex}";
        }
    }
}