/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;

public abstract class Test_PlatformBase : MonoBehaviour
{
#region Fields
    protected Bounds bounds;
    protected Vector2[] resetSlots;
    protected int[] resetSlotIndicesByID;
    
#if UNITY_EDITOR
    protected List< Vector2 > testPoints_Inside = new List< Vector2 >(), testPoints_Outside = new List< Vector2 >();
#endif
#endregion

#region Properties
#endregion

#region Unity API
    protected virtual void Start()
    {
        bounds = GetComponent< MeshRenderer >().bounds;
        
		Random.InitState( System.DateTime.Now.Millisecond * gameObject.GetInstanceID() );

		AssignResetSlots();
	}
#endregion

#region API
	public abstract Vector2 GetRandomPositionInsidePlatform();
	public abstract Vector2 GetRandomPositionOutsidePlatform();

#if UNITY_EDITOR
    [ Button() ]
	public void AddTestPoint_Inside()
    {
		testPoints_Inside.Add( GetRandomPositionInsidePlatform() );
	}
    
    [ Button() ]
	public void AddTestPoint_Outside()
    {
		testPoints_Outside.Add( GetRandomPositionOutsidePlatform() );
	}
#endif
#endregion

#region Implementation
    protected abstract void AssignResetSlots();

	protected Vector2 GetResetSlot( int ID )
	{
		return resetSlots[ resetSlotIndicesByID[ ID ] ];
	}
    
    protected void ShuffleResetSlots()
    {
		var rand = new System.Random( System.DateTime.Now.Millisecond * gameObject.GetInstanceID() );
		resetSlotIndicesByID = resetSlotIndicesByID.OrderBy( item => rand.Next() ).ToArray();
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if( !Application.isPlaying )
			return;

		GUIStyle style = new GUIStyle();
		style.normal.textColor = Handles.color = Color.yellow;
		var textOffset = new Vector2( -0.1f, 0.1f );

		for( var id = 0; id < resetSlots.Length; id++ )
		{
			Handles.Label( resetSlots[ resetSlotIndicesByID[ id ] ] + textOffset, "Reset Slot\nActor ID: " + id, style );
			Handles.DrawWireDisc( resetSlots[ resetSlotIndicesByID[ id ] ], Vector3.back, 0.05f );
		}

		if( testPoints_Inside != null && testPoints_Inside.Count > 0 )
		{
			Handles.color = Color.green;
			for( var i = 0; i < testPoints_Inside.Count; i++ )
				Handles.DrawWireDisc( testPoints_Inside[ i ], Vector3.back, 0.05f );
		}

		if( testPoints_Outside != null && testPoints_Outside.Count > 0 )
		{
			Handles.color = Color.red;
			for( var i = 0; i < testPoints_Outside.Count; i++ )
				Handles.DrawWireDisc( testPoints_Outside[ i ], Vector3.back, 0.05f );
		}
	}
#endif
#endregion
}
