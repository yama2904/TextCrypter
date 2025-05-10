using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TextCrypter
{
    /// <summary>
    /// RSAによる暗号化/復号を行うクラス
    /// </summary>
    public class RSACrypter
    {
        /// <summary>
        /// テキストの暗号化を行う
        /// </summary>
        /// <param name="publicKey">暗号化に使用する公開鍵</param>
        /// <param name="plainText">暗号化を行うテキスト</param>
        /// <returns>暗号化後のテキスト</returns>
        public static string Encrypt(string publicKey, string plainText)
        {
            // OAEPパディングにおける一度に暗号化可能な最大バイト数（https://learn.microsoft.com/ja-jp/dotnet/api/system.security.cryptography.rsacryptoserviceprovider.encrypt?view=netframework-4.8）
            // [キーサイズ - 2 - 2 * ハッシュサイズ(.NetFrameworkはSHA-1固定のため20)]
            int MAX_ENCRYPTED_SIZE = (AppConfigData.ReadConfig().KeySize / 8) - 2 - 40;

            List<string> blocks = new List<string>();
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                var data = Encoding.UTF8.GetBytes(plainText);

                for (int offset = 0; offset < data.Length; offset += MAX_ENCRYPTED_SIZE)
                {
                    // RSA暗号化は一度に最大MAX_ENCRYPTED_SIZEバイトまでしかできないため、分割して暗号化する
                    int size = Math.Min(data.Length - offset, MAX_ENCRYPTED_SIZE);
                    byte[] rgb = data.Skip(offset).Take(size).ToArray();
                    blocks.Add(Convert.ToBase64String(rsa.Encrypt(rgb, true)));
                }
            }

            return string.Join(",", blocks);
        }

        /// <summary>
        /// 暗号化されたテキストの復号を行う
        /// </summary>
        /// <param name="privateKey">復号に使用する秘密鍵</param>
        /// <param name="encryptText">復号を行う暗号化済みテキスト</param>
        /// <returns>復号後のテキスト</returns>
        public static string Decrypt(string privateKey, string encryptText)
        {
            List<byte> decryptBytes = new List<byte>();
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                foreach (string data in encryptText.Split(','))
                {
                    byte[] rgb = Convert.FromBase64String(data);
                    decryptBytes.AddRange(rsa.Decrypt(rgb, true));
                }
            }
            return Encoding.UTF8.GetString(decryptBytes.ToArray());
        }
    }
}
