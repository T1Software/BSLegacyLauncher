Imports System
Imports UnityEngine
Imports System.Runtime.InteropServices

Public Class BorderlessWindow

    Public Shared framed As Boolean = True

    <DllImport("user32.dll")>
    Private Shared Function GetActiveWindow() As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function SetWindowLong(hWnd As IntPtr, nIndex As Integer, dwNewLong As UInteger) As Integer
    End Function

    <DllImport("user32.dll")>
    Private Shared Function ShowWindow(hwnd As IntPtr, nCmdShow As Integer) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function MoveWindow(hWnd As IntPtr, x As Integer, y As Integer, nWidth As Integer, nHeight As Integer, bRepaint As Boolean) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetWindowRect(hwnd As IntPtr, <Out()> ByRef lpRect As WinRect) As Boolean
    End Function

    Private Structure WinRect
        Public left As Integer
        Public top As Integer
        Public right As Integer
        Public bottom As Integer
    End Structure

    Private Const GWL_STYLE As Integer = -16

    Private Const SW_MINIMIZE As Integer = 6
    Private Const SW_MAXIMIZE As Integer = 3
    Private Const SW_RESTORE As Integer = 9

    Private Const WS_VISIBLE As UInteger = &H10000000UI
    Private Const WS_POPUP As UInteger = &H80000000UI
    Private Const WS_BORDER As UInteger = &H800000UI
    Private Const WS_OVERLAPPED As UInteger = &H0UI
    Private Const WS_CAPTION As UInteger = &HC00000UI
    Private Const WS_SYSMENU As UInteger = &H80000UI
    Private Const WS_THICKFRAME As UInteger = &H40000UI ' WS_SIZEBOX
    Private Const WS_MINIMIZEBOX As UInteger = &H20000UI
    Private Const WS_MAXIMIZEBOX As UInteger = &H10000UI
    Private Const WS_OVERLAPPEDWINDOW As UInteger = WS_OVERLAPPED Or WS_CAPTION Or WS_SYSMENU Or WS_THICKFRAME Or WS_MINIMIZEBOX Or WS_MAXIMIZEBOX

    ' This attribute will make the method execute on game launch, after the Unity Logo Splash Screen.
    '<RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)>
    Public Shared Sub InitializeOnLoad()
#If Not UNITY_EDITOR And UNITY_STANDALONE_WIN Then ' Dont do this while on Unity Editor!
        SetFramelessWindow()
#End If
    End Sub

    Public Shared Sub SetFramelessWindow()
        Dim hwnd = GetActiveWindow()
        SetWindowLong(hwnd, GWL_STYLE, WS_POPUP Or WS_VISIBLE)
        framed = False
    End Sub

    Public Shared Sub SetFramedWindow()
        Dim hwnd = GetActiveWindow()
        SetWindowLong(hwnd, GWL_STYLE, WS_OVERLAPPEDWINDOW Or WS_VISIBLE)
        framed = True
    End Sub

    Public Shared Sub MinimizeWindow()
        Dim hwnd = GetActiveWindow()
        ShowWindow(hwnd, SW_MINIMIZE)
    End Sub

    Public Shared Sub MaximizeWindow()
        Dim hwnd = GetActiveWindow()
        ShowWindow(hwnd, SW_MAXIMIZE)
    End Sub

    Public Shared Sub RestoreWindow()
        Dim hwnd = GetActiveWindow()
        ShowWindow(hwnd, SW_RESTORE)
    End Sub

    Public Shared Sub MoveWindowPos(posDelta As Vector2, newWidth As Integer, newHeight As Integer)
        Dim hwnd = GetActiveWindow()

        Dim winRect As WinRect
        Dim windowRect = GetWindowRect(hwnd, winRect)

        Dim x = winRect.left + CInt(posDelta.x)
        Dim y = winRect.top - CInt(posDelta.y)
        MoveWindow(hwnd, x, y, newWidth, newHeight, False)
    End Sub
End Class
