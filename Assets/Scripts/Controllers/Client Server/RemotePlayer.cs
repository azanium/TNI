using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RemotePlayer : Photon.MonoBehaviour 
{
	#region MemVars & Props

	private Vector3 playerPos = Vector3.zero;

	#endregion


	#region Mono Methods

	public void Update()
	{
		if (PhotonNetwork.connectedAndReady && !photonView.isMine)
		{
			//transform.position = Vector3.Lerp(transform.position, this.playerPos, Time.deltaTime * 5);
			/*string currentBlock = "_Root";
			PlayerController player = GetComponent<PlayerController>();
			if (currentBlock != "")
			{
				player.transform.parent = null;
			}
			else
			{
				Node block = GameBoard.FindBlockByName(currentBlock); 
				Debug.Log(block.name);
				player.transform.parent = block.transform;
			}*/ 

		}
	}

	#endregion


	#region Internal Methods
	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//Debug.Log("============>");
		if (stream.isWriting)
		{
			stream.SendNext(transform.position);
			PlayerController player = GetComponent<PlayerController>();
			string blockName = player.currentBlock == null ? "" : player.currentBlock.name;
			stream.SendNext(blockName);
		}
		else
		{
			PlayerController player = GetComponent<PlayerController>();

			this.playerPos = (Vector3)stream.ReceiveNext();
			string currentBlock = (string)stream.ReceiveNext();
			if (currentBlock != "")
			{
				player.transform.parent = null;
			}
			else
			{
				Node block = GameBoard.FindBlockByName(currentBlock); 
				Debug.Log("===>"+block.name);
				player.transform.parent = block.transform;
			} 
			transform.localPosition = this.playerPos;
		}
	}

	#endregion


	#region Public Methods

	#endregion
}
