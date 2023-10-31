using System.Text.Json;

namespace Domain.Constants
{
    public class DrawchadSettings
    {
        public static Dictionary<string, object> Settings { get; set; }

        static DrawchadSettings()
        {
            string parentPath = Path.GetDirectoryName(typeof(DrawchadSettings).Assembly.Location);
            string path = Path.Combine(parentPath, "Constants", "DrawchadSettings.json");
            string dict = File.ReadAllText(path);
            Settings = JsonSerializer.Deserialize<Dictionary<string, object>>(dict);
        }
    }
}
