using vtt_campaign_wiki.Server.Features.Campaign.Services;
using vtt_campaign_wiki.Server.Features.Image.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.CreateCampaignItem
{
    public class CreateCampaignItemEndpoint : Endpoint<CreateCampaignItemRequest, CampaignItemDto>
    {
        private readonly ICampaignItemRepository _repository;

        public CreateCampaignItemEndpoint( ICampaignItemRepository repository )
        {
            _repository = repository;
        }

        public override void Configure()
        {
            Post( "/campaigns/{campaignId:int}/items" );
            AllowFileUploads();
        }

        public override async Task HandleAsync( CreateCampaignItemRequest req, CancellationToken ct )
        {
            var campaignId = Route<int>( "campaignId" );

            var campaignItem = req.Adapt<CampaignItemEntity>();
            campaignItem.CampaignId = campaignId;

            campaignItem.Image = await ImageHelper.GetImageFromRequest( req.Image );

            if ( req.ParentEntityId == 0)
            {
                campaignItem.ParentEntityId = null;
            }

            await _repository.AddAsync( campaignItem );

            var result = campaignItem.Adapt<CampaignItemDto>();

            await SendAsync( result, cancellation: ct );
        }
    }
}
