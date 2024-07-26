using vtt_campaign_wiki.Server.Features.Shared;

namespace vtt_campaign_wiki.Server.Features.Image
{
    public class ItemImageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
    }
}
