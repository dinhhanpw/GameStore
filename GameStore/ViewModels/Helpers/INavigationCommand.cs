using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.ViewModels.Helpers
{
    public interface INavigationCommand
    {
        public void Undo();
        public void Redo();
    }
}
