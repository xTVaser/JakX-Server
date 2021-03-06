﻿using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.Lobby
{
    [MediusApp(MediusAppPacketIds.DnasSignaturePost)]
    public class MediusDnasSignaturePost : BaseLobbyMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.DnasSignaturePost;

        public string SessionKey; // SESSIONKEY_MAXLEN
        public MediusDnasCategory DnasSignatureType;
        public byte DnasSignatureLength;
        public byte[] DnasSignature = new byte[MediusConstants.DNASSIGNATURE_MAXLEN];

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            SessionKey = reader.ReadString(MediusConstants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            DnasSignatureType = reader.Read<MediusDnasCategory>();
            DnasSignatureLength = reader.ReadByte();
            DnasSignature = reader.ReadBytes(MediusConstants.DNASSIGNATURE_MAXLEN);
            reader.ReadBytes(3);
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(SessionKey, MediusConstants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(DnasSignatureType);
            writer.Write(DnasSignatureLength);
            writer.Write(DnasSignature);
            writer.Write(new byte[3]);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"SessionKey:{SessionKey}" + " " +
$"DnasSignatureType:{DnasSignatureType}" + " " +
$"DnasSignatureLength:{DnasSignatureLength}" + " " +
$"DnasSignature:{BitConverter.ToString(DnasSignature)}";
        }
    }
}