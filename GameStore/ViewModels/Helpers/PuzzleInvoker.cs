using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.ViewModels.Helpers
{
    class PuzzleInvoker : Invoker
    {
        public PuzzleInvoker()
        {
            undoCommand = new Stack<INavigationCommand>();
            redoCommand = new Stack<INavigationCommand>();
        }
        public override void Redo()
        {
            if (redoCommand.Count <= 0) return;
            INavigationCommand command = redoCommand.Pop();
            undoCommand.Push(command);
            command.Redo();
            
        }

        public override void Undo()
        {
            // kiểm tra danh sách những lần di chuyển trước đó
            if (undoCommand.Count <= 0) return;

            INavigationCommand command = undoCommand.Pop();
            redoCommand.Push(command);
            command.Undo();

        }

        public void AddUndoCommand(PuzzleNavigationCommand command)
        {
            undoCommand.Push(command);
        }

        public void AddRedoCommand(PuzzleNavigationCommand command)
        {
            redoCommand.Push(command);
        }

        public void ClearRedoCommand()
        {
            redoCommand.Clear();
        }
    }
}
