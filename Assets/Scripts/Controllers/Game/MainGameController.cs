using UnityEngine;
using System.Collections;

public class MainGameController : MonoBehaviour, IGameListener
{
	#region MemVars & Props

	private static MainGameController mainGameController;

	public enum GameType
	{
		LocalSinglePlayer,
		LocalMultiPlayer,
		WifiMultiplayer,
		Online,
	}

	public class PlayerInfo
	{
		public PlayerController playerController;
		public GameData.PlayerData playerData;

		public PlayerInfo(PlayerController controller, GameData.PlayerData data)
		{
			playerController = controller;
			playerData = data;
		}
	}

	public GameBoard gameBoard;
	public UIBackgroundController mainBackground;
	public HUDController hudController;
	public QAController questionsController;
	public ConnectionController connectionViewController;

	private System.Collections.Generic.Dictionary<int, PlayerInfo> players = new System.Collections.Generic.Dictionary<int, PlayerInfo>();

	public IGameClient GameClientPlugin;
	private Node activeNode;

	#endregion


	#region Mono Methods

	public virtual void Awake()
	{
		mainGameController = this;
	}

	public virtual void Start()
	{
		PhotonNetwork.autoJoinLobby = false;
	}

	public virtual void OnEnable()
	{
		if (questionsController != null)
		{
			questionsController.OnCategorySelected += OnCategorySelected;	
			questionsController.OnChoiceSelected += OnChoiceSelected;
		}

		if (gameBoard != null)
		{
			gameBoard.OnNodeSelected += OnNodeSelected;
		}
	}

	public virtual void OnDisable()
	{
		if (questionsController != null)
		{
			questionsController.OnCategorySelected -= OnCategorySelected;	
			questionsController.OnChoiceSelected -= OnChoiceSelected;
		}

		if (gameBoard != null)
		{
			gameBoard.OnNodeSelected -= OnNodeSelected;
		}
	}

	public virtual void Update() 
	{
		if (GameClientPlugin != null)
		{
			GameClientPlugin.Update();
		}
	}

	#endregion


	#region Internal Methods


	#endregion


	#region Public Methods

	public static void EndGame()
	{
		if (mainGameController != null)
		{
			mainGameController.GameClientPlugin.EndGame();
		}
	}

	public static void ShowHud(bool state)
	{
		if (mainGameController != null)
		{
			if (mainGameController.hudController != null)
			{
				mainGameController.hudController.gameObject.SetActive(state);
			}
		}
	}

	public void ShowBoard(bool state)
	{
		gameBoard.gameObject.SetActive(state);
	}

	public static void StartLocalSinglePlayerGame()
	{
		if (mainGameController != null)
		{
			mainGameController.startGame(GameType.LocalSinglePlayer);
		}
	}

	public static void StartMultiplayerGame() 
	{
		if (mainGameController != null)
		{
			mainGameController.startGame(GameType.Online);
		}
	}

	public void startGame(GameType gameType)
	{
		Debug.Log("Starting the Game...");
		GameData.Load();

		var dataSlot = GameData.current.GetCurrentDataSlot();
		hudController.Setup(dataSlot);

		switch (gameType)
		{
		case GameType.LocalSinglePlayer:
			LocalSinglePlayerGame(dataSlot);
			break;

		case GameType.Online:
			OnlineMultiplayerGame(dataSlot);
			break;
		}
	}

	public void SetGameClientPlugin(IGameClient gameClient)
	{
		if (GameClientPlugin != gameClient)
		{
			GameData.Load();
			var dataSlot = GameData.current.GetCurrentDataSlot();

			GameClientPlugin = gameClient;
			GameClientPlugin.Initialize(this, dataSlot);
		}
	}

	protected void LocalSinglePlayerGame(GameData.DataSlot dataSlot)
	{
		SinglePlayerGameClient plugin = gameObject.GetComponent<SinglePlayerGameClient>();
		if (plugin == null)
		{
			plugin = gameObject.AddComponent<SinglePlayerGameClient>();
		}
		SetGameClientPlugin(plugin);

		GameClientPlugin.Connect();
	}

	protected void OnlineMultiplayerGame(GameData.DataSlot dataSlot)
	{
		OnlineGameClient plugin = gameObject.GetComponent<OnlineGameClient>();
		if (plugin == null)
		{
		 	plugin = gameObject.AddComponent<OnlineGameClient>();
		}
		SetGameClientPlugin(plugin);

		UINavigationController.PushController(connectionViewController);
		GameClientPlugin.Connect();
	}

	#endregion


	#region Events

	public void OnCategorySelected(QAController.QuestionCategory category)
	{
		ShowHud(true);
		UINavigationController.DismissBackground("/MainBackground");
		UINavigationController.DismissAllControllers();
		Debug.Log(category);
		var player = GetCurrentPlayerController();
		int steps = category == QAController.QuestionCategory.Easy ? 1 : category == QAController.QuestionCategory.Medium ? 2 : 3;
		gameBoard.PlayerHightlightBlock(player, steps);
	}

