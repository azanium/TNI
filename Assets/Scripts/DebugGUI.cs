using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugGUI : MonoBehaviour 
{
	#region MemVars & Props

	#endregion


	#region Mono Methods

	public void OnGUI()
	{
		GUILayout.Space(0);
		
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		
		if (!PhotonNetwork.connected)
		{
			if (GUILayout.Button("Connect"))
			{
				PhotonNetwork.ConnectUsingSettings("1");
			}
		}
		else
		{
			if (GUILayout.Button("Disconnect"))
			{
				PhotonNetwork.Disconnect();
			}
		}

		if (GUILayout.Button("Instantiate"))
		{
			Node root = GameBoard.GetRootBlock();
			PlayerController player = PlayerController.createAvatarPlayer(ColorPathType.Blue, root.GetTargetTransform().position, Quaternion.identity);
			GameBoard.MovePlayerToBlockNode(player, root);
		}
	}

	#endregion


	#region Internal Methods

	#endregion


	#region Public Methods

	#endregion
}
