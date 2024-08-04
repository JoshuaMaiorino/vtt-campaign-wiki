using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using vtt_campaign_wiki.Server.Data;
using vtt_campaign_wiki.Server.Features.Campaign.Utilities;
using vtt_campaign_wiki.Server.Features.Player.Services;
using vtt_campaign_wiki.Server.Features.Shared;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Services
{
    public class CampaignRepository : RepositoryBase<CampaignEntity>, ICampaignRepository
    {
        public CampaignRepository( VttCampaignWikiDbContext context ) : base( context )
        {
        }
        public override async Task AddAsync( CampaignEntity entity )
        {
            var currentPlayer = PlayerProvider.GetCurrentPlayer();

            if (!entity.Players?.Any(p => p.PlayerId == currentPlayer.Id ) ?? true )
            {
                if( entity.Players == null)
                {
                    entity.Players = new List<CampaignPlayerEntity>();
                }

                entity.Players.Add( new CampaignPlayerEntity()
                {
                    PlayerId = currentPlayer.Id,
                    Player = currentPlayer,
                    IsDM = true,
                } );
            }

            await base.AddAsync( entity );
        }
        public override async Task<IEnumerable<CampaignEntity>> GetAllAsync( Expression<Func<CampaignEntity, bool>> filter = null )
        {
            var query = _dbSet.PlayerCampaigns();
            var (items, length) = await base.GetAllAsync( query, null, filter );
            return items;
        }
        public override Task<(IEnumerable<CampaignEntity> Items, int ItemsLength)> GetAllAsync( PaginationParameter options, Expression<Func<CampaignEntity, bool>> filter = null )
        {
            return GetAllAsync( _dbSet.PlayerCampaigns(), options, filter );
        }
        public override Task<CampaignEntity> GetByIdAsync( int id )
        {
            return GetByIdAsync( id, 
                _dbSet
                .Include( c => c.Players )
                .ThenInclude( cp => cp.Player )
                .PlayerCampaigns() );
        }
        public override async Task UpdateAsync( CampaignEntity entity )
        {
            await base.UpdateAsync( entity, existingEntity =>
            {
                if (entity.Players != null)
                {
                    foreach (var campaignPlayer in entity.Players)
                    {
                        var exisitingPlayer = existingEntity.Players.FirstOrDefault( p => p.PlayerId == campaignPlayer.PlayerId );
                        if (exisitingPlayer == null)
                        {
                            existingEntity.Players.Add( new CampaignPlayerEntity()
                            {
                                CampaignId = existingEntity.Id,
                                PlayerId = campaignPlayer.PlayerId,
                                IsDM = campaignPlayer.IsDM
                            } );
                        }
                        else
                        {
                            exisitingPlayer.IsDM = campaignPlayer.IsDM;
                        }
                    }

                    var playersToRemove = existingEntity.Players.Where( ep => !entity.Players.Any( p => p.PlayerId == ep.PlayerId ) ).ToList();
                    foreach (var player in playersToRemove)
                    {
                        existingEntity.Players.Remove( player );
                    }
                }
            } );
        }
    }
}
