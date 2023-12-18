using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageMinimap : MonoBehaviour, IPointerClickHandler, IMinimapSubject, IPauseObserver
{
    public void Init()
    {
        listObserver = new List<IMinimapObserver>();
        tempFriendlyNodeList = new List<PF_Node>();
        tempEnemyNodeList = new List<PF_Node>();
        listStructureNode = new List<PF_Node>();

        ArrayPauseCommand.Use(EPauseCommand.REGIST, this);
        imageMinimap = GetComponent<Image>();
        tex2d = new Texture2D(256, 256);
        texW = tex2d.width;
        texH = tex2d.height;

        texRect = new Rect(0, 0, texW, texH);
        pivotVec = new Vector2(0.5f, 0.5f);

        int idx = 0;

        // 딱 한번 모든 픽셀을 검은색으로 초기화
        while (idx < texW * texH)
        {
            tex2d.SetPixel(idx % texW, idx / texH, Color.black);
            ++idx;
        }

        imageFriendlySignal.Init();
        imageAttackSignal.Init();
        for (int i = 0; i < arrImageBigEnemySignal.Length; ++i)
            arrImageBigEnemySignal[i].gameObject.SetActive(false);

        StartCoroutine("UpdateMinimap");
    }

    public void Init(float _worldSizeX, float _worldSizeY)
    {
        worldSizeX = _worldSizeX;
        worldSizeY = _worldSizeY;
    }

    public static void SetVisibleTexture(Texture2D _tex)
    {
        RenderTexture rt = new RenderTexture(256, 256, 0);
        // 전달받은 텍스쳐 2D의 내용을 rt에 복제
        Graphics.Blit(_tex, rt);

        // 렌더 텍스쳐를 사용할때는 이렇게 사용할 ㅇ렌더 텍스쳐를 설정해야함
        RenderTexture.active = rt;
        visibleAreaTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        visibleAreaTexture.Apply();

        // 렌더 텍스쳐 사용 후에는 해제해야함.
        RenderTexture.active = null;
        Destroy(rt);
    }

    public void AddStructureNodeToMinimap(PF_Node _node)
    {
        listStructureNode.Add(_node);
    }

    public void RemoveStructureNodeFromMinimap(PF_Node _node)
    {
        listStructureNode.Remove(_node);
    }

    private IEnumerator UpdateMinimap()
    {
        yield return null;
        while (true)
        {
            while (isPause)
                yield return null;

            UpdateTexture(ref tex2d);
            imageMinimap.sprite = Sprite.Create(tex2d, texRect, pivotVec);

            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// 미니맵을 갱신하는 함수
    /// </summary>
    /// <param name="tex2d"></param>
    private void UpdateTexture(ref Texture2D tex2d)
    {
        // 이전에 적군, 아군 위치를 모두 검은색으로 초기화
        // 이렇게 하지 않으면 모든 픽셀을 검사해야 하기 때문에 부하가 매우 심함.
        for (int i = 0; i < tempFriendlyNodeList.Count; ++i)
            tex2d.SetPixel(tempFriendlyNodeList[i].gridX, tempFriendlyNodeList[i].gridY, Color.black);

        for (int i = 0; i < tempEnemyNodeList.Count; ++i)
            tex2d.SetPixel(tempEnemyNodeList[i].gridX, tempEnemyNodeList[i].gridY, Color.black);

        tempFriendlyNodeList.Clear();
        tempEnemyNodeList.Clear();

        foreach (PF_Node node in SelectableObjectManager.DicNodeUnderFriendlyUnit.Values)
        {
            tempFriendlyNodeList.Add(node);
            tex2d.SetPixel(node.gridX, node.gridY, Color.green);
        }

        foreach (PF_Node node in SelectableObjectManager.DicNodeUnderEnemyUnit.Values)
        {
            tempEnemyNodeList.Add(node);
            tex2d.SetPixel(node.gridX, node.gridY, Color.red);
        }

        PF_Node tempNode = null;

        for (int i = 0; i < listStructureNode.Count; ++i)
        {
            tempNode = listStructureNode[i];
            tempFriendlyNodeList.Add(tempNode);
            tex2d.SetPixel(tempNode.gridX, tempNode.gridY, Color.green);
        }

        if (isBigEnemySignalDisplay)
        {
            isBigEnemySignalDisplay = false;
            for (int i = 0; i < arrBigEnemyTr.Length; ++i)
            {
                if (arrBigEnemyTr[i] && arrBigEnemyTr[i].gameObject.activeSelf)
                {
                    isBigEnemySignalDisplay = true;
                    arrImageBigEnemySignal[i].rectTransform.anchoredPosition =
                        WorldToMinimapPosition(
                            arrBigEnemyTr[i].position, 
                            imageMinimap.rectTransform, 
                            128, 
                            128);
                }
                else
                    arrImageBigEnemySignal[i].gameObject.SetActive(false);
            }
        }
        // 다 수정한 텍스쳐를 실제로 적용
        tex2d.Apply();
    }

    /// <summary>
    /// 빅 웨이브에서 적 대형유닛 위치 표시
    /// </summary>
    /// <param name="_arrTr"></param>
    public void BigEnemySignal(Transform[] _arrTr)
    {
        isBigEnemySignalDisplay = true;
        arrBigEnemyTr = _arrTr;

        for (int i = 0; i < arrBigEnemyTr.Length; ++i)
            arrImageBigEnemySignal[i].gameObject.SetActive(true);
    }

    /// <summary>
    /// 아군 유닛 생성시 표시
    /// </summary>
    /// <param name="_worldPos"></param>
    public void FriendlySignal(Vector3 _worldPos)
    {
        if (!isFriendlySignalDisplay)
            StartCoroutine(FriendlySignalAutoDisable(_worldPos));
    }

    private IEnumerator FriendlySignalAutoDisable(Vector3 _worldPos)
    {
        imageFriendlySignal.gameObject.SetActive(true);
        isFriendlySignalDisplay = true;
        imageFriendlySignal.SetAnchoredPos(WorldToMinimapPosition(_worldPos, imageMinimap.rectTransform, 128, 128));
        yield return new WaitForSeconds(5f);

        imageFriendlySignal.gameObject.SetActive(false);
        isFriendlySignalDisplay = false;
    }

    /// <summary>
    /// 공격당할 때 미니맵에 표시
    /// </summary>
    /// <param name="_worldPos"></param>
    public void AttackSignal(Vector3 _worldPos)
    {
        if (!isAttackSignalDisplay)
            StartCoroutine(AttackSignalAutoDisable(_worldPos));
    }

    private IEnumerator AttackSignalAutoDisable(Vector3 _worldPos)
    {
        imageAttackSignal.gameObject.SetActive(true);
        isAttackSignalDisplay = true;
        imageAttackSignal.SetAnchoredPos(WorldToMinimapPosition(_worldPos, imageMinimap.rectTransform, 128, 128));
        yield return new WaitForSeconds(5f);

        imageAttackSignal.gameObject.SetActive(false);
        isAttackSignalDisplay = false;
    }

    /// <summary>
    /// 마우스로 UI 클릭하면 호출, 포인터클릭핸들러
    /// 매일 호출되는 함수는 아니기에 지역변수 사용
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, null, out var localMousePosition);

        float halfWidth = rectTransform.rect.width * 0.5f;
        float halfHeight = rectTransform.rect.height * 0.5f;

        float relativeX = (localMousePosition.x + halfWidth) / rectTransform.rect.width - 0.5f;
        float relativeY = (localMousePosition.y + halfHeight) / rectTransform.rect.height - 0.5f;

        if (eventData.button.Equals(PointerEventData.InputButton.Left))
        {
            foreach (IMinimapObserver ob in listObserver)
                ob.GetCameraTargetPos(new Vector3(relativeX * worldSizeX, 0f, relativeY * worldSizeY));
        }
        else if (eventData.button.Equals(PointerEventData.InputButton.Right))
        {
            foreach (IMinimapObserver ob in listObserver)
                ob.GetUnitTargetPos(new Vector3(relativeX * worldSizeX, 0f, relativeY * worldSizeY));
        }
    }

    private Vector2 WorldToMinimapPosition(Vector3 worldPosition, RectTransform minimapRectTransform, float worldSizeX, float worldSizeY)
    {
        // 월드 좌표를 미니맵상의 상대 좌표로 변환.
        relativeX = (worldPosition.x / worldSizeX) + 0.5f;
        relativeY = (worldPosition.z / worldSizeY) + 0.5f;

        // 미니맵 RectTransform의 크기를 고려하여 실제 화면 좌표로 변환.
        minX = minimapRectTransform.rect.xMin + minimapRectTransform.anchoredPosition.x;
        maxX = minimapRectTransform.rect.xMax + minimapRectTransform.anchoredPosition.x;
        minY = minimapRectTransform.rect.yMin + minimapRectTransform.anchoredPosition.y;
        maxY = minimapRectTransform.rect.yMax + minimapRectTransform.anchoredPosition.y;

        // 미니맵상의 상대 좌표를 실제 화면 좌표로 매핑.
        mappedX = Mathf.Lerp(minX, maxX, relativeX);
        mappedY = Mathf.Lerp(minY, maxY, relativeY);

        // 실제 화면 좌표로 변환해서 반환.
        return RotatePointAroundPivot(new Vector2(mappedX, mappedY), minimapRectTransform.rect.center, 45f);
    }

    private Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float angle)
    {
        // 앵커를 중심으로 회전하는 Quaternion을 생성.
        // 현재는 45도 고정이긴 함.
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // 해당 위치를 앵커를 중심으로 회전.
        Vector2 rotatedPoint = rotation * (point - pivot);

        // 회전된 위치 반환.
        return rotatedPoint + pivot;
    }

    public void RegisterPauseObserver(IMinimapObserver _observer)
    {
        listObserver.Add(_observer);
    }

    public void RemovePauseObserver(IMinimapObserver _observer)
    {
        listObserver.Remove(_observer);
    }

    public void CheckPause(bool _isPause)
    {
        isPause = _isPause;
    }

    [SerializeField]
    private ImageMinimapSignal imageFriendlySignal = null;
    [SerializeField]
    private ImageMinimapSignal imageAttackSignal = null;
    [SerializeField]
    private Image[] arrImageBigEnemySignal = null;

    private static Texture2D visibleAreaTexture = null;

    private Image imageMinimap = null;
    private Texture2D tex2d = null;
    private Rect texRect;
    private Vector2 pivotVec;
    private List<PF_Node> listStructureNode = null;
    private Transform[] arrBigEnemyTr = null;

    private int texH = 0;
    private int texW = 0;
    private List<IMinimapObserver> listObserver = null;
    private List<PF_Node> tempFriendlyNodeList = null;
    private List<PF_Node> tempEnemyNodeList = null;

    private float worldSizeX = 0f;
    private float worldSizeY = 0f;

    private bool isPause = false;
    private bool isFriendlySignalDisplay = false;
    private bool isAttackSignalDisplay = false;
    private bool isBigEnemySignalDisplay = false;

    private float relativeX = 0f;
    private float relativeY = 0f;
    private float minX = 0f;
    private float maxX = 0f;
    private float minY = 0f;
    private float maxY = 0f;
    private float mappedX = 0f;
    private float mappedY = 0f;
}
