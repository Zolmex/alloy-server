#pragma warning disable CA1416 // Validate platform compatibility

#region

using Common.Resources.Sprites;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#endregion

namespace DiscordBot
{
    public class ImageHelper
    {
        public const int SPRITES_PER_ROW = 16;

        public static Bitmap GetSpriteFromSheet(string file, int index, int spriteWidth = 8, int spriteHeight = 8)
        {
            var spriteSheet = SpriteLibrary.GetSpriteSheet(file);
            var column = Math.DivRem(index, SPRITES_PER_ROW, out var row);

            var splicedSprite = new Bitmap(spriteWidth, spriteHeight);
            var graphics = Graphics.FromImage(splicedSprite);
            graphics.DrawImage(spriteSheet, new Rectangle(0, 0, spriteWidth, spriteHeight), new Rectangle(row * spriteWidth, column * spriteHeight, spriteWidth, spriteHeight), GraphicsUnit.Pixel);
            graphics.Dispose();
            splicedSprite = ResizeImage(splicedSprite, 128, 128);
            return splicedSprite;
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}