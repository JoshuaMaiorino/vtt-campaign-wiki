using vtt_campaign_wiki.Server.Features.Player.Services;

namespace vtt_campaign_wiki.Server.Features.Player.Endpoints.Player
{
    public static class PlayerEndpoints
    {
        public static void MapPlayerEndpoints( this IEndpointRouteBuilder endpoints )
        {
            endpoints
                .MapGet( "/players", async ( IPlayerRepository playerRepository ) =>
                {
                    var players = await playerRepository.GetAllAsync();

                    return Results.Ok( players.Select( player => new PlayerDto()
                    {
                        Id = player.Id,
                        UserName = player.UserName,
                        FirstName = player.FirstName,
                        LastName = player.LastName
                    } ) );
                } )
                .RequireAuthorization();

            endpoints
                .MapGet( "/players/{id}", async ( int id, IPlayerRepository playerRepository ) =>
                {
                    var player = await playerRepository.GetByIdAsync( id );
                    if (player == null)
                    {
                        return Results.NotFound();
                    }

                    var playerDto = new PlayerDto
                    {
                        Id = player.Id,
                        UserName = player.UserName,
                        FirstName = player.FirstName,
                        LastName = player.LastName
                    };

                    return Results.Ok( playerDto );
                } )
                .RequireAuthorization();
        }
    }
}
