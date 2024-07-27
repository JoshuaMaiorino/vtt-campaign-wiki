using Microsoft.AspNetCore.Mvc;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.CreateCampaignItem
{
    public class CreateCampaignItemRequest
    {
        [FromForm]
        public string Title { get; set; }
        [FromForm]
        public string Content { get; set; }
        [FromForm]
        public string ExternalLink { get; set; }
        [FromForm]
        public IFormFile Image { get; set; }
        [FromForm]
        public int? ParentEntityId { get; set; }
    }
}
