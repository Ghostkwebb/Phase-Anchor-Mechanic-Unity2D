using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CoolDownTimer
{
    public float coolDownAmount = 0.5f;

    private float m_coolDownCompleteTime;// { get; private set; }

    public bool IsCoolDownComplete => Time.unscaledTime > m_coolDownCompleteTime;

    public Action<bool> OnCompleteGlobalAction;

    public MonoBehaviour GlobalContext;

    public void StartCoolDown()
    {
        if (GlobalContext) OnCompleteGlobalAction?.Invoke(false);
        ////Degub.Log("ClickCoolDown::Time.unscaledTime::" + Time.unscaledTime);
        ////Degub.Log("ClickCoolDown::m_coolDownCompleteTime:before:" + m_coolDownCompleteTime);
        m_coolDownCompleteTime = Time.unscaledTime + coolDownAmount;
        ////Degub.Log("ClickCoolDown::m_coolDownCompleteTime:after:" + m_coolDownCompleteTime);
        if (GlobalContext && GlobalContext.gameObject.activeSelf)
        {
            GlobalContext.StartCoroutine(Cooldown(() => {
                OnCompleteGlobalAction?.Invoke(true);
            }));
        }
        else
        {
            OnCompleteGlobalAction?.Invoke(true);
        }
    }

    public void StartCoolDown(MonoBehaviour context, Action OnCompleteAction)
    {
        StartCoolDown();
        context.StartCoroutine(Cooldown(OnCompleteAction));
    }

    IEnumerator Cooldown(Action OnCompleteAction)
    {
        yield return new WaitUntil(()=> IsCoolDownComplete);

        OnCompleteAction?.Invoke();
    }

}
