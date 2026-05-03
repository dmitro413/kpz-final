using WpfLibrary.helpers;

namespace WpfLibrary.models
{
    public class GameRecord
    {
        public string PlayerName { get; set; }
        public int TimeSeconds { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public DateTime Date { get; set; }

        public string FormattedTime => TimeFormatter.Format(TimeSeconds);
        public string FormattedDate => Date.ToString("yyyy-MM-dd HH:mm");

        public GameRecord() { }

        public GameRecord(string playerName, int timeSeconds, DifficultyLevel difficulty)
        {
            PlayerName = playerName;
            TimeSeconds = timeSeconds;
            Difficulty = difficulty;
            Date = DateTime.Now;
        }
    }
}
