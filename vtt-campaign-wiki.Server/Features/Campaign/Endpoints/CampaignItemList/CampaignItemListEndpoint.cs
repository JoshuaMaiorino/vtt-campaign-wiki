﻿using vtt_campaign_wiki.Server.Features.Campaign.Services;

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
            Get( "/campaigns/{campaignId:int}/items" );
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            var campaignId = Route<int>( "campaignId" );
            
            var campaignItems = await _repository.GetAllAsync( campaignId );

            var result = campaignItems.ToList().Adapt<List<CampaignItemDto>>();

            await SendAsync( result, cancellation: ct );
        }
    }
}
