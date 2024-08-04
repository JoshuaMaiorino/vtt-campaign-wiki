using vtt_campaign_wiki.Server.Features.Player;

namespace vtt_campaign_wiki.Server.Features.Campaign
{
    public class CampaignPlayerDto
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int CampaignId { get; set; }
        public bool IsDM { get; set; }
        public PlayerDto Player { get; set; }

    }
}
