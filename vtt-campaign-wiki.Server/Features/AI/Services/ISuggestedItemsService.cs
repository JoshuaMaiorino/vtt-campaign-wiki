using vtt_campaign_wiki.Server.Features.Campaign;
using vtt_campaign_wiki.Server.Features.Session;

namespace vtt_campaign_wiki.Server.Features.AI.Services
{
    public interface ISuggestedItemsService
    {
        Task<IEnumerable<CampaignItemDto>> ExtractEntities( IEnumerable<SessionDto> sessions, IEnumerable<CampaignItemDto> items );
        Task<string> GenerateImage( string title, string content );
    }
}

