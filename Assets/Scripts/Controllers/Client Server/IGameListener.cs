using UnityEngine;
using System.Collections;

public interface IGameListener 
{
	void OnConnecting(IGameClient client);
	void OnConnected(IGameClient client);
	void OnDisconnected(IGameClient client);
	void OnJoined(IGameClient client, int index, GameData.PlayerData player);
	void OnGameEnded(IGameClient client);
	
	void OnPlayerTurn(int index);
	void OnMovePlayer(int index);
}
