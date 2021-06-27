using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEditor;
using UnityEngine;

namespace FFStudio
{
    public class MobileInput : MonoBehaviour
    {
#region Fields
		[ Header( "Fired Events" ) ]
		public SwipeInputEvent swipeInputEvent;
		public IntGameEvent tapInputEvent;
		public ScreenPressEvent screenPressEvent;

		[ Header( "Shared Variables" ) ]
		public SharedVector2Property inputDirection;
		public SharedFloatProperty stretchRatio;

		private int swipeThreshold;
		private LeanFingerDelegate fingerUpdate;

		// Drag Input Variables
		private Vector2 inputOrigin;

		private float input_DeadZone;
		private float input_Rotation;
		private float input_Stretch;

		private float stretch;
#endregion

#region UnityAPI
		private void Awake()
		{
			swipeThreshold = Screen.width * GameSettings.Instance.swipeThreshold / 100;
			input_DeadZone = Screen.height * GameSettings.Instance.input_threshold_DeadZone / 100;
			input_Rotation = Screen.height * GameSettings.Instance.input_threshold_Rotation / 100;
			input_Stretch  = Screen.height * GameSettings.Instance.input_threshold_Stretch / 100;
			stretch        = input_Stretch - input_Rotation;

			fingerUpdate   = FingerDown;
		}
#endregion

#region API
		public void Swiped( Vector2 delta )
		{
			swipeInputEvent.ReceiveInput( delta );
		}

		public void Tapped( int count )
		{
			tapInputEvent.eventValue = count;

			tapInputEvent.Raise();
		}

		public void LeanFingerUpdate( LeanFinger finger )
		{
			fingerUpdate( finger );
		}

		public void LeanFingerUp( LeanFinger finger )
		{
			screenPressEvent.isPressedDown  = false;
			screenPressEvent.screenPosition = finger.ScreenPosition;
			fingerUpdate                    = FingerDown;

			screenPressEvent.Raise();
			// inputDirection.InvokeValue( Vector2.zero );
		}
#endregion

#region Implementation
		private void FingerDown( LeanFinger finger ) 
		{
			fingerUpdate = FingerUpdate;
			inputOrigin  = finger.ScreenPosition;

			screenPressEvent.isPressedDown  = true;
			screenPressEvent.screenPosition = finger.ScreenPosition;

			screenPressEvent.Raise();
		}
		private void FingerUpdate( LeanFinger finger )
		{
			var input = finger.ScreenPosition - inputOrigin;

			input *= -1f;

			if( input.magnitude >= input_Rotation )
			{
				inputDirection.InvokeValue( input.normalized );
				stretchRatio.SetValue( Mathf.Lerp( 0, 1, ( input.magnitude - input_Rotation ) / (input_Stretch - input_Rotation) ) );
			}
			else if ( input.magnitude >= input_DeadZone )
			{
				inputDirection.InvokeValue( input.normalized );
				stretchRatio.SetValue( 0 );
			}
			else 
			{
				// inputDirection.InvokeValue( Vector2.zero );
				stretchRatio.SetValue( 0 );
			}
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Handles.color = Color.Lerp( Color.white, Color.red, stretchRatio.sharedValue );
			Handles.DrawLine( Vector3.zero, inputDirection.sharedValue , 3 );
		}
#endif
#endregion
    }
}