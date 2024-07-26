using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using vtt_campaign_wiki.Server.Data;
using vtt_campaign_wiki.Server.Features.Player;
using vtt_campaign_wiki.Server.Features.Player.Services;
using vtt_campaign_wiki.Server.Features.Shared.Services;
using vtt_campaign_wiki.Server.Lib;

var builder = WebApplication.CreateBuilder( args );

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options =>
{
    options.AddSecurityDefinition( "Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    } );

    options.AddSecurityRequirement( new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    } );
} );

SQLitePCL.Batteries.Init();

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

// Register the repository services
builder.Services.AddScoped( typeof( IRepositoryBase<> ), typeof( RepositoryBase<> ) );
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UsePlayerProvider();

app.MapAppEndpoints();

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
