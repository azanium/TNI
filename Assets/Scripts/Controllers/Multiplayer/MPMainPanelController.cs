using UnityEngine;
using System.Collections;

public class MultiplayerController : SinglePlayerController
{
    #region MemVars & Props

    #endregion


    #region MonoBehavior's Methods

    protected override void Awake()
    {
        base.Awake();
    }

	protected override void Start() 
    {
        base.Start();
	}

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    #endregion


    #region Public Methods

    #endregion


    #region Private Methods

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void DeInitialize()
    {
        base.DeInitialize();
    }

    protected override void OnNewGame()
    {
        GameData.current.NewGame();
        GameData.Save();
    }

    protected override void StartGame()
    {
        GameBoard.StartGame();
    }

    #endregion
}
