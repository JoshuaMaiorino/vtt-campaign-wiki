using System.Linq.Expressions;
using System.Numerics;
using vtt_campaign_wiki.Server.Data;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Player.Services
{
    public class PlayerRepository : RepositoryBase<PlayerEntity>, IPlayerRepository
    {
        public PlayerRepository( VttCampaignWikiDbContext context ) : base( context )
        {
        }

        public override Task AddAsync( PlayerEntity entity )
        {
            throw new InvalidOperationException( "Players can only be added via the register endpoint." );
        }

        public override async Task<IEnumerable<PlayerEntity>> GetAllAsync( Expression<Func<PlayerEntity, bool>> filter = null )
        {
            var players = await base.GetAllAsync( filter );
            return players.Select( player => new PlayerEntity()
            {
                Id = player.Id,
                UserName = player.UserName,
                FirstName = player.FirstName,
                LastName = player.LastName
            } );
        }

        public override async Task<PlayerEntity> GetByIdAsync( int id )
        {
            var player = await base.GetByIdAsync( id );
            if( player == null)
            {
                return null;
            }

            return new PlayerEntity()
            {
                Id = player.Id,
                UserName = player.UserName,
                FirstName = player.FirstName,
                LastName = player.LastName
            };
        }

        protected override IQueryable<PlayerEntity> ApplySearch( IQueryable<PlayerEntity> query, string search )
        {
            return query.Where( p  => p.UserName == search );
        }
    }
}
