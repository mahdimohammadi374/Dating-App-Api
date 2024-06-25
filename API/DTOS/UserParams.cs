using API.Enums;

namespace API.DTOS
{
    public class UserParams : BasePagination
    {
     
        public string currentUser { get; set; }
        public int minAge { get; set; } = 18;
        public int maxAge { get; set; } = 150;
        public GenderEnum Gender { get; set;  }=GenderEnum.male;
        public OrderBy OrderBy { get; set; } = OrderBy.lastActive;
        public TypeSort TypeSort { get; set; }
   
    }

    public enum OrderBy
    {
        lastActive,
        created,
        age
    }
    public enum TypeSort
    {
        acc,
        dec
    }
}
