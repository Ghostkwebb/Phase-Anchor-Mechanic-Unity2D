using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class CustomMenuToggle : CustomToggle
{
    [Tooltip("Selected Image is from Pressed Gameobject to show animation effect")]
    // [SerializeField] protected Image selectedImage;
    // [SerializeField] private RectTransform selectedLineRect;
    // [SerializeField] private bool isHorizontal;
    [SerializeField] private GameObject notificationIcon;
    [SerializeField] private RectTransform notificationIcon_Rect;
    [SerializeField] private Image notificationGlow_Img;

    protected override void OnValueChanged(bool isOn)
    {
        base.OnValueChanged(isOn);

        if(isOn && notificationIcon  && notificationIcon.activeInHierarchy)
            ToggleNotificationIcon(false);
    }


    protected override void ToggleGraphicsWhenSelect(bool isOn)
    {        
        base.ToggleGraphicsWhenSelect(isOn);
    
        // if (isOn)
        // {
        //     if (selectedLineRect != null)
        //     {
        //         if (isHorizontal)
        //         {
        //             selectedLineRect.localScale = Vector3.up;
        //         }
        //         else
        //         {
        //             selectedLineRect.localScale = Vector3.right;
        //         }
        //         selectedLineRect.DOScale(1.0f, 0.3f);
        //     }
        //     selectedImage?.DOFade(1.0f, 0.2f).From(0);
        // }
    }
    Sequence sq;
    Sequence sq1;
    public void ToggleNotificationIcon(bool toShow)
    {
        notificationIcon?.SetActive(toShow);
        notificationIcon_Rect.DOAnchorPosY(0.0f, 0.0f);
        notificationGlow_Img?.DOFade(0.0f, 0.0f);
        if(toShow)
        {
            sq = DOTween.Sequence();
            sq.Append(notificationIcon_Rect.DOAnchorPosY(5f, 0.5f))
                .Append(notificationIcon_Rect.DOAnchorPosY(0.0f, 0.5f)).SetLoops(-1,LoopType.Yoyo);
            // notificationIcon_Rect.DOShakeAnchorPos(0.5f, 10, 10, 90, false).SetLoops(-1,LoopType.Restart);

            // sq1 = DOTween.Sequence().SetLoops(-1,LoopType.Restart).SetDelay(3.0f);
            // sq1.Append(notificationGlow_Img.DOFade(0.5f, 0.5f))
            //     .Append(notificationGlow_Img.DOFade(0.0f, 0.5f));
        }
        else
        {
            if(sq != null)
                sq.Kill();

            if(sq1 != null)
                sq1.Kill();
        }
    }
    protected override void OnDisable() 
    {
        if(sq != null)
        {
            sq.Kill();
        }
        base.OnDisable();
    }
}
