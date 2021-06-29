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
		public SharedReferenceProperty followTarget_Rigidbody;
		public SharedFloatProperty playerStretchRatio;

		/* Private Fields */
		private Transform followTarget_Transform;
		private Vector3 originalDirection;

		private UnityMessage update;
#endregion

#region Unity API
		private void Awake()
		{
			update = ExtensionMethods.EmptyMethod;
		}
		private void Start()
		{
			originalDirection = transform.forward;
		} 

		private void OnEnable()
		{
			followTarget_Rigidbody.changeEvent += OnTargetRigidbodyChange;
		}

		private void OnDisable()
		{
			followTarget_Rigidbody.changeEvent -= OnTargetRigidbodyChange;
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
			var position       = transform.position;
			var followPosition = followTarget_Transform.position;

			transform.LookAtAxis( followPosition, GameSettings.Instance.camera_LookAtAxis );

			var distance = -Mathf.Lerp( GameSettings.Instance.camera_FollowDistance.x, // Min Value
				GameSettings.Instance.camera_FollowDistance.y, // Max Value
				playerStretchRatio.sharedValue /* Lerp Ratio */ );

			followPosition.x = 0;
			followPosition.y = Mathf.Lerp( position.y, followPosition.y, Time.deltaTime * GameSettings.Instance.camera_VerticalFollowSpeed );
			followPosition.z = Mathf.Lerp( position.z, distance, Time.deltaTime * GameSettings.Instance.camera_DepthFollowSpeed );

			transform.position = followPosition;

			ClampHorizontally();
		}
		void OnTargetRigidbodyChange()
		{
			if( followTarget_Rigidbody.sharedValue == null )
			{
				update = ExtensionMethods.EmptyMethod;
				followTarget_Transform = null;
			}
			else
			{
				update = CameraFollowPlayer;
				followTarget_Transform = ( followTarget_Rigidbody.sharedValue as Rigidbody ).transform;
			}
		}

		void ClampHorizontally()
		{
			/* Find delta angle with the original direction. */
			var deltaAngle = Vector3.SignedAngle( originalDirection, transform.forward.SetY( 0 ), Vector3.up );

			/* Clamp & set that as the new Y angle. */
			var newY = Mathf.Clamp( deltaAngle,
									GameSettings.Instance.camera_horizontalLookClamp.x, GameSettings.Instance.camera_horizontalLookClamp.y );
			transform.eulerAngles = transform.eulerAngles.SetY( newY );
		}
#endregion

#region EditorOnly
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Handles.color = new Color( 0.5f, 0.5f, 0.5f, 0.5f );

			var arcTotalAngle = Mathf.Abs( GameSettings.Instance.camera_horizontalLookClamp.x ) +
								Mathf.Abs( GameSettings.Instance.camera_horizontalLookClamp.y );
			var leftLimit = Quaternion.AngleAxis( -GameSettings.Instance.camera_horizontalLookClamp.x, Vector3.up ) * originalDirection;
			var rightLimit = Quaternion.AngleAxis( +GameSettings.Instance.camera_horizontalLookClamp.x, Vector3.up ) * originalDirection;

			Handles.DrawSolidArc( transform.position, Vector3.up, rightLimit, arcTotalAngle, 2.0f );
		}
#endif
#endregion
	}
}