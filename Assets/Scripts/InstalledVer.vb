Imports ComputerUtils.CommandLine
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports UnityEngine
Imports UnityEngine.UI

Public Class InstalledVer
    Inherits MonoBehaviour

    Public currentVersion As Text
    Public Shared publicCurrentVersion As Text
    Public Shared DiscordController As New DiscordController()

    Private Sub Start()
        DiscordController.BSVersion = InstalledVersionToggle.BSVersion
        publicCurrentVersion = currentVersion

        UpdateText(True)
    End Sub

    Public Shared Sub UpdateText(Optional first As Boolean = False)
        ' Handle old installs
        If Not Directory.Exists(InstalledVersionToggle.BSBaseDir) Then Directory.CreateDirectory(InstalledVersionToggle.BSBaseDir)
        Dim version As String = ""
        ' Make sure the Beat Saber version gets recognized and then saved to the new location
        If File.Exists("BeatSaberVersion.txt") Then
            version = File.ReadAllText("BeatSaberVersion.txt")
            File.Delete("BeatSaberVersion.txt")
        End If
        If File.Exists("Beat Saber\BeatSaberVersion.txt") Then
            version = File.ReadAllText("Beat Saber\BeatSaberVersion.txt")
            File.Delete("Beat Saber\BeatSaberVersion.txt")
        End If

        ' Write to new location if old found
        If version <> "" Then File.WriteAllText(InstalledVersionToggle.BSBaseDir & "BeatSaberVersion.txt", version)

        ' Move Beat Saber Install to corresponding new folder
        If Directory.Exists("Beat Saber") Then
            InstalledVersionToggle.SetBSVersion(version, True)
            Directory.Move("Beat Saber", InstalledVersionToggle.BSDirectory)
            File.WriteAllText(InstalledVersionToggle.BSDirectory & "BeatSaberVersion.txt", version)
        End If

        ' actually update text
        If File.Exists(InstalledVersionToggle.BSBaseDir & "BeatSaberVersion.txt") Then
            version = File.ReadAllText(InstalledVersionToggle.BSBaseDir & "BeatSaberVersion.txt")
            Debug.Log(version)
            publicCurrentVersion.text = "Currently Selected: " & version
            DiscordController.Installed = $"Currently Selected: {InstalledVersionToggle.BSVersion}"
        Else
            DiscordController.Installed = "No version installed"
        End If

        InstalledVersionToggle.SetBSVersion(version, True)
        InstalledVersionToggle.GetInstalledVersions()
        Dim commands As New CommandLineCommandContainer(Environment.GetCommandLineArgs())
        LaunchOptions.LoadSettings()
        If commands.HasArgument("--version") Then ' ignores case
            Debug.Log(commands.GetValue("--version").Replace("""", ""))
            InstalledVersionToggle.SetBSVersion(commands.GetValue("--version").Replace("""", ""), True) ' Replace shouldn't be needed but always good to have a safety net
            LaunchBS.instace.LaunchBeatSaber()
            Application.Quit()
        End If
        If commands.HasArgument("--launchBS") Then ' ignores case
            LaunchOptions.vars.fpfc = False
            LaunchBS.instace.LaunchBeatSaber()
            Application.Quit()
        End If
        DiscordController.VersionStart()
        UninstallCheck.DoUninstallCheck(first)
    End Sub
End Class
