using UnityEngine;
using System.Collections;

public class EditNameController : UIViewController
{
	#region MemVars & Props

	public UIInput editNameInput;

	#endregion


	#region View Controllers

	public override void OnAppear ()
	{
		base.OnAppear ();

		if (editNameInput == null)
		{
			Debug.LogWarning("Edit Name Input is not set!");
			return;
		}
		
		var avatarButton = ProfileController.GetEditPlayerAvatarButton();
		if (avatarButton)
		{
			editNameInput.text = avatarButton.transform.Find("Label").GetComponent<UILabel>().text;
			editNameInput.selected = true;
		}
		else 
		{
			Debug.Log("Avatar Button is null");
		}
	}

	#endregion


	#region Events
	

	protected void OnSubmitName()
	{
		if (editNameInput == null)
		{
			Debug.LogWarning("Edit Name Input is not set!");
			return;
		}

		var avatarButton = ProfileController.GetEditPlayerAvatarButton();
		if (avatarButton != null) 
		{
			Debug.Log("Player Name: " + editNameInput.text);
			avatarButton.transform.Find("Label").GetComponent<UILabel>().text = editNameInput.text;
			
			UIButtonSelection sel = ProfileController.GetCurrentSelectionButton();
			UILabel selLabel = sel.transform.Find("Label").GetComponent<UILabel>();
			selLabel.text = editNameInput.text;

			int selectedIndex = ProfileController.GetSelectedButtonIndex();
			PlayerProfile.current.profiles[selectedIndex].Name = editNameInput.text;
			PlayerProfile.Save();
		}
		else 
		{
			Debug.Log("Avatar Button is null");
		}
	}

	#endregion
}
