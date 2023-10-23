using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageWaveWheel : MonoBehaviour, IPauseObserver
{
    public void Init()
    {
        myRt = GetComponent<RectTransform>();
        waitSecondsHalf = new WaitForSeconds(0.5f);
        ArrayPauseCommand.Use(EPauseCommand.REGIST, this);
        StartCoroutine("WheelSpinCoroutine");
    }

    private IEnumerator WheelSpinCoroutine()
    {
        while (true)
        {
            while (isPaused)
                yield return waitSecondsHalf;

            myRt.Rotate(0f, 0f, -30f);
            yield return waitSecondsHalf;
        }
    }

    public void CheckPause(bool _isPause)
    {
        isPaused = _isPause;
    }

    private RectTransform myRt = null;
    private bool isPaused = false;
    private WaitForSeconds waitSecondsHalf = null;
}
