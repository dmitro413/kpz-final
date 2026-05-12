using System.IO;
using System.Text.Json;
using WpfLibrary.models;

namespace WpfLibrary.services
{
    public class JsonStatisticsService : IStatisticsService
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;
        private GameStatistics _cache;

        public JsonStatisticsService(string filePath = "statistics.json")
        {
            _filePath = filePath;
            _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            _cache = LoadFromFile();
        }

        public GameStatistics Load() => _cache;

        public void Save(GameStatistics statistics)
        {
            _cache = statistics;
            SaveToFile();
        }

        public void RecordWin(DifficultyLevel difficulty, int timeSeconds)
        {
            _cache.TotalGames++;
            _cache.TotalWins++;
            _cache.TotalTimeSecs += timeSeconds;
            _cache.GetStats(difficulty).RecordWin(timeSeconds);
            SaveToFile();
        }

        public void RecordLoss(DifficultyLevel difficulty)
        {
            _cache.TotalGames++;
            _cache.TotalLosses++;
            _cache.GetStats(difficulty).RecordLoss();
            SaveToFile();
        }

        public void Reset()
        {
            _cache = new GameStatistics();
            SaveToFile();
        }

        private GameStatistics LoadFromFile()
        {
            if (!File.Exists(_filePath)) return new GameStatistics();

            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<GameStatistics>(json, _jsonOptions)
                       ?? new GameStatistics();
            }
            catch
            {
                return new GameStatistics();
            }
        }

        private void SaveToFile()
        {
            var json = JsonSerializer.Serialize(_cache, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
    }
}