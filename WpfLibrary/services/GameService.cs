using WpfLibrary.models;

namespace WpfLibrary.services
{
    public enum GameState
    {
        NotStarted,
        InProgress,
        Won,
        Lost
    }

    public class GameService
    {
        private readonly IMineGenerator _mineGenerator;

        public Board Board { get; private set; }
        public GameState State { get; private set; }
        public bool IsFirstMove { get; private set; }

        public event Action OnGameWon;
        public event Action OnGameLost;
        public event Action OnBoardChanged;

        public GameService(IMineGenerator mineGenerator)
        {
            _mineGenerator = mineGenerator;
        }

        public void StartNewGame(Difficulty difficulty)
        {
            Board = new Board(difficulty.Rows, difficulty.Columns, difficulty.MineCount);
            State = GameState.NotStarted;
            IsFirstMove = true;
        }

        public void RevealCell(int row, int col)
        {
            if (State == GameState.Won || State == GameState.Lost) return;

            var cell = Board.GetCell(row, col);
            if (cell.IsRevealed || cell.IsFlagged) return;

            if (IsFirstMove)
            {
                _mineGenerator.PlaceMines(Board, row, col);
                State = GameState.InProgress;
                IsFirstMove = false;
            }

            if (cell.IsMine)
            {
                cell.State = CellState.Revealed;
                State = GameState.Lost;
                RevealAllMines();
                OnGameLost?.Invoke();
                return;
            }

            FloodReveal(row, col);
            OnBoardChanged?.Invoke();

            if (CheckWinCondition())
            {
                State = GameState.Won;
                OnGameWon?.Invoke();
            }
        }

        public void ToggleFlag(int row, int col)
        {
            if (State == GameState.Won || State == GameState.Lost) return;

            var cell = Board.GetCell(row, col);
            if (cell.IsRevealed) return;

            if (cell.IsFlagged)
            {
                cell.State = CellState.Hidden;
                Board.DecrementFlagCount();
            }
            else
            {
                cell.State = CellState.Flagged;
                Board.IncrementFlagCount();
            }

            OnBoardChanged?.Invoke();
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
            for (int row = 0; row < Board.Rows; row++)
                for (int col = 0; col < Board.Columns; col++)
                {
                    var cell = Board.GetCell(row, col);
                    if (!cell.IsMine && !cell.IsRevealed) return false;
                }
            return true;
        }

        private void RevealAllMines()
        {
            for (int row = 0; row < Board.Rows; row++)
                for (int col = 0; col < Board.Columns; col++)
                {
                    var cell = Board.GetCell(row, col);
                    if (cell.IsMine) cell.State = CellState.Revealed;
                }
        }
    }
}