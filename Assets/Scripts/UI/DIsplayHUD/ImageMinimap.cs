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

        ArrayPauseCommand.Use(EPauseCommand.REGIST,this);
        imageMinimap = GetComponent<Image>();
        tex2d = new Texture2D(256, 256);
        visibleAreaTexture = new Texture2D(256, 256);
        //tex2d.name = "Set Pixel";
        texW = tex2d.width;
        texH = tex2d.height;

        texRect = new Rect(0, 0, texW, texH);
        pivotVec = new Vector2(0.5f, 0.5f);

        int idx = 0;

        while (idx < texW * texH)
        {
            tex2d.SetPixel(idx % texW, idx / texH, Color.black);
            ++idx;
        }

        StartCoroutine("UpdateMinimap");
    }

    public static void SetVisibleTexture(Texture2D _tex)
    {
        RenderTexture rt = new RenderTexture(256, 256, 0);
        Graphics.Blit(_tex, rt);

        RenderTexture.active = rt;
        visibleAreaTexture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        visibleAreaTexture.Apply();

        // 더 이상 RenderTexture를 사용하지 않을 때 메모리에서 해제합니다.
        RenderTexture.active = null;
        Destroy(rt);
    }

    public void Init(float _worldSizeX, float _worldSizeY)
    {
        worldSizeX = _worldSizeX;
        worldSizeY = _worldSizeY;
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

    private void UpdateTexture(ref Texture2D tex2d)
    {
        for(int i = 0; i < tempFriendlyNodeList.Count; ++i) 
            tex2d.SetPixel(tempFriendlyNodeList[i].gridX, tempFriendlyNodeList[i].gridY, Color.black);

        for(int i = 0; i < tempEnemyNodeList.Count; ++i)
            tex2d.SetPixel(tempEnemyNodeList[i].gridX, tempEnemyNodeList[i].gridY, Color.black);

        tempFriendlyNodeList.Clear();
        tempEnemyNodeList.Clear();

        foreach (PF_Node node in SelectableObjectManager.DicNodeUnderFriendlyUnit.Values)
        {
            //if (visibleAreaTexture.GetPixel(node.gridX, node.gridY).Equals(Color.white))
            //{
                tempFriendlyNodeList.Add(node);
                tex2d.SetPixel(node.gridX, node.gridY, Color.green);
            //}
        }

        foreach (PF_Node node in SelectableObjectManager.DicNodeUnderEnemyUnit.Values)
        {
            //if (visibleAreaTexture.GetPixel(node.gridX, node.gridY).Equals(Color.white))
            //{
                tempEnemyNodeList.Add(node);
                tex2d.SetPixel(node.gridX, node.gridY, Color.red);
            //}
        }

        PF_Node tempNode = null;

        for (int i = 0; i < listStructureNode.Count; ++i)
        {
            tempNode = listStructureNode[i];
            tempFriendlyNodeList.Add(tempNode);
            tex2d.SetPixel(tempNode.gridX, tempNode.gridY, Color.green);
        }

        tex2d.Apply();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        // ���콺 Ŭ�� ��ġ�� RectTransform�� ���� ��ǥ��� ��ȯ�մϴ�.
        Vector2 localMousePosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, null, out localMousePosition);

        // �̴ϸ��� ũ���� ���� ����մϴ�.
        float halfWidth = rectTransform.rect.width * 0.5f;
        float halfHeight = rectTransform.rect.height * 0.5f;

        // �߽��� (0,0)���� �ϰ� ���ϴ�(-0.5, -0.5), �ֿ���(0.5, -0.5), �ֻ��(0.5, 0.5), ������(-0.5, 0.5)�� ���� ������� ��ǥ�� ����մϴ�.
        float relativeX = (localMousePosition.x + halfWidth) / rectTransform.rect.width - 0.5f;
        float relativeY = (localMousePosition.y + halfHeight) / rectTransform.rect.height - 0.5f;

        // ��� ���� (0,0)�� ���ϴ�, (1,1)�� �����̶�� �����ϰ� ����մϴ�.
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

    private static Texture2D visibleAreaTexture = null;

    private Image imageMinimap = null;
    private Texture2D tex2d = null;
    private Rect texRect;
    private Vector2 pivotVec;
    private List<PF_Node> listStructureNode = null;

    private int texH = 0;
    private int texW = 0;
    private List<IMinimapObserver> listObserver = null;
    private List<PF_Node> tempFriendlyNodeList = null;
    private List<PF_Node> tempEnemyNodeList = null;

    private float worldSizeX = 0f;
    private float worldSizeY = 0f;

    private bool isPause = false;
    
}
