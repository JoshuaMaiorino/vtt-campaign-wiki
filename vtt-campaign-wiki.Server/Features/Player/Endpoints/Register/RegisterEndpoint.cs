using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace vtt_campaign_wiki.Server.Features.Player.Endpoints.Register
{
    public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
    {
        private readonly UserManager<PlayerEntity> _userManager;

        public RegisterEndpoint( UserManager<PlayerEntity> userManager )
        {
            _userManager = userManager;
        }

        public override void Configure()
        {
            Post( "/register" );
            AllowAnonymous();
        }

        public override async Task HandleAsync( RegisterRequest req, CancellationToken ct )
        {
            var user = new PlayerEntity
            {
                UserName = req.Username,
                Email = req.Email,
                FirstName = req.FirstName,
                LastName = req.LastName
            };

            var result = await _userManager.CreateAsync( user, req.Password );

            if (result.Succeeded)
            {
                await SendOkAsync( new RegisterResponse { Message = "User registered successfully" }, ct );
            }
            else
            {
                foreach( var error in result.Errors)
                {
                    AddError( error.Description );
                }
                await SendErrorsAsync( 400, ct );
            }
        }
    }
}
