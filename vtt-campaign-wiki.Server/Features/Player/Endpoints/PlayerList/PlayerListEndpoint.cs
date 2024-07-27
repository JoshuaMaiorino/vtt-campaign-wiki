using FastEndpoints;
using vtt_campaign_wiki.Server.Features.Player.Services;

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
            Get( "/players" );
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            var players = await _playerRepository.GetAllAsync();
            var playerDtos = players.Select( player => new PlayerDto
            {
                Id = player.Id,
                UserName = player.UserName,
                FirstName = player.FirstName,
                LastName = player.LastName
            } );

            await SendOkAsync( playerDtos, ct );
        }
    }
}
