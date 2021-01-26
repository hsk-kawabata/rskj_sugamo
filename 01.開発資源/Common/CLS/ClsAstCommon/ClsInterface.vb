Imports System.Collections
Imports System.Windows.Forms

' ソート用インターフェイス
Public Class ListViewItemComparer
    Implements IComparer

    ' ソートオーダーフラグ
    Private SortOrderFlag As Boolean = True
    ' クリックした列の番号
    Private ClickedColumn As Integer
    ' 配置
    Private ColAlignment As HorizontalAlignment = HorizontalAlignment.Left

    Public Sub New()
        ClickedColumn = 0
    End Sub

    ' 処 理     ：インスタンスを作成       
    ' 引 数     ：column    クリック列番号
    '             sort      ソートオーダー
    Public Sub New(ByVal column As Integer, ByVal sort As Boolean, ByVal alignment As HorizontalAlignment)
        ClickedColumn = column
        SortOrderFlag = sort
        ColAlignment = alignment
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
        Implements IComparer.Compare

        ' Compare結果
        Dim retComp As Integer

        '*** 修正 mitsu 2008/09/30 処理高速化 ***
        'Dim Xitem As String = CType(x, ListViewItem).SubItems(ClickedColumn).Text
        'Dim Yitem As String = CType(y, ListViewItem).SubItems(ClickedColumn).Text
        Dim XI As ListViewItem = DirectCast(x, ListViewItem)
        Dim YI As ListViewItem = DirectCast(y, ListViewItem)
        Dim Xitem As String = XI.SubItems(ClickedColumn).Text
        Dim Yitem As String = YI.SubItems(ClickedColumn).Text
        '****************************************

        If ColAlignment = HorizontalAlignment.Right Then
            ' 右寄せで評価する
            '*** 修正 mitsu 2008/09/30 処理高速化 ***
            'Xitem = New String(" "c, 200) & Xitem
            'Xitem = Xitem.Substring(Xitem.Length - 200)
            'Yitem = New String(" "c, 200) & Yitem
            'Yitem = Yitem.Substring(Yitem.Length - 200)
            Xitem = Xitem.PadLeft(200)
            Yitem = Yitem.PadLeft(200)
            '****************************************
        End If

        If SortOrderFlag = True Then
            ' 昇順
            retComp = String.Compare(Xitem, Yitem)
        Else
            ' 降順
            retComp = String.Compare(Yitem, Xitem)
        End If
        '*** 修正 mitsu 2008/09/30 処理高速化 ***
        'If ClickedColumn <> 0 AndAlso retComp = 0 Then
        '    ' 先頭列以外で，値が同じだった場合
        '    Dim subXitem As String = CType(x, ListViewItem).SubItems(0).Text
        '    Dim subYitem As String = CType(y, ListViewItem).SubItems(0).Text
        '    If ColAlignment = HorizontalAlignment.Right Then
        '        ' 右寄せで評価する
        '        subXitem = New String(" "c, 200) & subXitem
        '        subXitem = subXitem.Substring(subXitem.Length - 200)
        '        subYitem = New String(" "c, 200) & subYitem
        '        subYitem = subYitem.Substring(subYitem.Length - 200)
        '    End If
        '    retComp = String.Compare(subXitem, subYitem)
        'End If
        If Not ClickedColumn = 0 AndAlso retComp = 0 Then
            ' 先頭列以外で，値が同じだった場合
            Dim subXitem As String = XI.SubItems(0).Text
            Dim subYitem As String = YI.SubItems(0).Text
            If ColAlignment = HorizontalAlignment.Right Then
                ' 右寄せで評価する
                subXitem = subXitem.PadLeft(200)
                subYitem = subYitem.PadLeft(200)
            End If
            retComp = String.Compare(subXitem, subYitem)
        End If
        '****************************************

        Return retComp
    End Function
End Class
