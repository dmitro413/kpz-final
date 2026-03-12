using System.Windows.Input;
using WpfLibrary.Commands;
using WpfLibrary.models;
using WpfLibrary.ViewModels;

namespace WpfLibrary.viewmodels
{
    public class CellViewModel : BaseViewModel
    {
        private readonly Cell _cell;

        public int Row => _cell.Row;
        public int Column => _cell.Column;
        public bool IsMine => _cell.IsMine;
        public bool IsRevealed => _cell.IsRevealed;
        public bool IsFlagged => _cell.IsFlagged;
        public bool IsHidden => !_cell.IsRevealed && !_cell.IsFlagged;
        public int AdjacentMines => _cell.AdjacentMines;
        public bool ShowNumber => IsRevealed && !IsMine && AdjacentMines > 0;

        public ICommand RevealCommand { get; }
        public ICommand ToggleFlagCommand { get; }

        public CellViewModel(Cell cell, RelayCommand revealCommand, ICommand toggleFlagCommand)
        {
            _cell = cell;
            RevealCommand = revealCommand;
            ToggleFlagCommand = toggleFlagCommand;
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(IsRevealed));
            OnPropertyChanged(nameof(IsFlagged));
            OnPropertyChanged(nameof(IsHidden));
            OnPropertyChanged(nameof(IsMine));
            OnPropertyChanged(nameof(ShowNumber));
        }
    }
}
