using System;

namespace footytips.Models
{
    public class ClubLadder
    {
        public string Name { get; set; }

        public int ForPoints { get; set; } // 10
        public int AgainstPoints { get; set; } //11
        public string HomeRecord { get; set; } //13
        public string AwayRecord { get; set; } //14
        public string Form { get; set; } //15

        public int GameCount => GetCount();

        public int HomeForm => GetForm(HomeRecord);
        public int AwayForm => GetForm(AwayRecord);
        public int Position { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        private int GetCount()
        {
            var wins = HomeRecord.Substring(0, HomeRecord.IndexOf("-", StringComparison.Ordinal));
            var losses = HomeRecord.Substring(HomeRecord.IndexOf("-", StringComparison.Ordinal) + 1);

            var winInt = Convert.ToInt32(wins);
            var lossesint = Convert.ToInt32(losses);


            return winInt + lossesint;
        }

        private static int GetForm(string record)
        {
            var wins = record.Substring(0, record.IndexOf("-", StringComparison.Ordinal));
            var losses = record.Substring(record.IndexOf("-", StringComparison.Ordinal) + 1);

            var winInt = Convert.ToInt32(wins);
            var lossesint = Convert.ToInt32(losses);
            return winInt - lossesint;
        }

        public override string ToString()
        {
            return
                $"N: {Name}, FP : {ForPoints}, AP: {AgainstPoints}, HR: {HomeRecord}, AW: {AwayRecord}, Form: {Form}";
        }
    }
}