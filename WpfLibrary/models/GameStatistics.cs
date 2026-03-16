using WpfLibrary.models;

namespace WpfLibrary.models
{
    public class GameStatistics
    {
        public int TotalGames { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }
        public int TotalTimeSecs { get; set; }

        public DifficultyStats UltraEasy { get; set; } = new();
        public DifficultyStats Easy { get; set; } = new();
        public DifficultyStats Medium { get; set; } = new();
        public DifficultyStats Hard { get; set; } = new();
        public DifficultyStats Custom { get; set; } = new();

        public double WinRate =>
            TotalGames == 0 ? 0.0 : Math.Round((double)TotalWins / TotalGames * 100, 1);

        public string AverageTime
        {
            get
            {
                if (TotalWins == 0) return "—";
                int avg = TotalTimeSecs / TotalWins;
                return $"{avg / 60:D2}:{avg % 60:D2}";
            }
        }

        public DifficultyStats GetStats(DifficultyLevel level) => level switch
        {
            DifficultyLevel.UltraEasy => UltraEasy,
            DifficultyLevel.Easy => Easy,
            DifficultyLevel.Medium => Medium,
            DifficultyLevel.Hard => Hard,
            DifficultyLevel.Custom => Custom,
            _ => Easy
        };
    }
    public class DifficultyStats
    {
        public int Games { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int BestTime { get; set; } = int.MaxValue;
        public int TotalTime { get; set; }

        public double WinRate =>
            Games == 0 ? 0.0 : Math.Round((double)Wins / Games * 100, 1);

        public string FormattedBestTime =>
            BestTime == int.MaxValue ? "—" : $"{BestTime / 60:D2}:{BestTime % 60:D2}";

        public string FormattedAvgTime
        {
            get
            {
                if (Wins == 0) return "—";
                int avg = TotalTime / Wins;
                return $"{avg / 60:D2}:{avg % 60:D2}";
            }
        }

        public void RecordWin(int timeSeconds)
        {
            Games++;
            Wins++;
            TotalTime += timeSeconds;
            if (timeSeconds < BestTime)
                BestTime = timeSeconds;
        }

        public void RecordLoss()
        {
            Games++;
            Losses++;
        }
    }
}