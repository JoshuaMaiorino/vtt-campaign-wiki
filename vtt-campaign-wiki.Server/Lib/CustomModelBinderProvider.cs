using Microsoft.AspNetCore.Mvc.ModelBinding;
using vtt_campaign_wiki.Server.Features.Campaign;

public class CustomModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder( ModelBinderProviderContext context )
    {
        if (context.Metadata.ModelType == typeof( IEnumerable<CampaignPlayerDto> ))
        {
            return new CampaignPlayerDtoModelBinder();
        }

        return null;
    }
}
