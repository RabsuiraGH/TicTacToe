using UnityEngine;

namespace CodeBase
{
    public static class ScreenToWorldPositionConverterUtility
    {
        private static Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = Camera.main;
                }

                return _mainCamera;
            }
        }


        private static Camera _mainCamera;


        public static Vector3 Convert(Vector2 screenPosition)
        {
            return MainCamera.ScreenToWorldPoint(screenPosition);
        }
    }
}