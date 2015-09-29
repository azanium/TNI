using UnityEngine;
using System.Collections;

public class EditPillarBoxController : UIViewController 
{
	#region MemVars & Props

	public UIGrid pillarBoxGrid;

	#endregion


	#region Events

	public void OnSubmitPillarBox()
	{
		GameObject selectedPillarBox;
		UIHelper.GetSpritesFromGrid(pillarBoxGrid, out selectedPillarBox);

		if (selectedPillarBox == null)
		{
			return;
		}

		var sprite = selectedPillarBox.GetComponentInChildren<UISlicedSprite>();
		if (sprite != null)
		{   
			var colorCodes = sprite.spriteName.Split('_');
			if (colorCodes.Length > 1)
			{
				var color = colorCodes[1];
				var avatarButton = ProfileController.GetEditPlayerAvatarButton();
				if (avatarButton != null)
				{
					var editAvatarSprite = UIHelper.GetButtonSprite(avatarButton);
					var spriteName = string.Format("{0}{1}", GameConstants.PLAYER_BOX_PREFIX, color);
					editAvatarSprite.spriteName = spriteName;
					Debug.Log("Changing Avatar Sprite: " + spriteName);
				}
				else
				{
					Debug.Log("Avatar Button is null");
				}
			}
		}
	}

	#endregion
}
