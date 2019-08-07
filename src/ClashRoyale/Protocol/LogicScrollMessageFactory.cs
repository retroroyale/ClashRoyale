using System;
using System.Collections.Generic;
using ClashRoyale.Protocol.Messages.Client;
using ClashRoyale.Protocol.Messages.Client.Alliance;
using ClashRoyale.Protocol.Messages.Client.Home;
using ClashRoyale.Protocol.Messages.Client.Login;
using ClashRoyale.Protocol.Messages.Client.Sector;
using ClashRoyale.Protocol.Messages.Client.Tournament;

namespace ClashRoyale.Protocol
{
    public class LogicScrollMessageFactory
    {
        public static Dictionary<int, Type> Messages;

        static LogicScrollMessageFactory()
        {
            Messages = new Dictionary<int, Type>
            {
                {10100, typeof(ClientHelloMessage)},
                {10101, typeof(LoginMessage)},
                {10107, typeof(ClientCapabilitiesMessage)},
                {10108, typeof(KeepAliveMessage)},
                //{10112, typeof(AuthenticationCheckMessage)},
                {10113, typeof(SetDeviceTokenMessage)},
                //{10116, typeof(ResetAccountMessage)},
                //{10117, typeof(ReportUserMessage)},
                //{10118, typeof(AccountSwitchedMessage)},
                //{10121, typeof(UnlockAccountMessage)},
                //{10150, typeof(AppleBillingRequestMessage)},
                //{10151, typeof(GoogleBillingRequestMessage)},
                //{10159, typeof(KunlunBillingRequestMessage)},
                {10212, typeof(ChangeAvatarNameMessage)},
                //{10512, typeof(AskForPlayingGamecenterFriendsMessage)},
                //{10513, typeof(AskForPlayingFacebookFriendsMessage)},
                {10905, typeof(InboxOpenedMessage)},
                {12903, typeof(RequestSectorStateMessage)},
                {12904, typeof(SectorCommandMessage)},
                //{12905, typeof(GetCurrentBattleReplayDataMessage)},
                {12951, typeof(SendBattleEventMessage)},
                {14101, typeof(GoHomeMessage)},
                {14102, typeof(EndClientTurnMessage)},
                {14104, typeof(StartMissionMessage)},
                //{14105, typeof(HomeLogicStoppedMessage)},
                {14107, typeof(CancelMatchmakeMessage)},
                {14113, typeof(VisitHomeMessage)},
                //{14114, typeof(HomeBattleReplayMessage)},
                //{14117, typeof(HomeBattleReplayViewedMessage)},
                {14120, typeof(AcceptChallengeMessage)},
                {14123, typeof(CancelChallengeMessage)},
                //{14201, typeof(BindFacebookAccountMessage)},
                //{14211, typeof(UnbindFacebookAccountMessage)},
                //{14212, typeof(BindGamecenterAccountMessage)},
                {14262, typeof(BindGoogleServiceAccountMessage)},

                {14301, typeof(CreateAllianceMessage)},
                {14302, typeof(AskForAllianceDataMessage)},
                {14303, typeof(AskForJoinableAlliancesListMessage)},
                {14304, typeof(AskForAllianceStreamMessage)},
                {14305, typeof(JoinAllianceMessage)},
                {14306, typeof(ChangeAllianceMemberRoleMessage)},
                //{14307, typeof(KickAllianceMemberMessage)},
                {14308, typeof(LeaveAllianceMessage)},
                //{14310, typeof(DonateAllianceUnitMessage)},
                {14315, typeof(ChatToAllianceStreamMessage)},
                {14316, typeof(ChangeAllianceSettingsMessage)},
                {14317, typeof(RequestJoinAllianceMessage)},
                //{14318, typeof(SelectSpellsFromCoOpenMessage)},
                //{14319, typeof(OfferChestForCoOpenMessage)},
                {14321, typeof(RespondToAllianceJoinRequestMessage)},
                {14322, typeof(SendAllianceInvitationMessage)},
                //{14323, typeof(JoinAllianceUsingInvitationMessage)},
                {14324, typeof(SearchAlliancesMessage)},
                //{14330, typeof(SendAllianceMailMessage)},

                {14401, typeof(AskForAllianceRankingListMessage)},
                {14402, typeof(AskForTvContentMessage)},
                {14403, typeof(AskForAvatarRankingListMessage)},
                {14404, typeof(AskForAvatarLocalRankingListMessage)},
                {14405, typeof(AskForAvatarStreamMessage)},
                {14406, typeof(AskForBattleReplayStreamMessage)},
                //{14408, typeof(AskForLastAvatarTournamentResultsMessage)},
                //{14418, typeof(RemoveAvatarStreamEntryMessage)},

                {14600, typeof(AvatarNameCheckRequestMessage)},

                //{16000, typeof(LogicDeviceLinkCodeRequestMessage)},
                //{16001, typeof(LogicDeviceLinkMenuClosedMessage)},
                //{16002, typeof(LogicDeviceLinkEnterCodeMessage)},
                //{16003, typeof(LogicDeviceLinkConfirmYesMessage)},

                {16103, typeof(AskForTournamentListMessage)}
            };
        }
    }
}