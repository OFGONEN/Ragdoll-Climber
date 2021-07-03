/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using FFStudio;
using NaughtyAttributes;
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
	public SharedFloat cameraDepthRatio;
    public SharedFloatProperty levelProgress;

	[ HorizontalLine ]
	public CameraFollowZone cameraFollowZone;


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

		actorName = "Player";
		Rank = 0;
	}

	protected override void Start()
	{
		base.Start();
		screenPressListener.OnDisable();
	}
#endregion

#region API
	public override void ResetActor()
	{
		cameraFollowZone.enabled = false;
		base.ResetActor();
	}

	protected override void ResetActorToWayPoint()
	{
		base.ResetActorToWayPoint();
		cameraFollowZone.enabled = true;
	}
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
		cameraDepthRatio.sharedValue = stretchRatioProperty.sharedValue;
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

	private void SubscribeProperties()
	{
		FFLogger.Log( "Subscribe" );
		inputDirectionProperty.changeEvent += OnInputDirectionChange;
		stretchRatioProperty.changeEvent   += OnStretchRationChange;
	}

	private void UnSubscribeProperties()
	{
		FFLogger.Log( "UnSubscribe" );
		inputDirectionProperty.changeEvent -= OnInputDirectionChange;
		stretchRatioProperty.changeEvent   -= OnStretchRationChange;
	}

	protected override void LevelStartResponse()
	{
		screenPressEvent = screenPressListener.gameEvent as ScreenPressEvent;
		screenPressListener.OnEnable();
	}

	protected override void ReleaseHands()
	{
		base.ReleaseHands();
		screenPressListener.response = TryToAttachHands;
	}

	protected override void OnHandsAttached()
	{
		screenPressListener.response = ScreenPressResponse_HandsAttached;
		stretchRatioProperty.SetValue( 0 );
		cameraDepthRatio.sharedValue = 0;

		var platformCount = platformSet.itemDictionary.Count / 2;

		levelProgress.SetValue( currentWayPoint / ( float )( platformCount - 1 ) );

		if( currentWayPoint + 1 == platformCount  ) // Way points starts at 0
		{
			screenPressListener.response = ExtensionMethods.EmptyMethod;
			UnSubscribeProperties();
			levelComplete.Raise();
		}
	}
#endregion
}
