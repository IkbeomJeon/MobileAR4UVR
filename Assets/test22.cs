using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class test22 : MonoBehaviour
{
    // Start is called before the first frame up
    VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        StartCoroutine(VideoPlayerInit());
     
    }

    IEnumerator VideoPlayerInit()
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.url = "https://drive.google.com/uc?export=download&id=18j9rJDZ43UCII0A_UlM4mLqdL9bJeCjS";
        videoPlayer.isLooping = true;
        //videoPlayer.errorReceived += ErrorReceived;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared == true);

        videoPlayer.Play();

        int videoWidth = videoPlayer.texture.width;
        int videoHeight = videoPlayer.texture.height;

        RenderTexture renderTexture = new RenderTexture(videoWidth, videoHeight, 24, RenderTextureFormat.ARGB32);
        videoPlayer.targetTexture = renderTexture;
        float scale = (float)videoWidth / videoHeight;
        gameObject.GetComponent<RawImage>().texture = videoPlayer.targetTexture;

    }
        // Update is called once per frame
    void Update()
    {
        
    }
}
