using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.ViewModels.Helpers
{
    public interface IGame
    {
        public void InitializeNewGame();
        public void PlayGame();
        public void PauseGame();
        public void StopGame();
        public void Undo();
        public void Redo();
    }
}
