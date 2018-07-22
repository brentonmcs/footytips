using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Models;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

namespace footytips
{
    public class MachineLearning
    {
        public PredictionModel<NrlResult, ClusterPrediction> TrainData(IEnumerable<NrlResult> nrlResults)
        {
            var pipeline = new LearningPipeline
            {
                CollectionDataSource.Create(nrlResults),
                new Dictionarizer("Label"),
                new ColumnConcatenator("Features", "PreviousWeeksHomeFor", "PreviousWeeksHomeAgainst",
                    "PreviousWeeksAwayFor", "PreviousWeeksAwayAgainst", "AwayTeamAwayForm", "AwayTeamHomeForm",
                    "HomeTeamAwayForm", "HomeTeamHomeForm", "HomeTeamLastWeekScore", "AwayTeamLastWeekScore"),
                new StochasticDualCoordinateAscentClassifier(),
                //new KMeansPlusPlusClusterer { K = 3 },
                //new FastTreeBinaryClassifier {NumLeaves = 5, NumTrees = 5, MinDocumentsInLeafs = 2},
                
                //new PredictedLabelColumnOriginalValueConverter {PredictedLabelColumn = "PredictedLabel"}
            };


            return pipeline.Train<NrlResult, ClusterPrediction>();
        }


        public IEnumerable<ClusterPrediction> Predict(IEnumerable<NrlResult> roundData,
            PredictionModel<NrlResult, ClusterPrediction> training)
        {
            var predictions = training.Predict(roundData);
            return predictions;
        }

        public void Evaluate(PredictionModel<NrlResult, ClusterPrediction> model,
            IEnumerable<NrlResult> nrlResults)
        {
            var testData = CollectionDataSource.Create(nrlResults);

            var evaluator = new BinaryClassificationEvaluator();

            Console.WriteLine("=============== Evaluating model ===============");

            var metrics = evaluator.Evaluate(model, testData);

            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.Auc:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End evaluating ===============");
            Console.WriteLine();
        }

        public void Evaluate2(PredictionModel<NrlResult, ClusterPrediction> model,
               IEnumerable<NrlResult> nrlResults)
        {
            var testData = CollectionDataSource.Create(nrlResults);
            
            // ClassificationEvaluator performs evaluation for Multiclass Classification type of ML problems.
            var evaluator = new ClassificationEvaluator {OutputTopKAcc = 3};
            
            Console.WriteLine("=============== Evaluating model ===============");

            var metrics = evaluator.Evaluate(model, testData);
            Console.WriteLine("Metrics:");
            Console.WriteLine($"    AccuracyMacro = {metrics.AccuracyMacro:0.####}, a value between 0 and 1, the closer to 1, the better");
            Console.WriteLine($"    AccuracyMicro = {metrics.AccuracyMicro:0.####}, a value between 0 and 1, the closer to 1, the better");
            Console.WriteLine($"    LogLoss = {metrics.LogLoss:0.####}, the closer to 0, the better");
            Console.WriteLine($"    LogLoss for class 1 = {metrics.PerClassLogLoss[0]:0.####}, the closer to 0, the better");
            Console.WriteLine($"    LogLoss for class 2 = {metrics.PerClassLogLoss[1]:0.####}, the closer to 0, the better");
            //    Console.WriteLine($"    LogLoss for class 3 = {metrics.PerClassLogLoss[2]:0.####}, the closer to 0, the better");
            Console.WriteLine();
            Console.WriteLine($"    ConfusionMatrix:");

            // Print confusion matrix
            for (var i = 0; i < metrics.ConfusionMatrix.Order; i++)
            {
                for (var j = 0; j < metrics.ConfusionMatrix.ClassNames.Count; j++)
                {
                    Console.Write("\t" + metrics.ConfusionMatrix[i, j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine("=============== End evaluating ===============");
            Console.WriteLine();
        }
    }


    public class ClusterPrediction
    {
        //        [ColumnName("PredictedLabel")]
        //        public uint SelectedClusterId;
        //
        //        [ColumnName("Score")]
        //        public float[] Distance;
        //[ColumnName("PredictedLabel")] public bool HomeTeamWin;
        
        [ColumnName("Score")]
        public float[] Score;

        public string Winner => Score[0] > Score[1] ? "Home" : "Away";
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
        public int Round { get; set; }
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