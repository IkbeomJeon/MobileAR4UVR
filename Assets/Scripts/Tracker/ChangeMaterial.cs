using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    bool enabled = false;
    bool pressed = false;

    public Color mainColor= new Color(0, 120/255.0f, 255/255, 0.5f);
    public Color outlineColor = new Color(255/255, 255/255, 255/255, 0);

    [Range(0, 1f)]
    public float alpha = 0.38f;

    [Range(0, 0.15f)]
    public float outlineWidth = 0f;


    [Range(0, 3)]
    public float fadeTime = 1.5f;

    public float maxiumAlpha = 0.4f;
    public MeshRenderer[] meshRenderers;
    float startTime;

    private void Awake()
    {
        if (enabled)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        GetRendererFromChildren();
        //SetMaterial(Shader.Find("Somian/Unlit/Transparent"));
        SetAlphaOutlineShader();
    }
    private void Update()
    {
        if(pressed)
        {
            float spent = Time.time - startTime;
            float _alpha = Mathf.Lerp(0, 0.35f, spent/fadeTime);
            SetAlpha(_alpha);
        }

    }
    public void SetAlpha(float alpha)
    {
        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.sharedMaterial.SetFloat("_Alpha", alpha);
        }  
    }
    public void SetColor(Color main, Color outline)
    {
        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.sharedMaterial.SetColor("_Color", main);
            mr.sharedMaterial.SetColor("_OutlineColor", outline);
        }
    }
    public void SetWidth(float width)
    {
        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.sharedMaterial.SetFloat("_Outline", width);
        }
    }

    public void GetRendererFromChildren()
    {
        meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
    }

    public void SetAlphaOutlineShader()
    {
        foreach (MeshRenderer mr in meshRenderers)
        {
            //Material newMat = new Material(Shader.Find("Screw/Alpha Outline"))
            Material newMat = new Material(Shader.Find("Somian/Unlit/Transparent"))
            //Material newMat = new Material(Shader.Find("Standard"))
            {
                mainTexture = mr.sharedMaterial.mainTexture,
                color = mainColor
            };
            newMat.SetColor("_OutlineColor", outlineColor);
            newMat.SetFloat("_Alpha", alpha);
            newMat.SetFloat("_Outline", outlineWidth);
            mr.sharedMaterial = newMat;
        }
    }

    public void PointerDown()
    {
        pressed = true;
        startTime = Time.time;
    }
    public void PointUp()
    { 
        pressed = false;
        SetAlpha(0);
    }

    public void ButtonClick()
    {
        enabled = !enabled;
        gameObject.SetActive(enabled);
    }
}
