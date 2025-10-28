#pragma warning disable CA1416 // Validate platform compatibility

#region

using Common.Utilities;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

#endregion

namespace Common.Resources.Sprites
{
    public class SpriteLibrary
    {
        private static readonly Logger _log = new(typeof(SpriteLibrary));
        private static readonly Dictionary<string, Image> _spriteSheets = new();

        /// <summary>
        /// Loads every .xml file in the directory <paramref name="dir"/>.
        /// </summary>
        /// <param name="dir">Directory containing XML asset files.</param>
        public static void Load(string dir)
        {
            var files = Directory.EnumerateFiles(dir, "*png", SearchOption.AllDirectories);
            foreach (var file in files.Where(x => x.Contains("EmbeddedAssets_")))
            {
                _log.Debug($"Loading Sprite {file}...");
                _spriteSheets.Add(GetSpriteSheetNameFromFormat(Path.GetFileName(file)), Image.FromFile(file));
            }

            _log.Info("Sprite Library loaded successfully.");
        }

        private static string GetSpriteSheetNameFromFormat(string fileName)
        {
            try
            {
                // EmbeddedData_xxxEmbed_ we just want the xxx
                var ret = fileName.Split("EmbeddedAssets_")[1];
                // xxxEmbed_
                ret = ret.Split("Embed_")[0];
                ret = Path.ChangeExtension(ret, null);
                return ret;
            }
            catch
            {
                return fileName;
            }
        }

        public static Image GetSpriteSheet(string fileName)
        {
            return _spriteSheets[fileName];
        }
    }
}