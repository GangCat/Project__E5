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
        // ���� ������Ʈ�� ��Ȱ��ȭ�� �� ����� �ٽ� 100, 100���� �����մϴ�.
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

        // 3�� ���� 0.5�� �������� �����Դϴ�.
        float blinkTime = 0.5f;

        while (true)
        {
            // ũ�⸦ 0, 0���� ����� �����Դϴ�.
            myRt.sizeDelta = new Vector2(0, 0);
            yield return waitTime;

            // ũ�⸦ 20, 20���� �ǵ��� �����Դϴ�.
            myRt.sizeDelta = new Vector2(20, 20);
            yield return waitTime;

            // �������� 3�� ���� �ݺ��˴ϴ�.
            blinkTime += 1f;
            if (blinkTime >= 3f)
            {
                break;
            }
        }
    }

    private RectTransform myRt = null;
}
