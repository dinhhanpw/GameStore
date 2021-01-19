using GameStore.Dialogs;
using GameStore.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GameStore.ViewModels
{
    public class PuzzleGameViewModel : BaseViewModel, IGame
    {
        String imageSource;

        // bitmap của hình ảnh được chọn
        private BitmapImage bitmapSource;
        // đồng hồ đếm thời gian chơi
        private DispatcherTimer timer = new DispatcherTimer();
        private PuzzleInvoker puzzleInvoker;
        // thời gian tối đa, thời gian còn lại của lần chơi hiên tại (phụ thuộc vào độ khó đã chọn)
        private TimeSpan remainTime;
        // số lượng hàng( số lượng hàng cột bằng nhau) của các mảnh ghép
        private int sizeBoard = 0;
        // xác định người chơi đang chơi game hay không
        private bool isPlaying = false;
        // xác định vị trí (hàng, cột) hiện tại của hình ảnh cuối cùng
        private int rowLastImage, colLastImage;
        // kích thước của những mảnh ghép được hiển thị trên màn hình người chơi
        private int widthDisplayedImage, heightDisplayedImage;
        // kích thước mảnh ghép được cắt từ hình gốc (hình người chơi đã chọn)
        private int widthCroppedImage, heightCroppedImage;
        // mảng 2 chiều, lưu vị trí những mảnh ghép
        private Image[,] images;
        // lưu thông tin mảnh ghép cuối cùng của hình được chọn ( mảnh ghép này được dùng để di chuyển)
        private Image lastImage = new Image();

        public Canvas BoardGame { get; set; } = new Canvas();
        public Image CurrentImage { get; set; } = new Image();
        private String _TimeString;
        public String TimeString
        {
            get { return _TimeString; }
            set
            {
                SetBindableProperty(ref _TimeString, value);
            }
        }
        private Image sampleImage = new Image();
        public ICommand MoveCommand { get; set; }
        public PuzzleGameViewModel()
        {
            puzzleInvoker = new PuzzleInvoker();
            MoveCommand = new RelayCommand<MouseButtonEventArgs>(e => true, e => { MoveImageWithMouse(e); });
            // đặt sự kiện xảy ra sau mỗi nhịp
            timer.Tick += tick_Handle;
            // khoảng thời gian giữa các nhịp
            timer.Interval = TimeSpan.FromSeconds(1);

        }

        public void InitializeNewGame()
        {
            GameConfigDialog dialog = new GameConfigDialog();

            //hiển thị hộp thoại cấu hình game
            if (dialog.ShowDialog() == true)
            {
                // nếu kêt quả trả về OK
                // lấy các thông tin: đường dẫn hình ảnh, kích cỡ bàn chơi, thời lượng trò chơi
                imageSource = dialog.ImagePath;
                sizeBoard = dialog.Size;
                remainTime = dialog.DurationGame;
            }
            else
            {
                // ngược lại, hủy tạo trò chơi mới
                return;
            }

            // tạo bitmap từ đường dẫn nhận được
            bitmapSource = new BitmapImage(new Uri(imageSource, UriKind.Absolute));
            // đặt hình mẫu
            CurrentImage.Source = bitmapSource;
            // khởi tạo mảng 2 chiều các hình ảnh
            images = new Image[sizeBoard, sizeBoard];

            // tính toán kích thước
            getDimension();

            // cắt hình ảnh gốc thành các mảnh tùy theo kích cỡ đã chọn
            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    Int32Rect rect
                        = new Int32Rect(j * widthCroppedImage, i * heightCroppedImage, widthCroppedImage, heightCroppedImage);
                    CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapSource, rect);

                    images[i, j] = createNewImage(i * sizeBoard + j, croppedBitmap);
                }
            }

            // đặt mảnh ghép cuối cùng trong suốt hơn các mảnh ghép khác
            images[sizeBoard - 1, sizeBoard - 1].Opacity = 0.4;
            
            // xáo trộn các mảnh ghép
            ShuffleImages();
            // hiển thị các mảnh ghép lên màn hình
            DisplayImages();
            // đặt trò chơi ở trạng thái đang chơi, đặt thời gian đếm ngược
            isPlaying = true;
            TimeString = String.Format($"{remainTime.Minutes:00}:{remainTime.Seconds:00}");
            timer.Start();


        }

        public void PlayGame()
        {
            // đặt trò chơi ở trạng thái đang chơi
            isPlaying = true;
            // chạy lại đồng hồ
            timer.Start();
        }
        public void PauseGame()
        {
            isPlaying = false;
            // dừng đồng hồ
            timer.Stop();
        }

        public void StopGame()
        {
            // đặt trò chơi ở trạng thái dừng
            isPlaying = false;
            // dừng dồng hồ
            timer.Stop();
        }

        public void Undo()
        {
            puzzleInvoker.Undo();
            
        }

        public void Redo()
        {
            puzzleInvoker.Redo();
        }



        /// <summary>
        /// đặt sự kiện sau mỗi nhịp đồng hồ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tick_Handle(object sender, EventArgs e)
        {
            // kiểm tra đã hết thời gian chưa
            if (remainTime.Minutes != 0 || remainTime.Seconds != 0)
            {
                // trừ đi 1 giây và hiển thị nếu chưa hết thời gian
                remainTime = remainTime.Subtract(TimeSpan.FromSeconds(1));
                TimeString = String.Format($"{remainTime.Minutes:00}:{remainTime.Seconds:00}");
            }
            else
            {
                // nếu hêt thời gian thì ngừng trò chơi và hiển thị thông báo
                StopGame();
                MessageBox.Show("Game over!", "Time Up", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }



        /// <summary>
        /// lấy, tính toán kích thước của phần hình ảnh được cắt và kích thước của mảnh ghép được hiển thị
        /// </summary>
        private void getDimension()
        {
            // tính toán kích thước của mảnh ghép được hiển thi
            widthDisplayedImage = (int)BoardGame.ActualWidth / sizeBoard;
            heightDisplayedImage = (int)BoardGame.ActualHeight / sizeBoard;
            // tính toán kích thước của phần hình ảnh được cắt
            widthCroppedImage = (int)bitmapSource.PixelWidth / sizeBoard;
            heightCroppedImage = (int)bitmapSource.PixelHeight / sizeBoard;
        }

        /// <summary>
        /// tạo một mảnh ghép mới
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="croppedBitmap"></param>
        /// <returns></returns>
        private Image createNewImage(int tag, CroppedBitmap croppedBitmap)
        {
            Image croppedImage = new Image();
            int margin = 2;
            croppedImage.Stretch = Stretch.Fill;
            croppedImage.Width = widthDisplayedImage - 2 * margin;
            croppedImage.Height = heightDisplayedImage - 2 * margin;
            // đặt lề cho mảnh ghép
            croppedImage.Margin = new Thickness(margin);
            croppedImage.Tag = tag;
            croppedImage.Source = croppedBitmap;

            return croppedImage;
        }

        /// <summary>
        /// đặt sự kiện di chuyển hình ảnh khi nhấn chuột trái
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveImageWithMouse(MouseButtonEventArgs e)
        {
            // nếu không ở trạng thái đang chơi thì không thể di chuyển
            if (isPlaying == false) return;

            // lấy tọa độ được chọn trên bàn chơi
            Point lastPoint = e.GetPosition(BoardGame);
            // nếu ngoài phạm vi bàn chơi thì không di chuyển
            if (lastPoint.X > BoardGame.ActualWidth ||
                lastPoint.Y > BoardGame.ActualHeight ||
                lastPoint.X <= 0 || lastPoint.Y <= 0) return;

            // tính toán hàng, cột tương ứng với tọa độ được chọn
            int newRow, newCol;
            newCol = (int)lastPoint.X / widthDisplayedImage;
            newRow = (int)lastPoint.Y / heightDisplayedImage;
            // kiểm tra vị trí mới có hợp lệ
            if (Math.Abs(rowLastImage - newRow) + Math.Abs(colLastImage - newCol) != 1) return;

            int hor = newCol - colLastImage;
            int ver = newRow - rowLastImage;
            // thay đổi vị trí, di chuyển hình ảnh đến vị trí mới
            //Tuple<int, int> changedPosition
            //    = new Tuple<int, int>(newCol - colLastImage, newRow - rowLastImage);
            PuzzleNavigationCommand command = new PuzzleNavigationCommand(this, hor, ver);
            MoveImage(ref rowLastImage, ref colLastImage, hor, ver);



            // làm mới danh sách redo
            //redo.Clear();
            puzzleInvoker.AddUndoCommand(command);
            // thêm lần di chuyển vừa rồi vào danh sách redo
            //undo.Add((changedPosition));
            puzzleInvoker.ClearRedoCommand();

            // kiểm tra đã chiến thắng
            if (CheckWin() == true)
            {
                WinGame();
            }
        }

        /// <summary>
        /// di chuyển mảnh ghép
        /// </summary>
        /// <param name="selectedRow">hàng của mảnh ghép được chọn</param>
        /// <param name="selectedCol">cột của mảnh ghép được chọn</param>
        /// <param name="hor">di chuyển theo hướng ngang</param>
        /// <param name="ver">di chuyển theo hướng dọc</param>
        private void MoveImage(ref int selectedRow, ref int selectedCol, int hor, int ver)
        {
            // tính toán hàng, cột mới cho mảnh ghép
            int newRow = selectedRow + ver;
            int newCol = selectedCol + hor;

            // nếu vượt ra ngoài bàn chơi thì dừng
            if (newRow < 0 || newRow >= sizeBoard || newCol < 0 || newCol >= sizeBoard) return;

            // hoán đổi vị trí 2 mảnh ghép
            SwapImages(ref selectedRow, ref selectedCol, ref newRow, ref newCol);
        }

        public void MoveImage(int hor, int ver)
        {
            MoveImage(ref rowLastImage, ref colLastImage, hor, ver);
        }


        /// <summary>
        /// hiển thị các mảnh ghép của trò chơi lên màn hình
        /// </summary>
        private void DisplayImages()
        {
            // dọn dẹp mảnh ghép cũ
            BoardGame.Children.Clear();

            // hiển thị các mảnh ghép lên bàn chơi tùy theo vị trí
            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    BoardGame.Children.Add(images[i, j]);

                    Canvas.SetLeft(images[i, j], j * (widthDisplayedImage));
                    Canvas.SetTop(images[i, j], i * (heightDisplayedImage));
                }
            }
        }

        /// <summary>
        /// kiểm tra chiến thắng
        /// </summary>
        /// <returns></returns>
        private bool CheckWin()
        {
            // kiểm tra tất cả các mảnh ghép đã vào đúng vị trí hay chưa
            for (int i = 0; i < sizeBoard; i++)
            {
                for (int j = 0; j < sizeBoard; j++)
                {
                    // mảnh ghép đúng vị trí khi Tag = hàng * kích cỡ + cột
                    int tag = (int)images[i, j].Tag;
                    if (tag != (i * sizeBoard + j)) return false;
                }
            }

            return true;
        }

        

        /// <summary>
        /// đặt sự kiện khi chiến thắng trò chơi
        /// </summary>
        public void WinGame()
        {
            // dừng trò chơi
            StopGame();
            // hiển thị thông báo chiến thắng
            MessageBox.Show("You won!");
            // nhập tên để lưu lại điểm số
            //HighScoreDialog dialog
            //    = new HighScoreDialog(durationGame.Minutes, (int)remainTime.TotalSeconds, sizeBoard);
            //dialog.ShowDialog();

        }

        /// <summary>
        /// hoán đổi vị trí 2 mảnh ghép
        /// </summary>
        /// <param name="rowImage1">hàng mảnh ghép thứ 1</param>
        /// <param name="colImage1">cột mảnh ghép thứ 1</param>
        /// <param name="rowImage2">hàng mảnh ghép thứ 2</param>
        /// <param name="colImage2">cột mảnh ghép thứ 2</param>
        private void SwapImages(ref int rowImage1, ref int colImage1, ref int rowImage2, ref int colImage2)
        {
            // hiệu ứng hoạt hình khi di chuyển mảnh ghép 1 đến vị trí của mảnh ghép 2
            DoubleAnimation widthAnimation1
                = new DoubleAnimation(colImage2 * widthDisplayedImage, TimeSpan.FromSeconds(0.75));
            DoubleAnimation heightAnimation1
                = new DoubleAnimation(rowImage2 * heightDisplayedImage, TimeSpan.FromSeconds(0.75));

            images[rowImage1, colImage1].BeginAnimation(Canvas.LeftProperty, widthAnimation1);
            images[rowImage1, colImage1].BeginAnimation(Canvas.TopProperty, heightAnimation1);

            // hiệu ứng hoạt hình khi di chuyển mảnh ghép 2 đến vị trí của mảnh ghép 1
            DoubleAnimation widthAnimation2
                = new DoubleAnimation(colImage1 * widthDisplayedImage, TimeSpan.FromSeconds(0.75));
            DoubleAnimation heightAnimation2
                = new DoubleAnimation(rowImage1 * heightDisplayedImage, TimeSpan.FromSeconds(0.75));

            images[rowImage2, colImage2].BeginAnimation(Canvas.LeftProperty, widthAnimation2);
            images[rowImage2, colImage2].BeginAnimation(Canvas.TopProperty, heightAnimation2);
            // hoán đổi dữ liệu của 2 mảnh ghép trên mảng 2 chiều
            Image tempImage = images[rowImage1, colImage1];
            images[rowImage1, colImage1] = images[rowImage2, colImage2];
            images[rowImage2, colImage2] = tempImage;
            // hoán đổi vị trí hàng, cột của 2 mảnh ghép
            int temp = rowImage1;
            rowImage1 = rowImage2;
            rowImage2 = temp;

            temp = colImage1;
            colImage1 = colImage2;
            colImage2 = temp;
        }

        /// <summary>
        /// xáo trộn các mảnh ghép của trò chơi
        /// </summary>
        private void ShuffleImages()
        {
            // bộ sinh số ngãu nhiên
            Random random = new Random();
            // lưu lại vị trí của mảnh ghép cuối
            rowLastImage = sizeBoard - 1;
            colLastImage = sizeBoard - 1;
            lastImage = images[rowLastImage, colLastImage];
            // số lần xáo trộn
            int times = 30 * sizeBoard;

            // xáo trộn các mảnh ghép
            for (int i = 0; i < times; i++)
            {
                // phát sinh ngẫu nhiên hướng di chuyển ( hướng ngang, dọc thuộc [-1;2))
                // hướng ngang: -1 sang trái, 0 đứng yên, 1 sang phải
                // hướng dọc: -1 lên trên, 0 đứng yên, 1 xuống dưới
                int rngRow = random.Next(-1, 2);
                int rngCol = random.Next(-1, 2);
                // tính vị trí mới cho mảnh ghép cuối
                int newRow = rowLastImage + rngRow;
                int newCol = colLastImage + rngCol;
                // hoán đổi vị trí nếu vị trí mới hợp lệ
                if (newRow >= 0 && newRow < sizeBoard)
                {
                    images[rowLastImage, colLastImage] = images[newRow, colLastImage];
                    images[newRow, colLastImage] = lastImage;
                    rowLastImage = newRow;
                }
                // hoán đổi vị trí nếu vị trí mới hợp lệ
                if (newCol >= 0 && newCol < sizeBoard)
                {
                    images[rowLastImage, colLastImage] = images[rowLastImage, newCol];
                    images[rowLastImage, newCol] = lastImage;
                    colLastImage = newCol;
                }

            }
        }
    }
}
