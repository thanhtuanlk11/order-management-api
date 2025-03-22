using System.Collections.Generic;
using System;

namespace WebApplication1.DTOs
{
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public PagedResultDto(List<T> items, int page, int pageSize, int totalItems)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalItems = totalItems;
        }
    }
}
