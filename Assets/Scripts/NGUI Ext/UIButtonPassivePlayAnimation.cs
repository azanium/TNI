//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using AnimationOrTween;

/// <summary>
/// Play the specified animation on click.
/// Sends out the "OnAnimationFinished()" notification to the target when the animation finishes.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Passive Play Animation")]
public class UIButtonPassivePlayAnimation : MonoBehaviour
{
	public GameObject target;

    public GameObject eventTrigger;

    public string callToTrigger;
	public ActiveAnimation.OnFinished onFinished;

    void OnClick()
    {
        if (enabled)
        {
			if (eventTrigger == null)
			{
				eventTrigger = gameObject;
			}
            if (eventTrigger != null && !string.IsNullOrEmpty(callToTrigger))
            {
                eventTrigger.SendMessage(callToTrigger, this, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void Play()
    {
		if (target != null)
        {			
			UIButtonPlayAnimation[] animPlay = target.gameObject.GetComponents<UIButtonPlayAnimation>();
	        foreach (UIButtonPlayAnimation anim in animPlay)
	        {
	            anim.Play(true);
	        }
	
	        UIButtonActivate[] actives = target.gameObject.GetComponents<UIButtonActivate>();
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
    }
	
	public void SimulateClick()
    {
		
    }

}