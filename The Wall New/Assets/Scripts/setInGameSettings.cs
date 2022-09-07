using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setInGameSettings : MonoBehaviour
{
    private void Start()
    {
        setOptions();
    }

    private void setOptions()
    {
        //vsync
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }

}
