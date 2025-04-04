using System.Collections;
using TMPro;
using UnityEngine;

namespace CodeBase
{
    public static class UIPopupUtility
    {
        public static RectTransform CreateTextPopup(string text, Rect rect, float duration, Transform parentCanvas,
                                                    Vector2 anchor)
        {
            GameObject popupObject = new("PopupNotification");
            popupObject.transform.SetParent(parentCanvas.transform, false);

            popupObject.AddComponent<CanvasRenderer>();

            RectTransform rectTransform = popupObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.sizeDelta = rect.size;
            rectTransform.anchoredPosition = rect.position;

            TextMeshProUGUI messageText = popupObject.AddComponent<TextMeshProUGUI>();
            messageText.enableAutoSizing = true;
            messageText.fontSizeMax = 108;
            messageText.text = text;
            messageText.alignment = TextAlignmentOptions.Center;
            messageText.color = Color.white;

            MonoBehaviour parentScript = parentCanvas.GetComponent<MonoBehaviour>();
            if (parentScript != null)
            {
                parentScript.StartCoroutine(DestroyAfterTime(popupObject, duration));
            }

            return rectTransform;
        }


        private static IEnumerator DestroyAfterTime(GameObject obj, float duration)
        {
            yield return new WaitForSeconds(duration);
            Object.Destroy(obj);
        }
    }
}