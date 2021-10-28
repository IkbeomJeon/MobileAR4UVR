using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KCTM.Network.Data;

namespace KCTM
{
    namespace Recommendation
    {
        [Serializable]
        public class VisitedContent
        {

            private double contentTime = 120;
            private double visualDist = 100.0;
            private double visualAngle = 100.0;

            public Anchor anchor;
            public DateTime visitedDateTime;
            public int liked = 1;

            public double userLat;
            public double userLon;
            public double userVisitedTime;
            public string user_id;

            public double stareFactor;
            public double skipFactor;
            public double likeFactor;
            public double interactionFactor;

            public double behaviouFactor;

            public double finalFactor;

            private Anchor nearestRecomPoint;
            private bool isNearToRecommendedARScene = false;
            private bool isNearToFixedARScene = false;

            private double standingWeight = 0.0;
            private double movingWeight = 0.0;
            private double turningWeight = 0.0;

            
            public double calFactors(Anchor fixedARScene, List<Anchor> recomARScenes)
            {
                calInteractionFactors();
                calBehaviorFactor(fixedARScene, recomARScenes);

                finalFactor =  behaviouFactor + interactionFactor;
                return finalFactor;
            }

            public double getNormalizedWeight(double maxValue)
            {
                if(maxValue == 0)
                {
                    return finalFactor;
                }
                else
                {
                    return finalFactor / maxValue;
                }
            }

            public void calInteractionFactors()
            {
                double timeFactor = userVisitedTime / contentTime;

                if (userVisitedTime >= contentTime)
                {
                    stareFactor = 1 - (1 / (2 * Math.Pow(timeFactor, 2)));
                    skipFactor = 0;
                }
                else if (userVisitedTime < contentTime)
                {
                    stareFactor = 0;
                    skipFactor = Math.Exp(-6 * Math.Pow((timeFactor - 1), 2));
                }

                likeFactor = likeFactor / 10;

                interactionFactor = (stareFactor + skipFactor + likeFactor) / 3;

            }

            public void calBehaviorFactor(Anchor fixedARScene, List<Anchor> recomARScenes)
            {
                isNearToFixedARScene = isUserNearToContent(fixedARScene);

                if (isNearToFixedARScene)
                {
                    standingWeight = getStandingWeight(fixedARScene);
                }
                else
                {
                    movingWeight = getMovingWeight(recomARScenes);
                }


                turningWeight = getTurningWeight(fixedARScene, recomARScenes);

                behaviouFactor = (standingWeight + movingWeight + turningWeight) / 3;

            }


            private double getStandingWeight(Anchor fixedARScene)
            {
                double weight = 0.0;

                if (fixedARScene.id == anchor.id)
                {
                    weight = 0.7;
                }
                else if (fixedARScene.id != anchor.id)
                {
                    weight = 1.0;
                }

                return weight;
            }


            private double getMovingWeight(List<Anchor> recomARScenes)
            {
                double weight = 1.0;

                bool isContained = containAnchor(recomARScenes, anchor);
                if (isContained)
                {
                    weight = 0.7;
                }
                else if (!isContained)
                {
                    weight = 1.0;
                }

                return weight;
            }

            private bool containAnchor(List<Anchor> anchors, Anchor anchor)
            {
                bool isContained = false;

                for(int i = 0; i < anchors.Count; i++)
                {
                    if (anchors[i].id == anchor.id)
                    {
                        isContained = true;
                    }
                }

                return isContained;
            }

            private double getTurningWeight(Anchor fixedARScene, List<Anchor> recomARScenes)
            {
                if (recomARScenes.Count != 0)
                {
                    isNearToRecommendedARScene = isUserNearToContent(recomARScenes[recomARScenes.Count - 1]);
                }
                else
                {
                    isNearToRecommendedARScene = false;
                }

                double weight = 0.0;

                double bearing1 = calBearing(userLat, userLon, anchor.point.latitude, anchor.point.longitude);

                double bearing2 = 0.0;
                if (isNearToFixedARScene)
                {
                    bearing2 = calBearing(userLat, userLon, fixedARScene.point.latitude, fixedARScene.point.longitude);
                }
                else if (isNearToRecommendedARScene)
                {
                    bearing2 = calBearing(userLat, userLon, recomARScenes[recomARScenes.Count - 1].point.latitude, recomARScenes[recomARScenes.Count - 1].point.longitude);
                }

                double angle = Math.Abs(bearing2 - bearing1);

                if (angle > visualAngle)
                {
                    weight = 1.0;
                }

                return weight;
            }

            private bool isUserNearToContent(Anchor ARscene)
            {
                bool isNeartoARScene = false;

                double dist = DistanceTo(userLat, userLon, ARscene.point.latitude, ARscene.point.longitude);
                if (dist < visualDist)
                {
                    isNeartoARScene = true;
                }

                return isNeartoARScene;
            }


            private void closestPointToUser(List<Anchor> fixedARScenes, List<Anchor> recomARScenes)
            {
                double minFixedDist = 1000;
                Anchor nearestFixedAnchor = null;
                isNearToFixedARScene = false;
                isNearToRecommendedARScene = false;

                for (int i = 0; i < fixedARScenes.Count; i++)
                {
                    double dist = DistanceTo(userLat, userLon, fixedARScenes[i].point.latitude, fixedARScenes[i].point.longitude);

                    if (dist < minFixedDist)
                    {
                        minFixedDist = dist;
                        nearestFixedAnchor = fixedARScenes[i];
                    }
                }


                double minRecomDist = 1000;
                Anchor nearestRecomAnchor = null;

                for (int i = 0; i < recomARScenes.Count; i++)
                {
                    double dist = DistanceTo(userLat, userLon, recomARScenes[i].point.latitude, recomARScenes[i].point.longitude);

                    if (dist < minRecomDist)
                    {
                        minRecomDist = dist;
                        nearestRecomAnchor = fixedARScenes[i];
                    }
                }

                if (minFixedDist < visualDist && minFixedDist < minRecomDist)
                {
                    nearestRecomPoint = nearestFixedAnchor;
                    isNearToFixedARScene = true;
                    isNearToRecommendedARScene = false;
                }
                else if (minRecomDist < visualDist && minRecomDist < minFixedDist)
                {
                    nearestRecomPoint = nearestRecomAnchor;
                    isNearToFixedARScene = false;
                    isNearToRecommendedARScene = true;
                }
            }

            public static double DistanceTo(double lat1, double lon1, double lat2, double lon2)
            {
                double rlat1 = Math.PI * lat1 / 180;
                double rlat2 = Math.PI * lat2 / 180;
                double theta = lon1 - lon2;
                double rtheta = Math.PI * theta / 180;
                double dist =
                    Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                    Math.Cos(rlat2) * Math.Cos(rtheta);
                dist = Math.Acos(dist);
                dist = dist * 180 / Math.PI;
                dist = dist * 60 * 1.1515;

                double distMeter = (dist * 1.609344) / 1000;
                return distMeter;

            }


            private double calBearing(double lat1, double lon1, double lat2, double lon2)
            {
                double bearing = 0.0;
                double part1 = Math.Sin(lon2 - lon1) * Math.Cos(lat2);
                double part2 = (Math.Cos(lat1) * Math.Sin(lat2)) - (Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1));

                bearing = Math.Atan2(part1, part2);

                return bearing;
            }

            public void setContentTime()
            {
                string[] strArr = anchor.description.Split(" "[0]);
                contentTime = strArr.Length * 0.3;
            }

        }
    }
}