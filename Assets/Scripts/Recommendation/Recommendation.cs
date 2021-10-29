using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KCTM.Network.Data;
using KCTM.Recommendation;
using Mapbox.Examples;
using Mapbox.Utils;
using ARRC_DigitalTwin_Generator;
using KCTM.Network;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;
using System.Linq;

public class Recommendation : MonoBehaviour
{
    //added by jeon.
    //public float default_height = 1.5f;


    public GameObject navOptions;
    
    //public GameObject direction;
    //public GameObject poiPrefab;
    public GameObject recommendedParent;

    public List<Anchor> anchorList;
    public List<Anchor> recomList;

    private List<VisitedContent> latestVisistedContents;
    private List<VisitedContent> allVisistedContents;
    private Dictionary<long, StoringInfo> userhistory;
    //private Dictionary<long, Anchor> userHistoryAnchor;
    private double maxUserFactor;
    private List<String> storyPlaceTags;

    private Dictionary<string, List<Token>> anchorTokens;

    public int recommendationType;

    // Start is called before the first frame update
    void Start()
    {
        maxUserFactor = 0.0;
        recommendationType = 1;
        Dictionary<long, Anchor> userHistoryAnchor = new Dictionary<long, Anchor>();
        
        //postRequest = new PostRequest();
        latestVisistedContents = new List<VisitedContent>();
        allVisistedContents = new List<VisitedContent>();
        
        getAnchorTokens();
        Debug.Log("My Recommendation --- Maryam");

        //Added by Ikbeom.
        navOptions = GameObject.Find("MapManager");
        recommendedParent = GameObject.Find("RecommendedParent");
    }

    public void addToVisitedList(VisitedContent visitedContent)
    {

        //List<Anchor> stories = navOptions.GetComponent<NavOptions>().stories;
        //int index = navOptions.GetComponent<NavOptions>().spaceTellingIndex;

        //Added by Ikbeom.
        List<Anchor> stories = navOptions.GetComponent<MapManager>().stories;
        int index = navOptions.GetComponent<MapManager>().spaceTellingIndex;

        if (stories.Count == 0)
        {
            return;
        }
        
        StartCoroutine(Upload(new StoringInfo(visitedContent)));

        //if (visitedContent.userVisitedTime < 1)
        //{
        //    return;
        //}

        Debug.Log("Recommendation size " + recomList.Count);

        int insertedIndex = checkAlreadyVisited(visitedContent);
        if (insertedIndex == -1)
        {

            Anchor arScene = visitedContent.anchor;

            Debug.Log("My addToVisitedList " + arScene.id);
            //Debug.Log("Trajectory length " + Recom.trajectory.Count);

            //visitedContent.calInteractionFactors();
            //visitedContent.calBehaviorFactor(stories[index], recomList);

            double weight = visitedContent.calFactors(stories[index], recomList);

            if(weight > maxUserFactor)
            {
                maxUserFactor = weight;
            }

            latestVisistedContents.Add(visitedContent);
            allVisistedContents.Add(visitedContent);
        }
        else
        {
            checkVisitedContent(insertedIndex, visitedContent);
            Debug.Log("It was already visited");

        }

        if (latestVisistedContents.Count > 2)
        {
            Debug.Log("Size is two");
        }
    }

    private int checkAlreadyVisited(VisitedContent visitedContent)
    {
        int visitedIndex = -1;

        for (int i = 0; i < latestVisistedContents.Count; i++)
        {
            if (latestVisistedContents[i].anchor.id == visitedContent.anchor.id)
            {
                visitedIndex = i;
                break;
            }
        }

        return visitedIndex;
    }

    private void checkVisitedContent(int insertedIndex, VisitedContent visitedContent)
    {
        double totalTime = latestVisistedContents[insertedIndex].userVisitedTime + visitedContent.userVisitedTime;
        latestVisistedContents[insertedIndex].userVisitedTime = totalTime;
        latestVisistedContents[insertedIndex].calInteractionFactors();
    }

    public void loadUserHistory()
    {
        if (recommendationType == 2 || recommendationType == 3)
        {
            StartCoroutine(LoadUserVisitedContent());
        }
    }

    public void additionalRecommendation()
    {

        getStoryPlaceTags();

        switch (recommendationType)
        {
            case 1:
                realTimeRecommendation();
                break;
            case 2:
                historyRateRecommendation();
                break;
            case 3:
                hybridRateRecommendation();
                break;
        }

    }

