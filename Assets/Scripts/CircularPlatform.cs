/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using FFStudio;
using UnityEditor;
using UnityEngine;

public class CircularPlatform : PlatformBase
{
#region Fields
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
#endregion

#region PlatformBase Overrides
	public override Vector2 GetRandomPositionInsidePlatform()
	{
		Vector3 randomPointInsideCircle = Random.insideUnitCircle;
		randomPointInsideCircle.Scale( bounds.extents * GameSettings.Instance.RandomPointInside_Radius );
		return randomPointInsideCircle + bounds.center;
	}

	public override Vector2 GetRandomPositionOutsidePlatform()
	{
		Vector3 randomPointOutsideCircle = Random.insideUnitCircle.normalized;
		randomPointOutsideCircle.Scale( bounds.extents * GameSettings.Instance.RandomPointOutside_Radius );
		return randomPointOutsideCircle + bounds.center;
	}

	protected override void AssignResetSlots()
	{
		var actorCount = GameSettings.actorCount;

		/* Start from 0 degrees on the unit circle and go counter-clockwise from there. */
		var     offsetFromCenter = bounds.extents.x * 2.0f / 3.0f;
		Vector2 startingPos      = bounds.center;
		var     deltaAngle       = 2.0f * Mathf.PI / ( actorCount );
		var     randomOffset     = Random.Range( 0.1f, 1.0f ) * 2.0f * Mathf.PI;

		resetSlots = new Vector2[ actorCount ];
		resetSlotIndicesByID = new int[ actorCount ];
		for( var i = 0; i < resetSlots.Length; i++ )
		{
			resetSlots[ i ] = startingPos + offsetFromCenter * new Vector2( Mathf.Cos( randomOffset + i * deltaAngle ),
                                                                            Mathf.Sin( randomOffset + i * deltaAngle ) );
			resetSlotIndicesByID[ i ] = i;
		}

		ShuffleResetSlots();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
		Handles.color = Color.green;
		Handles.CircleHandleCap( 0, bounds.center, Quaternion.identity, 
                                bounds.extents.x * GameSettings.Instance.randomPointInside_betweenRadii.x / 100.0f, EventType.Repaint );
		Handles.CircleHandleCap( 0, bounds.center, Quaternion.identity,
                                 bounds.extents.x * GameSettings.Instance.randomPointInside_betweenRadii.y / 100.0f, EventType.Repaint );
		
        Handles.color = Color.red;
		Handles.CircleHandleCap( 0, bounds.center, Quaternion.identity,
                                 bounds.extents.x * GameSettings.Instance.randomPointOutside_betweenRadii.x / 100.0f, EventType.Repaint );
		Handles.CircleHandleCap( 0, bounds.center, Quaternion.identity,
                                 bounds.extents.x * GameSettings.Instance.randomPointOutside_betweenRadii.y / 100.0f, EventType.Repaint );
	}
#endif
#endregion
}
