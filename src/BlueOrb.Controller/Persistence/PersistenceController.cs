using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace BlueOrb.Controller.Persistence
{
    [Serializable]
    public class PersistenceController
    {
        [SerializeField]
        private string saveFileName = "Blaster.sav";
        public string SaveFileName => saveFileName;
        [SerializeField]
        private string dataFileName = "Blaster.dat";
        public string DataFileName => dataFileName;

        public void Save<T>(string fileName, T persistData)
        {
            //Create the stream to add object into it.

            // TODO Making it readable for debugging purposes. Change to binary before release.
            // TODO Make text/binary serializing an option in the GameSettingsConfig.
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    string strData = JsonConvert.SerializeObject(persistData);
                    sw.Write(strData);
                }
            }
            catch { }
            //using (FileStream fileStream = new FileStream(GetPath(fileName), FileMode.Create, FileAccess.Write))
            //{
            //    //BinaryFormatter formatter = new BinaryFormatter();
            //    //formatter.Serialize(fileStream, persistData);
            //    string strData = JsonConvert.SerializeObject(persistData);
            //    fileStream.writ
            //}
        }

        public T Load<T>(string fileName)
            where T : class
        {
            string filePath = GetPath(fileName);
            if (!File.Exists(filePath))
            {
                return null;
            }
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    T persistData = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                    return persistData;
                }
            }
            catch
            {
                return null;
            }
            //Create the stream to add object into it.
            //using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read))
            //{
            //    try
            //    {
            //        BinaryFormatter formatter = new BinaryFormatter();
            //        return (T)formatter.Deserialize(fileStream);
            //    }
            //    catch
            //    {
            //        return null;
            //    }
            //}
        }

        public bool DoesExist(string fileName) => File.Exists(GetPath(fileName));

        string GetPath(string fileName) => Path.Combine(Application.persistentDataPath, fileName);
    }
}