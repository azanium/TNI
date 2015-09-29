using UnityEngine;
using System.Collections;

public class BannerBillboard : MonoBehaviour
{
    #region MemVars & Props

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
        if (Camera.main != null)
        {
            Debug.Log("cam");
            //Vector3 direction = Camera.main.transform.position - transform.position;
            transform.LookAt(Camera.mainCamera.transform.position);
        }
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