    private void realTimeRecommendation()
    {

        //List<Anchor> stories = navOptions.GetComponent<NavOptions>().stories;
        //int index = navOptions.GetComponent<NavOptions>().spaceTellingIndex;

        //Added by Ikbeom.
        List<Anchor> stories = navOptions.GetComponent<MapManager>().stories;
        int index = navOptions.GetComponent<MapManager>().spaceTellingIndex;

        if (stories.Count == 0)
        {
            return;
        }
        if(index == stories.Count - 1)
        {
            return;
        }

        Anchor currentFixed = stories[index];
        Anchor nextFixed = stories[index + 1];
        List<double> boundingBox = getBoundingBox(currentFixed.point, nextFixed.point);
        List<double> recomPosition = getMidddle(currentFixed.point, nextFixed.point);

        String anchor_trajectory_ids = currentFixed.id.ToString() + "-" + nextFixed.id.ToString();
        List<AnchorRecom> anchor_recoms = new List<AnchorRecom>();

        int recomIndex = -1;
        double recomScore = -1;

        GameObject ars = GameObject.Find("Real World/ARSceneParent");
        
        for (int i = 0; i < ars.transform.childCount; i++)
        {
            Anchor arScene = getAnchor(ars, i);
            if (isInBoundingBox(boundingBox, arScene) && isNotFixedRecomVisitedContensts(arScene) && isNotInTrajectoryPoIs(arScene, currentFixed, nextFixed))
            {

                double sumScores = 0.0;
                double sumSimilarities = 0.0;

                for (int j = 0; j < allVisistedContents.Count; j++)
                {
                    double s = getSimilarity(arScene, allVisistedContents[j].anchor);
                    sumScores += (s * (allVisistedContents[j].getNormalizedWeight(maxUserFactor)));
                    sumSimilarities += s;

                    //Debug.Log("Maryam Similarity: " + s);
                    //Debug.Log("Maryam Similarity: " + sumScores);
                }

                double score = 0.0;
                if (sumScores != 0)
                {
                    score = sumScores / sumSimilarities;
                }

                if (score > recomScore)
                {
                    recomIndex = i;
                    recomScore = score;
                }

                AnchorRecom anchorRecom = new AnchorRecom();
                anchorRecom.anchor_id = arScene.id;
                anchorRecom.weight = score;

                anchor_recoms.Add(anchorRecom);
            }
        }

        Transition transition = new Transition();
        transition.anchor_trajectory_ids = anchor_trajectory_ids;
        //transition.user_id = AccountInfo.Instance.user.id;
        transition.user_id = PlayerPrefs.GetString("email");
        transition.anchor_recoms = anchor_recoms;
        transition.recomType = recommendationType;

        StartCoroutine(StoreTransition(transition));

        if (recomIndex != -1)
        {
            setRecommendation(ars, recomIndex, recomPosition);
        }
    }

    private void historyRecommendation()
    {
        //List<Anchor> stories = navOptions.GetComponent<NavOptions>().stories;
        //int index = navOptions.GetComponent<NavOptions>().spaceTellingIndex;

        //Added by Ikbeom.
        List<Anchor> stories = navOptions.GetComponent<MapManager>().stories;
        int index = navOptions.GetComponent<MapManager>().spaceTellingIndex;

        if (stories.Count == 0)
        {
            return;
        }
        if (index == stories.Count - 1)
        {
            return;
        }

        Anchor currentFixed = stories[index];
        Anchor nextFixed = stories[index + 1];
        List<double> boundingBox = getBoundingBox(currentFixed.point, nextFixed.point);
        List<double> recomPosition = getMidddle(currentFixed.point, nextFixed.point);

        String anchor_trajectory_ids = currentFixed.id.ToString() + "-" + nextFixed.id.ToString();
        List<AnchorRecom> anchor_recoms = new List<AnchorRecom>();

        int recomIndex = -1;
        double recomScore = -1;

        GameObject ars = GameObject.Find("Real World/ARSceneParent");

        for (int i = 0; i < ars.transform.childCount; i++)
        {

            Anchor arScene = getAnchor(ars, i);
            if (isInBoundingBox(boundingBox, arScene) && isNotFixedRecomVisitedContensts(arScene) && isNotInTrajectoryPoIs(arScene, currentFixed, nextFixed))
            {
                Debug.Log("AR scene: " + arScene.id);

                double sumScores = 0.0;
                foreach (var anchor_id in userhistory.Keys)
                {

                    double s = getSimilarity(arScene, userhistory[anchor_id]);
                    sumScores += s;
                }

                double score = 0.0;
                if (sumScores != 0)
                {
                    score = sumScores / userhistory.Count;
                }

                if (score > recomScore)
                {
                    recomIndex = i;
                    recomScore = score;
                }

                AnchorRecom anchorRecom = new AnchorRecom();
                anchorRecom.anchor_id = arScene.id;
                anchorRecom.weight = score;

                anchor_recoms.Add(anchorRecom);
            }
        }

        Transition transition = new Transition();
        transition.anchor_trajectory_ids = anchor_trajectory_ids;
        //transition.user_id = AccountInfo.Instance.user.id;
        transition.user_id = PlayerPrefs.GetString("email");
        transition.anchor_recoms = anchor_recoms;
        transition.recomType = recommendationType;

        StartCoroutine(StoreTransition(transition));

        if (recomIndex != -1)
        {
            setRecommendation(ars, recomIndex, recomPosition);
        }
    }

