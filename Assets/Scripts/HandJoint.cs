/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class HandJoint : MonoBehaviour
{
#region Fields

	// Private Fields
	Transform targetTransform;
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
#endregion

#region API
	public void Attach(Vector3 position, GameObject target, Rigidbody hand)
	{
		targetTransform          = target.transform; // Set target platform transform
		transform.position       = position; // Set joint position
		fixedJoint.connectedBody = hand; // Attach the hand to joint

		update = TrackTarget; // Start tracking target platform's transform
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

	}
#endregion
}
