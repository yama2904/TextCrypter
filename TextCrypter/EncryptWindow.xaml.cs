using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TextCrypter
{
    /// <summary>
    /// EncryptWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EncryptWindow : MahApps.Metro.Controls.MetroWindow
    {
        private enum Step
        {
            Input,
            KeySelect,
            Encrypt
        }
        private Step _currentStep = Step.Input;

        public EncryptWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeStep(Step.Input);

            foreach (string name in KeyFileAccessor.GetPublicKeyOwners())
            {
                cmbKey.Items.Add(name);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Step toStep = Step.Input;
            switch (_currentStep)
            {
                case Step.KeySelect:
                    toStep = Step.Input;
                    break;

                case Step.Encrypt:
                    toStep = Step.KeySelect;
                    break;
            }
            ChangeStep(toStep);
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnInputNext.IsEnabled = txtInput.Text.Length > 0;
        }

        private void btnInputNext_Click(object sender, RoutedEventArgs e)
        {
            ChangeStep(Step.KeySelect);
        }

        private void cmbKey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnKeyNext.IsEnabled = cmbKey.SelectedItem != null;
        }

        private void btnKeyNext_Click(object sender, RoutedEventArgs e)
        {
            ChangeStep(Step.Encrypt);
        }

        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            using (var saveDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveDialog.Title = "暗号ファイル保存ダイアログ";
                saveDialog.Filter = "テキストファイル(*.txt)|*.txt|すべてのファイル(*.*)|*.*";
                saveDialog.FileName = $"encrypt_message_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                string text = RSACrypter.Encrypt(new KeyFileAccessor(cmbKey.SelectedItem.ToString()).ReadMyKey(KeyFileAccessor.KeyType.Public), txtInput.Text);
                File.WriteAllText(saveDialog.FileName, text, AppConfigData.ReadConfig().EncryptFileEncoding);
                MessageBox.Show("暗号ファイルの保存が完了しました。\nOKボタンを押下後、保存した暗号ファイルをLINE等のメッセージで相手に送付してください。", "確認", MessageBoxButton.OK);

                // エクスプローラーで保存先を開く
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveDialog.FileName}\"");
            }

            Close();
        }

        /// <summary>
        /// ステップを変更する
        /// </summary>
        /// <param name="step">変更先ステップ</param>
        private void ChangeStep(Step step)
        {
            if (step == Step.Input)
            {
                // Back/Next
                btnBack.IsEnabled = false;

                // Input
                txtInputDescription.Visibility = Visibility.Visible;
                txtInputLabel.Visibility = Visibility.Hidden;
                txtInput.Visibility = Visibility.Visible;
                txtInput.IsEnabled = true;
                btnInputNext.IsEnabled = txtInput.Text.Length > 0;

                // KeySelect
                txtKeyDescription.Visibility = Visibility.Hidden;
                txtKeyLabel.Visibility = Visibility.Hidden;
                cmbKey.Visibility = Visibility.Hidden;
                btnKeyNext.Visibility = Visibility.Hidden;

                // Encrypt
                txtOkDescription.Visibility = Visibility.Hidden;
                btnEncrypt.Visibility = Visibility.Hidden;
            }

            if (step == Step.KeySelect)
            {
                // Back/Next
                btnBack.IsEnabled = true;

                // Input
                txtInputDescription.Visibility = Visibility.Hidden;
                txtInputLabel.Visibility = Visibility.Visible;
                txtInput.IsEnabled = false;
                btnInputNext.IsEnabled = false;

                // KeySelect
                txtKeyDescription.Visibility = Visibility.Visible;
                txtKeyLabel.Visibility = Visibility.Hidden;
                cmbKey.Visibility = Visibility.Visible;
                cmbKey.IsEnabled = true;
                btnKeyNext.Visibility = Visibility.Visible;
                btnKeyNext.IsEnabled = cmbKey.SelectedItem != null;

                // Encrypt
                txtOkDescription.Visibility = Visibility.Hidden;
                btnEncrypt.Visibility = Visibility.Hidden;
            }

            if (step == Step.Encrypt)
            {
                // Back/Next
                btnBack.IsEnabled = true;

                // KeySelect
                txtKeyDescription.Visibility = Visibility.Hidden;
                txtKeyLabel.Visibility = Visibility.Visible;
                cmbKey.IsEnabled = false;
                btnKeyNext.IsEnabled = false;

                // Encrypt
                txtOkDescription.Visibility = Visibility.Visible;
                btnEncrypt.Visibility = Visibility.Visible;
            }

            _currentStep = step;
        }
    }
}
