Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Net

Module Program
    Sub Main()
        Console.WriteLine("Updating BSLegacyLauncher...")
        Using client As New WebClient()
            client.DownloadFile("https://github.com/RiskiVR/BSLegacyLauncher/releases/latest/download/BSLegacyLauncher.zip", "BSLegacyLauncher.zip")
        End Using
        ZipFile.ExtractToDirectory("BSLegacyLauncher.zip", "../", True)
        File.Delete("BSLegacyLauncher.zip")
        Console.WriteLine("Finished Updating")
        Try
            Process.Start("Beat Saber Legacy Launcher.exe")
        Catch
            Console.WriteLine("Please launch the Launcher.")
        End Try
    End Sub
End Module
