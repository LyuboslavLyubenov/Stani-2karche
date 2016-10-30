using System.Net.Sockets;

public static class SocketExtensions
{
    
    public static bool IsConnected(this Socket socket)
    {
        try
        {
            bool part1 = socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);
            return !(part1 && part2);
        }
        catch
        {
            
        }

        return false;
    }
}
