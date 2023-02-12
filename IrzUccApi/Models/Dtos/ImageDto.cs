using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Dtos
{
    public class ImageDto
    {
        public ImageDto(
            string id, 
            string name, 
            string extension, 
            string data)
        {
            Id = id;
            Name = name;
            Extension = extension;
            Data = data;
        }

        public string Id { get; }
        public string Name { get; }
        public string Extension { get; }
        public string Data { get; }
    }
}
