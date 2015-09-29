using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerIconController : MonoBehaviour 
{
	public GameObject offset;
	public PillarsHudController pillarsHudController;

	public void ShowPillars(IEnumerable<GameData.PillarType> pillars)
	{
		if (pillarsHudController != null)
		{
			pillarsHudController.SetupPillars(pillars);
		}
	}
}
