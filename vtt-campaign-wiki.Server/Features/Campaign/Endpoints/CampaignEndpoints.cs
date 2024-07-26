using Mapster;
using vtt_campaign_wiki.Server.Features.Shared.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using vtt_campaign_wiki.Server.Features.Image;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints
{
    public static class CampaignEndpoints
    {
        public static void MapCampaignEndpoints( this IEndpointRouteBuilder endpoints )
        {
            endpoints.MapGet( "/campaigns", async ( IRepositoryBase<CampaignEntity> campaignRepository ) =>
            {
                var campaigns = await campaignRepository.GetAllAsync();
                return Results.Ok( campaigns.Adapt<IEnumerable<CampaignDto>>() );
            } ).RequireAuthorization();

            endpoints.MapGet( "/campaigns/{id}", async ( int id, IRepositoryBase<CampaignEntity> campaignRepository ) =>
            {
                var campaign = await campaignRepository.GetByIdAsync( id );
                if (campaign == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok( campaign.Adapt<CampaignDto>() );
            } ).RequireAuthorization();

            endpoints.MapPost( "/campaigns", async ( HttpRequest request, [FromForm] CampaignDto campaign, [FromForm] IFormFile image, IRepositoryBase<CampaignEntity> campaignRepository ) =>
            {
                var campaignImage = await GetImageFromRequest( image );
                await campaignRepository.AddAsync( campaign.Adapt<CampaignEntity>() );
                return Results.Created( $"/campaigns/{campaign.Id}", campaign );
            } ).RequireAuthorization();

            endpoints.MapPut( "/campaigns/{id}", async ( int id, HttpRequest request, [FromForm] CampaignDto campaign, [FromForm] IFormFile image, IRepositoryBase<CampaignEntity> campaignRepository ) =>
            {
                if (id != campaign.Id)
                {
                    return Results.BadRequest();
                }

                var campaignImage = await GetImageFromRequest( image );
                await campaignRepository.UpdateAsync( campaign.Adapt<CampaignEntity>() );
                return Results.NoContent();
            } ).RequireAuthorization();

            endpoints.MapDelete( "/campaigns/{id}", async ( int id, IRepositoryBase<CampaignEntity> campaignRepository ) =>
            {
                var campaign = await campaignRepository.GetByIdAsync( id );
                if (campaign == null)
                {
                    return Results.NotFound();
                }

                await campaignRepository.DeleteAsync( id );
                return Results.NoContent();
            } ).RequireAuthorization();
        }

        private static async Task<ItemImageDto> GetImageFromRequest( IFormFile image )
        {
            if (image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync( memoryStream );
                    return new ItemImageDto
                    {
                        Name = image.FileName,
                        ContentType = image.ContentType,
                        Data = memoryStream.ToArray()
                    };
                }
            }
            return null;
        }
    }
}
