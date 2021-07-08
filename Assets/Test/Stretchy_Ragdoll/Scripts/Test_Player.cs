/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using FFStudio;

public class Test_Player : MonoBehaviour
{
	#region Fields
	public Vector3 forceDirection;
	public float forceMagnitude;
	public Rigidbody parentRigidbody;

	public Rigidbody[] rotationLimbs;

	[Foldout("Hand")] public Rigidbody hand_left;
	[Foldout("Hand")] public Rigidbody hand_right;

	[Foldout("Hand")] public Transform hand_optimal_left;  // left hand optimal position
	[Foldout("Hand")] public Transform hand_optimal_right; // right hand optimal position

	[Foldout("Hand")] public FixedJoint hand_target_left;
	[Foldout("Hand")] public FixedJoint hand_target_right;

	[Foldout( "Arm" )] public Transform[] arm_left_limbs;
	[Foldout( "Arm" )] public Transform[] arm_right_limbs;
	[Foldout( "Arm" )] public Rigidbody[] arm_left_rbs;
	[Foldout( "Arm" )] public Rigidbody[] arm_right_rbs;

	[ SerializeField ] private TransformData[] arm_holdPositions_Left;
	[ SerializeField ] private TransformData[] arm_holdPositions_Right;

	private Rigidbody[] limbs;
	private TransformData[] rotatingLimbsData;

	private Vector3 rotationOrigin;
	private Vector3 offset;
	private UnityMessage applyHandPosition;
#endregion

#region Unity API
	private void Awake()
	{
		limbs = parentRigidbody.GetComponentsInChildren< Rigidbody >();

		rotatingLimbsData = new TransformData[ rotationLimbs.Length ];

		for( var i = 0; i < rotationLimbs.Length; i++ )
		{
			rotatingLimbsData[ i ] = rotationLimbs[ i ].transform.GetLocalTransformData();
		}

		applyHandPosition = ApplyLeftArmPosition;
	}
#endregion

#region API
#endregion

#region Implementation
	[ Button() ]
	private void StopRagdoll()
	{
		parentRigidbody.velocity 		= Vector3.zero;
		parentRigidbody.angularVelocity = Vector3.zero;

		parentRigidbody.isKinematic = true;
		parentRigidbody.useGravity  = true;
	}

	[ Button() ]
	private void AttachLeftHand()
	{
		ZeroVelocityRagdoll();
		hand_target_left.transform.position = hand_optimal_left.position;
		hand_left.transform.position 		= hand_optimal_left.position;
		hand_left.transform.forward 		= Vector3.right; // rotate hand to face foward
		hand_target_left.connectedBody 		= hand_left;

		rotationOrigin = hand_optimal_left.position;
		applyHandPosition = ApplyLeftArmPosition;

		offset = hand_target_left.transform.position - new Vector3( -1.1f, 4f, 0 );
	}

	[ Button() ]
	private void AttachRightHand()
	{
		ZeroVelocityRagdoll();
		hand_target_right.transform.position = hand_optimal_right.position;
		hand_right.transform.position        = hand_optimal_right.position;
		hand_right.transform.forward 		 = Vector3.left; // rotate hand to face foward
		hand_target_right.connectedBody      = hand_right;

		rotationOrigin    = hand_optimal_right.position;
		applyHandPosition = ApplyRightArmPosition;

		// hand_target_right.transform.eulerAngles = new Vector3( 0, -90, 90 );

		offset = hand_target_right.transform.position - new Vector3( 1.1f, 4f, 0 );
	}

	private void ZeroVelocityRagdoll()
	{
		for( var i = 0; i < limbs.Length; i++ )
		{
			limbs[ i ].velocity 	   = Vector3.zero;
			limbs[ i ].angularVelocity = Vector3.zero;

			limbs[ i ].isKinematic = false;
			limbs[ i ].useGravity  = true;
		}
	}

	[ Button() ]
	private void ReleaseHand()
	{
		hand_target_left.connectedBody 	= null;
		hand_target_right.connectedBody = null;

		hand_target_left.transform.eulerAngles  = Vector3.zero;
		hand_target_right.transform.eulerAngles = Vector3.zero;
	}

	[ Button() ]
	private void GiveForce()
	{
		ReleaseInput();
		ZeroVelocityRagdoll();
		ReleaseHand();
		parentRigidbody.AddForce( parentRigidbody.transform.up * forceMagnitude );
	}

	[ Button() ]
	private void StartRotate()
	{
		applyHandPosition();

		parentRigidbody.isKinematic = true;
		parentRigidbody.useGravity  = false;

		ChangeKinematicRigidbody( rotationLimbs, true );

		for( var i = 0; i < rotationLimbs.Length; i++ )
		{
			rotationLimbs[ i ].transform.SetLocalTransformData( rotatingLimbsData[ i ] );
		}

		parentRigidbody.transform.localEulerAngles = Vector3.zero;
		parentRigidbody.transform.position = offset;
	}

	[ Button() ]
	private void Rotate()
	{
		parentRigidbody.transform.RotateAround( rotationOrigin, Vector3.forward, 5 );
	}

	[ Button() ]
	private void ReleaseInput()
	{
		parentRigidbody.isKinematic = false;
		parentRigidbody.useGravity  = true;

		ChangeKinematicRigidbody( rotationLimbs, false );
	}

	private void ChangeKinematicRigidbody( Rigidbody[] limbs, bool isKinematic )
	{
		for( var i = 0; i < limbs.Length; i++ )
		{
			limbs[ i ].isKinematic = isKinematic;
			limbs[ i ].useGravity  = !isKinematic;
		}
	}

	[ Button() ]
	private void SeriliazeArmPosition()
	{
		arm_holdPositions_Left  = new TransformData[ arm_left_limbs.Length ];
		arm_holdPositions_Right = new TransformData[ arm_right_limbs.Length ];

		for( var i = 0; i < arm_left_limbs.Length; i++ )
		{
			arm_holdPositions_Left[ i ]  = arm_left_limbs[ i ].GetLocalTransformData();
			arm_holdPositions_Right[ i ] = arm_right_limbs[ i ].GetLocalTransformData();
		}
	}

	[ Button() ]
	private void ApplyLeftArmPosition()
	{
		for( var i = 0; i < arm_left_limbs.Length; i++ )
		{
			arm_left_limbs[ i ].SetLocalTransformData( arm_holdPositions_Left[ i ] );
		}

		ChangeKinematicRigidbody( arm_left_rbs, true );
	}

	[ Button() ]
	private void ApplyRightArmPosition()
	{
		for( var i = 0; i < arm_right_limbs.Length; i++ )
		{
			arm_right_limbs[ i ].SetLocalTransformData( arm_holdPositions_Right[ i ] );
		}

		ChangeKinematicRigidbody( arm_right_rbs, true );
	}

	[ Button() ]
	private void LogDiff()
	{
		FFLogger.Log( "Diff: " + ( hand_target_left.transform.position - parentRigidbody.transform.position ) );
	}
#endregion
}
