using System;

namespace ER_Library.Helpers
{
    public static class ChangeDateTime
    {
        public static void AddDayHour(this ref DateTime dateTime, int day, int hour)
        {
            DateTime tempDate = dateTime.Date;
            TimeSpan timeSpan = new TimeSpan(day, hour, 0, 0);
            dateTime = tempDate.Add(timeSpan);
        }
    }
}
