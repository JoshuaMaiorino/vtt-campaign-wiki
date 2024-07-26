using Microsoft.AspNetCore.Identity;
using vtt_campaign_wiki.Server.Features.Player;

namespace vtt_campaign_wiki.Server.Features.Campaign
{
    public class CampaignPlayerEntity
    {
        public bool IsDM { get; set; } = false;
        public int CampaignId { get; set; }
        public int PlayerId { get; set; }

        public CampaignEntity? Campaign { get; set; } = null;
        public PlayerEntity? Player { get; set; } = null;
    }
}
