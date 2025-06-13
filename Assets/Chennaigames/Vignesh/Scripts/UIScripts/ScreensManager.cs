using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreensManager : MonoBehaviour
{
    public static ScreensManager instance = null;
    public Dictionary<ScreenName, Screen_Base> screensInScene;
    [SerializeField] private GameObject screenHolder;
    private Stack<ScreenName> activeScreens;
    [HideInInspector] public ScreenName currentScreen;
    private ScreenName previousScreen;
    public ScreenName GetPreviousScreen{ get{return previousScreen;} }
    public int shopSelection_index;
    public int navBarToggle_index;
    public bool isLoadingOpened { get; private set; } = false;
    public static (ScreenName, string) deepLinkScreen;

    public GameObject dummy_loader;

    void Awake() 
    {
        Debug.Log("************ScreenManager_Awake********");
        if(instance == null)
        {
            instance = this;
        }
        screensInScene = new Dictionary<ScreenName, Screen_Base>();
        activeScreens = new Stack<ScreenName>();
        screenHolder.SetActive(true);
    }

   
    IEnumerator Start() 
    {
        ShowLoading();
        yield return new WaitForSeconds(3.0f);
        ShowScreen(ScreenName.LandingScreen);
        HideLoading();
    
    }
    public void ShowScreen(ScreenName _screen)
    {
        Debug.Log("Show screen :::: Screename ::: "+_screen);
        CheckForCurrentScreenAndShow(_screen);
    }
    private void CheckForCurrentScreenAndShow(ScreenName _screen)
    {
        if (currentScreen != ScreenName.None && _screen != ScreenName.LoadingScreen && _screen != ScreenName.MiniLoadingScreen && (screensInScene[_screen].showAsPopup == false || screensInScene[currentScreen].overLapPopup))
        {
            // screensInScene[_screen].OnInitiateShow();
            screensInScene[currentScreen].NextScreen(_screen);
            screensInScene[currentScreen].Hide(() =>
            {
                screensInScene[currentScreen].isActive = false;

                if (screensInScene[currentScreen].overLapPopup && (screensInScene[currentScreen].showAsPopup || (!screensInScene[_screen].showAsPopup && activeScreens.Contains(_screen))))
                {
                    //LoggerUtils.Log(this, "activeScreens:CheckForCurrentScreenAndShow:pop:", activeScreens.Peek().ToString());
                    activeScreens.Pop();
                }
                else
                {
                    previousScreen = currentScreen;
                }
                currentScreen = _screen;

                ShowThisScreen(_screen);
            });
        }
        else
        {
            ShowScreenOnTop(_screen);
        }
        // ShowScreenOnTop(_screen);
    }
    public void ShowScreenOnTop(ScreenName _screen)
    {
        screensInScene[_screen].OnInitiateShow();

        if (currentScreen != ScreenName.None && _screen != ScreenName.LoadingScreen)// && _screen != ScreenName.MiniLoadingScreen)
        {
            screensInScene[currentScreen].GoToBackground(_screen);
        }

        if (_screen != ScreenName.LoadingScreen)
        {
            previousScreen = currentScreen; 
        }
        currentScreen = _screen;

        ShowThisScreen(_screen);
    }
    private void ShowThisScreen(ScreenName _screen)
    {
        if (activeScreens.Contains(_screen))
        {
            screensInScene[_screen].isActive = true;
            screensInScene[_screen].ShowAgain(previousScreen);
        }
        else
        {
            activeScreens.Push(_screen);

            screensInScene[_screen].isActive = true;
            screensInScene[_screen].Show();
        }

        if (_screen == ScreenName.LoadingScreen || _screen == ScreenName.MiniLoadingScreen)
        {
            screensInScene[_screen].transform.SetAsLastSibling();
        }
    }
    public void ShowPreviousScreen()
    {
        if (currentScreen == ScreenName.LandingScreen)
        {
            //Show exit popup
            // Debug.Log("Show exit popup");
            // PopUpManager.instance.ShowPopup(msg: "DO YOU WANT QUIT?", title: "INFO", yesText: "YES", noText: "NO", yesAction: ()=>
            // {
            //     Debug.Log("ShowPopup Yes Action");
            //     GameManager.instance.QuitGame();
            // });
        }
        else
        // else if (currentScreen != ScreenName.LoadingScreen && currentScreen != ScreenName.MiniLoadingScreen && screensInScene.ContainsKey(previousScreen))
        // else if (screensInScene.ContainsKey(previousScreen))
        {
            if (screensInScene.TryGetValue(currentScreen, out Screen_Base thisScreen))
            {
                // screensInScene[previousScreen].OnInitiateShow();
                thisScreen.NextScreen(previousScreen);
                thisScreen.Hide(() =>
                {
                    thisScreen.isActive = false;

                    //LoggerUtils.Log(this, "activeScreens:ShowPreviousScreen:pop:", activeScreens.Peek().ToString());
                    activeScreens.Pop();
                    previousScreen = currentScreen;
                    //LoggerUtils.Log(this, "activeScreens:ShowPreviousScreen:Peek:", activeScreens.Peek().ToString());
                    currentScreen = activeScreens.Peek();

                    if (currentScreen != ScreenName.None)
                    {
                        screensInScene[currentScreen].isActive = true;

                        if (thisScreen.showAsPopup || activeScreens.Contains(currentScreen))
                        {
                            // if(previousScreen != ScreenName.CoinAnimationScreen)
                            // {
                            //     screensInScene[currentScreen].ShowAgain(previousScreen);
                            // }
                        }
                        else
                        {
                            screensInScene[currentScreen].Show();
                        }
                    }
                });
            }
        }
    }
    public void ShowLoading(bool isMiniLoading = false, string customTxt = "")
    {
        Debug.Log("*******ScreenManager_ShowLoading******"+isMiniLoading);
        if (screensInScene[isMiniLoading ? ScreenName.MiniLoadingScreen : ScreenName.LoadingScreen].isActive == true) return;

        isLoadingOpened = true;
        if (!isMiniLoading)// && !string.IsNullOrEmpty(customTxt))
        {
            SendMessageToScreen(ScreenName.LoadingScreen, customTxt);
            //GetScreen<LoadingScreen>(ScreenName.LoadingScreen)?.updateCustomTxt(customTxt);
        }
        screensInScene[isMiniLoading ? ScreenName.MiniLoadingScreen : ScreenName.LoadingScreen].isActive = true;
        screensInScene[isMiniLoading ? ScreenName.MiniLoadingScreen : ScreenName.LoadingScreen].Show();

        screensInScene[ScreenName.MiniLoadingScreen].transform.SetAsLastSibling();
        screensInScene[ScreenName.LoadingScreen].transform.SetAsLastSibling();
    }

    public void HideLoading()
    {

        if (screensInScene[ScreenName.MiniLoadingScreen].isActive == true)
        {
            isLoadingOpened = false;
            screensInScene[ScreenName.MiniLoadingScreen].Hide(null);
            screensInScene[ScreenName.MiniLoadingScreen].isActive = false;
        }

        if (screensInScene[ScreenName.LoadingScreen].isActive == true)
        {
            isLoadingOpened = false;
            screensInScene[ScreenName.LoadingScreen].Hide(null);
            screensInScene[ScreenName.LoadingScreen].isActive = false;
        }

    }
    private UnityAction _OnHideAnimComplete;

    public void RegisterHideAnimCompleteEvent(UnityAction _action)
    {
        _OnHideAnimComplete = _action;
    }
    public void HideTopScreen(bool isNeedShowAgain = false)
    {
        var topScreen = activeScreens.Peek();
        if (topScreen != ScreenName.None)
        {
            screensInScene[topScreen].Hide(() =>
            {
                screensInScene[topScreen].isActive = false;

                activeScreens.Pop();

                var temp = currentScreen;

                if (activeScreens.Count == 0)
                {
                    currentScreen = ScreenName.None;
                }
                else
                {
                    currentScreen = activeScreens.Peek();
                }

                if (currentScreen != ScreenName.LoadingScreen && currentScreen != ScreenName.MiniLoadingScreen)
                {
                    previousScreen = temp;
                    if (isNeedShowAgain && activeScreens.Contains(currentScreen))
                    {
                        screensInScene[currentScreen].isActive = true;
                        screensInScene[currentScreen].ShowAgain(previousScreen);
                    }
                }

                _OnHideAnimComplete?.Invoke();
                _OnHideAnimComplete = null;
            });
        }
    }
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !isLoadingOpened)
        {
            // screensInScene[currentScreen].DeviceBack();
            // if (PopUpManager.instance.isActive)
            // {
            //     PopUpManager.instance.OnClick_Close();
            // }
            // else if (currentScreen != ScreenName.None)
            // {
            //     screensInScene[currentScreen].DeviceBack();
            // }
        }
    }
    public void RegisterScreen(ScreenName _screen, Screen_Base _thisScreen)
    {
        if (screensInScene.ContainsKey(_screen) == false)
        {
            screensInScene.Add(_screen, _thisScreen);
        }
    }
    public T GetScreen<T>(ScreenName _screen) where T : Screen_Base
    {
        return screensInScene[_screen] as T;
    }
    public void SendMessageToScreen(ScreenName _name, params object[] _params)
    {
        screensInScene[_name].OnReceiveMessage(_params);
        _params = null;
    }
}
// public enum SceneName
// {
//     None,
//     LandingScene,
//     GamePlayScene,
// }


public enum ScreenName : byte
{
    None,
    LandingScreen,
    LoadingScreen,
    MiniLoadingScreen,
    GamePlayScreen,
    ResultScreen
}
