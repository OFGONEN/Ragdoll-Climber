/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using TMPro;

public class UIProgressBar : UILoadingBar
{
#region Fields
	[ Header( "Event Listener" ) ]
	public EventListenerDelegateResponse levelLoadedListener;

	// Public Fields
	[ Header( "Text Renderers" ) ]
	public TextMeshProUGUI currentLevelText;
	public TextMeshProUGUI nextLevelText;

#endregion

#region Unity API
	protected override void OnEnable()
	{
		base.OnEnable();
		levelLoadedListener.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		levelLoadedListener.OnDisable();
	}

	protected override void Awake()
	{
		base.Awake();

		levelLoadedListener.response = LevelLoadedResponse;
	}
#endregion

#region API
#endregion

#region Implementation
	private void LevelLoadedResponse()
	{
		currentLevelText.text = CurrentLevelData.Instance.currentConsecutiveLevel.ToString();
		nextLevelText.text    = ( CurrentLevelData.Instance.currentConsecutiveLevel + 1 ).ToString();
	}
#endregion
}
