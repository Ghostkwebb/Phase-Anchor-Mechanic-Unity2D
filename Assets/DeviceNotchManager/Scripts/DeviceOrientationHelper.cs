using UnityEngine;

namespace ChennaiGames
{
    [RequireComponent(typeof(DeviceNotchManager))]
    public class DeviceOrientationHelper : MonoBehaviour
    {
        private const float ORIENTATION_CHECK_INTERVAL = 0.5f;

        ScreenOrientation m_CurrentOrientation;
        float nextOrientationCheckTime;
        Vector2 lastResolution = Vector2.zero;

        private void Awake()
        {
            nextOrientationCheckTime = Time.realtimeSinceStartup + 1f;
        }

        private void Update()
        {
            if (Time.realtimeSinceStartup >= nextOrientationCheckTime)
            {
                if (Screen.orientation != m_CurrentOrientation)
                    CalculateScreenOrientation();
                nextOrientationCheckTime = Time.realtimeSinceStartup + ORIENTATION_CHECK_INTERVAL;
            }
        }

        private void CalculateScreenOrientation()
        {
            //DebugX.Log("Orientation changed from " + m_CurrentOrientation + " to " + Screen.orientation + " at " + Time.time);

            m_CurrentOrientation = Screen.orientation;
            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;

            DeviceNotchManager.Instance.OnOrientationChanged(m_CurrentOrientation);
        }
    }
}
