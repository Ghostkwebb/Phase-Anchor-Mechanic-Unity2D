using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class CustomToggle : CustomUISelectable
{

    [SerializeField] private bool onEnableSetOn = false;

    [SerializeField] private Toggle toggle;

    public bool isOn { get { return toggle.isOn; } set { toggle.isOn = value; } }
    public Toggle.ToggleEvent onValueChanged = new Toggle.ToggleEvent();

    protected override void Awake()
    {
        base.Awake();

        if (!toggle)
            toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (onEnableSetOn)
        {
            //toggle.isOn = true;
            SetOnWithoutNotify(true);
        }
    }

    public override void OnGlobalCoolDownAction(bool isStarted)
    {
        base.OnGlobalCoolDownAction(isStarted);
        if (toggle) toggle.interactable = isStarted;

        if (!isInteractable || isSelectedUI) return;

        if (isPointerEnterOnUI)
        {
            isPointerEnterOnUI = false;
            ToggleGraphicsWhenHover(false);
        }
    }

    protected virtual void OnValueChanged(bool isOn)
    {
        ////Degub.Log("ClickCoolDown::" + gameObject.name + "::isSelectedUI::" + isSelectedUI + "::isOn::" + isOn);

        //if (isOn && !doubleClickCoolDown.IsCoolDownComplete) return;

        if (isOn)
        {
            if (canPlayDefaultAudio)
                // SoundsManager.instance.Play(SoundsName.Toggle_Sound);
            doubleClickCoolDown.StartCoolDown();
        }

        isSelectedUI = isOn;
        ToggleGraphicsWhenSelect(isOn);
        onValueChanged?.Invoke(isOn);
    }

    public void SetOnWithoutNotify(bool isOn, bool onlyHighlight = false)
    {
        //It's important to remove the listeners before setting the toggle value, otherwise volume will change as toggle value changes
        //toggle.onValueChanged.RemoveListener(OnValueChanged);
        //toggle.isOn = isOn;
        //toggle.onValueChanged.AddListener(OnValueChanged);

        if (!onlyHighlight)
        {
            isSelectedUI = isOn;
            toggle.SetIsOnWithoutNotify(isOn);
        }
        ToggleGraphicsWhenSelect(isOn);
    }


    public override void SetInteractable(bool isInteractable)
    {
        if (toggle) toggle.interactable = isInteractable;
        base.SetInteractable(isInteractable);
    }
}