    private void historyRateRecommendation()
    {
        //List<Anchor> stories = navOptions.GetComponent<NavOptions>().stories;
        //int index = navOptions.GetComponent<NavOptions>().spaceTellingIndex;

        //Added by Ikbeom.
        List<Anchor> stories = navOptions.GetComponent<MapManager>().stories;
        int index = navOptions.GetComponent<MapManager>().spaceTellingIndex;

        if (stories.Count == 0)
        {
            return;
        }
        if (index == stories.Count - 1)
        {
            return;
        }

        Anchor currentFixed = stories[index];
        Anchor nextFixed = stories[index + 1];
        String anchor_trajectory_ids = currentFixed.id.ToString() + "-" + nextFixed.id.ToString();
        List<AnchorRecom> anchor_recoms = new List<AnchorRecom>();


        List<double> boundingBox = getBoundingBox(currentFixed.point, nextFixed.point);
        //List<double> recomPosition = getMidddle(currentFixed.point, nextFixed.point);
        List<double> recomPosition = getMidddle(anchor_trajectory_ids);

        int recomIndex = -1;
        double recomScore = -1;

        GameObject ars = GameObject.Find("Real World/ARSceneParent");

        for (int i = 0; i < ars.transform.childCount; i++)
        {

            Anchor arScene = getAnchor(ars, i);
            if (isInBoundingBox(boundingBox, arScene) && isNotFixedRecomVisitedContensts(arScene) && isNotInTrajectoryPoIs(arScene, currentFixed, nextFixed))
            {
                Debug.Log("AR scene: " + arScene.id);

                double sumScores = 0.0;
                double sumSimilarities = 0.0;
                foreach (var anchor_id in userhistory.Keys)
                {

                    double s = getSimilarity(arScene, userhistory[anchor_id]);
                    sumScores += s * userhistory[anchor_id].user_like;
                    sumSimilarities += s;
                }

                double score = 0.0;
                if (sumScores != 0)
                {
                    score = sumScores / sumSimilarities;
                }

                if (score > recomScore)
                {
                    recomIndex = i;
                    recomScore = score;
                }

                AnchorRecom anchorRecom = new AnchorRecom();
                anchorRecom.anchor_id = arScene.id;
                anchorRecom.weight = score;

                anchor_recoms.Add(anchorRecom);
            }
        }

        Transition transition = new Transition();
        transition.anchor_trajectory_ids = anchor_trajectory_ids;
        //transition.user_id = AccountInfo.Instance.user.id;
        transition.user_id = PlayerPrefs.GetString("email");
        transition.anchor_recoms = anchor_recoms;
        transition.recomType = recommendationType;

        StartCoroutine(StoreTransition(transition));

        if (recomIndex != -1)
        {
            setRecommendation(ars, recomIndex, recomPosition);
        }
    }

