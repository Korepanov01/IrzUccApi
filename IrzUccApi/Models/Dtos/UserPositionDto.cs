namespace IrzUccApi.Models.Dtos
{
    public class UserPositionDto
    {
        public UserPositionDto(Guid id, DateTime start, DateTime? end, bool isActive)
        {
            Id = id;
            Start = start;
            End = end;
            IsActive = isActive;
        }

        public Guid Id { get; }
        public DateTime Start { get; }
        public DateTime? End { get; }
        public bool IsActive { get; }
    }
}
