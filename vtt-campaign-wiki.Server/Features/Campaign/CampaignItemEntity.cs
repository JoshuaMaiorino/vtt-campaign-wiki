using vtt_campaign_wiki.Server.Features.Shared;

namespace vtt_campaign_wiki.Server.Features.Campaign
{
    public class CampaignItemEntity : ItemBaseEntity
    {
        public int CampaignId { get; set; }
        public int? ParentEntityId { get; set; } = null;

        public CampaignEntity? Campaign { get; set; } = null;
        public CampaignItemEntity? ParentEntity { get; set; } = null;
        public IList<CampaignItemEntity>? Children { get; set; } = null;
    }
}