    private void hybridRecommendation()
    {
        //List<Anchor> stories = navOptions.GetComponent<NavOptions>().stories;
        //int index = navOptions.GetComponent<NavOptions>().spaceTellingIndex;

        //Added by Ikbeom.
        List<Anchor> stories = navOptions.GetComponent<MapManager>().stories;
        int index = navOptions.GetComponent<MapManager>().spaceTellingIndex;

        if (stories.Count == 0)
        {
            return;
        }
        if (index == stories.Count - 1)
        {
            return;
        }

        Anchor currentFixed = stories[index];
        Anchor nextFixed = stories[index + 1];
        List<double> boundingBox = getBoundingBox(currentFixed.point, nextFixed.point);
        List<double> recomPosition = getMidddle(currentFixed.point, nextFixed.point);

        String anchor_trajectory_ids = currentFixed.id.ToString() + "-" + nextFixed.id.ToString();
        List<AnchorRecom> anchor_recoms = new List<AnchorRecom>();

        int recomIndex = -1;
        double recomScore = -1;

        GameObject ars = GameObject.Find("Real World/ARSceneParent");

        for (int i = 0; i < ars.transform.childCount; i++)
        {
            Anchor arScene = getAnchor(ars, i);
            if (isInBoundingBox(boundingBox, arScene) && isNotFixedRecomVisitedContensts(arScene) && isNotInTrajectoryPoIs(arScene, currentFixed, nextFixed))
            {
                Debug.Log("AR scene: " + arScene.id);
                double sumBehaviorScores = 0.0;
                double sumBehaviorSimilarities = 0.0;
                for (int j = 0; j < allVisistedContents.Count; j++)
                {
                    double s = getSimilarity(arScene, allVisistedContents[j].anchor);
                    sumBehaviorScores += (s * (allVisistedContents[j].behaviouFactor + allVisistedContents[j].interactionFactor));
                    sumBehaviorSimilarities += s;

                    Debug.Log("Maryam Similarity: " + s);
                    Debug.Log("Maryam Similarity: " + sumBehaviorScores);
                }

                double behaviorScore = 0.0;
                if (sumBehaviorScores != 0)
                {
                    behaviorScore = sumBehaviorScores / sumBehaviorSimilarities;
                }


                double sumHistoryScores = 0.0;
                foreach (var anchor_id in userhistory.Keys)
                {
                    //userhistory[j].getAnchor(anchorList);
                    double s = getSimilarity(arScene, userhistory[anchor_id]);
                    sumHistoryScores += s;
                }

                double historyScore = 0.0;
                if (sumHistoryScores != 0)
                {
                    historyScore = sumHistoryScores / userhistory.Count;
                }


                double finalScore = (behaviorScore + historyScore) / 2;
                if (finalScore > recomScore)
                {
                    recomIndex = i;
                    recomScore = finalScore;
                }

                AnchorRecom anchorRecom = new AnchorRecom();
                anchorRecom.anchor_id = arScene.id;
                anchorRecom.weight = finalScore;

                anchor_recoms.Add(anchorRecom);
            }
        }

        Transition transition = new Transition();
        transition.anchor_trajectory_ids = anchor_trajectory_ids;
        //transition.user_id = AccountInfo.Instance.user.id;
        transition.user_id = PlayerPrefs.GetString("email");
        transition.anchor_recoms = anchor_recoms;
        transition.recomType = recommendationType;

        StartCoroutine(StoreTransition(transition));

        if (recomIndex != -1)
        {
            setRecommendation(ars, recomIndex, recomPosition);
        }
    }

