using WpfLibrary.models;

namespace WpfLibrary.services
{
    public class GameService
    {
        private readonly IMineGenerator _mineGenerator;

        public Board Board { get; private set; }
        public GameState State { get; private set; } = GameState.NotStarted;
        public Difficulty CurrentDifficulty { get; private set; }

        public event Action GameWon;
        public event Action GameLost;
        public event Action BoardChanged;

        public GameService(IMineGenerator mineGenerator)
        {
            _mineGenerator = mineGenerator;
        }

        public void StartNewGame(Difficulty difficulty)
        {
            CurrentDifficulty = difficulty;
            Board = new Board(difficulty.Rows, difficulty.Columns, difficulty.MineCount);
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
                cell.State = CellState.Revealed;
                State = GameState.Lost;
                RevealAllMines();
                BoardChanged?.Invoke();
                GameLost?.Invoke();
                return;
            }

            FloodReveal(row, col);
            BoardChanged?.Invoke();

            if (CheckWinCondition())
            {
                State = GameState.Won;
                GameWon?.Invoke();
            }
        }

        public void ToggleFlag(int row, int col)
        {
            if (State.IsOver) return;

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

        private bool CheckWinCondition()
        {
            foreach (var cell in Board.GetAllCells())
                if (!cell.IsMine && !cell.IsRevealed) return false;
            return true;
        }

        private void RevealAllMines()
        {
            foreach (var cell in Board.GetAllCells())
                if (cell.IsMine) cell.State = CellState.Revealed;
        }
    }
}