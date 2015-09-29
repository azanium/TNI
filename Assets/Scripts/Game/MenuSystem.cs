using UnityEngine;
using System.Collections;

public class MenuSystem : MonoBehaviour
{
    #region MemVars & Props

    public int defaultPanelBringIn = 0;

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
    }

	protected void Start() 
    {
	}
	
	protected void Update() 
    {
    }

    protected void OnEnable()
    {
    }

    protected void OnDisable()
    {
    }

    #endregion


    #region Public Methods

    public void StartGame()
    {
        MainController.SwitchScene("Game");
    }

    #endregion


    #region Private Methods

    #endregion
}
