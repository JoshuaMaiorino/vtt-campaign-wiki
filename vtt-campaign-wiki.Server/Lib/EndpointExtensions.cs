namespace vtt_campaign_wiki.Server.Lib
{
    public static class EndpointExtensions
    {
        public static void MapAppEndpoints( this IEndpointRouteBuilder endpoints )
        {
            // Register the login endpoints
            Features.Player.Endpoints.Login.LoginEndpoint.MapLoginEndpoints( endpoints );

            // Register the register endpoints
            Features.Player.Endpoints.Register.RegisterEndpoint.MapRegisterEndpoints( endpoints );

            // Register player endpoints
            Features.Player.Endpoints.Player.PlayerEndpoints.MapPlayerEndpoints( endpoints );

            // Register campaign endpoints
            Features.Campaign.Endpoints.CampaignEndpoints.MapCampaignEndpoints( endpoints );
        }
    }
}