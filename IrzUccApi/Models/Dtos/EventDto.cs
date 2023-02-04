using IrzUccApi.Models.Db;
using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Dtos
{
    public class EventDto
    {
        public EventDto(
            int id, 
            string title, 
            DateTime start, 
            DateTime end, 
            string? description, 
            string? cabinetName, 
            bool isPublic, 
            UserHeaderDto creator, 
            IEnumerable<UserHeaderDto> listeners)
        {
            Id = id;
            Title = title;
            Start = start;
            End = end;
            Description = description;
            CabinetName = cabinetName;
            IsPublic = isPublic;
            Creator = creator;
            Listeners = listeners;
        }

        public int Id { get; }
        public string Title { get; }
        public DateTime Start { get; }
        public DateTime End { get; }
        public string? Description { get; }
        public string? CabinetName { get; }
        public bool IsPublic { get; }

        public UserHeaderDto Creator { get; }
        public IEnumerable<UserHeaderDto> Listeners { get; }
    }
}
