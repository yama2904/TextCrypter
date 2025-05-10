using System.Windows;

namespace TextCrypter
{
    /// <summary>
    /// GenerateKeyWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class GenerateKeyWindow : MahApps.Metro.Controls.MetroWindow
    {
        public GenerateKeyWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblError.Visibility = Visibility.Hidden;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // 名前入力チェック
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                lblError.Visibility = Visibility.Visible;
                return;
            }

            // 鍵ファイルアクセサ初期化
            var keyAccessor = new KeyFileAccessor(txtName.Text.Trim());

            // 鍵ファイル存在チェック
            if (keyAccessor.ExistsMyKey())
            {
                var messageResult = MessageBox.Show("既に初期セットアップ済みです。上書きしてもよろしいですか？", "確認", MessageBoxButton.YesNo);
                if (messageResult != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            // 鍵ペア生成
            keyAccessor.GenerateMyKey();

            // 生成完了
            MessageBox.Show("初期セットアップが完了しました。\nOKボタンを押下後、表示されたファイルをLINE等のメッセージで相手に送付してください。", "確認", MessageBoxButton.OK);

            // エクスプローラーで公開鍵保存先を開く
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{keyAccessor.PublicKeyFile}\"");

            Close();
        }
    }
}
