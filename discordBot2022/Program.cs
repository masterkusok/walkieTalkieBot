using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;


namespace discordBot2022
{
    class Program
    {
        
        commandsHandler handler = new commandsHandler();

        DiscordSocketClient client;
        
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();
        private async Task MainAsync()
        {
            handler.Intialize();   

            string token = "Your discord application token here";
            client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            client.Log +=  WriteLogs;
            client.MessageReceived += handler.CommandHandler;

            Console.ReadLine();
        }


        private Task WriteLogs(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }

    }
}
