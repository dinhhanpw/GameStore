using GameStore.Dialogs;
using GameStore.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GameStore.ViewModels
{
    public class CaroGameViewModel : BaseViewModel, IGame
    {
        private int[,] board;
        private Button[,] buttons;
        private int sizeBoard;
        private double widthBoard;
        private double heightBoard;
        private readonly int margin = 3;
        public int Turn { set; get; } = 0;
        private CaroInvoker caroInvoker;

        public WrapPanel BoardGame { get; set; } = new WrapPanel();

        public void InitializeNewGame()
        {
            CaroGameConfigDialog dialog = new CaroGameConfigDialog();

            // gọi hộp thoại để điền kích thước bàn cờ
            if (dialog.ShowDialog() == true)
            {
                sizeBoard = dialog.Size;
            }

            // dọn sạch bàn cờ cũ
            BoardGame.Children.Clear();
            board = new int[sizeBoard, sizeBoard];
            buttons = new Button[sizeBoard, sizeBoard];
            Turn = 1;

            widthBoard = BoardGame.ActualWidth;
            heightBoard = BoardGame.ActualHeight;
            // tính toán chiều rộng, chiều cao của button
            double widthButton = (widthBoard - margin * sizeBoard) / sizeBoard;
            double heightButton = (heightBoard - margin * sizeBoard) / sizeBoard;

            // cấu hình và gắn các button vào bàn cờ
            for (int i = 0; i < sizeBoard; ++i)
            {
                for (int j = 0; j < sizeBoard; ++j)
                {
                    Button button = CreateNewButton(widthButton, heightButton, i, j);

                    buttons[i, j] = button;
                    BoardGame.Children.Add(buttons[i, j]);

                }
            }

            // kích hoạt bàn cờ
            BoardGame.IsEnabled = true;
        }

        public void PauseGame()
        {
            BoardGame.IsEnabled = false;
            BoardGame.Opacity = 0.5;
        }

        public void PlayGame()
        {
            BoardGame.IsEnabled = true;
            BoardGame.Opacity = 1;
        }

        public void Redo()
        {
            caroInvoker.Redo();
            
        }

        public void StopGame()
        {
            PauseGame();
        }

        public void Undo()
        {
            caroInvoker.Undo();        }

        public CaroGameViewModel()
        {
            caroInvoker = new CaroInvoker();
        }



        /// <summary>
        /// tạo và cấu hình cho một button mới
        /// </summary>
        /// <param name="width">chiều rộng button</param>
        /// <param name="height">chiều cao button</param>
        /// <param name="row">hàng của button trên bàn cờ</param>
        /// <param name="col">cột của button trên bàn cờ</param>
        /// <returns></returns>
        private Button CreateNewButton(double width, double height, int row, int col)
        {

            Thickness thickness = new Thickness();
            thickness.Left = thickness.Right = thickness.Top = thickness.Bottom = 1;

            Button button = new Button();
            button.Width = width;
            button.Height = height;
            button.Margin = new Thickness(margin, margin, 0, 0);
            button.BorderThickness = thickness;
            button.Tag = new ButtonState(row, col, 0);
            button.Click += Button_Click;

            return button;
        }

        public void SetButtonState(ButtonState state, int idState)
        {
            board[state.Row, state.Col] = idState;
            Button button = buttons[state.Row, state.Col];
            button.Tag = state;
            state.State.Dislay(button);
        }

        public ButtonState GetButtonState(int row, int col)
        {
            return (ButtonState)buttons[row, col].Tag;
        }

        /// <summary>
        /// sự kiện xảy ra khi nhấn một button trên bàn cờ caro
        /// (chưa xử lí ván đấu hòa)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ButtonState buttonState = button.Tag as ButtonState;
            int i, j;
            i = buttonState.Row;
            j = buttonState.Col;

            // kiểm tra vị trí này đã có cờ của người chơi
            if (board[i, j] == 0)
            {
                ButtonState oldState = new ButtonState(i, j, 0);
                caroInvoker.AddUndoCommand(new CaroNavigationCommand(this, oldState, 0));
                caroInvoker.ClearRedoCommand();

                buttonState.State = ICaroState.GetState(Turn);
                buttonState.State.Dislay(button);
                
                board[i, j] = Turn;
                // kiểm tra điều kiện thắng
                int winner = checkWin(board, i, j);

                if (winner == 0)
                {
                    // nếu chưa có người thắng thì đổi người đánh
                    Turn = Turn % 2 + 1;
                    return;
                }


                // hiển thị người chiến thắng
                if (winner == 1)
                {
                    MessageBox.Show("X won!");
                }

                if (winner == 2)
                {
                    MessageBox.Show("O won!");
                }
                BoardGame.IsEnabled = false;

            }
        }

        /// <summary>
        /// kiểm tra điều kiên thắng của người chơi
        /// </summary>
        /// <param name="arr">ma trận của bàn cờ</param>
        /// <param name="row">hàng của nước đi vừa rồi</param>
        /// <param name="col">cột của nước đi vừa rồi</param>
        /// <returns></returns>
        private int checkWin(int[,] arr, int row, int col)
        {
            if (checkWinOnRow(arr, row, col))
            {
                return arr[row, col];
            }

            if (checkWinOnCol(arr, row, col))
            {
                return arr[row, col];
            }

            if (checkWinOnDiagonal1(arr, row, col))
            {
                return arr[row, col];
            }

            if (checkWinOnDiagonal2(arr, row, col))
            {
                return arr[row, col];
            }

            return 0;
        }


        /// <summary>
        /// kiểm tra điều kiện thắng trên hàng
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool checkWinOnRow(int[,] arr, int row, int col)
        {
            int score = 1;
            int player = arr[row, col];

            // kiểm tra phía bên phải
            for (int i = 1; i <= 4 && row + i < sizeBoard; ++i)
            {
                if (arr[row + i, col] == player)
                {
                    ++score;
                }
                else
                {
                    break;
                }
            }

            // kiểm tra phía bên trái
            for (int i = 1; i <= 4 && row - i >= 0; ++i)
            {
                if (arr[row - i, col] == player)
                {
                    ++score;
                }
                else
                {
                    break;
                }
            }

            // trả về chiến thắng nếu đủ 5 quân cờ giống nhau (chặn trường hợp chiến thắng có 6 quân cờ liên tiếp nhau)
            if (score == 5)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// kiểm tra điều kiện thắng trên cột
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool checkWinOnCol(int[,] arr, int row, int col)
        {
            int score = 1;
            int player = arr[row, col];

            // kiểm tra phía trên
            for (int i = 1; i <= 4 && col + i < sizeBoard; ++i)
            {
                if (arr[row, col + i] == player)
                {
                    ++score;
                }
                else
                {
                    break;
                }
            }

            // kiểm tra phía dưới
            for (int i = 1; i <= 4 && col - i >= 0; ++i)
            {
                if (arr[row, col - i] == player)
                {
                    ++score;
                }
                else
                {
                    break;
                }
            }

            // trả về chiến thắng nếu đủ 5 quân cờ giống nhau (chặn trường hợp chiến thắng có 6 quân cờ liên tiếp nhau)
            if (score == 5)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// kiểm tra điều kiện thắng trên đường chéo thứ nhất
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool checkWinOnDiagonal1(int[,] arr, int row, int col)
        {
            int score = 1;
            int player = arr[row, col];

            // kiểm tra phía phải trên
            for (int i = 1; i <= 4 && row + i < sizeBoard && col + i < sizeBoard; ++i)
            {
                if (arr[row + i, col + i] == player)
                {
                    ++score;
                }
                else
                {
                    break;
                }
            }

            // kiểm tra phía trái dưới
            for (int i = 1; i <= 4 && row - i >= 0 && col - i >= 0; ++i)
            {
                if (arr[row - i, col - i] == player)
                {
                    ++score;
                }
                else
                {
                    break;
                }
            }

            // trả về chiến thắng nếu đủ 5 quân cờ giống nhau (chặn trường hợp chiến thắng có 6 quân cờ liên tiếp nhau)
            if (score == 5)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// kiểm tra điều kiên thắng trên đường chéo thứ hai
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool checkWinOnDiagonal2(int[,] arr, int row, int col)
        {
            int score = 1;
            int player = arr[row, col];

            // kiểm tra phía phải dưới
            for (int i = 1; i <= 4 && row + i < sizeBoard && col - i >= 0; ++i)
            {
                if (arr[row + i, col - i] == player)
                {
                    ++score;
                }
                else
                {
                    break;
                }
            }

            // kiểm tra phía trái trên
            for (int i = 1; i <= 4 && row - i >= 0 && col + i < sizeBoard; ++i)
            {
                if (arr[row - i, col + i] == player)
                {
                    ++score;
                }
                else
                {
                    break;
                }
            }

            // trả về chiến thắng nếu đủ 5 quân cờ giống nhau (chặn trường hợp chiến thắng có 6 quân cờ liên tiếp nhau)
            if (score == 5)
            {
                return true;
            }

            return false;
        }


    }
}
