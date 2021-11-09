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
    
    private RenderTexture renderTexture;
    public VideoPlayer videoPlayer;
    public GameObject videoPlayButton;
    public GameObject videoPauseButton;
    public Slider videoSlider;


    public void Init(Anchor anchor,int number)
    {
        base.Init(anchor, "Card/");

        videoPlayer = transform.Find("Card/VideoObject/Video Player").GetComponent<VideoPlayer> ();
        videoSlider = transform.Find("Card/VideoObject/Video Player/Slider").GetComponent<Slider>();
        videoPlayButton = transform.Find("Card/VideoObject/Video Player/PlayButton").gameObject;
        videoPauseButton = transform.Find("Card/VideoObject/Video Player/PauseButton").gameObject;

        indexText = transform.Find("Index/Text").GetComponent<Text>();
        indexText.text = number.ToString();

        videoPlayButton.SetActive(false);
        videoPauseButton.SetActive(false);

        Button btn_play = videoPlayButton.GetComponent<Button>();
        Button btn_pause = videoPauseButton.GetComponent<Button>();

        btn_play.onClick.AddListener(delegate { PlayVideo(); });
        btn_pause.onClick.AddListener(delegate { PauseVideo(); });
    }

    public override void DownloadContent()
    {
        base.DownloadContent();

        StartCoroutine(VideoPlayerInit());
       
    }

    IEnumerator VideoPlayerInit()
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.url = anchor.contentinfos[0].content.uri;
        videoPlayer.isLooping = true;
        videoPlayer.errorReceived += ErrorReceived;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared == true);

        videoPlayer.time = 1f;

        int videoWidth = videoPlayer.texture.width;
        int videoHeight = videoPlayer.texture.height;

        videoPlayer.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = (float)videoWidth / videoHeight;

        ////tex.sprite = Sprite.Create(preview, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        renderTexture = new RenderTexture(videoWidth, videoHeight, 24, RenderTextureFormat.ARGB32);
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.SetDirectAudioMute(0, true);
        videoPlayer.Play();
        yield return null;
        //yield return new WaitForSeconds(1.5f);

        //Texture2D preview = new Texture2D(videoWidth, videoHeight, TextureFormat.RGB24, false);
        RenderTexture.active = videoPlayer.targetTexture;
        //preview.ReadPixels(new Rect(0, 0, videoWidth, videoHeight), 0, 0);
        //preview.Apply();
        videoPlayer.Pause();
        videoPlayer.time = 0;
        videoPlayer.SetDirectAudioMute(0, false);
        //RenderTexture.active = null;

        //scale = (float)videoWidth / videoHeight;
        videoPlayer.gameObject.GetComponent<RawImage>().texture = videoPlayer.targetTexture;

        StartCoroutine(VideoSliderUpdator());

        videoPlayButton.SetActive(true);
        videoPauseButton.SetActive(false);
    }
    IEnumerator VideoSliderUpdator()
    {
        while (!videoPlayer.isPrepared)
            yield return new WaitForSeconds(1f);

        videoSlider.direction = Slider.Direction.LeftToRight;
        videoSlider.minValue = 0;
        videoSlider.maxValue = videoPlayer.frameCount / videoPlayer.frameRate;

        while (true)
        {
            videoSlider.value = (float)videoPlayer.time;
            yield return new WaitForFixedUpdate();
        }
    }

    private void ErrorReceived(VideoPlayer source, string message)
    {
        //description.text = "Error, " + message;
        videoPlayer.errorReceived -= ErrorReceived;//Unregister to avoid memory leaks
        videoPlayButton.SetActive(false);
        videoPauseButton.SetActive(false);
        videoPlayer.enabled = false;

        Debug.LogError("error in: " + anchor.id);
    }

    public void VideoSliderChange()
    {
        videoPlayer.time = videoSlider.value;
    }

    public void PlayVideo()
    {
        videoPlayer.Play();
        videoPlayButton.SetActive(false);
        videoPauseButton.SetActive(true);

        StartCoroutine(FadeButton(videoPauseButton, true));
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
        videoPlayButton.SetActive(true);
        videoPauseButton.SetActive(false);

        StartCoroutine(FadeButton(videoPlayButton, true));
    }


    IEnumerator FadeButton(GameObject button, bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                button.GetComponent<Image>().color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                button.GetComponent<Image>().color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }
}
