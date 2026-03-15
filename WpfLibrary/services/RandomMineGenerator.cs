using WpfLibrary.models;

namespace WpfLibrary.services
{
    public class RandomMineGenerator : IMineGenerator
    {
        private readonly Random _random = new Random();

        public void PlaceMines(Board board, int safeRow, int safeCol)
        {
            var positions = GetAvailablePositions(board, safeRow, safeCol);
            Shuffle(positions);

            for (int i = 0; i < board.MineCount; i++)
            {
                var (row, col) = positions[i];
                board.GetCell(row, col).IsMine = true;
            }

            CalculateAdjacentMineCounts(board);
        }

        private List<(int row, int col)> GetAvailablePositions(Board board, int safeRow, int safeCol)
        {
            var positions = new List<(int, int)>();

            for (int row = 0; row < board.Rows; row++)
                for (int col = 0; col < board.Columns; col++)
                {
                    bool inSafeZone = Math.Abs(row - safeRow) <= 1 && Math.Abs(col - safeCol) <= 1;
                    if (!inSafeZone)
                        positions.Add((row, col));
                }

            return positions;
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private void CalculateAdjacentMineCounts(Board board)
        {
            for (int row = 0; row < board.Rows; row++)
                for (int col = 0; col < board.Columns; col++)
                {
                    if (!board.GetCell(row, col).IsMine)
                        board.GetCell(row, col).AdjacentMines = CountAdjacentMines(board, row, col);
                }
        }

        private int CountAdjacentMines(Board board, int row, int col)
        {
            int count = 0;
            foreach (var neighbor in board.GetNeighbors(row, col))
                if (neighbor.IsMine) count++;
            return count;
        }
    }
}