using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GameStore.ViewModels.Helpers
{
    public abstract class CaroState
    {
        protected int row, col;

        public abstract void Dislay(Button button);
    }
}
