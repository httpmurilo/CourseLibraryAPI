using System;

namespace CourseLibrary.Helpers
{
    public static class DateTimeOffsetExtensions
    {
        
        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset, DateTimeOffset? dateOfDeath)
        {
            var dateToCalculate = DateTime.UtcNow;
            if(dateOfDeath != null)
            {
                dateToCalculate = dateOfDeath.Value.UtcDateTime;
            }
            var age = dateToCalculate.Year - dateTimeOffset.Year;
            if(dateToCalculate<dateTimeOffset.AddYears(age))
            {
                age--;
            }
            return age;
        }
    }
}
