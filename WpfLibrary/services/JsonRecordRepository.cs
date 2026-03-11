using System.IO;
using System.Text.Json;
using WpfLibrary.models;
using WpfLibrary.services;

namespace WpfLibrary.Services
{
    public class JsonRecordRepository : IRecordRepository
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;
        private List<GameRecord> _records;

        public JsonRecordRepository(string filePath = "records.json")
        {
            _filePath = filePath;
            _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            _records = LoadFromFile();
        }

        public IReadOnlyList<GameRecord> GetAll() => _records.AsReadOnly();

        public IReadOnlyList<GameRecord> GetByDifficulty(DifficultyLevel difficulty) =>
            _records.Where(r => r.Difficulty == difficulty)
                    .OrderBy(r => r.TimeSeconds)
                    .ToList()
                    .AsReadOnly();

        public void Save(GameRecord record)
        {
            _records.Add(record);
            SaveToFile();
        }

        public void Clear()
        {
            _records.Clear();
            SaveToFile();
        }

        private List<GameRecord> LoadFromFile()
        {
            if (!File.Exists(_filePath)) return new List<GameRecord>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<GameRecord>>(json, _jsonOptions)
                   ?? new List<GameRecord>();
        }

        private void SaveToFile()
        {
            var json = JsonSerializer.Serialize(_records, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
    }
}

