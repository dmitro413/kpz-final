using System.IO;
using System.Text.Json;
using WpfLibrary.models;
namespace WpfLibrary.services
{
    public class JsonSettingsService : ISettingsService
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonSettingsService(string filePath = "settings.json")
        {
            _filePath = filePath;
            _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        }

        public GameSettings Load()
        {
            if (!File.Exists(_filePath)) return new GameSettings();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<GameSettings>(json, _jsonOptions)
                   ?? new GameSettings();
        }

        public void Save(GameSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
    }
}