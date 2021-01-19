using GameStore.ViewModels.Helpers;
using System.Windows.Input;

namespace GameStore.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private BaseViewModel _ContentView;
        private ChooseGameViewModel defaultView;

        public BaseViewModel ContentView
        {
            get { return _ContentView; }
            set
            {
                SetBindableProperty(ref _ContentView, value);

                if (_ContentView is IGame)
                {
                    Game = (IGame)_ContentView;
                    Game.InitializeNewGame();
                }
                else Game = null;
            }
        }

        public IGame Game { get; set; }

        public ICommand NewGameCommand { get; set; }
        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand UndoCommand { get; set; }
        public ICommand RedoCommand { get; set; }

        public MainViewModel()
        {
            defaultView = new ChooseGameViewModel(this);
            ContentView = defaultView;

            //Game = ContentView;
            NewGameCommand = new RelayCommand<object>(obj => true, obj => Game?.InitializeNewGame());
            PlayCommand = new RelayCommand<object>(obj => true, obj => Game?.PlayGame());
            PauseCommand = new RelayCommand<object>(obj => true, obj => Game?.PauseGame());
            StopCommand = new RelayCommand<object>(obj => true, obj =>
            {
                Game?.StopGame();
                ContentView = defaultView;
            });
            UndoCommand = new RelayCommand<object>(obj => true, obj => Game?.Undo());
            RedoCommand = new RelayCommand<object>(obj => true, obj => Game?.Redo());

        }

    }
}
