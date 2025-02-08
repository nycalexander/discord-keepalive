using System.Text.Json.Serialization;

[JsonConverter(typeof(IntOrEnumConverter))]
public enum GatewayOpcodes : int
{
    Unknown = -1,
    Dispatch = 0,
    Heartbeat = 1,
    Identify = 2,
    PresenceUpdate = 3,
    VoiceStateUpdate = 4,
    VoiceServerPing = 5,
    Resume = 6,
    Reconnect = 7,
    RequestGuildMembers = 8,
    InvalidSession = 9,
    Hello = 10,
    HeartbeatAck = 11,
    SyncGuild = 12,
    SyncCall = 13
}

public enum ColorPalette
{
    Unknown = ConsoleColor.Gray,
    Dispatch = ConsoleColor.White,
    Heartbeat = ConsoleColor.Red,
    Identify = ConsoleColor.Blue,
    PresenceUpdate = ConsoleColor.White,
    VoiceStateUpdate = ConsoleColor.White,
    VoiceServerPing = ConsoleColor.White,
    Resume = ConsoleColor.White,
    Reconnect = ConsoleColor.White,
    RequestGuildMembers = ConsoleColor.DarkGreen,
    InvalidSession = ConsoleColor.Gray,
    Hello = ConsoleColor.Yellow,
    HeartbeatAck = ConsoleColor.DarkRed,
    SyncGuild = ConsoleColor.Green,
    SyncCall = ConsoleColor.DarkCyan
}

public class DiscordPayload
{
    /// <summary>
    /// Gateway opcode, which indicates the payload type
    /// </summary>
    public GatewayOpcodes op { get; set; }

    /// <summary>
    /// Event data
    /// </summary>
    public object? d { get; set; } 

    /// <summary>
    /// Sequence number of event used for resuming sessions and heartbeating
    /// </summary>
    public int? s { get; set; }

    /// <summary>
    /// Event name
    /// </summary>
    public string? t { get; set; }
}


public class IdentifyEvent 
{
    public string token { get; set; }
    public Properties properties { get; set; }
    public bool? compress { get; set; }
    public int? large_threshold { get; set; }
    public int[]? shard { get; set; }
    public Presence? presence { get; set; }
    public int intents { get; set; }
}

public class HeartbeatEvent
{
    public new GatewayOpcodes op { get; set; }
    public new int? d { get; set; }
}

public class HelloEvent
{
    public new GatewayOpcodes op { get; set; }
    public new HelloEventData d { get; set; }
}

public class HelloEventData
{
    public int heartbeat_interval { get; set; }
}

public class Properties
{
    public string os { get; set; }
    public string browser { get; set; }
    public string device { get; set; }
}

public class Presence
{
    public Activity[] activities { get; set; }
    public string status { get; set; }
    public int since { get; set; }
    public bool afk { get; set; }
}

public class Activity
{
    public string name { get; set; }
    public int type { get; set; }
}