using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextRemainEnemyCnt : TextBase
{
    public override void Init()
    {
        base.Init();
        gameObject.SetActive(false);
    }

    public void WaveStart(int _cnt)
    {
        UpdateText(_cnt);
        gameObject.SetActive(true);
    }

    public void WaveEnd()
    {
        gameObject.SetActive(false);
    }

    public override void UpdateText(params object[] _objects)
    {
        myText.text = ((int)_objects[0]).ToString();
    }
}
