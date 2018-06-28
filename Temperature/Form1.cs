using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Collections;
using OpenHardwareMonitor.Hardware;

namespace Temperature
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            bw.DoWork += new DoWorkEventHandler(bwDoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwCompleted);
            bw.WorkerSupportsCancellation = true;

        }
        // POSITION WINDOW IN RLQ
        protected override void OnLoad(EventArgs e)
        {
            var screen = Screen.FromPoint(this.Location);
            this.Location = new Point(screen.WorkingArea.Right - this.Width, screen.WorkingArea.Bottom - this.Height);
            base.OnLoad(e);
        }
        // REPOSITION WINDOW AS NEEDED
        private void ResetWindow()
        {
            var screen = Screen.FromPoint(this.Location);
            this.Location = new Point(screen.WorkingArea.Right - this.Width, screen.WorkingArea.Bottom - this.Height);
        }
        // LOCAL VARIABLES AND CLASSES
        private static string CurrentTemp = String.Empty;
        private static bool TempUpdated = false;
        private string HighestTempAsString = String.Empty;
        private OHData od = new OHData();
        // THREADING
        private BackgroundWorker bw = new BackgroundWorker();
        private void bwDoWork(object sender, DoWorkEventArgs e)
        {
            
            BackgroundWorker worker = sender as BackgroundWorker;
            TempUpdated = false;
            CurrentTemp = CPUTempFromOH();
 
        }
        private void bwCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                TempUpdated = true;
            }
            else
            {
                TempUpdated = false;
            }
        }

        // FORM LOAD
        private void MainForm_Load(object sender, EventArgs e)
        {
            
            timer1.Interval = 60000; // Update rate 1/min
            timer1.Start();
            updateToolStripMenuItem_Click(sender, e);
            
        }
        // FORM CLICKED
        private void MainForm_Click(object sender, EventArgs e)
        {
            Point p = new Point(this.Left, this.Bottom- (this.Height));
            this.contextMenuStrip1.Show(p);
        }
        // CONVERT TEMP STRING TO AN INTEGER VALUE
        private int GetNumericTemp(string source)
        {
            int x;
            char ch;
            int result = 0;
            string output = String.Empty;
            for (x=0;x<source.Length;x++)
            {
                ch = source[x];
                if (ch != '.')
                {
                    output += ch;
                    continue;
                }
                break;
            }
            try
            {
               result = Convert.ToInt16(output);
               return result;
            }
            catch
            {
                result = 0;
                return result;
            }
           
            

        }
        // TIMER TICK
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync();
            }
            string s;
            if (TempUpdated)
            {
                s = CurrentTemp;
            }
            else
            {
                return;
            }
            UpdateTempLabel(s);
        }
        // UPDATE TEMPERATURE LABEL
        private void UpdateTempLabel(string temp)
        {
            int newtemp = GetNumericTemp(temp);
            int oldtemp = GetNumericTemp(HighestTempAsString);
            if (newtemp >= oldtemp)
            {
                HighestTempAsString = temp;
            }

            // Color Code Result
            if (newtemp < 150) // max temp Intel i7 950 is 67deg C
            {
                label1.ForeColor = Color.Lime;
            }
            else
            {
                label1.ForeColor = Color.Red;
            }
            label1.Text = temp;

            this.Text = label1.Text;
            this.maxTempToolStripMenuItem.Text = "Max Temp = " + HighestTempAsString;
        }
        // EXTRACT CPU TEMP FROM OHData INSTANCE
        // REV 10-30-2017 Averages all available CPU Temperatures, both Core and Package
        private string CPUTempFromOH()
        {
            
            List<string> templist = new List<string>();
            string result = String.Empty;
            od.Update();
            int count = od.DataList.Count();
            int x;
            float temp = 0;
            // Build List of Temps
            for (x=0;x<count;x++)
            {
                if ((od.DataList[x].type == "Temperature: ") && (od.DataList[x].name.Contains("CPU")))
                {
                    result = od.DataList[x].reading.Split(' ')[0];
                    templist.Add(result);
                }
            }
            // Convert them back into floats and average
            foreach (string s in templist)
            {
                temp += (float)Convert.ToDouble(s);
            }
            temp = temp / templist.Count();
            //Convert numeric average back into string
            result = temp.ToString("F1") + " F";
            return result;
            
        }
        // UNUSED
        private string CPUfanRPM()
        {
            string result = String.Empty;
            
            return result;

        }
        // MENU QUIT
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // INFO MENU ITEM
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //timer1.Stop();
            // DEBUGGING 04062015
       
            Form2 f2 = new Form2((this.timer1.Interval/1000).ToString()+" sec");
            f2.Show();
            //timer1.Start();

        }
        // UPDATE MENU ITEM
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s;
            if (bw.IsBusy)
            {
                bw.CancelAsync();
            }
            label1.Text = ""; // BLANK TO SHOW ITS UPDATING
            s = CPUTempFromOH();
            UpdateTempLabel(s);

        }
        // 60 SECOND MENU ITEM
        private void secToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 60000;
            secToolStripMenuItem.Checked = true;
            secToolStripMenuItem1.Checked = false;
            secToolStripMenuItem2.Checked = false;
            
        }
        // 30 SECOND TIMER INTERVAL
        private void secToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 30000;
            secToolStripMenuItem1.Checked = true;
            secToolStripMenuItem.Checked = false;
            secToolStripMenuItem2.Checked = false;
        }
        // 1 SECOND TIMER INTERVAL
        private void secToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 1000;
            secToolStripMenuItem2.Checked = true;
            secToolStripMenuItem1.Checked = false;
            secToolStripMenuItem.Checked = false;
            
        }
        // for future developement BIOS ID
        // UNUSED - For debugging
        private List<string> BiosInfo()
        {
            
            
            List<string> results = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"\\.\root\cimv2","SELECT * FROM Win32_BIOS");
            ManagementObjectCollection collection = searcher.Get();
            foreach (ManagementObject o in collection )
            {
                results.Add(Convert.ToString(o.GetPropertyValue("Name")));
                results.Add(Convert.ToString(o.GetPropertyValue("ReleaseDate")));
                results.Add(Convert.ToString(o.GetPropertyValue("Caption")));
                results.Add(Convert.ToString(o.GetPropertyValue("Description")));
                results.Add(Convert.ToString(o.GetPropertyValue("Manufacturer")));
                results.Add(Convert.ToString(o.GetPropertyValue("SMBIOSBIOSVersion")));

            }
            searcher.Dispose();
            return results;

        }
        // RESET WINDOW MENU ITEM
        private void resetWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetWindow();
            return;
        }
    }
}
