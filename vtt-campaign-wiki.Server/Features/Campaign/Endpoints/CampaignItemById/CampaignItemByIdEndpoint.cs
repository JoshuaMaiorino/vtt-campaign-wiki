using vtt_campaign_wiki.Server.Features.Campaign.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.CampaignItemById
{
    public class GetCampaignItemByIdEndpoint : EndpointWithoutRequest<CampaignItemDto>
    {
        private readonly ICampaignItemRepository _repository;

        public GetCampaignItemByIdEndpoint( ICampaignItemRepository repository )
        {
            _repository = repository;
        }

        public override void Configure()
        {
            Get( "/campaigns/{campaignId:int}/items/{itemId:int}" );
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            var campaignId = Route<int>( "campaignId" );
            var itemId = Route<int>( "itemId" );

            var campaignItem = await _repository.GetByIdAsync( itemId );

            if (campaignItem == null || campaignItem.CampaignId != campaignId)
            {
                await SendNotFoundAsync( ct );
                return;
            }

            var result = campaignItem.Adapt<CampaignItemDto>();

            await SendAsync( result, cancellation: ct );
        }
    }
}
