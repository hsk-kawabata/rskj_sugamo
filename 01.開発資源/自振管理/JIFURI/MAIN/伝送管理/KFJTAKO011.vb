Imports CASTCommon
Public Class KFJTAKO011

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '--------------------------------------
        '選択されたリストインデックスを格納
        '--------------------------------------
        gintMAIN0101G_LIST_INDEX = lstSCHLIST.SelectedIndex
        If gintMAIN0101G_LIST_INDEX < 0 Then
            MessageBox.Show(MSG0100W, "伝送ファイル振分(KFJTAKO011)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Me.Close()
        End If
    End Sub
End Class
