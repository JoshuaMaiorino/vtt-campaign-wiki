using Microsoft.AspNetCore.Identity;
using vtt_campaign_wiki.Server.Features.Campaign;
using vtt_campaign_wiki.Server.Features.Session;
using vtt_campaign_wiki.Server.Features.Shared;

namespace vtt_campaign_wiki.Server.Features.Player
{
    public class PlayerEntity : IdentityUser<int>, IEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public IList<CampaignPlayerEntity> Campaigns { get; set; } = new List<CampaignPlayerEntity>();
        public IList<SessionPlayerEntity> Sessions { get; set; } = new List<SessionPlayerEntity>();
        public IList<ItemBaseEntity> Items { get; set; } = new List<ItemBaseEntity>();
    }
}
