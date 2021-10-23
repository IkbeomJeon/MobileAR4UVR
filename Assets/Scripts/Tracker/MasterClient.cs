using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class MasterClient : Client
{
    static MasterClient instance;

    //public IDictionary<string, DataPackage> dataPackages = new Dictionary<string, DataPackage>();
   
    public IDictionary<string, Matrix4x4> dicClientInfo = new Dictionary<string, Matrix4x4>();
    public IDictionary<string, Matrix4x4> dicImageAnchor = new Dictionary<string, Matrix4x4>();

    public object lockDataPackage = new object();
    public object lockImageAnchor = new object();

    public static MasterClient Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MasterClient();
            }
            return instance;
        }
    }

    public new void Connect(string server_IP, int port)
    {
        base.Connect(server_IP, port);
        

        // Inform 'Register' to Server.
        SendData(RequestType.RegisterClient, ClientType.Master, new TransformData());

        byte[] buffer = new byte[bufSize];
        socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epFrom, new AsyncCallback(ReceieveCallBack), buffer);
    }

    public void Broadcast(TransformData transformData)
    {
        SendData( RequestType.Broadcast,  ClientType.Master, transformData);
    }

    public void RequestUpdateImageAnchor(TransformData transformData)
    {
        SendData(RequestType.UpdateImageAnchorPose, ClientType.Master, transformData);
    }

    public void RequestImageAnchorPose(string guid)
    {
        TransformData data = new TransformData
        {
            imageAnchorGUID = guid
        };

        SendData(RequestType.RequestImageAnchorPose, ClientType.Master, data);
    }

    public void RequestAllImageAnchorPoses()
    {
        
        SendData(RequestType.RequestAllImageAnchorPoses, ClientType.Master, new TransformData());
    }

    public new void Disconnect()
    {
        SendData(RequestType.DeregisterClient, ClientType.Master, new TransformData());

        dicImageAnchor.Clear();
        dicClientInfo.Clear();
        
        base.Disconnect();
    }

    public override void ReceieveCallBack(IAsyncResult aResult)
    {
        //Debug.Log("Master Client recieved.");
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
                        if (receivedDataPackage.requestType == RequestType.Broadcast)
                        {
                            lock (lockDataPackage)
                            {
                                Matrix4x4 clientMat = Matrix4x4.identity;
                                Vector3 position = receivedDataPackage.data.cameraPosition;
                                Quaternion rotation = receivedDataPackage.data.cameraRotation;
                                clientMat.SetTRS(position, rotation, new Vector3(1, 1, 1));

                                if (!dicClientInfo.ContainsKey(receivedDataPackage.fromAddress))                               
                                    dicClientInfo.Add(receivedDataPackage.fromAddress, clientMat);
                                
                                else
                                    dicClientInfo[receivedDataPackage.fromAddress] = clientMat;
                            }
                        }

                        if (receivedDataPackage.requestType == RequestType.RequestImageAnchorPose)
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

    public void Destroy()
    {
        if (isConnected)
            Disconnect();

        instance = null;
    }

   
}

