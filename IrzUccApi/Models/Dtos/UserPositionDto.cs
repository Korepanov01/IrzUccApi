namespace IrzUccApi.Models.Dtos
{
    public class UserPositionDto
    {
        public UserPositionDto(int id, DateTime start, DateTime? end, bool isActive)
        {
            Id = id;
            Start = start;
            End = end;
            IsActive = isActive;
        }

        public int Id { get; }
        public DateTime Start { get; }
        public DateTime? End { get; }
        public bool IsActive { get; }
    }
}
