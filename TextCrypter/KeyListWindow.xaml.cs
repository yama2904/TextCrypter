using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TextCrypter
{
    /// <summary>
    /// KeyListWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class KeyListWindow : MahApps.Metro.Controls.MetroWindow
    {
        public KeyListWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // 選択された宛先がない場合はメッセージを表示
            if (lstKey.SelectedItems.Count == 0)
            {
                MessageBox.Show("削除する宛先を選択してください。", "確認", MessageBoxButton.OK);
                return;
            }

            // 削除確認メッセージ用に宛先一覧を取得
            StringBuilder owners = new StringBuilder();
            foreach (string owner in lstKey.SelectedItems)
            {
                if (owners.Length > 0)
                {
                    owners.Append(", ");
                }
                owners.Append(owner);
            }

            // 削除確認
            var messageResult = MessageBox.Show($"{owners}の宛先を削除します。よろしいですか？", "確認", MessageBoxButton.YesNo);
            if (messageResult != MessageBoxResult.Yes)
            {
                return;
            }

            // 選択された宛先を削除
            foreach (string owner in lstKey.SelectedItems)
            {
                new KeyFileAccessor(owner).DeleteMyKey(true);
            }

            LoadWindow();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // App.config読み込み
            AppConfigData config = AppConfigData.ReadConfig();

            string exsampleKey = config.PublicKeyFileNameFormat.Replace("{0}", "○○");
            MessageBox.Show($"OKボタンを押下後、LINE等で相手から送付された公開鍵（{exsampleKey}）を選択してください。", "確認", MessageBoxButton.OK);

            while (true)
            {
                using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
                {
                    string pubKeyFilter = config.PublicKeyFileNameFormat.Replace("{0}", "*");

                    dialog.Title = "公開鍵選択ダイアログ";
                    dialog.Filter = $"公開鍵ファイル|{pubKeyFilter}| すべてのファイル(*.*)|*.*";
                    //dialog.InitialDirectory = @"%USERPROFILE%\Downloads";

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // 公開鍵の有効性チェック
                        if (!KeyFileAccessor.IsValidPublicKeyFile(dialog.FileName))
                        {
                            MessageBox.Show("公開鍵ファイルが不正です。", "エラー", MessageBoxButton.OK);
                            continue;
                        }

                        // 公開鍵フォルダへコピー
                        KeyFileAccessor.AddPublicKey(dialog.FileName);

                        break;
                    }
                    break;
                }
            }
            LoadWindow();
        }

        private void LoadWindow()
        {
            lstKey.Items.Clear();
            txtMessage.Visibility = Visibility.Visible;

            // 公開鍵所有者リストを表示
            List<string> owners = KeyFileAccessor.GetPublicKeyOwners();
            foreach (string owner in owners)
            {
                lstKey.Items.Add(owner);
            }

            // 公開鍵が0件の場合はメッセージを表示
            if (lstKey.Items.Count > 0)
            {
                txtMessage.Visibility = Visibility.Hidden;
            }
        }
    }
}
