using System.Text;
using System.Text.Json;
using vtt_campaign_wiki.Server.Features.Campaign;
using vtt_campaign_wiki.Server.Features.AI.Services;
using vtt_campaign_wiki.Server.Features.Session;
using System.Text.Json.Serialization;

public class SuggestedItemsService : ISuggestedItemsService
{
    private readonly HttpClient _httpClient;
    private readonly string _openAiApiKey;

    public SuggestedItemsService( HttpClient httpClient, IConfiguration configuration )
    {
        _httpClient = httpClient;
        _openAiApiKey = configuration["OpenAI:ApiKey"];
    }

    public async Task<IEnumerable<CampaignItemDto>> ExtractEntities( IEnumerable<SessionDto> sessions, IEnumerable<CampaignItemDto> items )
    {
        var prompt = GeneratePrompt( sessions, items );
        var requestBody = new
        {
            model = "gpt-4o-mini-2024-07-18",
            messages = new[]
            {
                new { role = "system", content = "You are an AI assistant specialized in extracting RPG entities." },
                new { role = "user", content = prompt }
            },
            max_tokens = 1000,
            temperature = 0.7,
            response_format = new
            {
                type = "json_schema",
                json_schema = new
                {
                    name = "campaign_item_extraction",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            items = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        title = new { type = "string" },
                                        content = new { type = "string" }
                                    },
                                    required = new[] { "title", "content" },
                                    additionalProperties = false
                                }
                            }
                        },
                        required = new[] { "items" },
                        additionalProperties = false
                    },
                    strict = true
                }
            }
        };

        var requstJson = JsonSerializer.Serialize( requestBody );

        var requestContent = new StringContent( requstJson, Encoding.UTF8, "application/json" );

        using var requestMessage = new HttpRequestMessage( HttpMethod.Post, "https://api.openai.com/v1/chat/completions" );
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( "Bearer", _openAiApiKey );
        requestMessage.Content = requestContent;

        using var response = await _httpClient.SendAsync( requestMessage );
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine( responseBody );
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var chatResponse = JsonSerializer.Deserialize<ChatGPTResponse>( responseBody, options );

        return ParseSuggestions( chatResponse );
    }

    public async Task<string> GenerateImage( string title, string content )
    {
        var prompt = $"Generate an image for an RPG item titled '{title}'. The item has the following details: {content}";

        // Create the request body
        var requestBody = new
        {
            prompt = prompt,
            model = "dall-e-2",
            n = 1, // Number of images to generate
            size = "1024x1024", // Image size
        };

        var requestJson = JsonSerializer.Serialize( requestBody );
        var requestContent = new StringContent( requestJson, Encoding.UTF8, "application/json" );

        // Create the HTTP request
        using var requestMessage = new HttpRequestMessage( HttpMethod.Post, "https://api.openai.com/v1/images/generations" );
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( "Bearer", _openAiApiKey );
        requestMessage.Content = requestContent;

        // Send the request
        using var response = await _httpClient.SendAsync( requestMessage );
        response.EnsureSuccessStatusCode();

        // Parse the response
        var responseBody = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var imageResponse = JsonSerializer.Deserialize<ImageGenerationResponse>( responseBody, options );

        // Return the URL or path of the generated image
        return imageResponse.Data.FirstOrDefault()?.Url;
    }

    private string GeneratePrompt( IEnumerable<SessionDto> sessions, IEnumerable<CampaignItemDto> existingItems )
    {
        var sb = new StringBuilder();
        sb.AppendLine( "You are an AI assistant specialized in extracting RPG entities. Below is a set sessions and the items that were extracted from them. Use this context to understand the types of items that are valuable. Then, based on the following new sessions, suggest new items, characters, or locations that should be added to the wiki. Please do not include any items that have already been created.  Focus on content that exists in later sessions and earlier sessions" );

        // Include existing items
        sb.AppendLine( "Existing Items:" );
        foreach (var item in existingItems)
        {
            sb.AppendLine( $"Title: {item.Title}" );
            sb.AppendLine( $"Content: {item.Content}" );
            sb.AppendLine(); // Adding extra space for readability
        }

        // Add a separator to distinguish old data from new sessions
        sb.AppendLine( "---" );

        // Include new sessions
        sb.AppendLine( "New Sessions:" );
        foreach (var session in sessions)
        {
            sb.AppendLine( $"Title: Session:{session.Number} - {session.Title}" );
            sb.AppendLine( $"Content: {session.Content}" );
            sb.AppendLine();
        }

        return sb.ToString();
    }


    private IEnumerable<CampaignItemDto> ParseSuggestions( ChatGPTResponse response )
    {
        if (response != null && response.Choices.Any())
        {
            var jsonResponse = response.Choices.First().Message.Content;
            try
            {
                if( response.Choices.First().FinishReason != "length")
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var suggestions = JsonSerializer.Deserialize<CampaignContent>( jsonResponse, options );
                    return suggestions?.Items?.Adapt<List<CampaignItemDto>>() ?? new List<CampaignItemDto>();
                }
                else
                {
                    throw new Exception( "Repsonse wasn't able to complete" );
                }
            }
            catch (JsonException)
            {
                throw new Exception( "Failed to parse the response as JSON." );
            }
        }

        return Enumerable.Empty<CampaignItemDto>();
    }
}

// Response model classes
public class ChatGPTResponse
{
    [JsonPropertyName( "id" )]
    public string Id { get; set; }

    [JsonPropertyName( "object" )]
    public string Object { get; set; }

    [JsonPropertyName( "created" )]
    public long Created { get; set; }

    [JsonPropertyName( "model" )]
    public string Model { get; set; }

    [JsonPropertyName( "choices" )]
    public List<ChatGPTChoice> Choices { get; set; }

    [JsonPropertyName( "usage" )]
    public Usage Usage { get; set; }

    [JsonPropertyName( "system_fingerprint" )]
    public string SystemFingerprint { get; set; }
}

public class ChatGPTChoice
{
    [JsonPropertyName( "index" )]
    public int Index { get; set; }

    [JsonPropertyName( "message" )]
    public ChatGPTMessage Message { get; set; }

    [JsonPropertyName( "finish_reason" )]
    public string FinishReason { get; set; }
}

public class ChatGPTMessage
{
    [JsonPropertyName( "role" )]
    public string Role { get; set; }

    [JsonPropertyName( "content" )]
    public string Content { get; set; }
}

public class Usage
{
    [JsonPropertyName( "prompt_tokens" )]
    public int PromptTokens { get; set; }

    [JsonPropertyName( "completion_tokens" )]
    public int CompletionTokens { get; set; }

    [JsonPropertyName( "total_tokens" )]
    public int TotalTokens { get; set; }
}

public class CampaignContent
{
    [JsonPropertyName("items")]
    public List<SuggestedCampaignItem> Items { get; set; }
}

public class SuggestedCampaignItem
{
    [JsonPropertyName( "title" )]
    public string Title { get; set; }
    [JsonPropertyName( "content")]
    public string Content { get; set; }
}

public class ImageGenerationResponse
{
    public List<ImageData> Data { get; set; }
}

public class ImageData
{
    public string Url { get; set; }
}
