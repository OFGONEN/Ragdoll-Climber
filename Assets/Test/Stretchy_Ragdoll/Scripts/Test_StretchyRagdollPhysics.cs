/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Test_StretchyRagdollPhysics : MonoBehaviour
{
#region Fields
    [ Header( "Event Listeners" ) ]
    public EventListenerDelegateResponse screenTapEventListener;
	public SharedVector3 sharedInputDirection;

    [ HorizontalLine( 2, EColor.Yellow ) ]
	public float forceMultiplier;

    [ HorizontalLine( 2, EColor.Yellow ) ]
    [ SerializeField ]
    private Rigidbody[] ragdollRigidbodies;

	private Vector3 Direction => -sharedInputDirection.sharedValue;

	private bool dragInitiated = false;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		screenTapEventListener.OnEnable();
	}
    
    private void OnDisable()
    {
		screenTapEventListener.OnDisable();
	}
    
    private void Awake()
    {
		screenTapEventListener.response = ScreenTapResponse;
	}
    
    private void Start()
    {
		DeactivateRagdoll();
	}
#endregion

#region API
#endregion

#region Implementation
    private void ActivateRagdoll()
    {
		foreach( var jointRB in ragdollRigidbodies )
			jointRB.isKinematic = false;
            
        FFLogger.Log( "Activating ragdoll." );
    }
    
    private void DeactivateRagdoll()
    {
		foreach( var jointRB in ragdollRigidbodies )
			jointRB.isKinematic = true;
            
        FFLogger.Log( "Deactivating ragdoll" );
    }

    private void ApplyForceToJoints( Vector3 force )
    {
        foreach( var jointRB in ragdollRigidbodies )
			jointRB.AddForce( force );
            
        FFLogger.Log( "Applying force = " + force + " Newtons." );
    }
    
    private void ScreenTapResponse()
    {
        bool fingerDown = ( screenTapEventListener.gameEvent as BoolGameEvent ).eventValue == true;
        FFLogger.Log( "Screen Tap Response: " + fingerDown + "." );
        
		if( !dragInitiated && fingerDown )
        {
			dragInitiated = true;
			//DeactivateRagdoll();
		}
        else if( dragInitiated && !fingerDown )
        {
			dragInitiated = false;
			ActivateRagdoll();
			ApplyForceToJoints( Direction * forceMultiplier );
		}
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if( dragInitiated )
			foreach( var jointRB in ragdollRigidbodies )
    		    Gizmos.DrawRay( jointRB.transform.position, -Direction );
	}
#endif
#endregion
}
