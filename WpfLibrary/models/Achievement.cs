namespace WpfLibrary.models
{
    public enum AchievementCondition
    {
        FirstWin,
        WinWithoutFlags,
        WinUnder30Seconds,
        WinUnder60Seconds,
        Win10Games,
        Win50Games,
        WinHard,
        WinUltraEasy,
        WinNoMistakes,
        Survivor,
    }
    public class Achievement
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = "🏅";
        public AchievementCondition Condition { get; set; }
        public bool IsUnlocked { get; set; }
        public DateTime? UnlockedAt { get; set; }

        public string FormattedDate =>
            UnlockedAt.HasValue ? UnlockedAt.Value.ToString("yyyy-MM-dd") : "—";

        public void Unlock()
        {
            if (IsUnlocked) return;
            IsUnlocked = true;
            UnlockedAt = DateTime.Now;
        }
    }
}