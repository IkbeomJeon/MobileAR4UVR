using KCTM.Network;
using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoCard_Small : NormalCard
{
    //public Image image;
    public Text indexText;
    
    public VideoPlayer videoPlayer;
    public GameObject videoPlayButton;
    public GameObject videoPauseButton;
    public Slider videoSlider;
    public RawImage videoTexture;

    public void Init(Anchor anchor, int number)
    {
        base.Init(anchor, "Card/");

        videoPlayer = transform.Find("Card/VideoObject/Video Player").GetComponent<VideoPlayer>();
        videoSlider = transform.Find("Card/VideoObject/Video Player/Slider").GetComponent<Slider>();
        videoPlayButton = transform.Find("Card/VideoObject/Video Player/PlayButton").gameObject;
        videoPauseButton = transform.Find("Card/VideoObject/Video Player/PauseButton").gameObject;
        videoTexture = transform.Find("Card/VideoObject/Video Player").GetComponent<RawImage>();
        indexText = transform.Find("Index/Text").GetComponent<Text>();
        indexText.text = number.ToString();
    }
    public override IEnumerator DownloadContent()
    {
        yield return gameObject.AddComponent<VideoPlayerManager>().Init(videoPlayer, videoPlayButton, videoPauseButton, videoSlider, videoTexture
          , true, anchor.contentinfos[0].content.uri);
    }

}
