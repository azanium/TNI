using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameOverController : UIViewController 
{
	#region MemVars & Props
	
	public UITable table;
	
	public GameObject redPillar;
	public GameObject greenPillar;
	public GameObject orangePillar;
	public GameObject yellowPillar;
	public GameObject purplePillar;
	public GameObject bluePillar;
	
	#endregion
	
	
	#region UIViewController's Methods
	
	public override void OnAppear()
	{
		base.OnAppear();
		
		GameData.Load();
		
		var slot = GameData.current.GetCurrentDataSlot();
		var player = slot.GetCurrentPlayerData();

		DisplayPillars(player.pillarsAcquired);
	}

	#endregion
	
	
	#region Public Methods
	

	public void DisplayPillars(List<GameData.PillarType> pillars)
	{
		redPillar.SetActive(false);
		bluePillar.SetActive(false);
		greenPillar.SetActive(false);
		yellowPillar.SetActive(false);
		purplePillar.SetActive(false);
		orangePillar.SetActive(false);

		foreach (var pillar in pillars)
		{
			GameObject pillarObj = null;
			switch (pillar)
			{
			case GameData.PillarType.Blue:
				pillarObj = bluePillar;
				break;
				
			case GameData.PillarType.Red:
				pillarObj = redPillar;
				break;
				
			case GameData.PillarType.Green:
				pillarObj = greenPillar;
				break;
				
			case GameData.PillarType.Orange:
				pillarObj = orangePillar;
				break;
				
			case GameData.PillarType.Purple:
				pillarObj = purplePillar;
				break;
				
			case GameData.PillarType.Yellow:
				pillarObj = yellowPillar;
				break;
			}
			Debug.Log("Pillar Obj: " + pillarObj);
			pillarObj.SetActive(true);
		}
	}
	
	#endregion
}
