using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfLibrary.Commands;
using WpfLibrary.models;
using WpfLibrary.services;
namespace WpfLibrary.viewmodels
{
    public class GameViewModel : BaseViewModel
    {
        private readonly GameService _gameService;
        private readonly TimerService _timerService;

        private int _elapsedSeconds;
        private string _statusMessage = "Click any cell to start!";
        private string _faceEmoji = "🙂";

        public ObservableCollection<CellViewModel> Cells { get; } = new();

        public int Rows => _gameService.Board?.Rows ?? 0;
        public int Columns => _gameService.Board?.Columns ?? 0;
        public int RemainingMines => _gameService.Board?.RemainingMines ?? 0;

        public int ElapsedSeconds
        {
            get => _elapsedSeconds;
            private set { SetProperty(ref _elapsedSeconds, value); OnPropertyChanged(nameof(FormattedTime)); }
        }

        public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }
        public string FaceEmoji { get => _faceEmoji; private set => SetProperty(ref _faceEmoji, value); }
        public string FormattedTime => $"{ElapsedSeconds / 60:D2}:{ElapsedSeconds % 60:D2}";

        public ICommand RevealCellCommand { get; }
        public ICommand ToggleFlagCommand { get; }
        public ICommand NewGameCommand { get; }

        public event Action<int>? GameWon;

        public GameViewModel(GameService gameService, TimerService timerService)
        {
            _gameService = gameService;
            _timerService = timerService;

            _gameService.GameWon += OnGameWon;
            _gameService.GameLost += OnGameLost;
            _gameService.BoardChanged += RefreshAllCells;
            _timerService.OnTick += seconds => ElapsedSeconds = seconds;

            RevealCellCommand = new RelayCommand<CellViewModel>(OnRevealCell);
            ToggleFlagCommand = new RelayCommand<CellViewModel>(OnToggleFlag);
            NewGameCommand = new RelayCommand(OnNewGame);
        }

        public void LoadGame(Difficulty difficulty)
        {
            _timerService.Reset();
            _gameService.StartNewGame(difficulty);
            BuildCellGrid();
            FaceEmoji = "🙂";
            StatusMessage = "Click any cell to start!";
            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(Columns));
            OnPropertyChanged(nameof(RemainingMines));
        }

        private void OnRevealCell(CellViewModel? cellVm)
        {
            if (cellVm == null || _gameService.State.IsOver) return;

            bool wasNotStarted = !_gameService.State.IsActive;
            _gameService.RevealCell(cellVm.Row, cellVm.Column);

            if (wasNotStarted && _gameService.State.IsActive)
                _timerService.Start();

            OnPropertyChanged(nameof(RemainingMines));
        }

        private void OnToggleFlag(CellViewModel? cellVm)
        {
            if (cellVm == null || _gameService.State.IsOver) return;
            _gameService.ToggleFlag(cellVm.Row, cellVm.Column);
            OnPropertyChanged(nameof(RemainingMines));
        }

        private void OnNewGame() => LoadGame(_gameService.CurrentDifficulty);

        private void OnGameWon()
        {
            _timerService.Stop();
            FaceEmoji = "😎";
            StatusMessage = $"You won in {FormattedTime}!";
            GameWon?.Invoke(ElapsedSeconds);
        }

        private void OnGameLost()
        {
            _timerService.Stop();
            FaceEmoji = "😵";
            StatusMessage = "Game over! Boom 💥";
        }

        private void BuildCellGrid()
        {
            Cells.Clear();
            foreach (var cell in _gameService.Board.GetAllCells())
                Cells.Add(new CellViewModel(cell));
        }

        private void RefreshAllCells()
        {
            foreach (var cell in Cells)
                cell.Refresh();
        }
    }
}