using UnityEngine;
using System.Collections;

public class QuestionCategoryController : UIViewController
{
    #region MemVars & Props


    public GameObject hints;

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
        Intialize();
    }

    protected void OnDisable()
    {
        DeInitialize();
    }

    #endregion


    #region Public Methods

    protected void OnCategorySet()
    {
        Debug.Log("category set");
        var btn = UIHelper.GetSelectedButton(gameObject);
        Debug.Log(btn.name);

        if (hints != null)
        {
            UILabel label = UIHelper.GetLabel(gameObject);
            if (label != null)
            {
                label.text = btn.name;
            }
        }
    }


    #endregion


    #region Private Methods

    private void Intialize()
    {

    }

    private void DeInitialize()
    {

    }


    #endregion
}
