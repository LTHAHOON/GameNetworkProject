using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace UdpPeer
{
    public struct PeerMessageData()
    {
        public string PeerID { get; set; } = "";
        public int PeerMessageNumber { get; set; } = 0;
        public Vector2 PeerPos { get; set; } = new(0, 0);

        public readonly string ToJson(JsonSerializerOptions? jsonSerializerOptions = null)
        {
            return JsonSerializer.Serialize(this, jsonSerializerOptions);
        }
    }

    internal class Program
    {
        private static JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            IncludeFields = true
        };

        private static readonly UTF8Encoding UTF8 = new(true);
        private static readonly List<PeerMessageData> _ignoredLocalPeerMsgList = new();
        private static PeerMessageData _curLocalPeerMsgData;
        private static PeerMessageData _curRemotePeerMsgData;
        private const int MAX_NAME_SAFE_BUFFERSIZE = 32;
        private const int MAX_NAME_SAFE_SIZE = 10;

        public static async Task Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("사용법: dotnet run <내 포트> <상대 포트> <이름>");
                return;
            }

            if (!int.TryParse(args[0], out int localPort) ||
                !int.TryParse(args[1], out int remotePort) ||
                string.IsNullOrWhiteSpace(args[2]))
            {
                Console.WriteLine("사용법: dotnet run <내 포트> <상대 포트> <이름>");
                return;
            }

            string peerName = args[2];
            int nameBufferLength = UTF8.GetByteCount(peerName);
            while (peerName.Length > MAX_NAME_SAFE_SIZE || nameBufferLength > MAX_NAME_SAFE_BUFFERSIZE)
            {
                Console.Write($"다시 이름을 입력해주세요({MAX_NAME_SAFE_SIZE}자 이하 / 나가기: 0): ");
                peerName = Console.ReadLine() ?? "";
                if (int.TryParse(peerName, out int choiceNumber))
                {
                    if(choiceNumber == 0)
                    {
                        return;
                    }
                }
                nameBufferLength = UTF8.GetByteCount(peerName);
                
                if (peerName.Length > MAX_NAME_SAFE_SIZE)
                {
                    Console.WriteLine("최대 글자수를 넘었습니다.");
                    continue;
                }
                else if(nameBufferLength > MAX_NAME_SAFE_BUFFERSIZE)
                {
                    Console.WriteLine("최대 버퍼 사이즈에 도달하였습니다.");
                    continue;
                }
                break;
            }

            _curLocalPeerMsgData.PeerID = args[2];

            IPEndPoint localEndPoint = new(IPAddress.Loopback, localPort);
            IPEndPoint remoteEndPoint = new(IPAddress.Loopback, remotePort);
            using UdpClient client = new(localEndPoint);

            Task sendTask = Task.Run(() => SendPeerMsgProcess(client, remoteEndPoint));
            Task receiveTask = ReceivePeerMsgProcess(client);
            await Task.WhenAll(sendTask, receiveTask);
        }

        private static (bool IsSendMode, bool IsExit) SendChoiceProcess(UdpClient client)
        {
            Console.WriteLine("0: 전송모드 | 1: 무시된 메시지 보기 | 2: 나가기");
            if (!int.TryParse(Console.ReadLine(), out int choiceNumber))
            {
                Console.WriteLine("숫자를 입력하세요.");
                return (false, false);
            }
            if (choiceNumber == 1)
            {
                foreach (PeerMessageData ignoredPeerMsgData in _ignoredLocalPeerMsgList)
                {
                    Console.WriteLine($"무시된 {ignoredPeerMsgData.PeerMessageNumber}번 {ignoredPeerMsgData.PeerID}의 위치: " +
                                    $"({ignoredPeerMsgData.PeerPos.X}, {ignoredPeerMsgData.PeerPos.Y})");
                }
                Console.WriteLine("0: 뒤로가기 | 1: 무시된 메시지 정리하고 뒤로가기");
                if (!int.TryParse(Console.ReadLine(), out choiceNumber))
                {
                    Console.WriteLine("다시 시도해주세요.");
                    return (false, false);
                }
                if (choiceNumber == 1)
                {
                    _ignoredLocalPeerMsgList.Clear();
                }
                return (false, false);
            }

            if (choiceNumber == 2)
            {
                Console.WriteLine("프로그램을 종료하였습니다.");
                return (false, true);
            }

            if (choiceNumber != 0)
            {
                Console.WriteLine("다시 시도해주세요. ");
                return (false, false);
            }
            return (true, false);
        }

        public static async Task SendPeerMsgProcess(UdpClient client, IPEndPoint remoteEndPoint)
        {
            try
            {
                while (true)
                {
                    var sendChoice = SendChoiceProcess(client);
                    if (!sendChoice.IsSendMode && !sendChoice.IsExit)
                    {
                        continue;
                    }
                    else if (sendChoice.IsExit)
                    {
                        client.Dispose();
                        return;
                    }

                    ++_curLocalPeerMsgData.PeerMessageNumber;
                    Console.Write($"변경할 {_curLocalPeerMsgData.PeerMessageNumber}번 {_curLocalPeerMsgData.PeerID}의 위치 X: ");
                    if (!int.TryParse(Console.ReadLine(), out int x))
                    {
                        Console.WriteLine("숫자를 입력하세요.");
                        --_curLocalPeerMsgData.PeerMessageNumber;
                        continue;
                    }
                    Console.Write($"변경할 {_curLocalPeerMsgData.PeerMessageNumber}번 {_curLocalPeerMsgData.PeerID}의 위치 Y: ");
                    if (!int.TryParse(Console.ReadLine(), out int y))
                    {
                        Console.WriteLine("숫자를 입력하세요.");
                        --_curLocalPeerMsgData.PeerMessageNumber;
                        continue;
                    }

                    if (_curLocalPeerMsgData.PeerPos.X == x && _curLocalPeerMsgData.PeerPos.Y == y)
                    {
                        Console.WriteLine("해당 보낸 메시지는 무시되었습니다.");
                        PeerMessageData ignoredPeerMsgData = _curLocalPeerMsgData;
                        ignoredPeerMsgData.PeerPos = new(x, y);
                        _ignoredLocalPeerMsgList.Add(ignoredPeerMsgData);
                        continue;
                    }
                    _curLocalPeerMsgData.PeerPos = new(x, y);

                    string localPeerMsgJson = _curLocalPeerMsgData.ToJson(_jsonSerializerOptions);
                    byte[] localPeerMsgByte = UTF8.GetBytes(localPeerMsgJson);
                    await client.SendAsync(localPeerMsgByte, localPeerMsgByte.Length, remoteEndPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task ReceivePeerMsgProcess(UdpClient client)
        {
            try
            {
                while (true)
                {
                    UdpReceiveResult receivedResult = await client.ReceiveAsync();
                    byte[] receivedPeerMsgByte = receivedResult.Buffer;
                    string receivedPeerMsgJson = UTF8.GetString(receivedPeerMsgByte);
                    PeerMessageData? peerMessageData = JsonSerializer.Deserialize<PeerMessageData>(receivedPeerMsgJson, _jsonSerializerOptions);
                    if (peerMessageData is null)
                    {
                        Console.WriteLine("Warning: PeerMessageData로 변환할 수 없습니다.");
                        continue;
                    }
                    _curRemotePeerMsgData = peerMessageData.Value;
                    Console.WriteLine($"{_curRemotePeerMsgData.PeerMessageNumber}번 {_curRemotePeerMsgData.PeerID}의 위치: " +
                                    $"({_curRemotePeerMsgData.PeerPos.X}, {_curRemotePeerMsgData.PeerPos.Y})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

}
