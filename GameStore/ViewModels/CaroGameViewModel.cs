using GameStore.Dialogs;
using GameStore.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GameStore.ViewModels
{
    class CaroGameViewModel : BaseViewModel, IGame
    {
       

        private int[,] board;
        private Button[,] buttons;
        private int sizeBoard;
        private double widthBoard;
        private double heightBoard;
        private readonly int margin = 3;
        private int turn = 0;
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
            turn = 1;

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
            throw new NotImplementedException();
        }

        public void PlayGame()
        {
            throw new NotImplementedException();
        }

        public void Redo()
        {
            throw new NotImplementedException();
        }

        public void StopGame()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public CaroGameViewModel()
        {
            
        }

        /// <summary>
        /// khởi tạo một bàn cờ mới khi chọn New Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewGame_Click(object sender, RoutedEventArgs e)
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
            turn = 1;

            
            // tính toán chiều rộng, chiều cao của button
            double widthButton = (widthBoard - 4 * sizeBoard) / sizeBoard;
            double heightButton = (heightBoard - 4 * sizeBoard) / sizeBoard;

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
            button.Tag = new Tuple<int, int>(row, col);
            button.Click += Button_Click;

            return button;
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
            Tuple<int, int> pos = button.Tag as Tuple<int, int>;
            int i, j;
            i = pos.Item1;
            j = pos.Item2;

            // kiểm tra vị trí này đã có cờ của người chơi
            if (board[i, j] == 0)
            {
                if (turn == 1)
                {
                    button.Content = "X";
                }
                else
                {
                    button.Content = "O";
                }

                board[i, j] = turn;
                // kiểm tra điều kiện thắng
                int winner = checkWin(board, i, j);

                if (winner == 0)
                {
                    // nếu chưa có người thắng thì đổi người đánh
                    turn = turn % 2 + 1;
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
