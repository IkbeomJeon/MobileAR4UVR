using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationManager : MonoBehaviour
{

    public float size_anchors = 1f; //meter
    public float height_user = 1.5f; //meter
    public float height_anchors = 1.5f; //meter
    public float height_navigation_3d = 0.3f;// meter
    public float stepsize_position = 1; //metric.
    public float stepsize_rotation = 1f; //degree.
    public float distance_visible_anchor = 30; //metric.
    public float distance_to_remove_midle_waypoint = 20;
    public int use_terrain_height = 0;
    public int use_anchors_height = 1;


    float default_size_anchors = 1f; //meter
    float default_height_user = 1.5f; //meter
    float default_height_anchors = 1.5f; //meter
    float default_height_navigation_3d = 0.3f;// meter
    float default_stepsize_position = 1; //metric.
    float default_stepsize_rotation = 1f; //degree.
    float default_distance_visible_anchor = 30; //metric.
    float default_distance_to_remove_midle_waypoint = 20;

    int default_use_terrain_height = 0;
    int default_use_anchors_height = 1;

    private void Awake()
    {
        instance = this;
        Load();
    }


    private static ConfigurationManager instance;

    public static ConfigurationManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find("ConfigurationManager");
                if (obj == null)
                {
                    obj = new GameObject("ConfigurationManager");
                    instance = obj.AddComponent<ConfigurationManager>();
                }
                else
                {
                    instance = obj.GetComponent<ConfigurationManager>();
                }
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }

    public void Load()
    {

        if (PlayerPrefs.HasKey("size_anchors"))
            size_anchors = PlayerPrefs.GetFloat("size_anchors");
        else
            size_anchors = default_size_anchors;

        if (PlayerPrefs.HasKey("height_user"))
            height_user = PlayerPrefs.GetFloat("height_user");
        else
            height_user = default_height_user;

        if (PlayerPrefs.HasKey("height_anchors"))
            height_anchors = PlayerPrefs.GetFloat("height_anchors");
        else
            height_anchors = default_height_anchors;

        if (PlayerPrefs.HasKey("height_navigation_3d"))
            height_navigation_3d = PlayerPrefs.GetFloat("height_navigation_3d");
        else
            height_navigation_3d = default_height_navigation_3d;

        if (PlayerPrefs.HasKey("stepsize_position"))
            stepsize_position = PlayerPrefs.GetFloat("stepsize_position");
        else
            stepsize_position = default_stepsize_position;

        if (PlayerPrefs.HasKey("stepsize_rotation"))
            stepsize_rotation = PlayerPrefs.GetFloat("stepsize_rotation");
        else
            stepsize_rotation = default_stepsize_rotation;

        if (PlayerPrefs.HasKey("distance_visible_anchor"))
            distance_visible_anchor = PlayerPrefs.GetFloat("distance_visible_anchor");
        else
            distance_visible_anchor = default_distance_visible_anchor;

        if (PlayerPrefs.HasKey("distance_to_remove_midle_waypoint"))
            distance_to_remove_midle_waypoint = PlayerPrefs.GetFloat("distance_to_remove_midle_waypoint");
        else
            distance_to_remove_midle_waypoint = default_distance_to_remove_midle_waypoint;


        if (PlayerPrefs.HasKey("use_terrain_height"))
            use_terrain_height = PlayerPrefs.GetInt("use_terrain_height");
        else
            use_terrain_height = default_use_terrain_height;

        if (PlayerPrefs.HasKey("use_anchors_height"))
            use_anchors_height = PlayerPrefs.GetInt("use_anchors_height");
        else
            use_anchors_height = default_use_anchors_height;

    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey("size_anchors");
        PlayerPrefs.DeleteKey("height_user");
        PlayerPrefs.DeleteKey("height_anchors");
        PlayerPrefs.DeleteKey("height_navigation_3d");
        PlayerPrefs.DeleteKey("stepsize_position");
        PlayerPrefs.DeleteKey("stepsize_rotation");
        PlayerPrefs.DeleteKey("distance_visible_anchor");
        PlayerPrefs.DeleteKey("distance_to_remove_midle_waypoint");
        PlayerPrefs.DeleteKey("use_terrain_height");
        PlayerPrefs.DeleteKey("use_anchors_height");
    }
    public void Save(float size_anchors, float height_user, float height_anchors, float height_navigation_3d, float stepsize_position, float stepsize_rotation, float distance_visible_anchor,
        float distance_to_remove_midle_waypoint, int use_terrain_height, int use_anchors_height)
    {
        PlayerPrefs.SetFloat("size_anchors", size_anchors);
        PlayerPrefs.SetFloat("height_user", height_user);
        PlayerPrefs.SetFloat("height_anchors", height_anchors);
        PlayerPrefs.SetFloat("height_navigation_3d", height_navigation_3d);
        PlayerPrefs.SetFloat("stepsize_position", stepsize_position);
        PlayerPrefs.SetFloat("stepsize_rotation", stepsize_rotation);
        PlayerPrefs.SetFloat("distance_visible_anchor", distance_visible_anchor);
        PlayerPrefs.SetFloat("distance_to_remove_midle_waypoint", distance_to_remove_midle_waypoint);
        PlayerPrefs.SetInt("use_terrain_height", use_terrain_height);
        PlayerPrefs.SetInt("use_anchors_height", use_anchors_height);

        PlayerPrefs.Save();
    }
}
