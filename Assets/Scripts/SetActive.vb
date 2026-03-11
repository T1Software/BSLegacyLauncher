Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine

Public Class SetActive
    Inherits MonoBehaviour

    Public [Object] As GameObject

    Public Sub SetActiveTrue()
        [Object].SetActive(True)
    End Sub

    Public Sub SetActiveFalse()
        [Object].SetActive(False)
    End Sub
End Class
