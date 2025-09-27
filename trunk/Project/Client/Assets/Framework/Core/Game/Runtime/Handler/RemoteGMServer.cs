using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Game;
using Game.UI;
using UnityEngine;

public class RemoteGMServer
{
    private static Socket serverSocket = null;
    private static Socket clientSocket = null;
    private static int port = 30069;
    private static Thread acceptThread = null;
    private static Thread receiveThread = null;
    private static byte[] buffer = new byte[1024];
    private static bool closeServer = false;

    public static void StartServer()
    {
        if (IsPortUsed(port))
        {
            Debug.Log("create remote gm server fail, port occupied.");
            return;
        }
        try
        {
            closeServer = false;
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(localEndPoint);
            serverSocket.Listen(10);

            acceptThread = new Thread(OnAcceptClient);
            acceptThread.Start();
            Debug.Log("open remote gm server");
        }
        catch (Exception ex)
        {
            Debug.LogError("remote gm server start fail: " + ex.Message);
        }
    }

    private static bool IsPortUsed(int port)
    {
        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] points = properties.GetActiveTcpListeners();
        int count = points.Count(p => p.Port == port);
        return count > 0;
    }

    private static void OnAcceptClient()
    {
        while (!closeServer)
        {
            OnClientConnect();
            Thread.Sleep(100);
        }
    }

    private static void OnClientConnect()
    {
        // ֻ����һ����Ч����
        if (clientSocket != null || closeServer)
            return;

        try
        {
            clientSocket = serverSocket.Accept();
        }
        catch (Exception ex)
        {
            if (!closeServer)
                Debug.LogError("remote gm server accept error: " + ex.Message);
            Close();
            return;
        }

        receiveThread = new Thread(OnReceiveMessage);
        receiveThread.Start();
    }

    private static void OnReceiveMessage()
    {
        while (true)
        {
            try
            {
                if (clientSocket == null)
                {
                    Close();
                    return;
                }

                if (clientSocket.Poll(100, SelectMode.SelectError))
                { 
                    Debug.Log("client disconnet");
                    Close();
                    return;
                }

                int revChNum = clientSocket.Receive(buffer);
                string con = Encoding.ASCII.GetString(buffer, 0, revChNum);
                if (!string.IsNullOrEmpty(con))
                {
                    Debug.Log("GM Command " + con);
                    GameModule.Instance.RemoteGMCmd = con;
                    Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("remote gm server error: " + ex.Message);
                Close();
                return;
            }
        }
    }

    public static void Close()
    {
        if (receiveThread != null)
        {
            receiveThread = null;
        }
        if (clientSocket != null)
        {
            clientSocket = null;
        }
    }

    public static void StopServer()
    {
        closeServer = true;
        if (acceptThread != null)
        {
            try
            {
                acceptThread.Abort();
            }
            catch { }
            acceptThread = null;
        }
        if (serverSocket != null)
        {
            try
            {
                serverSocket.Close();
            }
            catch { }
            serverSocket = null;
        }
        Close();
        Debug.Log("close remote gm server");
    }
}
