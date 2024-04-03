using System.Net.Sockets;
using System.Net;
using System.Text;

public class UpdServer
{
    public Socket _socket;
    private const int bufSize = 8 * 1024;
    private State state = new State();
    private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
    private AsyncCallback recv = null;

    public class State
    {
        public byte[] buffer = new byte[bufSize];
    }

    public UpdServer(IPEndPoint endpoint)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
        _socket.Bind(endpoint);
        Receive();
    }

    public UpdServer(int port) : this(new IPEndPoint(IPAddress.Any, port))
    {
    }

    public void Stop()
    {
        _socket.Close();
    }

    private void Receive()
    {
        _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
        {
            try
            {
                State so = (State)ar.AsyncState!;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                var str = Encoding.ASCII.GetString(so.buffer, 0, bytes);
                Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, str);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
            }
            catch { }

        }, state);
    }
}
