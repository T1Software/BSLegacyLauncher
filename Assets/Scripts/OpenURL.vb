Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine

Public Class OpenURL
    Inherits MonoBehaviour

    Public Sub OpenWebsite()
        Application.OpenURL("https://discord.gg/MrwMx5e")
    End Sub

    Public Sub OpenGithub()
        Application.OpenURL("https://github.com/RiskiVR/BSLegacyLauncher/releases/latest")
    End Sub
End Class
