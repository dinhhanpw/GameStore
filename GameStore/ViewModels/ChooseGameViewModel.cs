using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GameStore.ViewModels
{
    class ChooseGameViewModel : BaseViewModel
    {
        private MainViewModel mainView;
        
        public ICommand PuzzleCommand { get; set; }
        public ICommand CaroCommand { get; set; }

        public ChooseGameViewModel(MainViewModel mainViewModel)
        {
            this.mainView = mainViewModel;
            PuzzleCommand = new RelayCommand<Object>(p => true, p => mainView.ContentView = BaseViewModel.GetView("puzzle"));
            CaroCommand = new RelayCommand<Object>(p => true, p => mainView.ContentView = BaseViewModel.GetView("caro"));
        }
    }
}
