using WpfLibrary.helpers;

namespace WpfLibrary.models
{
    public class GameHistoryEntry
    {
        public DifficultyLevel Difficulty { get; set; }
        public bool IsWon { get; set; }
        public int TimeSeconds { get; set; }
        public DateTime PlayedAt { get; set; }

        public string FormattedTime => TimeFormatter.Format(TimeSeconds);

        public string FormattedDate =>
            PlayedAt.ToString("MM/dd HH:mm");

        public string Result => IsWon ? "✅ Win" : "❌ Loss";

        public GameHistoryEntry() { }

        public GameHistoryEntry(DifficultyLevel difficulty, bool isWon, int timeSeconds)
        {
            Difficulty = difficulty;
            IsWon = isWon;
            TimeSeconds = timeSeconds;
            PlayedAt = DateTime.Now;
        }
    }
}