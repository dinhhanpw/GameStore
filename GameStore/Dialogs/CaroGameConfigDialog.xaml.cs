using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameStore.Dialogs
{
    /// <summary>
    /// Interaction logic for CaroGameConfigDialog.xaml
    /// </summary>
    public partial class CaroGameConfigDialog : Window
    {
        public CaroGameConfigDialog()
        {
            InitializeComponent();
        }

        public int Size { set; get; }

        /// <summary>
        /// kiểm tra giá trị có hơp lệ hay không trước khi nhận giá trị làm kích thước bàn cờ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Size = int.Parse(sizeTextBox.Text);
                if (Size < 6 || Size > 10)
                {
                    MessageBox.Show("Invalid value");
                }
                else
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid value");
            }

        }
    }
}
