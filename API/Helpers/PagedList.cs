using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PagedList<T> where T : class
    {
        public PagedList(IEnumerable<T> item , int pageNumber , int pageSize , int count) 
        {
            this.TotalPage = (int)Math.Ceiling(count / (double)pageSize);
            this.CurrentPage = pageNumber;
            this.PageSize = pageSize;
            this.TotalCount = count;
            this.Items = item;
        }

        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<T> Items { get; set; }=new List<T>();

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source , int pageNumber , int pageSize)
        {
            var count = await source.CountAsync();
            var item = await source.Skip((pageNumber -1)*pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(item , pageNumber , pageSize ,count);
        }
    }
}
