using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileNuclear : MonoBehaviour, IPauseObserver
{
    public void Init(Vector3 _pos)
    {
        effectCtrl = GetComponent<NuclearMissileEffectController>();
        effectCtrl.Init();
        transform.localPosition = _pos;
    }
    public void SetActive(bool _isActive)
    {
        gameObject.SetActive(_isActive);
    }

    public void ResetRotate()
    {
        transform.rotation = Quaternion.identity;
    }

    public void Launch(Vector3 _destPos)
    {
        effectCtrl.EffectOn(0);
        ArrayPauseCommand.Use(EPauseCommand.REGIST, this);
        StartCoroutine("LaunchCoroutine", _destPos);
    }

    private IEnumerator LaunchCoroutine(Vector3 _destPos)
    {
        while (transform.position.y < 30f)
        {
            while (isPause)
                yield return null;

            transform.position += Vector3.up * launchSpeed * Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(_destPos.x, 30f, _destPos.z);
        transform.rotation = Quaternion.Euler(new Vector3(180f, 0f, 0f));
        GameObject visibleGo  = Instantiate(visibleAreaGo, _destPos, Quaternion.identity);
        visibleGo.transform.localScale = new Vector3(30, 1, 30);
        while(transform.position.y > 0)
        {
            while (isPause)
                yield return null;

            transform.position -= Vector3.up * launchSpeed * Time.deltaTime;
            yield return null;
        }

        Collider[] arrCol = Physics.OverlapSphere(transform.position, attackRange, targetMask);
        for(int i = 0; i < arrCol.Length; ++i)
            arrCol[i].gameObject.GetComponent<SelectableObject>().GetDmg(attDmg);

        // Nuclear Explosion Audio
        AudioManager.instance.PlayAudio_Misc(EAudioType_Misc.NUCLEAR_EXPLOSION);

        effectCtrl.EffectOn(1, true);
        ArrayPauseCommand.Use(EPauseCommand.REMOVE, this);
        Destroy(visibleGo, 5f);
        Destroy(gameObject);
    }

    public void CheckPause(bool _isPause)
    {
        isPause = _isPause;
    }

    [SerializeField]
    private float attackRange = 0f;
    [SerializeField]
    private float launchSpeed = 0f;
    [SerializeField]
    private float attDmg = 0f;
    [SerializeField]
    private GameObject visibleAreaGo = null;

    private NuclearMissileEffectController effectCtrl = null;

    private bool isPause = false;

    private EObjectType objectType;
    private EAudioType_Misc audioType;

    [SerializeField] private LayerMask targetMask;

    // private NuclearMissileEffectController nuclearMissileEffect;
}
