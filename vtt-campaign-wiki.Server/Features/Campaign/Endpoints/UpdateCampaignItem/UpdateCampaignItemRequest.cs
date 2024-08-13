using Microsoft.AspNetCore.Mvc;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.UpdateCampaignItem
{
    public class UpdateCampaignItemRequest
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ExternalLink { get; set; }
        public int? ImageId { get; set; }
        public IFormFile? Image { get; set; }
        public int? ParentEntityId { get; set; } = null;
        public int? AuthorId { get; set; }
    }
}
