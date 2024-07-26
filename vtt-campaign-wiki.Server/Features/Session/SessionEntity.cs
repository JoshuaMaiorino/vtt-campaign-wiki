using vtt_campaign_wiki.Server.Features.Shared;
using vtt_campaign_wiki.Server.Features.Campaign;

namespace vtt_campaign_wiki.Server.Features.Session
{
    public class SessionEntity : ItemBaseEntity
    {
        public int Number { get; set; } = 0;
        public DateOnly Date { get; set; } = new DateOnly();
        public int CampaignId { get; set; }

        public CampaignEntity? Campaign { get; set; } = null;
        public IList<SessionPlayerEntity> Players { get; set; } = new List<SessionPlayerEntity>();
    }
}
