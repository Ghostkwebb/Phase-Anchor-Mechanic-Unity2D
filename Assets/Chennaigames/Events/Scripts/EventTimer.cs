using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace ChennaiGames
{
    namespace Events
    {
        public class EventTimer : MonoBehaviour
        {
            public static EventTimer instance;

            private Dictionary<string, EventDetails> timerEvents;

            private void Awake()
            {
                if(instance == null)
                {
                    instance = this;
                    DontDestroyOnLoad(this.gameObject);

                    timerEvents = new Dictionary<string, EventDetails>();
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }

            private class EventDetails
            {
                public int ExpiryTime;
                public int RemainingTimeValue;
                public UnityAction<int> RemainingTime;
                public UnityAction TimeCompleteEvent;
            }

            public void RegisterForTimer(string _type, int timeInSeconds, UnityAction<int> _remainingTime, UnityAction _onTimeComplete)
            {
                ////LoggerUtils.Log(this, "timerEvents has key::"+timerEvents.ContainsKey(_type));
                if(timerEvents.ContainsKey(_type))
                {
                    timerEvents[_type].ExpiryTime = (int)(Time.realtimeSinceStartup + timeInSeconds);
                    timerEvents[_type].RemainingTime = _remainingTime;
                    timerEvents[_type].TimeCompleteEvent = _onTimeComplete;
                }
                else
                {
                    var eventDetails = new EventDetails();
                    eventDetails.ExpiryTime = (int)(Time.realtimeSinceStartup + timeInSeconds);
                    eventDetails.RemainingTime = _remainingTime;
                    eventDetails.TimeCompleteEvent = _onTimeComplete;
                    timerEvents.Add(_type, eventDetails);
                }

                timerEvents[_type].RemainingTimeValue = (int)(timerEvents[_type].ExpiryTime - Time.realtimeSinceStartup);
                timerEvents[_type].RemainingTime?.Invoke(timerEvents[_type].RemainingTimeValue);
            }

            public void SubscribeForTimer(string _type, UnityAction<int> _remainingTime, UnityAction _onTimeComplete)
            {
                if(timerEvents.ContainsKey(_type))
                {
                    timerEvents[_type].RemainingTime += _remainingTime;
                    timerEvents[_type].TimeCompleteEvent += _onTimeComplete;
                }
            }

            public void UnSubscribeForTimer(string _type, UnityAction<int> _remainingTime, UnityAction _onTimeComplete)
            {
                if(timerEvents.ContainsKey(_type))
                {
                    timerEvents[_type].RemainingTime -= _remainingTime;
                    timerEvents[_type].TimeCompleteEvent -= _onTimeComplete;
                }
            }

            public int ExpiryTime(string _key)
            {
                if(timerEvents.TryGetValue(_key, out var _data))
                {
                    return _data.ExpiryTime;
                }

                return 0;
            }

            public int RemainingTime(string _key)
            {
                if (timerEvents.TryGetValue(_key, out var _data))
                {
                    return _data.RemainingTimeValue;
                }

                return 0;
            }

            public void RemoveEvent(string _type)
            {
                if (timerEvents.ContainsKey(_type))
                {
                    timerEvents[_type].RemainingTime = null;
                    timerEvents[_type].TimeCompleteEvent = null;
                    timerEvents.Remove(_type);
                }
            }

            public void UpdateEvents(string _type, UnityAction<int> _remainingTime, UnityAction _onTimeComplete)
            {
                if (timerEvents.ContainsKey(_type))
                {
                    timerEvents[_type].RemainingTime = _remainingTime;
                    timerEvents[_type].TimeCompleteEvent = _onTimeComplete;
                }
            }

            private float _time;
            private void Update()
            {
                if(_time + 1f < Time.realtimeSinceStartup)
                {
                    _time = Time.realtimeSinceStartup;
                    foreach(var _event in timerEvents)
                    {
                        if(_event.Value.ExpiryTime < Time.realtimeSinceStartup)
                        {
                            _event.Value.RemainingTimeValue = 0;
                            _event.Value.TimeCompleteEvent?.Invoke();
                            RemoveEvent(_event.Key);
                            break;//skip this frame - Exception throws in next iteration if remove element
                        }
                        else
                        {
                            _event.Value.RemainingTimeValue = (int)(_event.Value.ExpiryTime - Time.realtimeSinceStartup);
                            _event.Value.RemainingTime?.Invoke(_event.Value.RemainingTimeValue);
                        }
                    }
                }
            }
        }
    }
    public enum TimerEventType
    {
        OTP_ResendTime,
        Maintenance,
        DailyMissions,
        MultiplayerMatchMaking,
        MultiplayerErrorTime,
        SpinWheel,
        DailyBonusTimer,
        ShopOfferTimer,
        UserReconnectTimer
    }
}


