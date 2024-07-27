global using FastEndpoints;
global using FastEndpoints.Swagger;
global using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using vtt_campaign_wiki.Server.Data;
using vtt_campaign_wiki.Server.Features.Player.Services;
using vtt_campaign_wiki.Server.Features.Player;
using vtt_campaign_wiki.Server.Features.Shared.Services;
using vtt_campaign_wiki.Server.Lib;
using System.Security.Claims;
using vtt_campaign_wiki.Server.Features.Campaign.Services;

var builder = WebApplication.CreateBuilder( args );

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

builder.Services.AddDbContext<VttCampaignWikiDbContext>( options =>
{
    options.UseSqlite( "Data Source=Data/VttCampaignWiki.db" );
} );
builder.Services.AddIdentity<PlayerEntity, IdentityRole<int>>()
    .AddEntityFrameworkStores<VttCampaignWikiDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection( "Jwt" );
var key = Encoding.ASCII.GetBytes( jwtSettings["Key"] );

builder.Services.AddAuthentication( options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
} )
.AddJwtBearer( options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey( key ),
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };
} );

builder.Services.AddAuthorization();

builder.Services.AddScoped( typeof( IRepositoryBase<> ), typeof( RepositoryBase<> ) );
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ICampaignItemRepository, CampaignItemRepository>();

// Add CORS policy
builder.Services.AddCors( options =>
{
    options.AddPolicy( "AllowSpecificOrigin",
        builder => builder.WithOrigins( "https://localhost:5173" ) // Add your frontend URL here
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials() );
} );

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Enable CORS
app.UseCors( "AllowSpecificOrigin" );

app.UsePlayerProvider();

app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.MapFallbackToFile( "/index.html" );

// Seed the database with initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<VttCampaignWikiDbContext>();
        context.Database.Migrate(); // Ensure the database is created and migrated
        await DbInitializer.SeedUsersAsync( services ); // Seed the initial user
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError( ex, "An error occurred seeding the database." );
    }
}

app.Run();
