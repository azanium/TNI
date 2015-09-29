using UnityEngine;
using System.Collections;

public class GameSettings : MonoBehaviour
{
    #region MemVars & Props

    static private GameSettings gameSettings = null;

    /// <summary>
    /// First background image for 788w
    /// </summary>
    public Texture2D background;


    /// <summary>
    /// GUI Skin for the Game
    /// </summary>
    public GUISkin defaultGuiSkin;

    #endregion


    #region Mono Methods

    protected void Awake()
    {
        gameSettings = this;
    }

    protected void OnDestroy()
    {
        background = null;
    }

    #endregion


    #region Static Methods

    static public Texture2D Background()
    {
        return gameSettings.background;
    }

    static public GUISkin GuiSkin()
    {
        return gameSettings.defaultGuiSkin;
    }

    #endregion

}
