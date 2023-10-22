using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMinimapSignal : MonoBehaviour
{
    public void Init()
    {
        myRt = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void SetAnchoredPos(Vector2 _pos)
    {
        myRt.anchoredPosition = _pos;
    }

    private void OnEnable()
    {
        if (myRt)
        {
            myRt.sizeDelta = new Vector2(100, 100);
            StartCoroutine(SignalEffectCoroutine());
        }
    }

    private void OnDisable()
    {
        // 게임 오브젝트가 비활성화될 때 사이즈를 다시 100, 100으로 변경합니다.
        myRt.sizeDelta = new Vector2(100, 100);
    }

    private IEnumerator SignalEffectCoroutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(0.5f);
        float elapsedTime = 0;
        while(elapsedTime <= 2f)
        {
            myRt.sizeDelta = new Vector2(100 - 80 * elapsedTime, 100 - 80 * elapsedTime);
            yield return waitTime;
            elapsedTime += 0.5f;
        }

        // 3초 동안 0.5초 간격으로 깜빡입니다.
        float blinkTime = 0.5f;

        while (true)
        {
            // 크기를 0, 0으로 만들어 깜빡입니다.
            myRt.sizeDelta = new Vector2(0, 0);
            yield return waitTime;

            // 크기를 20, 20으로 되돌려 깜빡입니다.
            myRt.sizeDelta = new Vector2(20, 20);
            yield return waitTime;

            // 깜빡임이 3초 동안 반복됩니다.
            blinkTime += 1f;
            if (blinkTime >= 3f)
            {
                break;
            }
        }
    }

    private RectTransform myRt = null;
}
