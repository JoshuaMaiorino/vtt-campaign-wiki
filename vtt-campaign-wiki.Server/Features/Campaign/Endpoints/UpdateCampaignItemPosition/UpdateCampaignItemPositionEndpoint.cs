using vtt_campaign_wiki.Server.Features.Campaign.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.UpdateCampaignItemPosition
{
    public class UpdateCampaignItemPositionEndpoint : Endpoint<UpdateCampaignItemPositionRequest, CampaignItemDto>
    {
        private readonly ICampaignItemRepository _campaignItemRepository;

        public UpdateCampaignItemPositionEndpoint( ICampaignItemRepository campaignItemRepository )
        {
            _campaignItemRepository = campaignItemRepository;
        }

        public override void Configure()
        {
            Post( "/api/campaigns/{campaignId:int}/items/{itemId:int}/Position" );
        }

        public override async Task HandleAsync(UpdateCampaignItemPositionRequest req, CancellationToken ct )
        {
            var campaignId = Route<int>( "campaignId" );
            var itemId = Route<int>( "itemId" );

            if (itemId != req.ItemId || campaignId != req.CampaignId)
            {
                AddError( "Invalid Request" );
                await SendErrorsAsync( 400, ct );
                return;
            }

            var updatedEntity = await _campaignItemRepository.UpdatePositionAndParentAsync( itemId, req.ParentId, req.PriorPosition, req.NextPosition );

            await SendOkAsync( updatedEntity.Adapt<CampaignItemDto>(), ct );
        }
    }
}
