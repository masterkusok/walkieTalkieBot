
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace discordBot2022
{

    class SongManager
    {
        public struct Song
        {
            public string name;
            public string artist;
            public string path { get; set; }

            public Song(string _Name, string _Artist, string _Path)
            {
                name = _Name;
                artist = _Artist;
                path = _Path;
            }
        }
        public List<Song> getSongs()
        {
            List<Song> songList = new List<Song>();
            int numberOfSongs = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\audioInfos").GetFiles().Length;
            string[] fileNames = new string[numberOfSongs];
            Song temp = new Song();
            for (int i = 0; i < numberOfSongs; i++)
            {
                using (StreamReader sr = new StreamReader(@Directory.GetCurrentDirectory() + "\\audioInfos\\" + i + ".txt", Encoding.Default))
                {
                    temp.artist = sr.ReadLine();
                    temp.name = sr.ReadLine();
                    temp.path = Directory.GetCurrentDirectory() + "\\Audios\\" + sr.ReadLine();
                }
                songList.Add(temp);
            }
            return songList;
        }
    }
}
