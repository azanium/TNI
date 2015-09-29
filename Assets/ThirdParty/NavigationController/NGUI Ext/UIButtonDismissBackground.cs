using UnityEngine;

/// <summary>
/// Sends a message to the remote object when something happens.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Dismiss Background")]
public class UIButtonDismissBackground : MonoBehaviour
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

	public enum DismissMethod
	{
		UseInstance,
		UseControllerPath,
		UseLayer
	}
	
	/// <summary>
	/// Leave it blank if you already set the controller somewhere, it will auto detect
	/// </summary>
	public UINavigationController navigationController;
	public Trigger trigger = Trigger.OnClick;

	public UIBackgroundController backgroundInstance;
	public string backgroundControllerPath;
	public int backgroundLayer = 0;
	public DismissMethod dismissAction = DismissMethod.UseInstance;

	bool mStarted = false;
	bool mHighlighted = false;
	
	void Start()
	{
		mStarted = true;
		if (navigationController == null)
		{
			navigationController = UINavigationController.navigationController;
		}

		if (backgroundInstance == null)
		{
			backgroundInstance = this.gameObject.GetComponent<UIBackgroundController>();
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
	
	void Send()
	{
		if (navigationController != null)
		{
			switch (dismissAction)
			{
			case DismissMethod.UseControllerPath:
				navigationController.dismissBackground(backgroundControllerPath);
				break;

			case DismissMethod.UseInstance:
				navigationController.dismissBackground(backgroundInstance);
				break;

			case DismissMethod.UseLayer:
				navigationController.dismissBackground(backgroundLayer);
				break;
			}
		}
	}
}