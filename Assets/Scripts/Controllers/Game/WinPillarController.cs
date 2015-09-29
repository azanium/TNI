using UnityEngine;
using System.Collections;

public class WinPillarController : UIViewController 
{
	#region MemVars & Props

	public GameObject redPillar;
	public GameObject greenPillar;
	public GameObject orangePillar;
	public GameObject yellowPillar;
	public GameObject purplePillar;
	public GameObject bluePillar;
	public GameObject target;
	public string func;

	#endregion


	#region Mono Methods

	public override void OnAppeared()
	{
		base.OnAppeared();

		StartCoroutine(timer());
	}

	IEnumerator timer()
	{
		yield return new WaitForSeconds(3);

		if (target != null)
		{
			target.SendMessage(func, SendMessageOptions.DontRequireReceiver);
		}

		UINavigationController.DismissAllControllers();
	}

	#endregion


	#region Public Methods

	public void PillarWon(GameData.PillarType pillar)
	{
		Debug.Log("Won A Pillar: " + pillar.ToString());

		redPillar.SetActive(false);
		bluePillar.SetActive(false);
		greenPillar.SetActive(false);
		yellowPillar.SetActive(false);
		purplePillar.SetActive(false);
		orangePillar.SetActive(false);

		switch (pillar)
		{
		case GameData.PillarType.Blue:
			bluePillar.SetActive(true);
			break;
			
		case GameData.PillarType.Red:
			redPillar.SetActive(true);
			break;
			
		case GameData.PillarType.Green:
			greenPillar.SetActive(true);
			break;
			
		case GameData.PillarType.Orange:
			orangePillar.SetActive(true);
			break;
			
		case GameData.PillarType.Purple:
			purplePillar.SetActive(true);
			break;
			
		case GameData.PillarType.Yellow:
			yellowPillar.SetActive(true);
			Debug.Log("yellow");
			break;
		}

	}


	#endregion

}
