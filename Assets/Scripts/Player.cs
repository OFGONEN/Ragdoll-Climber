/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using FFStudio;
using UnityEngine;

public class Player : Actor
{
	#region Fields
	[ Header( "Event Listener" ) ]
	public EventListenerDelegateResponse screenPressListener;

	[ Header( "Event Listener" ) ]
	public GameEvent levelComplete;

	[ Header( "Shared Variables" ) ]
	public SharedVector2Property inputDirectionProperty;
	public SharedFloatProperty stretchRatioProperty;

	// Private Fields
	private ScreenPressEvent screenPressEvent;
#endregion

#region Unity API

	private void OnEnable()
	{
		screenPressListener.OnEnable();
	}

	private void OnDisable()
	{
		screenPressListener.OnDisable();
	}

	protected override void Awake()
	{
		base.Awake();

		screenPressEvent = screenPressListener.gameEvent as ScreenPressEvent;
	}
#endregion

#region API
#endregion

#region Implementation

	private void OnInputDirectionChange()
	{
		var direction   = inputDirectionProperty.sharedValue;
		var targetAngle = Vector2.SignedAngle( Vector2.up, direction );

		RotateToTargetAngle( targetAngle );
	}

	private void OnStretchRationChange()
	{
		Stretch( stretchRatioProperty.sharedValue );
	}

	private void ScreenPressResponse_HandsAttached()
	{
		if( screenPressEvent.isPressedDown )
		{
			StraightenUpRagdoll();

			SubscribeInput();
		}
		else if ( stretchRatioProperty.sharedValue >= 0.1f )
		{
			UnSubscribeInput();
			ReadyToLaunch();
		}
		else 
		{
			UnSubscribeInput();
			DefaultTheRagdoll();
		}
	}

	private void ScreenPressResponse_HandsFree()
	{
		screenPressListener.response = TryToAttachHands;
	}

	private void SubscribeInput()
	{
		inputDirectionProperty.changeEvent += OnInputDirectionChange;
		stretchRatioProperty.changeEvent   += OnStretchRationChange;
	}

	private void UnSubscribeInput()
	{
		inputDirectionProperty.changeEvent -= OnInputDirectionChange;
		stretchRatioProperty.changeEvent   -= OnStretchRationChange;
	}

	protected override void ReleaseHands()
	{
		base.ReleaseHands();

		screenPressListener.response = ScreenPressResponse_HandsFree;
	}

	protected override void OnHandsAttached()
	{
		screenPressListener.response = ScreenPressResponse_HandsAttached;

		if( currentWayPoint + 1 == platformSet.itemDictionary.Count / 2 )
		{
			UnSubscribeInput();
			levelComplete.Raise();
		}
	}
#endregion
}
