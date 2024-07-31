namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.UpdateCampaignItemPosition
{
    public class UpdateCampaignItemPositionRequest
    {
        public int ItemId { get; set; }
        public int CampaignId { get; set; }
        public int? ParentId { get; set; }
        public decimal? PriorPosition { get; set; }
        public decimal? NextPosition { get; set; }
    }
}
