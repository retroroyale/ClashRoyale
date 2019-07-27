using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceFullEntryUpdateMessage : PiranhaMessage
    {
        public AllianceFullEntryUpdateMessage(Device device) : base(device)
        {
            Id = 24324;
        }

        public Alliance Alliance { get; set; }

        public override void Encode()
        {
            Writer.WriteScString(Alliance.Description);
            Writer.WriteVInt(Alliance.Badge);
            Writer.WriteVInt(Alliance.Badge);

            /*
              if ( *((_DWORD *)v1 + 19) )
  {
    ByteStream::writeBoolean((AllianceFullEntryUpdateMessage *)((char *)v1 + 8), 1);
    result = ChecksumEncoder::writeLong(
               (AllianceFullEntryUpdateMessage *)((char *)v1 + 8),
               *((const LogicLong **)v1 + 19));
  }
  else
  {
    result = ByteStream::writeBoolean((AllianceFullEntryUpdateMessage *)((char *)v1 + 8), 0);
  }
             */

            Writer.WriteBoolean(true);
            Writer.WriteLong(Alliance.Id);
        }
    }
}