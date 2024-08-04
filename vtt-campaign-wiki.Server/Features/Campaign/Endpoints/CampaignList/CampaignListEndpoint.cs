using vtt_campaign_wiki.Server.Features.Campaign.Services;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.CampaignList
{
    public class CampaignListEndpoint : EndpointWithoutRequest<IEnumerable<CampaignDto>>
    {
        private readonly ICampaignRepository _campaignRepository;

        public CampaignListEndpoint( ICampaignRepository campaignRepository )
        {
            _campaignRepository = campaignRepository;
        }

        public override void Configure()
        {
            Get( "/api/campaigns" );
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            var campaigns = await _campaignRepository.GetAllAsync();
            var campaignDtos = campaigns.Adapt<IEnumerable<CampaignDto>>();
            await SendOkAsync( campaignDtos, ct );
        }
    }
}
