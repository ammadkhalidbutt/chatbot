using System;

namespace ColumnWiseSearch.Models;


// Filter models
public class FilterParameter
{
    public string Property { get; set; }
    public string Operation { get; set; }
    public string Value { get; set; }
}

public class SearchRequest
{
    public List<FilterParameter> Filters { get; set; } = new List<FilterParameter>();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; }
    public bool SortDescending { get; set; }
}
