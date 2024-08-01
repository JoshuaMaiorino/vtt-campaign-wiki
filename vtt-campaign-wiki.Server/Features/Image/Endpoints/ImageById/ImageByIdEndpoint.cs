using vtt_campaign_wiki.Server.Features.Shared.Services;

namespace vtt_campaign_wiki.Server.Features.Image.Endpoints.ImageById
{
    public class ImageByIdEndpoint : EndpointWithoutRequest
    {
        private readonly IRepositoryBase<ItemImageEntity> _imageRepository;

        public ImageByIdEndpoint( IRepositoryBase<ItemImageEntity> imageRepository )
        {
            _imageRepository = imageRepository;
        }

        public override void Configure()
        {
            Get( "/api/Image/{id:int}" );
            AllowAnonymous(); 
        }

        public override async Task HandleAsync( CancellationToken ct )
        {
            var id = Route<int>( "id" );
            
            var image = await _imageRepository.GetByIdAsync( id );
            if (image == null)
            {
                await SendNotFoundAsync( ct );
                return;
            }

            // Return the image with the correct MIME type
            await SendBytesAsync( image.Data, image.Name, image.ContentType, null, false, ct );
        }
    }
}
