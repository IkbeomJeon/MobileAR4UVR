using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

class TrackerClient : Client
{
    static TrackerClient instance;

    public IDictionary<string, Matrix4x4> dicImageAnchor = new Dictionary<string, Matrix4x4>();
    public object lockImageAnchor = new object();
    public bool bNewDataRecieved_ImageAnchorPoses;
    public delegate void ResponseHandler();
    public static TrackerClient Instance
    {
        get
        {
            if (instance == null)
                instance = new TrackerClient();

            return instance;
        }
    }

    public new void Connect(string server_IP, int port)
    {
        base.Connect(server_IP, port);

        // Inform 'Register' to Server.
        SendData(RequestType.RegisterClient, ClientType.Tracker, new TransformData());

        byte[] buffer = new byte[bufSize];
        socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epFrom, new AsyncCallback(ReceieveCallBack), buffer);
    }
    public void Broadcast(TransformData transformData)
    {
        SendData(RequestType.Broadcast, ClientType.Tracker, transformData);
    }

    public void UpdateCameraPose2Server(TransformData transformData)
    {   
        SendData(RequestType.UpdateCameraPose, ClientType.Tracker, transformData); 
    }

    public void RequestImageAnchorPose(string guid)
    {
        TransformData data = new TransformData
        {
            imageAnchorGUID = guid
        };

        SendData(RequestType.RequestImageAnchorPose, ClientType.Tracker, data);
       
    }
    
    public new void Disconnect()
    {
        SendData(RequestType.DeregisterClient, ClientType.Tracker, new TransformData());
        dicImageAnchor.Clear();
        base.Disconnect();
    }

    public override void ReceieveCallBack(IAsyncResult aResult)
    {
        Debug.Log("Tracker Client recieved.");
        try
        {
            if (socket != null)
            {
                int bytes = socket.EndReceiveFrom(aResult, ref epFrom);
                if (bytes > 0)
                {
                    byte[] receivedData = new byte[bufSize];
                    receivedData = (byte[])aResult.AsyncState;
                    string receivedDataString = Encoding.ASCII.GetString(receivedData, 0, bytes);

                    DataPackage receivedDataPackage = new DataPackage();
                    JsonUtility.FromJsonOverwrite(receivedDataString, receivedDataPackage);

                    //Debug.Log("received from:  " + receivedDataPackage.fromAddress + "\n" + receivedDataString);

                    if (receivedDataPackage.error == 1)
                    {
                        Debug.Log("Error, received message parse failed.");
                    }

                    else
                    {
                        if (receivedDataPackage.requestType == RequestType.RequestImageAnchorPose
                                || receivedDataPackage.requestType == RequestType.UpdateImageAnchorPose)
                        {
                            lock (lockImageAnchor)
                            {
                                string guid = receivedDataPackage.data.imageAnchorGUID;
                                Vector3 position = receivedDataPackage.data.imageAnchorPosition;
                                Quaternion quaternion = receivedDataPackage.data.imageAnchorRotation;
                                Matrix4x4 mat = Matrix4x4.TRS(position, quaternion, Vector3.one);

                                if (!dicImageAnchor.ContainsKey(guid))
                                    dicImageAnchor.Add(guid, mat);

                                else
                                    dicImageAnchor[guid] = mat;

                                bNewDataRecieved_ImageAnchorPoses = true;
                            }
                        }
                    }
                }

                byte[] buffer = new byte[bufSize];
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epFrom, new AsyncCallback(ReceieveCallBack), buffer);
            }
        }
        catch (Exception exp)
        {
            Debug.LogError(exp.ToString());
        }

    }

}

