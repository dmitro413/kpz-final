using WpfLibrary.models;

namespace WpfLibrary.services
{
    /// <summary>
    /// Базовий клас для ігрових сервісів, що містить спільну логіку розкриття клітинок.
    /// </summary>
    public abstract class BaseGameService : IGameService
    {
        protected readonly IMineGenerator _mineGenerator;

        public Board Board { get; protected set; }
        public GameState State { get; protected set; } = GameState.NotStarted;
        public Difficulty CurrentDifficulty { get; protected set; }

        public abstract event Action GameWon;
        public abstract event Action GameLost;
        public abstract event Action BoardChanged;

        protected BaseGameService(IMineGenerator mineGenerator)
        {
            _mineGenerator = mineGenerator;
        }

        public abstract void StartNewGame(Difficulty difficulty);

        public virtual void RevealCell(int row, int col)
        {
            if (State.IsOver) return;

            var cell = Board.GetCell(row, col);
            if (cell.IsRevealed || cell.IsFlagged) return;

            if (!State.IsActive)
            {
                _mineGenerator.PlaceMines(Board, row, col);
                OnFirstReveal();
            }

            if (cell.IsMine)
            {
                cell.State = CellState.Revealed;
                OnMineHit();
                return;
            }

            FloodReveal(row, col);
            OnSafeReveal();
        }

        /// <summary>
        /// Викликається при першому розкритті (після розміщення мін).
        /// </summary>
        protected virtual void OnFirstReveal()
        {
            State = GameState.InProgress;
        }

        /// <summary>
        /// Викликається, коли гравець натискає на міну.
        /// </summary>
        protected abstract void OnMineHit();

        /// <summary>
        /// Викликається після безпечного розкриття клітинки(ок).
        /// </summary>
        protected abstract void OnSafeReveal();

        public virtual void ToggleFlag(int row, int col)
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

        /// <summary>
        /// Рекурсивне розкриття порожніх сусідніх клітинок (Flood Fill).
        /// </summary>
        protected void FloodReveal(int row, int col)
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

        /// <summary>
        /// Показує всі міни на полі після програшу.
        /// </summary>
        protected void RevealAllMines()
        {
            foreach (var cell in Board.GetAllCells())
                if (cell.IsMine) cell.State = CellState.Revealed;
        }

        /// <summary>
        /// Перевіряє, чи всі безпечні клітинки розкрито.
        /// </summary>
        protected bool CheckWinCondition()
        {
            foreach (var cell in Board.GetAllCells())
                if (!cell.IsMine && !cell.IsRevealed) return false;
            return true;
        }
    }
}