using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UtageConverter
{
    public class BuddiesDataLoader
    {
        private HashSet<int> jacketMusicIds = new();
        private HashSet<string> fumenMusicIds = new();
        private HashSet<int> movieMusicIds = new();

        public BuddiesDataLoader(string packageFolder)
        {
            Task.WaitAll(new[]
            {
                Task.Run(() => LoadJackets(packageFolder)),
                Task.Run(() => LoadMusic(packageFolder)),
                Task.Run(() => LoadMovie(packageFolder)),
            });
        }

        private void LoadMovie(string packageFolder)
        {
            movieMusicIds.Clear();

            var regex = new Regex(@"(\d+).dat");
            var movies = Directory.GetFiles(packageFolder, "*.dat", SearchOption.AllDirectories);
            foreach (var match in movies.Select(x => regex.Match(Path.GetFileName(x))).Where(x => x.Success))
            {
                var musicId = int.Parse(match.Groups[1].Value);
                movieMusicIds.Add(musicId);
            }
        }

        private void LoadMusic(string packageFolder)
        {
            fumenMusicIds.Clear();

            var regex = new Regex(@"(\d+)_(\d+)\w*\.ma2");
            var jackets = Directory.GetFiles(packageFolder, "*.ma2", SearchOption.AllDirectories);
            foreach (var match in jackets.Select(x => regex.Match(Path.GetFileName(x))).Where(x => x.Success))
            {
                var musicId = int.Parse(match.Groups[1].Value);
                var diffId = int.Parse(match.Groups[2].Value);

                fumenMusicIds.Add($"{musicId}_{diffId}");
                fumenMusicIds.Add($"{musicId}");
            }
        }

        private void LoadJackets(string packageFolder)
        {
            jacketMusicIds.Clear();

            var regex = new Regex(@"ui_jacket_(\d+)(_s)?\.ab");
            var jackets = Directory.GetFiles(packageFolder, "ui_jacket_*.ab", SearchOption.AllDirectories);
            foreach (var musicId in jackets.Select(x => regex.Match(Path.GetFileName(x))).Where(x => x.Success).Select(x => int.Parse(x.Groups[1].Value)))
                jacketMusicIds.Add(musicId);
        }

        public bool ContainJacket(int musicId) => jacketMusicIds.Contains(musicId);
        public bool ContainMusic(int musicId) => fumenMusicIds.Contains($"{musicId}");
        public bool ContainMovie(int musicId) => movieMusicIds.Contains(musicId);
        public bool ContainMusicDiff(int musicId, int diffId) => fumenMusicIds.Contains($"{musicId}_{diffId}");
    }
}
