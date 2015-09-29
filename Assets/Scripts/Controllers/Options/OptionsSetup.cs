using UnityEngine;
using System.Collections;

public class OptionsSetup : MonoBehaviour
{
    public UIButton vibrationOn;
    public UIButton vibrationOff;
    public UIButton tutorialOn;
    public UIButton tutorialOff;
    public UISlider sfxLevel;
    public UISlider volume;
    public UIButton english;
    public UIButton french;
    public UIButton arabic;

    void Start()
    {
        bool isVibrate = GameConstants.IsVibration;
        bool isTutorial = GameConstants.IsTutorial;
        float sfx = GameConstants.SfxLevel;
        float volumeLevel = GameConstants.Volume;
        string lang = GameConstants.Language;

        Debug.Log("IsVibrate: " + isVibrate);
		//
        ToggleOnOff(ref vibrationOn, ref vibrationOff, isVibrate);
        ToggleOnOff(ref tutorialOn, ref tutorialOff, isTutorial);

        if (sfxLevel != null)
        {
            sfxLevel.sliderValue = sfx;
        }

        if (volume != null)
        {
            volume.sliderValue = volumeLevel;
        }

        ToggleLang(lang);
    }

    private void ToggleOnOff(ref UIButton target, ref UIButton other, bool state)
    {
        if (target != null)
        {
			target.isEnabled = state;
            Debug.Log(target.name);
            
        }
        if (other != null)
        {
            other.isEnabled = !state;
        }
    }

    private void ToggleLang(string lang)
    {
        if (english == null || french == null || arabic == null)
        {
            Debug.LogWarning("Some Language button is not set");
            return;
        }
        english.isEnabled = false;
        french.isEnabled = false;
        arabic.isEnabled = false;
        switch (lang)
        {
            case "en" :
                english.isEnabled = true;
                break;

            case "fr" :
                french.isEnabled = true;
                break;

            case "ar":
                arabic.isEnabled = true;
                break;
        }

    }
}
