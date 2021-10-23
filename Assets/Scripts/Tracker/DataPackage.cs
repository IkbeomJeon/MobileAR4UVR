using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ClientType
{
    Master = 1,
    Tracker = 2
}
public enum RequestType
{
    RegisterClient = 1,
    UpdateImageAnchorPose = 2,
    UpdateCameraPose = 3,
    RequestImageAnchorPose = 4,
    RequestAllImageAnchorPoses = 5,
    RequestAllCameraPoses = 6,
    Broadcast = 7,
    ToAddress = 8,
    DeregisterClient = 99
}

[System.Serializable]
public class TransformData
{
    public string imageAnchorGUID;          // image anchor's glaobl unique id
    public Vector3 imageAnchorPosition;   // image anchor location in unity space
    public Quaternion imageAnchorRotation;   // image anchor rotation in unity space
    public string imageAnchorDate;
    public Vector3 cameraPosition;    // camera position in unity space -> unity is in meters
    public Quaternion cameraRotation;   // camera rotation in unity space
    public double worldPositionGPSLat;    // camera position converted to GPS (Note: GPS based on the anchor GPS position)
    public double worldPositionGPSLong;
    public double worldPositionGPSAlt;
    public Quaternion worldRotationDegrees;  // camera rotation based on compass
    public Quaternion worldRotationGyro;   // camera rotation based on gyro sensor
    

    public TransformData()
    {

    }

    public TransformData(string imageAnchorGUID, Vector3 imgAnchorPosition, Quaternion imgAnchorRotation, Vector3 camPosition, Quaternion camRotation, double worldPosLat, double worldPosLong, double worldPosAlt, Quaternion worldRot, Quaternion worldGyro, string imageAnchorDate)
    {
        this.imageAnchorGUID = imageAnchorGUID;
        this.imageAnchorPosition = imgAnchorPosition;
        this.imageAnchorRotation = imgAnchorRotation;
        this.imageAnchorDate = imageAnchorDate;
        this.cameraPosition = camPosition;
        this.cameraRotation = camRotation;
        this.worldPositionGPSLat = worldPosLat;
        this.worldPositionGPSLong = worldPosLong;
        this.worldPositionGPSAlt = worldPosAlt;
        this.worldRotationDegrees = worldRot;
        this.worldRotationGyro = worldGyro;
    }

}

[System.Serializable]
public class DataPackage
{
    /// <summary>
    /// Protocol: used to identify request clientType
    ///     - requestType == 1 : request to register
    ///     - requestType == 2 : request to Broadcast
    ///     - requestType == 3 : request to De-Register
    /// Type: :  used to identify master/client 
    ///     - if clientType == 1 : it is a request from master
    ///     - if clientType == 2 : it is a request from client
    /// Error: flag when there is any kind of error (forgot to register and etc)
    ///     - error == 1 : error flag up and must check "message" for detail
    ///     - error == 0 : no error
    /// Address: address of the master/client, keep track of this!   If Master, MAKE SURE TO HAVE ADDRESS OF CLIENT WHEN SENDING 
    /// Port: port of the master/client, keep track of this!   If Master, MAKE SURE TO HAVE PORT OF CLIENT WHEN SENDING
    /// Message: Message if success or error with info
    /// Data: Actual data, contains quaternion and vector3
    /// </summary>

    public RequestType requestType;
    public ClientType clientType;
    public int error;
    public string fromAddress;
    public string port;

    public string message;

    public TransformData data;
}


