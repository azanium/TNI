using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    #region MemVars & Props

    static private List<Node> nodes;

	/// <summary>
	/// Auto Initialize
	/// </summary>
	public GameBoard board;

    public NodeType nodeType;
    public GameObject[] link;
    public bool isActive = false;
    public bool isPillar = false;

    public Vector3 originalPosition;
    private Vector3 desiredPosition;
    private bool bounceDown = false;
	private bool isBounce = false;
	public bool IsBounce
	{
		get { return isBounce; }
		set 
		{ 
			if (isBounce != value)
			{
				isBounce = value;
			}
		}
	}

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
        // Prepare the pool
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        if (nodes.Contains(this) == false)
        {
            nodes.Add(this);
        }

		board = NGUITools.FindInParents<GameBoard>(gameObject);
    }

	protected void Start() 
    {
        originalPosition = transform.position;
        desiredPosition = new Vector3(originalPosition.x, originalPosition.y + GameBoard.GetBounceOffset(), originalPosition.z);
	}
	
	protected void Update() 
    {
        if (isActive)
        {
            if (Mathf.Abs(Vector3.Distance(transform.position, desiredPosition)) >= 0.01f)
            {
                if (!bounceDown)
                {
                    transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * GameBoard.GetBounceSpeed());
                }
            }
			else
			{
				bounceDown = isBounce;
			}
            			
            if (Mathf.Abs(Vector3.Distance(transform.position, originalPosition)) >= 0.01f)
            {
                if (bounceDown)
                {
                    transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * GameBoard.GetBounceSpeed());
                }
            }
            else
            {
                bounceDown = false;
            }
			
			
            
        }
        else
        {
            // Put back the block if user turned off the bounce flag
            if (Mathf.Abs(Vector3.Distance(transform.position, originalPosition)) >= 0.001f)
            {
                transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * GameBoard.GetBounceSpeed());
            }
        }
    }


    #endregion


    #region Public Methods

    public string GetNodeTypeString()
    {
        string result = "";
        switch (nodeType)
        {
            case NodeType.Blue:
                result = "blue";
                break;

            case NodeType.Green:
                result = "green";
                break;

            case NodeType.Orange:
                result = "orange";
                break;

            case NodeType.Purple:
                result = "purple";
                break;

            case NodeType.Red:
                result = "red";
                break;

            case NodeType.Root:
                result = "green";
                break;

            case NodeType.Yellow:
                result = "yellow";
                break;
        }

        return result;
    }

    static public void ClearActives()
    {
        foreach (Node node in nodes)
        {
            node.isActive = false;
        }
    }

    static public void ClearBounces()
    {
        foreach (Node node in nodes)
        {
            node.isBounce = false;
        }
    }

    public Transform GetTargetTransform()
    {
        Transform t = transform.FindChild("target");

        return t;
    }

	public void EnableNodes(bool state)
	{
		nodes.ForEach((node) => {
			node.GetComponent<Collider>().enabled = state;
		});
	}

    #endregion


    #region Private Methods

    #endregion
}
