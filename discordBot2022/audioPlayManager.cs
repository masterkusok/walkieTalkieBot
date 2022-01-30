using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using static discordBot2022.SongManager;

namespace discordBot2022
{
    class audioPlayManager
    {
        //here i start ffmpeg
        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            }
            );
        }
        
        [Discord.Commands.Command(RunMode = RunMode.Async)]
        public async Task joinVoiceChannel(IVoiceChannel channel, string path)
        {
            try
            {
                Console.WriteLine("Joining channel - " + channel.Name);
                var audioConnect = await channel.ConnectAsync();
                StreamAsync(path, audioConnect);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public async Task StartSong(IVoiceChannel chnel, bool firstLaunch, ISocketMessageChannel msg_chnel, Song songToPlay)
        {

            if (firstLaunch)
            {
                msg_chnel.SendMessageAsync("Начнём!");
                await Task.Delay(100);
                msg_chnel.SendMessageAsync("Что это за песня?");
                joinVoiceChannel(chnel, songToPlay.path);
            }
            else
            {
                chnel.DisconnectAsync();
                joinVoiceChannel(chnel, songToPlay.path);

            }
        }

        private async Task StreamAsync(string path, Discord.Audio.IAudioClient audioClient)
        {
            try
            {
                using (var ffmpeg = CreateStream(path))
                using (var output = ffmpeg.StandardOutput.BaseStream)
                using (var discord = audioClient.CreatePCMStream(Discord.Audio.AudioApplication.Mixed))
                {

                    try
                    {
                        await output.CopyToAsync(discord);
                    }
                    finally
                    {
                        await discord.FlushAsync();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
