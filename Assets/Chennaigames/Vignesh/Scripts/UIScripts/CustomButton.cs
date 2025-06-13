using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CustomButton : CustomUISelectable
{
    
    [Tooltip("If set true then when button is clicked, doesn't react to any hover state until unless this was false")]
    [SerializeField] protected bool isButtonSelectable;

    [SerializeField] protected Button button;
    [SerializeField] protected RectTransform button_Rect;
    private Sequence sq;

    public Button.ButtonClickedEvent onClick = new Button.ButtonClickedEvent();
    
    protected override void Awake()
    {
        base.Awake();
        if (!button)
            button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        isSelectedUI = false;
    }
    protected override void OnDisable()
    {
        sq?.Kill();
    }

    public override void SetInteractable(bool isInteractable)
    {
        if (button) button.interactable = isInteractable;
        base.SetInteractable(isInteractable);
    }

    public override void OnGlobalCoolDownAction(bool isStarted)
    {
        base.OnGlobalCoolDownAction(isStarted);
        if (button) button.interactable = isStarted && isInteractable;
    }

    protected void OnClick()
    {
        if (!doubleClickCoolDown.IsCoolDownComplete) return;

        doubleClickCoolDown.StartCoolDown();

        if (isButtonSelectable)
            isSelectedUI = true;
        else
        {
            //ToggleGraphicsWhenHover(false);
            ToggleGraphicsWhenSelect(true);
        }
        onClick.Invoke();

        if(canPlayDefaultAudio)
            // SoundsManager.instance.Play(SoundsName.CommonBtn_Sound);

        
        if(canPlayAnimation)
        {
            sq = DOTween.Sequence();
            sq.Append(button_Rect.DOPunchScale(new Vector3(0.1f,0.1f,0.1f), 0.3f, 10, 1));
        }
    }

}
