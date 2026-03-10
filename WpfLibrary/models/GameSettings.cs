namespace WpfLibrary.models
{
    public class GameSettings
    {
        public DifficultyLevel SelectedDifficulty { get; set; }
        public int CustomRows { get; set; }
        public int CustomColumns { get; set; }
        public int CustomMineCount { get; set; }
        public string Theme { get; set; }
        public int CellSize { get; set; }

        public GameSettings()
        {
            SelectedDifficulty = DifficultyLevel.Easy;
            CustomRows = 10;
            CustomColumns = 10;
            CustomMineCount = 15;
            Theme = "Light";
            CellSize = 32;
        }

        public Difficulty GetDifficulty() => SelectedDifficulty switch
        {
            DifficultyLevel.Easy => Difficulty.Easy,
            DifficultyLevel.Medium => Difficulty.Medium,
            DifficultyLevel.Hard => Difficulty.Hard,
            DifficultyLevel.Custom => Difficulty.CreateCustom(CustomRows, CustomColumns, CustomMineCount),
            _ => Difficulty.Easy
        };
    }
}
