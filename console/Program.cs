using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using footytips.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace footytips
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Program
    {
        private static string GetNrldataJsonFileName(int round)
        {
            return $"./nrlData-{round}.json";
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        static void Main(string[] args)
        {
            var round = Convert.ToInt32(14);
            GetLatestDataFromNrlSite(round);

            var data = JsonConvert.DeserializeObject<CombinedData>(File.ReadAllText(GetNrldataJsonFileName(round)));

            RunTips(data.CurrentRound.ToList(), data.Ladder.ToList(), data.LastRound.ToList());

            var roundData = CreateCsv(round);

            var currentRound = GetCurrentRound(roundData);

            var learning = new MachineLearning();
            var training = learning.TrainData("nrlData.csv");


            var predictions = learning.Predict(currentRound, training);
            Console.WriteLine();
            Console.WriteLine("Nrl Predictions");
            Console.WriteLine("---------------------");

            var sentimentsAndPredictions =
                currentRound.Zip(predictions, (sentiment, prediction) => (sentiment, prediction));

            foreach (var item in sentimentsAndPredictions)
            {
                Console.WriteLine(
                    $"HomeTeam :{item.sentiment.HomeTeam} ({item.sentiment.HomeTeamLastWeekScore}) vs Away Team {item.sentiment.AwayTeam} ({item.sentiment.AwayTeamLastWeekScore})| Prediction: {item.prediction.HomeTeamWin}");
            }

            Console.WriteLine();
        }

        private static List<NrlResult> GetCurrentRound(IEnumerable<NrlResult2> roundData)
        {
            var currentRound = roundData.Select(x => new NrlResult
            {
                AwayTeam = x.AwayTeam,
                HomeTeam = x.HomeTeam,
                HomeTeamAwayForm = x.HomeTeamAwayForm,
                AwayTeamAwayForm = x.AwayTeamAwayForm,
                AwayTeamHomeForm = x.AwayTeamHomeForm,
                HomeTeamHomeForm = x.HomeTeamHomeForm,
                PreviousWeeksAwayAgainst = x.PreviousWeeksAwayAgainst,
                PreviousWeeksAwayFor = x.PreviousWeeksAwayFor,
                PreviousWeeksHomeAgainst = x.PreviousWeeksHomeAgainst,
                PreviousWeeksHomeFor = x.PreviousWeeksHomeFor,
                AwayTeamLastWeekScore = x.AwayTeamLastWeekScore,
                HomeTeamLastWeekScore = x.HomeTeamLastWeekScore
            }).ToList();
            return currentRound;
        }

        private static IEnumerable<NrlResult2> CreateCsv(int currentRound)
        {
            var result = new List<NrlResult2>();
            for (var i = 1; i < currentRound; i++)
            {
                CreateFlatModelForRound(i, result);
            }

            using (TextWriter streamWriter =
                new StreamWriter("nrlData.csv"))

            {
                var writer = new CsvHelper.CsvWriter(streamWriter);
                writer.WriteRecords(result);
            }

            var result2 = new List<NrlResult2>();
            CreateFlatModelForRound(currentRound, result2);
            return result2;
        }

        private static void CreateFlatModelForRound(int i, List<NrlResult2> result)
        {
            var data = JsonConvert.DeserializeObject<CombinedData>(
                File.ReadAllText(GetNrldataJsonFileName(i)));


            var ladderList = data.Ladder.ToList();
            var previousWeek = data.LastRound.ToList();
            var round = i;
            result.AddRange(from game in data.CurrentRound
                let homeTeamLadder = ladderList.GetLadder(game.HomeTeam.NickName)
                let awayTeamLadder = ladderList.GetLadder(game.AwayTeam.NickName)
                let previousRoundHome = previousWeek.GetLastRound(game.HomeTeam.NickName)
                let previousRoundAway = previousWeek.GetLastRound(game.AwayTeam.NickName)
                select new NrlResult2
                {
                    Round = round,
                    HomeTeam = game.HomeTeam.NickName,
                    AwayTeam = game.AwayTeam.NickName,
                    Label = game.HomeTeam.Score - game.AwayTeam.Score, // ? "Home" : "Away"
                    PreviousWeeksHomeFor = homeTeamLadder.ForPoints,
                    PreviousWeeksHomeAgainst = homeTeamLadder.AgainstPoints,
                    HomeTeamAwayForm = homeTeamLadder.AwayForm,
                    HomeTeamHomeForm = homeTeamLadder.HomeForm,
                    PreviousWeeksAwayFor = awayTeamLadder.ForPoints,
                    PreviousWeeksAwayAgainst = awayTeamLadder.AgainstPoints,
                    AwayTeamAwayForm = awayTeamLadder.AwayForm,
                    AwayTeamHomeForm = awayTeamLadder.HomeForm,
                    HomeTeamLastWeekScore = GetScore(game.HomeTeam.NickName, previousRoundHome),
                    AwayTeamLastWeekScore = GetScore(game.AwayTeam.NickName, previousRoundAway)
                });
        }

        private static float GetScore(string teamName, NrlMatches previousRoundHome)
        {
            if (previousRoundHome == null)
                return 0;

            if (previousRoundHome.HomeTeam.NickName == teamName)
            {
                return previousRoundHome.HomeTeam.Score - previousRoundHome.AwayTeam.Score;
            }

            return previousRoundHome.AwayTeam.Score - previousRoundHome.HomeTeam.Score;
        }

        private static void GetLatestDataFromNrlSite(int round)
        {
            var howOld = DateTime.Now - File.GetLastWriteTime(GetNrldataJsonFileName(round));
            if (howOld.TotalMinutes <= 300)
            {
                Console.WriteLine("Using Existing Json File");
                return;
            }

            using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
            {
                var currentRound = Convert.ToInt32(round);

                var ladderList = GetLadderList(driver, currentRound - 1).ToList();
                var lastWeeksResults = GetDraw(driver, currentRound - 1).ToList();
                var thisWeeksResults = GetDraw(driver, currentRound);

                var combinedData = new CombinedData
                {
                    CurrentRound = thisWeeksResults,
                    Ladder = ladderList,
                    LastRound = lastWeeksResults
                };
                Console.WriteLine("Writing New Json File");

                Console.WriteLine(combinedData);
                File.WriteAllText(GetNrldataJsonFileName(round), JsonConvert.SerializeObject(combinedData));
            }
        }

        private static void RunTips(IEnumerable<NrlMatches> thisWeeksResults, List<ClubLadder> ladderList,
            List<NrlMatches> lastWeeksResults)
        {
            var engine = new ScoreEngine();

            foreach (var game in thisWeeksResults)
            {
                var tip = engine.GetTip(game, ladderList, lastWeeksResults);
                Console.WriteLine(tip);
            }
        }

        private static IEnumerable<NrlMatches> GetDraw(IWebDriver driver, int round)
        {
            driver.Navigate().GoToUrl($"https://www.nrl.com/draw/?competition=111&season=2018&round={round}");

            var vae = driver.FindElement(By.Id("vue-draw")).GetAttribute("q-data");
            var draws = JsonConvert.DeserializeObject<NrlDrawData>(vae).DrawGroups;
            return draws.Where(x => x.Matches != null).SelectMany(x => x.Matches).ToList();
        }

        private static IEnumerable<ClubLadder> GetLadderList(IWebDriver driver, int round)
        {
            driver.Navigate().GoToUrl($"https://www.nrl.com/ladder/?competition=111&season=2018&round={round}");

            var ladder = driver.FindElement(By.ClassName("ladder")).FindElement(By.TagName("tbody"));

            return (from row in ladder.FindElements(By.ClassName("ladder__row"))
                let club = row.FindElement(By.ClassName("ladder__item--club-header"))
                let tds = row.FindElements(By.ClassName("ladder__item"))
                select new ClubLadder
                {
                    Position = Convert.ToInt32(row.FindElement(By.ClassName("ladder-position")).Text),
                    Name = club.Text,
                    Wins = Convert.ToInt32(GetTdText(tds, 5)),
                    Losses = Convert.ToInt32(GetTdText(tds, 6)),
                    ForPoints = Convert.ToInt32(GetTdText(tds, 9)),
                    AgainstPoints = Convert.ToInt32(GetTdText(tds, 10)),
                    HomeRecord = GetTdText(tds, 12),
                    AwayRecord = GetTdText(tds, 13),
                    Form = GetTdText(tds, 14)
                }).ToList();
        }

        private static string GetTdText(IReadOnlyList<IWebElement> tds, int col)
        {
            return tds[col].Text;
        }
    }
}