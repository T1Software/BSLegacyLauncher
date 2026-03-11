Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine

Public Class AutoLaunchManager
    Inherits MonoBehaviour

    Public LaunchBS As LaunchBS

    Public Sub Awake()
        If Environment.CommandLine.Contains("--LaunchBS") Then
            LaunchBS.LaunchBeatSaber()
            Application.Quit()
        End If
    End Sub
End Class
