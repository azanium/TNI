using UnityEngine;
using System.Collections;

public class CustomizationController : UIViewController
{
    #region MemVars & Props

    public UIButtonMultiSelection orangePillar;
    public UIButtonMultiSelection bluePillar;
    public UIButtonMultiSelection redPillar;
    public UIButtonMultiSelection purplePillar;
    public UIButtonMultiSelection yellowPillar;
    public UIButtonMultiSelection greenPillar;

    public UIButtonSelection time10;
    public UIButtonSelection time15;
    public UIButtonSelection time20;
    public UIButtonSelection time25;

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
    }

	protected void Start() 
    {
        if (orangePillar == null || bluePillar == null || redPillar == null ||
            purplePillar == null || yellowPillar == null || greenPillar == null)
        {
            Debug.LogWarning("Pillar not set correctly!");
        }

        if (time10 == null || time15 == null || time20 == null || time25 == null)
        {
            Debug.LogWarning("Time button not set correctly!");
        }
	}
	
    protected void OnEnable()
    {
        Initialize();
    }

    protected void OnDisable()
    {
        DeInitialize();
    }


    #endregion


    #region Public Methods

    #endregion


    #region Private Methods

    private void Initialize()
    {
        GameData.Load();

        bluePillar.selected = false;
        redPillar.selected = false;
        yellowPillar.selected = false;
        purplePillar.selected = false;
        greenPillar.selected = false;
        orangePillar.selected = false;

        var slot = GameData.current.GetCurrentDataSlot();
//		Debug.LogWarning(slot.pillarsNeeded.Count);
        
        foreach (GameData.PillarType pillar in slot.pillarsNeeded)
        {
//			Debug.Log(pillar);
            switch (pillar)
            {
                case GameData.PillarType.Blue:
                    bluePillar.SetMultiSelected();
                    break;

                case GameData.PillarType.Green:
                    greenPillar.SetMultiSelected();
                    break;

                case GameData.PillarType.Orange:
                    orangePillar.SetMultiSelected();
                    break;

                case GameData.PillarType.Purple:
                    purplePillar.SetMultiSelected();
                    break;

                case GameData.PillarType.Red:
                    redPillar.SetMultiSelected();
                    break;

                case GameData.PillarType.Yellow:
                    yellowPillar.SetMultiSelected();
                    break;
            }
        }

        if (slot.timeLimit == 10)
        {
            time10.SetSelected();
        }

        if (slot.timeLimit == 15)
        {
            time15.SetSelected();
        }

        if (slot.timeLimit == 20)
        {
            time20.SetSelected();
        }

        if (slot.timeLimit == 25)
        {
            time25.SetSelected();
        }
    }

    private void DeInitialize()
    {

    }

    protected void OnTimeSet()
    {
        var slot = GameData.current.GetCurrentDataSlot();
        var selectedTime = UIHelper.GetSelectedButton(gameObject);
        slot.timeLimit = float.Parse(UIHelper.GetButtonLabel(selectedTime).text);
        Debug.LogWarning("Time Limit set to " + slot.timeLimit);
        GameData.Save();
    }

    protected void OnPillarSet()
    {
        var slot = GameData.current.GetCurrentDataSlot();
        UIButtonMultiSelection[] pillars = UIHelper.GetSelectedButtonMultiSelection(gameObject);
		//Debug.LogWarning(pillars.Length);
        
        slot.pillarsNeeded.Clear();

        foreach (var pillar in pillars)
        {
            var pillarSprite = pillar.transform.FindChild("Background").GetComponent<UISlicedSprite>();
            
            var spriteName = pillarSprite.spriteName.ToLower();

            if (spriteName.Contains("blue"))
            {
                slot.pillarsNeeded.Add(GameData.PillarType.Blue);
            }
            if (spriteName.Contains("green"))
            {
                slot.pillarsNeeded.Add(GameData.PillarType.Green);
            }
            if (spriteName.Contains("yellow"))
            {
                slot.pillarsNeeded.Add(GameData.PillarType.Yellow);
            }
            if (spriteName.Contains("orange"))
            {
                slot.pillarsNeeded.Add(GameData.PillarType.Orange);
            }
            if (spriteName.Contains("purple"))
            {
                slot.pillarsNeeded.Add(GameData.PillarType.Purple);
            }
            if (spriteName.Contains("red") || spriteName.Contains("pink"))
            {
                slot.pillarsNeeded.Add(GameData.PillarType.Red);
            }
        }

        GameData.Save();
    }

    #endregion
}
