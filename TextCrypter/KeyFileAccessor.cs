using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TextCrypter
{
    /// <summary>
    /// 鍵ファイルのアクセスを行うクラス
    /// </summary>
    public class KeyFileAccessor
    {
        public enum KeyType
        {
            Public,
            Private
        }

        /// <summary>
        /// 鍵の所有者
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 公開鍵ファイルパス
        /// </summary>
        public string PublicKeyFile { get; private set; }

        /// <summary>
        /// 秘密鍵ファイルパス
        /// </summary>
        public string PrivateKeyFile { get; private set; }

        /// <summary>
        /// App.configの設定情報
        /// </summary>
        private AppConfigData _config;

        /// <summary>
        /// 鍵の所有者を指定してインスタンスを生成
        /// </summary>
        public KeyFileAccessor(string name)
        {
            Name = name;
            _config = AppConfigData.ReadConfig();

            // 公開鍵パス構築
            PublicKeyFile = Path.Combine(_config.PublicKeyDirectory,
                                         string.Format(_config.PublicKeyFileNameFormat, Name));

            // 秘密鍵パス構築
            PrivateKeyFile = Path.Combine(_config.PrivateKeyDirectory, _config.PrivateKeyFileName);
        }

        /// <summary>
        /// 既に自分の鍵が存在するかチェック
        /// </summary>
        /// <returns>存在する場合はtrue</returns>
        public bool ExistsMyKey(KeyType key)
        {
            if (key == KeyType.Public)
            {
                return File.Exists(PublicKeyFile);
            }
            else
            {
                return File.Exists(PrivateKeyFile);
            }
        }

        /// <summary>
        /// 自分の鍵を読み込む
        /// </summary>
        public string ReadMyKey(KeyType key)
        {
            if (key == KeyType.Public)
            {
                return File.ReadAllText(PublicKeyFile, _config.PublicKeyEncoding);
            }
            else
            {
                return File.ReadAllText(PrivateKeyFile, _config.PrivateKeyEncoding);
            }
        }

        /// <summary>
        /// 自分の鍵ペアを生成する
        /// </summary>
        public void GenerateMyKey()
        {
            using (var rsa = new RSACryptoServiceProvider(_config.KeySize))
            {
                // 公開鍵更新
                string publicKey = rsa.ToXmlString(false);
                WriteKeyFile(publicKey, PublicKeyFile, _config.PublicKeyEncoding);

                // 秘密鍵更新
                string privateKey = rsa.ToXmlString(true);
                WriteKeyFile(privateKey, PrivateKeyFile, _config.PrivateKeyEncoding);
            }
        }

        /// <summary>
        /// 自分の鍵を削除する
        /// </summary>
        /// <param name="includePrivateKey">秘密鍵も削除するか</param>
        public void DeleteMyKey(bool includePrivateKey = false)
        {
            // 公開鍵削除
            if (File.Exists(PublicKeyFile))
            {
                File.Delete(PublicKeyFile);
            }

            // 秘密鍵削除
            if (includePrivateKey && File.Exists(PrivateKeyFile))
            {
                File.Delete(PrivateKeyFile);
            }
        }

        /// <summary>
        /// 公開鍵所有者の一覧を取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPublicKeyOwners()
        {
            // 公開鍵フォルダが存在しない場合は0件
            if (!Directory.Exists(AppConfigData.ReadConfig().PublicKeyDirectory))
            {
                return new List<string>();
            }

            // 公開鍵所有者リスト初期化
            List<string> owners = new List<string>();

            // App.config読み込み
            AppConfigData config = AppConfigData.ReadConfig();

            // 公開鍵を全て取得
            string[] files = Directory.GetFiles(config.PublicKeyDirectory);

            // 公開鍵ファイル名から所有者名を取り出し
            foreach (string file in files)
            {
                // 公開鍵ファイル名のうち所有者名以降の文字列を取得
                string suffix = config.PublicKeyFileNameFormat.Replace("{0}", string.Empty);

                // ファイル名から所有者名を取得
                string owner = Path.GetFileName(file).Replace(suffix, string.Empty);

                owners.Add(owner);
            }
            return owners;
        }

        /// <summary>
        /// 公開鍵ファイルの有効性をチェックする
        /// </summary>
        /// <param name="publicKeyFile">公開鍵ファイルパス</param>
        /// <returns>有効である場合はtrue</returns>
        public static bool IsValidPublicKeyFile(string publicKeyFile)
        {
            // 公開鍵ファイルの存在チェック
            if (!File.Exists(publicKeyFile))
            {
                return false;
            }

            // 公開鍵ファイルの読み込み
            string publicKey = File.ReadAllText(publicKeyFile, AppConfigData.ReadConfig().PublicKeyEncoding);

            // 公開鍵の形式チェック
            try
            {
                RSACrypter.Encrypt(publicKey, "test");
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 公開鍵を追加する
        /// </summary>
        /// <param name="publicKeyFile">追加する公開鍵ファイルのパス</param>
        public static void AddPublicKey(string publicKeyFile)
        {
            // App.config読み込み
            AppConfigData config = AppConfigData.ReadConfig();

            // コピー先パス
            string copyTo = Path.Combine(config.PublicKeyDirectory, Path.GetFileName(publicKeyFile));

            // 追加元と追加先が同一の場合は何もしない
            if (publicKeyFile.TrimEnd(Path.DirectorySeparatorChar) == copyTo.TrimEnd(Path.DirectorySeparatorChar))
            {
                return;
            }

            // 公開鍵フォルダが存在しない場合は作成
            if (!Directory.Exists(config.PublicKeyDirectory))
            {
                Directory.CreateDirectory(config.PublicKeyDirectory);
            }

            // 公開鍵ファイルコピー
            File.Copy(publicKeyFile, copyTo, true);
        }

        /// <summary>
        /// 鍵保存
        /// </summary>
        /// <param name="text">鍵内容</param>
        /// <param name="file">ファイルパス</param>
        /// <param name="encoding">ファイルエンコーディング</param>
        private void WriteKeyFile(string text, string file, Encoding encoding)
        {
            // フォルダが存在しない場合作成
            string dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (StreamWriter write = new StreamWriter(file, false, encoding))
            {
                write.Write(text);
            }
        }
    }
}
