using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamess.Core.CustomEntities
{
    public class PaginationMetadata
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;
    }

    public class PagedList<T> : List<T>
    {
        public PaginationMetadata Pagination { get; }

        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            AddRange(items);
            Pagination = new PaginationMetadata
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }

        public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
