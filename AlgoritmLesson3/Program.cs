using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Diagnosers;

namespace AlgoritmLesson3
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            //BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
        }
    }

    //[MemoryDiagnoser] //Оставил для будущих экспериментов.
    [Config(typeof(MyConfig))]

    public class BenchmarkClass
    {
        #region ---- FIELDS ----

        /// <summary>Минимальное возможное значение координат</summary>
        private const int MIN = -10000;
        /// <summary>Максимальное возможное значение координат</summary>
        private const int MAX = 10000;
        /// <summary>Количество элементов в массиве со случайными координатами</summary>
        private const int ELEMENTS = 100000;
        /// <summary>Указатель для перебора кординат в массиве</summary>
        private int pointer = 0;

        /// <summary>Массив содержащий случайные float значения координат</summary>
        private int[] randomIntCoordinates = new int[ELEMENTS];
        /// <summary>Массив содержащий случайные float значения координат</summary>
        private long[] randomLongCoordinates = new long[ELEMENTS];
        /// <summary>Массив содержащий случайные float значения координат</summary>
        private float[] randomFloatCoordinates = new float[ELEMENTS];
        /// <summary>Массив содержащий случайные double значения координат</summary>
        private double[] randomDoubleCoordinates = new double[ELEMENTS];

        #endregion


        #region ---- CONFIG ----

        /// <summary>
        /// Конфиг бенчмарка
        /// </summary>
        private class MyConfig : ManualConfig
        {
            public MyConfig()
            {

                //AddJob(Job.Dry);
                AddJob(Job.ShortRun);
                AddLogger(ConsoleLogger.Default);
                AddColumn(
                    TargetMethodColumn.Method,
                    StatisticColumn.Mean,
                    StatisticColumn.StdErr,
                    StatisticColumn.StdDev,
                    StatisticColumn.Max,
                    StatisticColumn.Median,
                    BaselineRatioColumn.RatioMean);
                AddExporter(
                    AsciiDocExporter.Default,
                    MarkdownExporter.GitHub,
                    HtmlExporter.Default);
                AddAnalyser(EnvironmentAnalyser.Default);
                AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(maxDepth: 4, exportDiff: false)));
                UnionRule = ConfigUnionRule.AlwaysUseLocal;

            }
        }

        #endregion

        #region ---- CONSTRUCTORS ----

        /// <summary>
        /// Конструктор заполняющий массив координат случайной информацией
        /// </summary>
        public BenchmarkClass()
        {
            ///Заполняем массив случайными числами
            Random rnd = new Random();
            for (int i = 0; i < ELEMENTS; i++)
            {
                randomIntCoordinates[i] = rnd.Next(MIN, MAX);
                randomLongCoordinates[i] = rnd.Next(MIN, MAX) * 1000000;
                randomFloatCoordinates[i] = (float)(rnd.NextDouble() * (MAX - MIN) - MAX);
                randomDoubleCoordinates[i] = (rnd.NextDouble() * (MAX - MIN) - MAX);
            }
            pointer = rnd.Next(ELEMENTS); //Рандомизируем указатель в массиве.
        }

        #endregion

        #region ---- ADDITIONAL METHODS ----

        /// <summary>
        /// Выдает два числа из массива float координат и сдвигает его указатель на следующую позицию
        /// </summary>
        /// <returns>Кортеж с двумя float координатами</returns>
        public (float, float) giveMeFloatCoordinates()
        {
            if (pointer > ELEMENTS - 4)
                pointer = 0;
            else
                pointer += 2;
            return (randomFloatCoordinates[pointer], randomFloatCoordinates[pointer + 1]);
        }

        /// <summary>
        /// Выдает два числа из массива float координат и сдвигает его указатель на следующую позицию
        /// </summary>
        /// <returns>Кортеж с двумя float координатами</returns>
        public (double, double) giveMeDoubleCoordinates()
        {
            if (pointer > ELEMENTS - 4)
                pointer = 0;
            else
                pointer += 2;
            return (randomDoubleCoordinates[pointer], randomDoubleCoordinates[pointer + 1]);
        }

        #endregion

        #region ---- DATA TYPES ----

        /// <summary>Точка с координатами виде класса со сзначениями X, Y во float</summary>
        public class PointClassFloat
        {
            public float X { get; set; }
            public float Y { get; set; }

            public PointClassFloat((float x, float y) coord)
            {
                X = coord.x;
                Y = coord.y;
            }
        }

        /// <summary>Точка с координатами виде класса со сзначениями X, Y во double</summary>
        public class PointClassDouble
        {
            public double X { get; set; }
            public double Y { get; set; }

            public PointClassDouble((double x, double y) coord)
            {
                X = coord.x;
                Y = coord.y;
            }
        }

        /// <summary>Точка с координатами виде структуры со сзначениями X, Y во float</summary>
        public struct PointStructFloat
        {
            public float X;
            public float Y;

            public PointStructFloat((float x, float y) coord)
            {
                X = coord.x;
                Y = coord.y;
            }
        }

        /// <summary>Точка с координатами виде структуры со сзначениями X, Y в double</summary>
        public struct PointStructDouble
        {
            public double X;
            public double Y;
            public PointStructDouble((double x, double y) coord)
            {
                X = coord.x;
                Y = coord.y;
            }

        }

        #endregion

        #region ---- TESTED METHODS ----
        /// <summary>Рассчет расстояния между двумя точками реализованными классов во float</summary>
        /// <param name="pointOne">Точка 1</param>
        /// <param name="pointTwo">Точка 2</param>
        /// <returns>Расстояние между точками</returns>
        public float PointDistanceClassFloat(PointClassFloat pointOne, PointClassFloat pointTwo)
        {
            float x = pointOne.X - pointTwo.X;
            float y = pointOne.Y - pointTwo.Y;
            return MathF.Sqrt((x * x) + (y * y));
        }

        /// <summary>Рассчет расстояния между двумя точками реализованными классов в double</summary>
        /// <param name="pointOne">Точка 1</param>
        /// <param name="pointTwo">Точка 2</param>
        /// <returns>Расстояние между точками</returns>
        public double PointDistanceClassDouble(PointClassDouble pointOne, PointClassDouble pointTwo)
        {
            double x = pointOne.X - pointTwo.X;
            double y = pointOne.Y - pointTwo.Y;
            return Math.Sqrt((x * x) + (y * y));
        }


        /// <summary>Рассчет расстояния между двумя точками реализованными структурами во float</summary>
        /// <param name="pointOne">Точка 1</param>
        /// <param name="pointTwo">Точка 2</param>
        /// <returns>Расстояние между точками</returns>
        public float PointDistanceStructFloat(PointStructFloat pointOne, PointStructFloat pointTwo)
        {
            float x = pointOne.X - pointTwo.X;
            float y = pointOne.Y - pointTwo.Y;
            return MathF.Sqrt((x * x) + (y * y));
        }

        /// <summary>Рассчет расстояния между двумя точками реализованными структурами в double</summary>
        /// <param name="pointOne">Точка 1</param>
        /// <param name="pointTwo">Точка 2</param>
        /// <returns>Расстояние между точками</returns>
        public double PointDistanceStructDouble(PointStructDouble pointOne, PointStructDouble pointTwo)
        {
            double x = pointOne.X - pointTwo.X;
            double y = pointOne.Y - pointTwo.Y;
            return Math.Sqrt((x * x) + (y * y));
        }

        /// <summary>Рассчет квадрата расстояния между двумя точками реализованными структурами во float</summary>
        /// <param name="pointOne">Точка 1</param>
        /// <param name="pointTwo">Точка 2</param>
        /// <returns>Квадрат расстояния между точками</returns>
        public float PointDistanceShortStructFloat(PointStructFloat pointOne, PointStructFloat pointTwo)
        {
            float x = pointOne.X - pointTwo.X;
            float y = pointOne.Y - pointTwo.Y;
            return ((x * x) + (y * y));
        }

        /// <summary>Рассчет квадрата расстояния между двумя точками реализованными структурами во double</summary>
        /// <param name="pointOne">Точка 1</param>
        /// <param name="pointTwo">Точка 2</param>
        /// <returns>Квадрат расстояния между точками</returns>
        public double PointDistanceShortStructDouble(PointStructDouble pointOne, PointStructDouble pointTwo)
        {
            double x = pointOne.X - pointTwo.X;
            double y = pointOne.Y - pointTwo.Y;
            return ((x * x) + (y * y));
        }

        #endregion

        #region ---- BENCHMARKS ----
        [Benchmark(Description = "Расстояние через классы float")]
        public void TestPointDistanceClassFloat()
        {
            PointClassFloat pointOne = new PointClassFloat(giveMeFloatCoordinates());
            PointClassFloat pointTwo = new PointClassFloat(giveMeFloatCoordinates());
            PointDistanceClassFloat(pointOne, pointTwo);
        }

        [Benchmark(Description = "Расстояние через классы double")]
        public void TestPointDistanceClassDouble()
        {
            PointClassDouble pointOne = new PointClassDouble(giveMeDoubleCoordinates());
            PointClassDouble pointTwo = new PointClassDouble(giveMeDoubleCoordinates());
            PointDistanceClassDouble(pointOne, pointTwo);
        }

        [Benchmark(Description = "Расстояние через структуры float")]
        public void TestPointDistanceStructFloat()
        {
            PointStructFloat pointOne = new PointStructFloat(giveMeFloatCoordinates());
            PointStructFloat pointTwo = new PointStructFloat(giveMeFloatCoordinates());
            PointDistanceStructFloat(pointOne, pointTwo);
        }

        [Benchmark(Description = "Расстояние через структуры double")]
        public void TestPointDistanceStructDouble()
        {
            PointStructDouble pointOne = new PointStructDouble(giveMeDoubleCoordinates());
            PointStructDouble pointTwo = new PointStructDouble(giveMeDoubleCoordinates());
            PointDistanceStructDouble(pointOne, pointTwo);
        }


        [Benchmark(Description = "Квадрат расстояния через структуры float")]
        public void TestPointDistanceShortStructFloat()
        {
            PointStructFloat pointOne = new PointStructFloat(giveMeFloatCoordinates());
            PointStructFloat pointTwo = new PointStructFloat(giveMeFloatCoordinates());
            PointDistanceShortStructFloat(pointOne, pointTwo);
        }

        [Benchmark(Description = "Квадрат расстояния через структуры double")]
        public void TestPointDistanceShortStructDouble()
        {
            PointStructDouble pointOne = new PointStructDouble(giveMeDoubleCoordinates());
            PointStructDouble pointTwo = new PointStructDouble(giveMeDoubleCoordinates());
            PointDistanceShortStructDouble(pointOne, pointTwo);
        }

        [Benchmark(Description = "Расстояние через структуры double (с приведением из float)")]
        public void TestPointDistanceStructDoubleFromFloat()
        {
            PointStructDouble pointOne = new PointStructDouble(giveMeFloatCoordinates());
            PointStructDouble pointTwo = new PointStructDouble(giveMeFloatCoordinates());
            PointDistanceStructDouble(pointOne, pointTwo);
        }

        #endregion

    }
}