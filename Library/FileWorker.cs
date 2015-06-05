using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Library
{

    public class FileWorker
    {
        public async static Task<bool> SaveToFile<T>(string fileName, T saveObject)
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(saveObject);

                await SaveContentToFile(fileName, json);

                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
#endif
                return false;
            }
        }

        public static bool EncryptAndSaveToFile<T>(string fileName, T contentObject)
        {
            try
            {
                string json = JsonConvert.SerializeObject(contentObject);

                byte[] plainTextBytes = Encoding.UTF8.GetBytes(json);

                string encryptedContent = Convert.ToBase64String(plainTextBytes);

                SaveContentToFile(fileName, encryptedContent);

                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
#endif

                return false;
            }
        }

        public async static Task SaveContentToFile(string fileName, string content)
        {
            using (var file = File.Open(GetFilePath(fileName), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var strm = new StreamWriter(file))
                {
                    await strm.WriteAsync(content);
                }
            }
        }

        public static T LoadFromFile<T>(string fileName, T defaultValue)
        {
            try
            {
                string pureObject = ReadPureString(fileName);

                if (!string.IsNullOrEmpty(pureObject))
                {
                    return (T)Newtonsoft.Json.JsonConvert.DeserializeObject<T>(pureObject);
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
#endif

                return defaultValue;
            }
        }

        public static T LoadEncruptedFile<T>(string fileName)
        {
            try
            {
                string pureEncryptContent = ReadPureString(fileName);

                byte[] plainTextBytes = Convert.FromBase64String(pureEncryptContent);

                string decryptedFile = Encoding.UTF8.GetString(plainTextBytes);

                return (T)JsonConvert.DeserializeObject(decryptedFile);
            }
            catch (Exception ex)
            {
#if DEBUG
#endif

                return default(T);
            }
        }

        public static string ReadPureString(string fileName)
        {
            string fileString = GetFilePath(fileName);

            if (File.Exists(fileString))
            {
                using (var file = File.Open(fileString, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(file))
                    {
                        string readed = reader.ReadToEnd();

                        return readed;
                    }
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetFilePath(string fileName)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            string filePath = Path.Combine(path, fileName);

            return filePath;
        }
    }
}