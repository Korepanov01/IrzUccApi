using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class FileTooBigException : ArgumentException { }

    public class ForbiddenFileExtensionException : ArgumentException { }

    public class ImageRepository : AppRepository<Image, AppDbContext>
    {
        public ImageRepository(AppDbContext dbContext) : base(dbContext) { }

        private static readonly string[] _permittedExtensions = { ".jpg", ".png", ".gif" };
        private const int _maxFileSizeMB = 5;

        public async Task<Image> AddAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !_permittedExtensions.Contains(extension))
                throw new ForbiddenFileExtensionException();

            if (file.Length > _maxFileSizeMB * 1024 * 1024)
                throw new FileTooBigException();

            await file.CopyToAsync(memoryStream);

            var image = new Image()
            {
                Content = memoryStream.ToArray(),
                ContentType = file.ContentType
            };

            Add(image);

            return image;
        }
    }
}
