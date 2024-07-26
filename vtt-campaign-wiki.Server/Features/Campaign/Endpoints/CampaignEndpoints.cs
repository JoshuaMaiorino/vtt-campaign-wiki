using Mapster;
using vtt_campaign_wiki.Server.Features.Shared.Services;

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

            endpoints.MapPost( "/campaigns", async ( CampaignDto campaign, IRepositoryBase<CampaignEntity> campaignRepository ) =>
            {

                await campaignRepository.AddAsync( campaign.Adapt<CampaignEntity>() );

                return Results.Created( $"/campaigns/{campaign.Id}", campaign );
            } ).RequireAuthorization();

            endpoints.MapPut( "/campaigns/{id}", async ( int id, CampaignDto campaign, IRepositoryBase<CampaignEntity> campaignRepository ) =>
            {

                if( id != campaign.Id)
                {
                    return Results.BadRequest();
                }

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
    }
}
