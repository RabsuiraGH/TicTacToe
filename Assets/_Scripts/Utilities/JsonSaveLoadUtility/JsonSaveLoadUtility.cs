using System.IO;
using UnityEngine;

namespace CodeBase.Utilities.JsonSaveLoad
{
    /// <summary>
    /// Utility to save and load serializable object to json file format.
    /// </summary>
    public static class JsonSaveLoadUtility
    {
        /// <summary>
        /// Persistent data path.
        /// </summary>
        public static readonly string DefaultSavePath = Application.persistentDataPath;


        /// <summary>
        /// Saves an object to the given path.
        /// </summary>
        /// <param name="objectToSave">Serializable object to save.</param>
        /// <param name="path">Path to save file.</param>
        /// <param name="overwrite">Overwrites file if it already exists.</param>
        /// <param name="defaultPath">Instead of full path combines <see cref="DefaultSavePath"/> with
        /// <paramref name="path"/>/</param>
        /// <typeparam name="T">Type of saved object.</typeparam>
        /// <returns>True if saved successfully. False otherwise.</returns>
        public static bool TrySave<T>(T objectToSave, string path, bool overwrite, bool defaultPath = true)
        {
            if (defaultPath)
                path = Path.Combine(DefaultSavePath, path);

            if (!overwrite && PathExists(path))
                return false;


            string json = JsonUtility.ToJson(objectToSave);
            File.WriteAllText(path, json);

            return true;
        }


        /// <summary>
        /// Loads an object from the given path.
        /// </summary>
        /// <param name="path">Load path.</param>
        /// <param name="loadedObject">Loaded object. Can be null.</param>
        /// <param name="defaultPath">Instead of full path combines <see cref="DefaultSavePath"/> with
        /// <paramref name="path"/>/</param>
        /// <typeparam name="T">Type of saved object.</typeparam>
        /// <returns>True if loaded successfully. False otherwise.</returns>
        public static bool TryLoad<T>(string path, out T loadedObject, bool defaultPath = true)
        {
            loadedObject = default(T);

            if (defaultPath)
                path = Path.Combine(DefaultSavePath, path);

            if (!PathExists(path))
                return false;

            string json = File.ReadAllText(path);
            loadedObject = JsonUtility.FromJson<T>(json);

            return loadedObject != null;
        }


        private static bool PathExists(string path)
        {
            return File.Exists(path);
        }
    }
}