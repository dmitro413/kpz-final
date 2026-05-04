using WpfLibrary.models;

namespace WpfLibrary.services
{
    public class TimeAttackService : BaseGameService
    {
        public const int RoundDuration = 60;
        public const int BasePenalty = 10;

        public int Score { get; private set; }
        public int RevealedThisRound { get; private set; }
        public int MinesHit { get; private set; }
        public int LastPenalty { get; private set; }

        public override event Action GameWon;
        public override event Action GameLost;
        public override event Action BoardChanged;
        public event Action BoardReset;
        public event Action<int> TimeUp;

        public TimeAttackService(IMineGenerator mineGenerator) : base(mineGenerator)
        {
        }

        public override void StartNewGame(Difficulty difficulty)
        {
            CurrentDifficulty = difficulty;
            Score = 0;
            RevealedThisRound = 0;
            MinesHit = 0;
            ResetBoard();
            State = GameState.NotStarted;
        }

        public override void RevealCell(int row, int col)
        {
            if (State.IsOver) return;

            var cell = Board.GetCell(row, col);
            if (cell.IsRevealed || cell.IsFlagged) return;

            if (!State.IsActive)
            {
                _mineGenerator.PlaceMines(Board, row, col);
                State = GameState.InProgress;
            }

            if (cell.IsMine)
            {
                int penalty = RevealedThisRound + BasePenalty;
                LastPenalty = penalty;
                Score = Math.Max(0, Score - penalty);
                MinesHit++;
                ResetBoardKeepScore();
                return;
            }

            int before = CountRevealed();
            FloodReveal(row, col);
            int revealed = CountRevealed() - before;
            Score += revealed;
            RevealedThisRound += revealed;

            BoardChanged?.Invoke();
        }

        public void OnTimeUp()
        {
            State = GameState.Won;
            GameWon?.Invoke();
            TimeUp?.Invoke(Score);
        }

        protected override void OnMineHit()
        {
            // Не використовується в TimeAttack — логіка в RevealCell
        }

        protected override void OnSafeReveal()
        {
            // Не використовується в TimeAttack — логіка в RevealCell
        }

        private void ResetBoard()
        {
            Board = new Board(
                CurrentDifficulty.Rows,
                CurrentDifficulty.Columns,
                CurrentDifficulty.MineCount);
        }

        private void ResetBoardKeepScore()
        {
            ResetBoard();
            State = GameState.NotStarted;
            RevealedThisRound = 0;
            BoardReset?.Invoke();
        }

        private int CountRevealed() =>
            Board.GetAllCells().Count(c => c.IsRevealed && !c.IsMine);
    }
}