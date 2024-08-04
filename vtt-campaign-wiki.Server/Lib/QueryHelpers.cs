using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using vtt_campaign_wiki.Server.Features.Shared;

public static class QueryHelpers
{
    public static List<SortItem> ParseSortBy( IQueryCollection query )
    {
        var sortItems = new List<SortItem>();
        int index = 0;

        while (true)
        {
            var key = query[$"sortBy[{index}][key]"].FirstOrDefault();
            var order = query[$"sortBy[{index}][order]"].FirstOrDefault();

            if (key == null)
                break;

            SortOrder? sortOrder = null;
            if (order != null)
            {
                sortOrder = order switch
                {
                    "asc" => SortOrder.Ascending,
                    "desc" => SortOrder.Descending,
                    _ => null
                };
            }

            sortItems.Add( new SortItem
            {
                Key = key,
                Order = sortOrder
            } );

            index++;
        }

        return sortItems;
    }
}
