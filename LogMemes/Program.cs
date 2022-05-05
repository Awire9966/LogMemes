using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Net;
using System.IO;
using Discord.Webhook;
using DWP;
namespace LogMemes
{

    class Program
    {

        static string[] config = File.ReadAllLines(Environment.CurrentDirectory + @"\LogMemes.config");
        private readonly DiscordSocketClient _client;


        static void Main()
        {
            try
            {
                new Program()
                    .MainAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                Webhook hook = new Webhook();
                hook.SendMessage(config[3], ex.ToString(), config[4], "Error Catcher");

            }
        }

        public Program()
        {
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = new DiscordSocketClient();

            // Subscribing to client events, so that we may receive them whenever they're invoked.
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
            
        }

        public async Task MainAsync()
        {
            
            
                // Tokens should be considered secret data, and never hard-coded.
                await _client.LoginAsync(TokenType.Bot, config[0]);
                // Different approaches to making your token a secret is by putting them in local .json, .yaml, .xml or .txt files, then reading them on startup.

                await _client.StartAsync();


                await Task.Delay(Timeout.Infinite);
            
         
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

       
        private Task ReadyAsync()
        {
            _client.SetStatusAsync(UserStatus.Online);
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

  
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            
            if (message.Author.Id == _client.CurrentUser.Id)
                return;
            string sentence = " ";
            char[] charArr = sentence.ToCharArray();


            string[] fullmessage = message.Content.Split(charArr);


            if (message.Attachments.Count > 0)
            {
                     DirectoryInfo dinf = new DirectoryInfo(config[1]);
                    FileInfo[] currentlist = dinf.GetFiles();
                string[] channels = File.ReadAllLines(config[2]);


                foreach (string id in channels)
                {
                    if (message.Channel.Id == ulong.Parse(id))
                    {
                        using (var client = new WebClient())
                        {
                            foreach (IAttachment attachment in message.Attachments)
                            {
                                int leftover = currentlist.Length + 1;
                                string savepath = leftover.ToString();
                                string filetype = Path.GetExtension(attachment.Url);
                                if (filetype == ".png")
                                {
                                    client.DownloadFileAsync(new Uri(attachment.Url), config[1] + savepath + ".png");
                                     await message.Channel.SendMessageAsync("Meme Sent. Meme ID:" + savepath);
                                }
                                else
                                {
                                    await message.Channel.SendMessageAsync("Meme is not a png. Please use a converter!");
                                }
                            }
                        }
                    }
                }
                if (fullmessage[0] == config[5] + "addmemechannel")
                {
                    File.WriteAllText(config[2], File.ReadAllText(config[2] + "\n" + fullmessage[1]));
                    await message.Channel.SendMessageAsync("Channel Added: <#" + fullmessage[1] + ">");
                }

                if (fullmessage[0] == config[5] + "getmeme")
                {
                   
                    await message.Channel.SendFileAsync(new FileAttachment(config[1] + fullmessage[1] + ".png"));
                }
                if (fullmessage[0] == config[5] + "help")
                {

                    await message.Channel.SendMessageAsync("Prefix: "+ config[5] + "\nCommands: help -- Shows this message \ngetmeme <id> -- get a meme and post it in this channel.\naddmemechannel <channelid> -- add a channel to be loged" );
                }
            }
        }
     


    }
}