using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class RocketControl : MonoBehaviour
{
    public Rigidbody rb;
    public float thrustForce = 700000f; // Thrust force in Newtons
    public Transform thrustPoint; // Point where thrust is applied

    TcpListener listener;
    TcpClient client;
    NetworkStream stream;

    private void Start()
    {
        // Start the TCP listener
        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 25001);
        listener.Start();
        Debug.Log("Server started");

        // Accept a client connection
        client = listener.AcceptTcpClient();
        stream = client.GetStream();
        Debug.Log("Client connected");

        // Initial rigidbody setup
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (stream.DataAvailable)
        {
            // Read data from Python
            byte[] bytes = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(bytes, 0, client.ReceiveBufferSize);
            string dataReceived = Encoding.UTF8.GetString(bytes, 0, bytesRead).Trim();
            Debug.Log("Received from Python: " + dataReceived);

            // Parse throttle value
            float throttle = float.Parse(dataReceived);
            ApplyThrust(throttle);
        }

        // Send position and rotation back to Python
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        string dataToSend = $"{position.x},{position.y},{position.z};{rotation.eulerAngles.x},{rotation.eulerAngles.y},{rotation.eulerAngles.z}";
        byte[] sendData = Encoding.UTF8.GetBytes(dataToSend);
        stream.Write(sendData, 0, sendData.Length);
    }

    void ApplyThrust(float throttle)
    {
        // Calculate thrust vector
        Vector3 thrustVector = transform.up * thrustForce * throttle;
        rb.AddForceAtPosition(thrustVector, thrustPoint.position, ForceMode.Force);
    }

    private void OnDestroy()
    {
        // Clean up network resources
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();
        if (listener != null)
            listener.Stop();
    }
}
