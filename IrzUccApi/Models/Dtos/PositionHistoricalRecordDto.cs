namespace IrzUccApi.Models.Dtos
{
    public class PositionHistoricalRecordDto
    {
        public PositionHistoricalRecordDto(DateTime dateTime, string positionName)
        {
            DateTime = dateTime;
            PositionName = positionName;
        }
        public DateTime DateTime { get; }
        public string PositionName { get; }
    }
}
