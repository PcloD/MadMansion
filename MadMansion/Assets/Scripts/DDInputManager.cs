using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using InControl;

public class DDInputManager : MonoBehaviour
{
	public bool logDebugInfo = false;
	public bool invertYAxis = false;
	public bool enableXInput = false;
	public bool useFixedUpdate = false;
	public bool dontDestroyOnLoad = false;
	public List<string> customProfiles = new List<string> ();

	[SerializeField]
	bool
		_manualDevicesEnabled = true;
	[SerializeField]
	bool
		_forceManualDevicesOff = false;
	[SerializeField]
	GameObject
		_keyboardDebugDisplay;

	private List<UnityInputDevice> _manualDevices = new List<UnityInputDevice> ();

	void OnValidate ()
	{
		if (_keyboardDebugDisplay != null) {
			_keyboardDebugDisplay.SetActive (!_forceManualDevicesOff);
		}
	}

	void OnEnable ()
	{
		if (!_forceManualDevicesOff) {
			Debug.LogError ("Keyboard Debug Mode is On! Turn it off with 'forceManualDevicesOff' on " + gameObject.name);
		}

		_keyboardDebugDisplay.SetActive (_manualDevicesEnabled && !_forceManualDevicesOff);
		if (logDebugInfo) {
			Debug.Log ("InControl (version " + InputManager.Version + ")");
			Logger.OnLogMessage += HandleOnLogMessage;
		}

		InputManager.InvertYAxis = invertYAxis;
		InputManager.EnableXInput = enableXInput;
		InputManager.SetupInternal ();

		foreach (var className in customProfiles) {
			var classType = Type.GetType (className);
			if (classType == null) {
				Debug.LogError ("Cannot find class for custom profile: " + className);
			} else if (_manualDevicesEnabled && !_forceManualDevicesOff) {
				var customProfileInstance = Activator.CreateInstance (classType) as UnityInputDeviceProfile;
				UnityInputDevice device = new UnityInputDevice (customProfileInstance);
				_manualDevices.Add (device);
				InputManager.AttachDevice (device);
			}
		}

		if (dontDestroyOnLoad) {
			DontDestroyOnLoad (this);
		}
	}

	void ToggleManualDevicesEnabled ()
	{
		if (_manualDevicesEnabled || _forceManualDevicesOff) {
			foreach (var device in _manualDevices) {
				InputManager.DetachDevice (device);
			}
		} else {
			foreach (var device in _manualDevices) {
				InputManager.AttachDevice (device);
			}
		}
		_manualDevicesEnabled = !_manualDevicesEnabled;
		_keyboardDebugDisplay.SetActive (_manualDevicesEnabled && !_forceManualDevicesOff);
	}

	void OnDisable ()
	{
		_manualDevices.Clear ();
		InputManager.ResetInternal ();
	}


	#if UNITY_ANDROID && INCONTROL_OUYA && !UNITY_EDITOR
	void Start()
	{
		StartCoroutine( CheckForOuyaEverywhereSupport() );
	}


	IEnumerator CheckForOuyaEverywhereSupport()
	{
		while (!OuyaSDK.isIAPInitComplete())
		{
			yield return null;
		}

		OuyaEverywhereDeviceManager.Enable();
	}
	#endif


	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.BackQuote)) {
			ToggleManualDevicesEnabled ();
		}

		if (!useFixedUpdate || Mathf.Approximately (Time.timeScale, 0.0f)) {
			InputManager.UpdateInternal ();
		}
	}


	void FixedUpdate ()
	{
		if (useFixedUpdate) {
			InputManager.UpdateInternal ();
		}
	}


	void OnApplicationFocus (bool focusState)
	{
		InputManager.OnApplicationFocus (focusState);
	}


	void OnApplicationPause (bool pauseState)
	{
		InputManager.OnApplicationPause (pauseState);
	}


	void OnApplicationQuit ()
	{
		InputManager.OnApplicationQuit ();
	}


	void HandleOnLogMessage (LogMessage logMessage)
	{
		switch (logMessage.type) {
		case LogMessageType.Info:
			Debug.Log (logMessage.text);
			break;
		case LogMessageType.Warning:
			Debug.LogWarning (logMessage.text);
			break;
		case LogMessageType.Error:
			Debug.LogError (logMessage.text);
			break;
		}
	}
}