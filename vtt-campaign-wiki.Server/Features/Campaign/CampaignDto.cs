using vtt_campaign_wiki.Server.Features.Shared;

namespace vtt_campaign_wiki.Server.Features.Campaign
{
    public class CampaignDto : ItemBaseDto
    {
        public IEnumerable<CampaignPlayerDto> Players { get; set; } = new List<CampaignPlayerDto>();
    }
}