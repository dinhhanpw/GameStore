using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.ViewModels.Helpers
{
    public abstract class Invoker
    {
        protected Stack<INavigationCommand> undoCommand;
        protected Stack<INavigationCommand> redoCommand;

        public abstract void Undo();
        public abstract void Redo();
    }
}
