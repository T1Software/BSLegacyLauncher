Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports UnityEngine
Imports UnityEngine.UI

Public Class LaunchBS
    Inherits MonoBehaviour

    Public LaunchButton As Button

    <Header("Error Text Objects")>
    Public ErrorSound As AudioSource
    Public ErrorTextObject As GameObject
    Public ErrorText As Text
    Public Shared instace As LaunchBS

    Private Sub Awake()
        instace = GetComponent(Of LaunchBS)()
    End Sub

    Private Sub DisplayErrorText(text As String)
        ' Set to false to restart popup animation DON'T CHANGE
        ErrorTextObject.SetActive(False)
        ErrorTextObject.SetActive(True)
        ErrorText.text = text
        ErrorSound.Play()
    End Sub

    Private Sub Delayfunc(delay As Single, action As Action)
        StartCoroutine(DelayCoroutine(delay, action))
    End Sub

    Private Shared Iterator Function DelayCoroutine(delay As Single, action As Action) As IEnumerator
        Yield New WaitForSeconds(delay)
        action.Invoke()
    End Function

    Public Sub LaunchBeatSaber()
        Dim process As New Process() With {
            .StartInfo = New ProcessStartInfo() With {
                .FileName = InstalledVersionToggle.BSDirectory & "Beat Saber.exe",
                .Arguments = "--no-yeet " & If(LaunchOptions.vars.oculus, "-vrmode oculus ", "") & If(LaunchOptions.vars.verbose, "--verbose ", "") & If(LaunchOptions.vars.fpfc, "fpfc ", ""),
                .UseShellExecute = False,
                .WorkingDirectory = InstalledVersionToggle.BSDirectory
            }
        }

        If Process.GetProcessesByName("steam").Length > 0 Then
            Try
                process.StartInfo.Environment("SteamAppId") = "620980"
                process.Start()

                If LaunchOptions.vars.fpfc Then
                    Try
                        File.Move("C:\Program Files (x86)\Steam\steamapps\common\SteamVR", "C:\Program Files (x86)\Steam\steamapps\common\SteamVR.bak")
                        Delayfunc(3, Sub() File.Move("C:\Program Files (x86)\Steam\steamapps\common\SteamVR.bak", "C:\Program Files (x86)\Steam\steamapps\common\SteamVR"))
                    Catch
                        DisplayErrorText("FAILED TO STOP STEAMVR")
                    End Try
                End If

                If LaunchOptions.vars.verbose Then
                    Throw New Exception()
                End If
            Catch E As Exception
                UnityEngine.Debug.LogError(E.ToString())
                If LaunchOptions.vars.verbose Then
                    LaunchButton.interactable = False
                    Delayfunc(5, Sub() LaunchButton.interactable = True)
                    Throw New Exception("Opening in Debug mode (Launcher remaining open)")
                End If

                If Directory.Exists(InstalledVersionToggle.BSDirectory) Then
                    If Not File.Exists(InstalledVersionToggle.BSDirectory & "Beat Saber.exe") Then
                        DisplayErrorText("BEAT SABER.EXE NOT FOUND")
                    End If
                Else
                    DisplayErrorText("BEAT SABER NOT INSTALLED")
                End If
                Throw New Exception("Beat Saber Not Installed")
            End Try
        Else
            DisplayErrorText("STEAM NOT RUNNING")
            Throw New Exception("Steam Not Running")
        End If
    End Sub

End Class
