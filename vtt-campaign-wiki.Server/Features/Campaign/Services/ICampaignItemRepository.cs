using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Services
{
    public interface ICampaignItemRepository : IRepositoryBase<CampaignItemEntity>
    {
        Task AddAsync( int campaignId, CampaignItemEntity entity );
        Task<IEnumerable<CampaignItemEntity>> GetAllAsync( int campaignId );
        Task<IEnumerable<CampaignItemEntity>> GetChildrenAsync( int campaignItemId );
    }
}
