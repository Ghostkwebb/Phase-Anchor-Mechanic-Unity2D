using UnityEngine;

namespace ChennaiGames
{
    public class SafeAreaBlockHelper : MonoBehaviour
    {
        [SerializeField] private RectTransform m_Rect;
        [SerializeField] private RectTransform m_LeftRect;
        [SerializeField] private RectTransform m_RightRect;
        [SerializeField] private RectTransform m_TopRect;
        [SerializeField] private RectTransform m_BottomRect;

        private void OnEnable()
        {
            DeviceNotchManager.Instance.OnSafeAreaChange += OnSafeAreaChange;
            ApplySafeArea();
        }

        private void OnDisable()
        {
            if(DeviceNotchManager.Instance != null)
                DeviceNotchManager.Instance.OnSafeAreaChange -= OnSafeAreaChange;
        }

        private void OnSafeAreaChange(Vector2 anchorMin, Vector2 anchorMax)
        {
            ApplySafeArea();
        }

        protected virtual void ApplySafeArea()
        {
            //Set the anchor mode to full stretch first.
            //m_Rect.anchorMin = Vector2.zero;
            //m_Rect.anchorMax = Vector2.one;
            DeviceNotchManager.Instance.ApplyRectToSafeScreenAspect(m_Rect);
            //DeviceNotchManager.Instance.ApplyToSafeScreenAspect(m_LeftRect, m_RightRect, m_TopRect, m_BottomRect);
        }
    }
}
