using vtt_campaign_wiki.Server.Features.AI.Services;
using vtt_campaign_wiki.Server.Features.Campaign;
using vtt_campaign_wiki.Server.Features.Campaign.Services;
using vtt_campaign_wiki.Server.Features.Session;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.AI.Endpoints.GetSuggestions
{
    public class GetSuggestionsEndpoint : EndpointWithoutRequest<List<CampaignItemDto>>
    {
        private readonly ISuggestedItemsService _suggestionsService;
        private readonly IRepositoryBase<SessionEntity> _sessionRepository;
        private readonly ICampaignItemRepository _campaignItemRepository;

        public GetSuggestionsEndpoint( ISuggestedItemsService suggestionsService, IRepositoryBase<SessionEntity> sessionRepository, ICampaignItemRepository campaignItemRepository )
        {
            _suggestionsService = suggestionsService;
            _sessionRepository = sessionRepository;
            _campaignItemRepository = campaignItemRepository;
        }

        public override void Configure()
        {
            Get("api/suggestion/{campaignId:int}");
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            var campaignId = Route<int>( "campaignId" );

            var sessions = await _sessionRepository.GetAllAsync( s => s.CampaignId == campaignId );
            var items = await _campaignItemRepository.GetAllAsync( i => i.CampaignId == campaignId
                && i.Content != null
                && i.Content != string.Empty
                && ( i.Children == null || !i.Children.Any() ) );

            var suggestions = await _suggestionsService.ExtractEntities( sessions.Adapt<IEnumerable<SessionDto>>(), items.Adapt<IEnumerable<CampaignItemDto>>() );

            foreach (var item in suggestions)
            {
                try
                {
                    //var imageUrl = await _suggestionsService.GenerateImage( item.Title, item.Content );
                    //item.ExternalLink = imageUrl;
                    item.CampaignId = campaignId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine( $"Couldn't Get Image for Item: {item.Title}" );
                }
            }

            await SendOkAsync( suggestions.ToList() );
        }
    }
}
