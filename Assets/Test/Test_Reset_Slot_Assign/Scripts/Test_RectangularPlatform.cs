/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

public class Test_RectangularPlatform : PlatformBase
{
#region Fields
	private static Vector2[] zoneIndexPairs = new[] { new Vector2( 0, 0 ), new Vector2( 1, 0 ), new Vector2( 2, 0 ),
													  new Vector2( 0, 1 ),                      new Vector2( 2, 1 ),
													  new Vector2( 0, 2 ), new Vector2( 1, 2 ), new Vector2( 2, 2 ) };
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region PlatformBase Overrides
	public override Vector2 GetRandomPositionInsidePlatform()
	{
		var margin = GameSettings.Instance.randomPointInside_MarginPercentage / 100.0f * bounds.size;

		/* Since the bounds of the platform and the actual shape of the platform are identical, we can directly use bounds. */
        return new Vector2( Random.Range( bounds.min.x + margin.x, bounds.max.x - margin.x ),
                            Random.Range( bounds.min.y + margin.y, bounds.max.y - margin.y ) );
	}

	public override Vector2 GetRandomPositionOutsidePlatform()
	{
		/*
         *        0      1      2 
         *      __________________
         *  0   | 1 |    2    | 3 |         
         *      |___|_________|___|
         *      |   |         |   |
         *  1   | 4 |  Inside | 5 |
         *      |___|_________|___|
         *      |   |         |   |
         *  2   |_6_|____7____|_8_|
         *
         */

		Vector2 min       = bounds.min, max = bounds.max;
		float   maxOffset = GameSettings.Instance.randomPointOutside_MaxOffsetPercentage / 100.0f * Mathf.Min( bounds.size.x, bounds.size.y );

		var zoneIndex = zoneIndexPairs[ Random.Range( 0, 8 ) ];

		float x, y;

		if( zoneIndex.x == 0 )
			x = Random.Range( min.x - 2 * maxOffset, min.x - maxOffset );
        else if( zoneIndex.x == 1 )
            x = Random.Range( min.x, max.x );
		else /* if( zoneIndex.x == 2 ) */
            x = Random.Range( max.x + maxOffset, max.x + 2 * maxOffset );

		if( zoneIndex.y == 2 )
			y = Random.Range( min.y - 2 * maxOffset, min.y - maxOffset );
		else if( zoneIndex.y == 1 )
			y = Random.Range( min.y, max.y );
		else /* if( zoneIndex.y == 0 ) */
			y = Random.Range( max.y + maxOffset, max.y + 2 * maxOffset );

		return new Vector2( x, y );
	}
    
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

		ShuffleResetSlots();
	}
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
		/*
         *        0      1      2 
         *      __________________
         *  0   | 1 |    2    | 3 |         
         *      |___|_________|___|
         *      |   |         |   |
         *  1   | 4 |  Inside | 5 |
         *      |___|_________|___|
         *      |   |         |   |
         *  2   |_6_|____7____|_8_|
         *
         */

		Vector2 min = bounds.min, max = bounds.max;
		float   maxOffset = GameSettings.Instance.randomPointOutside_MaxOffsetPercentage / 100.0f * Mathf.Min( bounds.size.x, bounds.size.y );

		var maxWidth    = bounds.size.x + 2 * ( 2 * maxOffset );
		var maxHeight   = bounds.size.y + 2 * ( 2 * maxOffset );
		var smallWidth  = maxOffset; // For example the width of zones 1, 3, 4, 5, 6 and 8.
		var smallHeight = smallWidth;

		Gizmos.color = Color.red;
        
		/* Draw rectangle 1 + 2 + 3 */
		Gizmos.DrawWireCube( new Vector3( bounds.center.x, max.y + 1.5f * maxOffset ), new Vector2( maxWidth, smallHeight ) );

		/* Draw rectangle 1 + 4 + 6 */
		Gizmos.DrawWireCube( new Vector3( min.x - 1.5f * maxOffset, bounds.center.y ), new Vector2( smallWidth, maxHeight ) );

		/* Draw rectangle 3 + 5 + 8 */
		Gizmos.DrawWireCube( new Vector3( max.x + 1.5f * maxOffset, bounds.center.y ), new Vector2( smallWidth, maxHeight ) );

		/* Draw rectangle 6 + 7 + 8 */
		Gizmos.DrawWireCube( new Vector3( bounds.center.x, min.y - 1.5f * maxOffset ), new Vector2( maxWidth, smallHeight ) );

		Gizmos.color = Color.green;

		/* Draw inner rectangle */
		var margin = GameSettings.Instance.randomPointInside_MarginPercentage / 100.0f * bounds.size;
		Gizmos.DrawWireCube( bounds.center, bounds.size - margin );
	}
#endif
#endregion
}
