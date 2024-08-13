using Microsoft.EntityFrameworkCore;
using vtt_campaign_wiki.Server.Data;
using vtt_campaign_wiki.Server.Features.Campaign.Utilities;
using vtt_campaign_wiki.Server.Features.Shared;
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

            var campaign = await _context.Campaigns.FindAsync( entity.CampaignId );

            if( campaign == null)
            {
                throw new ArgumentException( "Campaign Doesn't Exisit" );
            }

            if( !campaign.IsDm())
            {
                throw new UnauthorizedAccessException( "Only a DM can add items to a campaign" );
            }

            if (entity.Position == 0)
            {
                double maxPosition = await _dbSet
                    .Where( e => e.ParentEntityId == entity.ParentEntityId )
                    .MaxAsync( e => (double?) e.Position ) ?? 0;

                entity.Position = (decimal)maxPosition + Shared.Constants.ItemBase.POSITION_GAP;
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
            if( !entity.IsDim())
            {
                throw new UnauthorizedAccessException( "Only a DM can edit content" );
            }
            entity.CampaignId = campaignId;

            if( entity.Campaign != null && entity.Campaign.Id != campaignId)
            {
                entity.Campaign = null;
            }

            await AddAsync( entity );
        }

        public async Task<IEnumerable<CampaignItemEntity>> GetAllAsync( int campaignId )
        {
            var root =  await base.GetAllAsync( i => i.CampaignId == campaignId );

            root = root.OrderBy( e => (double) e.Position );

            if( root.Any())
            {
                foreach( var item in root)
                {
                    await LoadChildrenRecursively( item );
                }
            }

            return root;
        }

        public async Task<IEnumerable<CampaignItemEntity>> GetChildrenAsync( int campaignItemId )
        {
            return await base.GetAllAsync( i => i.ParentEntityId.HasValue && i.ParentEntityId.Value == campaignItemId );
        }

        public override async Task<CampaignItemEntity> GetByIdAsync( int id )
        {
            var root = await _dbSet
                .Include( x => x.Children )
                .FirstOrDefaultAsync( i => i.Id == id );

            if (root != null)
            {
                root.Children.OrderBy( c => (double) c.Position );
                await LoadChildrenRecursively( root );
            }

            return root;
        }

        public async Task<CampaignItemEntity> UpdatePositionAndParentAsync( int itemId, int? newParentId, decimal? priorPosition, decimal? nextPosition )
        {
            // Find the entity by itemId
            var entity = await _dbSet.FirstOrDefaultAsync( e => e.Id == itemId );
            if (entity == null)
            {
                throw new ArgumentException( "Item not found." );
            }

            // Update the parentId
            entity.ParentEntityId = newParentId;

            // Calculate the new position
            if (priorPosition.HasValue && nextPosition.HasValue)
            {
                entity.Position = (priorPosition.Value + nextPosition.Value) / 2;
            }
            else if (priorPosition.HasValue)
            {
                entity.Position = priorPosition.Value / 2;
            }
            else if (nextPosition.HasValue)
            {
                entity.Position = nextPosition.Value / 2;
            }
            else
            {
                entity.Position = Shared.Constants.ItemBase.POSITION_GAP;
            }

            if ( entity.Position < Shared.Constants.ItemBase.POSITION_THRESHOLD )
            {
                // Re-spread position values for all items under the same parent
                await RespreadPositionValuesAsync( newParentId );
            }
            else
            {
                // Update the entity in the database
                _dbSet.Update( entity );
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        private async Task RespreadPositionValuesAsync( int? parentId )
        {
            var items = await _dbSet
                .Where( e => e.ParentEntityId == parentId )
                .OrderBy( e => e.Position )
                .ToListAsync();

            decimal position = Shared.Constants.ItemBase.POSITION_GAP;

            foreach (var item in items)
            {
                item.Position = position;
                position += Shared.Constants.ItemBase.POSITION_GAP;
            }

            _dbSet.UpdateRange( items );
            await _context.SaveChangesAsync();
        }

    }
}
