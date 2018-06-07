using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

namespace footytips
{
    public class MachineLearning
    {
        public PredictionModel<NrlResult, NrlPrediction> TrainData(string csvFilename)
        {
            var pipeline = new LearningPipeline
            {
                new TextLoader<NrlResult>(csvFilename, true, ","),
                new Dictionarizer("Label"),
                new ColumnConcatenator("Features", "PreviousWeeksHomeFor", "PreviousWeeksHomeAgainst",
                    "PreviousWeeksAwayFor", "PreviousWeeksAwayAgainst", "AwayTeamAwayForm", "AwayTeamHomeForm",
                    "HomeTeamAwayForm", "HomeTeamHomeForm", "HomeTeamLastWeekScore", "AwayTeamLastWeekScore"),
                new StochasticDualCoordinateAscentClassifier(),
                new PredictedLabelColumnOriginalValueConverter {PredictedLabelColumn = "PredictedLabel"}
            };


            return pipeline.Train<NrlResult, NrlPrediction>();
        }


        public IEnumerable<NrlPrediction> Predict(List<NrlResult> roundData,
            PredictionModel<NrlResult, NrlPrediction> training)
        {
            var predictions = training.Predict(roundData);
            return predictions;
        }
    }

    public class NrlPrediction
    {
        [ColumnName("PredictedLabel")] public float HomeTeamWin;
    }

    public class NrlResult
    {
        [Column("0")] public string HomeTeam;
        [Column("1")] public string AwayTeam;

        [Column("2")] [ColumnName("Label")] public float Label;

        [Column("3")] public float PreviousWeeksHomeFor;

        [Column("4")] public float PreviousWeeksHomeAgainst;

        [Column("5")] public float PreviousWeeksAwayFor;
        [Column("6")] public float PreviousWeeksAwayAgainst;

        [Column("7")] public float HomeTeamHomeForm;

        [Column("8")] public float HomeTeamAwayForm;

        [Column("9")] public float AwayTeamHomeForm;

        [Column("10")] public float AwayTeamAwayForm;

        [Column("11")] public float HomeTeamLastWeekScore;
        [Column("12")] public float AwayTeamLastWeekScore;
    }

    public class NrlResult2
    {
        public string HomeTeam { get; set; }

        public string AwayTeam { get; set; }

        public float Label { get; set; }

        public float PreviousWeeksHomeFor { get; set; }

        public float PreviousWeeksHomeAgainst { get; set; }

        public float PreviousWeeksAwayFor { get; set; }

        public float PreviousWeeksAwayAgainst { get; set; }

        public float HomeTeamHomeForm { get; set; }

        public float HomeTeamAwayForm { get; set; }

        public float AwayTeamHomeForm { get; set; }

        public float AwayTeamAwayForm { get; set; }

        public int Round { get; set; }
        public float HomeTeamLastWeekScore { get; set; }
        public float AwayTeamLastWeekScore { get; set; }
    }
}