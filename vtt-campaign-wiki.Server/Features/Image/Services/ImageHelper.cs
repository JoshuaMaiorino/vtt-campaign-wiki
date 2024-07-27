namespace vtt_campaign_wiki.Server.Features.Image.Services
{
    public static class ImageHelper
    {
        public static async Task<ItemImageEntity> GetImageFromRequest(IFormFile image)
        {
            if (image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    return new ItemImageEntity
                    {
                        Name = image.FileName,
                        ContentType = image.ContentType,
                        Data = memoryStream.ToArray()
                    };
                }
            }
            return null;
        }
    }
}

