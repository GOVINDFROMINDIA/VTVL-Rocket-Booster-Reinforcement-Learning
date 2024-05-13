using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Rocket : MonoBehaviour
{
    TcpListener listener;
    TcpClient client;
    NetworkStream stream;
    Thread clientThread;

    void Start()
    {
        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 25001);
        listener.Start();
        clientThread = new Thread(new ThreadStart(HandleClient));
        clientThread.Start();
    }

    void HandleClient()
    {
        using (client = listener.AcceptTcpClient())
        using (stream = client.GetStream())
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[] parts = receivedData.Split(';');
                if (parts.Length == 2)
                {
                    string[] posData = parts[0].Split(',');
                    string[] rotData = parts[1].Split(',');

                    Vector3 position = new Vector3(float.Parse(posData[0]), float.Parse(posData[1]), float.Parse(posData[2]));
                    Quaternion rotation = Quaternion.Euler(float.Parse(rotData[0]), float.Parse(rotData[1]), float.Parse(rotData[2]));

                    UpdateRocket(position, rotation);
                }
            }
        }
    }

    void UpdateRocket(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    void OnApplicationQuit()
    {
        if (clientThread != null)
        {
            clientThread.Abort();
        }
        if (client != null)
        {
            client.Close();
        }
        if (listener != null)
        {
            listener.Stop();
        }
    }
}
