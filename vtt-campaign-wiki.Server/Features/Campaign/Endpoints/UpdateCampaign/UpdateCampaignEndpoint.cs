using vtt_campaign_wiki.Server.Features.Shared.Services;
using vtt_campaign_wiki.Server.Features.Image.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.UpdateCampaign
{
    public class UpdateCampaignEndpoint : Endpoint<UpdateCampaignRequest, CampaignDto>
    {
        private readonly IRepositoryBase<CampaignEntity> _campaignRepository;

        public UpdateCampaignEndpoint( IRepositoryBase<CampaignEntity> campaignRepository )
        {
            _campaignRepository = campaignRepository;
        }

        public override void Configure()
        {
            Put( "/campaigns/{id:int}" );
            AllowFileUploads();
        }

        public override async Task HandleAsync( UpdateCampaignRequest req, CancellationToken ct )
        {
            var id = Route<int>( "id" );
            
            if (id != req.Id)
            {
                AddError( "Invalid Request" );
                await SendErrorsAsync( 400, ct );
                return;
            }

            var campaignEntity = req.Adapt<CampaignEntity>();
            campaignEntity.Image = await ImageHelper.GetImageFromRequest( req.Image );

            await _campaignRepository.UpdateAsync( campaignEntity );

            // Re-fetch the item to get the updated ImageId
            var updatedItemFromDb = await _campaignRepository.GetByIdAsync( campaignEntity.Id );
            if (updatedItemFromDb == null)
            {
                AddError( "Item not found after update" );
                await SendErrorsAsync( 404, ct );
                return;
            }

            await SendOkAsync( updatedItemFromDb.Adapt<CampaignDto>(), ct );
        }
    }
}
