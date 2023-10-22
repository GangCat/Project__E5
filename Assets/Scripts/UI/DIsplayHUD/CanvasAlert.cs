using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAlert : MonoBehaviour
{
    public void Init()
    {
        imageBox.Init();
    }

    public void AlertUpgradeComplete()
    {
        StartCalcAutoHideAlert();
        imageBox.Display(upgradeAlertText);
    }

    public void AlertBuildComplete()
    {
        StartCalcAutoHideAlert();
        imageBox.Display(buildCompleteText);
    }

    public void AlertUnderAttack()
    {
        if (canAlertUnderAttack)
        {
            imageBox.Display(underAttackText);
            StartCoroutine(UnderAttackDelayCoroutine());
            StartCalcAutoHideAlert();
        }
    }

    public void AlertWaveStart()
    {
        StartCalcAutoHideAlert();
        imageBox.Display(waveStartText);
    }

    private IEnumerator UnderAttackDelayCoroutine()
    {
        canAlertUnderAttack = false;
        yield return new WaitForSeconds(underAttackAlertDelay);

        canAlertUnderAttack = true;
    }

    private IEnumerator CalcAutoHideAlert()
    {
        float elapsedTime = 0f;
        while (alertHideTime > elapsedTime)
        {
            yield return new WaitForSeconds(0.05f);
            elapsedTime += 0.05f;
        }
        imageBox.Hide();
    }

    private void StartCalcAutoHideAlert()
    {
        StopCoroutine("CalcAutoHideAlert");
        StartCoroutine("CalcAutoHideAlert");
    }



    [SerializeField]
    [TextArea]
    private string upgradeAlertText = null;
    [SerializeField]
    [TextArea]
    private string buildCompleteText = null;
    [SerializeField]
    [TextArea]
    private string underAttackText = null;
    [SerializeField]
    [TextArea]
    private string waveStartText = null;

    [SerializeField]
    private ImageAlertBox imageBox = null;
    [SerializeField]
    private float underAttackAlertDelay = 0f;
    [SerializeField]
    private float alertHideTime = 0f;
    private bool canAlertUnderAttack = true;
}
