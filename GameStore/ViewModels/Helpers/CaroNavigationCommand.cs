using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.ViewModels.Helpers
{
    public class CaroNavigationCommand : INavigationCommand
    {
        // lưu trữ trạng thái trước đó hoặc kế tiếp
        // tùy theo lệnh đang nằm trong danh sách redoCommand, undoCommand
        private ButtonState buttonState;
        private int idState;
        private CaroGameViewModel caroGame;

        public CaroNavigationCommand(CaroGameViewModel caroGame, ButtonState state, int idState)
        {
            this.caroGame = caroGame;
            this.buttonState = state;
            this.idState = idState;
        }

        private void SwitchState()
        {
            ButtonState temp = caroGame.GetButtonState(buttonState.Row, buttonState.Col);
            caroGame.SetButtonState(buttonState, idState);
            buttonState = temp;
            caroGame.Turn = caroGame.Turn % 2 + 1;
        }

        public void Redo()
        {
            SwitchState();
        }

        public void Undo()
        {
            SwitchState();
        }
    }
}