    private void hybridRateRecommendation()
    {
        //List<Anchor> stories = navOptions.GetComponent<NavOptions>().stories;
        //int index = navOptions.GetComponent<NavOptions>().spaceTellingIndex;

        //Added by Ikbeom.
        List<Anchor> stories = navOptions.GetComponent<MapManager>().stories;
        int index = navOptions.GetComponent<MapManager>().spaceTellingIndex;


        if (stories.Count == 0)
        {
            return;
        }
        if (index == stories.Count - 1)
        {
            return;
        }

        Anchor currentFixed = stories[index];
        Anchor nextFixed = stories[index + 1];
        List<double> boundingBox = getBoundingBox(currentFixed.point, nextFixed.point);
        List<double> recomPosition = getMidddle(currentFixed.point, nextFixed.point);

        String anchor_trajectory_ids = currentFixed.id.ToString() + "-" + nextFixed.id.ToString();
        List<AnchorRecom> anchor_recoms = new List<AnchorRecom>();

        int recomIndex = -1;
        double recomScore = -1;

        GameObject ars = GameObject.Find("Real World/ARSceneParent");

        for (int i = 0; i < ars.transform.childCount; i++)
        {
            Anchor arScene = getAnchor(ars, i);
            if (isInBoundingBox(boundingBox, arScene) && isNotFixedRecomVisitedContensts(arScene) && isNotInTrajectoryPoIs(arScene, currentFixed, nextFixed))
            {
                Debug.Log("AR scene: " + arScene.id);
                double sumBehaviorScores = 0.0;
                double sumBehaviorSimilarities = 0.0;
                for (int j = 0; j < allVisistedContents.Count; j++)
                {
                    double s = getSimilarity(arScene, allVisistedContents[j].anchor);
                    sumBehaviorScores += (s * (allVisistedContents[j].getNormalizedWeight(maxUserFactor)));
                    sumBehaviorSimilarities += s;

                }

                double behaviorScore = 0.0;
                if (sumBehaviorScores != 0)
                {
                    behaviorScore = sumBehaviorScores / sumBehaviorSimilarities;
                }


                double sumHistoryScores = 0.0;
                double sumHistorySimilarities = 0.0;
                foreach (var anchor_id in userhistory.Keys)
                {
                    //userhistory[j].getAnchor(anchorList);
                    double s = getSimilarity(arScene, userhistory[anchor_id]);
                    sumHistoryScores += (s * userhistory[anchor_id].user_like);
                    sumHistorySimilarities += s;
                }

                double historyScore = 0.0;
                if (sumHistoryScores != 0)
                {
                    historyScore = sumHistoryScores / sumHistorySimilarities;
                }


                double finalScore = (behaviorScore + historyScore) / 2;

                if (finalScore > recomScore)
                {
                    recomIndex = i;
                    recomScore = finalScore;
                }

                AnchorRecom anchorRecom = new AnchorRecom();
                anchorRecom.anchor_id = arScene.id;
                anchorRecom.weight = finalScore;

                anchor_recoms.Add(anchorRecom);
            }
        }

        Transition transition = new Transition();
        transition.anchor_trajectory_ids = anchor_trajectory_ids;
        //transition.user_id = AccountInfo.Instance.user.id;
        transition.user_id = PlayerPrefs.GetString("email");
        transition.anchor_recoms = anchor_recoms;
        transition.recomType = recommendationType;

        StartCoroutine(StoreTransition(transition));

        if (recomIndex != -1)
        {
            setRecommendation(ars, recomIndex, recomPosition);
        }
    }

    private List<double> getMidddle(String trajectorySegment)
    {
        List<double> recomPosition = new List<double>();

        Dictionary<String, double[]> midpoints = new Dictionary<String, double[]>();
        midpoints["3144-3147"] = new double[] {36.368605, 127.364395};
        midpoints["3147-3151"] = new double[] {36.36854754, 127.3631116};
        midpoints["3151-3171"] = new double[] {36.37049389, 127.3620723};
        
        midpoints["3161-3165"] = new double[] {36.37356649, 127.3610764};
        midpoints["3165-3168"] = new double[] {36.37356675, 127.3599696};
        midpoints["3168-3170"] = new double[] {36.37283422, 127.3596441};

        double[] position = midpoints[trajectorySegment];

        recomPosition.Add(position[0]);
        recomPosition.Add(position[1]);

        return recomPosition;
    }
   
    private List<double> getMidddle(Point point1, Point point2)
    {
        List<double> recomPosition = new List<double>();

        //removed by Ikbeom.(It looks not be used.)
        //List<Vector2d> route = direction.GetComponent<PinLineToMap>().GetLine();

        double middleLatitude = 0.5 * (point1.latitude + point2.latitude);
        double middleLongitude = 0.5 * (point1.longitude + point2.longitude);


        double rLatitude = middleLatitude - point1.latitude;
        double rLongitude = middleLongitude - point1.longitude;

        double kLatitude = point2.latitude - point1.latitude;
        double kLongitude = point2.latitude - point1.longitude;

        double z = ((rLatitude*kLatitude)+(rLongitude*kLongitude)) / ((kLongitude*kLongitude)+(kLatitude*kLatitude));

        double qLatitude = point1.latitude + (z*kLatitude);
        double qLongitude = point1.longitude + (z * kLongitude);

        recomPosition.Add(middleLatitude);
        recomPosition.Add(middleLongitude);

        //List<double> distances = new List<double>();
        //List<double> distances_2 = new List<double>();

        //for (int j = 0; j < route.Count; j++)
        //{
        //   double dist = VisitedContent.DistanceTo(middleLatitude, middleLongitude, route[j][0], route[j][1]);
        //  distances.Add(dist);
        //   distances_2.Add(dist);
        //}

        //distances.Sort();
        //distances.Reverse();

        //int index1 = distances_2.IndexOf(distances[0]); 
        //int index2 = distances_2.IndexOf(distances[1]);

        //if(Math.Abs(index2 - index1) != 1)
        //{
        //   Debug.Log("Recommendation error !!!!!!!!!!!!");
        //}

        //recomPosition.Add((1 / 2) * (route[index1][0] + route[index2][0]));
        //recomPosition.Add((1 / 2) * (route[index1][1] + route[index2][1]));

        return recomPosition;
    }

