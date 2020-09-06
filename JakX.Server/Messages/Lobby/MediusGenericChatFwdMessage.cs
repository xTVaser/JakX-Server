using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.Lobby
{
    [MediusApp(MediusAppPacketIds.GenericChatFwdMessage)]
    public class MediusGenericChatFwdMessage : BaseAppMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.GenericChatFwdMessage;

        public int OriginatorAccountID;
        public string OriginatorAccountName; // ACCOUNTNAME_MAXLEN
        public MediusChatMessageType MessageType;
        public string Message; // CHATMESSAGE_MAXLEN

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            reader.ReadBytes(24);
            OriginatorAccountID = reader.ReadInt32();
            OriginatorAccountName = reader.ReadString(MediusConstants.ACCOUNTNAME_MAXLEN);
            MessageType = reader.Read<MediusChatMessageType>();
            Message = reader.ReadString(MediusConstants.CHATMESSAGE_MAXLEN);
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(new byte[24]);
            writer.Write(OriginatorAccountID);
            writer.Write(OriginatorAccountName, MediusConstants.ACCOUNTNAME_MAXLEN);
            writer.Write(MessageType);
            writer.Write(Message, MediusConstants.CHATMESSAGE_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"OriginatorAccountID:{OriginatorAccountID}" + " " +
$"OriginatorAccountName:{OriginatorAccountName}" + " " +
$"MessageType:{MessageType}" + " " +
$"Message:{Message}";
        }
    }
}
