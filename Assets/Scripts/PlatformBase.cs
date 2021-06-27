/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;
using System.Linq;

public abstract class PlatformBase : MonoBehaviour
{
	#region Fields
	// Public Fields
	public int platformIndex;
	public PlatformSet platformSet;

	// Private Fields
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
	private void OnEnable()
	{
		platformSet.AddDictionary( platformIndex, this );
		platformSet.AddDictionary( gameObject.GetInstanceID(), this );
	}

	private void OnDisable()
	{
		platformSet.RemoveDictionary( platformIndex );
		platformSet.RemoveDictionary( gameObject.GetInstanceID() );
	}
    protected virtual void Awake()
    {
        bounds = GetComponent< MeshRenderer >().bounds;
        
		Random.InitState( System.DateTime.Now.Millisecond * gameObject.GetInstanceID() );

		AssignResetSlots();
	}
#endregion

#region API
	public abstract Vector2 GetRandomPosition_InsidePlatform();
	public abstract Vector2 GetRandomPosition_OutsidePlatform();




	public Vector2 GetResetSlot( int ID )
	{
		return resetSlots[ resetSlotIndicesByID[ ID ] ];
	}
#endregion

#region Implementation
    protected abstract void AssignResetSlots();

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

    [ Button() ]
	public void AddTestPoint_Inside()
    {
		testPoints_Inside.Add( GetRandomPosition_InsidePlatform() );
	}
    
    [ Button() ]
	public void AddTestPoint_Outside()
    {
		testPoints_Outside.Add( GetRandomPosition_OutsidePlatform() );
	}
#endif
#endregion
}
