/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Test_Player : MonoBehaviour
{
#region Fields
	public Rigidbody parentRigidbody;

	public Rigidbody hand_left;
	public Rigidbody hand_right;

	public Transform hand_optimal_left;  // left hand optimal position
	public Transform hand_optimal_right; // right hand optimal position

	public FixedJoint hand_target_left;
	public FixedJoint hand_target_right;
#endregion

#region Unity API
	private void Awake()
	{
	}
#endregion

#region API
#endregion

#region Implementation
	[ Button() ]
	public void ZeroVelocity()
	{
		parentRigidbody.velocity = Vector3.zero;
	}
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
		hand_target_left.transform.position = hand_optimal_left.position;
		hand_left.transform.position 		= hand_optimal_left.position;
		hand_target_left.connectedBody 		= hand_left;
	}

	[ Button() ]
	private void AttachRightHand()
	{
		hand_target_right.transform.position = hand_optimal_right.position;
		hand_right.transform.position        = hand_optimal_right.position;
		hand_target_right.connectedBody      = hand_right;
	}

	[ Button() ]
	private void ReleaseHand()
	{
		hand_target_left.connectedBody 	= null;
		hand_target_right.connectedBody = null;
	}
#endregion
}
