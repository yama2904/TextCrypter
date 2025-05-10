using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace TextCrypter
{
    /// <summary>
    /// App.configの設定情報を管理するクラス
    /// </summary>
    public class AppConfigData
    {
        /// <summary>
        /// 秘密鍵格納先ディレクトリ
        /// </summary>
        public string PrivateKeyDirectory { get; private set; }

        /// <summary>
        /// 秘密鍵ファイル名
        /// </summary>
        public string PrivateKeyFileName { get; private set; }

        /// <summary>
        /// 秘密鍵ファイルエンコーディング
        /// </summary>
        public Encoding PrivateKeyEncoding { get; private set; }

        /// <summary>
        /// 公開鍵格納先ディレクトリ
        /// </summary>
        public string PublicKeyDirectory { get; private set; }

        /// <summary>
        /// 公開鍵ファイル名フォーマット
        /// </summary>
        public string PublicKeyFileNameFormat { get; private set; }

        /// <summary>
        /// 公開鍵ファイルエンコーディング
        /// </summary>
        public Encoding PublicKeyEncoding { get; private set; }

        /// <summary>
        /// キーサイズ
        /// </summary>
        public int KeySize { get; private set; }

        /// <summary>
        /// 暗号ファイルエンコーディング
        /// </summary>
        public Encoding EncryptFileEncoding { get; private set; }

        /// <summary>
        /// App.configから設定を読み込む
        /// </summary>
        /// <returns></returns>
        public static AppConfigData ReadConfig()
        {
            return new AppConfigData()
            {
                PrivateKeyDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["privateKeyFolder"]),
                PrivateKeyFileName = ConfigurationManager.AppSettings["privateKeyFileName"],
                PrivateKeyEncoding = Encoding.GetEncoding(ConfigurationManager.AppSettings["privateKeyEncoding"]),
                PublicKeyDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["publicKeyFolder"]),
                PublicKeyFileNameFormat = ConfigurationManager.AppSettings["publicKeyFileName"],
                PublicKeyEncoding = Encoding.GetEncoding(ConfigurationManager.AppSettings["publicKeyEncoding"]),
                KeySize = int.Parse(ConfigurationManager.AppSettings["keySize"]),
                EncryptFileEncoding = Encoding.GetEncoding(ConfigurationManager.AppSettings["encryptFileEncoding"])
            };
        }
    }
}
