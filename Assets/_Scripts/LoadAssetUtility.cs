using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CodeBase
{
    public static class LoadAssetUtility
    {
        public static T Load<T>(string fullPath) where T : UnityEngine.Object
        {
            string cutPath = fullPath.Replace("Assets/Resources/", "").Split(".")[0];

            T resource = Resources.Load<T>(cutPath);

            if (resource != null)
            {
                return resource;
            }

            string existingPath = FindExistingPath(fullPath);
            fullPath = string.IsNullOrEmpty(fullPath) ? "NO_PATH" : fullPath;
            existingPath = string.IsNullOrEmpty(existingPath) ? "NO_PATH" : existingPath;

            throw new FileNotFoundException($"FILE IN ==={fullPath}=== DOESNT EXIST. " +
                                            $"LAST EXISTING PATH <a href=\"{existingPath}\">{existingPath}</a>");
        }


        private static string FindExistingPath(string fullPath)
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