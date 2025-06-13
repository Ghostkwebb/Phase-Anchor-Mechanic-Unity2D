using System;
using UnityEngine;

namespace ChennaiGames
{
    public class SafeAreaHelper : MonoBehaviour
    {
        [NonSerialized] private RectTransform m_Rect;
        protected RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                {
                    m_Rect = GetComponent<RectTransform>();
                }

                return m_Rect;
            }
        }

        private void OnEnable()
        {
            DeviceNotchManager.Instance.OnSafeAreaChange += OnSafeAreaChange;
            ApplySafeArea();
        }

        private void OnDisable()
        {
            if (DeviceNotchManager.Instance != null)
                DeviceNotchManager.Instance.OnSafeAreaChange -= OnSafeAreaChange;
        }

        private void OnSafeAreaChange(Vector2 anchorMin, Vector2 anchorMax)
        {
            ApplySafeArea();
        }

        protected virtual void ApplySafeArea()
        {
            //Set the anchor mode to full stretch first.
            //currentRectTrans.anchorMin = Vector2.zero;
            //currentRectTrans.anchorMax = Vector2.one;

            DeviceNotchManager.Instance.ApplyRectToSafeArea(rectTransform);
        }
    }
}
