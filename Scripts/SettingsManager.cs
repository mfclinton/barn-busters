using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SettingsManager : MonoBehaviour
{
    public Slider volumeSlider, timeScaleSlider, povSpeedSlider;
    public Toggle[] toggles;

    ControlDolly cd;

    private void Awake()
    {
        if(!PlayerPrefs.HasKey("volume"))
            PlayerPrefs.SetFloat("volume", 70f);
        else if (volumeSlider != null)
            volumeSlider.value = PlayerPrefs.GetFloat("volume");

        if (!PlayerPrefs.HasKey("timeScale"))
            PlayerPrefs.SetFloat("timeScale", 1f);
        else if (timeScaleSlider != null)
            timeScaleSlider.value = PlayerPrefs.GetFloat("timeScale");

        if (!PlayerPrefs.HasKey("POVSpeed"))
            PlayerPrefs.SetFloat("POVSpeed", 150f);
        else if (povSpeedSlider != null)
            povSpeedSlider.value = PlayerPrefs.GetFloat("POVSpeed");

        if (!PlayerPrefs.HasKey("HighGraphics"))
            PlayerPrefs.SetInt("HighGraphics", 1);
        // TODO UPDATE SETTINGS

        AudioListener.volume = PlayerPrefs.GetFloat("volume") / 100f;
        Time.timeScale = PlayerPrefs.GetFloat("timeScale");

        cd = FindObjectOfType<ControlDolly>();
        if(cd != null)
            cd.sensitivity = PlayerPrefs.GetFloat("POVSpeed");

        if(toggles != null && toggles.Length != 0)
        {
            int clickedToggle = PlayerPrefs.GetInt("HighGraphics");
            int i = 0;
            foreach (Toggle t in toggles)
            {
                bool activeState = clickedToggle == i;
                t.isOn = activeState;
                i++;
            }
        }

        UpdateGrass();
    }

    public void ChangeVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("volume", newVolume);
        AudioListener.volume = PlayerPrefs.GetFloat("volume") / 100f;
    }

    public void ChangeTimescale(float newTimescale)
    {
        PlayerPrefs.SetFloat("timeScale", newTimescale);
        Time.timeScale = PlayerPrefs.GetFloat("timeScale");
    }

    public void ChangePOVSpeed(float newPOVSpeed)
    {
        PlayerPrefs.SetFloat("POVSpeed", newPOVSpeed);
        if(cd != null)
            cd.sensitivity = PlayerPrefs.GetFloat("POVSpeed");
    }

    public void SetGrassState(bool activeState)
    {
        int state = activeState ? 1 : 0;
        PlayerPrefs.SetInt("HighGraphics", state);

        UpdateGrass();
    }

    public void UpdateGrass()
    {
        bool activeState = PlayerPrefs.GetInt("HighGraphics") == 1;

        GameObject[] highGraphicsObjs = GameObject.FindGameObjectsWithTag("HighGraphics");
        foreach (GameObject go in highGraphicsObjs)
        {
            go.transform.GetChild(0).gameObject.SetActive(activeState);
        }
    }
}
