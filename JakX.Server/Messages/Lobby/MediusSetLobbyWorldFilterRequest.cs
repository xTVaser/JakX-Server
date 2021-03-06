﻿using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.Lobby
{
    [MediusApp(MediusAppPacketIds.SetLobbyWorldFilter)]
    public class MediusSetLobbyWorldFilterRequest : BaseLobbyMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.SetLobbyWorldFilter;

        public uint FilterMask1;
        public uint FilterMask2;
        public uint FilterMask3;
        public uint FilterMask4;
        public MediusLobbyFilterType LobbyFilterType;
        public MediusLobbyFilterMaskLevelType FilterMaskLevel;

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            reader.ReadBytes(3);
            FilterMask1 = reader.ReadUInt32();
            FilterMask2 = reader.ReadUInt32();
            FilterMask3 = reader.ReadUInt32();
            FilterMask4 = reader.ReadUInt32();
            LobbyFilterType = reader.Read<MediusLobbyFilterType>();
            FilterMaskLevel = reader.Read<MediusLobbyFilterMaskLevelType>();
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(new byte[3]);
            writer.Write(FilterMask1);
            writer.Write(FilterMask2);
            writer.Write(FilterMask3);
            writer.Write(FilterMask4);
            writer.Write(LobbyFilterType);
            writer.Write(FilterMaskLevel);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"FilterMask1:{FilterMask1}" + " " +
$"FilterMask2:{FilterMask2}" + " " +
$"FilterMask3:{FilterMask3}" + " " +
$"FilterMask4:{FilterMask4}" + " " +
$"LobbyFilterType:{LobbyFilterType}" + " " +
$"FilterMaskLevel:{FilterMaskLevel}";
        }
    }
}