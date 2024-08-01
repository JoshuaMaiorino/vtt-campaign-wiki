using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace vtt_campaign_wiki.Server.Features.Player.Endpoints.Login
{
    public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
    {
        private readonly UserManager<PlayerEntity> _userManager;
        private readonly SignInManager<PlayerEntity> _signInManager;
        private readonly IConfiguration _configuration;

        public LoginEndpoint( UserManager<PlayerEntity> userManager, SignInManager<PlayerEntity> signInManager, IConfiguration configuration )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public override void Configure()
        {
            Post( "/api/login" );
            AllowAnonymous();
        }

        public override async Task HandleAsync( LoginRequest req, CancellationToken ct )
        {
            var user = await _userManager.FindByNameAsync( req.Username );
            if (user == null)
            {
                await SendNotFoundAsync( ct );
                return;
            }

            var result = await _signInManager.PasswordSignInAsync( user, req.Password, isPersistent: false, lockoutOnFailure: false );

            if (result.Succeeded)
            {
                var token = GenerateJwtToken( user );
                await SendOkAsync( new LoginResponse { Message = "Login successful", Token = token, UserName = user.UserName, UserId = user.Id }, ct );
            }
            else if (result.IsLockedOut)
            {
                AddError( "User account locked" );
                await SendErrorsAsync( StatusCodes.Status423Locked, ct );
            }
            else
            {
                AddError( "Invalid login attempt" );
                await SendErrorsAsync( 401, ct );
            }
        }

        private string GenerateJwtToken( PlayerEntity user )
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _configuration["Jwt:Key"] ) );
            var creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays( 30 ),
                signingCredentials: creds );

            return new JwtSecurityTokenHandler().WriteToken( token );
        }
    }
}
