using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Vnoun.API;

public class ImageUploaderHelper
{
    private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png" };

    public static async Task<List<string>> UploadAndResizeImages(IFormFileCollection files, string imageName, string targetFolder)
    {
        if (files == null || files.Count == 0)
            return null;

        var imageNames = new List<string>();

        foreach (var file in files)
        {
            if (!AllowedImageTypes.Contains(file.ContentType))
                throw new Exception("Not an image! Please upload only images.");

            var fileName = $"{imageName}-{Guid.NewGuid().ToString("N")}.png";
            var targetPath = Path.Combine(targetFolder, fileName);

            using var image = await Image.LoadAsync(file.OpenReadStream());

            var sizes = new[]
            {
                new { Name = "small", Width = 100 },
                new { Name = "medium", Width = 400 },
                new { Name = "large", Width = 1200 },
            };

            foreach (var size in sizes)
            {
                var resizedImage = image.Clone(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(size.Width, 0),
                    Mode = ResizeMode.Max
                }));
                imageNames.Add($@"{size.Name}-{fileName}");
                var outputPath = Path.Combine(targetFolder, $"{size.Name}-{fileName}");
                await resizedImage.SaveAsync(outputPath);
            }
        }

        return imageNames;
    }
}