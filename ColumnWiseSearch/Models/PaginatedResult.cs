using System;
using Microsoft.EntityFrameworkCore;

namespace ColumnWiseSearch.Models;

// Custom paginated result class
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    public PagedResult(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
        PageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public static async Task<PagedResult<T>> CreateAsync(
        IQueryable<T> query,
        int page,
        int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        var sqlQuery = query.ToQueryString();
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>(items, totalCount, page, pageSize);
    }
}
