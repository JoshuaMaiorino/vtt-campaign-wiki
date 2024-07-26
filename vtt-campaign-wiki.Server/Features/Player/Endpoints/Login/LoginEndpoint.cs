using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace vtt_campaign_wiki.Server.Features.Player.Endpoints.Login
{
    public static class LoginEndpoint
    {
        public static void MapLoginEndpoints( this IEndpointRouteBuilder endpoints )
        {
            endpoints.MapPost( "/login", async ( LoginRequest loginRequest, UserManager<PlayerEntity> userManager, SignInManager<PlayerEntity> signInManager, IConfiguration configuration ) =>
            {
                var user = await userManager.FindByNameAsync( loginRequest.Username );
                if (user == null)
                {
                    return Results.NotFound( new { Message = "User not found" } );
                }

                var result = await signInManager.PasswordSignInAsync( user, loginRequest.Password, isPersistent: false, lockoutOnFailure: false );

                if (result.Succeeded)
                {
                    var token = GenerateJwtToken( user, configuration );
                    return Results.Ok( new { Message = "Login successful", Token = token } );
                }
                else if (result.IsLockedOut)
                {
                    return Results.Json( new { Message = "User account locked" }, statusCode: StatusCodes.Status423Locked );
                }
                else
                {
                    return Results.Json( new { Message = "Invalid login attempt" }, statusCode: StatusCodes.Status401Unauthorized );
                }
            } ).Produces<IResult>( StatusCodes.Status200OK )
              .Produces<IResult>( StatusCodes.Status404NotFound )
              .Produces<IResult>( StatusCodes.Status401Unauthorized )
              .Produces<IResult>( StatusCodes.Status423Locked );
        }

        private static string GenerateJwtToken( PlayerEntity user, IConfiguration configuration )
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( configuration["Jwt:Key"] ) );
            var creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes( 30 ),
                signingCredentials: creds );

            return new JwtSecurityTokenHandler().WriteToken( token );
        }
    }
}
