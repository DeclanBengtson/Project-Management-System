using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TaskManager
{
    private Dictionary<string, Task> tasks;

    public TaskManager()
    {
        tasks = new Dictionary<string, Task>();
    }

    public void LoadTasksFromFile(string filePath)
    {
        // Clears any existing tasks
        tasks.Clear();

        try
        {
            // Reads all the lines from the file and saves them to a string array
            string[] lines = File.ReadAllLines(filePath);

            //loops through each line of the string array
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                string taskId = parts[0].Trim();
                int timeNeeded = int.Parse(parts[1].Trim());

                Task task = new Task(taskId, timeNeeded);

                if (parts.Length > 2)
                {
                    for (int i = 2; i < parts.Length; i++)
                    {
                        string dependency = parts[i].Trim();
                        task.AddDependency(dependency);
                    }
                }
                
                tasks.Add(taskId, task);
            }

            Console.WriteLine("Tasks loaded successfully from the file.");
        }
        // Outputs error message if an exception was encountered
        catch (Exception e)
        {
            Console.WriteLine("Error loading tasks from the file: " + e.Message);
        }
    }

    public void AddTask(string taskId, int timeNeeded, List<string> dependencies)
    {
        // Check to see if the task being added already exists
        if (tasks.ContainsKey(taskId))
        {
            Console.WriteLine("Task with ID '{0}' already exists.", taskId);
            return;
        }
        // If the task doesn't exist, create a new task
        Task task = new Task(taskId, timeNeeded);

        if (dependencies != null)
        {
            foreach (string dependency in dependencies)
            {
                if (tasks.ContainsKey(dependency))
                {
                    task.AddDependency(dependency);
                }
                
                else
                {
                    Console.WriteLine("Dependency task '{0}' does not exist. Ignoring it.", dependency);
                }
            }
        }

        tasks.Add(taskId, task);
        Console.WriteLine("Task '{0}' added successfully.", taskId);
    }

    public void RemoveTask(string taskId)
    {
        // Checks if task exists
        if (!tasks.ContainsKey(taskId))
        {
            Console.WriteLine("Task not found.");
            return;
        }

        Task removedTask = tasks[taskId];
        tasks.Remove(taskId);

        // Remove the task from the dependencies of other tasks
        foreach (Task task in tasks.Values)
        {
            task.RemoveDependency(taskId);
        }

        Console.WriteLine($"Task {taskId} has been removed.");

        // Check if any tasks have no remaining dependencies and update their earliest times
        foreach (Task task in tasks.Values)
        {
            if (task.Dependencies.Count == 0)
            {
                GetEarliestTime(task);
            }
        }
    }
    

    public void UpdateTaskTime(string taskId, int timeNeeded)
    {
        //Checks if task exists
        if (!tasks.ContainsKey(taskId))
        {
            Console.WriteLine("Task with ID '{0}' does not exist.", taskId);
            return;
        }

        tasks[taskId].TimeNeeded = timeNeeded;
        Console.WriteLine("Time needed for task '{0}' updated successfully.", taskId);
    }

    public void SaveTasksToFile(string filePath)
    {
        // Saves the tasks to a file
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Task task in tasks.Values)
                {
                    string line = task.ToString();
                    writer.WriteLine(line);
                }

                Console.WriteLine("Tasks saved successfully to the file.");
            }
        }
        // Catches any errors that stops the tasks from being saved and outputs the error
        catch (Exception e)
        {
            Console.WriteLine("Error saving tasks to the file: " + e.Message);
        }
    }

    public void GenerateTaskSequence()
    {
        List<string> sequence = new List<string>();

        HashSet<string> visited = new HashSet<string>();

        foreach (Task task in tasks.Values)
        {
            if (!visited.Contains(task.TaskId))
            {
                Visit(task, visited, sequence);
            }
        }

        SaveSequenceToFile(sequence);
        Console.WriteLine("Task sequence generated and saved to Sequence.txt which can be found in the bin/debug/net6.0 folder");
    }

    private void Visit(Task task, HashSet<string> visited, List<string> sequence)
    {
        visited.Add(task.TaskId);

        foreach (string dependency in task.Dependencies)
        {
            if (!visited.Contains(dependency) && tasks.ContainsKey(dependency))
            {
                Visit(tasks[dependency], visited, sequence);
            }
        }

        sequence.Add(task.TaskId);
    }

    public void GenerateEarliestTimes()
    {
        Dictionary<string, int> earliestTimes = new Dictionary<string, int>();

        foreach (Task task in tasks.Values)
        {
            int earliestTime = GetEarliestTime(task);
            earliestTimes.Add(task.TaskId, earliestTime-task.TimeNeeded);
        }

        SaveEarliestTimesToFile(earliestTimes);
        Console.WriteLine("Earliest times calculated and saved to EarliestTimes.txt which can be found in the bin/debug/net6.0 folder");
    }

    private int GetEarliestTime(Task task)
    {
        if (task.EarliestTime.HasValue)
        {
            return task.EarliestTime.Value;
        }

        int maxDependencyTime = 0;

        foreach (string dependency in task.Dependencies)
        {
            if (tasks.ContainsKey(dependency))
            {
                int dependencyTime = GetEarliestTime(tasks[dependency]);
                maxDependencyTime = Math.Max(maxDependencyTime, dependencyTime);
            }
        }

        task.EarliestTime = maxDependencyTime + task.TimeNeeded;
        return task.EarliestTime.Value;
    }

    private void SaveSequenceToFile(List<string> sequence)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter("Sequence.txt"))
            {
                string line = string.Join(", ", sequence);
                writer.WriteLine(line);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error saving task sequence to the file: " + e.Message);
        }
    }

    private void SaveEarliestTimesToFile(Dictionary<string, int> earliestTimes)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter("EarliestTimes.txt"))
            {
                foreach (var pair in earliestTimes.OrderBy(p => p.Key))
                {
                    string line = string.Format("{0}, {1}", pair.Key, pair.Value);
                    writer.WriteLine(line);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error saving earliest times to the file: " + e.Message);
        }
    }

    public void PrintTasks()
    {
        Console.WriteLine("Current tasks in the project:");
        Console.WriteLine("-----------------------------");

        foreach (Task task in tasks.Values)
        {
            Console.WriteLine(task);
        }

        Console.WriteLine("-----------------------------");
    }
}

