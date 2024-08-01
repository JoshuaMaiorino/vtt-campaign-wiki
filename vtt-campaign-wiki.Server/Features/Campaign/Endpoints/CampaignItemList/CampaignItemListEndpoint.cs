using vtt_campaign_wiki.Server.Features.Campaign.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.CampaignItemList
{
    public class CampaignItemListEndpoint : EndpointWithoutRequest<List<CampaignItemDto>>
    {
        private readonly ICampaignItemRepository _repository;

        public CampaignItemListEndpoint( ICampaignItemRepository repository )
        {
            _repository = repository;
        }

        public override void Configure()
        {
            Get( "/api/campaigns/{campaignId:int}/items" );
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            var campaignId = Route<int>( "campaignId" );
            
            var campaignItems = await _repository.GetAllAsync( campaignId );

            var result = campaignItems
                .Adapt<List<CampaignItemDto>>()
                .Where( i => i.ParentEntityId == null )
                .ToList();

            await SendAsync( result, cancellation: ct );
        }
    }
}
