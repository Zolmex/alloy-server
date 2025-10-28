#region

using System.IO;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Common.Utilities
{
    public static class FileUtils
    {
        public static async Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            if (!File.Exists(sourcePath))
            {
                return;
            }

            await using var sourceStream = File.Open(sourcePath, FileMode.Open);
            await using var destinationStream = File.Create(destinationPath);

            await sourceStream.CopyToAsync(destinationStream, cancellationToken);
        }
    }
}