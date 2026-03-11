Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine
Imports UnityEngine.UI

Public Class UninstallAdminCheck
    Inherits MonoBehaviour

    Public Prompt As GameObject

    Private Sub Start()
        GetComponent(Of Button)().onClick.AddListener(Sub()
            Prompt.SetActive(True)
            gameObject.SetActive(False)
        End Sub)
    End Sub

End Class
