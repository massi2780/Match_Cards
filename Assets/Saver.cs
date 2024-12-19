using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
//using File = UnityEngine.Windows.File;

public class Saver : MonoBehaviour
    {
        public Save_Things Data_object = new Save_Things();
        const string EN_Key = "Soltan_Gham_Maadar";

        void Start()
        {
            string original = "Hello, World!";
            string key = "your-encryption-key"; // Ensure this key is secure and consistent

            // Encrypt the string
            string encrypted = EncryptString(original, key);
            Debug.Log("Encrypted: " + encrypted);

            // Decrypt the string
            string decrypted = DecryptString(encrypted, key);
            Debug.Log("Decrypted: " + decrypted);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save_Datas();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                Load_Datas();
            }
        }


        public void Save_Datas()
        {
            string filepath = Application.persistentDataPath + "/Saved_Data.json";
            string Save_object_Datas = JsonUtility.ToJson(Data_object);
            string encrypted = EncryptString(Save_object_Datas, EN_Key);
            Debug.Log(filepath);
            System.IO.File.WriteAllText(filepath, encrypted);
            Debug.Log("Everyting is saved");
        }



        public void Load_Datas()
        {
            
            string filepath = Application.persistentDataPath + "/Saved_Data.json";
          // File.Delete(filepath);
            if (File.Exists(filepath))
            {
                string Save_object_Datas = System.IO.File.ReadAllText(filepath);
                string decrypted = DecryptString(Save_object_Datas, EN_Key);
                Data_object = JsonUtility.FromJson<Save_Things>(decrypted);
                Debug.Log("Everyting is restored!");
            }
            else
            {
                SetDefaultValues();
            }

        }


        public void SetDefaultValues()
        {
          
            Save_Datas();
        }

        public static string EncryptString(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Generate a key from the provided key string
                var keyBytes = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes("SaltValue"), 1000).GetBytes(32);
                aesAlg.Key = keyBytes;
                aesAlg.IV = new byte[16]; // Initialize to zero

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }


        public static string DecryptString(string cipherText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Generate a key from the provided key string
                var keyBytes = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes("SaltValue"), 1000).GetBytes(32);
                aesAlg.Key = keyBytes;
                aesAlg.IV = new byte[16]; // Initialize to zero

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

    }


[System.Serializable]
public class Save_Things
{
    public int UnlockedLevel = 1;

}


[System.Serializable]
public class Fan
{
    public int fan_id;
    public bool fan_is_on;
}