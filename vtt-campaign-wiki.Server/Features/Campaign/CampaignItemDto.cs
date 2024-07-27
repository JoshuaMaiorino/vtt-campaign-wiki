namespace vtt_campaign_wiki.Server.Features.Campaign
{
    public class CampaignItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ExternalLink { get; set; } = string.Empty;
        public int? ImageId { get; set; }
        public int? AuthorId { get; set; }
        public int CampaignId { get; set; }
        public int? ParentEntityId { get; set; }
    }
}
