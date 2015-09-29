using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    #region MemVars & Props
    
    static private List<PlayerController> playerControllers;

    public ColorPathType colorPath = ColorPathType.Blue;

    public Transform target;
    public float lobHeight = 0.5f;
    public float lobTime = 1f;
	public Node currentBlock;

    public PlayerType playerType = PlayerType.LocalPlayer;

    private string bluePlayer;
    private string greenPlayer;
    private string orangePlayer;
    private string redPlayer;
    private string yellowPlayer;
    private string purplePlayer;

    public GameObject redPillarSlot;
    public GameObject greenPillarSlot;
    public GameObject bluePillarSlot;
    public GameObject yellowPillarSlot;
    public GameObject purplePillarSlot;
    public GameObject orangePillarSlot;

    public List<GameData.PillarType> pillarsAcquired = new List<GameData.PillarType>();

    protected GameObject redPillar;
    protected GameObject greenPillar;
    protected GameObject bluePillar;
    protected GameObject yellowPillar;
    protected GameObject purplePillar;
    protected GameObject orangePillar;

    public GameObject meshObject;

    public float pillarDropHeight = 0.5f;
    public float pillarDropTime = 3f;
	private System.Action<PlayerController> OnMoved, OnPlayerMoved;
	private System.Action<PlayerController> OnPillarLanded;

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
        if (playerControllers == null)
        {
            playerControllers = new List<PlayerController>();
        }
    }

    protected void OnDestroy()
    {
       // GameObject.Destroy(gameObject);
        GameObject.Destroy(redPillar);
        GameObject.Destroy(greenPillar);
        GameObject.Destroy(bluePillar);
        GameObject.Destroy(yellowPillar);
        GameObject.Destroy(purplePillar);
        GameObject.Destroy(orangePillar);
    }

	protected void Start() 
    {
        redPillar = (GameObject)Instantiate(Resources.Load("Prefabs/Pillars/Pillar_Red"));
        redPillar.SetActive(false);

        greenPillar = (GameObject)Instantiate(Resources.Load("Prefabs/Pillars/Pillar_Green"));
        greenPillar.SetActive(false);

        bluePillar = (GameObject)Instantiate(Resources.Load("Prefabs/Pillars/Pillar_Blue"));
        bluePillar.SetActive(false);

        yellowPillar = (GameObject)Instantiate(Resources.Load("Prefabs/Pillars/Pillar_Yellow"));

        yellowPillar.SetActive(false);

        purplePillar = (GameObject)Instantiate(Resources.Load("Prefabs/Pillars/Pillar_Purple"));
        purplePillar.SetActive(false);

        orangePillar = (GameObject)Instantiate(Resources.Load("Prefabs/Pillars/Pillar_Orange"));
        orangePillar.SetActive(false);
	}

	protected void Update()
	{
		if (transform.parent != null)
		{
			currentBlock = transform.parent.GetComponent<Node>();
		}
	}

    #endregion
	
    #region Public Methods

	private void EnableRigidBody(bool state)
	{
		Rigidbody rigidBody = GetComponent<Rigidbody>();
		if (rigidBody != null)
		{
			rigidBody.isKinematic = !state;
		}
	}

    public void Initialize(ColorPathType pathType, PlayerType ptype)
    {
        this.colorPath = pathType;
        this.playerType = ptype;
    }

    public void MoveTo(GameObject target)
    {
        Node node = target.GetComponent<Node>();
        if (node == null)
        {
            Debug.LogWarning(string.Format("{0} is not a valid Node gameobject!", target.name));
            return;
        }

        MoveTo(node, "PlayerMoved");
    }

	public void MoveTo(GameObject target, System.Action<PlayerController> onPlayerMoved)
	{
		this.OnMoved = onPlayerMoved;

		Node node = target.GetComponent<Node>();
		if (node == null)
		{
			Debug.LogWarning(string.Format("{0} is not a valid Node gameobject!", target.name));
			return;
		}

		MoveTo(node, "PlayerMoved");
	}

	public void MoveToJumpToAnyPillar(GameObject target, System.Action<PlayerController> onPlayerMoved)
    {
		this.OnPlayerMoved = onPlayerMoved;

        Node node = target.GetComponent<Node>();
        if (node == null)
        {
            Debug.LogWarning(string.Format("{0} is not a valid Node gameobject", target.name));
            return;
        }

        MoveTo(node, "PlayerMovedToJumpToAnyPillar");
    }

    public void MoveTo(Node target, string onCompleteMethod)
    {
		EnableRigidBody(false);

        Vector3 targetPos = target.GetTargetTransform().position;

        int childCount = transform.GetChildCount();
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            
            iTween.MoveBy(child.gameObject, iTween.Hash("z", lobHeight, "time", lobTime * 0.5f, "easeType", iTween.EaseType.easeInOutQuad));
            iTween.MoveBy(child.gameObject, iTween.Hash("z", -lobHeight, "delay", lobTime * 0.5f, "time", lobTime * 0.5f, "easeType", iTween.EaseType.easeInCubic, "onComplete", onCompleteMethod, "oncompletetarget", gameObject));
        }

        iTween.MoveTo(gameObject, iTween.Hash("position", targetPos, "time", lobTime, "easeType", iTween.EaseType.linear));
        if (target != currentBlock)
        {
            currentBlock = target;
            transform.parent = target.transform;
        }
    }

    private void PlayerMoved()
    {
		if (OnMoved != null)
		{
			OnMoved(this);
			OnMoved = null;
		}
        /*if (currentBlock != null)
        {
            if (currentBlock.isPillar)
            {
                var pillar = GameData.GetColorTypeFromNodeType(currentBlock.nodeType);
                if (AddPillar(pillar))
                {
                    GameData.Load();

                    var slot = GameData.current.GetCurrentDataSlot();

                    GameData.Save();
                }
                else
                {
                    // If no pillar added, call next turn
                    GameBoard.NextTurn();
                }
            }
            else
            {
                GameBoard.NextTurn();
            }
        }*/

		Debug.Log("moved");
		EnableRigidBody(true);
    }
	
    private void PlayerMovedToJumpToAnyPillar()
    {
		if (this.OnPlayerMoved != null)
		{
			OnPlayerMoved(this);
			OnPlayerMoved = null;
		}

        /*if (currentBlock != null)
        {
            if (currentBlock.nodeType == NodeType.JumpToAnyPillar)
            {
                GameBoard.HighlightPillarBlocks(this);
                HUDController.DisplayMessage("Select the pillar to jump");
            }
        }*/

		EnableRigidBody(true);
    }

	public bool GetFreePillar(out GameData.PillarType pillar)
	{
		var slot = GameData.current.GetCurrentDataSlot();
		
		pillar = GameData.PillarType.None;
		
		foreach (var pillarNeeded in slot.pillarsNeeded)
		{
			if (pillarsAcquired.Contains(pillarNeeded) == false)
			{
				pillar = pillarNeeded;
			}
		}
		
		if (pillar != GameData.PillarType.None)
		{
			return true;
		}
		
		return false;
	}

	public bool AddFreePillar(out GameData.PillarType pillar, System.Action<PlayerController> onPillarLanded)
	{
		var slot = GameData.current.GetCurrentDataSlot();

		pillar = GameData.PillarType.None;

		foreach (var pillarNeeded in slot.pillarsNeeded)
		{
			if (pillarsAcquired.Contains(pillarNeeded) == false)
			{
				pillar = pillarNeeded;
			}
		}

		if (pillar != GameData.PillarType.None)
		{
			return AddPillar(pillar, onPillarLanded);
		}

		return false;
	}

	public bool AddPillar(GameData.PillarType pillarType, System.Action<PlayerController> onPillarLanded)
	{
		this.OnPillarLanded = onPillarLanded;
		var renderer = gameObject.GetComponentInChildren<MeshRenderer>();
		
		Vector3 offset = new Vector3(0f, pillarDropHeight, 0.0f);
		
		if (pillarsAcquired.Contains(pillarType) == false)
		{
			pillarsAcquired.Add(pillarType);
			
			GameObject targetObject = null;
			Vector3 targetPosition = Vector3.zero;
			
			switch (pillarType)
			{
			case GameData.PillarType.Red:
				redPillar.transform.parent = redPillarSlot.transform;
				redPillar.transform.localPosition = offset;
				redPillar.SetActive(true);
				targetObject = redPillar.gameObject;
				targetPosition = redPillarSlot.transform.position;
				break;
				
			case GameData.PillarType.Green:
				greenPillar.transform.parent = greenPillarSlot.transform;
				greenPillar.transform.localPosition = offset;//.position = greenPillarSlot.transform.position + offset;
				greenPillar.SetActive(true);
				targetObject = greenPillar.gameObject;
				targetPosition = greenPillarSlot.transform.position;
				break;
				
			case GameData.PillarType.Blue:
				bluePillar.transform.parent = bluePillarSlot.transform;
				bluePillar.transform.localPosition = offset;//.position = bluePillarSlot.transform.position + offset;
				bluePillar.SetActive(true);
				targetObject = bluePillar.gameObject;
				targetPosition = bluePillarSlot.transform.position;
				break;
				
			case GameData.PillarType.Yellow:
				yellowPillar.transform.parent = yellowPillarSlot.transform;
				yellowPillar.transform.localPosition = offset;//.position = yellowPillarSlot.transform.position + offset;
				yellowPillar.SetActive(true);
				targetObject = yellowPillar.gameObject;
				targetPosition = yellowPillarSlot.transform.position;
				break;
				
			case GameData.PillarType.Purple:
				purplePillar.transform.parent = purplePillarSlot.transform;
				purplePillar.transform.localPosition = offset;//.position = purplePillarSlot.transform.position + offset;
				purplePillar.SetActive(true);
				targetObject = purplePillar.gameObject;
				targetPosition = purplePillarSlot.transform.position;
				break;
				
			case GameData.PillarType.Orange:
				orangePillar.transform.parent = orangePillarSlot.transform;
				orangePillar.transform.localPosition = offset;//.position = orangePillarSlot.transform.position + offset;
				orangePillar.SetActive(true);
				targetObject = orangePillar.gameObject;
				targetPosition = orangePillarSlot.transform.position;
				break;
			}
			
			if (targetObject != null)
			{
				GameCamera.LookCamera(targetObject);//, 2f);
				iTween.MoveTo(targetObject, iTween.Hash("position", targetPosition, "time", pillarDropTime, "easeType", iTween.EaseType.easeOutBounce, "onComplete", "PillarHasLanded", "oncompletetarget", gameObject));
			}
			
			return true;
		}
		return false;
	}
	
	protected void PillarHasLanded()
	{
		if (OnPillarLanded != null)
		{
			OnPillarLanded(this);
			OnPillarLanded = null;
		}
		//GameCamera.ResetCameraPosition();
		//GameBoard.NextTurn();
	}

    #endregion


    #region Static Methods

    static public PlayerController GetCurrentPlayer()
    {
        if (currentTurn < 0)
        {
            currentTurn = 0;
        }
        
        return playerControllers[currentTurn];
    }

    private static int currentTurn = -1;
    static public PlayerController GetNextTurn()
    {
        currentTurn++;

        if (currentTurn > playerControllers.Count)
        {
            currentTurn = 0;
        }

        return playerControllers[currentTurn];
    }

	static public string GetPlayerPrefab(ColorPathType path)
	{
		string obj = "";
		
		switch (path)
		{
		case ColorPathType.Blue:
			obj = "Prefabs/Cylinders/BlueCylinder";
			break;
			
		case ColorPathType.Green:
			obj = "Prefabs/Cylinders/GreenCylinder";
			break;
			
		case ColorPathType.Orange:
			obj = "Prefabs/Cylinders/OrangeCylinder";
			break;
			
		case ColorPathType.Purple:
			obj = "Prefabs/Cylinders/PurpleCylinder";
			break;
			
		case ColorPathType.Red:
			obj = "Prefabs/Cylinders/RedCylinder";
			break;
			
		case ColorPathType.Yellow:
			obj = "Prefabs/Cylinders/YellowCylinder";;
			break;
		}
		return obj;
	}

    static public PlayerController InstantiatePlayer(ColorPathType path)
    {
		string obj = GetPlayerPrefab(path);

        GameObject go = (GameObject)Instantiate(Resources.Load(obj), Vector3.zero, Quaternion.identity);
        var player = go.GetComponentInChildren<PlayerController>();

        return player;
    }

    static public PlayerController CreateLocalPlayer(ColorPathType colorPath)
    {
        PlayerController player = InstantiatePlayer(colorPath);
        player.Initialize(colorPath, PlayerType.LocalPlayer);

        playerControllers.Add(player);

        return player;
    }

	static public PlayerController createAvatarPlayer(ColorPathType colorPath, Vector3 position, Quaternion rotation)
	{
		string prefabName = GetPlayerPrefab(colorPath);
		GameObject obj = PhotonNetwork.Instantiate(prefabName, position, rotation, 0);

		PlayerController player = obj.GetComponent<PlayerController>();
		player.Initialize(colorPath, PlayerType.LocalPlayer);

		return player;
	}

    static public PlayerController CreateBotPlayer(ColorPathType colorPath)
    {
        PlayerController player = InstantiatePlayer(colorPath);
        player.Initialize(colorPath, PlayerType.BotPlayer);

        playerControllers.Add(player);

        return player;
    }

    static public PlayerController CreateForeignPlayer(ColorPathType colorPath)
    {
        PlayerController player = InstantiatePlayer(colorPath);
        player.Initialize(colorPath, PlayerType.ForeignPlayer);

        playerControllers.Add(player);

        return player;
    }


    static public void Clear()
    {
        if (playerControllers == null)
        {
            return;
        }

		foreach (PlayerController player in playerControllers)
        {
			player.pillarsAcquired.Clear();
			GameObject.Destroy(player.redPillar);
			GameObject.Destroy(player.bluePillar);
			GameObject.Destroy(player.greenPillar);
			GameObject.Destroy(player.yellowPillar);
			GameObject.Destroy(player.orangePillar);
			GameObject.Destroy(player.purplePillar);
            GameObject.Destroy(player.gameObject);
        }

        playerControllers.Clear();
    }

    

    #endregion
}
