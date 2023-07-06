using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Diagnostics;

public class ReactBuildPreprocessor : IPreprocessBuildWithReport {
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report) {
        // Run "npm run build" in the subdirectory
        string subdirectoryPath = Application.dataPath + "/../react";
        ReactBuildLogWindow.ClearLog();
        RunNpmBuild(subdirectoryPath);
    }

    private static string FindNpmExecutable() {
        string command = Application.platform == RuntimePlatform.WindowsEditor ? "where" : "which";
        string argument = Application.platform == RuntimePlatform.WindowsEditor ? "npm.cmd" : "npm";

        ProcessStartInfo psi = new ProcessStartInfo {
            FileName = command,
            Arguments = argument,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = new Process {
            StartInfo = psi
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return output.Trim();
    }

    private static void RunNpmBuild(string path) {
        string npmPath = FindNpmExecutable();
        if (string.IsNullOrEmpty(npmPath)) {
            ReactBuildLogWindow.AddLine("Could not find the npm executable. Make sure it is installed and in your system's PATH.");
            return;
        }

        ReactBuildLogWindow.AddLine($"npm executable path: {npmPath}");

        ProcessStartInfo psi = new ProcessStartInfo {
            FileName = npmPath,
            Arguments = "run build",
            WorkingDirectory = path,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = new Process {
            StartInfo = psi
        };

        process.OutputDataReceived += (sender, e) => ReactBuildLogWindow.AddLine(e.Data);
        process.ErrorDataReceived += (sender, e) => ReactBuildLogWindow.AddLine(e.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
    }
}