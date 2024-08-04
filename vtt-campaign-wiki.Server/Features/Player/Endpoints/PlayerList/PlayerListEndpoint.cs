using FastEndpoints;
using vtt_campaign_wiki.Server.Features.Player.Services;
using vtt_campaign_wiki.Server.Features.Shared;

namespace vtt_campaign_wiki.Server.Features.Player.Endpoints.PlayerList
{
    public class PlayerListEndpoint : EndpointWithoutRequest<IEnumerable<PlayerDto>>
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerListEndpoint( IPlayerRepository playerRepository )
        {
            _playerRepository = playerRepository;
        }

        public override void Configure()
        {
            Get( "/api/players" );
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            var options = new PaginationParameter()
            {
                Search = Query<string>( "search", false )
            };
            
            var (players, count ) = await _playerRepository.GetAllAsync( options );

            await SendOkAsync( players.Adapt<IEnumerable<PlayerDto>>(), ct );
        }
    }
}
