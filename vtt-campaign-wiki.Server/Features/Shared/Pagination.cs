using System.Text.Json.Serialization;

namespace vtt_campaign_wiki.Server.Features.Shared
{
    public class PaginationParameter
    {
        public int? Page { get; set; }
        public int? ItemsPerPage { get; set; }
        public IEnumerable<SortItem>? SortBy { get; set; }
        public string? Search { get; set; }
    }

    public class SortItem
    {
        public string Key { get; set; }
        public SortOrder? Order { get; set; }
    }

    public enum SortOrder
    {
        [JsonPropertyName( "asc" )]
        Ascending,
        [JsonPropertyName( "desc" )]
        Descending
    }

    public class PaginatedResult<T>
    {
        public int ItemsLength { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
