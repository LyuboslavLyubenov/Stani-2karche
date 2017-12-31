namespace IO
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using DTOs;

    using Interfaces;

    using Utils;

    public class LeaderboardDataManipulator : ILeaderboardDataManipulator
    {
        private const string FilePath = "LevelData\\теми";
        private const string FileName = "Rating.csv";
        
        private IList<PlayerScore> leaderboard = new List<PlayerScore>();

        private bool loaded = false;
        
        public string LevelCategory
        {
            get; set;
        }

        public bool AllowDublicates
        {
            get; set;
        }

        public IList<PlayerScore> Leaderboard
        {
            get
            {
                return !this.loaded ? null : this.leaderboard;
            }
        }

        public bool Loaded
        {
            get
            {
                return this.loaded;
            }
        }

        private string GetEndPath()
        {
            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..";
            return string.Format("{0}\\{1}\\{2}\\{3}", execPath, FilePath, this.LevelCategory, FileName);
        }

        private void CreateLeaderboardFileIfDoesntExists(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }

        private IEnumerator LoadLeaderboardAsync()
        {
            yield return null;

            var endPath = this.GetEndPath();

            this.CreateLeaderboardFileIfDoesntExists(endPath);

            var sr = new StreamReader(endPath);

            while (true)
            {
                var line = sr.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                var playerScoreData = line.Split(',');
                var name = playerScoreData[0];
                var score = int.Parse(playerScoreData[1]);
                PlayerScore playerScore;

                if (playerScoreData.Length == 3)
                {
                    var creationDate = DateTime.Parse(playerScoreData[2]);
                    playerScore = new PlayerScore(name, score, creationDate);
                }
                else
                {
                    playerScore = new PlayerScore(name, score);
                }

                this.leaderboard.Add(playerScore);
            }

            sr.Close();

            this.leaderboard = this.leaderboard.OrderByDescending(ps => ps.Score).ToList();
            this.loaded = true;
        }

        private IEnumerator SavePlayerScoreAsync(PlayerScore playerScore)
        {
            var path = this.GetEndPath();

            this.CreateLeaderboardFileIfDoesntExists(path);

            var playersScore = File.ReadAllLines(path).ToList();

            int playerIndex = -1;

            if (!this.AllowDublicates)
            {
                for (int i = 0; i < playersScore.Count; i++)
                {
                    var scoreData = playersScore[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (scoreData[0] == playerScore.PlayerName &&
                        int.Parse(scoreData[1]) < playerScore.Score)
                    {
                        playerIndex = i;
                        break;
                    }
                }
            }

            if (playerIndex == -1)
            {
                yield break;
            }

            var data = new List<string>();
            var propertyInfos = playerScore.GetType().GetProperties();

            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var propertyValue = propertyInfos[i].GetValue(playerScore, null);
                data.Add(propertyValue.ToString());
            }

            var score = string.Join(",", data.ToArray());

            if (playerIndex > -1)
            {
                playersScore[playerIndex] = score;
            }
            else
            {
                playersScore.Add(score);
            }

            File.WriteAllLines(path, playersScore.ToArray());

            yield return null;
        }

        public void SavePlayerScore(PlayerScore playerScore)
        {
            if (!this.Loaded)
            {
                throw new Exception("Still loading score");
            }

            this.leaderboard.Add(playerScore);
            ThreadUtils.Instance.RunOnBackgroundThread(this.SavePlayerScoreAsync(playerScore));
        }

        public void LoadDataAsync()
        {
            ThreadUtils.Instance.RunOnBackgroundThread(this.LoadLeaderboardAsync());
        }
    }
}
