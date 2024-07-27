using FastEndpoints;
using vtt_campaign_wiki.Server.Features.Player.Services;

namespace vtt_campaign_wiki.Server.Features.Player.Endpoints.PlayerById
{
    public class PlayerByIdEndpoint : EndpointWithoutRequest<PlayerDto>
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerByIdEndpoint( IPlayerRepository playerRepository )
        {
            _playerRepository = playerRepository;
        }

        public override void Configure()
        {
            Get( "/players/{id:int}" );
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            int id = Route<int>( "id" );

            var player = await _playerRepository.GetByIdAsync( id );
            if (player == null)
            {
                await SendNotFoundAsync( ct );
                return;
            }

            var playerDto = new PlayerDto
            {
                Id = player.Id,
                UserName = player.UserName,
                FirstName = player.FirstName,
                LastName = player.LastName
            };

            await SendOkAsync( playerDto, ct );
        }
    }
}
