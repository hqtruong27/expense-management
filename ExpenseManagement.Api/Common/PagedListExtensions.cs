using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExpenseManagement.Api.Common
{
    public abstract class QueryStringParameters
    {
        private string? _query;
        private int _pageSize = 10;
        private const int _maxPageSize = 20;

        public DateTime? From => string.IsNullOrEmpty(FromDate) ? null : DateTime.TryParse(FromDate, out var from) ? from : null;
        public DateTime? To => string.IsNullOrEmpty(ToDate) ? null : DateTime.TryParse(ToDate, out var from) ? from : null;
        public int PageSize { get => _pageSize; set => _pageSize = value > _maxPageSize ? _maxPageSize : value; }
        public int PageNumber { get; set; } = 1;
        [FromQuery(Name = "from")]
        public string? FromDate { get; set; }
        [FromQuery(Name = "to")]
        public string? ToDate { get; set; }

        [FromQuery(Name = "q")]
        public string? Query { get => _query; set => _query = string.IsNullOrWhiteSpace(value) ? null : value.Trim(); }
    }

    public class PagedList<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public List<T> Items { get; set; }
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }
    }
    public static class PagedListExtensions
    {
        /// <summary>
        /// Returns an <see cref="PagedList{TResult}" /> from an <see cref="IQueryable{T}" /> by enumerating it.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         See <see href=""> link</see> for more information and examples.
        ///     </para>
        /// </remarks>
        /// <returns> An <see cref="PagedList{TResult}"/> whose elements are the result of invoking a projection function on each element of source.</returns>
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
        /// <summary>
        /// Asynchronously returns an <see cref="List{TResult}" /> from an <see cref="IQueryable{T}" /> by enumerating it  asynchronously.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Multiple active operations on the same context instance are not supported. Use <see langword="await" /> to ensure
        ///         that any asynchronous operations have completed before calling another method on this context.
        ///          See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information and examples.
        ///     </para>
        ///     <para>
        ///         See <see href=""> link</see> for more information and examples.
        ///     </para>
        /// </remarks>
        /// <returns> An <see cref="List{TResult}"/> whose elements are the result of invoking a projection function on each element of source.</returns>
        public static async Task<List<TResult>> ToPagedListAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, int, TResult>> selector, int pageNumber, int pageSize)
        => await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(selector).ToListAsync();
        /// <summary>
        /// Asynchronously returns an <see cref="PagedList{TSource}" /> from an <see cref="IQueryable{T}" /> by enumerating it  asynchronously.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Multiple active operations on the same context instance are not supported. Use <see langword="await" /> to ensure
        ///         that any asynchronous operations have completed before calling another method on this context.
        ///          See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information and examples.
        ///     </para>
        ///     <para>
        ///         See <see href=""> link</see> for more information and examples.
        ///     </para>
        /// </remarks>
        /// <returns> An <see cref="PagedList{TSource}"/> whose elements are the result of invoking a projection function on each element of source.</returns>
        public static async Task<PagedList<TSource>> ToPagedListAsync<TSource>(this IQueryable<TSource> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<TSource>(items, count, pageNumber, pageSize);
        }
        /// <summary>
        /// Asynchronously returns an <see cref="PagedList{TResult}" /> from an <see cref="IQueryable{T}" /> by enumerating it  asynchronously.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Multiple active operations on the same context instance are not supported. Use <see langword="await" /> to ensure
        ///         that any asynchronous operations have completed before calling another method on this context.
        ///          See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information and examples.
        ///     </para>
        ///     <para>
        ///         See <see href=""> link</see> for more information and examples.
        ///     </para>
        /// </remarks>
        /// <returns> An <see cref="PagedList{TResult}"/> whose elements are the result of invoking a projection function on each element of source.</returns>
        public static async Task<PagedList<TResult>> ToPagedListAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(selector).ToListAsync();

            return new PagedList<TResult>(items, count, pageNumber, pageSize);
        }
        //public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
        //{
        //    var count = source.Count();
        //    var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        //    return new PagedList<T>(items, count, pageNumber, pageSize);
        //}
        public static PagedList<TResult> ToPagedList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(selector).ToList();

            return new PagedList<TResult>(items, count, pageNumber, pageSize);
        }
    }
}
