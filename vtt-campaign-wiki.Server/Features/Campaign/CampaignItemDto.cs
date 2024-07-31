using vtt_campaign_wiki.Server.Features.Shared;

namespace vtt_campaign_wiki.Server.Features.Campaign
{
    public class CampaignItemDto : ItemBaseDto
    {
        public int CampaignId { get; set; }
        public int? ParentEntityId { get; set; }
        public List<CampaignItemDto> Children { get; set; }
    }
}
