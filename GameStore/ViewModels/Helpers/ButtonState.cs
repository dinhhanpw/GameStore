using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.ViewModels.Helpers
{
    public class ButtonState
    {
        public ICaroState State { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }

        public ButtonState(int row, int col, int idState)
        {
            this.Row = row;
            this.Col = col;
            this.State = ICaroState.GetState(idState);
        }
    }
}
