/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor;

public class Agent : Actor
{
	#region Fields

	// Private Fields
	PlatformBase nextPlatform;
	Vector2 targetPoint;

	UnityMessage update;

	float rotationNormal;
	float rotationDirection;
	float stretchNormal;

#endregion

#region Unity API
	protected override void Awake()
	{
		base.Awake();
		update = ExtensionMethods.EmptyMethod;
	}

	private void Update()
	{
		update();
	}
#endregion

#region API
#endregion

#region Implementation
	private bool FindNextPlatform()
	{
		platformSet.itemDictionary.TryGetValue( currentWayPoint + 1, out nextPlatform );
		return nextPlatform != null;
	}
	
	private void LaunchNextPlatformSequence()
	{
		StraightenUpRagdoll();

		targetPoint = nextPlatform.GetRandomPositionInsidePlatform();

		var direction   = ( targetPoint - ActorPosition.CastV2() ).normalized;
		var rotateAmount = Vector2.SignedAngle( parentRigidbody.transform.up, direction );

		rotationDirection = Mathf.Sign( rotateAmount );
		rotationNormal = 0;
		stretchNormal  = 0;

		FFLogger.Log( "Agent :" + GameSettings.Instance.agent_rotationSpeed );
		float rotationDuration = Mathf.Abs( rotateAmount / GameSettings.Instance.agent_rotationSpeed );

		var rotateTween  = DOTween.To( () => rotationNormal, x => rotationNormal = x, 1, rotationDuration ); 
		rotateTween.OnUpdate( RotateActor );

		var stretchTween = DOTween.To( () => stretchNormal, x => stretchNormal = x, 1, GameSettings.Instance.agent_stretchDutation ); 
		stretchTween.OnUpdate( StretchActor );
		// stretchTween.SetDelay(  )

		var sequence = DOTween.Sequence();
		sequence.Append( rotateTween );
		sequence.Append( stretchTween );
		sequence.OnComplete( ReadyToLaunch );
	}

	private void CheckDistanceToTargetPlatform()
	{
		var agentPosition          = ActorPosition.CastV2();
		var left_ShoulderDistance  = Vector2.Distance( LeftShouldPos, targetPoint );
		var right_ShoulderDistance = Vector2.Distance( RightShouldPos, targetPoint );

		var reachDistance = ArmReachDistance * 1.5f;

		if( left_ShoulderDistance <= reachDistance || right_ShoulderDistance <= reachDistance )
			TryToAttachHands();
	}

	private void RotateActor()
	{
		Rotate( Time.deltaTime * GameSettings.Instance.agent_rotationSpeed * rotationDirection );
	}

	private void StretchActor()
	{
		Stretch( stretchNormal );
	}

	protected override void Launch()
	{
		parentRigidbody.AddForce( ( targetPoint - ActorPosition.CastV2()).normalized * GameSettings.Instance.actor_launchForce );
		update = CheckDistanceToTargetPlatform;
	}

	protected override void OnHandsAttached()
	{
		update = ExtensionMethods.EmptyMethod;
	}

#endregion

#region EditorOnly
#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine( ActorPosition.CastV2(), targetPoint );

		Handles.Label( ActorPosition, "Distance: " + Vector2.Distance( ActorPosition.CastV2(), targetPoint ) );
	}

	[ Button() ]
	private void Test()
	{
		FindNextPlatform();
		LaunchNextPlatformSequence();
	}
#endif
#endregion

}
