using UnityEngine;

/// <summary>
/// Sends a message to the remote object when something happens.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Push Background")]
public class UIButtonPushBackground : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick,
	}

	public enum PushMethod
	{
		UseInstance,
		UseControllerPath,
		UseLayer
	}
	
	/// <summary>
	/// Leave it blank if you already set the controller somewhere, it will auto detect
	/// </summary>
	public UINavigationController navigationController;

	public UIBackgroundController backgroundControllerInstance;
	public string backgroundControllerPath;
	public int backgroundLayer = 0;
	public PushMethod PushAction = PushMethod.UseInstance;


	public Trigger trigger = Trigger.OnClick;
	public GameObject callbackTarget;

	public string appearMethod;
	public string appearedMethod;
	public string controllerParam = "";
	
	bool mStarted = false;
	bool mHighlighted = false;
	
	void Start() 
	{ 
		mStarted = true;
		if (navigationController == null)
		{
			navigationController = UINavigationController.navigationController;
		}

		if (backgroundControllerInstance == null) 
		{
			backgroundControllerInstance = gameObject.GetComponent<UIBackgroundController>();
		}
	}
	
	void OnEnable() { if (mStarted && mHighlighted) OnHover(UICamera.IsHighlighted(gameObject)); }
	
	void OnHover(bool isOver)
	{
		if (enabled)
		{
			if (((isOver && trigger == Trigger.OnMouseOver) ||
			     (!isOver && trigger == Trigger.OnMouseOut))) Send();
			mHighlighted = isOver;
		}
	}
	
	void OnPress(bool isPressed)
	{
		if (enabled)
		{
			if (((isPressed && trigger == Trigger.OnPress) ||
			     (!isPressed && trigger == Trigger.OnRelease))) Send();
		}
	}
	
	void OnClick() { if (enabled && trigger == Trigger.OnClick) Send(); }
	
	void OnDoubleClick() { if (enabled && trigger == Trigger.OnDoubleClick) Send(); }
	
	void Update()
	{
		// Keep finding if we failed the first time
		if (navigationController == null)
		{
			navigationController = UINavigationController.navigationController;
		}
	}
	
	void Send()
	{
		if (navigationController != null)
		{
			switch (PushAction)
			{
			case PushMethod.UseControllerPath:
				navigationController.pushBackground(backgroundControllerPath);
				break;

			case PushMethod.UseInstance:
				navigationController.pushBackground(backgroundControllerInstance);
				break;

			case PushMethod.UseLayer:
				navigationController.pushBackground(backgroundLayer);
				break;
			}
		}
	}
}