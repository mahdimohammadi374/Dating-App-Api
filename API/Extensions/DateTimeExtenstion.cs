namespace API.Extensions
{
    public static class DateTimeExtenstion
    {
        public static int CalculateAge(this DateTime BirthDay)
        {
            var today=DateTime.Today;
            var age = today.Year - BirthDay.Year;
            if(BirthDay.Date > today.AddYears(-age).Date) 
            {
                age--;
            }
            return age;
        }
    }
}
