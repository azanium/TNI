using UnityEngine;
using System.Collections;

public class SinglePlayerController : MonoBehaviour
{
    #region MemVars & Props

    #endregion


    #region MonoBehavior's Methods

    protected virtual void Awake()
    {
    }

    protected virtual void Start() 
    {
	}

    protected virtual void OnEnable()
    {
        Initialize();
    }

    protected virtual void OnDisable()
    {
        DeInitialize();
    }

    #endregion


    #region Public Methods

    #endregion


    #region Private Methods

    protected virtual void Initialize()
    {
        GameData.Load();
    }

    protected virtual void DeInitialize()
    {
    }

    protected virtual void OnNewGame()
    {
        GameData.current.NewGame();
        GameData.Save();
		Debug.Log("New Game");
    }

	protected virtual void StartGame()
    {
		MainGameController.ShowHud(true);
		UINavigationController.DismissAllControllers();
		UINavigationController.DismissBackground("/MainBackground");

        MainGameController.StartLocalSinglePlayerGame();
    }

	public virtual void StartMultiplayer()
	{
		MainGameController.ShowHud(true);
		UINavigationController.DismissAllControllers();
		UINavigationController.DismissBackground("/MainBackground");
		
		MainGameController.StartMultiplayerGame();
	}


    #endregion
}
