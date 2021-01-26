Imports System.Collections
Imports System.Windows.Forms

' �\�[�g�p�C���^�[�t�F�C�X
Public Class ListViewItemComparer
    Implements IComparer

    ' �\�[�g�I�[�_�[�t���O
    Private SortOrderFlag As Boolean = True
    ' �N���b�N������̔ԍ�
    Private ClickedColumn As Integer
    ' �z�u
    Private ColAlignment As HorizontalAlignment = HorizontalAlignment.Left

    Public Sub New()
        ClickedColumn = 0
    End Sub

    ' �� ��     �F�C���X�^���X���쐬       
    ' �� ��     �Fcolumn    �N���b�N��ԍ�
    '             sort      �\�[�g�I�[�_�[
    Public Sub New(ByVal column As Integer, ByVal sort As Boolean, ByVal alignment As HorizontalAlignment)
        ClickedColumn = column
        SortOrderFlag = sort
        ColAlignment = alignment
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
        Implements IComparer.Compare

        ' Compare����
        Dim retComp As Integer

        '*** �C�� mitsu 2008/09/30 ���������� ***
        'Dim Xitem As String = CType(x, ListViewItem).SubItems(ClickedColumn).Text
        'Dim Yitem As String = CType(y, ListViewItem).SubItems(ClickedColumn).Text
        Dim XI As ListViewItem = DirectCast(x, ListViewItem)
        Dim YI As ListViewItem = DirectCast(y, ListViewItem)
        Dim Xitem As String = XI.SubItems(ClickedColumn).Text
        Dim Yitem As String = YI.SubItems(ClickedColumn).Text
        '****************************************

        If ColAlignment = HorizontalAlignment.Right Then
            ' �E�񂹂ŕ]������
            '*** �C�� mitsu 2008/09/30 ���������� ***
            'Xitem = New String(" "c, 200) & Xitem
            'Xitem = Xitem.Substring(Xitem.Length - 200)
            'Yitem = New String(" "c, 200) & Yitem
            'Yitem = Yitem.Substring(Yitem.Length - 200)
            Xitem = Xitem.PadLeft(200)
            Yitem = Yitem.PadLeft(200)
            '****************************************
        End If

        If SortOrderFlag = True Then
            ' ����
            retComp = String.Compare(Xitem, Yitem)
        Else
            ' �~��
            retComp = String.Compare(Yitem, Xitem)
        End If
        '*** �C�� mitsu 2008/09/30 ���������� ***
        'If ClickedColumn <> 0 AndAlso retComp = 0 Then
        '    ' �擪��ȊO�ŁC�l�������������ꍇ
        '    Dim subXitem As String = CType(x, ListViewItem).SubItems(0).Text
        '    Dim subYitem As String = CType(y, ListViewItem).SubItems(0).Text
        '    If ColAlignment = HorizontalAlignment.Right Then
        '        ' �E�񂹂ŕ]������
        '        subXitem = New String(" "c, 200) & subXitem
        '        subXitem = subXitem.Substring(subXitem.Length - 200)
        '        subYitem = New String(" "c, 200) & subYitem
        '        subYitem = subYitem.Substring(subYitem.Length - 200)
        '    End If
        '    retComp = String.Compare(subXitem, subYitem)
        'End If
        If Not ClickedColumn = 0 AndAlso retComp = 0 Then
            ' �擪��ȊO�ŁC�l�������������ꍇ
            Dim subXitem As String = XI.SubItems(0).Text
            Dim subYitem As String = YI.SubItems(0).Text
            If ColAlignment = HorizontalAlignment.Right Then
                ' �E�񂹂ŕ]������
                subXitem = subXitem.PadLeft(200)
                subYitem = subYitem.PadLeft(200)
            End If
            retComp = String.Compare(subXitem, subYitem)
        End If
        '****************************************

        Return retComp
    End Function
End Class
