using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using vtt_campaign_wiki.Server.Features.Campaign;
using vtt_campaign_wiki.Server.Features.Image;
using vtt_campaign_wiki.Server.Features.Player;
using vtt_campaign_wiki.Server.Features.Session;
using vtt_campaign_wiki.Server.Features.Shared;

namespace vtt_campaign_wiki.Server.Data
{
    public class VttCampaignWikiDbContext : IdentityDbContext<
        PlayerEntity,
        IdentityRole<int>,
        int,
        IdentityUserClaim<int>,
        IdentityUserRole<int>,
        IdentityUserLogin<int>,
        IdentityRoleClaim<int>,
        IdentityUserToken<int>>
    {
        public VttCampaignWikiDbContext( DbContextOptions<VttCampaignWikiDbContext> options )
            : base( options )
        {
        }

        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<CampaignEntity> Campaigns { get; set; }
        public DbSet<CampaignItemEntity> CampaignItems { get; set; }
        public DbSet<ItemImageEntity> Images { get; set; }
        public DbSet<SessionEntity> Sessions { get; set; }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            base.OnModelCreating( modelBuilder );

            // Configure the PlayerEntity primary key
            modelBuilder.Entity<PlayerEntity>()
                .HasKey( p => p.Id );

            // Configure EntityBase and its relationships
            modelBuilder.Entity<ItemBaseEntity>()
                .HasOne( e => e.Image )
                .WithOne( i => i.Entity )
                .HasForeignKey<ItemBaseEntity>( i => i.ImageId );

            // Configure CampaignEntity and its relationships
            modelBuilder.Entity<CampaignEntity>()
                .HasMany( c => c.Items )
                .WithOne( i => i.Campaign )
                .HasForeignKey( i => i.CampaignId )
                .OnDelete( DeleteBehavior.Cascade );

            modelBuilder.Entity<CampaignEntity>()
                .HasMany( c => c.Sessions )
                .WithOne( s => s.Campaign )
                .HasForeignKey( s => s.CampaignId )
                .OnDelete( DeleteBehavior.Cascade );

            // Configure PlayerEntity and its relationships
            modelBuilder.Entity<PlayerEntity>()
                .HasMany( p => p.Sessions )
                .WithOne( sp => sp.Player )
                .HasForeignKey( sp => sp.PlayerId )
                .OnDelete( DeleteBehavior.Cascade );

            modelBuilder.Entity<PlayerEntity>()
                .HasMany( p => p.Items )
                .WithOne( i => i.Author )
                .HasForeignKey( i => i.AuthorId )
                .OnDelete( DeleteBehavior.Cascade );

            // Configure CampaignPlayerEntity and its relationships
            modelBuilder.Entity<CampaignPlayerEntity>()
                .HasKey( cp => new { cp.CampaignId, cp.PlayerId } );

            modelBuilder.Entity<CampaignPlayerEntity>()
                .HasOne( cp => cp.Campaign )
                .WithMany( c => c.Players )
                .HasForeignKey( cp => cp.CampaignId )
                .OnDelete( DeleteBehavior.Cascade );

            modelBuilder.Entity<CampaignPlayerEntity>()
                .HasOne( cp => cp.Player )
                .WithMany( p => p.Campaigns )
                .HasForeignKey( cp => cp.PlayerId )
                .OnDelete( DeleteBehavior.Cascade );

            // Configure CampaignItemEntity and its relationships
            modelBuilder.Entity<CampaignItemEntity>()
                .HasMany( i => i.Children )
                .WithOne( ci => ci.ParentEntity )
                .HasForeignKey( ci => ci.ParentEntityId )
                .OnDelete( DeleteBehavior.Cascade );

            // Configure SessionEntity and its relationships
            modelBuilder.Entity<SessionEntity>()
                .HasMany( s => s.Players )
                .WithOne( sp => sp.Session )
                .HasForeignKey( sp => sp.SessionId )
                .OnDelete( DeleteBehavior.Cascade );

            // Configure SessionEntity and its relationships
            modelBuilder.Entity<SessionPlayerEntity>()
               .HasKey( sp => new { sp.SessionId, sp.PlayerId } );

            modelBuilder.Entity<SessionPlayerEntity>()
                .HasOne( sp => sp.Session )
                .WithMany( c => c.Players )
                .HasForeignKey( sp => sp.SessionId )
                .OnDelete( DeleteBehavior.Cascade );

            modelBuilder.Entity<SessionPlayerEntity>()
                .HasOne( sp => sp.Player )
                .WithMany( p => p.Sessions )
                .HasForeignKey( sp => sp.PlayerId )
                .OnDelete( DeleteBehavior.Cascade );
        }

        public class VttCampaignWikiDbContextFactory : IDesignTimeDbContextFactory<VttCampaignWikiDbContext>
        {
            public VttCampaignWikiDbContext CreateDbContext( string[] args )
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath( Directory.GetCurrentDirectory() )
                    .AddJsonFile( "appsettings.json" )
                    .Build();

                var connectionString = configuration.GetConnectionString( "VttCampaignWikiDbContext" );

                var builder = new DbContextOptionsBuilder<VttCampaignWikiDbContext>();
                builder.UseSqlite( connectionString );

                return new VttCampaignWikiDbContext( builder.Options );
            }
        }
    }
}
