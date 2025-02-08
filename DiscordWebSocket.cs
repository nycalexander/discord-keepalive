using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

public class DiscordWebSocket
{
    private ClientWebSocket ws = new();
    private int heartbeatLimit = 0;
    private int? lastEvent = null;
    private bool loggedIn = false;
    private string userToken = string.Empty;

    public DiscordWebSocket(string _token)
    {
        userToken = _token;
    }

    private async Task SendResponseAsync()
    {
        object jsonConvert = new HeartbeatEvent()
        {
            op = GatewayOpcodes.Heartbeat,
            d = lastEvent
        };

        JsonSerializerOptions jso = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        if (!loggedIn)
        {
            jsonConvert = new DiscordPayload()
            {
                op = GatewayOpcodes.Identify,
                d = new IdentifyEvent()
                {
                    compress = false,
                    token = userToken,
                    properties = new()
                    {
                        os = "Windows",
                        browser = "Discord Client"
                    },
                    presence = new()
                    {
                        afk = false,
                        status = "dnd",
                        since = 91879201,
                        activities = new Activity[1]
                        {
                            new()
                            {
                                name = "Cards against Humanity",
                                type = 0
                            }
                        }
                    }
                }
            };

            jso.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            loggedIn = true;
        }
        
        string serialize = JsonSerializer.Serialize(jsonConvert, jso);
        byte[] bytes = Encoding.UTF8.GetBytes(serialize);
        ArraySegment<byte> bytesToSend = new(bytes);
        await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        Log("WEBSOCKET_SEND", serialize, ConsoleColor.DarkBlue);

        await Task.Delay(heartbeatLimit);
    }

    private async Task HandleResponseAsync()
    {
        ArraySegment<byte> bytesReceived = new(new byte[1024 * 1024 * 10]); // ~10 MB
        WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);

        if (bytesReceived.Array == null)
            return;

        string utfText = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);

        DiscordPayload? payload = JsonSerializer.Deserialize<DiscordPayload>(utfText);

        if (payload != null)
        {
            switch (payload.op)
            {
                case GatewayOpcodes.Hello:
                    HelloEvent? helloEvent = JsonSerializer.Deserialize<HelloEvent>(utfText);
                    heartbeatLimit = helloEvent!.d.heartbeat_interval;
                    break;
                case GatewayOpcodes.Dispatch:
                    lastEvent = payload.s;
                    break;
            }
            Log(payload.op, utfText);
        }
    }

    private bool IsWebsocketOpen() => ws?.State == WebSocketState.Open;

    private async Task StartEventsAsync()
    {
        await Task.Run(() => HandleResponseAsync());
        await Task.Run(() => SendResponseAsync());
    }

    public async Task InitializeWebsocketAsync()
    {
        Uri serverUri = new("wss://gateway.discord.gg/?v=10&encoding=json");
        await ws.ConnectAsync(serverUri, CancellationToken.None);
        if (ws.State == WebSocketState.Open)
        {
            Log("WEBSOCKET_CONNECT", $"Websocket was opened! URL: {serverUri.AbsoluteUri}", ConsoleColor.DarkBlue);
            
            while (IsWebsocketOpen())
                await StartEventsAsync();

            Log("WEBSOCKET_CLOSED", "Websocket was closed!", ConsoleColor.DarkBlue);
        }
    }

    private static void Log(GatewayOpcodes opcode, string message)
    {
        string opcodePrefix = Enum.GetName(typeof(GatewayOpcodes), opcode)!;
        ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ColorPalette), opcodePrefix!);
        Log(opcodePrefix, message, color);
    }

    public static void Log(string prefix, string message, ConsoleColor color) 
    {
        ConsoleColor beforeColor = Console.ForegroundColor;
        Console.Write("[");
        Console.ForegroundColor = color;
        Console.Write(prefix.ToUpper());
        Console.ForegroundColor = beforeColor;
        Console.Write($"] => {message}");
        Console.WriteLine();
    } 
    
    public static void Log(string prefix, string message) => Console.WriteLine($"[{prefix.ToUpper()}] => {message}");
}