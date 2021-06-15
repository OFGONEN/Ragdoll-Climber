/* Created by and for usage of FF Studios (2021). */

using UnityEditor;
using UnityEngine;

public abstract class PlatformBase : MonoBehaviour
{
#region Fields
    public Bounds bounds;
    public Vector2[] resetSlots;
    public int[] resetSlotIndicesByID;
#endregion

#region Properties
#endregion

#region Unity API
    protected virtual void Start()
    {
        bounds = GetComponent< MeshRenderer >().bounds;

		AssignResetSlots();
	}
#endregion

#region API
#endregion

#region Implementation
    protected abstract void AssignResetSlots();

	protected Vector2 GetResetSlot( int ID )
	{
		return resetSlots[ resetSlotIndicesByID[ ID ] ];
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Handles.color = Color.yellow;
		var textOffset = new Vector2( -0.1f, 0.1f );

		for( var id = 0; id < resetSlots.Length; id++ )
		{
			Handles.Label( resetSlots[ resetSlotIndicesByID[ id ] ] + textOffset, "Reset Slot\nActor ID: " + id, style );
			Handles.DrawWireDisc( resetSlots[ resetSlotIndicesByID[ id ] ], Vector3.back, 0.05f );
		}
	}
#endif
#endregion
}
