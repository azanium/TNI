using UnityEngine;
using System.Collections;

public class OnlineGameClient : Photon.MonoBehaviour, IGameClient
{
	#region MemVars & Props
	
	private IGameListener gameListener;
	private int currentTurn = 0;
	private int totalPlayers = 0;
	private GameData.DataSlot dataSlot;
	
	#endregion
	
	
	#region IGameClient implementation
	
	public void Initialize(IGameListener listener, GameData.DataSlot dataSlot)
	{
		gameListener = listener;
		this.dataSlot = dataSlot;
	}

	public void Update() 
	{
	}
	
	public void Connect()
	{
		if (gameListener != null)
		{
			gameListener.OnConnecting(this);
		}
		PhotonNetwork.ConnectUsingSettings("1");
	}
	
	public void Disconnect()
	{
		if (gameListener != null)
		{
			gameListener.OnDisconnected(this);
		}
	}
	
	public void CreateGame()
	{
		
	}
	
	public void Join()
	{
		PhotonNetwork.JoinRandomRoom();
	}
	
	public void EndTurn(object data)
	{
		currentTurn++;
		if (currentTurn >= totalPlayers - 1);
		{
			currentTurn = totalPlayers - 1;
		}
		gameListener.OnPlayerTurn(currentTurn);
	}
	
	public void EndGame()
	{
		gameListener.OnGameEnded(this);
	}
	
	
	#endregion


	#region Photon Methods
	
	public virtual void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
		gameListener.OnConnected(this);
	}
	
	public virtual void OnPhotonRandomJoinFailed()
	{
		Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
		PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 4 }, null);
	}
	
	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.LogError("Failed to connect, cause: " + cause);
	}
	
	public virtual void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
		gameListener.OnJoined(this, 0, this.dataSlot.GetCurrentPlayerData());
	}
	
	public virtual void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby(). Use a GUI to show existing rooms available in PhotonNetwork.GetRoomList().");
	}
	
	#endregion
}
