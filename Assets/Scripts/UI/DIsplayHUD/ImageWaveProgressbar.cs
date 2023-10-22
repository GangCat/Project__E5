using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageWaveProgressbar : ImageProgressbar
{
    public override void UpdateLength(float _ratio)
    {
        myRt.sizeDelta = new Vector2(maxLength * (1 - _ratio), myHeight);
    }
}
