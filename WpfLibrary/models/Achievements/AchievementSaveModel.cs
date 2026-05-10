using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.models.Achievements
{
    public class AchievementSaveModel
    {
        public string Id { get; set; } = string.Empty;

        public bool IsUnlocked { get; set; }

        public DateTime? UnlockedAt { get; set; }
    }
}
