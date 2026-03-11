Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports UnityEngine
Imports UnityEngine.UI

Public Class Uninstall
    Inherits MonoBehaviour

    Public ErrorSound As AudioSource
    Public ErrorTextObject As GameObject
    Public ErrorText As Text
    Public InstalledVer As Text

    Public Sub DisplayErrorText(text As String)
        ErrorText.text = text
        ErrorTextObject.SetActive(False)
        ErrorTextObject.SetActive(True)
        ErrorSound.Play()
    End Sub

    Public Sub UninstallTrigger()
        Try
            AdvancedButtons.locations = SymLinkLocations.LoadFile(AdvancedButtons.jsonLocation)
            For Each folder As String In AdvancedButtons.locations.folders
                ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory & folder)
            Next

            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory & "CustomSongs")
            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory & "Beat Saber_Data" & Path.DirectorySeparatorChar & "CustomLevels")
            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory & "Beat Saber_Data" & Path.DirectorySeparatorChar & "CustomWIPLevels")
            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory & "DLC")

            Directory.Delete(InstalledVersionToggle.BSDirectory, True)

            If File.Exists(InstalledVersionToggle.BSBaseDir & "BeatSaberVersion.txt") Then
                File.Delete(InstalledVersionToggle.BSBaseDir & "BeatSaberVersion.txt")
            End If
        Catch E As Exception
            DisplayErrorText($"{E.GetType().Name.ToUpper()}: {E.Message.ToUpper()}")
            Throw E
        End Try

        UninstallCheck.DoUninstallCheck()
    End Sub

    Public Sub ProcessSymlinkDelete(path As String)
        If Directory.Exists(path) AndAlso New DirectoryInfo(path).Attributes.HasFlag(FileAttributes.ReparsePoint) Then
            Dim i As New ProcessStartInfo() With {
                .FileName = "cmd.exe",
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .UseShellExecute = False,
                .Arguments = $"/c rd /q ""{path}\"""
            }
            Dim p As Process = Process.Start(i)
            p.WaitForExit()
            p.Dispose()
        End If
    End Sub

    Public Sub ClearVerText()
        InstalledVer.text = ""
    End Sub

End Class
