using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDController : UIViewController
{
    #region MemVars & Props
	
	private static HUDController hudController;
	
    public PlayerIconController[] playerIcons;
	public GameObject questionTypeHint;
	public PillarsHudController pillarsHudController;

    public static Dictionary<string, string> mapPillarToCharacterIcon = new Dictionary<string, string>()
    {
        { "Box_Red", "character4" },
        { "Box_Green", "character2" }, 
        { "Box_Blue", "character1" },
        { "Box_Yellow", "character6" },
        { "Box_Purple", "character3" },
        { "Box_Orange", "character5" }
    };

    public static string GetCharacterNameByPillarName(string pillarName)
    {
        if (mapPillarToCharacterIcon.ContainsKey(pillarName))
        {
            return mapPillarToCharacterIcon[pillarName];
        }

        return "character1";
    }

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
		hudController = this;
    }

    #endregion


    #region Public Methods
	
	public static void DisplayQuestionTypeHint(NodeType blockType)
	{
		if (hudController != null)
		{
			hudController.ShowQuestionTypeHint(blockType);
		}
	}

    public static void DisplayMessage(string message)
    {
        if (hudController != null)
        {
            hudController.ShowMessage(message);
        }
    }
	
	public static void HideQuestionTypeHint()
	{
		if (hudController != null)
		{
			hudController.questionTypeHint.SetActive(false);
		}
	}

    #endregion


    #region Private Methods

	public void Setup(GameData.DataSlot dataSlot)
    {
        foreach (PlayerIconController obj in playerIcons)
        {
            obj.gameObject.SetActive(false);
		}
		
		if (questionTypeHint != null)
		{
			questionTypeHint.SetActive(false);
		} 

		if (pillarsHudController != null)
		{
			pillarsHudController.SetupPillars(dataSlot.pillarsNeeded);
		}

		SetupPlayersIcon(dataSlot);
    }

	public void SetupPlayersIcon(GameData.DataSlot slot)
	{
		if (playerIcons == null)
		{
			return;
		}

		for (int i = 0; i < slot.players.Count; i++)
		{
			var currentPlayer = slot.players[i];
			var playerObj = playerIcons[i];
			
			playerObj.gameObject.SetActive(true);
			var playerSprite = GetPlayerAvatar(playerObj.gameObject);
			var playerLabel = GetPlayerLabel(playerObj.gameObject);
			
			var playerProfile = currentPlayer.GetPlayerProfile();
			
			playerSprite.spriteName = GetCharacterNameByPillarName(playerProfile.Avatar);
			playerLabel.text = playerProfile.Name;

			playerObj.pillarsHudController.SetupPillars(currentPlayer.pillarsAcquired);
		}
	}

		


	protected UISlicedSprite GetSpriteByName(GameObject obj, string name)
	{
		var sprites = obj.GetComponentsInChildren<UISlicedSprite>(true);

		UISlicedSprite sprite = null;

		foreach (var sp in sprites)
		{
			if (sp.name == name)
			{
				sprite = sp;
				break;
			}
		}

		return sprite;
	}

	protected UILabel GetLabelByName(GameObject obj, string name)
	{
		var labels = obj.GetComponentsInChildren<UILabel>(true);
		
		UILabel label = null;
		
		foreach (var lb in labels)
		{
			if (lb.name == name)
			{
				label = lb;
				break;
			}
		} 
		
		return label;
	}

    protected UISlicedSprite GetPlayerAvatar(GameObject obj)
    {
		return GetSpriteByName(obj, "Avatar");
    }

    protected UILabel GetPlayerLabel(GameObject obj)
    {
		return GetLabelByName(obj, "Label");
    }

	private void ShowQuestionTypeHint(NodeType nodeType)
	{
		if (questionTypeHint != null)
		{
			string questionType;
			Color color = GameData.GetHintColorFromNodeType(nodeType, out questionType);
			UIHelper.ChangeSpriteColor(questionTypeHint, color);
			UILabel label = UIHelper.GetLabel(questionTypeHint);
			label.text = questionType;
			questionTypeHint.SetActive(true);
		}
	}
	
	private void ShowMessage(string message)
	{
		if (questionTypeHint != null)
		{
			UIHelper.ChangeSpriteColor(questionTypeHint, Color.gray);
			UILabel label = UIHelper.GetLabel(questionTypeHint);
			label.text = message;
			questionTypeHint.SetActive(true);
		}
	}

    #endregion
}
