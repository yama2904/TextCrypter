using System.Windows;

namespace TextCrypter
{
    /// <summary>
    /// ModeSelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ModeSelectWindow : MahApps.Metro.Controls.MetroWindow
    {
        public ModeSelectWindow()
        {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            new EncryptWindow().ShowDialog();
        }

        private void btnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            new DecryptWindow().ShowDialog();
        }

        private void btnKeyList_Click(object sender, RoutedEventArgs e)
        {
            new KeyListWindow().ShowDialog();
        }

        private void btnSetup_Click(object sender, RoutedEventArgs e)
        {
            new GenerateKeyWindow().ShowDialog();
        }
    }
}
