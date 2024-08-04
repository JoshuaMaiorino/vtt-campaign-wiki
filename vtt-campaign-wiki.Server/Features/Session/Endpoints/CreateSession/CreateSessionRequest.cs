using Microsoft.AspNetCore.Mvc;

namespace vtt_campaign_wiki.Server.Features.Session.Endpoints.CreateSession
{
    public class CreateSessionRequest
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
        public int Number { get; set; }
        [FromForm]
        public DateOnly? Date { get; set; }
        [FromForm]
        public int CampaignId { get; set; }
    }
}
