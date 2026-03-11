Imports UnityEngine
Imports System.Collections
Imports UnityEngine.EventSystems
Imports UnityEngine.UI
Imports TMPro

<RequireComponent(GetType(Button))>
Public Class TextHighlight
    Inherits MonoBehaviour
    Implements IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler

    Public txt As TextMesh
    Public btn As Button
    Public obj As GameObject

    Public normalColor As Color
    Public disabledColor As Color
    Public pressedColor As Color
    Public highlightedColor As Color
    Private lastButtonStatus As ButtonStatus = ButtonStatus.Normal
    Private isHighlightDesired As Boolean = False
    Private isPressedDesired As Boolean = False

    Private Sub Update()
        Dim desiredButtonStatus As ButtonStatus = ButtonStatus.Normal
        If Not btn.interactable Then
            desiredButtonStatus = ButtonStatus.Disabled
        Else
            If isHighlightDesired Then
                desiredButtonStatus = ButtonStatus.Highlighted
            End If
            If isPressedDesired Then
                desiredButtonStatus = ButtonStatus.Pressed
            End If
        End If

        If desiredButtonStatus <> Me.lastButtonStatus Then
            Me.lastButtonStatus = desiredButtonStatus
            Select Case Me.lastButtonStatus
                Case ButtonStatus.Normal
                    txt.color = normalColor
                Case ButtonStatus.Disabled
                    txt.color = disabledColor
                Case ButtonStatus.Pressed
                    txt.color = pressedColor
                Case ButtonStatus.Highlighted
                    txt.color = highlightedColor
            End Select
        End If
    End Sub

    Public Sub OnPointerEnter(eventData As PointerEventData) Implements IPointerEnterHandler.OnPointerEnter
        isHighlightDesired = True
        obj.SetActive(True)
    End Sub

    Public Sub OnPointerDown(eventData As PointerEventData) Implements IPointerDownHandler.OnPointerDown
        isPressedDesired = True
    End Sub

    Public Sub OnPointerUp(eventData As PointerEventData) Implements IPointerUpHandler.OnPointerUp
        isPressedDesired = False
    End Sub

    Public Sub OnPointerExit(eventData As PointerEventData) Implements IPointerExitHandler.OnPointerExit
        isHighlightDesired = False
        obj.SetActive(False)
    End Sub

    Public Enum ButtonStatus
        Normal
        Disabled
        Highlighted
        Pressed
    End Enum
End Class
