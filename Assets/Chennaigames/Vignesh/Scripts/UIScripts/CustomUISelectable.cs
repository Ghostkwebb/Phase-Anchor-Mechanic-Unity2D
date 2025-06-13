using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomUISelectable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler//, IDragHandler, IEndDragHandler
{
    [SerializeField] protected GameObject defaultObj;
    [SerializeField] protected GameObject pressedObj;
    [SerializeField] protected GameObject hoverObj;
    [SerializeField] protected GameObject disableObj;
    [SerializeField] protected Color defaultColor;
    [SerializeField] protected Color pressedColor;
    [SerializeField] protected Color hoveredColor;
    [SerializeField] protected Color disabledColor;

    [SerializeField] protected TMP_Text text;

    [SerializeField] protected bool shouldChangeText = false;
    [SerializeField] protected bool canPlayDefaultAudio = true;
    [SerializeField] protected bool canPlayAnimation = false;

    [SerializeField] protected CoolDownTimer doubleClickCoolDown;

    protected bool isSelectedUI = false;
    protected bool isInteractable = true;
    protected bool isPointerEnterOnUI = false;

    UnityAction<bool> onInteractableAction;
    UnityAction<PointerEventData> onPointerDownAction;
    UnityAction<PointerEventData> onPointerUpAction;
    UnityAction<PointerEventData> onPointerEnterAction;
    UnityAction<PointerEventData> onPointerExitAction;

    private void Start()
    {
        // SetGlobalCoolDown(GameManager.instance.globalCoolDown);
    }


    public virtual void SetGlobalCoolDown(CoolDownTimer coolDownTimer)
    {
        this.doubleClickCoolDown = coolDownTimer;

        this.doubleClickCoolDown.OnCompleteGlobalAction += OnGlobalCoolDownAction;
    }

    public virtual void OnGlobalCoolDownAction(bool isStarted)
    {
        ////Degub.Log("ClickCoolDown::" + gameObject.name + "::OnGlobalCoolDownAction::completed");
    }

    public virtual void SetInteractable(bool isInteractable)
    {
        this.isInteractable = isInteractable;

        if (disableObj)
        {
            disableObj.SetActive(!isInteractable);
            if (defaultObj && pressedObj && defaultObj == pressedObj)
            {
                defaultObj.SetActive(isInteractable);
            }
            else
            {
                if (defaultObj) defaultObj.SetActive(isInteractable);
                if (pressedObj) pressedObj.SetActive(false);
            }
        }
        else
        {
            if (defaultObj && pressedObj && defaultObj == pressedObj)
            {
                defaultObj.SetActive(!isInteractable);
            }
            else
            {
                if (defaultObj) defaultObj.SetActive(!isInteractable);
                if (pressedObj) pressedObj.SetActive(false);
            }
            //if (disableObj) disableObj.SetActive(false);
        }

        if (hoverObj) hoverObj.SetActive(false);

        if (text)
        {
            if (isInteractable)
            {
                text.color = defaultColor;
            }
            else
            {
                text.color = disabledColor;
            }
        }

        onInteractableAction?.Invoke(isInteractable);
    }


    protected virtual void ToggleGraphicsWhenSelect(bool isOn)
    {
        if (defaultObj && pressedObj && defaultObj == pressedObj)
        {
            defaultObj.SetActive(isOn);
        }
        else
        {
            if (defaultObj) defaultObj.SetActive(!isOn);
            if (pressedObj) pressedObj.SetActive(isOn);
        }

        if (disableObj) disableObj.SetActive(false);
        if (hoverObj) hoverObj.SetActive(false);

        if (shouldChangeText && text)
        {
            if (!isOn)
            {
                text.color = defaultColor;
            }
            else
            {
                text.color = pressedColor;
            }
        }
    }

    protected virtual void ToggleGraphicsWhenHover(bool isOn)
    {
        if (hoverObj)
        {
            if (this.isInteractable)
            {
                if (defaultObj && pressedObj && defaultObj == pressedObj)
                {
                    defaultObj.SetActive(!isOn);
                }
                else
                {
                    if (defaultObj) defaultObj.SetActive(!isOn);
                    if (pressedObj) pressedObj.SetActive(false);
                }
                if (disableObj) disableObj.SetActive(false);
            }
            else
            {
                if (disableObj) disableObj.SetActive(true);
                if (defaultObj && pressedObj && defaultObj == pressedObj)
                {
                    defaultObj.SetActive(false);
                }
                else
                {
                    if (defaultObj) defaultObj.SetActive(false);
                    if (pressedObj) pressedObj.SetActive(false);
                }
            }

            hoverObj.SetActive(isOn);

            //if (hoverObj) hoverObj.SetActive(isOn);

            if (shouldChangeText && text)
            {
                if (!isOn)
                {
                    text.color = defaultColor;
                }
                else
                {
                    text.color = hoveredColor;
                }
            }
        }
    }

    protected virtual void Awake()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {
        isPointerEnterOnUI = false;
        ToggleGraphicsWhenHover(false);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!isInteractable) return;

        //ToggleGraphicsWhenSelect(true);

        onPointerDownAction?.Invoke(eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!isInteractable) return;

        //ToggleGraphicsWhenSelect(false);

        onPointerUpAction?.Invoke(eventData);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable || isSelectedUI) return;

        isPointerEnterOnUI = true;
        ToggleGraphicsWhenHover(true);

        onPointerEnterAction?.Invoke(eventData);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable || isSelectedUI) return;

        isPointerEnterOnUI = false;
        ToggleGraphicsWhenHover(false);

        onPointerExitAction?.Invoke(eventData);
    }

    public void SetText(string value)
    {
        text.SetText(value);
    }
}
