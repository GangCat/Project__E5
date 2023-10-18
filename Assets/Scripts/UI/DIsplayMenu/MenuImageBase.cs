using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuImageBase : MonoBehaviour
{
    public virtual void Init()
    {
        myRt = GetComponent<RectTransform>();
    }

    public void DisplayImage()
    {
        SetActive(true);
    }

    public void HideImage()
    {
        SetActive(false);
    }

    protected void SetActive(bool _isActive)
    {
        gameObject.SetActive(_isActive);
    }

    public void ChangeImageHeight()
    {
        myRt.anchorMax = new Vector2(myRt.anchorMax.x, 0.2f);
        myRt.anchorMin = new Vector2(myRt.anchorMin.x, 0.3f);

        //myRt.offsetMin = Vector2.zero;
        //myRt.offsetMax = Vector2.zero;
        //myRt.rect.width = 100;
        //Screen.width
    }

    protected RectTransform myRt = null;
    [SerializeField, Range(0, 0.5f)]
    protected float topOffset = 0f;
    [SerializeField, Range(0, 0.5f)]
    protected float bottomOffset = 0f;
}
