using UnityEngine.Networking;

public class NetworkData
{
    public NetworkData(int connectionId, byte[] buffer, NetworkEventType networkEventType)
    {
        this.ConnectionId = connectionId;
        this.Buffer = buffer;
        this.NetworkEventType = networkEventType;
    }

    public int ConnectionId
    {
        get;
        private set;
    }

    public byte[] Buffer
    {
        get;
        private set;
    }

    public NetworkEventType NetworkEventType
    {
        get;
        private set;
    }

    public string ConvertBufferToString()
    {
        var message = System.Text.Encoding.UTF8.GetString(Buffer).ToCharArray();
        var result = new System.Text.StringBuilder();

        foreach (var c in message)
        {
            if (c != '\0')
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}
