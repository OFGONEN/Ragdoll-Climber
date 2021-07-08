/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderListener_ActorReseter : MonoBehaviour
{
#region Fields
#endregion

#region Unity API
	private void OnTriggerEnter( Collider other )
	{
		var actor = other.GetComponentInParent< Actor >();

		if( actor != null )
		{
			actor.ResetActor();
		}
	}
#endregion

#region API
#endregion

#region Implementation
#endregion
}
