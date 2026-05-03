using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfLibrary.Commands;
using WpfLibrary.helpers;
using WpfLibrary.models;
using WpfLibrary.services;

namespace WpfLibrary.viewmodels
{
    public class TimeAttackViewModel : BaseViewModel
    {
        private readonly TimeAttackService _taService;
        private readonly ITimerService _timerService;

        private int _timeLeft = TimeAttackService.RoundDuration;
        private bool _isRunning;
        private int _cellSize = 28;

        private System.Windows.Threading.DispatcherTimer? _penaltyTimer;
        private bool _isResetting;
        private string _penaltyFlash = string.Empty;
        public string PenaltyFlash
        {
            get => _penaltyFlash;
            private set => SetProperty(ref _penaltyFlash, value);
        }

        public ObservableCollection<CellViewModel> Cells { get; } = new();

        public int Rows => _taService.Board?.Rows ?? 0;
        public int Columns => _taService.Board?.Columns ?? 0;

        public int Score => _taService.Score;
        public int MinesHit => _taService.MinesHit;

        public int TimeLeft
        {
            get => _timeLeft;
            private set
            {
                SetProperty(ref _timeLeft, value);
                OnPropertyChanged(nameof(FormattedTimeLeft));
                OnPropertyChanged(nameof(IsTimeLow));
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            private set => SetProperty(ref _isRunning, value);
        }

        public int CellSize
        {
            get => _cellSize;
            private set => SetProperty(ref _cellSize, value);
        }

        public string FormattedTimeLeft => TimeFormatter.Format(TimeLeft);

        public bool IsTimeLow => TimeLeft <= 10 && IsRunning;

        private bool _isGameOver;
        public bool IsGameOver
        {
            get => _isGameOver;
            private set => SetProperty(ref _isGameOver, value);
        }

        public string FinalScoreMessage =>
            $"Time's up!  Score: {Score}  |  Mines hit: {MinesHit}";

        public ICommand RevealCellCommand { get; }
        public ICommand ToggleFlagCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand RestartCommand { get; }

        public TimeAttackViewModel(TimeAttackService taService, ITimerService timerService)
        {
            _taService = taService;
            _timerService = timerService;

            _taService.BoardChanged += RefreshCells;
            _taService.BoardReset += OnBoardReset;
            _taService.TimeUp += OnTimeUp;
            _timerService.OnTick += OnTick;

            RevealCellCommand = new RelayCommand<CellViewModel>(OnRevealCell);
            ToggleFlagCommand = new RelayCommand<CellViewModel>(OnToggleFlag);
            StartCommand = new RelayCommand(StartGame);
            RestartCommand = new RelayCommand(ResetToInitial);

            _taService.StartNewGame(Difficulty.Medium);
            BuildCells();
            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(Columns));
        }

        public void ResetToInitial()
        {
            _timerService.Stop();
            _timerService.Reset();
            TimeLeft = TimeAttackService.RoundDuration;
            IsRunning = false;
            IsGameOver = false;
            PenaltyFlash = string.Empty;
            _penaltyTimer?.Stop();

            _taService.StartNewGame(Difficulty.Medium);
            BuildCells();
            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(Columns));
            OnPropertyChanged(nameof(Score));
            OnPropertyChanged(nameof(MinesHit));
        }

        public void StartGame()
        {
            _timerService.Stop();
            _timerService.Reset();

            TimeLeft = TimeAttackService.RoundDuration;
            IsGameOver = false;
            IsRunning = true;

            _taService.StartNewGame(Difficulty.Medium);
            BuildCells();

            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(Columns));
            OnPropertyChanged(nameof(Score));
            OnPropertyChanged(nameof(MinesHit));
            OnPropertyChanged(nameof(FinalScoreMessage));

            _timerService.Start();
        }

        private void OnRevealCell(CellViewModel? cellVm)
        {
            if (cellVm == null || !IsRunning || _isResetting) return;
            _taService.RevealCell(cellVm.Row, cellVm.Column);
            OnPropertyChanged(nameof(Score));
            OnPropertyChanged(nameof(MinesHit));
        }

        private void OnToggleFlag(CellViewModel? cellVm)
        {
            if (cellVm == null || !IsRunning || _isResetting) return;
            _taService.ToggleFlag(cellVm.Row, cellVm.Column);
        }

        private void OnTick(int elapsed)
        {
            if (!IsRunning) return;

            TimeLeft = Math.Max(0, TimeAttackService.RoundDuration - elapsed);
            if (TimeLeft == 0)
            {
                _timerService.Stop();
                _taService.OnTimeUp();
            }
        }

        private void OnTimeUp(int finalScore)
        {
            IsRunning = false;
            IsGameOver = true;
            OnPropertyChanged(nameof(FinalScoreMessage));
        }

        private void OnBoardReset()
        {
            _isResetting = true;
            BuildCells();
            OnPropertyChanged(nameof(Score));
            OnPropertyChanged(nameof(MinesHit));
            ShowPenaltyFlash(_taService.LastPenalty);
            _isResetting = false;
        }

        private void ShowPenaltyFlash(int penalty)
        {
            if (penalty <= 0) return;

            PenaltyFlash = $"-{penalty}";

            _penaltyTimer?.Stop();
            _penaltyTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _penaltyTimer.Tick += (_, __) =>
            {
                PenaltyFlash = string.Empty;
                _penaltyTimer.Stop();
            };
            _penaltyTimer.Start();
        }

        private void BuildCells()
        {
            Cells.Clear();
            foreach (var cell in _taService.Board.GetAllCells())
                Cells.Add(new CellViewModel(cell));
        }

        private void RefreshCells()
        {
            int expectedCount = (_taService.Board?.Rows ?? 0) * (_taService.Board?.Columns ?? 0);
            if (Cells.Count != expectedCount)
            {
                BuildCells();
            }
            else
            {
                foreach (var cell in Cells)
                    cell.Refresh();
            }

            OnPropertyChanged(nameof(Score));
            OnPropertyChanged(nameof(MinesHit));
        }
    }
}