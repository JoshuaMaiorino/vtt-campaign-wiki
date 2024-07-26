using vtt_campaign_wiki.Server.Features.Player.Services;

namespace vtt_campaign_wiki.Server.Lib
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UsePlayerProvider( this IApplicationBuilder builder )
        {
            return builder.UseMiddleware<PlayerProvider>();
        }
    }
}
