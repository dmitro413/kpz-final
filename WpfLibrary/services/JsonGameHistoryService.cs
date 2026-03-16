using System.IO;
using System.Text.Json;
using WpfLibrary.models;

namespace WpfLibrary.services
{
    public class JsonGameHistoryService : IGameHistoryService
    {
        private const int MaxEntries = 20;

        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;
        private List<GameHistoryEntry> _history;

        public JsonGameHistoryService(string filePath = "history.json")
        {
            _filePath = filePath;
            _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            _history = LoadFromFile();
        }

        public IReadOnlyList<GameHistoryEntry> GetRecent(int count = 20) =>
            _history.TakeLast(count).Reverse().ToList().AsReadOnly();

        public void Record(GameHistoryEntry entry)
        {
            _history.Add(entry);
            if (_history.Count > MaxEntries)
                _history = _history.TakeLast(MaxEntries).ToList();
            SaveToFile();
        }

        public void Clear()
        {
            _history.Clear();
            SaveToFile();
        }

        private List<GameHistoryEntry> LoadFromFile()
        {
            if (!File.Exists(_filePath)) return new List<GameHistoryEntry>();
            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<GameHistoryEntry>>(json, _jsonOptions)
                       ?? new List<GameHistoryEntry>();
            }
            catch { return new List<GameHistoryEntry>(); }
        }

        private void SaveToFile()
        {
            var json = JsonSerializer.Serialize(_history, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
    }
}