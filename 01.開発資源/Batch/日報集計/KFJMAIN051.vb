﻿
Public Class KFJMAIN051
    Private noClose As Boolean = False

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        noClose = True
        Me.Close()
    End Sub

    Private Sub KFJMAIN051_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub
End Class