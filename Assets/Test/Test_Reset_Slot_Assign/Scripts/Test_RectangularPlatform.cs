/* Created by and for usage of FF Studios (2021). */

using System.Linq;
using UnityEngine;
using FFStudio;

public class Test_RectangularPlatform : PlatformBase
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

		Vector2 leftmostPos = bounds.min + new Vector3( 0, bounds.extents.y );
		var delta = new Vector2( bounds.size.x / ( actorCount + 1 ), 0 );

		resetSlots = new Vector2[ actorCount ];
		resetSlotIndicesByID = new int[ actorCount ];
		for( var i = 0; i < resetSlots.Length; i++ )
		{
			resetSlots[ i ] = leftmostPos + ( i + 1 ) * delta;
			resetSlotIndicesByID[ i ] = i;
		}

		/* Shuffle indices array. */
		var rand = new System.Random();
		resetSlotIndicesByID = resetSlotIndicesByID.OrderBy( item => rand.Next() ).ToArray();
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
