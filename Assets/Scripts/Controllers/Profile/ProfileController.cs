using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProfileController : UIViewController
{
    #region MemVars & Props
    
    private static ProfileController profileController;

    public UIButtonSelection[] selectionButtons;
    public UIButton editPlayerAvatar;
    public GameObject profileSelectionPanel1;
    public GameObject profileSelectionPanel2;

    protected Dictionary<string, int> mapColorToNumber = new Dictionary<string, int>()
    {
        { "Purple", 1 },
        { "Green", 2 },
        { "Blue", 3 },
        { "Red", 4 },
        { "Yellow", 5 },
        { "Orange", 6 },
    };

	public Dictionary<int, string> mapProfileIndexToSpriteName = new Dictionary<int, string>()
	{
		{ 1, "Box_Purple" },
		{ 2, "Box_Green" },
		{ 3, "Box_Blue" },
		{ 4, "Box_Red" },
		{ 5, "Box_Yellow" },
		{ 6, "Box_Orange" }
	};

    protected int proceedRedirectorPanelIndex = 0;
	private bool dismissOnSelect = false;
	private string controllerRedirectOnSelect = "";

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {

        profileController = this;
    }

	public override void OnAppear()
	{
		base.OnAppear();

		string funcValue;
		if (TryGetControllerParamValue("f", out funcValue))
		{
			controllerRedirectOnSelect = funcValue;
			dismissOnSelect = false;
		}
		else
		{
			dismissOnSelect = true;
		}
	}

    protected void OnEnable()
    {
        PlayerProfile.Load();
        
        UIHelper.ClearSelectedButtonSelection(gameObject);
        
        for (int i = 0; i < PlayerProfile.current.profiles.Length; i++)
        {
            var profile = PlayerProfile.current.profiles[i];
            
            SetProfileButton(selectionButtons[i], profile.Name, profile.Avatar, profile.Piece);

            if (i == PlayerProfile.current.selectedProfileIndex)
            {
                if (i <= selectionButtons.Length)
                {
                    var button = selectionButtons[i];
                    button.selected = true;
                }
            }
        }

        profileSelectionPanel1.SetActive(false);
        profileSelectionPanel1.SetActive(false);

        if (PlayerProfile.current.selectedProfileIndex < 3)
        {
            profileSelectionPanel1.SetActive(true);
        }
        else
        {
            profileSelectionPanel2.SetActive(true);
        }
    }

    protected void OnDisable()
    {   
        PlayerProfile.Save();
    }


    #endregion


    #region Public Methods

	static public UIButton GetEditPlayerAvatarButton()
	{
		if (profileController == null)
		{
			return null;
        }
        
		return profileController.editPlayerAvatar;
	}

    static public UIButtonSelection GetCurrentSelectionButton()
    {
        if (profileController == null)
        {
            return null;
        }
        
        UIButtonSelection selection = null;

        foreach (UIButtonSelection sel in profileController.selectionButtons)
        {
            if (sel.selected)
            {
                selection = sel;
                break;
            }
        }

        return selection;
    }

	static public int GetSelectionButtonIndex(UIButtonSelection sel)
	{
		if (profileController == null)
		{
			return 0;
		}

        int selectionIndex = 0;
        for (int i = 0; i < profileController.selectionButtons.Length; i++)
		{
			if (sel == profileController.selectionButtons[i])
			{
				selectionIndex = i;
				break;
			}
		}

		return selectionIndex;
	}

	static public UIButtonSelection GetSelectionButton(int index)
	{
		if (profileController == null)
		{
			return null;
		}

		return profileController.selectionButtons[index];
	}

	static public int GetSelectedButtonIndex()
	{
		if (profileController == null)
		{
			return 0;
        }
        
		UIButtonSelection sel = GetCurrentSelectionButton();
		
		int selection = 0;
		for (; selection < profileController.selectionButtons.Length; selection++)
		{
			if (profileController.selectionButtons[selection] == sel)
			{
				break;
			}
		}
		
		return selection;
	}
	
	static public void SetProfileButton(UIButtonSelection button, string name, string avatar, string piece)
	{
		UISlicedSprite selBg = button.transform.FindChild("Background").GetComponent<UISlicedSprite>();
		UILabel selLabel = button.transform.Find("Label").GetComponent<UILabel>();
		selBg.spriteName = avatar;
        selLabel.text = name;
    }
    
    #endregion
    
    
    #region Internal Methods
    
    
    #endregion
    
    
    #region Events Methods
    
    protected void OnEditPlayer()
    {
        UIButtonSelection sel = GetCurrentSelectionButton();
        UISlicedSprite selBg = sel.transform.FindChild("Background").GetComponent<UISlicedSprite>();
        UILabel selLabel = sel.transform.Find("Label").GetComponent<UILabel>();
        UISlicedSprite avatarBg = editPlayerAvatar.transform.FindChild("Background").GetComponent<UISlicedSprite>();
        UILabel avatarLabel = editPlayerAvatar.transform.Find("Label").GetComponent<UILabel>();

        avatarBg.spriteName = selBg.spriteName;
        avatarLabel.text = selLabel.text;
    }

    protected void OnDeletePlayer()
    {
        UIButtonSelection sel = GetCurrentSelectionButton();
        int selectionIndex = GetSelectionButtonIndex(sel);
		var spriteName = convertProfileIndexToSpriteName(selectionIndex + 1);
        string playerName = "Player " + (selectionIndex + 1).ToString();

        sel.transform.FindChild("Label").GetComponent<UILabel>().text = playerName;
        sel.transform.FindChild("Background").GetComponent<UISlicedSprite>().spriteName = spriteName;

        int selectedIndex = GetSelectedButtonIndex();
        PlayerProfile.current.profiles[selectedIndex] = new PlayerProfile.Profile(playerName, spriteName, "");
        PlayerProfile.Save();

		Debug.Log("Delete and use SpriteName: " + spriteName);
    }

    /// <summary>
    /// Event where a player is selected and Proceed selected to continut to game - Default/Customized
    /// </summary>
    protected void OnSelectPlayer()
    {
        // Get selected profile
        UIButtonSelection sel = GetCurrentSelectionButton();

        // Get the index of the player profile
        int selectionIndex = GetSelectionButtonIndex(sel);

        PlayerProfile.current.selectedProfileIndex = selectionIndex;

        // Get the current game data slot
        var dataSlot = GameData.current.GetCurrentDataSlot();

        dataSlot.ClearPlayer();
        
        // Add the selected player to it
        var player = dataSlot.AddPlayer(selectionIndex);

        // Set the Color Path for the selected player
        player.colorPath = PlayerProfile.current.GetColorPathFromProfile(selectionIndex);
        
		Debug.Log("Player Selection: " + selectionIndex);

        GameData.Save();

        PlayerProfile.Save();       

 
		if (dismissOnSelect)
		{
			UINavigationController.DismissController();
		}
		else
		{
			UINavigationController.PushController(controllerRedirectOnSelect);
		}
    }

    protected int ConvertColorToNumber(string color)
    {
        if (mapColorToNumber.ContainsKey(color))
        {
            return mapColorToNumber[color];
        }

        return 1;
    }

	protected string convertProfileIndexToSpriteName(int number)
	{
		if (mapProfileIndexToSpriteName.ContainsKey(number))
		{
			return mapProfileIndexToSpriteName[number];
		}

		return "Box_Green";
	}


    #endregion
}
