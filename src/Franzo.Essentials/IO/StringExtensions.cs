namespace Franzo.Essentials.IO;

public static class StringExtensions
{
    // https://stackoverflow.com/questions/26102809/c-sharp-check-if-a-file-path-contains-a-specific-directory
    // @Robustness: case sensitivity should be platform dependent, doesn't look at . or .., etc.
    public static bool FileSystemPathIsRootedIn(this string self, string directoryPath)
    {
        if (!directoryPath.EndsWith(Path.DirectorySeparatorChar))
        {
            directoryPath = $"{directoryPath}{Path.DirectorySeparatorChar}";
        }

        if (self.StartsWith(Path.DirectorySeparatorChar))
        {
            self = self[1..];
        }

        return self.StartsWith(directoryPath);
    }
}
