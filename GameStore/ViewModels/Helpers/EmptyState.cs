using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GameStore.ViewModels.Helpers
{
    class EmptyState : ICaroState
    {
        public void Dislay(Button button)
        {
            button.Content = "";
        }
    }
}
