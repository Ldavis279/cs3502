using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace CpuScheduler
{
    /// <summary>
    /// Main form for demonstrating CPU scheduling algorithms.
    /// </summary>
    public partial class CpuSchedulerForm : Form
    {
        private DataTable processTable;
        private Random random = new Random();
        private bool isDarkMode = true; // Default to dark mode

        private const int MIN_PROCESS_COUNT = 1;
        private const int MAX_PROCESS_COUNT = 100;
        private const int DEFAULT_PROCESS_COUNT = 3;

        public CpuSchedulerForm()
        {
            InitializeComponent();
            InitializeProcessTable();
        }

        private void WelcomeButton_Click(object sender, EventArgs e)
        {
            ShowPanel(welcomePanel);
            sidePanel.Height = btnWelcome.Height;
            sidePanel.Top = btnWelcome.Top;
        }

        private void DashBoardButton_Click(object sender, EventArgs e)
        {
            ShowPanel(resultsPanel);
            sidePanel.Height = btnDashBoard.Height;
            sidePanel.Top = btnDashBoard.Top;
        }

        private void CpuSchedulerButton_Click(object sender, EventArgs e)
        {
            ShowPanel(schedulerPanel);
            sidePanel.Height = btnCpuScheduler.Height;
            sidePanel.Top = btnCpuScheduler.Top;
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            ShowPanel(aboutPanel);
            sidePanel.Height = btnAbout.Height;
            sidePanel.Top = btnAbout.Top;
        }

        private void DarkModeToggle_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;
            ApplyTheme();
        }

        private void ShowPanel(Panel panelToShow)
        {
            welcomePanel.Visible = false;
            schedulerPanel.Visible = false;
            resultsPanel.Visible = false;
            aboutPanel.Visible = false;
            panelToShow.Visible = true;
            panelToShow.BringToFront();
        }

        private void InitializeWelcomeContent()
        {
            welcomeTextBox.Text = @"Welcome to CPU Scheduler Simulator

This educational tool helps students learn and experiment with CPU scheduling algorithms used in operating systems.

GETTING STARTED

Navigate using the sidebar buttons on the left:

WELCOME
This introduction page explaining the simulator and navigation.

SCHEDULER
The main interface where you can:
• Enter the number of processes to simulate
• Choose from 6 scheduling algorithms:
  - FCFS (First Come, First Serve)
  - SJF (Shortest Job First)
  - Priority Scheduling
  - Round Robin
  - SRTF (Shortest Remaining Time First)
  - HRRN (Highest Response Ratio Next)
• Run simulations and see immediate feedback

RESULTS
View detailed results from your last algorithm execution:
• Process execution details
• Algorithm-specific information
• Summary statistics

ABOUT
Learn about the algorithms:
• How each algorithm works
• When to use each algorithm
• Algorithm characteristics and trade-offs

HOW TO USE
1. Click Scheduler to start
2. Enter number of processes
3. Click an algorithm button
4. View results in the Results section
5. Compare different algorithms";
        }

        private void InitializeAboutContent()
        {
            aboutTextBox.Text = @"CPU Scheduling Algorithms

This simulator demonstrates six CPU scheduling algorithms used in operating systems:

FIRST COME, FIRST SERVE (FCFS)
• Non-preemptive algorithm
• Processes are executed in the order they arrive
• Simple to implement

SHORTEST JOB FIRST (SJF)
• Non-preemptive algorithm
• Selects process with shortest burst time first
• Good for minimizing average waiting time

PRIORITY SCHEDULING
• Processes have a priority number
• CPU goes to the highest priority process
• Can cause starvation of low-priority processes

ROUND ROBIN (RR)
• Preemptive algorithm using time quantum
• Each process gets equal CPU time slices
• Good for time-sharing systems

SHORTEST REMAINING TIME FIRST (SRTF)
• Preemptive version of SJF
• CPU always picks the process with the smallest remaining time
• Can interrupt current process if a shorter one arrives

HIGHEST RESPONSE RATIO NEXT (HRRN)
• Non-preemptive algorithm
• Chooses process with highest response ratio
• Formula: (Waiting Time + Burst Time) / Burst Time
• Helps prevent starvation

Learning Objectives:
• Understand how algorithms schedule processes
• Compare fairness and efficiency
• Learn scheduling trade-offs";
        }

        public List<ProcessData> GetProcessDataFromGrid()
        {
            var processList = new List<ProcessData>();

            foreach (DataRow row in processTable.Rows)
            {
                processList.Add(new ProcessData
                {
                    ProcessID = row["Process ID"].ToString(),
                    BurstTime = Convert.ToInt32(row["Burst Time"]),
                    Priority = Convert.ToInt32(row["Priority"]),
                    ArrivalTime = Convert.ToInt32(row["Arrival Time"])
                });
            }

            return processList;
        }

        public class ProcessData
        {
            public string ProcessID { get; set; }
            public int BurstTime { get; set; }
            public int Priority { get; set; }
            public int ArrivalTime { get; set; }
        }

        private bool IsValidProcessCount(string input, out int processCount)
        {
            if (int.TryParse(input, out processCount))
            {
                return processCount >= MIN_PROCESS_COUNT && processCount <= MAX_PROCESS_COUNT;
            }

            processCount = 0;
            return false;
        }

        private List<SchedulingResult> RunFCFSAlgorithm(List<ProcessData> processes)
        {
            var results = new List<SchedulingResult>();
            int currentTime = 0;

            var sortedProcesses = processes.OrderBy(p => p.ArrivalTime).ToList();

            foreach (var process in sortedProcesses)
            {
                int startTime = Math.Max(currentTime, process.ArrivalTime);
                int finishTime = startTime + process.BurstTime;
                int waitingTime = startTime - process.ArrivalTime;
                int turnaroundTime = finishTime - process.ArrivalTime;

                results.Add(new SchedulingResult
                {
                    ProcessID = process.ProcessID,
                    ArrivalTime = process.ArrivalTime,
                    BurstTime = process.BurstTime,
                    StartTime = startTime,
                    FinishTime = finishTime,
                    WaitingTime = waitingTime,
                    TurnaroundTime = turnaroundTime
                });

                currentTime = finishTime;
            }

            return results;
        }

        private List<SchedulingResult> RunSJFAlgorithm(List<ProcessData> processes)
        {
            var results = new List<SchedulingResult>();
            int currentTime = 0;
            var remainingProcesses = processes.ToList();

            while (remainingProcesses.Count > 0)
            {
                var availableProcesses = remainingProcesses
                    .Where(p => p.ArrivalTime <= currentTime)
                    .ToList();

                if (availableProcesses.Count == 0)
                {
                    currentTime = remainingProcesses.Min(p => p.ArrivalTime);
                    continue;
                }

                var nextProcess = availableProcesses
                    .OrderBy(p => p.BurstTime)
                    .ThenBy(p => p.ArrivalTime)
                    .First();

                int startTime = Math.Max(currentTime, nextProcess.ArrivalTime);
                int finishTime = startTime + nextProcess.BurstTime;
                int waitingTime = startTime - nextProcess.ArrivalTime;
                int turnaroundTime = finishTime - nextProcess.ArrivalTime;

                results.Add(new SchedulingResult
                {
                    ProcessID = nextProcess.ProcessID,
                    ArrivalTime = nextProcess.ArrivalTime,
                    BurstTime = nextProcess.BurstTime,
                    StartTime = startTime,
                    FinishTime = finishTime,
                    WaitingTime = waitingTime,
                    TurnaroundTime = turnaroundTime
                });

                currentTime = finishTime;
                remainingProcesses.Remove(nextProcess);
            }

            return results.OrderBy(r => r.StartTime).ToList();
        }

        private List<SchedulingResult> RunPriorityAlgorithm(List<ProcessData> processes)
        {
            var results = new List<SchedulingResult>();
            int currentTime = 0;
            var remainingProcesses = processes.ToList();

            while (remainingProcesses.Count > 0)
            {
                var availableProcesses = remainingProcesses
                    .Where(p => p.ArrivalTime <= currentTime)
                    .ToList();

                if (availableProcesses.Count == 0)
                {
                    currentTime = remainingProcesses.Min(p => p.ArrivalTime);
                    continue;
                }

                var nextProcess = availableProcesses
                    .OrderByDescending(p => p.Priority)
                    .ThenBy(p => p.ArrivalTime)
                    .First();

                int startTime = Math.Max(currentTime, nextProcess.ArrivalTime);
                int finishTime = startTime + nextProcess.BurstTime;
                int waitingTime = startTime - nextProcess.ArrivalTime;
                int turnaroundTime = finishTime - nextProcess.ArrivalTime;

                results.Add(new SchedulingResult
                {
                    ProcessID = nextProcess.ProcessID,
                    ArrivalTime = nextProcess.ArrivalTime,
                    BurstTime = nextProcess.BurstTime,
                    StartTime = startTime,
                    FinishTime = finishTime,
                    WaitingTime = waitingTime,
                    TurnaroundTime = turnaroundTime
                });

                currentTime = finishTime;
                remainingProcesses.Remove(nextProcess);
            }

            return results.OrderBy(r => r.StartTime).ToList();
        }

        private List<SchedulingResult> RunRoundRobinAlgorithm(List<ProcessData> processes, int quantumTime = 4)
        {
            var processQueue = new Queue<ProcessData>();
            var processResults = new Dictionary<string, SchedulingResult>();
            var remainingBurstTimes = new Dictionary<string, int>();
            int currentTime = 0;

            foreach (var process in processes)
            {
                remainingBurstTimes[process.ProcessID] = process.BurstTime;

                processResults[process.ProcessID] = new SchedulingResult
                {
                    ProcessID = process.ProcessID,
                    ArrivalTime = process.ArrivalTime,
                    BurstTime = process.BurstTime,
                    StartTime = -1,
                    FinishTime = 0,
                    WaitingTime = 0,
                    TurnaroundTime = 0
                };
            }

            foreach (var process in processes.Where(p => p.ArrivalTime <= currentTime).OrderBy(p => p.ArrivalTime))
            {
                processQueue.Enqueue(process);
            }

            var processesNotInQueue = processes.Where(p => p.ArrivalTime > currentTime)
                                              .OrderBy(p => p.ArrivalTime)
                                              .ToList();

            while (processQueue.Count > 0 || processesNotInQueue.Count > 0)
            {
                while (processesNotInQueue.Count > 0 && processesNotInQueue[0].ArrivalTime <= currentTime)
                {
                    processQueue.Enqueue(processesNotInQueue[0]);
                    processesNotInQueue.RemoveAt(0);
                }

                if (processQueue.Count == 0)
                {
                    currentTime = processesNotInQueue[0].ArrivalTime;
                    continue;
                }

                var currentProcess = processQueue.Dequeue();
                var result = processResults[currentProcess.ProcessID];

                if (result.StartTime == -1)
                {
                    result.StartTime = currentTime;
                }

                int executionTime = Math.Min(quantumTime, remainingBurstTimes[currentProcess.ProcessID]);
                currentTime += executionTime;
                remainingBurstTimes[currentProcess.ProcessID] -= executionTime;

                while (processesNotInQueue.Count > 0 && processesNotInQueue[0].ArrivalTime <= currentTime)
                {
                    processQueue.Enqueue(processesNotInQueue[0]);
                    processesNotInQueue.RemoveAt(0);
                }

                if (remainingBurstTimes[currentProcess.ProcessID] == 0)
                {
                    result.FinishTime = currentTime;
                    result.TurnaroundTime = result.FinishTime - result.ArrivalTime;
                    result.WaitingTime = result.TurnaroundTime - result.BurstTime;
                }
                else
                {
                    processQueue.Enqueue(currentProcess);
                }
            }

            return processResults.Values.OrderBy(r => r.StartTime).ToList();
        }

        private List<SchedulingResult> RunSRTFAlgorithm(List<ProcessData> processes)
        {
            var processResults = new Dictionary<string, SchedulingResult>();
            var remainingBurstTimes = new Dictionary<string, int>();
            var hasStarted = new Dictionary<string, bool>();

            int currentTime = processes.Min(p => p.ArrivalTime);
            int completedCount = 0;
            int processCount = processes.Count;

            foreach (var process in processes)
            {
                remainingBurstTimes[process.ProcessID] = process.BurstTime;
                hasStarted[process.ProcessID] = false;

                processResults[process.ProcessID] = new SchedulingResult
                {
                    ProcessID = process.ProcessID,
                    ArrivalTime = process.ArrivalTime,
                    BurstTime = process.BurstTime,
                    StartTime = -1,
                    FinishTime = 0,
                    WaitingTime = 0,
                    TurnaroundTime = 0
                };
            }

            while (completedCount < processCount)
            {
                var availableProcesses = processes
                    .Where(p => p.ArrivalTime <= currentTime && remainingBurstTimes[p.ProcessID] > 0)
                    .OrderBy(p => remainingBurstTimes[p.ProcessID])
                    .ThenBy(p => p.ArrivalTime)
                    .ToList();

                if (availableProcesses.Count == 0)
                {
                    currentTime++;
                    continue;
                }

                var currentProcess = availableProcesses.First();
                var result = processResults[currentProcess.ProcessID];

                if (!hasStarted[currentProcess.ProcessID])
                {
                    result.StartTime = currentTime;
                    hasStarted[currentProcess.ProcessID] = true;
                }

                remainingBurstTimes[currentProcess.ProcessID]--;
                currentTime++;

                if (remainingBurstTimes[currentProcess.ProcessID] == 0)
                {
                    result.FinishTime = currentTime;
                    result.TurnaroundTime = result.FinishTime - result.ArrivalTime;
                    result.WaitingTime = result.TurnaroundTime - result.BurstTime;
                    completedCount++;
                }
            }

            return processResults.Values.OrderBy(r => r.StartTime).ToList();
        }

        private List<SchedulingResult> RunHRRNAlgorithm(List<ProcessData> processes)
        {
            var results = new List<SchedulingResult>();
            var remainingProcesses = processes.ToList();
            int currentTime = remainingProcesses.Min(p => p.ArrivalTime);

            while (remainingProcesses.Count > 0)
            {
                var availableProcesses = remainingProcesses
                    .Where(p => p.ArrivalTime <= currentTime)
                    .ToList();

                if (availableProcesses.Count == 0)
                {
                    currentTime = remainingProcesses.Min(p => p.ArrivalTime);
                    continue;
                }

                double highestRatio = -1;
                ProcessData selectedProcess = null;

                foreach (var process in availableProcesses)
                {
                    int waitingTime = currentTime - process.ArrivalTime;
                    double responseRatio = (double)(waitingTime + process.BurstTime) / process.BurstTime;

                    if (responseRatio > highestRatio)
                    {
                        highestRatio = responseRatio;
                        selectedProcess = process;
                    }
                    else if (responseRatio == highestRatio)
                    {
                        if (selectedProcess != null && process.ArrivalTime < selectedProcess.ArrivalTime)
                        {
                            selectedProcess = process;
                        }
                    }
                }

                int startTime = Math.Max(currentTime, selectedProcess.ArrivalTime);
                int finishTime = startTime + selectedProcess.BurstTime;
                int waiting = startTime - selectedProcess.ArrivalTime;
                int turnaround = finishTime - selectedProcess.ArrivalTime;

                results.Add(new SchedulingResult
                {
                    ProcessID = selectedProcess.ProcessID,
                    ArrivalTime = selectedProcess.ArrivalTime,
                    BurstTime = selectedProcess.BurstTime,
                    StartTime = startTime,
                    FinishTime = finishTime,
                    WaitingTime = waiting,
                    TurnaroundTime = turnaround
                });

                currentTime = finishTime;
                remainingProcesses.Remove(selectedProcess);
            }

            return results.OrderBy(r => r.StartTime).ToList();
        }

        public class SchedulingResult
        {
            public string ProcessID { get; set; }
            public int ArrivalTime { get; set; }
            public int BurstTime { get; set; }
            public int StartTime { get; set; }
            public int FinishTime { get; set; }
            public int WaitingTime { get; set; }
            public int TurnaroundTime { get; set; }
        }

        private void DisplaySchedulingResults(List<SchedulingResult> results, string algorithmName)
        {
            listView1.Clear();
            listView1.View = View.Details;

            listView1.Columns.Add("Process ID", 100, HorizontalAlignment.Center);
            listView1.Columns.Add("Arrival", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Burst", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Start", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Finish", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Waiting", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Turnaround", 90, HorizontalAlignment.Center);

            if (results == null || results.Count == 0)
            {
                return;
            }

            // show each process
            foreach (var result in results)
            {
                var item = new ListViewItem(result.ProcessID);
                item.SubItems.Add(result.ArrivalTime.ToString());
                item.SubItems.Add(result.BurstTime.ToString());
                item.SubItems.Add(result.StartTime.ToString());
                item.SubItems.Add(result.FinishTime.ToString());
                item.SubItems.Add(result.WaitingTime.ToString());
                item.SubItems.Add(result.TurnaroundTime.ToString());
                listView1.Items.Add(item);
            }

            // averages
            double avgWaiting = results.Average(r => r.WaitingTime);
            double avgTurnaround = results.Average(r => r.TurnaroundTime);

            // Add up all burst times from every process
            // Each process has a BurstTime (how long it runs on CPU)
            // We use Sum() to get the total CPU work done
            int totalBurstTime = results.Sum(r => r.BurstTime);

            // Find the total time the CPU was running until everything finished
            // We look at all processes and take the largest FinishTime
            // The last process to finish determines the total elapsed time
            int totalElapsedTime = results.Max(r => r.FinishTime);

            // Count how many processes we have
            // results is a list of all processes after scheduling
            // Count gives us the total number of processes
            int processCount = results.Count;

            double cpuUtilization = 0;
            double throughput = 0;
            double avgResponseTime = results.Average(r => r.StartTime - r.ArrivalTime);

            if (totalElapsedTime > 0)
            {
                cpuUtilization = ((double)totalBurstTime / totalElapsedTime) * 100.0;
                throughput = (double)processCount / totalElapsedTime;
            }

            //algos
            var infoRow = new ListViewItem("Algorithm");
            infoRow.SubItems.Add(algorithmName);
            infoRow.SubItems.Add($"Processes: {processCount}");
            infoRow.SubItems.Add("");
            infoRow.SubItems.Add("");
            infoRow.SubItems.Add("");
            infoRow.SubItems.Add("");
            listView1.Items.Add(infoRow);

            //summary
            var summaryItem = new ListViewItem("SUMMARY");
            summaryItem.SubItems.Add("");
            summaryItem.SubItems.Add("");
            summaryItem.SubItems.Add("");
            summaryItem.SubItems.Add("");
            summaryItem.SubItems.Add($"Avg Wait: {avgWaiting:F1}");
            summaryItem.SubItems.Add($"Avg Turn: {avgTurnaround:F1}");
            listView1.Items.Add(summaryItem);

            //cpu utilization 
            var cpuRow = new ListViewItem("CPU Utilization");
            cpuRow.SubItems.Add("");
            cpuRow.SubItems.Add("");
            cpuRow.SubItems.Add("");
            cpuRow.SubItems.Add("");
            cpuRow.SubItems.Add($"{cpuUtilization:F1}%");
            cpuRow.SubItems.Add("");
            listView1.Items.Add(cpuRow);

            // throughput 
            var throughputRow = new ListViewItem("Throughput");
            throughputRow.SubItems.Add("");
            throughputRow.SubItems.Add("");
            throughputRow.SubItems.Add("");
            throughputRow.SubItems.Add("");
            throughputRow.SubItems.Add($"{throughput:F2}");
            throughputRow.SubItems.Add("");
            listView1.Items.Add(throughputRow);

            // response time 
            var responseRow = new ListViewItem("Response Time");
            responseRow.SubItems.Add("");
            responseRow.SubItems.Add("");
            responseRow.SubItems.Add("");
            responseRow.SubItems.Add("");
            responseRow.SubItems.Add($"{avgResponseTime:F1}");
            responseRow.SubItems.Add("");
            listView1.Items.Add(responseRow);
        }




        private void InitializeProcessTable()
        {
            processTable = new DataTable();
            processTable.Columns.Add("Process ID", typeof(string));
            processTable.Columns.Add("Burst Time", typeof(int));
            processTable.Columns.Add("Priority", typeof(int));
            processTable.Columns.Add("Arrival Time", typeof(int));

            processDataGrid.DataSource = processTable;
            processDataGrid.AllowUserToAddRows = false;
            processDataGrid.AllowUserToDeleteRows = false;

            if (processDataGrid.Columns.Count > 0)
            {
                processDataGrid.Columns[0].Width = 100;
                processDataGrid.Columns[1].Width = 100;
                processDataGrid.Columns[2].Width = 100;
                processDataGrid.Columns[3].Width = 100;

                processDataGrid.VirtualMode = false;
                processDataGrid.RowHeadersVisible = false;
                processDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            }
        }

        private void SetProcessCount_Click(object sender, EventArgs e)
        {
            if (IsValidProcessCount(txtProcess.Text, out int processCount))
            {
                if (processCount > 50)
                {
                    var result = MessageBox.Show(
                        $"You are creating {processCount} processes. This may impact performance.\n\nContinue?",
                        "Large Dataset Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        txtProcess.Focus();
                        return;
                    }
                }

                processTable.Clear();

                for (int i = 0; i < processCount; i++)
                {
                    DataRow row = processTable.NewRow();
                    row["Process ID"] = $"P{i + 1}";
                    row["Burst Time"] = random.Next(1, 11);
                    row["Priority"] = i + 1;
                    row["Arrival Time"] = 0;
                    processTable.Rows.Add(row);
                }

                cmbLoadExample.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show(
                    $"Please enter a valid number of processes ({MIN_PROCESS_COUNT}-{MAX_PROCESS_COUNT})",
                    "Invalid Input",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtProcess.Focus();
            }
        }

        private void GenerateRandom_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in processTable.Rows)
            {
                row["Burst Time"] = random.Next(1, 21);
                row["Priority"] = random.Next(1, processTable.Rows.Count + 1);
                row["Arrival Time"] = random.Next(0, 10);
            }
        }

        private void ClearAll_Click(object sender, EventArgs e)
        {
            processTable.Clear();
            txtProcess.Text = DEFAULT_PROCESS_COUNT.ToString();
            cmbLoadExample.SelectedIndex = 0;
            txtProcess.Focus();
        }

        private void LoadExample_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLoadExample.SelectedIndex <= 0 || processTable.Rows.Count == 0)
                return;

            switch (cmbLoadExample.SelectedIndex)
            {
                case 1:
                    foreach (DataRow row in processTable.Rows)
                    {
                        row["Burst Time"] = random.Next(1, 6);
                        row["Priority"] = random.Next(1, 5);
                        row["Arrival Time"] = 0;
                    }
                    break;

                case 2:
                    foreach (DataRow row in processTable.Rows)
                    {
                        row["Burst Time"] = random.Next(1, 21);
                        row["Priority"] = random.Next(1, 10);
                        row["Arrival Time"] = random.Next(0, 5);
                    }
                    break;

                case 3:
                    foreach (DataRow row in processTable.Rows)
                    {
                        row["Burst Time"] = random.Next(10, 31);
                        row["Priority"] = random.Next(1, 5);
                        row["Arrival Time"] = random.Next(0, 10);
                    }
                    break;

                case 4:
                    int priority = processTable.Rows.Count;
                    foreach (DataRow row in processTable.Rows)
                    {
                        row["Burst Time"] = random.Next(5, 15);
                        row["Priority"] = priority--;
                        row["Arrival Time"] = 0;
                    }
                    break;
            }

            cmbLoadExample.SelectedIndex = 0;
        }

        private void SaveData_Click(object sender, EventArgs e)
        {
            if (processTable.Rows.Count == 0)
            {
                MessageBox.Show("No process data to save. Please set process count first.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveDialog.DefaultExt = "csv";
                saveDialog.FileName = "ProcessData.csv";
                saveDialog.Title = "Save Process Data";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new System.IO.StreamWriter(saveDialog.FileName))
                        {
                            writer.WriteLine("Process ID,Burst Time,Priority,Arrival Time");

                            foreach (DataRow row in processTable.Rows)
                            {
                                writer.WriteLine($"{row["Process ID"]},{row["Burst Time"]},{row["Priority"]},{row["Arrival Time"]}");
                            }
                        }

                        MessageBox.Show($"Process data saved successfully to:\n{saveDialog.FileName}",
                            "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}",
                            "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadData_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openDialog.DefaultExt = "csv";
                openDialog.Title = "Load Process Data from CSV";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var loadedData = new List<ProcessData>();

                        using (var reader = new System.IO.StreamReader(openDialog.FileName))
                        {
                            var headerLine = reader.ReadLine();

                            if (headerLine == null)
                            {
                                MessageBox.Show("The CSV file is empty.", "Load Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            string line;
                            int lineNumber = 1;

                            while ((line = reader.ReadLine()) != null)
                            {
                                lineNumber++;
                                var parts = line.Split(',');

                                if (parts.Length != 4)
                                {
                                    MessageBox.Show(
                                        $"Invalid format on line {lineNumber}. Expected format: ProcessID,BurstTime,Priority,ArrivalTime",
                                        "Load Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                                    return;
                                }

                                try
                                {
                                    loadedData.Add(new ProcessData
                                    {
                                        ProcessID = parts[0].Trim(),
                                        BurstTime = int.Parse(parts[1].Trim()),
                                        Priority = int.Parse(parts[2].Trim()),
                                        ArrivalTime = int.Parse(parts[3].Trim())
                                    });
                                }
                                catch (FormatException)
                                {
                                    MessageBox.Show($"Invalid number format on line {lineNumber}.",
                                        "Load Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }

                        if (loadedData.Count == 0)
                        {
                            MessageBox.Show("No process data found in the CSV file.",
                                "Load Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }

                        if (loadedData.Count > MAX_PROCESS_COUNT)
                        {
                            MessageBox.Show(
                                $"CSV contains {loadedData.Count} processes, but maximum allowed is {MAX_PROCESS_COUNT}. Loading first {MAX_PROCESS_COUNT} processes.",
                                "Process Count Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                            loadedData = loadedData.Take(MAX_PROCESS_COUNT).ToList();
                        }

                        processTable.Clear();

                        foreach (var process in loadedData)
                        {
                            DataRow row = processTable.NewRow();
                            row["Process ID"] = process.ProcessID;
                            row["Burst Time"] = process.BurstTime;
                            row["Priority"] = process.Priority;
                            row["Arrival Time"] = process.ArrivalTime;
                            processTable.Rows.Add(row);
                        }

                        txtProcess.Text = loadedData.Count.ToString();
                        cmbLoadExample.SelectedIndex = 0;

                        MessageBox.Show($"Successfully loaded {loadedData.Count} processes from:\n{openDialog.FileName}",
                            "Load Complete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file: {ex.Message}",
                            "Load Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void FirstComeFirstServeButton_Click(object sender, EventArgs e)
        {
            var processData = GetProcessDataFromGrid();

            if (processData.Count > 0)
            {
                var results = RunFCFSAlgorithm(processData);
                DisplaySchedulingResults(results, "FCFS - First Come First Serve");

                ShowPanel(resultsPanel);
                sidePanel.Height = btnDashBoard.Height;
                sidePanel.Top = btnDashBoard.Top;
            }
            else
            {
                MessageBox.Show("Please set process count and ensure the data grid has process data.",
                    "No Process Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtProcess.Focus();
            }
        }

        private void ShortestJobFirstButton_Click(object sender, EventArgs e)
        {
            var processData = GetProcessDataFromGrid();

            if (processData.Count > 0)
            {
                var results = RunSJFAlgorithm(processData);
                DisplaySchedulingResults(results, "SJF - Shortest Job First");

                ShowPanel(resultsPanel);
                sidePanel.Height = btnDashBoard.Height;
                sidePanel.Top = btnDashBoard.Top;
            }
            else
            {
                MessageBox.Show("Please set process count and ensure the data grid has process data.",
                    "No Process Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtProcess.Focus();
            }
        }

        private void PriorityButton_Click(object sender, EventArgs e)
        {
            var processData = GetProcessDataFromGrid();

            if (processData.Count > 0)
            {
                var results = RunPriorityAlgorithm(processData);
                DisplaySchedulingResults(results, "Priority Scheduling");

                ShowPanel(resultsPanel);
                sidePanel.Height = btnDashBoard.Height;
                sidePanel.Top = btnDashBoard.Top;
            }
            else
            {
                MessageBox.Show("Please set process count and ensure the data grid has process data.",
                    "No Process Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtProcess.Focus();
            }
        }

        private void RoundRobinButton_Click(object sender, EventArgs e)
        {
            var processData = GetProcessDataFromGrid();

            if (processData.Count > 0)
            {
                string quantumInput = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter quantum time for Round Robin scheduling:",
                    "Quantum Time",
                    "4");

                if (int.TryParse(quantumInput, out int quantumTime) && quantumTime > 0)
                {
                    var results = RunRoundRobinAlgorithm(processData, quantumTime);
                    DisplaySchedulingResults(results, $"Round Robin (Quantum = {quantumTime})");

                    ShowPanel(resultsPanel);
                    sidePanel.Height = btnDashBoard.Height;
                    sidePanel.Top = btnDashBoard.Top;
                }
                else
                {
                    MessageBox.Show("Please enter a valid quantum time.",
                        "Invalid Quantum Time",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please set process count and ensure the data grid has process data.",
                    "No Process Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtProcess.Focus();
            }
        }

        private void SRTFButton_Click(object sender, EventArgs e)
        {
            var processData = GetProcessDataFromGrid();

            if (processData.Count > 0)
            {
                var results = RunSRTFAlgorithm(processData);
                DisplaySchedulingResults(results, "SRTF - Shortest Remaining Time First");

                ShowPanel(resultsPanel);
                sidePanel.Height = btnDashBoard.Height;
                sidePanel.Top = btnDashBoard.Top;
            }
            else
            {
                MessageBox.Show("Please set process count and ensure the data grid has process data.",
                    "No Process Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtProcess.Focus();
            }
        }

        private void HRRNButton_Click(object sender, EventArgs e)
        {
            var processData = GetProcessDataFromGrid();

            if (processData.Count > 0)
            {
                var results = RunHRRNAlgorithm(processData);
                DisplaySchedulingResults(results, "HRRN - Highest Response Ratio Next");

                ShowPanel(resultsPanel);
                sidePanel.Height = btnDashBoard.Height;
                sidePanel.Top = btnDashBoard.Top;
            }
            else
            {
                MessageBox.Show("Please set process count and ensure the data grid has process data.",
                    "No Process Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtProcess.Focus();
            }
        }

        private void ProcessTextBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void RestartApp_Click(object sender, EventArgs e)
        {
            Hide();
            CpuSchedulerForm cpuScheduler = new CpuSchedulerForm();
            cpuScheduler.ShowDialog();
        }

        private void ApplyRoundedCorners(Button button, int radius = 15)
        {
            if (button == null)
                return;

            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, button.Width - 1, button.Height - 1);

            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();

            button.Region = new Region(path);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
        }

        private void CpuSchedulerForm_Load(object sender, EventArgs e)
        {
            sidePanel.Height = btnWelcome.Height;
            sidePanel.Top = btnWelcome.Top;
            listView1.View = View.Details;
            listView1.GridLines = true;

            listView1.Clear();
            listView1.Columns.Add("Information", 400, HorizontalAlignment.Left);
            var welcomeItem = new ListViewItem("No results yet");
            welcomeItem.SubItems.Add("Run a scheduling algorithm to see results here");
            listView1.Items.Add(welcomeItem);

            InitializeWelcomeContent();
            InitializeAboutContent();
            LoadDefaultProcessData();

            ApplyRoundedCorners(btnSetProcessCount);
            ApplyRoundedCorners(btnGenerateRandom);
            ApplyRoundedCorners(btnClearAll);
            ApplyRoundedCorners(btnSaveData);
            ApplyRoundedCorners(btnLoadData);
            ApplyRoundedCorners(btnFCFS);
            ApplyRoundedCorners(btnSJF);
            ApplyRoundedCorners(btnPriority);
            ApplyRoundedCorners(btnRoundRobin);
            ApplyRoundedCorners(btnSRTF);
            ApplyRoundedCorners(btnHRRN);
            ApplyRoundedCorners(btnDarkModeToggle);

            ApplyTheme();
            ShowPanel(welcomePanel);
        }

        private void LoadDefaultProcessData()
        {
            for (int i = 0; i < 5; i++)
            {
                DataRow row = processTable.NewRow();
                row["Process ID"] = $"P{i + 1}";
                row["Burst Time"] = new int[] { 6, 8, 7, 3, 4 }[i];
                row["Priority"] = i + 1;
                row["Arrival Time"] = new int[] { 0, 2, 4, 6, 8 }[i];
                processTable.Rows.Add(row);
            }

            txtProcess.Text = "5";
            cmbLoadExample.SelectedIndex = 0;
        }

        private void ApplyTheme()
        {
            if (isDarkMode)
            {
                ApplyDarkTheme();
                btnDarkModeToggle.Text = "☀️ Light Mode";
            }
            else
            {
                ApplyLightTheme();
                btnDarkModeToggle.Text = "🌙 Dark Mode";
            }
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(45, 45, 48);

            panel1.BackColor = Color.FromArgb(37, 37, 38);
            sidePanel.BackColor = Color.FromArgb(0, 122, 204);

            ApplyDarkThemeToButton(btnWelcome);
            ApplyDarkThemeToButton(btnCpuScheduler);
            ApplyDarkThemeToButton(btnDashBoard);
            ApplyDarkThemeToButton(btnAbout);
            ApplyDarkThemeToButton(btnDarkModeToggle);

            restartApp.BackColor = Color.FromArgb(37, 37, 38);
            restartApp.ForeColor = Color.FromArgb(241, 241, 241);

            label1.ForeColor = Color.FromArgb(153, 153, 153);

            contentPanel.BackColor = Color.FromArgb(30, 30, 30);
            welcomePanel.BackColor = Color.FromArgb(30, 30, 30);
            schedulerPanel.BackColor = Color.FromArgb(30, 30, 30);
            resultsPanel.BackColor = Color.FromArgb(30, 30, 30);
            aboutPanel.BackColor = Color.FromArgb(30, 30, 30);

            welcomeTextBox.BackColor = Color.FromArgb(37, 37, 38);
            welcomeTextBox.ForeColor = Color.FromArgb(241, 241, 241);
            aboutTextBox.BackColor = Color.FromArgb(37, 37, 38);
            aboutTextBox.ForeColor = Color.FromArgb(241, 241, 241);

            labelProcess.ForeColor = Color.FromArgb(241, 241, 241);
            txtProcess.BackColor = Color.FromArgb(51, 51, 55);
            txtProcess.ForeColor = Color.FromArgb(241, 241, 241);

            processDataGrid.BackgroundColor = Color.FromArgb(37, 37, 38);
            processDataGrid.DefaultCellStyle.BackColor = Color.FromArgb(51, 51, 55);
            processDataGrid.DefaultCellStyle.ForeColor = Color.FromArgb(241, 241, 241);
            processDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            processDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(241, 241, 241);
            processDataGrid.GridColor = Color.FromArgb(62, 62, 66);

            cmbLoadExample.BackColor = Color.FromArgb(51, 51, 55);
            cmbLoadExample.ForeColor = Color.FromArgb(241, 241, 241);

            listView1.BackColor = Color.FromArgb(37, 37, 38);
            listView1.ForeColor = Color.FromArgb(241, 241, 241);

            ApplyDarkThemeToSchedulerButton(btnSetProcessCount);
            ApplyDarkThemeToSchedulerButton(btnGenerateRandom);
            ApplyDarkThemeToSchedulerButton(btnClearAll);
            ApplyDarkThemeToSchedulerButton(btnSaveData);
            ApplyDarkThemeToSchedulerButton(btnLoadData);
            ApplyDarkThemeToSchedulerButton(btnFCFS);
            ApplyDarkThemeToSchedulerButton(btnSJF);
            ApplyDarkThemeToSchedulerButton(btnPriority);
            ApplyDarkThemeToSchedulerButton(btnRoundRobin);
            ApplyDarkThemeToSchedulerButton(btnSRTF);
            ApplyDarkThemeToSchedulerButton(btnHRRN);
        }

        private void ApplyLightTheme()
        {
            this.BackColor = SystemColors.Control;

            panel1.BackColor = SystemColors.InactiveBorder;
            sidePanel.BackColor = Color.SeaGreen;

            ApplyLightThemeToButton(btnWelcome);
            ApplyLightThemeToButton(btnCpuScheduler);
            ApplyLightThemeToButton(btnDashBoard);
            ApplyLightThemeToButton(btnAbout);
            ApplyLightThemeToButton(btnDarkModeToggle);

            restartApp.BackColor = SystemColors.InactiveBorder;
            restartApp.ForeColor = Color.DarkBlue;

            label1.ForeColor = SystemColors.ControlText;

            contentPanel.BackColor = SystemColors.Control;
            welcomePanel.BackColor = SystemColors.Control;
            schedulerPanel.BackColor = SystemColors.Control;
            resultsPanel.BackColor = SystemColors.Control;
            aboutPanel.BackColor = SystemColors.Control;

            welcomeTextBox.BackColor = SystemColors.Window;
            welcomeTextBox.ForeColor = SystemColors.WindowText;
            aboutTextBox.BackColor = SystemColors.Window;
            aboutTextBox.ForeColor = SystemColors.WindowText;

            labelProcess.ForeColor = SystemColors.ControlText;
            txtProcess.BackColor = SystemColors.Window;
            txtProcess.ForeColor = SystemColors.WindowText;

            processDataGrid.BackgroundColor = SystemColors.Window;
            processDataGrid.DefaultCellStyle.BackColor = SystemColors.Window;
            processDataGrid.DefaultCellStyle.ForeColor = SystemColors.WindowText;
            processDataGrid.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            processDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
            processDataGrid.GridColor = SystemColors.ControlDark;

            cmbLoadExample.BackColor = SystemColors.Window;
            cmbLoadExample.ForeColor = SystemColors.WindowText;

            listView1.BackColor = SystemColors.Window;
            listView1.ForeColor = SystemColors.WindowText;

            ApplyLightThemeToSchedulerButton(btnSetProcessCount);
            ApplyLightThemeToSchedulerButton(btnGenerateRandom);
            ApplyLightThemeToSchedulerButton(btnClearAll);
            ApplyLightThemeToSchedulerButton(btnSaveData);
            ApplyLightThemeToSchedulerButton(btnLoadData);

            btnFCFS.BackColor = Color.Beige;
            btnSJF.BackColor = Color.AntiqueWhite;
            btnPriority.BackColor = Color.Bisque;
            btnRoundRobin.BackColor = Color.PapayaWhip;

            if (btnSRTF != null)
                btnSRTF.BackColor = Color.MistyRose;

            if (btnHRRN != null)
                btnHRRN.BackColor = Color.LemonChiffon;

            btnFCFS.ForeColor = SystemColors.ControlText;
            btnSJF.ForeColor = SystemColors.ControlText;
            btnPriority.ForeColor = SystemColors.ControlText;
            btnRoundRobin.ForeColor = SystemColors.ControlText;

            if (btnSRTF != null)
                btnSRTF.ForeColor = SystemColors.ControlText;

            if (btnHRRN != null)
                btnHRRN.ForeColor = SystemColors.ControlText;
        }

        private void ApplyDarkThemeToButton(Button button)
        {
            if (button == null)
                return;

            button.BackColor = Color.FromArgb(37, 37, 38);
            button.ForeColor = Color.FromArgb(241, 241, 241);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 66);
        }

        private void ApplyLightThemeToButton(Button button)
        {
            if (button == null)
                return;

            button.BackColor = SystemColors.InactiveBorder;
            button.ForeColor = SystemColors.ControlText;
            button.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
        }

        private void ApplyDarkThemeToSchedulerButton(Button button)
        {
            if (button == null)
                return;

            button.BackColor = Color.FromArgb(51, 51, 55);
            button.ForeColor = Color.FromArgb(241, 241, 241);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);
        }

        private void ApplyLightThemeToSchedulerButton(Button button)
        {
            if (button == null)
                return;

            button.BackColor = SystemColors.ButtonFace;
            button.ForeColor = SystemColors.ControlText;
            button.FlatAppearance.MouseOverBackColor = Color.PaleGreen;
        }
    }

    public class RoundedButton : Button
    {
        private int borderRadius = 10;
        private Color borderColor = Color.FromArgb(200, 200, 200);

        public int BorderRadius
        {
            get { return borderRadius; }
            set { borderRadius = value; Invalidate(); }
        }

        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            path.AddArc(rect.X, rect.Y, borderRadius, borderRadius, 180, 90);
            path.AddArc(rect.X + rect.Width - borderRadius, rect.Y, borderRadius, borderRadius, 270, 90);
            path.AddArc(rect.X + rect.Width - borderRadius, rect.Y + rect.Height - borderRadius, borderRadius, borderRadius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - borderRadius, borderRadius, borderRadius, 90, 90);
            path.CloseAllFigures();

            Region = new Region(path);

            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                g.FillPath(brush, path);
            }

            using (Pen pen = new Pen(borderColor, 1))
            {
                g.DrawPath(pen, path);
            }

            TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            path.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}