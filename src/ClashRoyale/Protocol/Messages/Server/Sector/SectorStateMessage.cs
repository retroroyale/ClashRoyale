using ClashRoyale.Logic;
using ClashRoyale.Logic.Battle;
using ClashRoyale.Utilities.Netty;
using ClashRoyale.Utilities.Utils;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class SectorStateMessage : PiranhaMessage
    {
        public SectorStateMessage(Device device) : base(device)
        {
            Id = 21903;
            device.CurrentState = Device.State.Battle;
        }

        public LogicBattle Battle { get; set; }

        public override void Encode()
        {
            Writer.WriteBoolean(false); // IsCompressed
            Writer.WriteVInt(Battle.BattleTime); // Time
            Writer.WriteVInt(0); // Checksum
            Writer.WriteVInt(TimeUtils.CurrentUnixTimestamp); // Timestamp

            Writer.WriteVInt(11);
            Writer.WriteVInt(0); // Time
            Writer.WriteVInt(38); // Random

            Writer.WriteVInt(9);

            Writer.WriteVInt(4);
            Writer.WriteVInt(7419667);

            Writer.WriteVInt(1);

            for (var p = 0; p < Battle.Count; p++)
            {
                var player = Battle[p];

                for (var i = 0; i < 3; i++)
                {
                    Writer.WriteVInt(player.Home.HighId);
                    Writer.WriteVInt(player.Home.LowId);
                }

                Writer.WriteScString(player.Home.Name); // Player Name
                Writer.WriteVInt(player.Home.ExpLevel); // Level
                Writer.WriteVInt(player.Home.Arena.Trophies);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(32);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(8);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(1);

                var info = player.Home.AllianceInfo;
                if (info.HasAlliance)
                {
                    Writer.WriteVInt(2); // Has Clan = 2

                    Writer.WriteVInt(info.HighId); // HighId
                    Writer.WriteVInt(info.LowId); // LowId
                    Writer.WriteScString(info.Name); // Name 
                    Writer.WriteVInt(info.Badge); // Badge 
                }
                else
                    Writer.WriteVInt(0);

                Writer.WriteVInt(29);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(2);

                Writer.WriteVInt(2);
                Writer.WriteVInt(1);
                Writer.WriteVInt(5);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(1);

                Writer.WriteVInt(p == 0 ? 2 : 0); 
            }

            Battle.Encode(Writer);

            // Challenge
            //Writer.WriteHex("01B0060000789CA5535D6B1C65147EDE9939B31FB39326E6A3CD9949DD4E62D32669DDCD66376984851AA5964A2ED41B11298B59E2E2BA5B365BAC24A1F18324C4A815D39A68AD048389584AA45A83176DBDF02788A9222DBD2A82207AABE2BBDBFD78416FB467BECEFB9CE79C79CF7966B0E7D56F7E5DB62CFCB9EBDC97D797FEB81414E6BBF3C1EA05C03AFCE463C70E8F868F1D1D3D625F4D5EA48D87AE26C3581D69F0B095DC4AC2F65B24666EB5913667930E3240A12BCD64831A403BBE6EA1B6CD4672411DB3F3DF3D220B1299AB23E45B32C9EAA0961039F61B820116600DAC830D30814DB00FEC0707C041B0050E816D70037807B811DC04BE0FDC0C6E01B782DBC03BC1BBC0ED60063B6017DC01DE0DBE1F1C06EF017BE04E7017F801F05E7037781F783FB807DC0B3E003E088EB81C0527740770041C0D8E0EC78043704C383E387E38013841B8802BE06A7075B8065C826BC2F5C1F5C30DC00DEE77AD8F841B826BC36D947D07B5EEC50F44C9C91452B9F0C4897CA178E0C3E46721BCDFBEB293865B218CA7EF2C490234EDD24D51BDE4DA7C385D7821950B5EEB5A36BEEA2A31E095EF7E9BC4D224699F1BD5C97FDC509DFCDB26B59D31A97DF3A70BBF09E2405983B99A06D7F691EFAC4696472D7E7202AFDDA3063AB7EA650D164459049D7951545530CA2AE81C166509F6962568AA48509A7F7379FEC63DCC5C2FCDDC4ABA21C3B5E56C02DAEEF7DECC943EDF6C7E227C22533C39969FB82CCE369CD7B0616ED1A92B4217CEC24A1867866A63177E0D66F9B3AFCC5DB155FDE69414C2EC1495039D10F2802090208D743288FCF6DA6D71FE2D1D388DEB53D05051E962622D5DC2660C883A5627D640995E63D6B23716C5D6C10A5835E951D9395DB1BBBCF96FAB25AB46866F4A4CCB6A644A2EFE91F61FED93EEFAB9912C9D155B136BE27FBA7FE90F62503F27E43FD7E30FFB7B61CB26C8D43443137AB50D99D00484A47BE197DFEFD8E8114B728DE6C7D3E3E9DC58AAF07278E4F954369BCE8DA7B5CDEF7F5EB6363F5D58B136DFB9B56C29DBFF37BAD4707DD27B2A53CCA6BD61EF87D9EDF51BAF6F7F11968F57B62FCBFBEC8DB9DB338BDBEB3F2E7A7DDED1E7F2B9474F95FEDDD1D48B257E4602C78BF993859C5CE78AC7C7F3D931C97B22FD52AA3036E10D3F33E91D2941C38391489F3772178C46A6FBAAF8A1B882C7EB78B45F09F4AB8184526920560FF447944042C99095EA81434A462CAA6E4A7D496C488DA8C50612EABED44EE2836A44CD1954DF138B299121356720A6548B46D576E2F1C8F4B3D3BD7F03F5394213");

            // First NPC
            //Writer.WriteHex("0130020000789C9590314EC4301045FF4C3CD62E10A838C02AA20169450337E21620A70C08E5045BA44881B448BBDA2A250DD7A0E0081C00318E1D07509A7C4BA3D19F791E7BB072C750319353A117E18F1643F2DB27BED97D6C8FC6300592504B53B0E2D73A1089EC657C4FCD5F0F80B505C583C2F72B241012964C8C08F2F69336750638BCDD83E30DF9EB6D7BE7BDD2A481EA8D8DC9543C7526FAE599BA75340709A15F4BBF9C80BBA84054EFC3E509D1E1C6316701927FD03C75E7D82F622CCFD05C0C8576DCEDDC34833EF95B7FC338C513E191E03F2C96D9B05683AC224BE044D3EA50361697B8FA010922443E");
        }
    }
}