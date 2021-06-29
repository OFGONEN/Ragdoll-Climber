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

	protected override void OnEnable()
	{
		base.OnEnable();
		screenPressListener.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		screenPressListener.OnDisable();
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		screenPressListener.OnDisable();
		UnSubscribeProperties();
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

			SubscribeProperties();
		}
		else if ( stretchRatioProperty.sharedValue >= 0.1f )
		{
			UnSubscribeProperties();
			ReadyToLaunch();
			stretchRatioProperty.SetValue( 1 );
		}
		else 
		{
			UnSubscribeProperties();
			DefaultTheRagdoll();
		}
	}

	private void ScreenPressResponse_HandsFree()
	{
		screenPressListener.response = TryToAttachHands;
	}

	private void SubscribeProperties()
	{
		inputDirectionProperty.changeEvent += OnInputDirectionChange;
		stretchRatioProperty.changeEvent   += OnStretchRationChange;
	}

	private void UnSubscribeProperties()
	{
		inputDirectionProperty.changeEvent -= OnInputDirectionChange;
		stretchRatioProperty.changeEvent   -= OnStretchRationChange;
	}

	protected override void LevelStartResponse()
	{
		screenPressEvent = screenPressListener.gameEvent as ScreenPressEvent;
		screenPressListener.OnEnable();
		SubscribeProperties();
	}

	protected override void ReleaseHands()
	{
		base.ReleaseHands();

		screenPressListener.response = ScreenPressResponse_HandsFree;
	}

	protected override void OnHandsAttached()
	{
		screenPressListener.response = ScreenPressResponse_HandsAttached;
		stretchRatioProperty.SetValue( 0 );

		if( currentWayPoint + 1 == platformSet.itemDictionary.Count / 2 )
		{
			screenPressListener.response = ExtensionMethods.EmptyMethod;
			UnSubscribeProperties();
			levelComplete.Raise();
		}
	}
#endregion
}
