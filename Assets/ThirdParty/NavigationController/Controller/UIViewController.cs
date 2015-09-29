using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIViewController : MonoBehaviour
{
    #region MemVars & Props

    public virtual bool StackPushable
    {
        get { return true; }
    }

    public AnimationClip showForwardAnimation;
    public AnimationClip hideForwardAnimation;
    public AnimationClip showBackAnimation;
    public AnimationClip hideBackAnimation;
	public AudioClip backgroundAudio;

    public Dictionary<string, string> controllerParameters = new Dictionary<string, string>();

    public string controllerPath;

	protected bool IsControllerParamExist(string param)
	{
		return controllerParameters.ContainsKey(param);
	}

	protected string GetControllerParamValue(string param)
	{
		if (IsControllerParamExist(param))
		{
			return controllerParameters[param];
		}
		return "";
	}

	protected bool TryGetControllerParamValue(string param, out string value)
	{
		bool ret = IsControllerParamExist(param);

		if (ret)
		{
			value = GetControllerParamValue(param);
		}
		else 
		{
			value = "";
		}

		return ret;
	}

    #endregion


    #region Virtual Methods

    public virtual void Awake()
    {
    }

    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void OnEnable()
    {
    }

    public virtual void OnDisable()
    {
    }

    public virtual void OnAppear()
    {
    }

    public virtual void OnAppeared()
    {
    }

    public virtual void OnDissapear()
    {
    }

    public virtual void OnDisappeared()
    {
    }

    #endregion


    #region Public Methods

    #endregion


    #region Internal Methods

    #endregion
}
