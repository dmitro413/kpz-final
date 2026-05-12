namespace WpfLibrary.models
{
    public class Board
    {
        public int Rows { get; }
        public int Columns { get; }
        public int MineCount { get; }
        public Cell[,] Cells { get; }

        public int FlagCount { get; private set; }
        public int RemainingMines => MineCount - FlagCount;

        public Board(int rows, int columns, int mineCount)
        {
            Rows = rows;
            Columns = columns;
            MineCount = mineCount;
            Cells = new Cell[rows, columns];

            InitializeCells();
        }

        private void InitializeCells()
        {
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Columns; col++)
                    Cells[row, col] = new Cell(row, col);
        }

        public Cell GetCell(int row, int col) => Cells[row, col];

        public IEnumerable<Cell> GetNeighbors(int row, int col)
        {
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;

                    int newRow = row + dr;
                    int newCol = col + dc;

                    if (IsInBounds(newRow, newCol))
                        yield return Cells[newRow, newCol];
                }
        }
        public IEnumerable<Cell> GetAllCells()
        {
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Columns; col++)
                    yield return Cells[row, col];
        }

        public bool IsInBounds(int row, int col) =>
            row >= 0 && row < Rows && col >= 0 && col < Columns;

        public void IncrementFlagCount() => FlagCount++;
        public void DecrementFlagCount() => FlagCount--;

        public void RecalculateAdjacentMines()
        {
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Columns; col++)
                {
                    if (!GetCell(row, col).IsMine)
                        GetCell(row, col).AdjacentMines = CountAdjacentMines(row, col);
                }
        }

        private int CountAdjacentMines(int row, int col)
        {
            int count = 0;
            foreach (var neighbor in GetNeighbors(row, col))
                if (neighbor.IsMine) count++;
            return count;
        }
    }
}