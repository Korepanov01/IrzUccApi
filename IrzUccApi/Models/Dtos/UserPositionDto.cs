namespace IrzUccApi.Models.Dtos
{
    public class PositionHistoricalRecordDto
    {
        public PositionHistoricalRecordDto(DateTime dateTime, PositionDto position)
        {
            DateTime = dateTime;
            Position = position;
        }
        public DateTime DateTime { get; }
        public PositionDto Position { get; }
    }
}
