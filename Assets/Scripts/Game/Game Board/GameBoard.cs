using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    #region MemVars & Props

    private static GameBoard gameBoard;

    public Light mainLight;
    public Light[] spotLights;
 
    public Camera gameCamera;
    public Transform board;
    public GameObject root;
    public GameObject bluePath;
    public GameObject purplePath;
    public GameObject redPath;
    public GameObject yellowPath;
    public GameObject orangePath;
    public GameObject greenPath;
	public GameObject schoolBoxBlock;
	public GameObject winAPillarOrGoToSchoolBlock;

    public GameObject currentBlock;
	
	public float activeSpeed = 1f;
	public float bounceSpeed = 5f;
	public float bounceOffset = 0.05f;

	public List<Node> nodeBlocks;
	public event System.Action<Node> OnNodeSelected = null;
	
    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
        gameBoard = this;
    }

	protected void Start()
    {
        #region Null Checking 

        if (bluePath == null)
        {
            Debug.LogWarning("Blue Path is null");
        }

        if (purplePath == null)
        {
            Debug.LogWarning("Purple Path is null");
        }

        if (redPath == null)
        {
            Debug.LogWarning("Red Path is null");
        }

        if (yellowPath == null)
        {
            Debug.LogWarning("Yellow Path is null");
        }

        if (orangePath == null)
        {
            Debug.LogWarning("Orange Path is null");
        }

        if (greenPath == null)
        {
            Debug.LogWarning("Green Path is null");
        }

        #endregion
		
    }

    protected void Update() 
    {
		// Check for hit testing
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 pos = Input.mousePosition;
			Ray ray = GameCamera.GetCamera().ScreenPointToRay(pos);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider.gameObject.GetComponent<Node>() != null)
				{
					var node = hit.collider.gameObject.GetComponent<Node>();
					if (node.isActive)
					{
						if (node.IsBounce == false)
						{
							Node.ClearBounces();
							node.IsBounce = true;

							if (MainGameController.GetCurrentPlayerController() != null)
							{
								GameCamera.LookCamera(MainGameController.GetCurrentPlayerController().gameObject);
							}
							HUDController.DisplayQuestionTypeHint(node.nodeType);
						}
						else
						{
							GameCamera.ResetCameraPosition();
							
							node.IsBounce = false;
							
							Node.ClearActives();
							gameObject.transform.position = node.originalPosition;
							
							SelectBlock(node);
	                        HUDController.HideQuestionTypeHint();

							if (OnNodeSelected != null)
							{
								OnNodeSelected(node);
							}
	                    }
					}
                }
            }
        }
    }

    protected void OnEnable()
    {
    }

    protected void OnDisable()
    {
    }

    #endregion


    #region Public Methods

    private static Node activeNode = null;
    private static bool isLastQuestionCorrect = false;

    static public void StartGame()
    {
        if (gameBoard == null)
        {
            return;
        }

        GameData.Load();

        var dataSlot = GameData.current.GetCurrentDataSlot();

        switch (dataSlot.gameType)
        {
            case GameData.GameType.SinglePlayerClassic:
            case GameData.GameType.SinglePlayerClassicWithAi:
                gameBoard.PrepareSinglePlayerClassic();
                break;

            case GameData.GameType.SinglePlayerPursuit:
            case GameData.GameType.SinglePlayerPursuitWithAi:
                gameBoard.PrepareSinglePlayerPursuit();
                break;
        }
    }

    static public void EndGame()
    {
		Node.ClearActives();
		Node.ClearBounces();
		KillSpotLights();

        PlayerController.Clear();
    }

    /// <summary>
    /// Kill all spot lights
    /// </summary>
    public static void KillSpotLights()
    {
        foreach (Light light in gameBoard.spotLights)
        {
            light.gameObject.SetActive(false);
        }
        gameBoard.mainLight.gameObject.SetActive(true);
    }

    /// <summary>
    /// Player click a node to highlight it so he knows that he will answer that node's question
    /// </summary>
    /// <param name="stepsEarned"></param>
    public static void HighlightBlock(PlayerController player, int stepsEarned)
    {
		if (gameBoard != null)
		{
        	gameBoard.PlayerHightlightBlock(player, stepsEarned);
		}
    }

    public static void HighlightPillarBlocks(PlayerController player)
    {
        if (gameBoard != null)
        {
            gameBoard.PlayerHighlightPillarBlocks(player);
        }
    }

    /// <summary>
    /// Event where player click a node block to show question
    /// </summary>
    /// <param name="node"></param>
    public static void SelectBlock(Node node)
    {
        activeNode = node;

        KillSpotLights();

        switch (node.nodeType)
        {
            case NodeType.JumpToAnyPillar:
                //QAController.ShowCategory(node);
                break;

            case NodeType.Root:
            case NodeType.ChooseCategory:
                //QAController.ShowCategory(node);
                break;

            case NodeType.SchoolBox:
                break;

            case NodeType.AnswerEasyToWinTwoJump:
            case NodeType.WinAPillarOrGoToSchool:
            case NodeType.Blue:
            case NodeType.Green:
            case NodeType.Orange:
            case NodeType.Purple:
            case NodeType.Red:
            case NodeType.Yellow:
                //QAController.ShowQuestion(node);        
                break;
        }

    }

    /// <summary>
    /// Player answered a question and ready to end their turn
    /// </summary>
    /// <param name="isCorrect"></param>
    public static void EndTurn(bool isCorrect)
    {
        if (activeNode != null)
        {
            if (isCorrect)
            {
                //GameBoard.MovePlayer(activeNode.gameObject);
            }
            else
            {
                // Next Turn for next player please
                GameBoard.NextTurn();
            }
        }

        isLastQuestionCorrect = isCorrect;
        activeNode = null;
    }

    /// <summary>
    /// Next Turn of other player please
    /// </summary>
    public static void NextTurn()
    {
        if (gameBoard != null)
        {
            // if we are in jump to any pillar, we should never call next turn, because the player has free access to any pillar
            if (PlayerController.GetCurrentPlayer().currentBlock != null)
            {
                if (PlayerController.GetCurrentPlayer().currentBlock.nodeType == NodeType.JumpToAnyPillar)
                {
                    //GameBoard.HighlightPillarBlocks();
                    HUDController.DisplayMessage("Select the pillar to jump");
                    return;
                }
            }

            //QAController.NextTurn();
        }
    }


    #region Node Related 

    public static float GetBounceSpeed()
	{
		return gameBoard.bounceSpeed;
	}
	
	public static float GetBounceOffset()
	{
		return gameBoard.bounceOffset;
	}
	
	public static Node GetRootBlock()
	{
        if (gameBoard == null)
        {
            return null;
        }

		if (gameBoard.root == null)
		{
            Debug.LogError("Root is null");
			return null;
		}
		
		return gameBoard.root.GetComponent<Node>();
	}

    public static Node GetSchoolBlock()
    {
        if (gameBoard == null)
        {
            return null;
        }

        GameObject board = gameBoard.gameObject;

        Node found = null;
        Node[] nodes = board.GetComponentsInChildren<Node>(true);
        foreach (Node node in nodes)
        {
            if (node.nodeType == NodeType.SchoolBox)
            {
                found = node;
            }
        }

        return found;
    }

    public static Node[] GetPillarBlocks()
    {
        if (gameBoard == null)
        {
            return null;
        }

        GameObject board = gameBoard.gameObject;

        List<Node> founds = new List<Node>();
        Node[] nodes = board.GetComponentsInChildren<Node>(true);
        foreach (Node node in nodes)
        {
            if (node.isPillar)
            {
                founds.Add(node);
            }
        }

        return founds.ToArray();
    }

    /// <summary>
    /// Find reference to the Node Block by its name
    /// </summary>
    /// <param name="blockName">Block Name</param>
    /// <returns>Node of the Block</returns>
    public static Node FindBlockByName(string blockName)
    {
        if (gameBoard == null)
        {
            return null;
        }

        GameObject board = gameBoard.gameObject;

        Node found = null;
        Node[] nodes = board.GetComponentsInChildren<Node>(true);
        foreach (Node n in nodes)
        {
            if (n.name.ToLower() == blockName.ToLower())
            {
                found = n;
                break;
            }
        }

        return found;
    }

    public static void MovePlayer(PlayerController player, GameObject block, System.Action<PlayerController> OnMoved)
    {
        Node node = block.GetComponent<Node>();
        switch (node.nodeType)
        {
            case NodeType.JumpToAnyPillar:
                player.MoveToJumpToAnyPillar(block, OnMoved);
                break;

            default:
                player.MoveTo(block, OnMoved);
                break;
        }
    }

    public static List<GameObject> FindTargetBlocks(ColorPathType path, int step)
    {
        GameObject obj = gameBoard.GetColorPathObject(path);

        //Debug.LogWarning("begin: " + obj.name);
        return gameBoard.FindMoveTargets(obj, step - 1, true);
    }

    public static List<GameObject> FindTargetBlocks(GameObject start, int step, bool isFirstMove)
    {
        return gameBoard.FindMoveTargets(start, step, isFirstMove);
    }

    public GameObject GetColorPathObject(ColorPathType path)
    {
        GameObject obj = bluePath;

        switch (path)
        {
            case ColorPathType.Blue:
                obj = bluePath;
                break;

            case ColorPathType.Green:
                obj = greenPath;
                break;

            case ColorPathType.Orange:
                obj = orangePath;
                break;

            case ColorPathType.Purple:
                obj = purplePath;
                break;

            case ColorPathType.Red:
                obj = redPath;
                break;

            case ColorPathType.Yellow:
                obj = yellowPath;
                break;
        }

        return obj;
    }

    public List<GameObject> FindMoveTargets(GameObject start, int step, bool isFirstMove)
    {
        List<GameObject> targets = new List<GameObject>();
        List<GameObject> visitedObjs = new List<GameObject>();
        visitedObjs.Add(start);

        if (step == 0)
        {
            targets.Add(start);
            Debug.Log("steps = 0");
        }
        else
        {
            if (isFirstMove)
            {
                Node node = start.GetComponent<Node>();
                if (node.nodeType != NodeType.Root)
                {
                    Traverse(start, step, ref visitedObjs, ref targets, isFirstMove);
                }
                else
                {
                    Debug.Log("found root");
                }
                //Debug.Log("Move to Node Type: " + node.nodeType);
            }
            else
            {
                //Debug.Log("not first move");
				Node node = start.GetComponent<Node>();
				if (node.nodeType == NodeType.SchoolBox)
				{
					targets.Add(winAPillarOrGoToSchoolBlock);
				}
				else
				{
                	Traverse(start, step, ref visitedObjs, ref targets, isFirstMove);
				}
            }
        }

        return targets;
    }

    private void Traverse(GameObject start, int step, ref List<GameObject> visitedObjs, ref List<GameObject> targets, bool isFirstMove)
    {
        Node node = start.GetComponent<Node>();

        for (int i = 0; i < node.link.Length; i++)
        {
            GameObject go = node.link[i];

            if (step == 1)
            {
                if (visitedObjs.Contains(go) == false)
                {
                    targets.Add(go);
                    //Debug.Log("=>" + go.name);
                }
            }
            else
            {
                if (visitedObjs.Contains(go) == false)
                {
                    visitedObjs.Add(go);

                    if (isFirstMove)
                    {
                        if (go.GetComponent<Node>().nodeType != NodeType.Root)
                        {
                            Traverse(go, step - 1, ref visitedObjs, ref targets, isFirstMove);
                        }
                    }
                    else
                    {
                        Traverse(go, step - 1, ref visitedObjs, ref targets, isFirstMove);
                    }
                    //Debug.Log("step: " + (step - 1).ToString() + ", go: " + go.name);
                }
            }
        }
    }

    protected Light GetSpotLight()
    {
        foreach (Light light in spotLights)
        {
            if (light.gameObject.activeSelf == false)
            {
                light.gameObject.SetActive(true);
                return light;
            }
        }

        return null;
    }

    #endregion

    #endregion


    #region Internal Methods

    public void PlayerHightlightBlock(PlayerController player, int stepsEarned)
    {
        if (mainLight != null)
        {
            mainLight.gameObject.SetActive(false);
        }

        //ShowBoard(true);

        List<GameObject> nodes;
        if (player.currentBlock == null)
        {
            nodes = FindTargetBlocks(player.colorPath, stepsEarned);
        }
        else
        {
            nodes = FindTargetBlocks(player.currentBlock.gameObject, stepsEarned, false);
        }

        Node.ClearActives();

        foreach (GameObject nodeObj in nodes)
        {
            Node node = nodeObj.GetComponent<Node>();
            node.isActive = true;
            Vector3 pos = node.GetTargetTransform().position;

            Light light = GetSpotLight();
            light.transform.position = new Vector3(pos.x, light.transform.position.y, pos.z);
        }
    }

    public void PlayerHighlightPillarBlocks(PlayerController player)
    {
        if (mainLight != null)
        {
            mainLight.gameObject.SetActive(false);
        }

        Node[] pillars = GetPillarBlocks();

        foreach (Node node in pillars)
        {
            node.isActive = true;
            Vector3 pos = node.GetTargetTransform().position;

            Light light = GetSpotLight();
            light.transform.position = new Vector3(pos.x, light.transform.position.y, pos.z);
        }
    }

	public static void MovePlayerToBlockNode(PlayerController player, Node node)
	{
		if (player && node)
		{
			player.gameObject.transform.parent = node.GetTargetTransform();
			player.gameObject.transform.localPosition = Vector3.zero;//node.GetTargetTransform().localPosition;

			player.gameObject.transform.localRotation = Quaternion.identity;
			player.gameObject.SetActive(true);
		}
	}

    private void PrepareSinglePlayerClassic()
    {
        var dataSlot = GameData.current.GetCurrentDataSlot();
        var playerData = dataSlot.GetCurrentPlayerData();

        Node currentBlock = FindBlockByName(playerData.currentBlockName);

        if (currentBlock != null)
        {
            PlayerController player = PlayerController.CreateLocalPlayer(playerData.colorPath);

            player.gameObject.transform.position = currentBlock.GetTargetTransform().position;
            player.gameObject.transform.parent = currentBlock.transform;
            player.gameObject.transform.localRotation = Quaternion.identity;
            player.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Can't find block node by the name of " + playerData.currentBlockName);
        }
    }

    private void PrepareSinglePlayerPursuit()
    {

    }

    #endregion
}
