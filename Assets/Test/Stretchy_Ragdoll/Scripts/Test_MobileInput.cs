using UnityEngine;
using Lean.Touch;

namespace FFStudio
{
    public class Test_MobileInput : MonoBehaviour
    {
#region Fields
		[ Header( "Fired Events" ) ]
		public SwipeInputEvent swipeInputEvent;
		public IntGameEvent tapInputEvent;
		public BoolGameEvent screenTapEvent;

		[ Header( "Shared Variables" ) ]
		public SharedVector3 shared_InputDirection;

	//* Private Fields *//
		private Vector2 inputOrigin;
		private LeanFingerDelegate fingerUpdate;
		
		private int swipeThreshold;
#endregion
		
#region Unity API
		private void Awake()
		{
			shared_InputDirection.sharedValue = Vector3.zero;
			inputOrigin = Vector2.zero;

			fingerUpdate = FingerDown;
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

		public void LeanFingerUp()
		{
			fingerUpdate = FingerDown;

			inputOrigin = Vector2.zero;

			screenTapEvent.eventValue = false;
			screenTapEvent.Raise();

			shared_InputDirection.sharedValue = Vector3.zero;
		}
#endregion

#region Implementation
		private void FingerDown( LeanFinger finger )
		{
			inputOrigin = finger.ScreenPosition;
			fingerUpdate = FingerUpdate;

			shared_InputDirection.sharedValue = Vector3.zero;

			screenTapEvent.eventValue = true;
			screenTapEvent.Raise();
		}

		private void FingerUpdate( LeanFinger finger )
		{
			var diff = ( finger.ScreenPosition - inputOrigin );

			shared_InputDirection.sharedValue = diff.normalized;
		}
#endregion
	}
}