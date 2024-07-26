using Microsoft.AspNetCore.Identity;
using vtt_campaign_wiki.Server.Features.Player;
using vtt_campaign_wiki.Server.Features.Session;
using vtt_campaign_wiki.Server.Features.Shared;

namespace vtt_campaign_wiki.Server.Features.Campaign
{
    public class CampaignEntity : ItemBaseEntity
    {
        public IList<SessionEntity> Sessions { get; set; } = new List<SessionEntity>();
        public IList<CampaignPlayerEntity> Players { get; set; } = new List<CampaignPlayerEntity>();
        public IList<CampaignItemEntity> Items { get; set; } = new List<CampaignItemEntity>();
    }
}