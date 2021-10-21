using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

using KCTM.Network.Data;

// Class: AccountInfo
// This class is used to determine user contentdepth and contenttype for filtering.
// Use this if you plan to use questionaire and determine usertype from it.
public class AccountInfo : MonoBehaviour
{
    public struct PreferenceContentInfo
    {
        public ContentDepth contentDepth;
        public ContentType contentType;

        public PreferenceContentInfo(ContentDepth contentDepth, ContentType contentType)
        {
            this.contentDepth = contentDepth;
            this.contentType = contentType;
        }

        public override string ToString()
        {
            string result = contentDepth == ContentDepth.BASIC ? "기본적인 " : "심화적인 ";

            switch(contentType)
            {
                case ContentType.EDUCATIONAL:
                    result += "교육";
                    break;
                case ContentType.RECREATIONAL:
                    result += "유희";
                    break;
                case ContentType.PRACTICAL:
                    result += "관리";
                    break;
            }

            return result;
        }
    }
    private static AccountInfo instance;


    private List<TourType> tourTypes;
    public ReadOnlyCollection<TourType> TourTypes
    {
        get
        {
            return tourTypes.AsReadOnly();
        }
    }
    private List<PreferenceContentInfo> preferenceContentInfos;
    public ReadOnlyCollection<PreferenceContentInfo> PreferenceContentInfos
    {
        get
        {
            return preferenceContentInfos.AsReadOnly();
        }
    }

    public User user;
    public List<User> userRelationships;

    public static AccountInfo Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find("AccountInfo");
                if (obj == null)
                {
                    obj = new GameObject("AccountInfo");

                    instance = obj.AddComponent<AccountInfo>();
                }
                else
                {
                    instance = obj.GetComponent<AccountInfo>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        tourTypes = new List<TourType>();
        preferenceContentInfos = new List<PreferenceContentInfo>();
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SetTourType(params TourType[] tourTypes)
    {
        this.tourTypes = new List<TourType>(tourTypes);
        foreach(TourType tourType in tourTypes)
        {
            switch(tourType)
            {
                case TourType.PURPOSEFUL:
                    preferenceContentInfos.Add(new PreferenceContentInfo(ContentDepth.DEEP, ContentType.EDUCATIONAL));
                    break;
                case TourType.SIGHTSEEING:
                    preferenceContentInfos.Add(new PreferenceContentInfo(ContentDepth.DEEP, ContentType.RECREATIONAL));
                    break;
                case TourType.INCIDENTAL:
                    preferenceContentInfos.Add(new PreferenceContentInfo(ContentDepth.BASIC, ContentType.EDUCATIONAL));
                    break;
                case TourType.CASUAL:
                    preferenceContentInfos.Add(new PreferenceContentInfo(ContentDepth.BASIC, ContentType.RECREATIONAL));
                    break;
                case TourType.SERENDIPITOUS:
                    preferenceContentInfos.Add(new PreferenceContentInfo(ContentDepth.BASIC, ContentType.RECREATIONAL));
                    break;
            }
        }
    }

    //private List<PreferenceContentInfo> GetPreferenceContentInfo()
    //{
    //    var result = new List<PreferenceContentInfo>();
        
    //    if (purposeType == 1)
    //    {
    //        if (experienceType == 1)
    //        {
    //            result.Add(new PreferenceContentInfo(ContentDepth.DEEP, ContentType.EDUCATIONAL));
    //        }
    //        else if (experienceType == 2)
    //        {
    //            result.Add(new PreferenceContentInfo(ContentDepth.DEEP, ContentType.RECREATIONAL));
    //        }
    //        else if (experienceType == 3)
    //        {
    //            result.Add(new PreferenceContentInfo(ContentDepth.DEEP, ContentType.EDUCATIONAL));
    //            result.Add(new PreferenceContentInfo(ContentDepth.DEEP, ContentType.RECREATIONAL));
    //        }
    //    }
    //    else if (purposeType == 2)
    //    {
    //        if (experienceType == 1)
    //        {
    //            result.Add(new PreferenceContentInfo(ContentDepth.BASIC, ContentType.EDUCATIONAL));
    //            result.Add(new PreferenceContentInfo(ContentDepth.DEEP, ContentType.EDUCATIONAL));
    //        }
    //        else if (experienceType == 2)
    //        {
    //            result.Add(new PreferenceContentInfo(ContentDepth.BASIC, ContentType.RECREATIONAL));
    //        }
    //        else if (experienceType == 3)
    //        {
    //            result.Add(new PreferenceContentInfo(ContentDepth.BASIC, ContentType.EDUCATIONAL));
    //            result.Add(new PreferenceContentInfo(ContentDepth.BASIC, ContentType.RECREATIONAL));
    //            result.Add(new PreferenceContentInfo(ContentDepth.DEEP, ContentType.EDUCATIONAL));
    //        }
    //    }
    //    else if (purposeType == 3)
    //    {
    //        if (experienceType == 1)
    //        {
    //            result.Add(new PreferenceContentInfo(ContentDepth.BASIC, ContentType.EDUCATIONAL));
    //        }
    //        else if (experienceType == 2)
    //        {
    //            result.Add(new PreferenceContentInfo(ContentDepth.BASIC, ContentType.RECREATIONAL));
    //        }
    //        else if (experienceType == 3)
    //        {
    //            result.Add(new PreferenceContentInfo(ContentDepth.BASIC, ContentType.EDUCATIONAL));
    //            result.Add(new PreferenceContentInfo(ContentDepth.BASIC, ContentType.RECREATIONAL));
    //        }
    //    }

    //    return result;
    //}
}
