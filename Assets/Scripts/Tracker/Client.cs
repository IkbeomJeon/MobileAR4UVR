using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client
{
    string serverIP; 
    int serverPort;  

    protected int bufSize = 8 * 1024;
    protected Socket socket;
    protected EndPoint epFrom;// = new IPEndPoint(IPAddress.Any, 0);

    protected void Connect(string server_IP, int server_Port)
    {
        serverIP = server_IP;
        serverPort = server_Port;

        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 1000);
            epFrom = new IPEndPoint(IPAddress.Parse(server_IP), server_Port);
            socket.Connect(epFrom);
            Debug.Log("Server connection succeed");

        }

        catch (SocketException socketException)
        {
            Debug.LogError("Socket connect error! : " + socketException.ToString());
            socket.Close();
        }
    }
    public bool isConnected
    {
        get
        {
            if (socket == null)
                return false;

            return socket.Connected;
        }
    }
    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket so = (Socket)ar.AsyncState;
            int bytes = socket.EndSend(ar);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

    }

    public virtual void ReceieveCallBack(IAsyncResult aResult) { }
  
    protected void SendData(RequestType requestType, ClientType clientType, TransformData transformData)
    {
        DataPackage dataPackage = new DataPackage
        {
            requestType = requestType,
            clientType = clientType,
            data = transformData
        };
        string textToSend = JsonUtility.ToJson(dataPackage);
        byte[] data = Encoding.ASCII.GetBytes(textToSend);
        socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
        //Debug.Log("Sent: " + textToSend);
    }

    public void Disconnect()
    {
        //Waiting for exiting ohter thread.
        Thread.Sleep(100);
        if (socket != null && socket.Connected)
        {
            //socket.Disconnect(false);
            socket.Close();
            socket = null;
        }
    }

}
