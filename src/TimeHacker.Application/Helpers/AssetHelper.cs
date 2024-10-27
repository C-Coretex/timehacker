using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TimeHacker.Application.Helpers
{
    public static class AssetHelper
    {
        public static Dictionary<string, string> GetAssets()
        {
            var manifestPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "js", "build", "asset-manifest.json");
            var jsonContent = File.ReadAllText(manifestPath);
            var manifest = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(manifest["files"].ToString());
        }
    }
}
