/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using FFStudio;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTrajectory : MonoBehaviour
{
#region Fields
	[ Header( "Shared Variables" ) ]
	public SharedVector2Property inputDirectionProperty;
	public SharedFloatProperty stretchRatioProperty;
	public SharedBool playerOnAir;

	[ HorizontalLine ]
	public Transform target;
	public Image trajectoryImage;

#endregion

#region Unity API
	private void Update()
	{

		if( !playerOnAir.sharedValue & stretchRatioProperty.sharedValue > 0.1f )
		{
			trajectoryImage.enabled = true;
			transform.eulerAngles = target.eulerAngles;

			var direction = inputDirectionProperty.sharedValue.CastV3();
			var offset    = direction * GameSettings.Instance.worldUI_Trajectory_Offset;
			    offset.z  = GameSettings.Instance.worldUI_Trajectory_Depth;

			transform.position = target.position + offset;
		}
		else
			trajectoryImage.enabled = false;
	}
#endregion
}
