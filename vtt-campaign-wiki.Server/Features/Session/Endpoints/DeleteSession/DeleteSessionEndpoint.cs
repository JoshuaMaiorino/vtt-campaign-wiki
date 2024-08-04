using vtt_campaign_wiki.Server.Features.Campaign;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Session.Endpoints.DeleteSession
{
    public class DeleteSessionEndpoint : EndpointWithoutRequest<EmptyResponse>
    {
        private readonly IRepositoryBase<SessionEntity> _repository;

        public DeleteSessionEndpoint( IRepositoryBase<SessionEntity> repository )
        {
            _repository = repository;
        }

        public override void Configure()
        {
            Delete( "/api/campaigns/{campaignId:int}/sessions/{id:int}" );
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            int id = Route<int>( "id" );

            var session = await _repository.GetByIdAsync( id );
            if (session == null)
            {
                await SendNotFoundAsync( ct );
                return;
            }

            await _repository.DeleteAsync( id );
            await SendNoContentAsync( ct );
        }
    }
}
