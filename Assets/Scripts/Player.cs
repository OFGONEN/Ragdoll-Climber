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

	[ Header( "Fired Events" ) ]
	public GameEvent levelComplete;

	[ Header( "Shared Variables" ) ]
	public SharedVector2Property inputDirectionProperty;
	public SharedFloatProperty stretchRatioProperty;
	public SharedFloat cameraDepthRatio;
	public SharedFloat camera_CurrentDepthRatio;
	public SharedBool playerOnAir;
	public SharedFloatProperty levelProgress;

	[ HorizontalLine ]
	public CameraFollowZone cameraFollowZone;


	// Private Fields
	private ScreenPressEvent screenPressEvent;

	private bool freezeCameraRatio;
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
		inputDirectionProperty.changeEvent -= OnInputDirectionChange_WithoutFingerUp;
	}
#endregion

#region API
	public override void ResetActor()
	{
		cameraFollowZone.StopFollow();
		base.ResetActor();
	}

	protected override void ResetActorToWayPoint()
	{
		base.ResetActorToWayPoint();
		cameraFollowZone.StartFollow( ActorPosition );
	}
#endregion

#region Implementation

	private void OnInputDirectionChange()
	{
		OffsetTheRagdoll();

		var direction   = inputDirectionProperty.sharedValue;
		var targetAngle = Vector2.SignedAngle( Vector2.up, direction );

		RotateToTargetAngle( targetAngle );
	}

	private void OnInputDirectionChange_WithoutFingerUp()
	{
		inputDirectionProperty.changeEvent -= OnInputDirectionChange_WithoutFingerUp;

		if( !Mathf.Approximately( camera_CurrentDepthRatio.sharedValue, 0 ) )
		{
			cameraDepthRatio.sharedValue = camera_CurrentDepthRatio.sharedValue;
			freezeCameraRatio = true;
		}

		StraightenUpRagdoll();
		SubscribeProperties();
		screenPressListener.response = ScreenPressResponse_HandsAlreadyAttached;
	}

	private void OnStretchRationChange()
	{
		Stretch( stretchRatioProperty.sharedValue );
		
		if( freezeCameraRatio && stretchRatioProperty.sharedValue > cameraDepthRatio.sharedValue )
		{
			freezeCameraRatio = false;
			cameraDepthRatio.sharedValue = stretchRatioProperty.sharedValue;
		}
		else 
			cameraDepthRatio.sharedValue = stretchRatioProperty.sharedValue;
	}

	private void ScreenPressResponse_HandsFree()
	{
		if( screenPressEvent.isPressedDown )
			TryToAttachHands();
	}

	private void ScreenPressResponse_HandsFirstAttached()
	{
		if( screenPressEvent.isPressedDown )
		{
			inputDirectionProperty.changeEvent -= OnInputDirectionChange_WithoutFingerUp;

			StraightenUpRagdoll();
			SubscribeProperties();
			screenPressListener.response = ScreenPressResponse_HandsAlreadyAttached;

			if( !Mathf.Approximately( camera_CurrentDepthRatio.sharedValue, 0 ) )
			{
				cameraDepthRatio.sharedValue = camera_CurrentDepthRatio.sharedValue;
				freezeCameraRatio = true;
			}
		}
	}

	private void ScreenPressResponse_HandsAlreadyAttached()
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
			stretchRatioProperty.SetValue( 0 );
		}
		else 
		{
			UnSubscribeProperties();
			DefaultTheRagdoll();

			freezeCameraRatio = false;
			cameraDepthRatio.sharedValue = 0;
		}
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
		actor_Participate_Race.eventValue = this;
		actor_Participate_Race.Raise();

		screenPressEvent = screenPressListener.gameEvent as ScreenPressEvent;
		screenPressListener.OnEnable();
	}

	protected override void ReleaseHands()
	{
		base.ReleaseHands();
		screenPressListener.response = ScreenPressResponse_HandsFree;
		playerOnAir.sharedValue      = true;
	}

	protected override void OnHandsAttached()
	{
		base.OnHandsAttached();
		particleSpawnEvent.Raise();

		playerOnAir.sharedValue      = false;
		cameraDepthRatio.sharedValue = 0;
		stretchRatioProperty.SetValue( 0 );

		var platformCount = platformSet.itemDictionary.Count / 2;

		levelProgress.SetValue( currentWayPoint / ( float )( platformCount - 1 ) );

		if( currentWayPoint + 1 == platformCount  ) // Way points starts at 0
		{
			screenPressListener.response = ExtensionMethods.EmptyMethod;
			UnSubscribeProperties();
			levelComplete.Raise();
		}
		else 
		{
			screenPressListener.response = ScreenPressResponse_HandsFirstAttached;
			inputDirectionProperty.changeEvent += OnInputDirectionChange_WithoutFingerUp;
		}
	}
#endregion
}
