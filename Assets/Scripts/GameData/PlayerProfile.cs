using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerProfile 
{
    static public PlayerProfile current;

    public class Profile
    {
        public string Name;
        public string Piece;
        public string Avatar;
		public int lastBlock = 0;
        public bool IsUsed = false;

        public Profile()
        {
            Name = "PLAYER 1";
            Piece = "";
            Avatar = "Box_Purple";
        }

        public Profile(string name, string avatar, string piece)
        {
            this.Name = name;
            this.Avatar = avatar;
            this.Piece = piece;
        }
    }

    public Profile[] profiles = new Profile[] 
    {
        new Profile("PLAYER 1", "Box_Purple", ""),
        new Profile("PLAYER 2", "Box_Green", ""),
        new Profile("PLAYER 3", "Box_Blue", ""),
        new Profile("PLAYER 4", "Box_Red", ""),
        new Profile("PLAYER 5", "Box_Yellow", ""),
        new Profile("PLAYER 6", "Box_Orange", "")
    };

    public int selectedProfileIndex = 0;

    public Profile GetPlayerProfile(int index)
    {
        if (index >= 0 && index < profiles.Length)
        {
            return profiles[index];
        }

        return null;
    }

    private Dictionary<string, ColorPathType> colorToCodeMap = new Dictionary<string, ColorPathType>()
    {
        { "Box_Purple", ColorPathType.Purple },
        { "Box_Green", ColorPathType.Green },
        { "Box_Blue", ColorPathType.Blue },
        { "Box_Red", ColorPathType.Red },
        { "Box_Yellow", ColorPathType.Yellow },
        { "Box_Orange", ColorPathType.Orange }
    };

    public ColorPathType GetColorPathFromProfile(int index)
    {
        Profile profile = GetPlayerProfile(index);

        if (profile != null)
        {
            string avatarBox = profile.Avatar;
            if (colorToCodeMap.ContainsKey(avatarBox))
            {
                return colorToCodeMap[avatarBox];
            }
        }

        return ColorPathType.Green;
    }

    static public void Load()
    {
        if (current == null)
        {
            current = new PlayerProfile();
        }

        string xml = PlayerPrefs.GetString("PlayerProfile");
        if (string.IsNullOrEmpty(xml.Trim()) == false)
        {
            //Debug.Log(xml);
            current = (PlayerProfile)XmlManager.DeserializeObject(typeof(PlayerProfile), xml);
        }
    }

    static public void Save()
    {
        if (current == null)
        {
            current = new PlayerProfile();
        }

        string xml = XmlManager.SerializeObject(typeof(PlayerProfile), current);
        PlayerPrefs.SetString("PlayerProfile", xml);
        PlayerPrefs.Save();
    }

	static public void Clear()
	{
		if (current == null)
		{
			current = new PlayerProfile();

			PlayerPrefs.SetString("PlayerProfile", "");
			PlayerPrefs.Save();
		}
	}
}
