namespace Conference.Resources
{
    public class HelperFunction
    {
        public static class HelperFunctions
        {
            public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
            {
                // Get information about the source directory
                var dir = new DirectoryInfo(sourceDir);

                // Check if the source directory exists
                if (!dir.Exists)
                    throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

                // Cache directories before we start copyin
            }
        }
    }
}
