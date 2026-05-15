using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using WpfLibrary.Commands;
using WpfLibrary.helpers;
using WpfLibrary.models;
using WpfLibrary.services;

namespace WpfLibrary.viewmodels
{
    public class TimeAttackViewModel : BaseViewModel
    {
        private const Difficulty DefaultDifficulty = Difficulty.Medium;
        private static readonly TimeSpan PenaltyFlashDuration = TimeSpan.FromSeconds(2);

        private readonly TimeAttackService _taService;
        private readonly ITimerService _timerService;

        private int _timeLeft = TimeAttackService.RoundDuration;
        private bool _isRunning;
        private int _cellSize = 28;

        private DispatcherTimer? _penaltyTimer;
        private bool _isResetting;
        private string _penaltyFlash = string.Empty;

        private bool _isGameOver;

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

            _penaltyTimer = CreatePenaltyTimer();

            InitializeRound();
        }

        public void ResetToInitial()
        {
            InitializeRound();
        }

        public void StartGame()
        {
            InitializeRound();
            IsRunning = true;
            _timerService.Start();
        }

        private void InitializeRound()
        {
            StopRoundTimer();
            ResetRoundState();

            _taService.StartNewGame(DefaultDifficulty);
            RebuildCells();

            NotifyBoardDimensionsChanged();
            NotifyScoreChanged();
            OnPropertyChanged(nameof(FinalScoreMessage));
        }

        private void ResetRoundState()
        {
            TimeLeft = TimeAttackService.RoundDuration;
            IsRunning = false;
            IsGameOver = false;
            PenaltyFlash = string.Empty;
            StopPenaltyTimer();
        }

        private void StopRoundTimer()
        {
            _timerService.Stop();
            _timerService.Reset();
        }

        private void OnRevealCell(CellViewModel? cellVm)
        {
            if (cellVm == null || !IsRunning || _isResetting) return;

            _taService.RevealCell(cellVm.Row, cellVm.Column);
            NotifyScoreChanged();
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

        private void OnTimeUp(int _)
        {
            IsRunning = false;
            IsGameOver = true;
            OnPropertyChanged(nameof(FinalScoreMessage));
        }

        private void OnBoardReset()
        {
            _isResetting = true;

            try
            {
                RebuildCells();
                NotifyScoreChanged();
                ShowPenaltyFlash(_taService.LastPenalty);
            }
            finally
            {
                _isResetting = false;
            }
        }

        private DispatcherTimer CreatePenaltyTimer()
        {
            var timer = new DispatcherTimer
            {
                Interval = PenaltyFlashDuration
            };

            timer.Tick += OnPenaltyTimerTick;
            return timer;
        }

        private void OnPenaltyTimerTick(object? sender, EventArgs e)
        {
            PenaltyFlash = string.Empty;
            StopPenaltyTimer();
        }

        private void ShowPenaltyFlash(int penalty)
        {
            if (penalty <= 0) return;

            PenaltyFlash = $"-{penalty}";
            _penaltyTimer ??= CreatePenaltyTimer();

            _penaltyTimer.Stop();
            _penaltyTimer.Start();
        }

        private void StopPenaltyTimer()
        {
            _penaltyTimer?.Stop();
        }

        private void RebuildCells()
        {
            Cells.Clear();

            var board = _taService.Board;
            if (board == null) return;

            foreach (var cell in board.GetAllCells())
            {
                Cells.Add(new CellViewModel(cell));
            }
        }

        private void RefreshCells()
        {
            var board = _taService.Board;
            if (board == null)
            {
                Cells.Clear();
                NotifyScoreChanged();
                return;
            }

            int expectedCount = board.Rows * board.Columns;

            if (Cells.Count != expectedCount)
            {
                RebuildCells();
            }
            else
            {
                foreach (var cell in Cells)
                {
                    cell.Refresh();
                }
            }

            NotifyScoreChanged();
        }

        private void NotifyBoardDimensionsChanged()
        {
            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(Columns));
        }

        private void NotifyScoreChanged()
        {
            OnPropertyChanged(nameof(Score));
            OnPropertyChanged(nameof(MinesHit));
        }
    }
}