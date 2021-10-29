using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetConfigurationValue : MonoBehaviour
{

    public TMP_InputField size_anchors;
    public TMP_InputField height_user;
    public TMP_InputField height_anchors;
    public TMP_InputField height_navigation_3d;
    public TMP_InputField stepsize_position;
    public TMP_InputField stepsize_rotation;
    public TMP_InputField distance_visible_anchor;
    public Toggle use_terrain_height;

    // Start is called before the first frame update
    void Start()
    {
        
        size_anchors = transform.Find("size_anchors/InputField (TMP)").GetComponent<TMP_InputField>();
        height_user = transform.Find("height_user/InputField (TMP)").GetComponent<TMP_InputField>();
        height_anchors = transform.Find("height_anchors/InputField (TMP)").GetComponent<TMP_InputField>();
        height_navigation_3d = transform.Find("height_navigation_3d/InputField (TMP)").GetComponent<TMP_InputField>();
        stepsize_position = transform.Find("stepsize_position/InputField (TMP)").GetComponent<TMP_InputField>();
        stepsize_rotation = transform.Find("stepsize_rotation/InputField (TMP)").GetComponent<TMP_InputField>();
        distance_visible_anchor = transform.Find("distance_visible_anchor/InputField (TMP)").GetComponent<TMP_InputField>();
        use_terrain_height = transform.Find("Toggle_terrain").GetComponent<Toggle>();

        Load();
    }
    public void Load()
    {
        ConfigurationManager.Instance.Load();

        size_anchors.text = ConfigurationManager.Instance.size_anchors.ToString();
        height_user.text = ConfigurationManager.Instance.height_user.ToString();
        height_anchors.text = ConfigurationManager.Instance.height_anchors.ToString();
        height_navigation_3d.text = ConfigurationManager.Instance.height_navigation_3d.ToString();
        stepsize_position.text = ConfigurationManager.Instance.stepsize_position.ToString();
        stepsize_rotation.text = ConfigurationManager.Instance.stepsize_rotation.ToString();
        distance_visible_anchor.text = ConfigurationManager.Instance.distance_visible_anchor.ToString();
        use_terrain_height.isOn = ConfigurationManager.Instance.use_terrain_height == 1 ? true : false;
    }
    public void Save()
    {
        ConfigurationManager.Instance.Save(float.Parse(size_anchors.text), float.Parse(height_user.text), float.Parse(height_anchors.text),
            float.Parse(height_navigation_3d.text), float.Parse(stepsize_position.text), float.Parse(stepsize_rotation.text), float.Parse(distance_visible_anchor.text)
            , use_terrain_height.isOn?1:0);

        Load();

        var arScenesParent = GameObject.Find("ARSceneParent").transform;
        var arScenesParent_poi = GameObject.Find("ARSceneParent_Target").transform;
        var recommendedParent = GameObject.Find("RecommendedParent").transform;

        arScenesParent.localPosition = new Vector3(0, float.Parse(height_anchors.text), 0);
        arScenesParent_poi.localPosition = new Vector3(0, float.Parse(height_anchors.text), 0);
        recommendedParent.localPosition = new Vector3(0, float.Parse(height_anchors.text), 0);
    }
    public void ResetAll()
    {
        ConfigurationManager.Instance.Reset();
        Load();

    }
}
