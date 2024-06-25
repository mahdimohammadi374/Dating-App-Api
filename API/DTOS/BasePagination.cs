namespace API.DTOS
{
    public class BasePagination
    {
        private int _maxPageSize { get; set; } = 50;
        private int _pageSize { get; set; } = 10;
        public int PageNumber { get; set; }
        public int PageSize
        {
            get => _pageSize;

            set => _pageSize = (value > _maxPageSize ? _maxPageSize : value);
        }
    }
}
