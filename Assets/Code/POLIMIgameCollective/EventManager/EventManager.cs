using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace POLIMIGameCollective
{
	class IntUnityEvent : UnityEvent<int> { }
	class FloatUnityEvent : UnityEvent<float> { }
	class StringUnityEvent : UnityEvent<string> { }
	class ComplexObjectUnityEvent : UnityEvent<TutorialDialogObject> { }

	
	
	public class EventManager : MonoBehaviour {

		private Dictionary <string, UnityEvent> _simpleEventDictionary;
		private Dictionary <string, UnityEvent<int>> _intEventDictionary;
		private Dictionary <string, UnityEvent<float>> _floatEventDictionary;
		private Dictionary <string, UnityEvent<string>> _stringEventDictionary;

		private Dictionary<string, UnityEvent<TutorialDialogObject>> _complexObjectEventDictionary;
		

		private static EventManager eventManager;

		public static EventManager instance
		{
			get
			{
				if (!eventManager)
				{
					eventManager = FindObjectOfType (typeof (EventManager)) as EventManager;

					if (!eventManager) 
					{
						Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
					}
					else
					{
						eventManager.Init (); 
					}
				}

				return eventManager;
			}
		}

		void Init ()
		{
			if (_simpleEventDictionary == null)
			{
				_simpleEventDictionary = new Dictionary<string, UnityEvent>();
			}
			
			if (_intEventDictionary == null)
			{
				_intEventDictionary = new Dictionary<string, UnityEvent<int>>();
			}
			
			if (_floatEventDictionary == null)
			{
				_floatEventDictionary = new Dictionary<string, UnityEvent<float>>();
			}

			if (_stringEventDictionary == null)
			{
				_stringEventDictionary = new Dictionary<string, UnityEvent<string>>();
			}
			
			if (_complexObjectEventDictionary == null)
			{
				_complexObjectEventDictionary = new Dictionary<string, UnityEvent<TutorialDialogObject>>();
			}
			
		}

		#region StartListening
		public static void StartListening (string eventName, UnityAction listener)
		{
			UnityEvent thisEvent = null;
			if (instance._simpleEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.AddListener (listener);
			} 
			else
			{
				thisEvent = new UnityEvent ();
				thisEvent.AddListener (listener);
				instance._simpleEventDictionary.Add (eventName, thisEvent);
			}
		}

		public static void StartListening (string eventName, UnityAction<int> listener)
		{
			UnityEvent<int> thisEvent = null;
			if (instance._intEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.AddListener (listener);
			} 
			else
			{
				thisEvent = new IntUnityEvent();
				thisEvent.AddListener (listener);
				instance._intEventDictionary.Add (eventName, thisEvent);
			}
		}
		
		public static void StartListening(string eventName, UnityAction<float> listener)
		{
			UnityEvent<float> thisEvent = null;
			
			if (instance._floatEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.AddListener (listener);
			} 
			else
			{
				thisEvent = new FloatUnityEvent();
				thisEvent.AddListener (listener);
				instance._floatEventDictionary.Add (eventName, thisEvent);
			}
		}

		public static void StartListening(string eventName, UnityAction<string> listener)
		{
			UnityEvent<string> thisEvent = null;
			
			if (instance._stringEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.AddListener (listener);
			} 
			else
			{
				thisEvent = new StringUnityEvent();
				thisEvent.AddListener (listener);
				instance._stringEventDictionary.Add (eventName, thisEvent);
			}
		}
		
		public static void StartListening(string eventName, UnityAction<TutorialDialogObject> listener)
		{
			UnityEvent<TutorialDialogObject> thisEvent = null;
			
			if (instance._complexObjectEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.AddListener (listener);
			} 
			else
			{
				thisEvent = new ComplexObjectUnityEvent();
				thisEvent.AddListener (listener);
				instance._complexObjectEventDictionary.Add (eventName, thisEvent);
			}
		}
		#endregion
		
		#region StopListening
		public static void StopListening (string eventName, UnityAction listener)
		{
			if (eventManager == null) return;
			UnityEvent thisEvent = null;
			if (instance._simpleEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.RemoveListener (listener);
			}
		}
		
		public static void StopListening (string eventName, UnityAction<int> listener)
		{
			if (eventManager == null) return;
			UnityEvent<int> thisEvent = null;
			if (instance._intEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.RemoveListener (listener);
			}
		}

		public static void StopListening (string eventName, UnityAction<float> listener)
		{
			if (eventManager == null) return;
			UnityEvent<float> thisEvent = null;
			if (instance._floatEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.RemoveListener (listener);
			}
		}

		public static void StopListening (string eventName, UnityAction<string> listener)
		{
			if (eventManager == null) return;
			UnityEvent<string> thisEvent = null;
			if (instance._stringEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.RemoveListener (listener);
			}
		}
		public static void StopListening (string eventName, UnityAction<TutorialDialogObject> listener)
		{
			if (eventManager == null) return;
			UnityEvent<TutorialDialogObject> thisEvent = null;
			if (instance._complexObjectEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.RemoveListener (listener);
			}
		}
		#endregion
		
		#region TriggerEvent
		public static void TriggerEvent (string eventName)
		{
			UnityEvent thisEvent = null;
			if (instance._simpleEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.Invoke ();
			}
		}
		
		public static void TriggerEvent (string eventName, int value)
		{
			UnityEvent<int> thisEvent = null;
			if (instance._intEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.Invoke (value);
			}
		}

		public static void TriggerEvent (string eventName, float value)
		{
			UnityEvent<float> thisEvent = null;
			if (instance._floatEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.Invoke (value);
			}
		}

		public static void TriggerEvent (string eventName, string value)
		{
			UnityEvent<string> thisEvent = null;
			if (instance._stringEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.Invoke (value);
			}
		}
		
		public static void TriggerEvent (string eventName, TutorialDialogObject value)
		{
			UnityEvent<TutorialDialogObject> thisEvent = null;
			if (instance._complexObjectEventDictionary.TryGetValue (eventName, out thisEvent))
			{
				thisEvent.Invoke (value);
			}
		}
		#endregion

	}

	#region EventObjects

	public class TutorialDialogObject
	{
		private string _dialog;
		private float _time;
		private bool _slowDown;
		private bool _highlightTimer;

		public TutorialDialogObject(string dialog, float time, bool slowDown, bool highlightTimer)
		{
			_dialog = dialog;
			_time = time;
			_slowDown = slowDown;
			_highlightTimer = highlightTimer;
		}

		public string GetDialog()
		{
			return _dialog;
		}

		public float GetTime()
		{
			return _time;
		}

		public bool IsSlowDown()
		{
			return _slowDown;
		}

		public bool IsTimerHighlighted()
		{
			return _highlightTimer;
		}
	}

	#endregion
	
}
