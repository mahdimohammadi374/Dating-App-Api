using API.Enums;

namespace API.DTOS
{
    public class GetLikeParams : BasePagination
    {
        public PredicateLikeEnum PredicateUserLike { get; set; }
    }
}
