using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Screen_Base_Anim : Screen_Base
{
    public RectTransform content_Rect;
    public Image panelBg_Img;
    private Sequence sq;
    protected override void Awake()
    {
        panelBg_Img.DOFade(0.0f, 0.0f);
        content_Rect.anchoredPosition = new Vector2(0.0f, 1800.0f);
        base.Awake();
    }
    public override void Show()
    {
        InAnimation();
        base.Show();
    }

    public override void Hide(UnityAction OnAnimComplete)
    {
        sq = DOTween.Sequence().SetUpdate(true);

        // sq.Append(panelBg_Img.DOFade(0.0f, 0.3f))
        //     .Join(content_Rect.DOAnchorPosY(1800.0f, 0.3f).SetEase(Ease.InBack)).OnComplete(()=>
        //     {
        //         base.Hide(OnAnimComplete);
        //     });

        sq.Append(content_Rect.DOAnchorPosY(1800.0f, 0.3f).SetEase(Ease.InBack))
        .Append(panelBg_Img.DOFade(0.0f, 0.1f))
        .OnComplete(()=>
        {
            sq?.Kill();
            base.Hide(OnAnimComplete);
        });
    }
    private void InAnimation()
    {
        sq = DOTween.Sequence().SetUpdate(true);

        sq.Append(panelBg_Img.DOFade(0.85f, 0.5f))
            .Join(content_Rect.DOAnchorPosY(0.0f, 0.5f).SetEase(Ease.OutBack));
    }
    private void OnDestroy() 
    {
        if(sq != null)
        {
            sq.Kill();
            sq = null;
        }
        panelBg_Img = null;
        content_Rect = null;
    }
}
