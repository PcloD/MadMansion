using System;
using System.Collections;
using UnityEngine;
using InControl;

namespace InputProfiles
{
	// This custom profile is enabled by adding it to the Custom Profiles list
	// on the InControlManager component, or you can attach it yourself like so:
	// InputManager.AttachDevice( new UnityInputDevice( "KeyboardAndMouseProfile" ) );
	//
	public class KeyboardAndMouseProfile : UnityInputDeviceProfile
	{
		public KeyboardAndMouseProfile()
		{
			Name = "Keyboard/Mouse";
			Meta = "A keyboard profile.";

			// This profile only works on desktops.
			SupportedPlatforms = new[]
			{
				"Windows",
				"Mac",
				"Linux"
			};

			Sensitivity = 1.0f;
			LowerDeadZone = 0.0f;
			UpperDeadZone = 1.0f;

			ButtonMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Switch/Listen",
					Target = InputControlType.Action1,
					Source = KeyCodeButton(KeyCode.X)
				},
				new InputControlMapping
				{
					Handle = "Haunt",
					Target = InputControlType.Action3,
					Source = KeyCodeButton(KeyCode.Z)
				},
				new InputControlMapping
				{
					Handle = "Catch",
					Target = InputControlType.Action4,
					Source = KeyCodeButton(KeyCode.Z)
				},
			};

			AnalogMappings = new[]
			{
				new InputControlMapping {
					Handle = "Move X",
					Target = InputControlType.LeftStickX,
					Source = KeyCodeAxis( KeyCode.LeftArrow, KeyCode.RightArrow )
				},
				new InputControlMapping {
					Handle = "Move Y",
					Target = InputControlType.LeftStickY,
					Source = KeyCodeAxis( KeyCode.DownArrow, KeyCode.UpArrow )
				},
			};
		}
	}
}
