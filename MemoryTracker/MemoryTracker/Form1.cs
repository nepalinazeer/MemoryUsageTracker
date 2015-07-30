//A simple multi-threaded program that keeps track of current memory usage and the peak usage
//Author: Najir Pandey
//website: www.nepalinazeer.com


using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace MemoryTracker
{
    public partial class Form1 : Form
    {

        private BackgroundWorker m_AsyncWorker = new BackgroundWorker();

        private string _memUsage = "0.0";

        public Form1()
        {
            InitializeComponent();

            _memUsage = performanceMonitor().ToString(CultureInfo.InvariantCulture);

            textBox1.Text = _memUsage;
            textBoxPeak.Text = _memUsage;
            textBoxInitial.Text = _memUsage;

            m_AsyncWorker.WorkerReportsProgress = true;
            m_AsyncWorker.WorkerSupportsCancellation = true;
            m_AsyncWorker.ProgressChanged += backgroundWorker1_ProgressChanged;
            m_AsyncWorker.RunWorkerCompleted +=backgroundWorker1_RunWorkerCompleted;
            m_AsyncWorker.DoWork +=backgroundWorker1_DoWork;

            //execute the tracker during initialization
            ExecuteTracker();

        }
   
        private void ExecuteTracker()
        {
            if (m_AsyncWorker.IsBusy)
            {
                button1.Enabled = false;
                label3.Text = "Stopping...";

                m_AsyncWorker.CancelAsync();
            }
            else
            {
                button1.Text = "Stop";
                label3.Text = "Running...";

                m_AsyncWorker.RunWorkerAsync();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            ExecuteTracker();
            
                        
        }

        private double performanceMonitor()
        {

        
                Int64 phav = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
                Int64 tot = PerformanceInfo.GetTotalMemoryInMiB();
                decimal percentFree = ((decimal)phav / (decimal)tot) * 100;
                decimal percentOccupied = 100 - percentFree;


                return ((double)(tot - phav) / 1024);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            BackgroundWorker backgroundWorker1 = sender as BackgroundWorker;
                      
            double currentUsage = performanceMonitor();

     
            while(true)
            {
                Thread.Sleep(100);
                currentUsage = performanceMonitor();
                e.Result = currentUsage.ToString();
                _memUsage = currentUsage.ToString();

               backgroundWorker1.ReportProgress(1);

                if (backgroundWorker1.CancellationPending)
                {
                    Thread.Sleep(1000);

                    e.Cancel = true;

                    return;

                }
            }

        
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Text = "Start";
            button1.Enabled = true;

          
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }

            label3.Text = e.Cancelled ? "Stopped..." : "Completed...";
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            textBox1.Text = _memUsage;
        
            if (Convert.ToDouble(textBoxPeak.Text) < Convert.ToDouble(_memUsage))
            {
                textBoxPeak.Text = _memUsage;
            }
        }

    }

}




