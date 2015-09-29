using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    #region MemVars & Props

    static private GameController gameController;
    private List<GameObject> objects;

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
        gameController = this;
    }

    protected void OnDestroy()
    {
        gameController = null;

        foreach (GameObject obj in objects)
        {
            GameObject.Destroy(obj);
        }
    }

	protected void Start() 
    {
        Level.Load();
        PlayerProfile.Load();

        objects = new List<GameObject>();

        /*foreach (Level.Item item in Level.current.items)
        {
            //Debug.LogWarning(item.name);
            //GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/" + item.name));

            //objects.Add(obj);

        }*/
        MainController.SetLoadingProgess(1);
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
