using System;
using CodeBase.Config.MarkConfig;
using UnityEngine;
using Zenject;

namespace CodeBase
{
    public class MarkCell : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private MarkAssetConfig _assetConfig;
        [SerializeField] private Marker _marker = Marker.None;


        [Inject]
        public void Construct(MarkAssetConfig assetConfig)
        {
            _assetConfig = assetConfig;
            _spriteRenderer ??= GetComponent<SpriteRenderer>();
        }


        public void Initialize(Marker marker)
        {
            _marker = marker;
            try
            {
                _spriteRenderer.sprite = _assetConfig.GetSprite(_marker);
            }
            catch (Exception)
            {
                Destroy(this.gameObject);
                throw;
            }
        }


        public void ChangeMarker(Marker marker)
        {
            _marker = marker;
            if (marker == Marker.None)
            {
                gameObject.SetActive(false);
                return;
            }

            _spriteRenderer.sprite = _assetConfig.GetSprite(_marker);
            gameObject.SetActive(true);
        }
    }
}