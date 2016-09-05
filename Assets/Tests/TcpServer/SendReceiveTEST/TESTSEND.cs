using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Text;

public class TESTSEND : MonoBehaviour
{
    public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    void Start()
    {
        
    }

    void BeginConnect(string ip)
    {
        socket.BeginConnect(ip, TESTRECEIVE.Port, new AsyncCallback(EndConnect), socket);
    }

    void EndConnect(IAsyncResult result)
    {
        try
        {
            socket.EndConnect(result);

            var message = new StringBuilder();

            for (int i = 0; i < 10000; i++)
            {
                message.AppendLine("Baba qga obicha mnogo da gotvi deca!");
            }

            var messageBuffer = Encoding.UTF8.GetBytes(message.ToString());
            var messageLengthBuffer = BitConverter.GetBytes(messageBuffer.Length);
            var buffer = new byte[messageBuffer.Length + messageLengthBuffer.Length];
            var state = new SendMessageState() { Client = socket, DataToSend = buffer };

            Buffer.BlockCopy(messageLengthBuffer, 0, buffer, 0, messageLengthBuffer.Length);
            Buffer.BlockCopy(messageBuffer, 0, buffer, messageLengthBuffer.Length, messageBuffer.Length);

            socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(EndSend), state);

        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);    
        }
    }

    void EndSend(IAsyncResult result)
    {
        try
        {
            var state = (SendMessageState)result.AsyncState;
            var sentCount = socket.EndSend(result);

            state.DataSentLength += sentCount;

            if (state.DataSentLength != state.DataToSend.Length)
            {
                var dataToSendSize = state.DataToSend.Length - state.DataSentLength;
                socket.BeginSend(state.DataToSend, state.DataSentLength, dataToSendSize, SocketFlags.None, new AsyncCallback(EndSend), state);
            }    
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    string ip = "";

    void OnGUI()
    {
        ip = GUI.TextField(new Rect(200, 50, 150, 200), ip);

        if (GUI.Button(new Rect(200, 270, 200, 200), "Connect and send"))
        {
            BeginConnect(ip);
        }
    }
}
