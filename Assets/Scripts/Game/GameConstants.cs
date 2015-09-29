using UnityEngine;
using System.Collections;

public class GameConstants
{
    public const string VIBRATION = "tni_vibration";
    public const string LANGUAGE = "tni_language";
    public const string TUTORIAL = "tni_tutorial";
    public const string SFXLEVEL = "tni_sfxlevel";
    public const string VOLUME = "tni_volume";
    public const string PLAYER_BOX_PREFIX = "Box_";

    public static string Language
    {
        get
        {
            return PlayerPrefs.GetString(LANGUAGE, "en");
        }
        set
        {
            PlayerPrefs.SetString(LANGUAGE, value);
        }
    }

    public static bool IsVibration
    {
        get
        {
            int vib = PlayerPrefs.GetInt(VIBRATION, 1);
            Debug.Log("Vib " + vib);
            return vib == 1;
        }
        set
        {
            PlayerPrefs.SetInt(VIBRATION, value ? 1 : 0);
        }
    }

    public static bool IsTutorial
    {
        get
        {
            return PlayerPrefs.GetInt(TUTORIAL, 1) == 1 ? true : false;
        }
        set
        {
            PlayerPrefs.SetInt(TUTORIAL, value ? 1 : 0);
        }
    }

    public static float SfxLevel
    {
        get
        {
            return PlayerPrefs.GetFloat(SFXLEVEL, 0.3f);
        }
        set
        {
            PlayerPrefs.SetFloat(SFXLEVEL, value);
        }
    }

    public static float Volume
    {
        get
        {
            return PlayerPrefs.GetFloat(VOLUME, 0.3f);
        }
        set
        {
            PlayerPrefs.SetFloat(VOLUME, value);
        }
    }

}
