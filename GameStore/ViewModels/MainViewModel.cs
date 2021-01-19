using GameStore.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace GameStore.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        public PuzzleGameViewModel ContentView
        {
            get; set;
        } = new PuzzleGameViewModel();

        public IGame Game { get; set; }

        public ICommand NewGameCommand { get; set; }
        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand UndoCommand { get; set; }
        public ICommand RedoCommand { get; set; }

        public MainViewModel()
        {
            Game = ContentView;
            NewGameCommand = new RelayCommand<object>(obj => true, obj => Game.InitializeNewGame());
            PlayCommand = new RelayCommand<object>(obj => true, obj => Game.PlayGame());
            PauseCommand = new RelayCommand<object>(obj => true, obj => Game.PauseGame());
            StopCommand = new RelayCommand<object>(obj => true, obj => Game.StopGame());
            UndoCommand = new RelayCommand<object>(obj => true, obj => Game.Undo());
            RedoCommand = new RelayCommand<object>(obj => true, obj => Game.Redo());
            
        }
    }
}
