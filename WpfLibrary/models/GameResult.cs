using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.models
{
    public class GameResult
    {
        public GameResult(DifficultyLevel difficulty, int timeSeconds, int flagsUsed, int winStreak)
        {
            Difficulty = difficulty;
            TimeSeconds = timeSeconds;
            FlagsUsed = flagsUsed;
            WinStreak = winStreak;
        }

        public DifficultyLevel Difficulty { get; set; }
        public int TimeSeconds { get; set; }
        public int FlagsUsed { get; set; }
        public int WinStreak { get; set; }
        public int TotalWins { get; set; }
    }
}
