using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Band2310.Classes;

namespace Band2310
{
    public partial class mainForm : Form
    {
        ThreadsManager threadsManager;

        //galima ir boolean naudoti
        private enum ThreadButtonState
        {
            Started,
            Stopped
        }

        private string[] ThreadButtonsTexts = new string[]
        {
            "Stop",
            "Start"
        };

        private ThreadButtonState buttonState = ThreadButtonState.Stopped;

        //galima ir paprastus metodus naudoti
        public event EventHandler StartThreads;
        public event EventHandler StopThreads;

        public mainForm()
        {
            StartThreads += StartThreadsWork;
            StopThreads += StopThreadsWork;

            threadsManager = ThreadsManager.Instance;
            threadsManager.ThreadsFinished += ShowResults;
            threadsManager.ThreadCalculated += ShowResults;
            threadsManager.UiControl = this;

            InitializeComponent();
            threadLabel.Text += "( nuo " + ThreadsManager.THREADS_MIN.ToString() + " iki " + ThreadsManager.THREADS_MAX.ToString() + " )";

            changeButtonState(ThreadButtonState.Stopped);
        }

        private void threadMainButton_Click(object sender, EventArgs e)
        {
            if(buttonState == ThreadButtonState.Started) 
            {
                changeButtonState(ThreadButtonState.Stopped);
                StopThreads?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                changeButtonState(ThreadButtonState.Started);
                StartThreads?.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual void StartThreadsWork(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Atsaku skaicius: " + threadNumericUpDown.Value.ToString());
                threadsManager.StartThreadsForRandomLines((int)threadNumericUpDown.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("Klaida metode StartThreadsWork: " + ex.ToString(), "Exception Aptikimas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                changeButtonState(ThreadButtonState.Stopped);
            }
        }

        protected virtual void StopThreadsWork(object sender, EventArgs e)
        {
            try
            {
                threadsManager.StopThreads();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("Klaida metode StopThreadsWork: " + ex.ToString(), "Exception Aptikimas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                changeButtonState(ThreadButtonState.Started);
            }
        }

        public void ShowResults(object sender, EventArgs e)
        {
            ConcurrentQueue<ThreadInformation> results = ThreadsManager.GeneratedThreadInformation;
            threadsListView.Items.Clear();

            foreach (var item in results) 
            {
                threadsListView.Items.Add(new ListViewItem(new[] { item.ThreadID.ToString(), item.GeneratedLine }));
            }
        }

        private void changeButtonState(ThreadButtonState changeStateTo)
        {
            if(changeStateTo == ThreadButtonState.Started)
            {
                buttonState = ThreadButtonState.Started;
                threadMainButton.Text = ThreadButtonsTexts[(int)ThreadButtonState.Started];
            }
            else
            {
                buttonState = ThreadButtonState.Stopped;
                threadMainButton.Text = ThreadButtonsTexts[(int)ThreadButtonState.Stopped];
            }
        }
    }
}
