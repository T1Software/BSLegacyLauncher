Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports UnityEngine
Imports UnityEngine.UI

Public Class UninstallCheck
    Inherits MonoBehaviour

    Public LaunchOptions As GameObject
    Public UninstallButton As GameObject
    Public InstalledVerText As GameObject
    Public GameFilesButton As Button
    Public InstallIPAButton As Button
    Public InstallNewIPAButton As Button
    Public SharedFoldersButton As Button
    Public PixelModpackButton As Button
    Public RelinkButton As Button
    Public Shared instance As UninstallCheck
    Public Shared showLaunchOptions As Boolean = False

    Public Sub Start()
        instance = Me
    End Sub

    Public Shared Sub DoUninstallCheck(Optional autoShow As Boolean = False)
        showLaunchOptions = False
        If File.Exists(InstalledVersionToggle.BSDirectory & "Beat Saber.exe") Then
            showLaunchOptions = True
            instance.LaunchOptions.SetActive(False)
            instance.InstalledVerText.SetActive(False)
            instance.InstallIPAButton.interactable = True
            instance.InstallNewIPAButton.interactable = True
            instance.GameFilesButton.interactable = True
            instance.UninstallButton.SetActive(True)
            instance.SharedFoldersButton.interactable = True
            instance.PixelModpackButton.interactable = True
            instance.RelinkButton.interactable = True
        Else
            instance.LaunchOptions.SetActive(False)
            instance.InstalledVerText.SetActive(False)
        End If
        If autoShow Then ShowLaunchOptions()
    End Sub

    Public Shared Sub ShowLaunchOptions()
        instance.LaunchOptions.SetActive(showLaunchOptions)
        instance.InstalledVerText.SetActive(showLaunchOptions)
    End Sub
End Class
