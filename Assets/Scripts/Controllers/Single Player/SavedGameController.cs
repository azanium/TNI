using UnityEngine;
using System.Collections;

public class SavedGameController : UIViewController
{
    #region MemVars & Props

    public UIButtonSelection[] gameDataSlots;
    public UISlicedSprite[] avatarSlots;
    public UIButton deleteButton;
	
	public enum SavedGameStateType
	{
		None,
		NewGame,
		ContinueGame
	}
	private SavedGameStateType state = SavedGameStateType.None;

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
    }

	protected void Start() 
    {
        if (gameDataSlots == null || avatarSlots == null)
        {
            Debug.LogError("Game Slots is not set");
        }
	}
	
    protected void OnEnable()
    {
        Initialize();
    }

    protected void OnDisable()
    {
        DeInitialize();
    }

    #endregion


    #region Public Methods

    #endregion


    #region Private Methods

    protected void Initialize()
    {
		state = SavedGameStateType.None;
		
        GameData.Load();
        PlayerProfile.Load();
		
		bool isDeleteEnabled = false;
		
        //foreach (GameDataSlot.DataSlot slot in GameDataSlot.current.dataSlots)
        for (int i = 0; i < GameData.current.dataSlots.Length; i++)
        {
            var slot = GameData.current.dataSlots[i];

            var slotButton = gameDataSlots[i];
            var slotLabel = UIHelper.GetButtonLabel(slotButton);
			var avatarSprite = avatarSlots[i];
			
            if (slot.isEmpty)
            {
                slotLabel.text = "Empty";
				avatarSprite.spriteName = "transparent box";
            }
            else
            {
                var profile = PlayerProfile.current.profiles[slot.currentPlayerIndex];

                slotLabel.text = profile.Name;
                avatarSprite.spriteName = profile.Avatar;
				isDeleteEnabled = true;
            }
        }
		
		deleteButton.gameObject.SetActive(isDeleteEnabled);
    }

    protected void DeInitialize()
    {
        GameData.Save();
    }

    protected void OnDelete()
    {
        //UIButtonSelection button = UIHelper.GetSelectedButtonSelection(this.gameObject);
        int selectedIndex = GetSelectedButtonIndex();

        GameData.current.Delete(selectedIndex);

        // Reinitialize again
        Initialize();
    }

    protected int GetSelectedButtonIndex()
    {
        var button = UIHelper.GetSelectedButtonSelection(gameObject);
        int selection = 0;
        for (; selection < gameDataSlots.Length; selection++)
        {
            if (gameDataSlots[selection] == button)
            {
                break;
            }
        }

        return selection;
    }
    protected int lastSelectionIndex = -1;
    protected void OnSelectSlot(GameObject obj)
    {
        int selectedIndex = GetSelectedButtonIndex();
        var dataSlot = GameData.current.dataSlots[selectedIndex];

        deleteButton.gameObject.SetActive(!dataSlot.isEmpty);
        if (lastSelectionIndex == selectedIndex)
        {
            GameData.current.currentSlot = selectedIndex;
            OnPlay();
        }
        lastSelectionIndex = selectedIndex;
    }

    protected void OnPlay()
    {
        
    }

    protected bool isCurrentSavedDataEmtpy()
    {
        int selectedIndex = GetSelectedButtonIndex();
        var dataSlot = GameData.current.dataSlots[selectedIndex];

        return dataSlot.isEmpty;
    }

    protected void OnNewGame(UIButtonPassivePlayAnimation btn)
    {
        if (isCurrentSavedDataEmtpy() && state == SavedGameStateType.None)
        {
			int selectedIndex = GetSelectedButtonIndex();
			GameData.current.NewGame(selectedIndex);
			GameData.Save();
			Debug.Log("new game");
            btn.Play();
			state = SavedGameStateType.NewGame;
        }
    }

    protected void OnContinueGame(UIButtonPassivePlayAnimation btn)
    {
        if (isCurrentSavedDataEmtpy() == false && state == SavedGameStateType.None)
        {
			Debug.Log("continue game");
			
            btn.Play();
			state = SavedGameStateType.ContinueGame;
        }
    }

    #endregion
}
