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

	#endregion

	#region Unity API
	private void Start()
	{
		target_CurrentPosition = target.position;
	}
	private void Update()
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

#region API
#endregion

#region Implementation
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

