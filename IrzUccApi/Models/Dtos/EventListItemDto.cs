namespace IrzUccApi.Models.Dtos
{
    public class EventListItemDto
    {
        public EventListItemDto(
            int id,
            string title,
            DateTime start,
            DateTime end,
            string? cabinetName)
        {
            Id = id;
            Title = title;
            Start = start;
            End = end;
            CabinetName = cabinetName;
        }

        public int Id { get; }
        public string Title { get; }
        public DateTime Start { get; }
        public DateTime End { get; }
        public string? CabinetName { get; }
    }
}
