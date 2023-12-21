using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FogManager : MonoBehaviour, IPauseObserver
{
    public void Init()
    {
        ArrayPauseCommand.Use(EPauseCommand.REGIST, this);
        curFogTexture = GenerateTexture(fogRenderTexture);
        backBufftexture = GenerateTexture(fogRenderTexture);

        newFogRenderTexture = new RenderTexture(fogRenderTexture.width, fogRenderTexture.height, 0, RenderTextureFormat.ARGB32);
        newFogRenderTexture.enableRandomWrite = true;
        newFogRenderTexture.Create();
        newBackBuffRenderTexture = new RenderTexture(fogRenderTexture.width, fogRenderTexture.height, 0, RenderTextureFormat.ARGB32);
        newBackBuffRenderTexture.enableRandomWrite = true;
        newBackBuffRenderTexture.Create();

        fogComputeShader.SetTexture(0, "fogRenderTexture", newFogRenderTexture);
        fogComputeShader.SetTexture(0, "backBuffRenderTexture", newBackBuffRenderTexture);

        UpdateFogTexture();
    }

    /// <summary>
    /// 맵핵 ONOff
    /// </summary>
    public void ToggleFogVisible()
    {
        isFogVisible = !isFogVisible;
        combineGo.SetActive(isFogVisible);
    }

    public void IsDebugMode(bool _isDebugMode)
    {
        isDebugMode = _isDebugMode;
    }

    private void UpdateFogTexture()
    {
        if (isPause)
        {
            Invoke("UpdateFogTexture", updateFogDelay);
            return;
        }

        // 카메라를 이용해 포그를 렌더텍스쳐에 기록
        mainCam.RenderFog();

        // 카메라로 찍은 포그를 복사
        Graphics.CopyTexture(fogRenderTexture, newFogRenderTexture);

        // 스레드를 몇*몇으로 돌릴지 결정하기 위한 변수 2개
        // 지금의 경우는 총 8*8개로 돌릴 것이라는 의미.
        int threadGroupsX = Mathf.CeilToInt(newFogRenderTexture.width / 8f);
        int threadGroupsY = Mathf.CeilToInt(newFogRenderTexture.height / 8f);
        // 0은 컴퓨트 쉐이더의 ㅇ몇번째 커넬인지, 총 몇 스레드인지(현재는 8 * 8 * 1개임)
        fogComputeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        // 포그 텍스쳐를 다시 현재 포그 텍스쳐에 복붙
        Graphics.CopyTexture(newFogRenderTexture, curFogTexture);
        // 컴퓨트 쉐이더로 백버퍼로 만든 백버퍼 텍스쳐를 복붙
        Graphics.CopyTexture(newBackBuffRenderTexture, backBufftexture);


        combineGo.GetComponent<MeshRenderer>().material.SetTexture("_FogTexture", curFogTexture);
        combineGo.GetComponent<MeshRenderer>().material.SetTexture("_BackBufferTexture", backBufftexture);
        combineGo.GetComponent<MeshRenderer>().material.SetTexture("_MapTexture", mapRenderTexture);
        ImageMinimap.SetVisibleTexture(curFogTexture);

        // 해당 모드일 경우 현재 위치를 기록하는 포그와 지나간 위치를 기록하는 포그를 보여줌.
        if (isDebugMode)
        {
            Sprite spriteFog = Sprite.Create(
                curFogTexture, 
                new Rect(0, 0, curFogTexture.width, curFogTexture.height), 
                new Vector2(0.5f, 0.5f));
            spriteFog.name = "Fog";
            fogImage.sprite = spriteFog;
            Sprite spriteBuffer = Sprite.Create(
                backBufftexture, 
                new Rect(0, 0, backBufftexture.width, backBufftexture.height), 
                new Vector2(0.5f, 0.5f));
            spriteBuffer.name = "Buffer";
            bufferImage.sprite = spriteBuffer;
        }

        Invoke("UpdateFogTexture", updateFogDelay);
    }

    private Texture2D GenerateTexture(RenderTexture _texture)
    {
        Texture2D newTexture = new Texture2D(_texture.width, _texture.height, TextureFormat.RGBA32, false);

        RenderTexture.active = _texture;
        newTexture.ReadPixels(new Rect(0, 0, _texture.width, _texture.height), 0, 0);
        newTexture.Apply();
        RenderTexture.active = null;
        return newTexture;
    }

    public void CheckPause(bool _isPause)
    {
        isPause = _isPause;
    }

    [SerializeField]
    private float updateFogDelay = 0f;
    [SerializeField]
    private RenderTexture fogRenderTexture = null;
    [SerializeField]
    private RenderTexture mapRenderTexture = null;
    [SerializeField]
    private Image fogImage = null;
    [SerializeField]
    private Image bufferImage = null;
    [SerializeField]
    private ComputeShader fogComputeShader = null;
    [SerializeField]
    private GameObject combineGo = null;
    [SerializeField]
    private CameraMovement mainCam = null;
    [SerializeField]
    private bool isDebugMode = false;

    private Texture2D curFogTexture = null;
    private Texture2D backBufftexture = null;

    private RenderTexture newFogRenderTexture = null;
    private RenderTexture newBackBuffRenderTexture = null;
    private bool isPause = false;
    private bool isFogVisible = true;
}
