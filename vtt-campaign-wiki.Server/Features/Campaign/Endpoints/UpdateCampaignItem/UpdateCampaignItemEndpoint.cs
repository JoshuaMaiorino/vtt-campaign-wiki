using vtt_campaign_wiki.Server.Features.Campaign.Services;
using vtt_campaign_wiki.Server.Features.Image.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.UpdateCampaignItem
{
    public class UpdateCampaignItemEndpoint : Endpoint<UpdateCampaignItemRequest, CampaignItemDto>
    {
        private readonly ICampaignItemRepository _repository;

        public UpdateCampaignItemEndpoint( ICampaignItemRepository repository )
        {
            _repository = repository;
        }

        public override void Configure()
        {
            Put( "/campaigns/{campaignId:int}/items/{itemId:int}" );
            AllowFileUploads();
        }

        public override async Task HandleAsync( UpdateCampaignItemRequest req, CancellationToken ct )
        {
            var campaignId = Route<int>( "campaignId" );
            var itemId = Route<int>( "itemId" );

            if (itemId != req.Id || campaignId != req.CampaignId )
            {
                AddError( "Invalid Request" );
                await SendErrorsAsync( 400, ct );
                return;
            }

            var updatedItem = req.Adapt<CampaignItemEntity>();
            updatedItem.Image = await ImageHelper.GetImageFromRequest( req.Image );

            if (req.ParentEntityId == 0)
            {
                updatedItem.ParentEntityId = null;
            }

            await _repository.UpdateAsync( updatedItem );
            await SendNoContentAsync( ct );
        }
    }
}
