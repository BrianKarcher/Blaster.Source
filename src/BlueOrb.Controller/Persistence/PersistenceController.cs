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
            using (FileStream fileStream = new FileStream(GetPath(fileName), FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, persistData);
            }
        }

        public T Load<T>(string fileName)
            where T : class
        {
            string filePath = GetPath(fileName);
            if (!File.Exists(filePath))
            {
                return null;
            }
            //Create the stream to add object into it.
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(fileStream);
            }
        }

        public bool DoesExist(string fileName) => File.Exists(GetPath(fileName));

        string GetPath(string fileName) => Path.Combine(Application.persistentDataPath, fileName);
    }
}