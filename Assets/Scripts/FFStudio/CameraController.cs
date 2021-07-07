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
		public SharedFloat cameraDepthRatio;
		public SharedFloat camera_CurrentDepthRatio;

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

		private void FixedUpdate()
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
				cameraDepthRatio.sharedValue /* Lerp Ratio */ );

			var newDepthDistance = Mathf.Lerp( position.z, depthDistance, Time.fixedDeltaTime * GameSettings.Instance.camera_Depth_FollowSpeed );  // Target Z position

			var diff = GameSettings.Instance.camera_Depth_FollowDistance.y - GameSettings.Instance.camera_Depth_FollowDistance.x;
			var min = Mathf.Abs( newDepthDistance ) - GameSettings.Instance.camera_Depth_FollowDistance.x;
			camera_CurrentDepthRatio.sharedValue = min / diff;

			position.z = 0;
			var newPosition   = Vector3.Lerp( position, followPosition, Time.fixedDeltaTime * GameSettings.Instance.camera_FollowSpeed );       // New position obtained by lerping
			newPosition.z = newDepthDistance;
			transform.position = newPosition; // Set new position 
			// transform.position = followPosition;
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