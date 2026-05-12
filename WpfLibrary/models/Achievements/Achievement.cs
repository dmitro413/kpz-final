namespace WpfLibrary.models
{
    public abstract class Achievement
    {
        public string Id { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Icon { get; init; } = "🏅";

        public bool IsUnlocked { get; private set; }
        public DateTime? UnlockedAt { get; private set; }

        public string FormattedDate =>
            UnlockedAt?.ToString("yyyy-MM-dd") ?? "—";

        public abstract bool CanUnlock(GameResult result);

        public void Unlock(DateTime? unlockedAt = null)
        {
            if (IsUnlocked)
                return;

            IsUnlocked = true;
            UnlockedAt = unlockedAt ?? DateTime.Now;
        }
    }
}