using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KCTM.Network;
using KCTM.Network.Data;
using Newtonsoft.Json;

public class AuthoringDictionary : MonoBehaviour
{
    public Dictionary<string, long> prefabDictionary;
    public List<string> coverImageDictionary;
    public Dictionary<string, long> emojiDictionary;
    public Dictionary<long, Content> idDictionary;
 
	public GameObject[] allPrefabs;
	public Texture2D[] allCoverImages;
	public Texture2D[] allEmojiImages;

    private TextAsset jsonFile;

	private static AuthoringDictionary instance;
    
    public static AuthoringDictionary Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find("AuthoringDictionary");
                if (obj == null)
                {
                    obj = new GameObject("AuthoringDictionary");

                    instance = obj.AddComponent<AuthoringDictionary>();
                }
                else
                {
                    instance = obj.GetComponent<AuthoringDictionary>();
                }



            }

            return instance;
        }
    }

    private void Awake()
    {

        prefabDictionary = new Dictionary<string, long>();
        coverImageDictionary = new List<string>();
        emojiDictionary = new Dictionary<string, long>();
        idDictionary = new Dictionary<long, Content>();
        jsonFile = new TextAsset();

        allPrefabs = Resources.LoadAll<GameObject>("Models/DefaultAssets");
        allCoverImages = Resources.LoadAll<Texture2D>("Models/DefaultImages");
        allEmojiImages = Resources.LoadAll<Texture2D>("Models/emojiAssets");
        jsonFile = Resources.Load<TextAsset>("AssetDB/assetContent");

        List<Content> jsonContents = JsonConvert.DeserializeObject<List<Content>>(jsonFile.text);



        for (int i = 0; i < jsonContents.Count; i++)
        {
            idDictionary.Add(jsonContents[i].id, jsonContents[i]);
            int idx = jsonContents[i].filename.LastIndexOf('.');

            if (idx > 0)
            {
                string fileExtension = jsonContents[i].filename.Substring(idx);

                if (Util.IsitImage(fileExtension))
                    emojiDictionary.Add(jsonContents[i].filename.Substring(0, idx), jsonContents[i].id);
                else if (Util.IsitModel(fileExtension))
                    prefabDictionary.Add(jsonContents[i].filename.Substring(0, idx), jsonContents[i].id);
            }
        }

        for (int i = 0; i < allCoverImages.Length; i++)
        {
            coverImageDictionary.Add(allCoverImages[i].name);
        }
    }


    private void Start()
    {
        DontDestroyOnLoad(this);
    }

}
