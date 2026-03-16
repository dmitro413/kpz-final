using System.IO;
using System.Text.Json;
using WpfLibrary.models;

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
            _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            _achievements = LoadFromFile();
        }

        public IReadOnlyList<Achievement> GetAll() =>
            _achievements.AsReadOnly();

        public IReadOnlyList<Achievement> GetUnlocked() =>
            _achievements.Where(a => a.IsUnlocked).ToList().AsReadOnly();

        public IReadOnlyList<Achievement> CheckOnWin(
            DifficultyLevel difficulty,
            int timeSeconds,
            int flagsUsed,
            int winStreak)
        {
            var newlyUnlocked = new List<Achievement>();

            TryUnlock(AchievementCondition.FirstWin,
                true, newlyUnlocked);

            TryUnlock(AchievementCondition.WinWithoutFlags,
                flagsUsed == 0, newlyUnlocked);

            TryUnlock(AchievementCondition.WinUnder30Seconds,
                timeSeconds < 30, newlyUnlocked);

            TryUnlock(AchievementCondition.WinUnder60Seconds,
                timeSeconds < 60, newlyUnlocked);

            TryUnlock(AchievementCondition.Win10Games,
                _achievements.Count(a => a.Condition == AchievementCondition.FirstWin && a.IsUnlocked) > 0
                && GetTotalWins() >= 10, newlyUnlocked);

            TryUnlock(AchievementCondition.Win50Games,
                GetTotalWins() >= 50, newlyUnlocked);

            TryUnlock(AchievementCondition.WinHard,
                difficulty == DifficultyLevel.Hard, newlyUnlocked);

            TryUnlock(AchievementCondition.WinUltraEasy,
                difficulty == DifficultyLevel.UltraEasy, newlyUnlocked);

            TryUnlock(AchievementCondition.Survivor,
                winStreak >= 5, newlyUnlocked);

            if (newlyUnlocked.Any())
                SaveToFile();

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
        private void TryUnlock(
            AchievementCondition condition,
            bool conditionMet,
            List<Achievement> newlyUnlocked)
        {
            if (!conditionMet) return;

            var achievement = _achievements
                .FirstOrDefault(a => a.Condition == condition);

            if (achievement == null || achievement.IsUnlocked) return;

            achievement.Unlock();
            newlyUnlocked.Add(achievement);
        }

        private int GetTotalWins() =>
            _achievements
                .Where(a => a.Condition == AchievementCondition.FirstWin && a.IsUnlocked)
                .Count();

        private List<Achievement> LoadFromFile()
        {
            if (!File.Exists(_filePath))
                return BuildDefaultAchievements();

            try
            {
                var json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<List<Achievement>>(json, _jsonOptions);
                if (loaded == null || loaded.Count == 0)
                    return BuildDefaultAchievements();

                var defaults = BuildDefaultAchievements();
                foreach (var def in defaults)
                {
                    if (!loaded.Any(a => a.Id == def.Id))
                        loaded.Add(def);
                }
                return loaded;
            }
            catch
            {
                return BuildDefaultAchievements();
            }
        }

        private void SaveToFile()
        {
            var json = JsonSerializer.Serialize(_achievements, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        private static List<Achievement> BuildDefaultAchievements() => new()
        {
            new() { Id = "first_win",       Condition = AchievementCondition.FirstWin,
                    Title = "First Blood",         Icon = "🎉",
                    Description = "Win your first game" },

            new() { Id = "no_flags",        Condition = AchievementCondition.WinWithoutFlags,
                    Title = "Flagless",            Icon = "🚫",
                    Description = "Win without placing any flags" },

            new() { Id = "speed_30",        Condition = AchievementCondition.WinUnder30Seconds,
                    Title = "Speed Demon",         Icon = "⚡",
                    Description = "Win in under 30 seconds" },

            new() { Id = "speed_60",        Condition = AchievementCondition.WinUnder60Seconds,
                    Title = "Quick Fingers",       Icon = "⏱",
                    Description = "Win in under 60 seconds" },

            new() { Id = "win_10",          Condition = AchievementCondition.Win10Games,
                    Title = "Veteran",             Icon = "🏆",
                    Description = "Win 10 games total" },

            new() { Id = "win_50",          Condition = AchievementCondition.Win50Games,
                    Title = "Legend",              Icon = "👑",
                    Description = "Win 50 games total" },

            new() { Id = "hard_win",        Condition = AchievementCondition.WinHard,
                    Title = "Fearless",            Icon = "💀",
                    Description = "Win on Hard difficulty" },

            new() { Id = "ultra_easy_win",  Condition = AchievementCondition.WinUltraEasy,
                    Title = "Baby Steps",          Icon = "🧪",
                    Description = "Win on Ultra Easy (it's a start!)" },

            new() { Id = "survivor",        Condition = AchievementCondition.Survivor,
                    Title = "Survivor",            Icon = "🔥",
                    Description = "Win 5 games in a row" },
        };
    }
}