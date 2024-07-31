using Microsoft.AspNetCore.Mvc;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.UpdateCampaignItem
{
    public class UpdateCampaignItemRequest
    {
        [FromForm]
        public int Id { get; set; }
        [FromForm]
        public int CampaignId { get; set; }
        [FromForm]
        public string Title { get; set; }
        [FromForm]
        public string Content { get; set; }
        [FromForm]
        public string ExternalLink { get; set; }
        [FromForm]
        public int? ImageId { get; set; }
        [FromForm]
        public IFormFile Image { get; set; }
        [FromForm]
        public int? ParentEntityId { get; set; } = null;
        public int? AuthorId { get; set; }
    }
}
