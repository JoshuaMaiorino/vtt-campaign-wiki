using Microsoft.AspNetCore.Identity;
using vtt_campaign_wiki.Server.Features.Image;
using vtt_campaign_wiki.Server.Features.Player;

namespace vtt_campaign_wiki.Server.Features.Shared
{
    public class ItemBaseEntity : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ExternalLink { get; set; } = string.Empty;
        public int? ImageId { get; set; } = null;
        public int? AuthorId { get; set; } = null;
        public decimal Position { get; set; }
        public PlayerEntity? Author { get; set; }
        public ItemImageEntity? Image { get; set; }
    }
}
