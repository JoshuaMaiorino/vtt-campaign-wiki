﻿using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Campaign.Endpoints.DeleteCampaign
{
    public class DeleteCampaignEndpoint : EndpointWithoutRequest<EmptyResponse>
    {
        private readonly IRepositoryBase<CampaignEntity> _campaignRepository;

        public DeleteCampaignEndpoint( IRepositoryBase<CampaignEntity> campaignRepository )
        {
            _campaignRepository = campaignRepository;
        }

        public override void Configure()
        {
            Delete( "/campaigns/{id:int}" );
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

            await _campaignRepository.DeleteAsync( id );
            await SendNoContentAsync( ct );
        }
    }
}
