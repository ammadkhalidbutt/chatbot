using System;
using System.Linq.Expressions;
using System.Reflection;
using ColumnWiseSearch.Models;

namespace ColumnWiseSearch.Helpers.Extensions;

// Extension methods for search capabilities
public static class QueryableExtensions
{
    private static readonly Dictionary<string, string> OperationMap = new Dictionary<string, string>
        {
            { "eq", "Equals" },
            { "ne", "NotEquals" },
            { "gt", "GreaterThan" },
            { "ge", "GreaterThanOrEqual" },
            { "lt", "LessThan" },
            { "le", "LessThanOrEqual" },
            { "contains", "Contains" },
            { "startswith", "StartsWith" },
            { "endswith", "EndsWith" }
        };

    public static IQueryable<T> ApplySearch<T>(
        this IQueryable<T> query,
        SearchRequest request) where T : class
    {
        if (request?.Filters == null || !request.Filters.Any())
            return query;

        foreach (var filter in request.Filters)
        {
            if (string.IsNullOrWhiteSpace(filter.Property) ||
                string.IsNullOrWhiteSpace(filter.Operation) ||
                string.IsNullOrWhiteSpace(filter.Value))
                continue;

            query = ApplyFilter(query, filter);
        }

        // Apply sorting if specified
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = ApplySorting(query, request.SortBy, request.SortDescending);
        }

        return query;
    }

    private static IQueryable<T> ApplyFilter<T>(
        IQueryable<T> query,
        FilterParameter filter) where T : class
    {
        // Map the operation string to a method name
        if (!OperationMap.TryGetValue(filter.Operation.ToLower(), out var methodName))
            return query;

        PropertyInfo property = typeof(T).GetProperty(filter.Property,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (property == null)
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(parameter, property);

        // Convert the value to the property type
        object convertedValue;
        try
        {
            convertedValue = ConvertValue(filter.Value, property.PropertyType);
        }
        catch
        {
            // If conversion fails, skip this filter
            return query;
        }

        var value = Expression.Constant(convertedValue);
        Expression expression;

        // Handle string operations
        if (property.PropertyType == typeof(string) &&
            (methodName == "Contains" || methodName == "StartsWith" || methodName == "EndsWith"))
        {
            var method = typeof(string).GetMethod(methodName, new[] { typeof(string) });
            expression = Expression.Call(propertyAccess, method, value);
        }
        // Handle comparison operations
        else
        {
            switch (methodName)
            {
                case "Equals":
                    expression = Expression.Equal(propertyAccess, value);
                    break;
                case "NotEquals":
                    expression = Expression.NotEqual(propertyAccess, value);
                    break;
                case "GreaterThan":
                    expression = Expression.GreaterThan(propertyAccess, value);
                    break;
                case "GreaterThanOrEqual":
                    expression = Expression.GreaterThanOrEqual(propertyAccess, value);
                    break;
                case "LessThan":
                    expression = Expression.LessThan(propertyAccess, value);
                    break;
                case "LessThanOrEqual":
                    expression = Expression.LessThanOrEqual(propertyAccess, value);
                    break;
                default:
                    return query;
            }
        }

        var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
        return query.Where(lambda);
    }

    private static object ConvertValue(string value, Type targetType)
    {
        if (targetType == typeof(string))
            return value;

        if (targetType == typeof(int) || targetType == typeof(int?))
            return int.Parse(value);

        if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            return decimal.Parse(value);

        if (targetType == typeof(double) || targetType == typeof(double?))
            return double.Parse(value);

        if (targetType == typeof(bool) || targetType == typeof(bool?))
            return bool.Parse(value);

        if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
            return DateTime.Parse(value);

        return Convert.ChangeType(value, targetType);
    }

    private static IQueryable<T> ApplySorting<T>(
        IQueryable<T> query,
        string sortBy,
        bool descending) where T : class
    {
        PropertyInfo property = typeof(T).GetProperty(sortBy,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (property == null)
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(parameter, property);
        var lambda = Expression.Lambda(propertyAccess, parameter);

        var orderByMethod = descending
            ? "OrderByDescending"
            : "OrderBy";

        var resultType = typeof(IOrderedQueryable<>).MakeGenericType(typeof(T));
        var orderByExpression = Expression.Call(
            typeof(Queryable),
            orderByMethod,
            new Type[] { typeof(T), property.PropertyType },
            query.Expression,
            Expression.Quote(lambda));

        return (IQueryable<T>)query.Provider.CreateQuery(orderByExpression);
    }
}
