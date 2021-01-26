Imports System
Imports System.Text.RegularExpressions
Imports System.Windows.Forms

'���� �C�x���g��`�N���X
Public Class Events
    '��ʕ\���F   DiSPlay
    Private DSPCOLOR As System.Drawing.Color = System.Drawing.Color.FromArgb(255, System.Drawing.Color.FromArgb(&HFFFFFF))
    '��ʃt�H�[�J�X�F GoT Focus
    Private GTFCOLOR As System.Drawing.Color = System.Drawing.Color.LightCyan

    Public Enum KeyMode
        NONE = -1           '�w��Ȃ�
        GOOD = 0            '�������w��
        BAD = 1             '�֎~�����w��
    End Enum

    Private msForbid As String      '�֎~����
    Private mnMode As KeyMode       '���[�h �O�F�������w��A�P�F�֎~�����w��

    Public Sub New()
        mnMode = KeyMode.NONE
    End Sub

    Public Sub New(ByVal asForbid As String, ByVal aMode As KeyMode)
        '�֎~�����ݒ�
        msForbid = asForbid
        If msForbid.Length = 0 Then
            mnMode = KeyMode.NONE
        Else
            mnMode = aMode
        End If

        '���֎~������ݒ�
        'msForbid = Regex.Replace(msForbid, "(?<moji>[\.\$\^\{\[\(\|\)\*\+\?\\])", "\${moji}")
        msForbid = Regex.Escape(msForbid)
        msForbid = Regex.Replace(msForbid, "$", "]")
        msForbid = Regex.Replace(msForbid, "^", "[")
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' �R���g���[�����t�H�[�J�X���󂯎�����Ƃ��̏���
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub GotFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        '�w�i�F�̕ύX�A���͕�����̑S�I�����s��
        With sender
            If TypeOf sender Is TextBox Or TypeOf sender Is ComboBox Then
                .BackColor = GTFCOLOR
                .SelectAll()

            End If
        End With
    End Sub

    ''' <summary>
    ''' �R���g���[���Ƀt�H�[�J�X���Ȃ��Ȃ����Ƃ��̏���
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub LostFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        '�w�i�F�̕ύX
        With sender
            If TypeOf sender Is TextBox Or TypeOf sender Is ComboBox Then
                If Not .BackColor.Equals(DSPCOLOR) Then
                    .BackColor = DSPCOLOR
                End If
            End If
        End With
    End Sub

    ''' <summary>
    ''' �R���g���[���Ƀt�H�[�J�X������Ƃ��ŃL�[�������ꂽ���̏���(�ő包���͂ŃJ�[�\���ړ��L��)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Dim sEntryText As String                '���͕�����̕ҏW�p
        Dim KeyAscii As Short = Microsoft.VisualBasic.Asc(e.KeyChar)  '���̓L�[

        Select Case KeyAscii
            Case Keys.Back                                  'BackSpace�L�[
            Case Keys.Enter                                 'Enter�L�[
                '���̃R���g���[���փt�H�[�J�X���ڂ�
                e.Handled = True
                If sender.Parent.SelectNextControl(sender, True, True, True, False) = False Then
                    Try
                        sender.Parent.Parent.SelectNextControl(sender, True, True, True, False)
                    Catch ex As Exception
                    End Try
                End If
            Case Else
                If Not (TypeOf sender Is TextBox Or TypeOf sender Is ComboBox) Then
                    Exit Sub
                End If
                '�����̃I�u�W�F�N�g�����݂��Ȃ��ꍇ�́A�������Ȃ�
                If sender Is Nothing Then
                    Exit Sub
                End If
                If Not TypeOf sender Is TextBox Then
                    Exit Sub
                End If
                '�e�L�X�g�{�b�N�X��ReadOnly�̏ꍇ�͉������Ȃ� '2009/09/08 T-Sakai
                If CType(sender, TextBox).ReadOnly Then
                    Exit Sub
                End If
                '==============================================
                With sender
                    '���͂���������ҏW����
                    sEntryText = .Text
                    sEntryText = sEntryText.Substring(0, .SelectionStart) _
                                & Microsoft.VisualBasic.Chr(KeyAscii) _
                                & sEntryText.Substring(.SelectionStart + .SelectionLength)
                    Select Case System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(sEntryText) 'sEntryText.Length
                        Case Is > CInt(.MaxLength)
                            '�ő���͕������𒴂�����͂𖳌��ɂ���
                            e.Handled = True
                        Case Else
                            If mnMode <> KeyMode.NONE Then
                                If Regex.IsMatch(Microsoft.VisualBasic.ChrW(KeyAscii), msForbid) = (mnMode <> KeyMode.GOOD) Then
                                    Select Case KeyAscii
                                        Case 22, Keys.Cancel    'CTRL+V, CTRL+C
                                        Case Else
                                            e.Handled = True
                                            Exit Sub
                                    End Select
                                End If
                            Else
                                If KeyAscii = Microsoft.VisualBasic.AscW("'") Then
                                    e.Handled = True
                                    Exit Sub
                                End If
                            End If
                    End Select
                    If sEntryText.Length = .MaxLength Then
                        '�ő���͕������ɒB�����ꍇ�́A���̃R���g���[���փt�H�[�J�X���ڂ�
                        e.Handled = True
                        .Text = sEntryText
                        If sender.Parent.SelectNextControl(sender, True, True, True, False) = False Then
                            Try
                                sender.Parent.Parent.SelectNextControl(sender, True, True, True, False)
                            Catch ex As Exception
                            End Try
                        End If
                    End If
                End With
        End Select
    End Sub

    ''' <summary>
    ''' �R���g���[���Ƀt�H�[�J�X������Ƃ��ŃL�[�������ꂽ���̏����������̂ݓ��͉\��(�ő包���͂ŃJ�[�\���ړ��L��)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPressNum(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Dim sEntryText As String                '���͕�����̕ҏW�p
        Dim KeyAscii As Short = Microsoft.VisualBasic.Asc(e.KeyChar)  '���̓L�[

        Select Case KeyAscii
            Case Keys.Back                                  'BackSpace�L�[
            Case Keys.Enter                                 'Enter�L�[
                '���̃R���g���[���փt�H�[�J�X���ڂ�
                e.Handled = True
                If sender.Parent.SelectNextControl(sender, True, True, True, False) = False Then
                    Try
                        sender.Parent.Parent.SelectNextControl(sender, True, True, True, False)
                    Catch ex As Exception
                    End Try
                End If
            Case Else
                '�����̃I�u�W�F�N�g�����݂��Ȃ��ꍇ�́A�������Ȃ�
                If sender Is Nothing Then
                    Exit Sub
                End If
                If Not TypeOf sender Is TextBox Then
                    Exit Sub
                End If
                With sender
                    '���͂���������ҏW����
                    sEntryText = .Text
                    sEntryText = sEntryText.Substring(0, .SelectionStart) _
                                & Microsoft.VisualBasic.ChrW(KeyAscii) _
                                & sEntryText.Substring(.SelectionStart + .SelectionLength)
                    Select Case sEntryText.Length
                        Case Is > CInt(.MaxLength)
                            '�ő���͕������𒴂�����͂𖳌��ɂ���
                            e.Handled = True
                        Case Else
                            If Regex.IsMatch(Microsoft.VisualBasic.ChrW(KeyAscii), "[0-9]") = False Then
                                Select Case KeyAscii
                                    Case 22, Keys.Cancel    'CTRL+V, CTRL+C
                                    Case Else
                                        e.Handled = True
                                        Exit Sub
                                End Select
                            End If
                    End Select
                    If sEntryText.Length = .MaxLength Then
                        '�ő���͕������ɒB�����ꍇ�́A���̃R���g���[���փt�H�[�J�X���ڂ�
                        e.Handled = True
                        .Text = sEntryText
                        If sender.Parent.SelectNextControl(sender, True, True, True, False) = False Then
                            Try
                                sender.Parent.Parent.SelectNextControl(sender, True, True, True, False)
                            Catch ex As Exception
                            End Try
                        End If
                    End If
                End With
        End Select
    End Sub

    ''' <summary>
    ''' �R���g���[�����t�H�[�J�X���󂯎�����Ƃ��̏���
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub GotFocusMoney(ByVal sender As Object, ByVal e As System.EventArgs)
        '�w�i�F�̕ύX�A���͕�����̑S�I�����s��
        With sender
            If TypeOf sender Is TextBox Or TypeOf sender Is ComboBox Then
                .BackColor = GTFCOLOR
                Try
                    .Text = Regex.Replace(.Text, "[,]", "")
                Catch ex As Exception

                End Try
                .SelectAll()

            End If
        End With
    End Sub

    ''' <summary>
    ''' �R���g���[���Ƀt�H�[�J�X���Ȃ��Ȃ����Ƃ��̏���
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub LostFocusMoney(ByVal sender As Object, ByVal e As System.EventArgs)
        '�w�i�F�̕ύX
        With sender
            If TypeOf sender Is TextBox Or TypeOf sender Is ComboBox Then
                If Not .BackColor.Equals(DSPCOLOR) Then
                    Try
                        Dim sValue As String = String.Format("{0:###,###,###,###,##0}", Decimal.Parse(.Text))
                        .Text = sValue
                    Catch ex As Exception

                    End Try
                    .BackColor = DSPCOLOR
                End If
            End If
        End With
    End Sub

    ''' <summary>
    ''' �R���g���[���Ƀt�H�[�J�X������Ƃ��ŃL�[�������ꂽ���̏��������z�p��(�ő包���͂ŃJ�[�\���ړ��L��)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPressMoney(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Call KeyPressNum(sender, e)
    End Sub

    ''' <summary>
    ''' �R���g���[���Ƀt�H�[�J�X���Ȃ��Ȃ����Ƃ��̏���(�[���p�f�B���O)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub LostFocusZero(ByVal sender As Object, ByVal e As System.EventArgs)
        '�w�i�F�̕ύX
        With sender
            If TypeOf sender Is TextBox Or TypeOf sender Is ComboBox Then
                If Not .BackColor.Equals(DSPCOLOR) Then
                    Try
                        If IsDecimal(.text) AndAlso .text.trim <> "" Then
                            Dim sValue As String = Decimal.Parse(.Text.Trim)
                            Console.WriteLine(sValue.PadLeft(.Maxlength(), "0"))
                            .text = sValue.PadLeft(.Maxlength(), "0")
                        End If
                        Console.WriteLine(Decimal.Parse(.Text.Trim))
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                    .BackColor = DSPCOLOR
                End If
            End If
        End With
    End Sub

    ''' <summary>
    ''' �R���g���[���Ƀt�H�[�J�X���Ȃ��Ȃ����Ƃ��̏���(�����ϊ�)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub LostFocusKana(ByVal sender As Object, ByVal e As System.EventArgs)
        '�w�i�F�̕ύX
        With sender
            If TypeOf sender Is TextBox Or TypeOf sender Is ComboBox Then
                If Not .BackColor.Equals(DSPCOLOR) Then
                    Try
                        Dim Kana As String = ""
                        fn_change_kana_SPACE(.text, Kana)
                        .text = Kana
                    Catch ex As Exception

                    End Try
                    .BackColor = DSPCOLOR
                End If
            End If
        End With
    End Sub

    ''' <summary>
    ''' ���������ϊ�
    ''' </summary>
    ''' <param name="strTXT"></param>
    ''' <param name="strRETURN"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function fn_change_kana_SPACE(ByVal strTXT As String, ByRef strRETURN As String) As Boolean
        fn_change_kana_SPACE = False
        Dim i As Integer
        Dim sbRETURN As New System.Text.StringBuilder("", strTXT.Length)

        For i = 0 To strTXT.Length - 1
            Select Case strTXT.Chars(i)
                Case "�"
                    sbRETURN.Append("�")
                Case "�"
                    sbRETURN.Append("�")
                Case "�"
                    sbRETURN.Append("�")
                Case "�"
                    sbRETURN.Append("�")
                Case "�"
                    sbRETURN.Append("�")
                Case "�"
                    sbRETURN.Append("�")
                Case "�"
                    sbRETURN.Append("�")
                Case "�"
                    sbRETURN.Append("�")
                Case "�"
                    sbRETURN.Append("�")
                Case "�"
                    sbRETURN.Append("-")
                    '2017/04/11 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
                    '�w�Z�Łu��v����͂ł���悤�ɏC��
                Case "A" To "Z", "�" To "�", "�", "0" To "9"
                    'Case "A" To "Z", "�" To "�", "0" To "9"
                    '2017/04/11 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END
                    sbRETURN.Append(strTXT.Chars(i))
                Case "�", "�"
                    sbRETURN.Append(strTXT.Chars(i))
                Case "\", ",", ".", "�", "�", "-", "/", "(", ")"
                    sbRETURN.Append(strTXT.Chars(i))
                Case " "
                    sbRETURN.Append(strTXT.Chars(i))
                Case Else
                    sbRETURN.Append(" ")
            End Select
        Next i
        strRETURN = sbRETURN.ToString
        fn_change_kana_SPACE = True
    End Function

    ''' <summary>
    ''' �R���g���[���Ƀt�H�[�J�X���Ȃ��Ȃ����Ƃ��̏���(�S�p�ϊ�)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub LostFocusZenkaku(ByVal sender As Object, ByVal e As System.EventArgs)
        '�w�i�F�̕ύX
        With sender
            If TypeOf sender Is TextBox Or TypeOf sender Is ComboBox Then
                If Not .BackColor.Equals(DSPCOLOR) Then
                    Try
                        .text = Microsoft.VisualBasic.StrConv(.text, Microsoft.VisualBasic.VbStrConv.Wide)
                    Catch ex As Exception

                    End Try
                    .BackColor = DSPCOLOR
                End If
            End If
        End With
    End Sub

    '2017/11/22 �^�X�N�j���� ADD (�W���ŏC��(��178)) -------------------- START
#Region "�ő包�����͂ŃJ�[�\���ړ����Ȃ��C�x���g"
    ''' <summary>
    ''' �R���g���[���Ƀt�H�[�J�X������Ƃ��ŃL�[�������ꂽ���̏���(�ő包���͂ŃJ�[�\���ړ�����)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPress2(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Dim sEntryText As String                '���͕�����̕ҏW�p
        Dim KeyAscii As Short = Microsoft.VisualBasic.Asc(e.KeyChar)  '���̓L�[

        Select Case KeyAscii
            Case Keys.Back                                  'BackSpace�L�[
            Case Keys.Enter                                 'Enter�L�[
                '���̃R���g���[���փt�H�[�J�X���ڂ�
                e.Handled = True
                If sender.Parent.SelectNextControl(sender, True, True, True, False) = False Then
                    Try
                        sender.Parent.Parent.SelectNextControl(sender, True, True, True, False)
                    Catch ex As Exception
                    End Try
                End If
            Case Else
                If Not (TypeOf sender Is TextBox Or TypeOf sender Is ComboBox) Then
                    Exit Sub
                End If
                '�����̃I�u�W�F�N�g�����݂��Ȃ��ꍇ�́A�������Ȃ�
                If sender Is Nothing Then
                    Exit Sub
                End If
                If Not TypeOf sender Is TextBox Then
                    Exit Sub
                End If
                '�e�L�X�g�{�b�N�X��ReadOnly�̏ꍇ�͉������Ȃ� '2009/09/08 T-Sakai
                If CType(sender, TextBox).ReadOnly Then
                    Exit Sub
                End If
                '==============================================
                With sender
                    '���͂���������ҏW����
                    sEntryText = .Text
                    sEntryText = sEntryText.Substring(0, .SelectionStart) _
                                & Microsoft.VisualBasic.Chr(KeyAscii) _
                                & sEntryText.Substring(.SelectionStart + .SelectionLength)
                    Select Case System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(sEntryText) 'sEntryText.Length
                        Case Is > CInt(.MaxLength)
                            '�ő���͕������𒴂�����͂𖳌��ɂ���
                            e.Handled = True
                        Case Else
                            If mnMode <> KeyMode.NONE Then
                                If Regex.IsMatch(Microsoft.VisualBasic.ChrW(KeyAscii), msForbid) = (mnMode <> KeyMode.GOOD) Then
                                    Select Case KeyAscii
                                        Case 22, Keys.Cancel    'CTRL+V, CTRL+C
                                        Case Else
                                            e.Handled = True
                                            Exit Sub
                                    End Select
                                End If
                            Else
                                If KeyAscii = Microsoft.VisualBasic.AscW("'") Then
                                    e.Handled = True
                                    Exit Sub
                                End If
                            End If
                    End Select
                End With
        End Select
    End Sub

    ''' <summary>
    ''' �R���g���[���Ƀt�H�[�J�X������Ƃ��ŃL�[�������ꂽ���̏����������̂ݓ��͉\��(�ő包���͂ŃJ�[�\���ړ�����)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPressNum2(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Dim sEntryText As String                '���͕�����̕ҏW�p
        Dim KeyAscii As Short = Microsoft.VisualBasic.Asc(e.KeyChar)  '���̓L�[

        Select Case KeyAscii
            Case Keys.Back                                  'BackSpace�L�[
            Case Keys.Enter                                 'Enter�L�[
                '���̃R���g���[���փt�H�[�J�X���ڂ�
                e.Handled = True
                If sender.Parent.SelectNextControl(sender, True, True, True, False) = False Then
                    Try
                        sender.Parent.Parent.SelectNextControl(sender, True, True, True, False)
                    Catch ex As Exception
                    End Try
                End If
            Case Else
                '�����̃I�u�W�F�N�g�����݂��Ȃ��ꍇ�́A�������Ȃ�
                If sender Is Nothing Then
                    Exit Sub
                End If
                If Not TypeOf sender Is TextBox Then
                    Exit Sub
                End If
                With sender
                    '���͂���������ҏW����
                    sEntryText = .Text
                    sEntryText = sEntryText.Substring(0, .SelectionStart) _
                                & Microsoft.VisualBasic.ChrW(KeyAscii) _
                                & sEntryText.Substring(.SelectionStart + .SelectionLength)
                    Select Case sEntryText.Length
                        Case Is > CInt(.MaxLength)
                            '�ő���͕������𒴂�����͂𖳌��ɂ���
                            e.Handled = True
                        Case Else
                            If Regex.IsMatch(Microsoft.VisualBasic.ChrW(KeyAscii), "[0-9]") = False Then
                                Select Case KeyAscii
                                    Case 22, Keys.Cancel    'CTRL+V, CTRL+C
                                    Case Else
                                        e.Handled = True
                                        Exit Sub
                                End Select
                            End If
                    End Select
                    If sEntryText.Length = .MaxLength Then
                        '�ő���͕������ɒB�����ꍇ�́A���̃R���g���[���փt�H�[�J�X���ڂ�
                        e.Handled = True
                        .Text = sEntryText
                        If sender.Parent.SelectNextControl(sender, True, True, True, False) = False Then
                            Try
                                sender.Parent.Parent.SelectNextControl(sender, True, True, True, False)
                            Catch ex As Exception
                            End Try
                        End If
                    End If
                End With
        End Select
    End Sub

    ''' <summary>
    ''' �R���g���[���Ƀt�H�[�J�X������Ƃ��ŃL�[�������ꂽ���̏��������z�p��(�ő包���͂ŃJ�[�\���ړ�����)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPressMoney2(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Call KeyPressNum2(sender, e)
    End Sub

#End Region
    '2017/11/22 �^�X�N�j���� ADD (�W���ŏC��(��178)) -------------------- END

End Class
