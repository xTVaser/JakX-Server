﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.RTIME
{
    [Message(RT_MSG_TYPE.RT_MSG_SERVER_CRYPTKEY_PEER)]
    public class RT_MSG_SERVER_CRYPTKEY_PEER : BaseMessage
    {

        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_CRYPTKEY_PEER;

        // 
        public byte[] Key = null;

        public override void Deserialize(BinaryReader reader)
        {
            Key = reader.ReadBytes(0x40);
        }

        protected override void Serialize(BinaryWriter writer)
        {
            if (Key == null || Key.Length != 0x40)
                throw new InvalidOperationException("Unable to serialize SERVER_SET_CLIENT_SESSION_KEY key because key is either null or not 64 bytes long!");

            writer.Write(Key);
        }
    }
}
