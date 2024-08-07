using vtt_campaign_wiki.Server.Features.Player.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Utilities
{
    public static class CampaignExtensions
    {
        public static readonly List<string> Items = new List<string>
    {
        "NPCs and Factions",
        "Locations",
        "Items",
        "Lore",
        "House Rules"
    };

        public static IQueryable<CampaignEntity> PlayerCampaigns( this IQueryable<CampaignEntity> campaigns )
        {
            var currentPlayer = PlayerProvider.GetCurrentPlayer();
            return campaigns.Where( c => c.Players.Any( p => p.PlayerId == currentPlayer.Id ) );
        }

        public static CampaignEntity AddStartingSections( this CampaignEntity campaign )
        {
                    
            for( int index=0; index < Items.Count(); index++ )
            {
                campaign.Items.Add( new CampaignItemEntity()
                {
                    Author = campaign.Author,
                    AuthorId = campaign.AuthorId,
                    Title = Items[index],
                    Position = ( index+1 ) * Shared.Constants.ItemBase.POSITION_GAP
                } );
            }
            
            return campaign;
        }
    }
}
