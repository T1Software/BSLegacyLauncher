Imports System.Collections
Imports System.Collections.Generic
Imports UnityEngine
Imports UnityEngine.EventSystems
Imports UnityEngine.UI

Public Class ButtonInfo
    Inherits MonoBehaviour
    Implements IPointerEnterHandler, IPointerExitHandler

    Private isHighlightDesired As Boolean = False

    Public Info As GameObject
    Public InfoText As Text
    Public InfoData As String

    Public Sub OnPointerEnter(eventData As PointerEventData) Implements IPointerEnterHandler.OnPointerEnter
        isHighlightDesired = True
        Info.SetActive(True)
        InfoText.text = $"{InfoData}"
    End Sub

    Public Sub OnPointerExit(eventData As PointerEventData) Implements IPointerExitHandler.OnPointerExit
        isHighlightDesired = False
        Info.SetActive(False)
        InfoText.text = $"{InfoData}"
    End Sub
End Class
