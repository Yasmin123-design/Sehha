using System;

namespace E_PharmaHub.Dtos
{
    public class DoctorAvailabilityDto
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationInMinutes { get; set; } = 30;
    }
}
