/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;
using UnityEngine.UI;

public class UILoadingBar : UIEntity
{
    #region Fields
    [Header("Shared Variables")]
    public SharedFloatProperty progressProperty;

	[HorizontalLine]
	[Header( "UI Elements" )]
	public Image fillingImage;

	#endregion

	#region Unity API
    protected virtual void OnEnable()
    {
		progressProperty.changeEvent += OnValueChange;
	}

    protected virtual void OnDisable()
    {
		progressProperty.changeEvent -= OnValueChange;
    }

	protected virtual void Awake()
	{
		OnValueChange(); // Set filling amount to value at the start 
	}
	#endregion

	#region API
	#endregion

	#region Implementation
    private void OnValueChange()
    {
		fillingImage.fillAmount = progressProperty.sharedValue;
	}
	#endregion
}
