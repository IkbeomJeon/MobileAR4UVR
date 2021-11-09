using KCTM.Network;
using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoCard : NormalCard
{
    //public VideoPlayerManager videoPlayerManager;


    public void Init(Anchor anchor, string parentName = "", bool showGoButton = false, PreviewCard videoCard_Preview = null)
    {
        base.Init(anchor, parentName);

        GameObject videoPlayButton = transform.Find(parentName + "VideoObject/Video Player/PlayButton").gameObject;
        GameObject videoPauseButton = transform.Find(parentName + "VideoObject/Video Player/PauseButton").gameObject;
        Slider videoSlider = transform.Find(parentName + "VideoObject/Video Player/Slider").GetComponent<Slider>();
        GameObject fullScreenButton = transform.Find(parentName + "VideoObject/Video Player/Fullscreen").gameObject;
        RawImage videoTexture = transform.Find(parentName + "VideoObject/Video Player").GetComponent<RawImage>();
        AspectRatioFitter aspectRatioFilter = transform.Find("VideoObject/Video Player").GetComponent<AspectRatioFitter>();

        Transform goButton = transform.Find(parentName + "BottomInfo/GoButton");
        if (goButton != null)
        {
            goButton.gameObject.SetActive(false);

            if (showGoButton)
                goButton.gameObject.SetActive(true);
        }


        VideoPlayer videoPlayer;
        if (videoCard_Preview)
        {
            videoPlayer = ((VideoCard_Preview)videoCard_Preview).gameObject.GetComponent<VideoPlayerManager>().videoPlayer;
            StartCoroutine(gameObject.AddComponent<VideoPlayerManager>().Init(videoPlayer,
                videoPlayButton, videoPauseButton, videoSlider, videoTexture, aspectRatioFilter));
        }

        else
        {
            videoPlayer = transform.Find(parentName + "VideoObject/Video Player").GetComponent<VideoPlayer>();
            StartCoroutine(gameObject.AddComponent<VideoPlayerManager>().Init(videoPlayer,
               videoPlayButton, videoPauseButton, videoSlider, videoTexture, aspectRatioFilter, true, anchor.contentinfos[0].content.uri));
        }

        fullScreenButton.SetActive(true);
        fullScreenButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            panelManager.GetComponent<PanelManager>().ShowFullScreenPanel(videoPlayer);
        });
    }
    


}
