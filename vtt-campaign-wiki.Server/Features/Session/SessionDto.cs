using vtt_campaign_wiki.Server.Features.Shared;

namespace vtt_campaign_wiki.Server.Features.Session
{
    public class SessionDto : ItemBaseDto
    {
        public int Number { get; set; } = 0;
        public DateOnly Date { get; set; } = new DateOnly();
        public int CampaignId { get; set; }
    }
}
