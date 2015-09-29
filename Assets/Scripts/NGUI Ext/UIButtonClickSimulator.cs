using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Interaction/Button Click Simulator")]
public class UIButtonClickSimulator : MonoBehaviour
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


    #endregion


    #region Public Methods

    public void SimulateClick()
    {
        UIButtonPlayAnimation[] animPlay = gameObject.GetComponents<UIButtonPlayAnimation>();
        foreach (UIButtonPlayAnimation anim in animPlay)
        {
            anim.Play(true);
        }

        UIButtonActivate[] actives = gameObject.GetComponents<UIButtonActivate>();
        foreach (UIButtonActivate act in actives)
        {
            act.Activate();
        }

        /*UIButtonPlayAnimation[] components = gameObject.GetComponents<UIButtonPlayAnimation>();
        foreach (UIButtonPlayAnimation comp in components)
        {
            Debug.LogWarning(comp.name);
            comp.SendMessage("OnClick");
        }*/
    }

    #endregion


    #region Private Methods

    #endregion
}
