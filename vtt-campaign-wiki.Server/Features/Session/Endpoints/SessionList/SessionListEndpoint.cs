using System.Globalization;
using vtt_campaign_wiki.Server.Features.Shared;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Session.Endpoints.SessionList
{
    public class SessionListEndpoint : EndpointWithoutRequest<PaginatedResult<SessionDto>>
    {
        private readonly IRepositoryBase<SessionEntity> _sessionRepository;

        public SessionListEndpoint( IRepositoryBase<SessionEntity> sessionRepository )
        {
            _sessionRepository = sessionRepository;
        }

        public override void Configure()
        {
            Verbs( Http.GET );
            Routes( "/api/sessions", "/api/campaigns/{campaignId:int}/sessions" );
        }

        public override async Task HandleAsync(CancellationToken ct )
        {
            var campaignId = Route<int?>( "campaignId", false );

            var options = new PaginationParameter
            {
                Page = Query<int?>( "page", false ),
                ItemsPerPage = Query<int?>( "itemsPerPage", false ),
                SortBy = QueryHelpers.ParseSortBy( HttpContext.Request.Query ),
                Search = Query<string?>( "search", false )
            };

            IEnumerable<SessionEntity> sessions;
            int sessionsLength;

            if( campaignId.HasValue)
            {
                ( sessions, sessionsLength ) = await _sessionRepository.GetAllAsync( options, s => s.CampaignId == campaignId );
            }
            else
            {
                (sessions, sessionsLength) = await _sessionRepository.GetAllAsync( options );
            }

            var result = new PaginatedResult<SessionDto>
            {
                Items = sessions.Adapt<IEnumerable<SessionDto>>(),
                ItemsLength = sessionsLength
            };

            await SendOkAsync( result, ct );
        }
    }
}
