using vtt_campaign_wiki.Server.Features.Image;
using vtt_campaign_wiki.Server.Features.Shared;
using vtt_campaign_wiki.Server.Features.Shared.Constants;
using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Session.Endpoints.SessionHeroImage
{
    public class SessionHeroImageEndpoint : EndpointWithoutRequest
    {
        private readonly IRepositoryBase<SessionEntity> _sessionRepository;
        private readonly IRepositoryBase<ItemImageEntity> _imageRepository;

        public SessionHeroImageEndpoint( IRepositoryBase<SessionEntity> sessionRepository, IRepositoryBase<ItemImageEntity> imageRepository )
        {
            _sessionRepository = sessionRepository;
            _imageRepository = imageRepository;
        }

        public override void Configure()
        {
            Verbs( Http.GET );
            Routes( "/api/sessions/hero-image", "/api/campaigns/{campaignId:int}/sessions/hero-image" );
            AllowAnonymous();
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            var campaignId = Route<int?>( "campaignId", false );

            IEnumerable<SessionEntity> sessions;

            if (campaignId.HasValue)
            {
                sessions = await _sessionRepository.GetAllAsync( s => s.ImageId != null && s.CampaignId == campaignId);
            }
            else
            {
                sessions = await _sessionRepository.GetAllAsync( s => s.ImageId != null );
            }


            if(sessions.Any())
            {
                var imageId = sessions
                .OrderByDescending( s => s.Number )
                .FirstOrDefault()
                .ImageId ?? 0;

                var image = await _imageRepository.GetByIdAsync( imageId );

                await SendBytesAsync( image.Data, image.Name, image.ContentType, null, false, ct );
            }
            else
            {
                await SendNotFoundAsync( ct );
            }

            
        }
    }
}
