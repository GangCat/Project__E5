using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasWaveInfo : MonoBehaviour
{
    public void Init()
    {
        imageWaveProgressbar.Init();
        wheel.Init();
        textRemainEnemyCnt.Init();
    }

    public void Init(float _ttlBigWaveTime)
    {
        ttlBigWaveTime = _ttlBigWaveTime;
    }

    public void UpdateWaveTime(float _bigWaveTime_sec)
    {
        textBigWaveTime.text = string.Format("{0:D2}:{1:D2}", (int)_bigWaveTime_sec / 60, (int)_bigWaveTime_sec % 60);
        
        imageWaveProgressbar.UpdateLength((ttlBigWaveTime - _bigWaveTime_sec) / ttlBigWaveTime);
    }

    public void WaveStart(int _count)
    {
        textRemainEnemyCnt.WaveStart(_count);
    }

    public void UpdateRemainEnemyCnt(int _count)
    {
        textRemainEnemyCnt.UpdateText(_count);
    }

    public void WaveEnd()
    {
        textRemainEnemyCnt.WaveEnd();
    }

    [SerializeField]
    private ImageProgressbar imageWaveProgressbar = null;
    [SerializeField]
    private TMP_Text textBigWaveTime = null;
    [SerializeField]
    private ImageWaveWheel wheel = null;
    [SerializeField]
    private TextRemainEnemyCnt textRemainEnemyCnt = null;

    private float ttlBigWaveTime = 0f;
}
