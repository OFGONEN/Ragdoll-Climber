using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using DG.Tweening;
using NaughtyAttributes;

namespace FFStudio
{
    public class CameraController : MonoBehaviour
    {
#region Fields
		[Header( "Shared Variables" )]
		public SharedReferenceProperty followZoneProperty;
		public SharedFloatProperty playerStretchRatio;
		public SharedBool isPlayerSoaring;

		/* Private Fields */
		private Transform followTarget_Transform;
		private UnityMessage update;
#endregion

#region Unity API
		private void Awake()
		{
			update = ExtensionMethods.EmptyMethod;
		}

		private void OnEnable()
		{
			followZoneProperty.changeEvent += OnTargetRigidbodyChange;
		}

		private void OnDisable()
		{
			followZoneProperty.changeEvent -= OnTargetRigidbodyChange;
		}

		private void Update()
		{
			update();
		}


#endregion

#region API
#endregion
#region Implementation

		private void CameraFollowPlayer()
		{
			var position       = transform.position; // Current Position
			var followPosition = followTarget_Transform.position; // Target Position

			// Lerp depth distance using player's stretch raito
			var depthDistance = -Mathf.Lerp( GameSettings.Instance.camera_Depth_FollowDistance.x, // Min Value
				GameSettings.Instance.camera_Depth_FollowDistance.y, // Max Value
				playerStretchRatio.sharedValue /* Lerp Ratio */ );

			float depthFollowCofactor = 1;

			if( isPlayerSoaring.sharedValue )
				depthFollowCofactor = 0;

			followPosition.z   = Mathf.Lerp( position.z, depthDistance, Time.deltaTime * GameSettings.Instance.camera_Depth_FollowSpeed * depthFollowCofactor ); // Target Z position
			// var newPosition        = Vector3.Lerp( position, followPosition, Time.deltaTime * GameSettings.Instance.camera_FollowSpeed ); // New position obtained by lerping
			// transform.position = newPosition; // Set new position 
			transform.position = followPosition;
		}

		void OnTargetRigidbodyChange()
		{
			if( followZoneProperty.sharedValue == null )
			{
				update = ExtensionMethods.EmptyMethod;
				followTarget_Transform = null;
			}
			else
			{
				update = CameraFollowPlayer;
				followTarget_Transform = followZoneProperty.sharedValue as Transform;
			}
		}

#endregion

#region EditorOnly
#if UNITY_EDITOR

#endif
#endregion
	}
}