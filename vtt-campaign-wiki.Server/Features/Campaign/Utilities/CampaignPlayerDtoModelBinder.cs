using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Threading.Tasks;
using vtt_campaign_wiki.Server.Features.Campaign;

public class CampaignPlayerDtoModelBinder : IModelBinder
{
    public Task BindModelAsync( ModelBindingContext bindingContext )
    {
        var players = new List<CampaignPlayerDto>();
        var form = bindingContext.HttpContext.Request.Form;

        for (int i = 0; ; i++)
        {
            var playerIdKey = $"players[{i}][playerId]";
            var userNameKey = $"players[{i}][userName]";
            var isDMKey = $"players[{i}][isDM]";
            var campaignIdKey = $"players[{i}][campaignId]";

            if (!form.ContainsKey( playerIdKey ) ||
                !form.ContainsKey( userNameKey ) ||
                !form.ContainsKey( isDMKey ) ||
                !form.ContainsKey( campaignIdKey ))
            {
                break;
            }

            var player = new CampaignPlayerDto
            {
                PlayerId = int.Parse( form[playerIdKey] ),
                IsDM = bool.Parse( form[isDMKey] ),
                CampaignId = int.Parse( form[campaignIdKey] )
            };

            players.Add( player );
        }

        bindingContext.Result = ModelBindingResult.Success( players );
        return Task.CompletedTask;
    }
}
