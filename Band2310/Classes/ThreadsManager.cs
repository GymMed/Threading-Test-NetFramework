using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.IO;

namespace Band2310.Classes
{
    public class ThreadsManager
    {
        private static string symbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public const int THREADS_MIN = 2;
        public const int THREADS_MAX = 15;

        public const int THREADS_LINE_MAX_LENGTH = 10;
        public const int TRHEADS_LINE_MIN_LENGTH = 5;
        private int threadsMaxLengthInclusive = 0;

        public const int MAX_STORED_RANDOM_LINES = 20;

        public const float THREADS_MIN_TIME = 0.5f;
        public const float THREADS_MAX_TIME = 2.0f;

        private enum ThreadsState
        {
            Working,
            Stopped
        }

        private ThreadsState threadsState = ThreadsState.Stopped;

        public delegate void ThreadsFinishedEventHandler(object sender, EventArgs e);
        public event ThreadsFinishedEventHandler ThreadsFinished;
        public event EventHandler ThreadCalculated;

        private static ThreadsManager instance = null;
        private static readonly object lockObject = new object();

        private Control uiControl = null;
        private static Random random = new Random();
        //naudojamas Queue, nes yra FIFO (First In First Out) kolekcija
        private static ConcurrentQueue<ThreadInformation> generatedThreadInformation = new ConcurrentQueue<ThreadInformation>();

        private List<System.Threading.Thread> threads = new List<System.Threading.Thread>();

        private static DatabaseManager dbManager;

        private ThreadsManager()
        {
            threadsMaxLengthInclusive = THREADS_LINE_MAX_LENGTH + 1;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string mdbFileName = "Threads-uvs.mdb";
            string mdbFilePath = Path.Combine(baseDirectory, mdbFileName);

            dbManager = new DatabaseManager(mdbFilePath);
        }

        public static ThreadsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        // dar karta patikrint, nes kai kurios gijos
                        // gali praeiti pirma patikrinima
                        if (instance == null)
                        {
                            instance = new ThreadsManager();
                        }
                    }
                }
                return instance;
            }
        }

        public static ConcurrentQueue<ThreadInformation> GeneratedThreadInformation 
        {
            get => generatedThreadInformation;
            set => generatedThreadInformation = value; 
        }

        public Control UiControl 
        { 
            get => uiControl;
            set => uiControl = value; 
        }

        public void StartThreadsForRandomLines(int threadsCount)
        {
            if (threadsCount < 1)
                throw new Exception("Nustatytas gijų(atšakų) skaičius negali būti 0 arba neigiamas skaičius!");

            if (threadsCount < THREADS_MIN)
                throw new Exception("Nustatyas gijų(atšakų) skaičius negali būti mažesnis nei " + threadsCount.ToString());

            if (threadsCount > THREADS_MAX)
                throw new Exception("Nustatytas gijų(atšakų) skaičius negali būti didesnis jei " + threadsCount.ToString());

            threadsState = ThreadsState.Working;

            for (int i = 0; i < threadsCount; i++)
            {
                //yra galimybe, kad pirmas id bus 2, nes pradetas
                //atsakos darbas bus uzdelstas daugiau atsitiktinai
                int threadIndex = i;

                threads.Add(
                    new System.Threading.Thread(
                        new ThreadStart(
                            () => ThreadCalculations(threadIndex)
                        )
                    )
                );
                threads[i].Start();
            }
        }

        public virtual void StopThreads()
        {
            threadsState = ThreadsState.Stopped;

            foreach (var thread in threads)
            {
                thread.Join();
            }

            threads.RemoveAll(thread => !thread.IsAlive);

            Console.WriteLine("Visos gijos pabaige darbą. Pradedamas ThreadsFinished eventas...");
            ThreadsFinished?.Invoke(this, EventArgs.Empty);
        }

        public void ThreadCalculations(int fakeID)
        {
            try
            {
                float sleepTimeInSeconds = GetRandomFloatBetweenValues(THREADS_MIN_TIME, THREADS_MAX_TIME);
                int sleepTimeInMilliseconds = (int)(sleepTimeInSeconds * 1000);

                System.Threading.Thread.Sleep(sleepTimeInMilliseconds);

                if (threadsState == ThreadsState.Stopped)
                    return;

                string randomLine = GenerateRandomLine();
                int threadID = fakeID + 1;// System.Threading.Thread.CurrentThread.ManagedThreadId;
                dbManager.InsertData(threadID, DateTime.Now, randomLine);
                AddGeneratedString(threadID, randomLine);

                Console.WriteLine($"Thread ID: {threadID}, Atsitiktine skaiciu eliute: {randomLine}");
                UiControl?.Invoke(new Action(() => ThreadCalculated?.Invoke(this, EventArgs.Empty)));

                ThreadCalculations(fakeID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Klaida metode ThreadCalculations: " + e.Message);
            }
        }

        public string GenerateRandomLine()
        {
            int lineLength = random.Next(TRHEADS_LINE_MIN_LENGTH, threadsMaxLengthInclusive);

            char[] lineArray = new char[lineLength];

            for (int i = 0; i < lineLength; i++)
            {
                int randomIndex = random.Next(symbols.Length);
                lineArray[i] = symbols[randomIndex];
            }

            return new string(lineArray);
        }

        public static float GetRandomFloatBetweenValues(float minValue, float maxValue)
        {
            return (float)random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static void AddGeneratedString(int threadID, string generatedString)
        {
            GeneratedThreadInformation.Enqueue(new ThreadInformation(threadID, generatedString));
            if (GeneratedThreadInformation.Count > MAX_STORED_RANDOM_LINES)
            {
                ThreadInformation dequeueItem;

                if (!GeneratedThreadInformation.TryDequeue(out dequeueItem)) 
                {
                    throw new Exception("Nepavyko pašalinti įrašo");
                }
            }
        }
    }
}