	public void HighlightBlock(PlayerController player, int steps)
	{
		gameBoard.PlayerHightlightBlock(player, steps);
	}

	/// <summary>
	/// Called when a player tap on a block
	/// </summary>
	/// <param name="node">Node.</param>
	public void OnNodeSelected(Node node)
	{
		var prevNode = activeNode;
		
		activeNode = node;
		Debug.Log("OnNodeSelected");
		GameBoard.KillSpotLights();
		
		bool cancelOtherJumps = false;
		if (prevNode != null)
		{
			if (prevNode.nodeType == NodeType.AnswerEasyToWinTwoJump)
			{
				cancelOtherJumps = true;
				GameBoard.MovePlayer(GetCurrentPlayerController(), activeNode.gameObject, ProcessAfterPlayerMoved);
			}
		}
		
		if (!cancelOtherJumps)
		{
			switch (node.nodeType)
			{
			case NodeType.JumpToAnyPillar:
				questionsController.ShowCategory(node);
				break;
				
			case NodeType.Root:
			case NodeType.ChooseCategory:
				questionsController.ShowCategory(node);
				break;
				
			default:
				questionsController.ShowQuestion(node);
				break;
			}
		}
	}

	/// <summary>
	/// Called when a user click on a block and answered a question related to that block
	/// </summary>
	/// <param name="isCorrect">If set to <c>true</c> is correct.</param>
	public void OnChoiceSelected(bool isCorrect)
	{
		Debug.Log("OnChoiceSelected: " + isCorrect + ", ActiveNode: " + activeNode + ", NodeType: " + activeNode.nodeType);
		ShowHud(true);

		if (isCorrect)
		{
			GameBoard.MovePlayer(GetCurrentPlayerController(), activeNode.gameObject, (p) =>
			{
				switch (activeNode.nodeType)
				{
				case NodeType.JumpToAnyPillar:
					GameBoard.HighlightPillarBlocks(p);
					HUDController.DisplayMessage("Select the pillar to jump");
					break;
					
				case NodeType.AnswerEasyToWinTwoJump:
					Debug.Log("Win 2 Jumps");
					HUDController.DisplayMessage("Select the block for free jump");
					HighlightBlock(GetCurrentPlayerController(), 2);
					break;

				case NodeType.WinAPillarOrGoToSchool:
					ProcessAfterPlayerMovedToWinAPillarOrGoToSchool(GetCurrentPlayerController());
					Debug.Log("Player win a free pillar");
					break;

				default:
					ProcessAfterPlayerMoved(p);
					Debug.Log("Moved normally");
					break;
				}
			});
		}
		else 
		{
			// If a player is tapping a Win A Pillar or go to school, but failed to answer then punish them
			if (activeNode.nodeType == NodeType.WinAPillarOrGoToSchool)
			{
				GameBoard.MovePlayer(GetCurrentPlayerController(), gameBoard.schoolBoxBlock.gameObject, ProcessAfterPlayerMoved);
			}
			else
			{
				activeNode = null;
				GameClientPlugin.EndTurn(null);
			}
		}
	}

	/// <summary>
	/// Called after a player is landed on WinAPillarOrGoTschool and answer correctly
	/// </summary>
	/// <param name="player">Player.</param>
	private void ProcessAfterPlayerMovedToWinAPillarOrGoToSchool(PlayerController player)
	{
		currentPlayerGotPillar = player;

		GameData.PillarType pillar;
		if (player.GetFreePillar(out pillar))
		{
			UINavigationController.PushController("/WinPillar", (c) => {
				WinPillarController pillarController = (WinPillarController)c;
				if (pillarController)
				{
					pillarController.PillarWon(pillar);
					pillarController.target = this.gameObject;
					pillarController.func = "AddFreePillar";
				}
			}, null);
		}
	}

	PlayerController currentPlayerGotPillar = null;
	/// <summary>
	/// Called after a player is succesfully landed on the target block and the player is correctly answers
	/// </summary>
	/// <param name="player">Player.</param>
	private void ProcessAfterPlayerMoved(PlayerController player)
	{
		if (activeNode != null)
		{
			currentPlayerGotPillar = player;

			Debug.Log("Player has moved to " + activeNode);
			if (activeNode.isPillar)
			{
				var pillar = GameData.GetColorTypeFromNodeType(activeNode.nodeType);

				UINavigationController.PushController("/WinPillar", (c) => {
					WinPillarController pillarController = (WinPillarController)c;
					if (pillarController)
					{
						pillarController.PillarWon(pillar);
						pillarController.target = this.gameObject;
						pillarController.func = "AddPillar";
					}
				}, null);
			}
			else
			{
				switch (activeNode.nodeType)
				{
				case NodeType.Root:
					ShowHud(false);
					UINavigationController.PushBackground("/MainBackground");
					UINavigationController.PushController("/GameOver");
					break;

				default:
					GameClientPlugin.EndTurn(null);
					break;
				}
			}
		}
	}
	
