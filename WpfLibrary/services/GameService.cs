using WpfLibrary.models;

namespace WpfLibrary.services
{
    public class GameService : BaseGameService
    {
        public override event Action GameWon;
        public override event Action GameLost;
        public override event Action BoardChanged;

        public GameService(IMineGenerator mineGenerator) : base(mineGenerator)
        {
        }

        public override void StartNewGame(Difficulty difficulty)
        {
            CurrentDifficulty = difficulty;
            Board = new Board(difficulty.Rows, difficulty.Columns, difficulty.MineCount);
            State = GameState.NotStarted;
        }

        protected override void OnMineHit()
        {
            State = GameState.Lost;
            RevealAllMines();
            BoardChanged?.Invoke();
            GameLost?.Invoke();
        }

        protected override void OnSafeReveal()
        {
            BoardChanged?.Invoke();

            if (CheckWinCondition())
            {
                State = GameState.Won;
                GameWon?.Invoke();
            }
        }
    }
}