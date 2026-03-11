Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine

Public Class OpenModassistant
    Inherits MonoBehaviour

    Public Sub OpenModassistantProgram()
        System.Diagnostics.Process.Start(InstalledVersionToggle.BaseDirectory & "Resources/ModAssistant.exe")
    End Sub
End Class
