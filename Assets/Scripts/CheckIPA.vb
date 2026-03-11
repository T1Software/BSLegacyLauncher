Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports UnityEngine
Imports UnityEngine.UI

Public Class CheckIPA
    Inherits MonoBehaviour

    Public IPA4Button As GameObject
    Public IPA3Button As GameObject
    Public UninstallIPAButton As GameObject

    Public Sub Start()
        If File.Exists(InstalledVersionToggle.BSDirectory & "Beat Saber.exe") Then
            IPA()
        Else
            UninstallIPAButton.SetActive(False)
        End If
    End Sub

    Public Sub IPAInstalled()
        UninstallIPAButton.SetActive(True)
        IPA3Button.SetActive(False)
        IPA4Button.SetActive(False)
    End Sub

    Public Sub IPANotInstalled()
        UninstallIPAButton.SetActive(False)
        IPA3Button.SetActive(True)
        IPA4Button.SetActive(True)
    End Sub

    Public Sub IPA()
        Dim IPADir As String = $"{InstalledVersionToggle.BSDirectory}IPA"
        Dim IPAexe As String = $"{InstalledVersionToggle.BSDirectory}IPA.exe"

        If Directory.Exists(IPADir) AndAlso File.Exists(IPAexe) Then
            Debug.Log("IPA Exists")
            IPAInstalled()
            Return
        End If
        Debug.Log("IPA Does Not Exist")
        IPANotInstalled()
    End Sub
End Class
