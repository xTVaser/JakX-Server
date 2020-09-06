using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.Lobby
{
    [MediusApp(MediusAppPacketIds.AccountUpdatePassword)]
    public class MediusAccountUpdatePasswordRequest : BaseLobbyMessage
    {
        public override MediusAppPacketIds Id => MediusAppPacketIds.AccountUpdatePassword;

        public string SessionKey; // SESSIONKEY_MAXLEN
        public string OldPassword; // PASSWORD_MAXLEN
        public string NewPassword; // PASSWORD_MAXLEN

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            SessionKey = reader.ReadString(MediusConstants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            OldPassword = reader.ReadString(MediusConstants.PASSWORD_MAXLEN);
            NewPassword = reader.ReadString(MediusConstants.PASSWORD_MAXLEN);
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(SessionKey, MediusConstants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(OldPassword, MediusConstants.PASSWORD_MAXLEN);
            writer.Write(NewPassword, MediusConstants.PASSWORD_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"SessionKey:{SessionKey}" + " " +
$"OldPassword:{OldPassword}" + " " +
$"NewPassword:{NewPassword}";
        }
    }
}
