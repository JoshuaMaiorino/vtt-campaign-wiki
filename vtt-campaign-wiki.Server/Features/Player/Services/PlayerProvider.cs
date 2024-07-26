using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace vtt_campaign_wiki.Server.Features.Player.Services
{
    public class PlayerProvider
    {
        private readonly RequestDelegate _next;
        private static readonly AsyncLocal<PlayerEntity> _currentPlayer = new AsyncLocal<PlayerEntity>();

        public PlayerProvider( RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync( HttpContext context )
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userManager = context.RequestServices.GetRequiredService<UserManager<PlayerEntity>>();

                PlayerEntity user = null;
                var userIdClaim = context.User.FindFirst( ClaimTypes.NameIdentifier );
                if (userIdClaim != null && int.TryParse( userIdClaim.Value, out var userId ))
                {
                    user = await userManager.FindByIdAsync( userId.ToString() );
                }
                else
                {
                    var usernameClaim = context.User.FindFirst( ClaimTypes.Name );
                    if (usernameClaim != null)
                    {
                        user = await userManager.FindByNameAsync( usernameClaim.Value );
                    }
                }

                if (user != null)
                {
                    _currentPlayer.Value = user;
                }
            }
            await _next( context );
        }

        public static PlayerEntity GetCurrentPlayer()
        {
            return _currentPlayer.Value;
        }
    }
}
