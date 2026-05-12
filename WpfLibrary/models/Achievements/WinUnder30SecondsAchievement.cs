using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.models.Achievements
{
    public class WinUnder30SecondsAchievement : Achievement
    {
        public WinUnder30SecondsAchievement() 
        {
            Id = "speed_30";
            Title = "Speed Demon";
            Description = "Win in under 30 seconds";
            Icon = "⚡";
        }
        public override bool CanUnlock(GameResult result)
        {
            return result.TimeSeconds < 30;
        }
    }
}
