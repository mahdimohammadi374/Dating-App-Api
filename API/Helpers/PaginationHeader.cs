namespace API.Helpers
{
    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemPerPage, int totalItems  ,int totalPages)
        {
            CurrentPage = currentPage;
            ItemPerPage = itemPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; }
        public int ItemPerPage { get; }
        public int TotalItems { get; }
        public int TotalPages { get; }

        //public int CurrentPage { get; }
        //public int ItemPerPage { get; }
        //public int TotalItems { get; }
        //public int TotalPages { get; }
    }
}
