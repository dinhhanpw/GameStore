using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GameStore.ViewModels.Helpers
{
    public interface ICaroState
    {
        private static ICaroState[] caroStates = new ICaroState[] { new EmptyState(), new FirstPlayerState(), new SecondPlayerState() };
        
        public static ICaroState GetState(int id)
        {
            return caroStates[id];
        }

        public void Dislay(Button button);
    }
}
