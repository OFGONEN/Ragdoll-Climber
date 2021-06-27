/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using FFStudio;
using UnityEngine;

public class Player : Actor
{
	#region Fields
	[Header( "Event Listener" )]
	public EventListenerDelegateResponse screenPressListener;

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

			inputDirectionProperty.changeEvent += OnInputDirectionChange;
			stretchRatioProperty.changeEvent   += OnStretchRationChange;
		}
		else if ( stretchRatioProperty.sharedValue >= 0.1f )
		{
			inputDirectionProperty.changeEvent -= OnInputDirectionChange;
			stretchRatioProperty.changeEvent   -= OnStretchRationChange;

			ReadyToLaunch();
		}
		else 
		{
			inputDirectionProperty.changeEvent -= OnInputDirectionChange;
			stretchRatioProperty.changeEvent   -= OnStretchRationChange;

			DefaultTheRagdoll();
		}
	}

	private void ScreenPressResponse_HandsFree()
	{
		screenPressListener.response = TryToAttachHands;
	}
	
	protected override void ReleaseHands()
	{
		base.ReleaseHands();

		screenPressListener.response = ScreenPressResponse_HandsFree;
	}

	protected override void OnHandsAttached()
	{
		screenPressListener.response = ScreenPressResponse_HandsAttached;
	}
#endregion
}
