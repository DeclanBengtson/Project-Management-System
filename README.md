# Project-Management System

This project was developed as part of a university assessment for a course on algorithms and complexity.

## Problem Statement

The task was to create a project management system capable of managing tasks with dependencies. For instance, in a software development project, tasks like design, coding, testing, and deployment have dependencies, such as testing depending on coding completion. Task information and dependencies are stored in a text file, with each task's details on a separate line. For example:

```plaintext
T1, 100
T2, 30, T1
T3, 50, T2, T5
T4, 90, T1, T7
T5, 70, T2, T4
T6, 55, T5
T7, 50
```


This indicates, among other things, that task T1 takes 100 units of time, and T2 cannot start until T1 is completed.

## Project Details

The project management system is implemented as a Microsoft Console Application, featuring a command-line menu with the following options:

- **Load from File**: Read task and dependency information from a user-specified text file.
- **Add Task**: Add a new task, specifying completion time and dependent tasks.
- **Remove Task**: Remove a task from the project.
- **Change Completion Time**: Modify the time needed to complete a task.
- **Save to File**: Save the updated task and dependency information back to the input text file.
- **Find Task Sequence**: Identify a sequence of tasks that satisfies all dependencies and save it to Sequence.txt.
- **Earliest Commencement Times**: Determine the earliest possible commencement time for each task and save the results to EarliestTimes.txt.

## Demo

![Demo.png](/Task3/Docs/Demo.png)
