using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestionairePanel : MonoBehaviour
{
    private int questionOne = 0;
    private int questionTwo = 1;
    private int travelDurationType;

    public GameObject firstPage;
    public GameObject resultPage;
    public Text purposeOfTourText;
    public Text contentTypeText;

    private void OnEnable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == 0 ? true : false);
        }
    }

    public void BacktoLogin()
    {
        firstPage.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ToPurpose()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == 0 ? true : false);
        }
    }

    public void ToExperience1()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == 1 ? true : false);
        }
    }

    //public void ToExperience2()
    //{
    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        transform.GetChild(i).gameObject.SetActive(i == 2 ? true : false);
    //    }
    //}

    //public void ToExperience3()
    //{
    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        transform.GetChild(i).gameObject.SetActive(i == 3 ? true : false);
    //    }
    //}

    public void ToResultPage()
    {
        SetTourType();
        resultPage.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SetPurposeType(int type)
    {
        questionOne = type;
    }

    public void SetExperienceType(int type)
    {
        questionTwo = type;
    }

    public void SetTravelDurationType(int type)
    {
        travelDurationType = type;
    }

    public void SetTourType()
    {
        string text = "";
        if (questionOne == 1)
        {
            if (questionTwo == 1)
            {
                PlayerPrefs.SetString("tourtype",TourType.PURPOSEFUL.ToString());
                text = "목적형 문화관광";
                //contentTypeText.text = "심화 교육 콘텐츠";
            }
            else if (questionTwo == 2)
            {
                PlayerPrefs.SetString("tourtype", TourType.SIGHTSEEING.ToString());
                //AccountInfo.Instance.SetTourType(TourType.SIGHTSEEING);
                text = "유희형 문화관광";
                //contentTypeText.text = "심화 유희 콘텐츠";
            }
            else if (questionTwo == 3)
            {
                PlayerPrefs.SetString("tourtype", TourType.PURPOSEFUL.ToString() +","+ TourType.SIGHTSEEING.ToString());
                //AccountInfo.Instance.SetTourType(TourType.PURPOSEFUL, TourType.SIGHTSEEING);
                text = "목적형, 유희형 문화관광";
                //contentTypeText.text = "심화 교육, 심화 유희 콘텐츠";
            }

            
        }
        else if (questionOne == 2)
        {
            if (questionTwo == 1)
            {
                PlayerPrefs.SetString("tourtype", TourType.INCIDENTAL.ToString());
                //AccountInfo.Instance.SetTourType(TourType.INCIDENTAL);
                text = "우연형 문화관광";
                //contentTypeText.text = "기본 교육, 심화 교육 콘텐츠";
            }
            else if (questionTwo == 2)
            {
                PlayerPrefs.SetString("tourtype", TourType.CASUAL.ToString());
                //AccountInfo.Instance.SetTourType(TourType.CASUAL);
                text = "일반형 문화관광";
                //contentTypeText.text = "기본 유희 콘텐츠";
            }
            else if (questionTwo == 3)
            {
                PlayerPrefs.SetString("tourtype", TourType.INCIDENTAL.ToString() + "," + TourType.CASUAL.ToString());
                //AccountInfo.Instance.SetTourType(TourType.INCIDENTAL, TourType.CASUAL);
                text = "우연형, 일반형 문화관광";
                //contentTypeText.text = "기본 교육, 심화 교육, 기본 유희 콘텐츠";
            }
        }
        else if (questionOne == 3)
        {
            if (questionTwo == 1)
            {
                PlayerPrefs.SetString("tourtype", TourType.INCIDENTAL.ToString());
                //AccountInfo.Instance.SetTourType(TourType.INCIDENTAL);
                text = "우연형 문화관광";
                //contentTypeText.text = "기본 교육, 심화 교육 콘텐츠";
            }
            else if (questionTwo == 2)
            {
                PlayerPrefs.SetString("tourtype", TourType.SERENDIPITOUS.ToString());
                //AccountInfo.Instance.SetTourType(TourType.SERENDIPITOUS);
                text = "비목적형 문화관광";
                //contentTypeText.text = "";
            }
            else if (questionTwo == 3)
            {
                PlayerPrefs.SetString("tourtype", TourType.INCIDENTAL.ToString() + "," + TourType.SERENDIPITOUS.ToString());
                //AccountInfo.Instance.SetTourType(TourType.INCIDENTAL, TourType.SERENDIPITOUS);
                text = "우연형, 비목적형 문화관광";
                //contentTypeText.text = "기본 교육, 심화 교육 콘텐츠";
            }
        }

        purposeOfTourText.text = "당신의 투어 유형은 " + text + "입니다.";
        string contentType = "";
        foreach(var contentPreferenceInfo in AccountInfo.Instance.PreferenceContentInfos)
        {
            if(contentType.Length > 0)
            {
                contentType += ",";
            }
            contentType += contentPreferenceInfo.ToString();
        }

        PlayerPrefs.SetString("contenttype", contentType);
        contentTypeText.text = "이에 따라 " + contentType + "가 제공될 예정입니다.";
        PlayerPrefs.Save();
    }
}