public class Task
{
    public string TaskId { get; }
    public int TimeNeeded { get; set; }
    public List<string> Dependencies { get; }
    public int? EarliestTime { get; set; }

    public Task(string taskId, int timeNeeded)
    {
        TaskId = taskId;
        TimeNeeded = timeNeeded;
        Dependencies = new List<string>();
        EarliestTime = null;
    }

    public void AddDependency(string dependency)
    {
        Dependencies.Add(dependency);
    }
    public void RemoveDependency(string dependency)
    {
        Dependencies.Remove(dependency);
    }

    public override string ToString()
    {
        string dependencies = string.Join(", ", Dependencies);
        return string.Format("{0}, {1}, {2}", TaskId, TimeNeeded, dependencies);
    }
}

public class Program
{
    private static TaskManager taskManager;

    public static void Main()
    {
        taskManager = new TaskManager();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Project Management System");
            Console.WriteLine("1. Load tasks from file");
            Console.WriteLine("2. Add a new task");
            Console.WriteLine("3. Remove a task");
            Console.WriteLine("4. Update task time");
            Console.WriteLine("5. Save tasks to file");
            Console.WriteLine("6. Generate task sequence");
            Console.WriteLine("7. Generate earliest times");
            Console.WriteLine("8. Print tasks");
            Console.WriteLine("9. Exit");
            Console.WriteLine();

            Console.Write("Enter your choice (1-9): ");
            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter the file path: ");
                    string filePath = Console.ReadLine();
                    taskManager.LoadTasksFromFile(filePath);
                    Console.WriteLine();
                    break;

                case "2":
                    Console.Write("Enter task ID: ");
                    string taskId = Console.ReadLine().ToUpper();
                    Console.Write("Enter time needed: ");
                    int timeNeeded = int.Parse(Console.ReadLine());
                    Console.Write("Enter comma-separated dependencies (if any): ");
                    string dependencyInput = Console.ReadLine().ToUpper();
                    List<string> dependencies = null;

                    if (!string.IsNullOrWhiteSpace(dependencyInput))
                    {
                        dependencies = dependencyInput.Split(',').Select(d => d.Trim()).ToList();
                    }

                    taskManager.AddTask(taskId, timeNeeded, dependencies);
                    Console.WriteLine();
                    break;

                case "3":
                    Console.Write("Enter task ID to remove: ");
                    string taskIdToRemove = Console.ReadLine().ToUpper();
                    taskManager.RemoveTask(taskIdToRemove);
                    Console.WriteLine();
                    break;

                case "4":
                    Console.Write("Enter task ID to update time: ");
                    string taskIdToUpdate = Console.ReadLine().ToUpper();
                    Console.Write("Enter new time needed: ");
                    int newTimeNeeded = int.Parse(Console.ReadLine());
                    taskManager.UpdateTaskTime(taskIdToUpdate, newTimeNeeded);
                    Console.WriteLine();
                    break;

                case "5":
                    Console.Write("Enter the file path: ");
                    string saveFilePath = Console.ReadLine();
                    taskManager.SaveTasksToFile(saveFilePath);
                    Console.WriteLine();
                    break;

                case "6":
                    taskManager.GenerateTaskSequence();
                    Console.WriteLine();
                    break;

                case "7":
                    taskManager.GenerateEarliestTimes();
                    Console.WriteLine();
                    break;

                case "8":
                    taskManager.PrintTasks();
                    Console.WriteLine();
                    break;

                case "9":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    Console.WriteLine();
                    break;
            }
        }
    }
}