using System.IO;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace NationalAvatar;
public class ImageGenerator
{
    public async Task<MemoryStream> Preview(string source, string template)
    {
        var memory = new MemoryStream();
        using var sourceImage = await Image.LoadAsync(source);
        sourceImage.Mutate(async ctx =>
        {
            using var temp = await Image.LoadAsync(template);
            ctx.DrawImage(temp, new Rectangle(new Point { X = 0, Y = 0 }, new Size { Height = sourceImage.Height, Width = sourceImage.Width }), 1);
        });
        await sourceImage.SaveAsync(memory, PngFormat.Instance);
        return memory;
    }
}
