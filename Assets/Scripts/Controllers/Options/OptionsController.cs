using UnityEngine;
using System.Collections;

public class OptionsController : UIViewController
{
   
    void OnSfxLevelChange(float val)
    {
        GameConstants.SfxLevel = val;
    }

    void OnVolumeChange(float val)
    {
        GameConstants.Volume = val;
    }

    void OnEnglishOn(UIButton button)
    {
        if (button.isEnabled)
        {
            GameConstants.Language = "en";
        }
    }

    void OnFrenchOn(UIButton button)
    {
        if (button.isEnabled)
        {
            GameConstants.Language = "fr";
        }
    }

    void OnArabicOn(UIButton button)
    {
        if (button.isEnabled)
        {
            GameConstants.Language = "ar";
        }
    }

    void OnVibrationOn(UIButton button)
    {
        ToggleVibration(button.isEnabled);
    }

    void OnVibrationOff(UIButton button)
    {
        ToggleVibration(!button.isEnabled);
    }

    void OnTutorialOn(UIButton button)
    {
        ToggleTutorial(button.isEnabled);
    }

    void OnTutorialOff(UIButton button)
    {
        ToggleTutorial(!button.isEnabled);
    }

    void ToggleVibration(bool enabled)
    {
        GameConstants.IsVibration = enabled;
        Debug.Log("vibration: " + enabled);
    }

    void ToggleTutorial(bool enabled)
    {
        GameConstants.IsTutorial = enabled;
    }
}
