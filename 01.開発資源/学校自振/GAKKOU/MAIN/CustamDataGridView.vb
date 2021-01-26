Public Class CustomDataGridView
    Inherits DataGridView

    Public Sub New()
        MyBase.New()

        'DataGridViewでセル、行、列が複数選択されないようにする
        Me.MultiSelect = False
        'DataGridViewにユーザーが新しい行を追加できないようにする
        Me.AllowUserToAddRows = False
        ''セルを選択すると行全体が選択されるようにする
        'SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders

        'AddHandler CellPainting, AddressOf CustomDataGridView_CellPainting
        'AddHandler RowPostPaint, AddressOf CustomDataGridView_RowPostPaint
    End Sub


    'Protected Overrides Function ProcessDialogKey(ByVal keyData As Keys) As Boolean

    '    If (keyData And Keys.KeyCode) = Keys.Tab OrElse (keyData And Keys.KeyCode) = Keys.Enter Then

    '        Dim col As Integer = Me.CurrentCell.ColumnIndex + 1
    '        While col < Me.Columns.Count
    '            If Not Me.Columns(col).ReadOnly Then
    '                Exit While
    '            End If
    '            col = col + 1
    '        End While

    '        If col < Me.Columns.Count Then
    '            Me.CurrentCell = Me.Rows(Me.CurrentCell.RowIndex).Cells(col)
    '        Else
    '            If Me.CurrentCell.RowIndex <> Me.Rows.Count - 1 Then

    '                For col = 0 To Me.Columns.Count - 1
    '                    If Not Me.Columns(col).ReadOnly Then
    '                        Exit For
    '                    End If
    '                Next
    '                Me.CurrentCell = Me.Rows(Me.CurrentCell.RowIndex + 1).Cells(col)

    '            End If
    '        End If
    '        Return True

    '    End If

    '    Return MyBase.ProcessDialogKey(keyData)

    'End Function

    'Protected Overrides Function ProcessDataGridViewKey(ByVal e As KeyEventArgs) As Boolean

    '    If e.KeyData = Keys.Tab OrElse e.KeyData = Keys.Enter Then

    '        Dim col As Integer = Me.CurrentCell.ColumnIndex + 1
    '        While col < Me.Columns.Count
    '            If Not Me.Columns(col).ReadOnly Then
    '                Exit While
    '            End If
    '            col = col + 1
    '        End While
    '        If col < Me.Columns.Count Then

    '            Me.CurrentCell = Me.Rows(Me.CurrentCell.RowIndex).Cells(col)
    '        Else

    '            If Me.CurrentCell.RowIndex <> Me.Rows.Count - 1 Then

    '                For col = 0 To Me.Columns.Count - 1

    '                    If Not Me.Columns(col).ReadOnly Then
    '                        Exit For
    '                    End If
    '                Next
    '                Me.CurrentCell = Me.Rows(Me.CurrentCell.RowIndex + 1).Cells(col)

    '            End If
    '        End If

    '        Return True

    '    End If

    '    Return MyBase.ProcessDataGridViewKey(e)

    'End Function

    Protected Overrides Function ProcessDialogKey(ByVal keyData As Keys) As Boolean

        'Enterキーが押された際に、タブキーが押されたことにし、
        'それ以外のキーの場合は、通常どおり実行します。
        If (keyData And Keys.KeyCode) = Keys.Enter Then
            Return Me.ProcessTabKey(keyData)
        Else
            Return MyBase.ProcessDialogKey(keyData)
        End If
    End Function

    Protected Overrides Function ProcessDataGridViewKey(ByVal e As KeyEventArgs) As Boolean

        'Enterキーが押された際に、タブキーが押されたことにし、
        'それ以外のキーの場合は、通常どおり実行します。
        If e.KeyCode = Keys.Enter Then
            Return Me.ProcessTabKey(e.KeyCode)
        Else
            Return MyBase.ProcessDataGridViewKey(e)
        End If
    End Function

    ' DataGridViewのRowPostPaintイベント・ハンドラ
    'Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs)

    '    Dim dgv As DataGridView = CType(sender, DataGridView)

    '    ' 行ヘッダのセル領域を、行番号を描画する長方形とする
    '    ' （ただし右端に4ドットのすき間を空ける）
    '    Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, dgv.Rows(e.RowIndex).Height)

    '    ' 上記の長方形内に行番号を縦方向中央＆右詰で描画する
    '    ' フォントや色は行ヘッダの既定値を使用する
    '    TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, _
    '                          rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
    '                          Or TextFormatFlags.Right)

    'End Sub
End Class
