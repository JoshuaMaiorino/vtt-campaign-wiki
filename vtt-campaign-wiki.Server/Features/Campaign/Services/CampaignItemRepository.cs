using Microsoft.EntityFrameworkCore;
using vtt_campaign_wiki.Server.Data;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Services
{
    public class CampaignItemRepository : RepositoryBase<CampaignItemEntity>, ICampaignItemRepository
    {
        public CampaignItemRepository( VttCampaignWikiDbContext context ) : base( context )
        {
        }

        public override async Task AddAsync( CampaignItemEntity entity )
        {
            if( entity.CampaignId == 0 )
            {
                throw new ArgumentException( "CampaignId must be provided for a CampaignItemEntity." );
            }

            await base.AddAsync( entity );
        }

        public override async Task UpdateAsync( CampaignItemEntity entity )
        {
            if (entity.CampaignId == 0)
            {
                throw new ArgumentException( "CampaignId must be provided for a CampaignItemEntity." );
            }

            await base.UpdateAsync( entity );
        }

        public async Task AddAsync( int campaignId, CampaignItemEntity entity )
        {
            entity.CampaignId = campaignId;

            if( entity.Campaign != null && entity.Campaign.Id != campaignId)
            {
                entity.Campaign = null;
            }

            await AddAsync( entity );
        }

        public async Task<IEnumerable<CampaignItemEntity>> GetAllAsync( int campaignId )
        {
            return await base.GetAllAsync( i => i.CampaignId == campaignId );
        }

        public async Task<IEnumerable<CampaignItemEntity>> GetChildrenAsync( int campaignItemId )
        {
            return await base.GetAllAsync( i => i.ParentEntityId.HasValue && i.ParentEntityId.Value == campaignItemId );
        }
    }
}
