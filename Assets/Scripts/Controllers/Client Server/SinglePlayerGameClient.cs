using UnityEngine;
using System.Collections;

public class SinglePlayerGameClient : MonoBehaviour, IGameClient 
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
			gameListener.OnConnected(this);
		}
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
		if (gameListener != null)
		{
			foreach (var player in this.dataSlot.players)
			{
				gameListener.OnJoined(this, totalPlayers, player);
			}
			totalPlayers ++;
			currentTurn = 0;
			gameListener.OnPlayerTurn(currentTurn);
		}
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
}
