﻿using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.MGCL
{
    [MediusApp(MediusAppPacketIds.MediusServerEndGameOnMeResponse)]
    public class MediusServerEndGameOnMeResponse : BaseMGCLMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.MediusServerEndGameOnMeResponse;

        public MGCL_ERROR_CODE Confirmation;

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            Confirmation = reader.Read<MGCL_ERROR_CODE>();
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(Confirmation);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"Confirmation:{Confirmation}";
        }
    }
}