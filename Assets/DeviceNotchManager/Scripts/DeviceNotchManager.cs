using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChennaiGames
{
    [RequireComponent(typeof(Canvas))]
    public class DeviceNotchManager : MonoBehaviour
    {
        private static DeviceNotchManager instance;
        private static bool alive = true;
        public static DeviceNotchManager Instance
        {
            get
            {
                // Check if application is quitting and AudioManager is destroyed
                if (!alive)
                {
                    return null;
                }

                return instance;
            }
        }

        private DeviceNotchManager() { }

        public event Action<Vector2, Vector2> OnSafeAreaChange;
        public event Action<ScreenOrientation> OnScreenOrientationChanged;

        public ScreenOrientation CurrentOrientation { get; private set; }
        public bool IsLandscape { get; private set; }

        public Rect screenAspectRect { get; private set; }
        public Vector2 screenAspectAnchorMin { get; private set; }
        public Vector2 screenAspectAnchorMax { get; private set; }

        public Rect safeAreaRect { get; private set; }
        public Vector2 safeAreaAnchorMin { get; private set; }
        public Vector2 safeAreaAnchorMax { get; private set; }

        [Tooltip("Whether safe area's top edge will be respected")]
        [SerializeField] private bool topEdge = true;
        [Tooltip("Whether safe area's left edge will be respected")]
        [SerializeField] private bool leftEdge = true;
        [Tooltip("Whether safe area's right edge will be respected")]
        [SerializeField] private bool rightEdge = true;
        [Tooltip("Whether safe area's bottom edge will be respected")]
        [SerializeField] private bool bottomEdge = true;
        [Tooltip("Scale down the resulting value read from an edge to be less than an actual value.")]
        [SerializeField, Range(0f, 1f)] private float safeAreaPadding = 1;

        public Rect safeScreenAspectRect { get; private set; }
        public Vector2 safeScreenAspectAnchorMin { get; private set; }
        public Vector2 safeScreenAspectAnchorMax { get; private set; }

        Vector2 fixedResolution;
        float windowAspect;
        float targetAspect;
        float scaleHeight;
        float scaleWidth;

        Rect lastSafeArea = Rect.zero;


        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            // Make object persist throughout lifetime of application (including in between scene transitions)
            DontDestroyOnLoad(this.gameObject);

            fixedResolution = GetComponent<CanvasScaler>().referenceResolution;

            CurrentOrientation = Screen.orientation;

            CalculateSafeArea();
        }

        private void OnDestroy()
        {
            if (instance != this)
            {
                StopAllCoroutines();
            }
        }

        private void OnApplicationQuit()
        {
            alive = false;
        }

        internal void OnOrientationChanged(ScreenOrientation orientation)
        {
            this.CurrentOrientation = orientation;

            IsLandscape = CurrentOrientation == ScreenOrientation.LandscapeLeft || CurrentOrientation == ScreenOrientation.LandscapeRight || CurrentOrientation == ScreenOrientation.LandscapeLeft;
            
            OnScreenOrientationChanged?.Invoke(CurrentOrientation);

            CalculateSafeArea();
        }

        internal void CalculateSafeArea()
        {
            //the aspect ratio we want
            targetAspect = IsLandscape ? fixedResolution.x / fixedResolution.y : fixedResolution.y / fixedResolution.x;

            //the real aspect ratio of the screen that we're going to fit inside of
            windowAspect = (float)Screen.width / (float)Screen.height;

            //this is the ratio of the window aspect to the target aspect, see {1}
            scaleHeight = windowAspect / targetAspect;
            //we have the ratio in terms of height, invert to get it in terms of width
            scaleWidth = 1f / scaleHeight;

            float xRect, yRect, widthRect, heightRect;
            if (scaleHeight < 1.0f)
            {
                //the screen is taller than the desired aspect ratio
                widthRect = 1f; //take all the width
                heightRect = scaleHeight; //take as much of the height we need
                xRect = 0f; //we don't need to asjust horizontal, as we took all the width
                yRect = (1f - scaleHeight) / 2f; //1 - scaleHeight is the amount of black, move up half that to split it so we have black on both top and bottom
            }
            else
            {
                //the screen is wider than the desired aspect ratio
                widthRect = scaleWidth; //take as much width as we need
                heightRect = 1f; //take all the height
                xRect = (1f - scaleWidth) / 2f; //1 - scaleWidth is the amount of black, move horizontal half that to split it
                yRect = 0f; //we don't need to adjust vertically, as we took all height
            }

            screenAspectRect = new Rect(xRect, yRect, widthRect, heightRect);

            screenAspectAnchorMin = new Vector2(screenAspectRect.xMin, screenAspectRect.yMin);
            screenAspectAnchorMax = new Vector2(screenAspectRect.xMax, screenAspectRect.yMax);

            lastSafeArea = Screen.safeArea;
            Rect safeAreaRelative = ToScreenRelativeRect(lastSafeArea);

            //safeAreaRelative.xMin = leftEdge ? Mathf.Max(safeAreaRelative.xMin, 0.01f) : 0;
            //safeAreaRelative.xMax = rightEdge ? Mathf.Min(safeAreaRelative.xMax, 0.99f) : 1;
            safeAreaRelative.xMin = leftEdge ? safeAreaRelative.xMin : 0;
            safeAreaRelative.xMax = rightEdge ? safeAreaRelative.xMax : 1;
            safeAreaRelative.yMin = bottomEdge ? safeAreaRelative.yMin : 0;
            safeAreaRelative.yMax = topEdge ? safeAreaRelative.yMax : 1;

            float safeAreaPaddingTemp = safeAreaPadding;
#if UNITY_ANDROID
            safeAreaPaddingTemp = 1 + (1 - safeAreaPadding);
#endif

            //Apply padding to the calculated rect
            safeAreaRelative.xMin *= safeAreaPaddingTemp;
            safeAreaRelative.xMax = Mathf.Clamp01(1f - (1f - safeAreaRelative.xMax) * safeAreaPaddingTemp);
            safeAreaRelative.yMin *= safeAreaPaddingTemp;
            safeAreaRelative.yMax = Mathf.Clamp01(1f - (1f - safeAreaRelative.yMax) * safeAreaPaddingTemp);

            safeAreaAnchorMin = new Vector2(safeAreaRelative.xMin, safeAreaRelative.yMin);
            safeAreaAnchorMax = new Vector2(safeAreaRelative.xMax, safeAreaRelative.yMax);

            safeScreenAspectRect = Rect.MinMaxRect(
                screenAspectRect.xMin > safeAreaRelative.xMin ? safeAreaRelative.xMin : screenAspectRect.xMin,
                screenAspectRect.yMin > safeAreaRelative.yMin ? safeAreaRelative.yMin : screenAspectRect.yMin,
                screenAspectRect.xMax < safeAreaRelative.xMax ? safeAreaRelative.xMax : screenAspectRect.xMax,
                screenAspectRect.yMax < safeAreaRelative.yMax ? safeAreaRelative.yMax : screenAspectRect.yMax
                );

            safeScreenAspectAnchorMin = new Vector2(safeScreenAspectRect.xMin, safeScreenAspectRect.yMin);
            safeScreenAspectAnchorMax = new Vector2(safeScreenAspectRect.xMax, safeScreenAspectRect.yMax);

            safeAreaRect = safeAreaRelative;
// #if UNITY_EDITOR
//             safeAreaAnchorMin = Vector2.zero;
//             safeAreaAnchorMax = Vector2.one;
//             safeScreenAspectAnchorMin = Vector2.zero;
//             safeScreenAspectAnchorMax = Vector2.one;

//             screenAspectRect = Rect.MinMaxRect(0,0,1,1);
//             safeScreenAspectRect = Rect.MinMaxRect(0, 0, 1, 1);
// #endif
            OnSafeAreaChange?.Invoke(safeAreaAnchorMin, safeAreaAnchorMax);
        }


        public void ApplyRectToSafeArea(RectTransform rectTransform)
        {
            //Set the anchor mode to full stretch first.
            //rectTransform.anchorMin = Vector2.zero;
            //rectTransform.anchorMax = Vector2.one;

            rectTransform.anchorMin = safeAreaAnchorMin;
            rectTransform.anchorMax = safeAreaAnchorMax;
        }

        public void ApplyRectToSafeScreenAspect(RectTransform rectTransform)
        {
            rectTransform.anchorMin = safeScreenAspectAnchorMin;
            rectTransform.anchorMax = safeScreenAspectAnchorMax;
        }

        public void ApplyToScreenAspect(RectTransform leftRect, RectTransform rightRect, RectTransform topRect, RectTransform bottomRect)
        {
            ApplyToSafeScreenAspectLR(leftRect, rightRect, screenAspectRect);
            ApplyToSafeScreenAspectTB(topRect, bottomRect, screenAspectRect);
        }

        public void ApplyToSafeScreenAspect(RectTransform leftRect, RectTransform rightRect, RectTransform topRect, RectTransform bottomRect)
        {
            ApplyToSafeScreenAspectLR(leftRect, rightRect, safeScreenAspectRect);
            ApplyToSafeScreenAspectTB(topRect, bottomRect, safeScreenAspectRect);
        }

        public void ApplyToSafeScreenAspectLR(RectTransform leftRect, RectTransform rightRect, Rect rect)
        {
            leftRect.anchorMin = new Vector2(0f, rect.yMin);
            leftRect.anchorMax = new Vector2(rect.xMin, rect.yMax);

            rightRect.anchorMin = new Vector2(rect.xMax, rect.yMin);
            rightRect.anchorMax = new Vector2(1f, rect.yMax);
        }

        public void ApplyToSafeScreenAspectTB(RectTransform topRect, RectTransform bottomRect, Rect rect)
        {
            bottomRect.anchorMin = new Vector2(rect.xMin, 0f);
            bottomRect.anchorMax = new Vector2(rect.xMax, rect.yMin);

            topRect.anchorMin = new Vector2(rect.xMin, rect.yMax);
            topRect.anchorMax = new Vector2(rect.xMax, 1f);
        }

        private Rect ToScreenRelativeRect(Rect absoluteRect)
        {
#if UNITY_STANDALONE
            var w = absoluteRect.width;
            var h = absoluteRect.height;
#else
            int w = Screen.currentResolution.width;
            int h = Screen.currentResolution.height;
#endif
            //DebugX.Log($"{w} {h} {Screen.currentResolution} {absoluteRect}");
            return new Rect(
                Mathf.Clamp01(absoluteRect.x / w),
                Mathf.Clamp01(absoluteRect.y / h),
                Mathf.Clamp01(absoluteRect.width / w),
                Mathf.Clamp01(absoluteRect.height / h)
            );
        }
    }
}