using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CodeBase.Utilities.LoadAsset
{
    /// <summary>
    /// Utility provides easy access to any resource placed in the <b>resource</b> folder.
    /// </summary>
    public static class LoadAssetUtility
    {
        /// <summary>
        /// Loads the specified asset from the specified
        /// </summary>
        /// <param name="fullPath">Absolute path to asset. <para>(Starts with <b>Assets/Resources/</b>)</para></param>
        /// <typeparam name="T">Type of loaded asset.</typeparam>
        /// <returns>Object of given type if successfully.
        /// <para>Null if not.</para></returns>
        public static T Load<T>(string fullPath) where T : UnityEngine.Object
        {
            string cutPath = fullPath.Replace("Assets/Resources/", "").Split(".")[0];

            T resource = Resources.Load<T>(cutPath);

            if (resource != null)
            {
                return resource;
            }

            string existingPath = FindLastExistingPath(fullPath);
            fullPath = string.IsNullOrEmpty(fullPath) ? "NO_PATH" : fullPath;
            existingPath = string.IsNullOrEmpty(existingPath) ? "NO_PATH" : existingPath;

            Debug.LogError($"FILE IN ==={fullPath}=== DOESNT EXIST. " +
                           $"LAST EXISTING PATH <a href=\"{existingPath}\">{existingPath}</a>");
            return null;
        }


        private static string FindLastExistingPath(string fullPath)
        {
            string[] folders = fullPath.Split('/');
            string currentPath = "";
            string lastExistingPath = "";

            foreach (string folder in folders)
            {
                currentPath = Path.Combine(currentPath, folder);

                if (Directory.Exists(currentPath))
                {
                    lastExistingPath = currentPath;
                }
                else
                {
                    break;
                }
            }

            return lastExistingPath;
        }
    }
}