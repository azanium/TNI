using UnityEngine;
using System.Collections;

public interface IGameClient
{
	void Initialize(IGameListener listener, GameData.DataSlot gameSlot);
	void Connect();
	void Disconnect();
	void CreateGame();
	void Join();
	void EndTurn(object data);
	void EndGame();

	void Update();
}
