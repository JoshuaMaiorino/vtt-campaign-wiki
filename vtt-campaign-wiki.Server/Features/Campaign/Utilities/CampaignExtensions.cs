using vtt_campaign_wiki.Server.Features.Player.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Utilities
{
    public static class CampaignExtensions
    {
        public static IQueryable<CampaignEntity> PlayerCampaigns( this IQueryable<CampaignEntity> campaigns )
        {
            var currentPlayer = PlayerProvider.GetCurrentPlayer();
            return campaigns.Where( c => c.Players.Any( p => p.PlayerId == currentPlayer.Id ) );
        }
    }
}
