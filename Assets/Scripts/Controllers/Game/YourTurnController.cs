using UnityEngine;
using System.Collections;

public class YourTurnController : UIViewController
{
	#region MemVars & Props

	public UIButton turnButton;

	#endregion

	#region UIViewController's

	#endregion

	#region Public Methods


	public void ChangeText(string text)
	{
		var label = UIHelper.GetLabel(turnButton.gameObject	);
		label.text = text;

	}

	#endregion
}
