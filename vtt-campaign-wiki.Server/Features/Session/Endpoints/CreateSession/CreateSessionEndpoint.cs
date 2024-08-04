using vtt_campaign_wiki.Server.Features.Campaign;
using vtt_campaign_wiki.Server.Features.Campaign.Services;
using vtt_campaign_wiki.Server.Features.Image.Services;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Session.Endpoints.CreateSession
{
    public class CreateSessionEndpoint : Endpoint<CreateSessionRequest, SessionDto>
    {
        private readonly IRepositoryBase<SessionEntity> _repository;

        public CreateSessionEndpoint( IRepositoryBase<SessionEntity> repository )
        {
            _repository = repository;
        }

        public override void Configure()
        {
            Verbs( Http.POST );
            Routes( "/api/sessions", "/api/campaigns/{campaignId:int}/sessions" );
            AllowFileUploads();
        }

        public override async Task HandleAsync( CreateSessionRequest req, CancellationToken ct)
        {
            var session = req.Adapt<SessionEntity>();

            session.Image = await ImageHelper.GetImageFromRequest(req.Image);

            await _repository.AddAsync( session );

            var result = session.Adapt<SessionDto>();

            await SendAsync(result, cancellation: ct);
        }
    }
}
