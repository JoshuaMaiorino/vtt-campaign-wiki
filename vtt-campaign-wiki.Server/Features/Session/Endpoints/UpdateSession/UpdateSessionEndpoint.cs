using vtt_campaign_wiki.Server.Features.Image.Services;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Session.Endpoints.UpdateSession
{
    public class UpdateSessionEndpoint : Endpoint<UpdateSessionRequest, SessionDto>
    {
        private IRepositoryBase<SessionEntity> _repository;

        public UpdateSessionEndpoint( IRepositoryBase<SessionEntity> repository )
        {
            _repository = repository;
        }

        public override void Configure()
        {
            Verbs( Http.PUT );
            Routes( "/api/sessions/{id:int}", "/api/campaigns/{campaignId:int}/sessions/{id:int}" );
            AllowFileUploads();
        }

        public override async Task HandleAsync( UpdateSessionRequest req, CancellationToken ct )
        {
            var id = Route<int>( "id" );

            if (id != req.Id)
            {
                AddError( "Invalid Request" );
                await SendErrorsAsync( 400, ct );
                return;
            }

            var sessionEntity = req.Adapt<SessionEntity>();
            sessionEntity.Image = await ImageHelper.GetImageFromRequest( req.Image );

            await _repository.UpdateAsync( sessionEntity );

            var updatedFromDb = await _repository.GetByIdAsync( sessionEntity.Id );
            if (updatedFromDb == null)
            {
                AddError( "Item not found after update" );
                await SendErrorsAsync( 404, ct );
                return;
            }

            await SendOkAsync( updatedFromDb.Adapt<SessionDto>(), ct );
        }
    }
}
