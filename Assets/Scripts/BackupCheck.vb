Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports UnityEngine
Imports UnityEngine.UI
Imports System.IO

Namespace CheckBackup
    Public Class BackupCheck
        Inherits MonoBehaviour

        Public check As GameObject

        Public Sub Check()
            check.SetActive(False)
            If Not Directory.Exists(InstalledVersionToggle.BSDirectory) Then Return
            If Directory.Exists(InstalledVersionToggle.BaseDirectory & $"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/CustomSabers") Then
                check.SetActive(True)
            End If
            If Directory.Exists(InstalledVersionToggle.BaseDirectory & $"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/UserData") Then
                check.SetActive(True)
            End If
        End Sub

        Private Sub Start()
            Check()
        End Sub
    End Class
End Namespace
