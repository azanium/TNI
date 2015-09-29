using UnityEngine;
using System.Collections;

public class GameServerController : MonoBehaviour 
{
	#region MemVars & Props

	private static GameServerController gameServerController;
	public string ipAddress = "127.0.0.1";
	public string port = "9999";

	#endregion


	#region Mono Methods

	protected void Awake()
	{
		gameServerController = this;
	}

	#endregion


	#region Internal Methods

	#endregion


	#region Public Methods



	#endregion
}
