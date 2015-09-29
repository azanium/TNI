using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
    #region MemVars & Props

    private GameObject mainMenu;

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
    }

    protected void OnDestroy()
    {
        GameObject.Destroy(mainMenu);
    }

	protected void Start() 
    {
        MainController.SetLoadingProgess(1);
        mainMenu = (GameObject)Instantiate(Resources.Load("Prefabs/Main Menu"));
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

    #endregion


    #region Private Methods

    #endregion
}
