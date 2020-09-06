using JakX.Server.Stream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JakX.Server.Messages.Lobby
{
    [MediusApp(MediusAppPacketIds.SetGameListFilter)]
    public class MediusSetGameListFilterRequest : BaseLobbyMessage
    {

        public override MediusAppPacketIds Id => MediusAppPacketIds.SetGameListFilter;

        public string SessionKey; // SESSIONKEY_MAXLEN
        public MediusGameListFilterField FilterField;
        //public uint Mask;
        public MediusComparisonOperator ComparisonOperator;
        public int BaselineValue;

        public override void Deserialize(BinaryReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            SessionKey = reader.ReadString(MediusConstants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            FilterField = reader.Read<MediusGameListFilterField>();
            //Mask = reader.ReadUInt32();
            ComparisonOperator = reader.Read<MediusComparisonOperator>();
            BaselineValue = reader.ReadInt32();
        }

        public override void Serialize(BinaryWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(SessionKey, MediusConstants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(FilterField);
            //writer.Write(Mask);
            writer.Write(ComparisonOperator);
            writer.Write(BaselineValue);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"SessionKey:{SessionKey}" + " " +
$"FilterField:{FilterField}" + " " +
//$"Mask:{Mask}" + " " +
$"ComparisonOperator:{ComparisonOperator}" + " " +
$"BaselineValue:{BaselineValue}";
        }
    }
}
