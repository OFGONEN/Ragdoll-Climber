/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using TMPro;
using NaughtyAttributes;

public class UIWorldSpace : MonoBehaviour
{
#region Fields
	[Header( "Shared Variables" )]
	public SharedReferenceProperty mainCameraReference;

	[HorizontalLine]

	[Header( "UI Elements" )]
	public TextMeshProUGUI entityName;

	// Hidden Fields
	[ HideInInspector ] public Transform followTarget;

	// Private Fields
	private Transform mainCamera;
	private UnityMessage update;
#endregion

#region Unity API

	private void OnEnable()
	{
		mainCameraReference.changeEvent += OnMainCameraChange;
	}

	private void OnDisable()
	{
		mainCameraReference.changeEvent -= OnMainCameraChange;
	}

	private void Awake()
	{
		update = ExtensionMethods.EmptyMethod;
	}
	private void Update()
	{
		update();
	}

#endregion

#region API
#endregion

#region Implementation
	void FollowTarget()
	{
		// Follow Ragdoll Target
		var targetPosition = followTarget.position;
		var targetOffset   = GameSettings.Instance.worldUI_AgentName_Offset;

		targetPosition.y += targetOffset.y;
		targetPosition.z  = targetOffset.z;

		transform.position = targetPosition;

		// Look at Camera
		var lookDirection = transform.position - mainCamera.position;
		Vector3 newDirection = Vector3.RotateTowards( transform.forward, lookDirection, 6.3f, 0 ); // One complete circle is 6.28 radian. 
		var eulerLookRotation = Quaternion.LookRotation( newDirection ).eulerAngles;

		transform.eulerAngles = eulerLookRotation;
	}
	void OnMainCameraChange()
	{
		if(mainCameraReference.sharedValue == null)
		{
			mainCamera = null;
			update     = ExtensionMethods.EmptyMethod;

		}
		else 
		{
			mainCamera = mainCameraReference.sharedValue as Transform;
			update     = FollowTarget;
		}
	}
#endregion
}
