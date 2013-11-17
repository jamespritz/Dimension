using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Linq;


namespace intrinsic.security.encryption {

    /// <summary>
    /// Represents and encrypted value with its initialization vector.
    /// </summary>
    [Serializable]
    public class EncryptedValue {
        /// <summary>
        /// encrypted value as base 64 string
        /// </summary>
        public string Base64EncryptedValue { get; set; }

        /// <summary>
        /// initialization vector as base 64 string
        /// </summary>
        public string Base64IV { get; set; }

        public string Base64Salt { get; set; }



    }

    public class EncryptionService {


        public EncryptedValue FromRepository(string base64PassCode, string base64CombinedSaltandIV) {
            XDocument doc = XDocument.Parse(base64CombinedSaltandIV);

            return new EncryptedValue() { Base64EncryptedValue = base64PassCode, Base64IV = doc.Root.Attribute("iv").Value, Base64Salt = doc.Root.Attribute("s").Value };

        }

        public string CombineSaltAndIVforRepo(string base64Salt, string base64IV) {
            XDocument salt = new XDocument();
            salt.Add(new XElement("salt", new XAttribute("iv", base64IV), new XAttribute("s", base64Salt)));
            return salt.ToString();
        }


        public string Decrypt(string passCode, EncryptedValue data) {




            byte[] salt = Convert.FromBase64String(data.Base64Salt);
            byte[] iv = Convert.FromBase64String(data.Base64IV);
            byte[] key = null;
            byte[] encrypted = Convert.FromBase64String(data.Base64EncryptedValue);

            try {
                Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(passCode, salt);

                key = k1.GetBytes(16);




                using (var rijndael = new RijndaelManaged() {
                    Mode = CipherMode.CBC
                                                                ,
                    IV = iv
                                                                ,
                    Key = key
                                                                ,
                    Padding = PaddingMode.PKCS7
                }) {
                    using (var decryptor = rijndael.CreateDecryptor(key, iv))
                    using (var memoryStream = new MemoryStream(encrypted))
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    using (var writer = new StreamReader(cryptoStream)) {
                        //writer.Write(encrypted);
                        //writer.Flush();
                        //cryptoStream.Flush();
                        //cryptoStream.FlushFinalBlock();
                        //return Convert.ToBase64String(memoryStream.ToArray());
                        return writer.ReadToEnd();
                    }
                }
            } catch (Exception) {

                throw;
            } finally {
                ClearBytes(salt);
                ClearBytes(iv);
                ClearBytes(key);
                ClearBytes(encrypted);
            }


        }

        public EncryptedValue Encrypt(string passCode, string data, string salt, string iv) {
            return this.Encrypt(passCode, data, Convert.FromBase64String(salt), Convert.FromBase64String(iv));

        }

        public EncryptedValue Encrypt(string passCode, string data) {

            byte[] salt = new byte[8];
            byte[] iv = null;
            byte[] key = null;

            try {

                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) {
                    rng.GetBytes(salt);
                }

                Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(passCode, salt);
                key = k1.GetBytes(16);

                using (var rijndael = new RijndaelManaged() {
                    Mode = CipherMode.CBC
                    ,
                    Padding = PaddingMode.PKCS7

                }) {
                    rijndael.GenerateIV();
                    iv = rijndael.IV;
                    return this.Encrypt(passCode, data, salt, iv);
                }
            } catch (Exception) {

                throw;
            } finally {
                ClearBytes(salt);
                ClearBytes(iv);
                ClearBytes(key);
            }


        }

        private EncryptedValue Encrypt(string passCode, string data, byte[] salt, byte[] iv) {


            byte[] key = null;

            try {



                Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(passCode, salt);
                key = k1.GetBytes(16);

                using (var rijndael = new RijndaelManaged() {
                    Mode = CipherMode.CBC
                    ,
                    Key = key
                    ,
                    IV = iv
                    ,
                    Padding = PaddingMode.PKCS7

                }) {
                    //rijndael.GenerateIV();
                    //iv = rijndael.IV;


                    using (var encryptor = rijndael.CreateEncryptor(key, iv))
                    using (var mem = new MemoryStream())
                    using (var crypt = new CryptoStream(mem, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(crypt)) {
                        writer.Write(data);
                        writer.Flush();
                        crypt.Flush();
                        crypt.FlushFinalBlock();

                        byte[] encrypted = mem.ToArray();

                        return new EncryptedValue() {
                            Base64IV = Convert.ToBase64String(iv)

                                                        ,
                            Base64EncryptedValue = Convert.ToBase64String(encrypted)
                                                        ,
                            Base64Salt = Convert.ToBase64String(salt)
                        };
                    }

                }
            } catch (Exception) {

                throw;
            } finally {
                ClearBytes(salt);
                ClearBytes(iv);
                ClearBytes(key);
            }

        }


        public static void ClearBytes(byte[] buffer) {
            // Check arguments.
            if (buffer == null) {
                throw new ArgumentException("buffer");
            }

            // Set each byte in the buffer to 0.
            for (int x = 0; x < buffer.Length; x++) {
                buffer[x] = 0;
            }
        }
    }


}
