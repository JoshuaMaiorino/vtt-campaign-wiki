using Microsoft.AspNetCore.Mvc;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.CreateCampaign
{
    public class CreateCampaignRequest
    {
        [FromForm]
        public string Title { get; set; }
        [FromForm]
        public string Content { get; set; }
        [FromForm]
        public string ExternalLink { get; set; }
        [FromForm]
        public IFormFile Image { get; set; }
    }
}
