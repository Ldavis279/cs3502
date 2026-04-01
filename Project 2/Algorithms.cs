using System;
using System.Windows.Forms;

namespace CpuScheduler
{
    public static class Algorithms
    {
        /// <summary>
        /// Executes the First Come First Serve scheduling algorithm.
        /// </summary>
        /// <param name="processCountInput">The number of processes to schedule.</param>
        public static void RunFirstComeFirstServe(string processCountInput)
        {
            if (!int.TryParse(processCountInput, out int processCount) || processCount <= 0)
            {
                MessageBox.Show("Invalid number of processes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double[] burstTimes = new double[processCount];
            double[] waitingTimes = new double[processCount];
            double totalWaitingTime = 0.0;
            double averageWaitingTime;
            int i;

            DialogResult result = MessageBox.Show(
                "First Come First Serve Scheduling",
                string.Empty,
                MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                for (i = 0; i < processCount; i++)
                {
                    string input = Microsoft.VisualBasic.Interaction.InputBox(
                        "Enter Burst time:",
                        "Burst time for P" + (i + 1),
                        string.Empty,
                        -1,
                        -1);

                    if (!double.TryParse(input, out burstTimes[i]) || burstTimes[i] < 0)
                    {
                        MessageBox.Show("Invalid burst time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }

                for (i = 0; i < processCount; i++)
                {
                    if (i == 0)
                    {
                        waitingTimes[i] = 0;
                    }
                    else
                    {
                        waitingTimes[i] = waitingTimes[i - 1] + burstTimes[i - 1];
                        MessageBox.Show(
                            "Waiting time for P" + (i + 1) + " = " + waitingTimes[i],
                            "Job Queue",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.None);
                    }
                }
                for (i = 0; i < processCount; i++)
                {
                    totalWaitingTime = totalWaitingTime + waitingTimes[i];
                }
                averageWaitingTime = totalWaitingTime / processCount;
                MessageBox.Show(
                    "Average waiting time for " + processCount + " processes = " + averageWaitingTime + " sec(s)",
                    "Average Waiting Time",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.None);
            }
        }

        /// <summary>
        /// Executes the Shortest Job First scheduling algorithm.
        /// </summary>
        /// <param name="processCountInput">The number of processes to schedule.</param>
        public static void RunShortestJobFirst(string processCountInput)
        {
            if (!int.TryParse(processCountInput, out int processCount) || processCount <= 0)
            {
                MessageBox.Show("Invalid number of processes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double[] burstTimes = new double[processCount];
            double[] waitingTimes = new double[processCount];
            double[] sortedBurstTimes = new double[processCount];
            double totalWaitingTime = 0.0;
            double averageWaitingTime;
            int x, i;
            double temp = 0.0;
            bool found = false;

            DialogResult result = MessageBox.Show(
                "Shortest Job First Scheduling",
                string.Empty,
                MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                for (i = 0; i < processCount; i++)
                {
                    string input =
                        Microsoft.VisualBasic.Interaction.InputBox("Enter burst time: ",
                                                           "Burst time for P" + (i + 1),
                                                           "",
                                                           -1, -1);

                    if (!double.TryParse(input, out burstTimes[i]) || burstTimes[i] < 0)
                    {
                        MessageBox.Show("Invalid burst time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                for (i = 0; i < processCount; i++)
                {
                    sortedBurstTimes[i] = burstTimes[i];
                }
                for (x = 0; x <= processCount - 2; x++)
                {
                    for (i = 0; i <= processCount - 2; i++)
                    {
                        if (sortedBurstTimes[i] > sortedBurstTimes[i + 1])
                        {
                            temp = sortedBurstTimes[i];
                            sortedBurstTimes[i] = sortedBurstTimes[i + 1];
                            sortedBurstTimes[i + 1] = temp;
                        }
                    }
                }
                for (i = 0; i < processCount; i++)
                {
                    if (i == 0)
                    {
                        for (x = 0; x < processCount; x++)
                        {
                            if (sortedBurstTimes[i] == burstTimes[x] && found == false)
                            {
                                waitingTimes[i] = 0;
                                MessageBox.Show(
                                    "Waiting time for P" + (x + 1) + " = " + waitingTimes[i],
                                    "Waiting time:",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.None);
                                burstTimes[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                    else
                    {
                        for (x = 0; x < processCount; x++)
                        {
                            if (sortedBurstTimes[i] == burstTimes[x] && found == false)
                            {
                                waitingTimes[i] = waitingTimes[i - 1] + sortedBurstTimes[i - 1];
                                MessageBox.Show(
                                    "Waiting time for P" + (x + 1) + " = " + waitingTimes[i],
                                    "Waiting time",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.None);
                                burstTimes[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                }
                for (i = 0; i < processCount; i++)
                {
                    totalWaitingTime = totalWaitingTime + waitingTimes[i];
                }
                averageWaitingTime = totalWaitingTime / processCount;
                MessageBox.Show(
                    "Average waiting time for " + processCount + " processes = " + averageWaitingTime + " sec(s)",
                    "Average waiting time",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Executes the Priority scheduling algorithm.
        /// </summary>
        /// <param name="processCountInput">The number of processes to schedule.</param>
        public static void RunPriorityScheduling(string processCountInput)
        {
            if (!int.TryParse(processCountInput, out int processCount) || processCount <= 0)
            {
                MessageBox.Show("Invalid number of processes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Priority Scheduling",
                string.Empty,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                double[] burstTimes = new double[processCount];
                double[] waitingTimes = new double[processCount + 1];
                int[] priorities = new int[processCount];
                int[] sortedPriorities = new int[processCount];
                int x, i;
                double totalWaitingTime = 0.0;
                double averageWaitingTime;
                int temp = 0;
                bool found = false;
                for (i = 0; i < processCount; i++)
                {
                    string input =
                        Microsoft.VisualBasic.Interaction.InputBox("Enter burst time: ",
                                                           "Burst time for P" + (i + 1),
                                                           "",
                                                           -1, -1);
                    if (!double.TryParse(input, out burstTimes[i]) || burstTimes[i] < 0)
                    {
                        MessageBox.Show("Invalid burst time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                for (i = 0; i < processCount; i++)
                {
                    string input2 =
                        Microsoft.VisualBasic.Interaction.InputBox("Enter priority: ",
                                                           "Priority for P" + (i + 1),
                                                           "",
                                                           -1, -1);
                    if (!int.TryParse(input2, out priorities[i]) || priorities[i] < 0)
                    {
                        MessageBox.Show("Invalid priority", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                for (i = 0; i < processCount; i++)
                {
                    sortedPriorities[i] = priorities[i];
                }
                for (x = 0; x <= processCount - 2; x++)
                {
                    for (i = 0; i <= processCount - 2; i++)
                    {
                        if (sortedPriorities[i] > sortedPriorities[i + 1])
                        {
                            temp = sortedPriorities[i];
                            sortedPriorities[i] = sortedPriorities[i + 1];
                            sortedPriorities[i + 1] = temp;
                        }
                    }
                }
                for (i = 0; i < processCount; i++)
                {
                    if (i == 0)
                    {
                        for (x = 0; x < processCount; x++)
                        {
                            if (sortedPriorities[i] == priorities[x] && found == false)
                            {
                                waitingTimes[i] = 0;
                                MessageBox.Show(
                                    "Waiting time for P" + (x + 1) + " = " + waitingTimes[i],
                                    "Waiting time",
                                    MessageBoxButtons.OK);
                                temp = x;
                                priorities[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                    else
                    {
                        for (x = 0; x < processCount; x++)
                        {
                            if (sortedPriorities[i] == priorities[x] && found == false)
                            {
                                waitingTimes[i] = waitingTimes[i - 1] + burstTimes[temp];
                                MessageBox.Show(
                                    "Waiting time for P" + (x + 1) + " = " + waitingTimes[i],
                                    "Waiting time",
                                    MessageBoxButtons.OK);
                                temp = x;
                                priorities[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                }
                for (i = 0; i < processCount; i++)
                {
                    totalWaitingTime = totalWaitingTime + waitingTimes[i];
                }
                averageWaitingTime = totalWaitingTime / processCount;
                MessageBox.Show(
                    "Average waiting time for " + processCount + " processes = " + averageWaitingTime + " sec(s)",
                    "Average waiting time",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Executes the Round Robin scheduling algorithm.
        /// </summary>
        /// <param name="processCountInput">The number of processes to schedule.</param>
        public static void RunRoundRobin(string processCountInput)
        {
            if (!int.TryParse(processCountInput, out int processCount) || processCount <= 0)
            {
                MessageBox.Show("Invalid number of processes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int index, counter = 0;
            double total;
            double timeQuantum;
            double waitTime = 0, turnaroundTime = 0;
            double averageWaitTime, averageTurnaroundTime;
            double[] arrivalTimes = new double[processCount];
            double[] burstTimes = new double[processCount];
            double[] remainingTimes = new double[processCount];
            int remainingProcesses = processCount;

            DialogResult result = MessageBox.Show(
                "Round Robin Scheduling",
                string.Empty,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                for (index = 0; index < processCount; index++)
                {
                    string arrivalInput =
                            Microsoft.VisualBasic.Interaction.InputBox("Enter arrival time: ",
                                                               "Arrival time for P" + (index + 1),
                                                               "",
                                                               -1, -1);
                    if (!double.TryParse(arrivalInput, out arrivalTimes[index]) || arrivalTimes[index] < 0)
                    {
                        MessageBox.Show("Invalid arrival time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string burstInput =
                            Microsoft.VisualBasic.Interaction.InputBox("Enter burst time: ",
                                                               "Burst time for P" + (index + 1),
                                                               "",
                                                               -1, -1);
                    if (!double.TryParse(burstInput, out burstTimes[index]) || burstTimes[index] < 0)
                    {
                        MessageBox.Show("Invalid burst time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    remainingTimes[index] = burstTimes[index];
                }
                string timeQuantumInput =
                            Microsoft.VisualBasic.Interaction.InputBox("Enter time quantum: ", "Time Quantum",
                                                               "",
                                                               -1, -1);

                if (!double.TryParse(timeQuantumInput, out timeQuantum) || timeQuantum <= 0)
                {
                    MessageBox.Show("Invalid quantum time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Helper.QuantumTime = timeQuantumInput;

                for (total = 0, index = 0; remainingProcesses != 0;)
                {
                    if (remainingTimes[index] <= timeQuantum && remainingTimes[index] > 0)
                    {
                        total = total + remainingTimes[index];
                        remainingTimes[index] = 0;
                        counter = 1;
                    }
                    else if (remainingTimes[index] > 0)
                    {
                        remainingTimes[index] = remainingTimes[index] - timeQuantum;
                        total = total + timeQuantum;
                    }
                    if (remainingTimes[index] == 0 && counter == 1)
                    {
                        remainingProcesses--;
                        MessageBox.Show("Turnaround time for Process " + (index + 1) + " : " + (total - arrivalTimes[index]), "Turnaround time for Process " + (index + 1), MessageBoxButtons.OK);
                        MessageBox.Show("Wait time for Process " + (index + 1) + " : " + (total - arrivalTimes[index] - burstTimes[index]), "Wait time for Process " + (index + 1), MessageBoxButtons.OK);
                        turnaroundTime = turnaroundTime + total - arrivalTimes[index];
                        waitTime = waitTime + total - arrivalTimes[index] - burstTimes[index];
                        counter = 0;
                    }
                    if (index == processCount - 1)
                    {
                        index = 0;
                    }
                    else if (arrivalTimes[index + 1] <= total)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                }
                averageWaitTime = Convert.ToInt64(waitTime * 1.0 / processCount);
                averageTurnaroundTime = Convert.ToInt64(turnaroundTime * 1.0 / processCount);
                MessageBox.Show("Average wait time for " + processCount + " processes: " + averageWaitTime + " sec(s)", string.Empty, MessageBoxButtons.OK);
                MessageBox.Show("Average turnaround time for " + processCount + " processes: " + averageTurnaroundTime + " sec(s)", string.Empty, MessageBoxButtons.OK);
            }
        }

        // TODO: Add new scheduling algorithms below. Use the above methods as
        // examples when expanding functionality.




        /// <summary>
        /// Executes the Shortest Remaining Time First (SRTF) scheduling algorithm.
        /// SRTF is the preemptive version of SJF. At each time unit the CPU picks
        /// whichever arrived process has the least remaining burst time, preempting
        /// the current process if a shorter one arrives.
        /// </summary>
        /// <param name="processCountInput">The number of processes to schedule.</param>
        public static void RunShortestRemainingTimeFirst(string processCountInput)
        {
            // Try to convert the process count input into an integer
            // If it is not a number OR it is 0 or less, show an error and stop
            if (!int.TryParse(processCountInput, out int processCount) || processCount <= 0)
            {
                MessageBox.Show("Invalid number of processes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show a message box telling the user this is the SRTF algorithm
            // Yes means continue, No means do nothing
            DialogResult result = MessageBox.Show(
                "Shortest Remaining Time First (SRTF) Scheduling",
                string.Empty,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            // Only run the algorithm if the user clicks Yes
            if (result == DialogResult.Yes)
            {
                // Array to store the burst time for each process
                double[] burstTimes = new double[processCount];

                // Array to store the arrival time for each process
                double[] arrivalTimes = new double[processCount];

                // Array to store how much burst time each process still has left
                double[] remainingTimes = new double[processCount];

                // Array to store the waiting time for each process
                double[] waitingTimes = new double[processCount];

                // Array to store the turnaround time for each process
                double[] turnaroundTimes = new double[processCount];

                // Array to track whether each process is finished or not
                bool[] completed = new bool[processCount];

                // Loop variable
                int i;

                // Loop through every process and ask the user for input
                for (i = 0; i < processCount; i++)
                {
                    // Ask the user for the arrival time of this process
                    string arrivalInput = Microsoft.VisualBasic.Interaction.InputBox(
                        "Enter arrival time:",
                        "Arrival time for P" + (i + 1),
                        "",
                        -1, -1);

                    // Try to convert the arrival time into a number
                    // If it is invalid or negative, show an error and stop
                    if (!double.TryParse(arrivalInput, out arrivalTimes[i]) || arrivalTimes[i] < 0)
                    {
                        MessageBox.Show("Invalid arrival time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Ask the user for the burst time of this process
                    string burstInput = Microsoft.VisualBasic.Interaction.InputBox(
                        "Enter burst time:",
                        "Burst time for P" + (i + 1),
                        "",
                        -1, -1);

                    // Try to convert the burst time into a number
                    // If it is invalid or 0/negative, show an error and stop
                    if (!double.TryParse(burstInput, out burstTimes[i]) || burstTimes[i] <= 0)
                    {
                        MessageBox.Show("Invalid burst time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // At the start, the remaining time is the same as the full burst time
                    remainingTimes[i] = burstTimes[i];
                }

                // Start CPU time at 0
                double currentTime = 0;

                // Keep track of how many processes have finished
                int completedCount = 0;

                // Keep running until all processes are completed
                while (completedCount < processCount)
                {
                    // This will store the index of the process with the shortest remaining time
                    int shortest = -1;

                    // Start with the largest possible value so any real remaining time will be smaller
                    double minRemaining = double.MaxValue;

                    // Check every process to find the best one to run right now
                    for (i = 0; i < processCount; i++)
                    {
                        // Pick the process only if:
                        // 1. it is not completed
                        // 2. it has already arrived
                        // 3. its remaining time is smaller than the current minimum
                        if (!completed[i] && arrivalTimes[i] <= currentTime && remainingTimes[i] < minRemaining)
                        {
                            minRemaining = remainingTimes[i];
                            shortest = i;
                        }
                    }

                    // If no process is ready yet, move time forward by 1 and check again
                    if (shortest == -1)
                    {
                        currentTime++;
                        continue;
                    }

                    // Run the selected process for 1 time unit
                    // SRTF is preemptive, so it only runs one unit at a time and then rechecks
                    remainingTimes[shortest]--;
                    currentTime++;

                    // If the remaining time becomes 0, that process is finished
                    if (remainingTimes[shortest] == 0)
                    {
                        // Mark the process as completed
                        completed[shortest] = true;

                        // Increase the number of finished processes
                        completedCount++;

                        // Turnaround time = finish time - arrival time
                        turnaroundTimes[shortest] = currentTime - arrivalTimes[shortest];

                        // Waiting time = turnaround time - burst time
                        waitingTimes[shortest] = turnaroundTimes[shortest] - burstTimes[shortest];

                        // Show the waiting time and turnaround time for the finished process
                        MessageBox.Show(
                            "Waiting time for P" + (shortest + 1) + " = " + waitingTimes[shortest] +
                            "\nTurnaround time for P" + (shortest + 1) + " = " + turnaroundTimes[shortest],
                            "SRTF Result for P" + (shortest + 1),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.None);
                    }
                }

                // Variables to add up all waiting and turnaround times
                double totalWaitingTime = 0.0, totalTurnaroundTime = 0.0;

                // Loop through all processes and add their times
                for (i = 0; i < processCount; i++)
                {
                    totalWaitingTime += waitingTimes[i];
                    totalTurnaroundTime += turnaroundTimes[i];
                }

                // Show the average waiting time and average turnaround time
                MessageBox.Show(
                    "Average waiting time for " + processCount + " processes = " + (totalWaitingTime / processCount) + " sec(s)" +
                    "\nAverage turnaround time for " + processCount + " processes = " + (totalTurnaroundTime / processCount) + " sec(s)",
                    "SRTF Average Times",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Executes the Highest Response Ratio Next (HRRN) scheduling algorithm.
        /// HRRN is non-preemptive. At each scheduling point it picks the process
        /// with the highest response ratio:
        ///     Response Ratio = (Waiting Time + Burst Time) / Burst Time
        /// This naturally favours short jobs while preventing starvation of long ones.
        /// </summary>
        /// <param name="processCountInput">The number of processes to schedule.</param>
        
        
        public static void RunHighestResponseRatioNext(string processCountInput)
        {// Try to convert input into a valid number of processes
         // If invalid or <= 0, show error and stop
            if (!int.TryParse(processCountInput, out int processCount) || processCount <= 0)
            {
                MessageBox.Show("Invalid number of processes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show a message box explaining this is HRRN scheduling
            DialogResult result = MessageBox.Show(
                "Highest Response Ratio Next (HRRN) Scheduling",
                string.Empty,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            // Only continue if user clicks Yes
            if (result == DialogResult.Yes)
            {
                // Arrays to store process data
                double[] burstTimes = new double[processCount];       // total CPU time needed
                double[] arrivalTimes = new double[processCount];     // when process arrives
                double[] waitingTimes = new double[processCount];     // how long it waits
                double[] turnaroundTimes = new double[processCount];  // total time in system
                bool[] completed = new bool[processCount];            // track if process is done
                int i;

                // Get input for each process
                for (i = 0; i < processCount; i++)
                {
                    // Ask for arrival time
                    string arrivalInput = Microsoft.VisualBasic.Interaction.InputBox(
                        "Enter arrival time:",
                        "Arrival time for P" + (i + 1),
                        "",
                        -1, -1);

                    // Validate arrival time
                    if (!double.TryParse(arrivalInput, out arrivalTimes[i]) || arrivalTimes[i] < 0)
                    {
                        MessageBox.Show("Invalid arrival time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Ask for burst time
                    string burstInput = Microsoft.VisualBasic.Interaction.InputBox(
                        "Enter burst time:",
                        "Burst time for P" + (i + 1),
                        "",
                        -1, -1);

                    // Validate burst time
                    if (!double.TryParse(burstInput, out burstTimes[i]) || burstTimes[i] <= 0)
                    {
                        MessageBox.Show("Invalid burst time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Start current time at the earliest arrival time
                double currentTime = arrivalTimes[0];
                for (i = 1; i < processCount; i++)
                    if (arrivalTimes[i] < currentTime)
                        currentTime = arrivalTimes[i];

                // Track how many processes are finished
                int completedCount = 0;

                // Keep running until all processes are done
                while (completedCount < processCount)
                {
                    // This will store which process we choose
                    int selected = -1;

                    // Start with smallest possible value
                    double highestRatio = double.MinValue;

                    // Check every process
                    for (i = 0; i < processCount; i++)
                    {
                        // Only consider processes that:
                        // 1. are not completed
                        // 2. have already arrived
                        if (!completed[i] && arrivalTimes[i] <= currentTime)
                        {
                            // Calculate waiting time so far
                            double waitingTime = currentTime - arrivalTimes[i];

                            // HRRN formula:
                            // Response Ratio = (Waiting Time + Burst Time) / Burst Time
                            double responseRatio = (waitingTime + burstTimes[i]) / burstTimes[i];

                            // Pick the process with the highest response ratio
                            if (responseRatio > highestRatio)
                            {
                                highestRatio = responseRatio;
                                selected = i;
                            }
                        }
                    }

                    // If no process is ready yet
                    if (selected == -1)
                    {
                        // Jump time forward to the next arriving process
                        double nextArrival = double.MaxValue;

                        for (i = 0; i < processCount; i++)
                            if (!completed[i] && arrivalTimes[i] > currentTime && arrivalTimes[i] < nextArrival)
                                nextArrival = arrivalTimes[i];

                        currentTime = nextArrival;
                        continue;
                    }

                    // Run the selected process fully (HRRN is non-preemptive)

                    // Waiting time = current time - arrival time
                    waitingTimes[selected] = currentTime - arrivalTimes[selected];

                    // Move time forward by the full burst time
                    currentTime += burstTimes[selected];

                    // Turnaround time = finish time - arrival time
                    turnaroundTimes[selected] = currentTime - arrivalTimes[selected];

                    // Mark process as done
                    completed[selected] = true;

                    // Increase completed count
                    completedCount++;

                    // Show results for this process
                    MessageBox.Show(
                        "Response ratio for P" + (selected + 1) + " = " + Math.Round(highestRatio, 2) +
                        "\nWaiting time for P" + (selected + 1) + " = " + waitingTimes[selected] +
                        "\nTurnaround time for P" + (selected + 1) + " = " + turnaroundTimes[selected],
                        "HRRN Result for P" + (selected + 1),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.None);
                }

                // Calculate totals
                double totalWaitingTime = 0.0, totalTurnaroundTime = 0.0;

                for (i = 0; i < processCount; i++)
                {
                    totalWaitingTime += waitingTimes[i];
                    totalTurnaroundTime += turnaroundTimes[i];
                }

                // Show averages
                MessageBox.Show(
                    "Average waiting time for " + processCount + " processes = " + (totalWaitingTime / processCount) + " sec(s)" +
                    "\nAverage turnaround time for " + processCount + " processes = " + (totalTurnaroundTime / processCount) + " sec(s)",
                    "HRRN Average Times",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

    }
}

