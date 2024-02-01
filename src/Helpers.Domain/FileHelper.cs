namespace Helpers.Domain
{
    public class FileHelper
    {
        public static string[] ReadFile(string path)
        {
            return File.ReadAllLines(path);
        }

        public static async Task<string> SaveFile(byte[] content, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (directory == null)
                throw new Exception("Incorrect filePath");

            Directory.CreateDirectory(directory);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await stream.WriteAsync(content, 0, content.Length);
            }

            return filePath;
        }

        public static async Task<string> SaveFile(string text, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (directory == null)
                throw new Exception("Incorrect filePath");

            Directory.CreateDirectory(directory);

            await File.WriteAllTextAsync(filePath, text);

            return filePath;
        }
    }
}
