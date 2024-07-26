using Microsoft.AspNetCore.Identity;

namespace vtt_campaign_wiki.Server.Features.Player.Endpoints.Register
{
    public static class RegisterEndpoint
    {
        public static void MapRegisterEndpoints( this IEndpointRouteBuilder endpoints )
        {
            endpoints.MapPost( "/register", async ( RegisterRequest registerRequest, UserManager<PlayerEntity> userManager ) =>
            {
                var user = new PlayerEntity
                {
                    UserName = registerRequest.Username,
                    Email = registerRequest.Email,
                    FirstName = registerRequest.FirstName,
                    LastName = registerRequest.LastName
                };

                var result = await userManager.CreateAsync( user, registerRequest.Password );

                if (result.Succeeded)
                {
                    return Results.Ok( new { Message = "User registered successfully" } );
                }
                else
                {
                    return Results.BadRequest( result.Errors );
                }
            } ).Produces<IResult>( StatusCodes.Status200OK )
              .Produces<IResult>( StatusCodes.Status400BadRequest );
        }
    }
}
