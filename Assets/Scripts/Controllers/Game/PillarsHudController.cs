using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PillarsHudController : MonoBehaviour
{
    #region MemVars & Props

    public List<GameObject> pillars;

    private bool pillarsValid = true;
    public bool IsPillarsValid
    {
        get { return pillarsValid; }
    }

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
    }
	
    #endregion


    #region Public Methods

    public void SetupPillars(IEnumerable<GameData.PillarType> pillarTypes)
    {
        int i = 0;
		foreach (var p in pillars)
		{
			p.SetActive(false);
		}

        foreach (var pType in pillarTypes)
        {
            if (i < pillars.Count)
            {
                var currentPillar = pillars[i];

                var pillarSprite = currentPillar.GetComponent<UISlicedSprite>();
                pillarSprite.spriteName = "Pillar_" + pType.ToString();
				currentPillar.SetActive(true);
            }
            i++;
        }
    }
	
	

    #endregion


    #region Private Methods

    #endregion
}
