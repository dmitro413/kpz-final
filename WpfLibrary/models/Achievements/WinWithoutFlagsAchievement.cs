using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.models.Achievements
{
    public class WinWithoutFlagsAchievement : Achievement
    {
        public WinWithoutFlagsAchievement()
        {
            Id = "no_flags";
            Title = "Flagless";
            Description = "Win without placing any flags";
            Icon = "🚫";
        }

        public override bool CanUnlock(GameResult result)
        {
            return result.FlagsUsed == 0;
        }
    }
}
