using vtt_campaign_wiki.Server.Features.Campaign.Services;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.CampaignById
{
    public class CampaignByIdEndpoint : EndpointWithoutRequest<CampaignDto>
    {
        private readonly ICampaignRepository _campaignRepository;

        public CampaignByIdEndpoint( ICampaignRepository campaignRepository )
        {
            _campaignRepository = campaignRepository;
        }

        public override void Configure()
        {
            Get( "/api/campaigns/{id:int}" );
            
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            int id = Route<int>( "id" );

            var campaign = await _campaignRepository.GetByIdAsync( id );
            if (campaign == null)
            {
                await SendNotFoundAsync( ct );
                return;
            }

            var campaignDto = campaign.Adapt<CampaignDto>();
            await SendOkAsync( campaignDto, ct );
        }
    }
}
