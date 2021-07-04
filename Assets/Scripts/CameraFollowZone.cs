/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FFStudio;

public class CameraFollowZone : MonoBehaviour
{
#region Fields
	public Transform target;

	private Vector3 target_CurrentPosition;

	private float Radius => GameSettings.Instance.camera_FollowZoneRadius;
	private UnityMessage update;

	#endregion

	#region Unity API

	private void Start()
	{
		target_CurrentPosition = target.position;
	}
	private void Update()
	{
		update();
	}

#endregion

#region API
	public void StartFollow(Vector3 position)
	{
		transform.position = position;
		update             = Follow;
	}

	public void StopFollow()
	{
		update = ExtensionMethods.EmptyMethod;
	}
#endregion

#region Implementation
	private void Follow()
	{
		var position = target.position;
		var distance = Vector3.Distance( transform.position, position );

		if( distance > Radius )
		{
			var diff = target.position - target_CurrentPosition;
			diff.z = 0;

			transform.position += diff;
		}

		target_CurrentPosition = target.position;
	}
#endregion

#region EditorOnly
#if UNITY_EDITOR 
	public Color zoneColor;
	private void OnDrawGizmos()
	{
		Handles.color = zoneColor;
		Handles.DrawSolidDisc( transform.position, Vector3.forward, Radius );
	}
#endif
#endregion
}

