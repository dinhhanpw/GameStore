using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.ViewModels.Helpers
{
    public class CaroInvoker : Invoker
    {
        public CaroInvoker()
        {
            undoCommand = new Stack<INavigationCommand>();
            redoCommand = new Stack<INavigationCommand>();
        }

        public override void Redo()
        {
            if (redoCommand.Count == 0) return;

            INavigationCommand command = redoCommand.Pop();
            command.Redo();
            undoCommand.Push(command);
        }

        public override void Undo()
        {
            if (undoCommand.Count == 0) return;

            INavigationCommand command = undoCommand.Pop();
            command.Undo();
            redoCommand.Push(command);
        }

        public void AddUndoCommand(CaroNavigationCommand command)
        {
            undoCommand.Push(command);
        }

        public void AddRedoCommand(CaroNavigationCommand command)
        {
            redoCommand.Push(command);
        }

        public void ClearRedoCommand()
        {
            redoCommand.Clear();
        }
    }
}
