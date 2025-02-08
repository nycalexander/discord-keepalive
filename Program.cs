using System.Text;

internal class Program
{
    async static Task Main(string[] args)
    {
        Console.WriteLine("Discord Keep-alive");
        Console.WriteLine();

        if (File.Exists("token.txt"))
        {
            string tokenUser = await File.ReadAllTextAsync("token.txt", Encoding.UTF8);
            DiscordWebSocket socket = new(tokenUser);
            await socket.InitializeWebsocketAsync();
            await Task.Delay(-1);
        }
        else
        {
            await File.WriteAllTextAsync("token.txt", "EXAMPLE_DISCORD_TOKEN", Encoding.UTF8);
            DiscordWebSocket.Log("MISSING_TOKEN", "Created a token.txt file. Please fill it with your Discord token and restart the program.", ConsoleColor.Red);
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}