	private void AddPillar()
	{
		if (currentPlayerGotPillar != null)
		{
			Debug.Log("Got Pillar");
			AddPillar(currentPlayerGotPillar, activeNode);
		}
		currentPlayerGotPillar = null;
	}

	private void AddFreePillar()
	{
		if (currentPlayerGotPillar != null)
		{
			Debug.Log("Got A Free Pillar");
			AddFreePillar(currentPlayerGotPillar);
		}
		currentPlayerGotPillar = null;
	}

	private void AddPillar(PlayerController player, Node node)
	{
		var pillar = GameData.GetColorTypeFromNodeType(node.nodeType);
		if (player.AddPillar(pillar, ProcessAfterPillarLanded))
		{
			GameData.Load();
			
			var slot = GameData.current.GetCurrentDataSlot();
			var playerData = slot.GetCurrentPlayerData();
			playerData.pillarsAcquired.Add(pillar);
			
			GameData.Save(); 
		}
		else
		{
			// If no pillar added, call next turn
			GameClientPlugin.EndTurn(null);
		}
	}

	private void AddFreePillar(PlayerController player)
	{
		GameData.PillarType pillar;
		if (player.AddFreePillar(out pillar, ProcessAfterPillarLanded))
		{
			if (pillar != GameData.PillarType.None)
			{
				GameData.Load();
				
				var slot = GameData.current.GetCurrentDataSlot();
				var playerData = slot.GetCurrentPlayerData();
				playerData.pillarsAcquired.Add(pillar);
				
				GameData.Save();
			}
		}
		else
		{
			GameClientPlugin.EndTurn(null);
		}
	}

	/// <summary>
	/// After the block is landed and it was Pillar, and after pillar landed this function are called
	/// </summary>
	/// <param name="player">Player.</param>
	private void ProcessAfterPillarLanded(PlayerController player)
	{
		GameData.Load();
		var slot = GameData.current.GetCurrentDataSlot();
		hudController.SetupPlayersIcon(slot);
		
		GameCamera.ResetCameraPosition();
		GameClientPlugin.EndTurn(null);
		activeNode = null;
	}
	
	#endregion
	

	#region IGameListener implementation

	public void OnConnecting(IGameClient client)
	{
		Debug.LogWarning("Connecting");
	}

	public void OnConnected(IGameClient client) 
	{
		client.Join();
	}

	public void OnDisconnected(IGameClient client) 
	{
		if (gameObject.GetComponent<SinglePlayerGameClient>() != null)
		{

		}
	}

	public void OnJoined(IGameClient client, int index, GameData.PlayerData playerData)
	{
		Node startBlock = GameBoard.FindBlockByName(playerData.currentBlockName);

		if (connectionViewController != null)
		{
			connectionViewController.Dismiss();
		}

		if (startBlock)
		{
			PlayerController playerController;
			if (PhotonNetwork.connectedAndReady)
			{
				playerController = PlayerController.createAvatarPlayer(playerData.colorPath, Vector3.zero, Quaternion.identity);
			}
			else
			{
				playerController = PlayerController.CreateLocalPlayer(playerData.colorPath);
			}

			GameBoard.MovePlayerToBlockNode(playerController, startBlock);


			if (players.ContainsKey(index) == false)
			{
				Debug.Log("add player at index: " + index);
				players.Add(index, new PlayerInfo(playerController, playerData));
			}
		}
	}

	private void ClearPlayers()
	{
		foreach (var player in players.Values)
		{
			Debug.Log(player);
			var mesh = player.playerController.gameObject.GetComponentInChildren<MeshRenderer>();
			if (mesh != null)
			{
				Destroy(mesh.gameObject);
			}
			Destroy(player.playerController.gameObject);
		}
		players.Clear();

		Node.ClearActives();
		Node.ClearBounces();
		GameBoard.KillSpotLights();
	}

	public void OnCreatePlayer(int index)
	{
	}

	private int currentPlayerIndex = 0;

	public static PlayerController GetCurrentPlayerController()
	{
		if (mainGameController != null)
		{
			return mainGameController.players[mainGameController.currentPlayerIndex].playerController;
		}
		return null;
	}

	public void OnPlayerTurn(int index)
	{
		if (players.ContainsKey(index))
		{
			Debug.Log("StartTurn player: " + index);
			var player = players[index];

			currentPlayerIndex = index;

			questionsController.StartTurn(player);
		}
	}

	public void OnGameEnded(IGameClient game)
	{
		Debug.Log("Game Ended");

		ClearPlayers();

		ShowHud(false);
		//UINavigationController.PushBackground("/MainBackground");
		//UINavigationController.PushController("/GameOver");
	}

	public void OnMovePlayer(int index)
	{
	
	}

	#endregion
}