    private List<double> getBoundingBox(Point point1, Point point2)
    {
        double r = (1 * 0.1) /111.2;
        //doublr r = (5 * 0.1) / 6378.1;

        List<double> boundingBox = new List<double>();

        double minLat;
        double minLon;
        double maxLat;
        double maxLon;

        if (point1.latitude < point2.latitude)
        {
            minLat = point1.latitude;
            maxLat = point2.latitude;
        }
        else
        {
            minLat = point2.latitude;
            maxLat = point1.latitude;
        }

        if (point1.longitude < point2.longitude)
        {
            minLon = point1.longitude;
            maxLon = point2.longitude;
        }
        else
        {
            minLon = point2.longitude;
            maxLon = point1.longitude;
        }

        boundingBox.Add(minLat - r);
        boundingBox.Add(minLon -r);
        boundingBox.Add(maxLat + r);
        boundingBox.Add(maxLon + r);

        return boundingBox;
    }

    private bool isInBoundingBox(List<double> boundingBox, Anchor anchor)
    {
        bool result = false;
        bool latitudeCheck = (anchor.point.latitude == boundingBox[0] || anchor.point.latitude >= boundingBox[0]) &&
                         (anchor.point.latitude == boundingBox[2] || anchor.point.latitude < boundingBox[2]);

        bool longitudeCheck = (anchor.point.longitude == boundingBox[1] || anchor.point.longitude >= boundingBox[1]) &&
                         (anchor.point.longitude == boundingBox[3] || anchor.point.longitude < boundingBox[3]);

        if (latitudeCheck && longitudeCheck)
        {
            result = true;
        }

        return result;
    }

