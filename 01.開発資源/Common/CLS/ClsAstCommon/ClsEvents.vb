Imports System
Imports System.Text.RegularExpressions
Imports System.Windows.Forms

'共通 イベント定義クラス
Public Class Events
    '画面表示色   DiSPlay
    Private DSPCOLOR As System.Drawing.Color = System.Drawing.Color.FromArgb(255, System.Drawing.Color.FromArgb(&HFFFFFF))
    '画面フォーカス色 GoT Focus
    Private GTFCOLOR As System.Drawing.Color = System.Drawing.Color.LightCyan

    Public Enum KeyMode
        NONE = -1           '指定なし
        GOOD = 0            '許可文字指定
        BAD = 1             '禁止文字指定
    End Enum

    Private msForbid As String      '禁止文字
    Private mnMode As KeyMode       'モード ０：許可文字指定、１：禁止文字指定

    Public Sub New()
        mnMode = KeyMode.NONE
    End Sub

    Public Sub New(ByVal asForbid As String, ByVal aMode As KeyMode)
        '禁止文字設定
        msForbid = asForbid
        If msForbid.Length = 0 Then
            mnMode = KeyMode.NONE
        Else
            mnMode = aMode
        End If

        '許可禁止文字列設定
        'msForbid = Regex.Replace(msForbid, "(?<moji>[\.\$\^\{\[\(\|\)\*\+\?\\])", "\${moji}")
        msForbid = Regex.Escape(msForbid)
        msForbid = Regex.Replace(msForbid, "$", "]")
        msForbid = Regex.Replace(msForbid, "^", "[")
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' コントロールがフォーカスを受け取ったときの処理
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub GotFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        '背景色の変更、入力文字列の全選択を行う
        With sender
            If TypeOf sender Is TextBox Or TypeOf sender Is ComboBox Then
                .BackColor = GTFCOLOR
                .SelectAll()

            End If
        End With
    End Sub

    ''' <summary>
    ''' コントロールにフォーカスがなくなったときの処理
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub LostFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        '背景色の変更
        With sender
            If TypeOf sender Is TextBox Or TypeOf sender Is ComboBox Then
                If Not .BackColor.Equals(DSPCOLOR) Then
                    .BackColor = DSPCOLOR
                End If
            End If
        End With
    End Sub

    ''' <summary>
    ''' コントロールにフォーカスがあるときでキーが押された時の処理(最大桁入力でカーソル移動有り)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Dim sEntryText As String                '入力文字列の編集用
        Dim KeyAscii As Short = Microsoft.VisualBasic.Asc(e.KeyChar)  '入力キー

        Select Case KeyAscii
            Case Keys.Back                                  'BackSpaceキー
            Case Keys.Enter                                 'Enterキー
                '次のコントロールへフォーカスを移す
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
                '引数のオブジェクトが存在しない場合は、何もしない
                If sender Is Nothing Then
                    Exit Sub
                End If
                If Not TypeOf sender Is TextBox Then
                    Exit Sub
                End If
                'テキストボックスがReadOnlyの場合は何もしない '2009/09/08 T-Sakai
                If CType(sender, TextBox).ReadOnly Then
                    Exit Sub
                End If
                '==============================================
                With sender
                    '入力した文字を編集する
                    sEntryText = .Text
                    sEntryText = sEntryText.Substring(0, .SelectionStart) _
                                & Microsoft.VisualBasic.Chr(KeyAscii) _
                                & sEntryText.Substring(.SelectionStart + .SelectionLength)
                    Select Case System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(sEntryText) 'sEntryText.Length
                        Case Is > CInt(.MaxLength)
                            '最大入力文字数を超える入力を無効にする
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
                        '最大入力文字数に達した場合は、次のコントロールへフォーカスを移す
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
    ''' コントロールにフォーカスがあるときでキーが押された時の処理＜数字のみ入力可能＞(最大桁入力でカーソル移動有り)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPressNum(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Dim sEntryText As String                '入力文字列の編集用
        Dim KeyAscii As Short = Microsoft.VisualBasic.Asc(e.KeyChar)  '入力キー

        Select Case KeyAscii
            Case Keys.Back                                  'BackSpaceキー
            Case Keys.Enter                                 'Enterキー
                '次のコントロールへフォーカスを移す
                e.Handled = True
                If sender.Parent.SelectNextControl(sender, True, True, True, False) = False Then
                    Try
                        sender.Parent.Parent.SelectNextControl(sender, True, True, True, False)
                    Catch ex As Exception
                    End Try
                End If
            Case Else
                '引数のオブジェクトが存在しない場合は、何もしない
                If sender Is Nothing Then
                    Exit Sub
                End If
                If Not TypeOf sender Is TextBox Then
                    Exit Sub
                End If
                With sender
                    '入力した文字を編集する
                    sEntryText = .Text
                    sEntryText = sEntryText.Substring(0, .SelectionStart) _
                                & Microsoft.VisualBasic.ChrW(KeyAscii) _
                                & sEntryText.Substring(.SelectionStart + .SelectionLength)
                    Select Case sEntryText.Length
                        Case Is > CInt(.MaxLength)
                            '最大入力文字数を超える入力を無効にする
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
                        '最大入力文字数に達した場合は、次のコントロールへフォーカスを移す
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
    ''' コントロールがフォーカスを受け取ったときの処理
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub GotFocusMoney(ByVal sender As Object, ByVal e As System.EventArgs)
        '背景色の変更、入力文字列の全選択を行う
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
    ''' コントロールにフォーカスがなくなったときの処理
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub LostFocusMoney(ByVal sender As Object, ByVal e As System.EventArgs)
        '背景色の変更
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
    ''' コントロールにフォーカスがあるときでキーが押された時の処理＜金額用＞(最大桁入力でカーソル移動有り)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPressMoney(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Call KeyPressNum(sender, e)
    End Sub

    ''' <summary>
    ''' コントロールにフォーカスがなくなったときの処理(ゼロパディング)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub LostFocusZero(ByVal sender As Object, ByVal e As System.EventArgs)
        '背景色の変更
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
    ''' コントロールにフォーカスがなくなったときの処理(仮名変換)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub LostFocusKana(ByVal sender As Object, ByVal e As System.EventArgs)
        '背景色の変更
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
    ''' 仮名文字変換
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
                Case "ｧ"
                    sbRETURN.Append("ｱ")
                Case "ｨ"
                    sbRETURN.Append("ｲ")
                Case "ｩ"
                    sbRETURN.Append("ｳ")
                Case "ｪ"
                    sbRETURN.Append("ｴ")
                Case "ｫ"
                    sbRETURN.Append("ｵ")
                Case "ｬ"
                    sbRETURN.Append("ﾔ")
                Case "ｭ"
                    sbRETURN.Append("ﾕ")
                Case "ｮ"
                    sbRETURN.Append("ﾖ")
                Case "ｯ"
                    sbRETURN.Append("ﾂ")
                Case "ｰ"
                    sbRETURN.Append("-")
                    '2017/04/11 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
                    '学校で「ｦ」を入力できるように修正
                Case "A" To "Z", "ｱ" To "ﾝ", "ｦ", "0" To "9"
                    'Case "A" To "Z", "ｱ" To "ﾝ", "0" To "9"
                    '2017/04/11 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
                    sbRETURN.Append(strTXT.Chars(i))
                Case "ﾞ", "ﾟ"
                    sbRETURN.Append(strTXT.Chars(i))
                Case "\", ",", ".", "｢", "｣", "-", "/", "(", ")"
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
    ''' コントロールにフォーカスがなくなったときの処理(全角変換)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub LostFocusZenkaku(ByVal sender As Object, ByVal e As System.EventArgs)
        '背景色の変更
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

    '2017/11/22 タスク）西野 ADD (標準版修正(№178)) -------------------- START
#Region "最大桁数入力でカーソル移動しないイベント"
    ''' <summary>
    ''' コントロールにフォーカスがあるときでキーが押された時の処理(最大桁入力でカーソル移動無し)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPress2(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Dim sEntryText As String                '入力文字列の編集用
        Dim KeyAscii As Short = Microsoft.VisualBasic.Asc(e.KeyChar)  '入力キー

        Select Case KeyAscii
            Case Keys.Back                                  'BackSpaceキー
            Case Keys.Enter                                 'Enterキー
                '次のコントロールへフォーカスを移す
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
                '引数のオブジェクトが存在しない場合は、何もしない
                If sender Is Nothing Then
                    Exit Sub
                End If
                If Not TypeOf sender Is TextBox Then
                    Exit Sub
                End If
                'テキストボックスがReadOnlyの場合は何もしない '2009/09/08 T-Sakai
                If CType(sender, TextBox).ReadOnly Then
                    Exit Sub
                End If
                '==============================================
                With sender
                    '入力した文字を編集する
                    sEntryText = .Text
                    sEntryText = sEntryText.Substring(0, .SelectionStart) _
                                & Microsoft.VisualBasic.Chr(KeyAscii) _
                                & sEntryText.Substring(.SelectionStart + .SelectionLength)
                    Select Case System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(sEntryText) 'sEntryText.Length
                        Case Is > CInt(.MaxLength)
                            '最大入力文字数を超える入力を無効にする
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
    ''' コントロールにフォーカスがあるときでキーが押された時の処理＜数字のみ入力可能＞(最大桁入力でカーソル移動無し)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPressNum2(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Dim sEntryText As String                '入力文字列の編集用
        Dim KeyAscii As Short = Microsoft.VisualBasic.Asc(e.KeyChar)  '入力キー

        Select Case KeyAscii
            Case Keys.Back                                  'BackSpaceキー
            Case Keys.Enter                                 'Enterキー
                '次のコントロールへフォーカスを移す
                e.Handled = True
                If sender.Parent.SelectNextControl(sender, True, True, True, False) = False Then
                    Try
                        sender.Parent.Parent.SelectNextControl(sender, True, True, True, False)
                    Catch ex As Exception
                    End Try
                End If
            Case Else
                '引数のオブジェクトが存在しない場合は、何もしない
                If sender Is Nothing Then
                    Exit Sub
                End If
                If Not TypeOf sender Is TextBox Then
                    Exit Sub
                End If
                With sender
                    '入力した文字を編集する
                    sEntryText = .Text
                    sEntryText = sEntryText.Substring(0, .SelectionStart) _
                                & Microsoft.VisualBasic.ChrW(KeyAscii) _
                                & sEntryText.Substring(.SelectionStart + .SelectionLength)
                    Select Case sEntryText.Length
                        Case Is > CInt(.MaxLength)
                            '最大入力文字数を超える入力を無効にする
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
                        '最大入力文字数に達した場合は、次のコントロールへフォーカスを移す
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
    ''' コントロールにフォーカスがあるときでキーが押された時の処理＜金額用＞(最大桁入力でカーソル移動無し)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub KeyPressMoney2(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Call KeyPressNum2(sender, e)
    End Sub

#End Region
    '2017/11/22 タスク）西野 ADD (標準版修正(№178)) -------------------- END

End Class
