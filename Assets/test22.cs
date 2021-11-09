using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class test22 : MonoBehaviour
{
    // Start is called before the first frame up
    VideoPlayer videoPlayer;
    private RenderTexture renderTexture;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        StartCoroutine(VideoPlayerInit());
     
    }

    IEnumerator VideoPlayerInit()
    {
        videoPlayer.playOnAwake = false;
        //videoPlayer.url = anchor.contentinfos[0].content.uri;
        videoPlayer.isLooping = true;
        //videoPlayer.errorReceived += ErrorReceived;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared == true);

        int videoWidth = videoPlayer.texture.width;
        int videoHeight = videoPlayer.texture.height;

        renderTexture = new RenderTexture(videoWidth, videoHeight, 24, RenderTextureFormat.ARGB32);
        videoPlayer.targetTexture = renderTexture;
        float scale = (float)videoWidth / videoHeight;
        gameObject.GetComponent<RawImage>().texture = renderTexture;

    }
        // Update is called once per frame
    void Update()
    {
        
    }
}
