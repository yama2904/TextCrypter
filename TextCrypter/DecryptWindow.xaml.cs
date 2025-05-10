using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TextCrypter
{
    /// <summary>
    /// DecryptWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DecryptWindow : MahApps.Metro.Controls.MetroWindow
    {
        public DecryptWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnCopy.Visibility = Visibility.Hidden;
            txtMessage.Visibility = Visibility.Hidden;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // 鍵ファイルアクセサ初期化
            var keyAccessor = new KeyFileAccessor(string.Empty);

            while (true)
            {
                using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
                {
                    dialog.Title = "暗号ファイル選択ダイアログ";
                    dialog.Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*";

                    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }

                    try
                    {
                        string encryptText = File.ReadAllText(dialog.FileName, AppConfigData.ReadConfig().EncryptFileEncoding);
                        txtMessage.Text = RSACrypter.Decrypt(keyAccessor.ReadMyPrivateKey(), encryptText);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("暗号ファイルの復号に失敗しました。選択したファイルに誤りがないか確認してください。", "エラー", MessageBoxButton.OK);
                        continue;
                    }
                    break;
                }
            }

            btnCopy.Visibility = Visibility.Visible;
            txtMessage.Visibility = Visibility.Visible;
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtMessage.Text);
        }
    }
}
