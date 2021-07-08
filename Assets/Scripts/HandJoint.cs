/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class HandJoint : MonoBehaviour
{
#region Fields

// Private Fields
	public float rotationDiff;
	// Target platform
	Transform targetTransform;
	Vector3 targetPosition;
	Vector3 targetUp;

	Rigidbody jointRigidbody;
	FixedJoint fixedJoint;
	UnityMessage update;


#endregion

#region Unity API
	private void Awake()
	{
		jointRigidbody = GetComponent< Rigidbody >();
		fixedJoint     = GetComponent< FixedJoint >();

		update = ExtensionMethods.EmptyMethod;
	}

	private void Update()
	{
		update();
	}

#endregion

#region API
	public void Attach(Vector3 position, GameObject target, Rigidbody hand)
	{
		transform.position       = position; // Set joint position
		targetTransform          = target.transform.parent; // Set target platform transform
		fixedJoint.connectedBody = hand; // Attach the hand to joint

		targetPosition = targetTransform.position;
		targetUp       = targetTransform.up;
		update         = TrackTarget; // Start tracking target platform's transform
	}

	public void Release()
	{
		fixedJoint.connectedBody = null; // Null the connected body of the joint to release any attached rigidbody
		update = ExtensionMethods.EmptyMethod;
	}
#endregion

#region Implementation
	void TrackTarget()
	{
		var position = targetTransform.position;
		var up = targetTransform.up;

		var position_diff = position - targetPosition;
		var rotation_diff = Vector3.SignedAngle( targetUp, up, Vector3.forward );

		rotationDiff = rotation_diff;

		transform.position += position_diff;
		transform.RotateAround( targetTransform.position, Vector3.forward, rotationDiff );
		transform.eulerAngles = Vector3.zero;


		targetPosition = targetTransform.position;
		targetUp       = targetTransform.up;
	}
	
	[ Button() ]
	public void RotateAround()
	{
		transform.RotateAround( targetTransform.position, Vector3.forward, -0.1f );
	}
#endregion
}
