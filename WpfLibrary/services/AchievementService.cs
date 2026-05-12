using System.IO;
using System.Text.Json;
using WpfLibrary.models;
using WpfLibrary.models.Achievements;

namespace WpfLibrary.services
{
    public class AchievementService : IAchievementService
    {
        private readonly string _filePath;
        private readonly IDataStorage<List<AchievementSaveModel>> _storage; // Використовуємо стратегію
        private List<Achievement> _achievements;

        private List<Achievement> _achievements;

        public AchievementService(IDataStorage<List<AchievementSaveModel>> storage, string filePath = "achievements.json")
    {
        _storage = storage;
        _filePath = filePath;
        _achievements = LoadFromFile();
    }

    private List<Achievement> LoadFromFile()
    {
        var defaultList = BuildDefaultAchievements();
        // Вся складна логіка десеріалізації тепер в один рядок:
        var savedData = _storage.Load(_filePath, new List<AchievementSaveModel>());

        foreach (var saved in savedData)
        {
            var achievement = defaultList.FirstOrDefault(a => a.Id == saved.Id);
            if (achievement != null && saved.IsUnlocked)
            {
                achievement.Unlock(saved.UnlockedAt);
            }
        }
        return defaultList;
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
        var models = _achievements.Select(a => new AchievementSaveModel {
            Id = a.Id, IsUnlocked = a.IsUnlocked, UnlockedAt = a.UnlockedAt 
        }).ToList();
        
        _storage.Save(_filePath, models); // Просто віддаємо дані на збереження
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
