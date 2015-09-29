using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectionController : UIViewController 
{
	#region MemVars & Props

	public UILabel connectionLabel;
	private bool isShowing = false;

	#endregion


	#region Mono Methods

	public override void OnAppear()
	{
		isShowing = true;
	}

	public override void OnDissapear ()
	{
		isShowing = false;
	}

	public override void Update()
	{
		base.Update();

		if (connectionLabel != null)
		{
			string detailConnection = PhotonNetwork.connectionStateDetailed.ToString();

			connectionLabel.text = detailConnection;
		}

	}

	#endregion


	#region Internal Methods
	

	#endregion


	#region Public Methods

	public void Dismiss()
	{
		if (isShowing) 
		{
			UINavigationController.DismissAllControllers();
		}
	}

	#endregion
}
