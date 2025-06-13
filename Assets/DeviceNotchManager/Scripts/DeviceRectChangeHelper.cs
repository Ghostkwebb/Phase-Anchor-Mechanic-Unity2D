using UnityEngine;

namespace ChennaiGames
{
    [RequireComponent(typeof(DeviceNotchManager))]
    public class DeviceRectChangeHelper : MonoBehaviour
    {
        private void OnRectTransformDimensionsChange()
        {
            DeviceNotchManager.Instance.CalculateSafeArea();
        }
    }
}
