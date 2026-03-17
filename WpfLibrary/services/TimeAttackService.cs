using WpfLibrary.models;

namespace WpfLibrary.services
{
    public class TimeAttackService : IGameService
    {
        private readonly IMineGenerator _mineGenerator;

        public const int RoundDuration = 60;
        public const int BasePenalty = 10;

        public Board Board { get; private set; }
        public GameState State { get; private set; } = GameState.NotStarted;
        public Difficulty CurrentDifficulty { get; private set; }

        public int Score { get; private set; }
        public int RevealedThisRound { get; private set; }
        public int MinesHit { get; private set; }
        public int LastPenalty { get; private set; }

        public event Action GameWon;
        public event Action GameLost;
        public event Action BoardChanged;
        public event Action BoardReset;
        public event Action<int> TimeUp;

        public TimeAttackService(IMineGenerator mineGenerator)
        {
            _mineGenerator = mineGenerator;
        }

        public void StartNewGame(Difficulty difficulty)
        {
            CurrentDifficulty = difficulty;
            Score = 0;
            RevealedThisRound = 0;
            MinesHit = 0;
            ResetBoard();
            State = GameState.NotStarted;
        }

        public void RevealCell(int row, int col)
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

        public void ToggleFlag(int row, int col)
        {
            if (State.IsOver || !State.IsActive) return;
            var cell = Board.GetCell(row, col);
            if (cell.IsRevealed) return;

            if (cell.IsFlagged)
            {
                cell.State = CellState.Hidden;
                Board.DecrementFlagCount();
            }
            else
            {
                if (Board.FlagCount >= Board.MineCount) return;
                cell.State = CellState.Flagged;
                Board.IncrementFlagCount();
            }
            BoardChanged?.Invoke();
        }
        public void OnTimeUp()
        {
            State = GameState.Won;
            GameWon?.Invoke();
            TimeUp?.Invoke(Score);
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

        private void FloodReveal(int row, int col)
        {
            if (!Board.IsInBounds(row, col)) return;
            var cell = Board.GetCell(row, col);
            if (cell.IsRevealed || cell.IsFlagged || cell.IsMine) return;

            cell.State = CellState.Revealed;

            if (cell.AdjacentMines == 0)
            {
                foreach (var neighbor in Board.GetNeighbors(row, col))
                    FloodReveal(neighbor.Row, neighbor.Column);
            }
        }

        private void RevealAllMines()
        {
            foreach (var cell in Board.GetAllCells())
                if (cell.IsMine) cell.State = CellState.Revealed;
        }

        private int CountRevealed() =>
            Board.GetAllCells().Count(c => c.IsRevealed && !c.IsMine);
    }
}