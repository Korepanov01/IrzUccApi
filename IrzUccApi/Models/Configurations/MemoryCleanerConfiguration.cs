namespace IrzUccApi.Models.Configurations
{
    public class MemoryCleanerConfiguration
    {
        public int MessageLifetimeDays { get; set; }
        public int EventsLifetimeDays { get; set; }
        public int DeletionPeriodDays { get; set; }
    }
}
