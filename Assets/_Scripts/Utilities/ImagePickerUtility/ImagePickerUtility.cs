using System;
using System.IO;
using UnityEngine;
using Application = UnityEngine.Application;

namespace CodeBase.Utilities.ImagePicker
{
    /// <summary>
    /// Utility provides access to device storage for picking an image.
    /// </summary>
    public static class ImagePickerUtility
    {
        /// <summary>
        /// Opens file browser for picking image and invoke given callback.
        /// </summary>
        /// <param name="onPickedCallback">Called after picking an image, passing it as an argument.</param>
        /// <param name="maxImageSize">Maximum size of image. If image size is greater, downscale to this.</param>
        public static bool TryPickImage(Action<Texture2D> onPickedCallback, int maxImageSize)
        {
            Texture2D texture = new(1, 1);

#if !UNITY_EDITOR && UNITY_ANDROID
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
            {
                if (string.IsNullOrEmpty(path))
                {
                    return false;
                }
                texture = NativeGallery.LoadImageAtPath(path, maxImageSize, false);
                if (texture == null)
                {
                    return false;
                }

                onPickedCallback?.Invoke(texture);
            });

#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            using (System.Windows.Forms.OpenFileDialog dialog = new())
            {
                dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = dialog.FileName;
                    texture.LoadImage(File.ReadAllBytes(path));
                    onPickedCallback?.Invoke(texture);
                }
                else
                {
                    return false;
                }
            }
#endif
            return true;
        }


        /// <summary>
        /// Saves the given texture to persistent storage with the given name.
        /// </summary>
        /// <param name="image">Texture to save.</param>
        /// <param name="fileName">Name of new file, including extension.</param>
        public static void SaveImage(Texture2D image, string fileName)
        {
            byte[] bytes = image.EncodeToPNG();
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
#if UNITY_EDITOR_ImagePickerUtility
            Debug.Log(($"Avatar image was saved to path: {filePath}"));
#endif
            File.WriteAllBytes(filePath, bytes);
        }


        /// <summary>
        /// Loads an image from persistent storage with the specified filename.
        /// </summary>
        /// <param name="fileName">Name of new file, including extension.</param>
        /// <param name="image">Loaded texture.</param>
        /// <returns>True if loaded, False otherwise.</returns>
        public static bool TryLoadImage(string fileName, out Texture2D image)
        {
            image = null;
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            if (!File.Exists(filePath))
            {
                return false;
            }

#if UNITY_EDITOR_ImagePickerUtility
            Debug.Log(($"Avatar image was loaded from path: {filePath}"));
#endif
            byte[] bytes = File.ReadAllBytes(filePath);

            image = new Texture2D(1, 1);
            return image.LoadImage(bytes);
        }
    }
}