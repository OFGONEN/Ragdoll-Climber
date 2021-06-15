/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

public class Test_CircularPlatform : PlatformBase
{
#region Fields
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region PlatformBase Overrides
	protected override void AssignResetSlots()
	{
		var actorCount = GameSettings.actorCount;

		Random.InitState( System.DateTime.Now.Millisecond * gameObject.GetInstanceID() );

		/* Start from 0 degrees on the unit circle and go counter-clockwise from there. */
		var offsetFromCenter = bounds.extents.x * 2.0f / 3.0f;
		Vector2 startingPos = bounds.center;
		var deltaAngle = 2.0f * Mathf.PI / ( actorCount );
		var randomOffset = Random.Range( 0.1f, 1.0f ) * 2.0f * Mathf.PI;

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

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
