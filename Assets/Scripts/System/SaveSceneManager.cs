using UnityEngine;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using data.structs;

public static class SaveSceneManager
{
    static string savePath => Application.persistentDataPath + "/save.dat";

    static readonly byte[] aesKey = Encoding.UTF8.GetBytes("1234567890123456"); 
    static readonly byte[] hmacKey = Encoding.UTF8.GetBytes("papA_ppawsID_123");


    public static void Save(GameState state)
    {
        string json = JsonUtility.ToJson(state);
        byte[] encrypted = Encrypt(json);
        byte[] hash = GenerateHMAC(encrypted);

        using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            writer.Write(hash.Length);
            writer.Write(hash);
            writer.Write(encrypted);
        }

        Debug.Log("Filee saved succesfully");
    }

    public static GameState Load()
    {
        if (!File.Exists(savePath))
            return null;

        using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            int hashLength = reader.ReadInt32();
            byte[] savedHash = reader.ReadBytes(hashLength);
            byte[] encrypted = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

            byte[] newHash = GenerateHMAC(encrypted);

            if (!CompareHashes(savedHash, newHash))
            {
                Debug.LogError("Save file tampered!");
                return null;
            }

            string json = Decrypt(encrypted);
            return JsonUtility.FromJson<GameState>(json);
        }
    }

    // SecuritySystem
    static byte[] Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = aesKey;
            aes.GenerateIV();
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                return ms.ToArray();
            }
        }
    }

    static string Decrypt(byte[] cipherData)
    {
        using (Aes aes = Aes.Create())
        {
            byte[] iv = new byte[16];
            System.Array.Copy(cipherData, iv, iv.Length);
            aes.Key = aesKey;
            aes.IV = iv;

            using (MemoryStream ms = new MemoryStream(cipherData, 16, cipherData.Length - 16))
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }

    static byte[] GenerateHMAC(byte[] data)
    {
        using (HMACSHA256 hmac = new HMACSHA256(hmacKey))
        {
            return hmac.ComputeHash(data);
        }
    }

    static bool CompareHashes(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;
        return true;
    }
}