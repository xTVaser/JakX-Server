﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.RTIME
{
    [Message(RT_MSG_TYPE.RT_MSG_CLIENT_HELLO)]
    public class RT_MSG_CLIENT_HELLO : BaseMessage
    {

        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_HELLO;

        // 
        public ushort[] Parameters = null;


        public override void Deserialize(BinaryReader reader)
        {
            Parameters = new ushort[5];
            for (int i = 0; i < 5; ++i)
                Parameters[i] = reader.ReadUInt16();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            for (int i = 0; i < 5; ++i)
                writer.Write((Parameters == null || i >= Parameters.Length) ? ushort.MinValue : Parameters[i]);
        }
    }
}
