using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static discordBot2022.SongManager;
using static discordBot2022.memeManager;
namespace discordBot2022
{
    //this class is responsible for accepting user commands
    class commandsHandler
    {
        SongManager song_manager = new SongManager();
        audioPlayManager audio_manager = new audioPlayManager();
        memeManager meme_manager = new memeManager();
        User user_manager;

        bool isConnected = false;
        ulong connectedId;
        string ffmpeg_path;
        ulong playerId;
        bool tryAgain;

        Song current_song;
        Song last_song;
        List<Song> songList = new List<Song>();

        public async void Intialize()
        {
            meme_manager.Initialize(25).Wait();
            songList = song_manager.getSongs();
        }
        public async Task CommandHandler(SocketMessage arg)
        {
            if (!arg.Author.IsBot)
            {
                if (arg.Author.Id == playerId)
                {
                    if (arg.Content == "!стоп")
                    {
                        IVoiceChannel chnel = (arg.Author as IGuildUser).VoiceChannel;
                        arg.Channel.SendMessageAsync("Жаль! Заходите поиграть ещё!");
                        playerId = 0;
                        current_song = new Song("b", "b", "b");
                        await chnel.DisconnectAsync();

                    }
                    else
                    {
                        if (tryAgain)
                        {
                            if (arg.Content == "!да")
                            {
                                IVoiceChannel chnel = (arg.Author as IGuildUser).VoiceChannel;
                                if (chnel != null)
                                {
                                    tryAgain = false;
                                    Random rand = new Random();
                                    do
                                    {
                                        current_song = songList[rand.Next(0, songList.Count)];
                                    } while (current_song.artist == last_song.artist
                                    && current_song.name == last_song.name);

                                    arg.Channel.SendMessageAsync("Отлично! Что это за песня?");
                                    audio_manager.StartSong(chnel, false, arg.Channel, current_song);

                                }
                                else
                                {
                                    arg.Channel.SendMessageAsync("Что бы начать вы должны находиться в голосовом канале");
                                }

                            }
                            else
                            {
                                arg.Channel.SendMessageAsync("Жаль! Заходите поиграть ещё!");
                                playerId = 0;
                                current_song = new Song("b", "b", "b");
                            }
                        }
                        else
                        {
                            if (arg.Content.ToLower() == current_song.name.ToLower())
                            {
                                arg.Channel.SendMessageAsync("Верно!");
                                IVoiceChannel chnel = (arg.Author as IGuildUser).VoiceChannel;
                                chnel.DisconnectAsync();
                                arg.Channel.SendMessageAsync("Сыграем ещё?");
                                tryAgain = true;
                                last_song = current_song;

                            }
                            else
                            {
                                arg.Channel.SendMessageAsync("Неверно! Попробуйте послушать еще раз");
                                audio_manager.StartSong((arg.Author as IGuildUser).VoiceChannel, false, arg.Channel, current_song);
                            }
                        }
                    }
                }
                else
                {
                    switch (arg.ToString())
                    {
                        case "!игра угадай песню":
                            {
                                IVoiceChannel chnel = (arg.Author as IGuildUser).VoiceChannel;
                                if (chnel != null)
                                {
                                    connectedId = chnel.Id;
                                    isConnected = true;
                                    playerId = arg.Author.Id;
                                    Random rand = new Random();
                                    current_song = songList[rand.Next(0, songList.Count)];
                                    audio_manager.StartSong(chnel, true, arg.Channel, current_song);

                                }
                                else
                                {
                                    arg.Channel.SendMessageAsync("Что бы начать вы должны находиться в голосовом канале");
                                }
                                break;
                            }
                        case "!мем":
                            {
                                meme_manager.sendMeme(arg.Channel);
                                break;
                            }
                    }
                }
            }
        }

    }
}
