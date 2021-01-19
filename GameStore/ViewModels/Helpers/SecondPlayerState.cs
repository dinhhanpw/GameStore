using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GameStore.ViewModels.Helpers
{
    public class SecondPlayerState : ICaroState
    {
        public void Dislay(Button button)
        {
            button.Content = "O";
        }
    }
}