    private bool isNotFixedRecomVisitedContensts(Anchor anchor)
    {
        bool result = true;
        
        //List<Anchor> stories = navOptions.GetComponent<NavOptions>().stories;
        //Added by Ikbeom.
        List<Anchor> stories = navOptions.GetComponent<MapManager>().stories;

        for (int i = 0; i < stories.Count; i++)
        {
            if (stories[i].id == anchor.id)
            {
                result = false;
                break;
            }
        }

        for (int i = 0; i < recomList.Count; i++)
        {
            if (recomList[i].id == anchor.id)
            {
                result = false;
                break;
            }
        }

        for (int i = 0; i < allVisistedContents.Count; i++)
        {
            if (allVisistedContents[i].anchor.id == anchor.id)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    private bool isNotInTrajectoryPoIs(Anchor arScene, Anchor currentFixed, Anchor nextFixed)
    {
        bool result = true;

        string arsceTag = "";
        for (int i = 0; i < arScene.tags.Count; i++)
        {
            if (arScene.tags[i].category == "PlaceTag")
            {
                arsceTag = arScene.tags[i].tag;
                break;
            }
        }

        

        if(storyPlaceTags.Contains(arsceTag))
        {
            result = false;
        }

        return result;
    }

    private void getStoryPlaceTags()
    {
        storyPlaceTags = new List<string>();

        //List<Anchor> stories = navOptions.GetComponent<NavOptions>().stories;
        //Added by Ikbeom.
        List<Anchor> stories = navOptions.GetComponent<MapManager>().stories;

        for (int i = 0; i < stories.Count; i++)
        {
            for (int j = 0; j < stories[i].tags.Count; j++)
            {
                if (stories[i].tags[j].category == "PlaceTag")
                {
                    storyPlaceTags.Add(stories[i].tags[j].tag);
                    break;
                }
            }
        }
    }

    private double getSimilarity(Anchor arScene1, Anchor arScene2)
    {

        double textSimilarity = getTextSimilarity(arScene1.id.ToString(), arScene2.id.ToString());
        double tagSimilarity = getTagSimilarity(arScene1.tags, arScene2.tags);


        double similarity = (textSimilarity + tagSimilarity)/2;
        return similarity;
    }

    private double getSimilarity(Anchor arScene1, StoringInfo arScene2)
    {

        double textSimilarity = getTextSimilarity(arScene1.id.ToString(), arScene2.anchor_id.ToString());
        double tagSimilarity = getTagSimilarity(arScene1.tags, arScene2.tags);


        double similarity = (textSimilarity + tagSimilarity) / 2;
        return similarity;
    }

    private double getTagSimilarity(List<Tag> arScene1, List<Tag> arScene2)
    {
        double similarity = 0.0;
        double sum = 0.0;

        if (arScene1.Count > 0 && arScene2.Count > 0)
        {
            for (int i = 0; i < arScene1.Count; i++)
            {
                for (int j = 0; j < arScene2.Count; j++)
                {
                    if (arScene1[i].tag == arScene2[j].tag)
                    {
                        sum += 1;
                    }
                }
            }

            similarity = sum / (Math.Sqrt(arScene1.Count) * Math.Sqrt(arScene2.Count));
        }

        return similarity;
    }
    
    private double getTextSimilarity(String arSceneId1, String arSceneId2)
    {
        double similarity = 0.0;
        double sum = 0.0;

        double sumWeights1 = 0.0;
        double sumWeights2 = 0.0;

        List<Token> sceneTokens1 = getAnchorTokenList(arSceneId1);
        List<Token> sceneTokens2 = getAnchorTokenList(arSceneId2);

        if (sceneTokens1.Count > 0 && sceneTokens2.Count > 0)
        {
            int k = 1;

            for (int i = 0; i < sceneTokens1.Count; i++)
            {
                for (int j = 0; j < sceneTokens2.Count; j++)
                {
                    if (sceneTokens1[i].token == sceneTokens2[j].token)
                    {
                        sum += sceneTokens1[i].weight * sceneTokens2[j].weight;
                    }

                    if(k == 1)
                    {
                        sumWeights2 += Math.Pow(sceneTokens2[j].weight, 2);
                    }
                }

                sumWeights1 += Math.Pow(sceneTokens1[i].weight, 2);
                k = 2;
            }

            similarity = sum / (Math.Sqrt(sumWeights1) * Math.Sqrt(sumWeights2));
        }

        return similarity;
    }

    private List<Token> getAnchorTokenList(String anchor_id)
    {
        if (anchorTokens.ContainsKey(anchor_id))
        {
            return anchorTokens[anchor_id];
        }
        else
        {
            return new List<Token>();
        }
    }

    //private Anchor getAnchor(GameObject ars, int index)
    //{
    //    GameObject ar = ars.transform.GetChild(index).gameObject;
    //    UGCPrefab script = ar.GetComponent<UGCPrefab>();
    //    AnchorPrefab script2 = ar.GetComponent<AnchorPrefab>();

    //    Anchor arScene = null;

    //    if (script != null)
    //    {
    //        arScene = script.arScene;
    //    }
    //    else if (script2 != null)
    //    {
    //        arScene = script2.arScene;
    //    }

    //    return arScene;
    //}
    
    //editted by jeon.
    private Anchor getAnchor(GameObject ars, int index)
    {
        GameObject ar = ars.transform.GetChild(index).gameObject;
        var anchor = ar.GetComponent<IconManager>().anchor;

        return anchor;
    }
    //private void setRecommendation(GameObject ars, int recomIndex, List<double> recomPosition)
    //{
    //    GameObject ar = ars.transform.GetChild(recomIndex).gameObject;
    //    UGCPrefab script = ar.GetComponent<UGCPrefab>();
    //    AnchorPrefab script2 = ar.GetComponent<AnchorPrefab>();

    //    if (script2 != null)
    //    {
    //        createRecomObject(script2.arScene, recomPosition);
    //        //script2.updateIcon();
    //        recomList.Add(script2.arScene);
    //    }

    //    latestVisistedContents.Clear();
    //}
    
    //editted by jeon.
    private void setRecommendation(GameObject ars, int recomIndex, List<double> recomPosition)
    {
        GameObject ar = ars.transform.GetChild(recomIndex).gameObject;
        var script = ar.GetComponent<IconManager>();

        if (script != null)
        {
            createRecomObject(script.anchor, recomPosition);
            //script2.updateIcon();
            recomList.Add(script.anchor);
        }

        latestVisistedContents.Clear();
    }

    //private void createRecomObject(Anchor anchor, List<double> recomPosition)
    //{
    //    Camera arCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

    //    double lon = recomPosition[1];
    //    double lat = recomPosition[0];
    //    //double alt = anchorList[i].point.altitude; //not in use.

    //    Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, lat, lon);

    //    GameObject recommendedParent = GameObject.Find("Real World/RecommendedParent");

    //    GameObject arScene = Instantiate(poiPrefab, Vector3.zero, Quaternion.identity, recommendedParent.transform) as GameObject;
    //    arScene.transform.localPosition = new Vector3(pos.x, 0, pos.z);
    //    AnchorPrefab script = arScene.GetComponent<AnchorPrefab>();
    //    script.arScene = anchor;
    //    //script.GetComponent<AnchorPrefab>().indexContent = j;
    //    script.directions = direction;

    //    script.Init(arCamera, true);
    //    arScene.SetActive(true);
    //}
    private void createRecomObject(Anchor anchor, List<double> recomPosition)
    {
        Camera arCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        double lon = recomPosition[1];
        double lat = recomPosition[0];
        //double alt = anchorList[i].point.altitude; //not in use.

        Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, lat, lon);

        GameObject recommendedParent = GameObject.Find("Real World/RecommendedParent");

        GameObject newIcon = Instantiate(ResourceLoader.Instance.icon_recommendation, recommendedParent.transform) as GameObject;
        
        var script = newIcon.GetComponent<IconManager>();


        //string category = anchor.tags.Where(t1 => t1.category == "InterestTag").Select(t2 => t2.tag).ToArray()[0];
        string category = "Recommenation";
        script.Init(anchor, category, arCamera.transform);

        newIcon.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);
    }

