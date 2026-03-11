Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine
Imports System.IO

Public Class Backup
    Inherits MonoBehaviour

    Public Sub StartBackup()
        ' Begin restore process
        If Directory.Exists($"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/CustomSabers") Then
            Directory.Move($"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/CustomSabers", InstalledVersionToggle.BSDirectory & "CustomSabers")
        End If

        If Directory.Exists($"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/UserData") Then
            Directory.Move($"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/UserData", InstalledVersionToggle.BSDirectory & "UserData")
        End If

        If Directory.Exists($"Backups/Old {InstalledVersionToggle.BSVersion} Plugins") Then
            Directory.Move($"Backups/Old {InstalledVersionToggle.BSVersion} Plugins", InstalledVersionToggle.BSDirectory & $"Old {InstalledVersionToggle.BSVersion} Plugins")
        End If
    End Sub
End Class
