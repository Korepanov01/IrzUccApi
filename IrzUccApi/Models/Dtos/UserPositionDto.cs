namespace IrzUccApi.Models.Dtos
{
    public class UserPositionDto
    {
        public UserPositionDto(Guid id, DateTime start, DateTime? end)
        {
            Id = id;
            Start = start;
            End = end;
        }

        public Guid Id { get; }
        public DateTime Start { get; }
        public DateTime? End { get; }
    }
}
