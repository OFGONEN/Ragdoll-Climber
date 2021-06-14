/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;

public class Test_BlendShapes : MonoBehaviour
{
#region Fields
	public EventListenerDelegateResponse startBlendShapeListener;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	private float blendShapeWeight;
#endregion

#region Unity API
	private void OnEnable()
	{
		startBlendShapeListener.OnEnable();
	}

	private void OnDisable()
	{
		startBlendShapeListener.OnDisable();
	}

	private void Awake()
	{
		startBlendShapeListener.response = StartBlendShaping;

		skinnedMeshRenderer = GetComponent< SkinnedMeshRenderer >();
	}
#endregion

#region API
#endregion

#region Implementation
	private void StartBlendShaping()
	{
		var changeEvet = startBlendShapeListener.gameEvent as IntGameEvent;

		blendShapeWeight = 0;

		DOTween.To( () => blendShapeWeight, x => blendShapeWeight = x, 100, changeEvet.eventValue ).OnUpdate( SetBlandeShape );
	}

	void SetBlandeShape()
	{
		skinnedMeshRenderer.SetBlendShapeWeight( 0, blendShapeWeight );
	}
#endregion
}
