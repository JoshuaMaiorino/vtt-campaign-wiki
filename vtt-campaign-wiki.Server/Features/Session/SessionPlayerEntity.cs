using Microsoft.AspNetCore.Identity;
using vtt_campaign_wiki.Server.Features.Player;

namespace vtt_campaign_wiki.Server.Features.Session
{
    public class SessionPlayerEntity
    {
        public int SessionId { get; set; }
        public int PlayerId { get; set; }
        public string Notes { get; set; } = string.Empty;

        public SessionEntity? Session { get; set; } = null;
        public PlayerEntity? Player { get; set; } = null;
    }
}
