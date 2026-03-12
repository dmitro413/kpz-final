namespace WpfLibrary.models
{
    public enum GamePhase
    {
        NotStarted,
        InProgress,
        Won,
        Lost
    }

    public class GameState
    {
        public GamePhase Phase { get; }

        public bool IsActive => Phase == GamePhase.InProgress;
        public bool IsOver => Phase == GamePhase.Won || Phase == GamePhase.Lost;
        public bool IsWon => Phase == GamePhase.Won;

        private GameState(GamePhase phase)
        {
            Phase = phase;
        }

        public static GameState NotStarted => new GameState(GamePhase.NotStarted);
        public static GameState InProgress => new GameState(GamePhase.InProgress);
        public static GameState Won => new GameState(GamePhase.Won);
        public static GameState Lost => new GameState(GamePhase.Lost);
    }
}