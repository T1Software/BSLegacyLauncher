Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports UnityEngine
Imports UnityEngine.UI

Public Class SteamCreds
    Inherits MonoBehaviour

    Public User As InputField
    Public Pass As InputField
    Public Toggle As Toggle

    Private Sub Start()
        If File.Exists(InstalledVersionToggle.BaseDirectory & "Beat Saber Legacy Launcher_Data\Saved\steamcreds\password.txt") Then
            File.Delete(InstalledVersionToggle.BaseDirectory & "Beat Saber Legacy Launcher_Data\Saved\steamcreds\password.txt")
        End If

        If Directory.Exists(InstalledVersionToggle.BaseDirectory & "Beat Saber Legacy Launcher_Data\Saved\steamcreds") Then
            Dim SavedUser As String
            SavedUser = File.ReadAllText(InstalledVersionToggle.BaseDirectory & "Beat Saber Legacy Launcher_Data\Saved\steamcreds\username.txt")
            User.text = $"{SavedUser}"

            Toggle.isOn = True
        Else
            Debug.Log("No Saved steamcreds found.")
        End If
    End Sub
End Class
