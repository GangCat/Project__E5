using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSFXTestPlay : MonoBehaviour
{
    public void Init()
    {
        instance = AudioManager.instance;

        GetComponent<Button>().onClick.AddListener(
            () =>
            {
                instance.PlayAudio_Advisor(EAudioType_Advisor.RESEARCH);
            });
    }

    private AudioManager instance = null;
}
