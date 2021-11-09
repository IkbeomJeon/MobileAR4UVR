using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoPlayButton;
    public GameObject videoPauseButton;
    public Slider videoSlider;
    public RawImage videoTexture;
    public bool doInitVideoPlayer;
    public string url; 

    public IEnumerator Init(VideoPlayer videoPlayer, GameObject videoPlayButton, GameObject videoPauseButton, Slider videoSlider, RawImage videoTexture, bool doInitVideoPlayer=false, string url="")
    {
        this.videoPlayer = videoPlayer;
        this.videoPlayButton = videoPlayButton;
        this.videoPauseButton = videoPauseButton;
        this.videoSlider = videoSlider;
        this.videoTexture = videoTexture;

        Button btn_play = videoPlayButton.GetComponent<Button>();
        Button btn_pause = videoPauseButton.GetComponent<Button>();

        btn_play.onClick.AddListener(delegate { PlayVideo(); });
        btn_pause.onClick.AddListener(delegate { PauseVideo(); });

        videoPlayButton.SetActive(false);
        videoPauseButton.SetActive(false);

        this.doInitVideoPlayer = doInitVideoPlayer;
        this.url = url;

        
        if (doInitVideoPlayer)
        {
            yield return VideoPlayerInit(url);
        }

        videoTexture.texture = videoPlayer.texture;

        videoPlayButton.SetActive(true);
        videoPauseButton.SetActive(false);

        yield return VideoSliderUpdator();
    }
 


    public IEnumerator VideoPlayerInit(string url)
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.url = url;
        videoPlayer.isLooping = true;
        videoPlayer.errorReceived += ErrorReceived;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared == true);

        videoPlayer.time = 1f;

        int videoWidth = videoPlayer.texture.width;
        int videoHeight = videoPlayer.texture.height;

        //videoPlayer.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = (float)videoWidth / videoHeight;

        videoPlayer.SetDirectAudioMute(0, true);
        videoPlayer.Play();
        yield return null;

        RenderTexture renderTexture = new RenderTexture(videoWidth, videoHeight, 24, RenderTextureFormat.ARGB32);
        videoPlayer.targetTexture = renderTexture;

        RenderTexture.active = videoPlayer.targetTexture;
        videoPlayer.Pause();
        videoPlayer.time = 0;
        videoPlayer.SetDirectAudioMute(0, false);
        //RenderTexture.active = null;
     

       
    }

    public IEnumerator VideoSliderUpdator()
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

            if(videoPlayer.isPlaying)
            {
                videoPlayButton.SetActive(false);
                videoPauseButton.SetActive(true);
            }
            else
            {
                videoPlayButton.SetActive(true);
                videoPauseButton.SetActive(false);
            }
        }
    }

    private void ErrorReceived(VideoPlayer source, string message)
    {
        //description.text = "Error, " + message;
        videoPlayer.errorReceived -= ErrorReceived;//Unregister to avoid memory leaks
        videoPlayButton.SetActive(false);
        videoPauseButton.SetActive(false);
        videoPlayer.enabled = false;

        Debug.LogError("error in: " + message);
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
