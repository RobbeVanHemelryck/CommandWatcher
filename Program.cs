using System.Diagnostics;

var commandDirectory = args[0];
using var watcher = new FileSystemWatcher(commandDirectory);

watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
watcher.Created += (_, e) => ExecuteCommandFromFile(e.FullPath);
watcher.Filter = "*.*";
watcher.EnableRaisingEvents = true;

Console.ReadLine();

void ExecuteCommandFromFile(string path)
{
    while (!IsFileReady(path))
        Thread.Sleep(100); 
    
    var command = File.ReadAllText(path);
    ExecuteCommand(command);
    
    File.Delete(path);
}

bool IsFileReady(string filename)
{
    try
    {
        using var inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None);
        return inputStream.Length > 0;
    }
    catch (Exception)
    {
        return false;
    }
}

void ExecuteCommand(string command)
{
    try
    {
        var cmd = new Process();
        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.UseShellExecute = false;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.Start();

        cmd.StandardInput.WriteLine(command);
        cmd.StandardInput.Flush();
        cmd.StandardInput.Close();
        cmd.WaitForExit();
        cmd.StandardOutput.ReadToEnd();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
    
}