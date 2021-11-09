using KCTM.Network;
using KCTM.Network.Data;
using Mapbox.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoCard_Preview : PreviewCard
{
    //public VideoPlayerManager videoPlayerManager;
    
    public VideoPlayer videoPlayer;
    public GameObject videoPlayButton;
    public GameObject videoPauseButton;
    public Slider videoSlider;
    public RawImage videoTexture;

    // Start is called before the first frame update
    public override void Init(Anchor anchor, bool isStory, int index_story)
    {
        base.Init(anchor, isStory, index_story);

        videoPlayer = transform.Find("Card/VideoObject/Video Player").GetComponent<VideoPlayer>();
        videoPauseButton = transform.Find("Card/VideoObject/Video Player/PauseButton").gameObject;
        videoPlayButton = transform.Find("Card/VideoObject/Video Player/PlayButton").gameObject;
        videoSlider = transform.Find("Card/VideoObject/Video Player/Slider").GetComponent<Slider>();
        videoTexture = transform.Find("Card/VideoObject/Video Player").GetComponent<RawImage>();

        string url = string.Format("/arscene?id={0}&secret=false", anchor.id);
        NetworkManager.Instance.Get(url, GetARScene, FailHandler);
    }

  
    /*
     * Function: GetARScene
     *
     * Details:
     * - Callback function for getting arscene.
     * - Once received arscene, save and set preview card.
     */
    private void GetARScene(Result result)
    {
        var anchor = JsonConvert.DeserializeObject<Anchor>(result.result.ToString());
        base.anchor = anchor;

        likeNumber.text = string.Format("¡¡æ∆ø‰ : {0}", anchor.likes.Count.ToString());
        StartCoroutine(gameObject.AddComponent<VideoPlayerManager>().Init(videoPlayer, videoPlayButton, videoPauseButton, videoSlider, videoTexture
            , true, anchor.contentinfos[0].content.uri));
    }

}
