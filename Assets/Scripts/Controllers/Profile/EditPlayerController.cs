using UnityEngine;
using System.Collections;

public class EditPlayerController : UIViewController
{
	#region Events
	
	protected void OnSubmitAvatar()
	{
		var avatarButton = ProfileController.GetEditPlayerAvatarButton();
		if (avatarButton == null)
		{
			Debug.LogWarning("Avatar Button is not set!");
			return;
		}
		
		UISlicedSprite avatarBg = UIHelper.GetButtonSprite(avatarButton);
		
		int selectedIndex = ProfileController.GetSelectedButtonIndex();
		var selectedPlayer = ProfileController.GetSelectionButton(selectedIndex);
		
		var selectedPlayerAvatar = UIHelper.GetButtonSprite(selectedPlayer);
		selectedPlayerAvatar.spriteName = avatarBg.spriteName;
		
		PlayerProfile.current.profiles[selectedIndex].Avatar = avatarBg.spriteName;
		PlayerProfile.Save();
	}

	#endregion
}
