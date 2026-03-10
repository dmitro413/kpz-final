namespace WpfLibrary.models
{
    public enum CellState
    {
        Hidden,
        Revealed,
        Flagged
    }

    public class Cell
    {
        public int Row { get; }
        public int Column { get; }
        public bool IsMine { get; set; }
        public CellState State { get; set; }
        public int AdjacentMines { get; set; }

        public bool IsRevealed => State == CellState.Revealed;
        public bool IsFlagged => State == CellState.Flagged;

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
            State = CellState.Hidden;
        }
    }
}
