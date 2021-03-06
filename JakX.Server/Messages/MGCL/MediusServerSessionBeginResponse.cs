﻿using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.MGCL
{
    [MediusApp(MediusAppPacketIds.MediusServerSessionBeginResponse)]
    public class MediusServerSessionBeginResponse : BaseMGCLMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.MediusServerSessionBeginResponse;

        public MGCL_ERROR_CODE Confirmation;
        public NetConnectionInfo ConnectInfo;

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            Confirmation = reader.Read<MGCL_ERROR_CODE>();
            reader.ReadBytes(2);
            ConnectInfo = reader.Read<NetConnectionInfo>();
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(Confirmation);
            writer.Write(new byte[2]);
            writer.Write(ConnectInfo);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"Confirmation:{Confirmation}" + " " +
$"ConnectInfo:{ConnectInfo}";
        }
    }
}