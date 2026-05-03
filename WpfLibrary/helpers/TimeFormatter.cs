using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary.helpers
{
    public static class TimeFormatter
    {
        public static string Format(int totalSeconds)
            => $"{totalSeconds / 60:D2}:{totalSeconds % 60:D2}";

        public static string FormatOrDash(int totalSeconds, bool hasValue)
            => hasValue ? Format(totalSeconds) : "—";
    }
}
