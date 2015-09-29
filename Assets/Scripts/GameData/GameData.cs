using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData 
{
    static public GameData current = null;

    public const string QTRandom = "";
    public const string QTChooseCategory = "Choose Category";
    public const string QTQuran = "Quran";
    public const string QTHadith = "Hadith";
    public const string QTTheProphets = "The Prophets";
    public const string QTIslamicHistory = "Islamic History";
    public const string QTIslamicScience = "Islamic Science";
    public const string QTGeneralKnowledge = "General Knowledge";
    public const string QTGoToSchool = "Win A Pillar or Go to School";
    public const string QTJumpToAnyPillar = "Jump to any pillar";
    public const string QTWinTwoJump = "Answer easy question & win 2 jumps";

	public enum QuestionType
	{
        Random,
		Quran,
		Hadith,
		TheProphets,
		IslamicHistory,
		IslamicScience,
		GeneralKnowledge
	}
	
    public enum PillarType
    {
		None,
        Blue,
        Green,
        Red, 
        Yellow,
        Purple,
        Orange,
        Gray,
    }

    /// <summary>
    /// Game Type
    /// </summary>
    public enum GameType
    {
        SinglePlayerClassic,
        SinglePlayerClassicWithAi,
        SinglePlayerPursuit,
        SinglePlayerPursuitWithAi,
        MultiPlayerClassic,
        MultiPlayerPursuit
    }

    /// <summary>
    /// AI Difficulty Type
    /// </summary>
    public enum DifficultyType
    {
        None,
        Easy,
        Medium,
        Hard
    }

    /// <summary>
    /// Player Data
    /// </summary>
    public class PlayerData
    {
        /// <summary>
        /// The Index to the PlayerProfile
        /// </summary>
        public int playerProfileIndex = 0;

        /// <summary>
        /// The player last position
        /// </summary>
        public string currentBlockName = "_Root";

        /// <summary>
        /// The Color path that this player choose
        /// </summary>
        public ColorPathType colorPath = ColorPathType.Blue;

		/// <summary>
		/// The pillars acquired.
		/// </summary>
		public List<PillarType> pillarsAcquired = new List<PillarType>();

        public PlayerProfile.Profile GetPlayerProfile()
        {
            return PlayerProfile.current.profiles[playerProfileIndex];
        }
    }

    public class DataSlot
    {
        public int currentPlayerIndex = 0;

        /// <summary>
        /// Player list that play this game
        /// </summary>
        public List<PlayerData> players = new List<PlayerData>();

        /// <summary>
        /// The game type that being played
        /// </summary>
        public GameType gameType = GameType.SinglePlayerClassic;

        /// <summary>
        /// The Pillars needed
        /// </summary>
        public List<PillarType> pillarsNeeded = new List<PillarType>();

        /// <summary>
        /// Time Limit needed
        /// </summary>
        public float timeLimit = 15f;

        /// <summary>
        /// AI difficulti
        /// </summary>
        public DifficultyType aiLevel = DifficultyType.None;

        /// <summary>
        /// Is this game data used or not
        /// </summary>
        public bool isEmpty = true;

        public PlayerData GetCurrentPlayerData()
        {
            return players[currentPlayerIndex];
        }

        public PlayerProfile.Profile GetPlayerProfile(PlayerData playerData)
        {
            PlayerProfile.Load();

            return PlayerProfile.current.profiles[playerData.playerProfileIndex];
        }

        public void ClearPlayer()
        {
            players.Clear();

            currentPlayerIndex = 0;
        }

        public PlayerData AddPlayer(int playerProfileIndex)
        {
            PlayerData player = new PlayerData();
            player.playerProfileIndex = playerProfileIndex;

            players.Add(player);

            currentPlayerIndex = players.IndexOf(player);

            return player;
        }

    }

    /// <summary>
    /// Data Slots maximum is 4
    /// </summary>
    public DataSlot[] dataSlots = new DataSlot[] 
    {
        new DataSlot(),
        new DataSlot(),
        new DataSlot(),
        new DataSlot()
    };

    public int currentSlot = 0;

    public DataSlot NewGame()
    {
        currentSlot = 0;
        var slot = CreateSlot();

        dataSlots[currentSlot] = slot;
        
        return slot;
    }
	
	public DataSlot NewGame(int slotIndex)
	{
        if (slotIndex < dataSlots.Length)
        {
            var slot = CreateSlot();

            dataSlots[slotIndex] = slot;

            return slot;
        }
		
		return NewGame();
	}

    private DataSlot CreateSlot()
    {
        var slot = new DataSlot();
        slot.isEmpty = false;
        slot.AddPlayer(0);

        slot.pillarsNeeded = new List<PillarType>()
        {
            GameData.PillarType.Blue,
            GameData.PillarType.Red,
            GameData.PillarType.Green,
            GameData.PillarType.Yellow,
            GameData.PillarType.Orange,
            GameData.PillarType.Purple
        };

        return slot;
    }
	

    public void Delete(int index)
    {
        if (index >= 0 && index < 4)
        {
            var slot = new DataSlot();
            slot.isEmpty = true;
            dataSlots[index] = slot;

            Save();
        }
    }

    public DataSlot GetCurrentDataSlot()
    {
        if (currentSlot >= 0 && currentSlot < dataSlots.Length)
        {
            return dataSlots[currentSlot];
        }

        return null;
    }

    public void SetCurrentDataSlot(DataSlot slot)
    {
        if (currentSlot >= 0 && currentSlot < dataSlots.Length)
        {
            dataSlots[currentSlot] = slot;

            Save();
        }
    }

    static public void Load()
    {
        string xml = PlayerPrefs.GetString("GameDataSlot");

        if (string.IsNullOrEmpty(xml) == false)
        {
            current = (GameData)XmlManager.DeserializeObject(typeof(GameData), xml);
        }

        if (current == null)
        {
            current = new GameData();
        }
    }

    static public void Save()
    {
        if (current == null)
        {
            current = new GameData();
        }

        string xml = XmlManager.SerializeObject(typeof(GameData), current);
        //Debug.Log("GameData saved");
        Debug.Log("GameDataSlot: " + xml);
        PlayerPrefs.SetString("GameDataSlot", xml);
        PlayerPrefs.Save();
    }

    public static Color GetHintColorFromNodeType(NodeType nodeType, out string questionType)
    {
        Color color = Color.gray;
        questionType = QTRandom;

        switch (nodeType)
        {
            case NodeType.Root:
            case NodeType.ChooseCategory:
                questionType = QTChooseCategory;
                break;

            case NodeType.WinAPillarOrGoToSchool:
                questionType = QTGoToSchool;
                break;

            case NodeType.JumpToAnyPillar:
                questionType = QTJumpToAnyPillar;
                break;

            case NodeType.SchoolBox:
                break;

            case NodeType.AnswerEasyToWinTwoJump:
                questionType = QTWinTwoJump;
                break;

            case NodeType.Blue:
                color = Color.blue;
                questionType = QTQuran;
                break;

            case NodeType.Green:
                color = Color.green;
                questionType = QTHadith;
                break;

            case NodeType.Red:
                color = Color.red;
                questionType = QTGeneralKnowledge;
                break;

            case NodeType.Orange:
                color = new UnityEngine.Color(1.0f, 0.4f, 0.0f, 1.0f);
                questionType = QTIslamicScience;
                break;

            case NodeType.Purple:
                color = new UnityEngine.Color(160f / 255f, 32f / 255f, 240f / 255f);
                questionType = QTTheProphets;
                break;

            case NodeType.Yellow:
                color = Color.yellow;
                questionType = QTIslamicHistory;
                break;
        }

        return color;
    }

    public static string GetQuestionTypeFromPillarType(PillarType pillarType)
    {
        string questionType = QTRandom;

        switch (pillarType)
        {
            case PillarType.Blue:
                questionType = QTQuran;
                break;

            case PillarType.Green:
                questionType = QTHadith;
                break;

            case PillarType.Red:
                questionType = QTGeneralKnowledge;
                break;

            case PillarType.Orange:
                questionType = QTIslamicScience;
                break;

            case PillarType.Purple:
                questionType = QTTheProphets;
                break;

            case PillarType.Yellow:
                questionType = QTIslamicHistory;
                break;
        }

        return questionType;
    }

    public static QuestionType GetQuestionTypeFromNodeType(NodeType nodeType)
    {
        QuestionType qtype = QuestionType.Random;
        
        switch (nodeType)
        {
            case NodeType.ChooseCategory:
            case NodeType.WinAPillarOrGoToSchool:
            case NodeType.JumpToAnyPillar:
            case NodeType.SchoolBox:
            case NodeType.AnswerEasyToWinTwoJump:
                qtype = QuestionType.Random;
                break;
            case NodeType.Red:
                qtype = QuestionType.GeneralKnowledge;
                break;
            case NodeType.Blue:
                qtype = QuestionType.Quran;
                break;
            case NodeType.Green:
                qtype = QuestionType.Hadith;
                break;
            case NodeType.Orange:
                qtype = QuestionType.IslamicScience;
                break;
            case NodeType.Purple:
                qtype = QuestionType.TheProphets;
                break;
            case NodeType.Yellow:
                qtype = QuestionType.IslamicHistory;
                break;
        }

        return qtype;
    }

    public static PillarType GetColorTypeFromNodeType(NodeType node)
    {
        PillarType ptype = PillarType.Gray;

        switch (node)
        {
            case NodeType.Red:
                ptype = PillarType.Red;
                break;
            case NodeType.Blue:
                ptype = PillarType.Blue;
                break;
            case NodeType.Green:
                ptype = PillarType.Green;
                break;
            case NodeType.Orange:
                ptype = PillarType.Orange;
                break;
            case NodeType.Purple:
                ptype = PillarType.Purple;
                break;
            case NodeType.Yellow:
                ptype = PillarType.Yellow;
                break;
        }

        return ptype;
    }
	
}
