using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour 
{
	#region MemVars & Props
	
	public float motionSpeed = 5f;
	
	private static GameCamera gameCamera;
	private Vector3 originalPosition;
	private Vector3 desiredPosition;
	private Quaternion desiredRotation;

    private GameObject targetToFollow = null;
		
	#endregion
	
	
	#region MonoBehavior Methods
	
	private void Awake()
	{
		gameCamera = this;
	}
	
	private void OnDestroy()
	{
	}
	
	private void Start() 
	{
		originalPosition = desiredPosition = transform.position;
	}
	
	private void Update() 
	{
        if (targetToFollow != null)
        {
            if (CalcDistance(GetCamera().transform.position, targetToFollow.transform.position) >= 0.01f)
            {
                GetCamera().transform.position = Vector3.Lerp(GetCamera().transform.position, targetToFollow.transform.position, Time.deltaTime * motionSpeed);
            }
        }
        else
        {
            if (CalcDistance(GetCamera().transform.position, desiredPosition) >= 0.01f)
            {
                GetCamera().transform.position = Vector3.Lerp(GetCamera().transform.position, desiredPosition, Time.deltaTime * motionSpeed);
            }
        }
	}
	
	private float CalcDistance(Vector3 fromVec, Vector3 toVec)
	{
		return Mathf.Abs(Vector3.Distance(fromVec, toVec));
	}
	
	
	#endregion
	
	
	#region Public Methods
	
	public static Camera GetCamera()
	{
		if (gameCamera != null)
		{
			return gameCamera.GetComponent<Camera>();
		}
		
		return Camera.mainCamera;
	}
	
	public static void LookCamera(Node node)
	{
		Transform transform = node.GetTargetTransform();
		Vector3 targetPos = transform.position + Vector3.up * 0.5f;
		Vector3 direction = targetPos - gameCamera.originalPosition;
		direction.Normalize();
		targetPos = targetPos - direction * 2f;
		
		gameCamera.desiredPosition = targetPos;
	}

    public static void LookCamera(GameObject obj)
    {
        Vector3 targetPos = obj.transform.position + Vector3.up * 0.5f;
        Vector3 direction = targetPos - gameCamera.originalPosition;
        direction.Normalize();
        targetPos = targetPos - direction * 0.8f;

        gameCamera.desiredPosition = targetPos;
    }

    public static void FollowCamera(GameObject obj, float distance)
    {
        Vector3 targetPos = obj.transform.position + Vector3.up * 0.5f;
        Vector3 direction = targetPos - gameCamera.originalPosition;//new Vector3(gameCamera.originalPosition.x, targetPos.y, gameCamera.originalPosition.y);
        direction.Normalize();
        targetPos = targetPos - direction * 0.8f;

        //gameCamera.desiredPosition = targetPos;
        gameCamera.targetToFollow = obj;
        GetCamera().transform.position = targetPos;
    }

	public static void ResetCameraPosition()
	{
		gameCamera.desiredPosition = gameCamera.originalPosition;
        gameCamera.targetToFollow = null;
	}
	
	#endregion
}
