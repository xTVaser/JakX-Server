using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.MGCL
{
    [MediusApp(MediusAppPacketIds.MediusServerSessionEndRequest)]
    public class MediusServerSessionEndRequest : BaseMGCLMessage
    {
        public override MediusAppPacketIds Id => MediusAppPacketIds.MediusServerSessionEndRequest;


        public override string ToString()
        {
            return base.ToString();
        }
    }
}