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
using System.Collections;

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

        public enum ThreadsState
        {
            Working,
            Stopped
        }

        public ThreadsState threadsState = ThreadsState.Stopped;

        public delegate void ThreadsFinishedEventHandler(object sender, EventArgs e);
        public event ThreadsFinishedEventHandler ThreadsFinished;
        public event EventHandler ThreadCalculated;

        private static ThreadsManager instance = null;
        private static readonly object lockObject = new object();

        private Control uiControl = null;
        private static Random random = new Random();
        //naudojamas Queue, nes yra FIFO (First In First Out) kolekcija
        private static ConcurrentQueue<ThreadInformation> generatedThreadInformation = new ConcurrentQueue<ThreadInformation>();

        private static object syncLock = new object();

        private List<System.Threading.Thread> threads = new List<System.Threading.Thread>();

        private ThreadsManager()
        {
            threadsMaxLengthInclusive = THREADS_LINE_MAX_LENGTH + 1;
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
            try
            {
                threadsState = ThreadsState.Stopped;

                foreach (var thread in threads)
                {
                    //thread.Abort();
                    thread.Join();
                }

                threads.RemoveAll(thread => !thread.IsAlive);

                Console.WriteLine("Visos gijos pabaige darba. Pradedamas ThreadsFinished eventas...");
                ThreadsFinished?.Invoke(this, EventArgs.Empty);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Klaida StopThreads metode: " + ex.ToString());
            }
        }

        public void ThreadCalculations(int fakeID)
        {
            try
            {
                float sleepTimeInSeconds = GetRandomFloatBetweenValues(THREADS_MIN_TIME, THREADS_MAX_TIME);
                int sleepTimeInMilliseconds = (int)(sleepTimeInSeconds * 1000);

                System.Threading.Thread.Sleep(sleepTimeInMilliseconds);

                //dar karta patikrinam po laukimo ar reikia testi darba
                if (threadsState == ThreadsState.Stopped)
                    return;

                string randomLine = GenerateRandomLine();
                int threadID = fakeID + 1;// System.Threading.Thread.CurrentThread.ManagedThreadId;

                if (SyncThreadsInsert(threadID, randomLine))
                {
                    AddGeneratedString(threadID, randomLine);
                    Console.WriteLine($"Thread ID: {threadID}, Atsitiktine skaiciu eliute: {randomLine}");
                }

                //tikrinam, kad nebutu deadlock
                //UiControl ThreadCalculated eventas yra pradedamas pagrindineje atsakoje
                //ir laukiamas atsakymas, thread.join priverstinai nutrauke darba ir buna ui freeze
                //tam tikrinama obijekto busena
                if (threadsState == ThreadsState.Stopped)
                    return;

                //naudojam async(BeginInvoke), saugiau jei(Invoke)
                UiControl?.BeginInvoke(new Action(() => ThreadCalculated?.Invoke(this, EventArgs.Empty)));

                ThreadCalculations(fakeID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Klaida metode ThreadCalculations: " + e.Message);
            }
        }

        /// <summary>
        /// grazinama bool reiksme, nes db connection nera staigus procesas
        /// ir InsertData metodas tikrina threadstate, bei naudojamas lock
        /// </summary>
        /// <param name="threadID"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SyncThreadsInsert(int threadID, string data)
        {
            lock (syncLock)
            {
                if (threadsState == ThreadsState.Stopped)
                {
                    return false;
                }

                return DatabaseManager.Instance.InsertData(threadID, DateTime.Now, data); ;
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

        public static void DequeueAll<T>(Queue<T> queue)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue), "Queue reiksme negali buti null.");
            }

            while (queue.Count > 0)
            {
                T item = queue.Dequeue();
            }
        }
    }
}
