namespace IrzUccApi.Models.Dtos
{
    public class UserPositionDto
    {
        public UserPositionDto(
            Guid id, 
            DateTime start, 
            DateTime? end, 
            PositionDto position)
        {
            Id = id;
            Start = start;
            End = end;
            Position = position;
        }

        public Guid Id { get; }
        public DateTime Start { get; }
        public DateTime? End { get; }
        public PositionDto Position { get; set; }
    }
}
