using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.ViewModels.Helpers
{
    public class PuzzleNavigationCommand : INavigationCommand
    {
        private PuzzleGameViewModel game;
        private int hor;
        private int ver;

        public PuzzleNavigationCommand(PuzzleGameViewModel game, int hor, int ver)
        {
            this.game = game;
            this.ver = ver;
            this.hor = hor;
        }

        public void Redo()
        {
            game.MoveImage(hor, ver);
        }

        public void Undo()
        {
            game.MoveImage(-hor, -ver);
        }
    }
}
