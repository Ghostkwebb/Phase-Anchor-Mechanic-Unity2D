using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Screen_Base : MonoBehaviour
{
    [SerializeField] protected ScreenName _screenName;
    public bool showAsPopup;
    public bool overLapPopup;


    [HideInInspector]
    public bool isActive;

    public virtual void OnInitiateShow()
    {

    }
    protected virtual void Awake()
    {
        ScreensManager.instance.RegisterScreen(_screenName, this);
        this.gameObject.SetActive(false);
    }
    public virtual void Show()
    {
        this.gameObject.SetActive(true);
        // InAnimation();
    }
    public virtual void ShowAgain(ScreenName prevScreen)
    {
        Show();
    }
    public virtual void Hide(UnityAction OnAnimationComplete)
    {
        this.gameObject.SetActive(false);
        OnAnimationComplete?.Invoke();
        OnAnimationComplete = null;
    }
    public virtual void GoToBackground(ScreenName topScreen)
    {

    }

    public virtual void NextScreen(ScreenName nextScreen)
    {

    }
    public virtual void DeviceBack()
    {
        // OutAnimation();
        // if(_screenName != ScreenName.CoinAnimationScreen)
        // SoundsManager.instance.Play(SoundsName.CloseBtn_Sound);
        ScreensManager.instance.ShowPreviousScreen();
    }
    public virtual void OnReceiveMessage(params object[] _params)
    {

    }
}