    private void getAnchorTokens()
    {
        StartCoroutine(setTokens());
    }

    private IEnumerator setTokens()
    {
        string url = string.Format("http://kctmrecomapp-env.eba-6p3axegk.us-east-1.elasticbeanstalk.com/getTokens");

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Dictionary<string, List<Token>> uploadResult = new Dictionary<string, List<Token>>();
            Debug.Log(www.downloadHandler.text.ToString()); // this log is returning the requested data. 
            uploadResult = JsonConvert.DeserializeObject<Dictionary<string, List<Token>>>(www.downloadHandler.text);
            anchorTokens = uploadResult;
        }

    }

    public IEnumerator StoreTransition(Transition transition)
    {
        string url = "http://kctmrecomapp-env.eba-6p3axegk.us-east-1.elasticbeanstalk.com/inserttransition";
        string storingJson = JsonUtility.ToJson(transition);
        Debug.Log("Storing Json: " + storingJson);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(storingJson);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
    }

    public IEnumerator Upload(StoringInfo storingInfo)
    {
        string url = "http://kctmrecomapp-env.eba-6p3axegk.us-east-1.elasticbeanstalk.com/insertvisitedcontent";
        string storingJson = JsonUtility.ToJson(storingInfo);
        //Debug.Log("Storing Json: " + storingJson);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(storingJson);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);

    }

    public IEnumerator LoadUserVisitedContent()
    {
        string url = string.Format("http://kctmrecomapp-env.eba-6p3axegk.us-east-1.elasticbeanstalk.com/getvisitedcontentlist");
        //Dictionary<string, string> data = new Dictionary<string, string>();
        //data.Add("user_id", PlayerPrefs.GetString("email"));

        //NetworkManager.Instance.Post(url, data, GetVisitedContentContent, FailureHandler);

        WWWForm formData = new WWWForm();
        formData.AddField("user_id", PlayerPrefs.GetString("email"));

        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError || www.responseCode >= 400)
        {
            Debug.Log(www.error);
        }
        else
        {
            List<StoringInfo> history = new List<StoringInfo>();
            history = JsonConvert.DeserializeObject<List<StoringInfo>>(www.downloadHandler.text.ToString());

            userhistory = new Dictionary<long, StoringInfo>();
            //userHistoryAnchor = new Dictionary<long, Anchor>();

            for (int i = 0; i < history.Count; i++)
            {
                userhistory [history[i].anchor_id] = history[i];
            }

            Debug.Log("Maryam Post Request " + history.Count);
            Debug.Log(userhistory.Count);
        }
    }
}
