using vtt_campaign_wiki.Server.Features.Campaign.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.DeleteCampaignItem
{
    public class DeleteCampaignItemEndpoint : EndpointWithoutRequest
    {
        private readonly ICampaignItemRepository _repository;

        public DeleteCampaignItemEndpoint( ICampaignItemRepository repository )
        {
            _repository = repository;
        }

        public override void Configure()
        {
            Delete( "/api/campaigns/{campaignId:int}/items/{itemId:int}" );
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            var campaignId = Route<int>( "campaignId" );
            var itemId = Route<int>( "itemId" );

            var existingItem = await _repository.GetByIdAsync( itemId );

            if (existingItem == null || existingItem.CampaignId != campaignId)
            {
                await SendNotFoundAsync( ct );
                return;
            }

            await _repository.DeleteAsync( itemId );
            await SendNoContentAsync( ct );
        }
    }
}
