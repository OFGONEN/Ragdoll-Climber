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

	private Rigidbody[] limbs;
	private TransformData[] rotatingLimbsData;

	private Vector3 rotationOrigin;
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
		hand_target_left.connectedBody 		= hand_left;

		rotationOrigin = hand_optimal_left.position;
	}

	[ Button() ]
	private void AttachRightHand()
	{
		ZeroVelocityRagdoll();
		hand_target_right.transform.position = hand_optimal_right.position;
		hand_right.transform.position        = hand_optimal_right.position;
		hand_target_right.connectedBody      = hand_right;


		rotationOrigin = hand_optimal_right.position;
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
	}

	[ Button() ]
	private void GiveForce()
	{
		ReleaseInput();
		ZeroVelocityRagdoll();
		ReleaseHand();
		parentRigidbody.AddForce( forceDirection * forceMagnitude );
	}

	[ Button() ]
	private void StartRotate()
	{
		parentRigidbody.isKinematic = true;
		parentRigidbody.useGravity  = false;

		ChangeKinematicRigidbody( rotationLimbs, true );

		for( var i = 0; i < rotationLimbs.Length; i++ )
		{
			rotationLimbs[ i ].transform.SetLocalTransformData( rotatingLimbsData[ i ] );
		}
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
#endregion
}
