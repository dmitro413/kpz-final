using WpfLibrary.models;

namespace WpfLibrary.services
{
    public interface IGameService
    {
        Board Board { get; }
        GameState State { get; }
        Difficulty CurrentDifficulty { get; }

        event Action GameWon;
        event Action GameLost;
        event Action BoardChanged;

        void StartNewGame(Difficulty difficulty);
        void RevealCell(int row, int col);
        void ToggleFlag(int row, int col);
    }
}