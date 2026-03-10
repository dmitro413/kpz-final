namespace WpfLibrary.models
{
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard,
        Custom
    }

    public class Difficulty
    {
        public DifficultyLevel Level { get; }
        public int Rows { get; }
        public int Columns { get; }
        public int MineCount { get; }
        public string DisplayName { get; }

        private Difficulty(DifficultyLevel level, int rows, int columns, int mineCount, string displayName)
        {
            Level = level;
            Rows = rows;
            Columns = columns;
            MineCount = mineCount;
            DisplayName = displayName;
        }

        public static Difficulty Easy => new Difficulty(DifficultyLevel.Easy, 9, 9, 10, "Easy");
        public static Difficulty Medium => new Difficulty(DifficultyLevel.Medium, 16, 16, 40, "Medium");
        public static Difficulty Hard => new Difficulty(DifficultyLevel.Hard, 16, 30, 99, "Hard");

        public static Difficulty CreateCustom(int rows, int columns, int mineCount) =>
            new Difficulty(DifficultyLevel.Custom, rows, columns, mineCount, "Custom");
    }
}