using vtt_campaign_wiki.Server.Features.Campaign.Endpoints.CampaignById;
using vtt_campaign_wiki.Server.Features.Image.Services;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.CreateCampaign
{
    public class CreateCampaignEndpoint : Endpoint<CreateCampaignRequest, CampaignDto>
    {
        private readonly IRepositoryBase<CampaignEntity> _campaignRepository;

        public CreateCampaignEndpoint( IRepositoryBase<CampaignEntity> campaignRepository )
        {
            _campaignRepository = campaignRepository;
        }

        public override void Configure()
        {
            Post( "/campaigns" );
            AllowFileUploads();
        }

        public override async Task HandleAsync( CreateCampaignRequest req, CancellationToken ct )
        {
            var campaignEntity = req.Adapt<CampaignEntity>();

            campaignEntity.Image = await ImageHelper.GetImageFromRequest( req.Image );

            await _campaignRepository.AddAsync( campaignEntity );

            var campaignDto = campaignEntity.Adapt<CampaignDto>();
            await SendCreatedAtAsync<CampaignByIdEndpoint>( $"/campaigns/{campaignDto.Id}", campaignDto, null, null, false, ct );
        }
    }
}
