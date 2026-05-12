using System.IO;
using System.Text.Json;
using WpfLibrary.models;
using WpfLibrary.models.Achievements;

namespace WpfLibrary.services
{
    public class AchievementService : IAchievementService
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        private List<Achievement> _achievements;

        public AchievementService(string filePath = "achievements.json")
        {
            _filePath = filePath;

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            _achievements = LoadFromFile();
        }

        public IReadOnlyList<Achievement> GetAll() =>
            _achievements.AsReadOnly();

        public IReadOnlyList<Achievement> GetUnlocked() =>
            _achievements
                .Where(a => a.IsUnlocked)
                .ToList()
                .AsReadOnly();

        public IReadOnlyList<Achievement> CheckOnWin(GameResult result)
        {
            var newlyUnlocked = new List<Achievement>();

            foreach (var achievement in _achievements)
            {
                if (achievement.IsUnlocked)
                    continue;

                if (!achievement.CanUnlock(result))
                    continue;

                achievement.Unlock();

                newlyUnlocked.Add(achievement);
            }

            if (newlyUnlocked.Any())
            {
                SaveToFile();
            }

            return newlyUnlocked.AsReadOnly();
        }

        public IReadOnlyList<Achievement> CheckOnLoss()
        {
            return Array.Empty<Achievement>();
        }

        public void Reset()
        {
            _achievements = BuildDefaultAchievements();

            SaveToFile();
        }

        private List<Achievement> LoadFromFile()
        {
            var defaultAchievements = BuildDefaultAchievements();

            if (!File.Exists(_filePath))
                return defaultAchievements;

            try
            {
                var json = File.ReadAllText(_filePath);

                var savedAchievements =
                    JsonSerializer.Deserialize<List<AchievementSaveModel>>(
                        json,
                        _jsonOptions);

                if (savedAchievements == null)
                    return defaultAchievements;

                foreach (var achievement in defaultAchievements)
                {
                    var savedAchievement = savedAchievements
                        .FirstOrDefault(a => a.Id == achievement.Id);

                    if (savedAchievement == null)
                        continue;

                    if (savedAchievement.IsUnlocked)
                    {
                        achievement.Unlock(savedAchievement.UnlockedAt);
                    }
                }

                return defaultAchievements;
            }
            catch
            {
                return defaultAchievements;
            }
        }

        private void SaveToFile()
        {
            var saveModels = _achievements
                .Select(a => new AchievementSaveModel
                {
                    Id = a.Id,
                    IsUnlocked = a.IsUnlocked,
                    UnlockedAt = a.UnlockedAt
                })
                .ToList();

            var json = JsonSerializer.Serialize(
                saveModels,
                _jsonOptions);

            File.WriteAllText(_filePath, json);
        }

        private static List<Achievement> BuildDefaultAchievements() => new()
        {
            new FirstWinAchievement(),
            new SurvivorAchievement(),
            new Win10GamesAchievement(),
            new Win50GamesAchievement(),
            new WinHardAchievement(),
            new WinUltraEasyAchievement(),
            new WinUnder30SecondsAchievement(),
            new WinUnder60SecondsAchievement(),
            new WinWithoutFlagsAchievement()
        };
    }
}