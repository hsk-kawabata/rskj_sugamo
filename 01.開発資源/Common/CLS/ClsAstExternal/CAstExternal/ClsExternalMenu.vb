Imports Microsoft.VisualBasic.FileIO
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.Drawing


Public Class ClsExternalMenu
    'ログインユーザ用
    Public gstrUSER_ID As String
    Private ownerFrm As System.Windows.Forms.Form
    Private LOG As New CASTCommon.BatchLOG("CAstExternal", "ClsExternalMenu")
    Private Const msgTitle As String = "メニュー画面拡張"
    Private Const ERRLOG_001 As String = "{0}定義エラー：画面拡張メニューのタブ表示名が定義されていません。"
    Private Const ERRLOG_002 As String = "{0}定義エラー：[{1}]セクションが定義されていません。"
    Private Const ERRLOG_003 As String = "{0}定義エラー：[{1}]セクションの{2}番目のボタン情報に誤りがあります。"
    Private Const ERRLOG_004 As String = "画面拡張メニュー定義ファイルの画面IDに指定された'{0}'クラスが見つかりません。"

    '拡張印刷メニュー用業務種別
    Public Const ExternalMENU_JIFURI As Integer = 1
    Public Const ExternalMENU_GAKKOU As Integer = 2
    Public Const ExternalMENU_SOUFURI As Integer = 3
    Public Const ExternalMENU_KESSAI As Integer = 4
    Public Const ExternalMENU_UNYO As Integer = 5
    Public Const ExternalMENU_WEBDEN As Integer = 6
    Public Const ExternalMENU_BAITAI As Integer = 7
    Public Const ExternalMENU_CUST01 As Integer = 8
    Public Const ExternalMENU_CUST02 As Integer = 9
    '2017/01/17 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
    Public Const ExternalMENU_SSS As Integer = 10
    '2017/01/17 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

    '------------------------------------------------------------------------------
    ' 機能   ： 拡張処理メニューを作成する
    ' 引数   ： usrID - ログインID
    '           frm   - 親フォームオブジェクト
    '           TAB   - タブコントロールオブジェクト
    '           mode  - 業務種別 1:口座振替
    '                            2:学校諸会費
    '                            3:総合振込
    '                            4:資金決済
    '                            5:運用管理
    '                            6:WEB伝送
    '                            7:媒体変換 
    '                            8:固有カスタマイズ１
    '                            9:固有カスタマイズ２
    '                           10:集金代行　2017/01/17 追加
    ' 戻り値 ： タブコントロールオブジェクト
    '------------------------------------------------------------------------------
    Public Function Read_Menu(ByVal usrID As String, ByVal frm As System.Windows.Forms.Form, _
                              ByVal TAB As TabControl, ByVal mode As Integer) As TabControl

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter1("ClsExternalMenu.Read_Menu")

        Try
            ownerFrm = frm
            gstrUSER_ID = usrID

            '業務種別の判定
            Dim category As String = ""
            Select Case mode
                Case 1
                    category = "MENU_JIFURI.ini"
                Case 2
                    category = "MENU_GAKKOU.ini"
                Case 3
                    category = "MENU_SOUFURI.ini"
                Case 4
                    category = "MENU_KESSAI.ini"
                Case 5
                    category = "MENU_UNYO.ini"
                Case 6
                    category = "MENU_WEBDEN.ini"
                Case 7
                    category = "MENU_BAITAI.ini"
                Case 8
                    category = "MENU_CUST01.ini"
                Case 9
                    category = "MENU_CUST02.ini"
                    '2017/01/17 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
                Case 10
                    category = "MENU_SSS.ini"
                    '2017/01/17 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END
                Case Else
                    'NOP
            End Select

            '末尾チェック
            Dim filepath As String = CASTCommon.GetFSKJIni("COMMON", "DSP_FLD")
            If filepath.EndsWith("\") = False Then
                filepath = filepath & "\"
            End If

            '拡張印刷メニュー定義ファイルを読み込む
            Dim filename As String = filepath & category
            If Not System.IO.File.Exists(filename) Then
                Return TAB
            End If

            '===============================================================
            ' 拡張処理メニュー定義ファイルの解析
            '
            ' 例：MENU_JIFURI.ini
            '------------------------------------------------
            ' [MENUS]
            ' Menu=日常処理　                                  ←タブ表示名
            ' Menu=拡張処理
            '
            ' [日常処理]
            ' BUTTON=KFJMAST010,取引先マスタメンテナンス,0,1   ←ボタン情報(呼出し元画面呼出)
            ' BUTTON=NULL                                      ←ボタン情報(非表示)
            '
            ' [拡張処理]
            ' BUTTON=KFJEXTR010,追加画面処理,1,1               ←ボタン情報(CAstExternal画面呼出)
            ' BUTTON=KFJEXTR010,追加画面処理,2,0               ←ボタン情報(網掛け)
            ' BUTTON=NULL                                      ←ボタン情報(非表示)
            '------------------------------------------------
            '===============================================================

            'タブの最終位置を取得する
            Dim Makeindex As Integer = TAB.TabPages.Count
            Dim Remvindex As Integer = -1
            Dim Remvflag As Boolean = False
            Dim Makeflag As Boolean = False

            'タブ表示名の取得
            Dim strSplits As String() = CASTCommon.GetIniFileValues(filename, "MENUS", "MENU")
            If strSplits Is Nothing Then
                LOG.Write_Err("ClsExternalMenu.Read_Menu", String.Format(ERRLOG_001, filename))
                MessageBox.Show(EXTR_MSG0001E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return TAB
            End If

            ' 2017/06/29 タスク）綾部 CHG (RSV2標準対応 No.10(タブ削除機能追加)) -------------------- START
            For TabIndex As Integer = TAB.TabPages.Count - 1 To 0 Step -1
                Dim Tab_DelFlg As Boolean = True

                For Each strValue As String In strSplits
                    If TAB.TabPages(TabIndex).Text = strValue Then
                        Tab_DelFlg = False
                    End If
                Next

                If Tab_DelFlg = True Then
                    TAB.TabPages.RemoveAt(TabIndex)
                End If
            Next
            ' 2017/06/29 タスク）綾部 CHG (RSV2標準対応 No.10(タブ削除機能追加)) -------------------- END

            '定義されているタブ情報の数だけループ
            For Each strValue As String In strSplits

                '定義したタブ名を検索し位置を取得する
                Dim intCheck As Integer = 0
                For TabIndex As Integer = 0 To TAB.TabPages.Count - 1
                    Select Case TAB.TabPages(TabIndex).Text
                        Case strValue
                            intCheck = 1
                            Makeflag = True
                            Makeindex = TabIndex + 1
                            Remvflag = True
                            Remvindex = TabIndex
                            Exit For
                    End Select
                Next

                If intCheck = 0 Then
                    Makeflag = True
                    Makeindex = TAB.TabPages.Count
                    Remvflag = False
                    Remvindex = -1
                    '2016/11/28 saitou RSV2 DEL メンテナンス ---------------------------------------- START
                    'ここでループを抜けると新しくタブが追加されないため削除
                    'Exit For
                    '2016/11/28 saitou RSV2 DEL ----------------------------------------------------- END
                End If

                If Makeflag = True Then

                    '新しいタブページを作成する
                    Dim tabpage As TabPage = New TabPage(strValue)
                    tabpage.BackColor = Color.Transparent
                    tabpage.UseVisualStyleBackColor = True

                    'ボタン情報の取得
                    Dim strSplits2 As String() = CASTCommon.GetIniFileValues(filename, strValue, "BUTTON")
                    If strSplits2 Is Nothing Then
                        LOG.Write_Err("ClsExternalMenu.Read_Menu", String.Format(ERRLOG_002, filename, strValue))
                        MessageBox.Show(EXTR_MSG0002E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return TAB
                    End If

                    '定義されているボタン情報の数だけループ
                    Dim i As Integer = 0
                    For Each strValue2 As String In strSplits2

                        'ボタンが16個以上になったらループを抜ける（以降のボタン情報は無視する）
                        If i > 15 Then
                            Exit For
                        End If

                        'カンマで区切られているボタン定義情報を分割し、btnInfに格納する
                        '例：btnInf(0)=KFJEXTR010
                        '    btnInf(1)=追加画面処理
                        '    btnIng(2)=0
                        Dim btnInf As String() = Split(strValue2, ",")

                        Select Case btnInf(0).Trim
                            Case "NULL"
                                ' ボタンの場所の調整のため、何も行わず次のボタン箇所へ
                            Case Else
                                If btnInf.Length < 4 Then
                                    LOG.Write_Err("ClsExternalMenu.Read_Menu", String.Format(ERRLOG_003, filename, strValue, i + 1))
                                    MessageBox.Show(EXTR_MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Return TAB
                                End If

                                If btnInf(0).Length = 0 OrElse btnInf(1).Length = 0 Then
                                    LOG.Write_Err("ClsExternalMenu.Read_Menu", String.Format(ERRLOG_003, filename, strValue, i + 1))
                                    MessageBox.Show(EXTR_MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Return TAB
                                End If

                                'ボタンを生成する
                                Dim Menu_Button As Button = New Button()

                                Menu_Button.Name = "MenuButton" & i
                                Menu_Button.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, _
                                                               System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, _
                                                               CType(128, Byte))
                                Menu_Button.UseVisualStyleBackColor = True

                                If i <= 7 Then  'ボタンをメニューの左側に配置
                                    Menu_Button.Location = New Point(55, 20 + 50 * i)
                                Else            'ボタンをメニューの右側に配置
                                    Menu_Button.Location = New Point(395, 20 + 50 * (i - 8))
                                End If
                                Menu_Button.Size = New System.Drawing.Size(300, 40)
                                Menu_Button.Text = btnInf(1).Trim
                                AddHandler Menu_Button.Click, AddressOf MenuButton_Click

                                'ボタンに補足情報を設定する
                                ' ⇒Tagプロパティ(String型)に、画面IDを設定し、
                                '   文字列の先頭に呼出種別情報を付加する
                                '   (0:既存画面呼出,1:追加画面呼出)
                                Menu_Button.Tag = btnInf(2) & btnInf(0).Trim

                                If btnInf(3).Trim = "0" Then
                                    Menu_Button.Enabled = False
                                End If

                                tabpage.Controls.Add(Menu_Button)
                        End Select

                        i += 1

                    Next

                    '作成したタブページを追加する
                    TAB.TabPages.Insert(Makeindex, tabpage)

                End If

                '既存ページを削除する
                If Remvflag = True Then
                    TAB.TabPages.RemoveAt(Remvindex)
                End If

            Next

        Catch ex As Exception
            'エラー処理
            LOG.Write_Err("ClsExternalMenu.Read_Menu", ex)
            MessageBox.Show(EXTR_MSG0004E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            LOG.Write_Exit1(sw, "ClsExternalMenu.Read_Menu")
        End Try

        Return TAB
    End Function

    '------------------------------------------------------------------------------
    ' 機能   ： 呼出種別を判定し、拡張印刷画面または外部画面の表示処理を呼び出す
    '           ボタンクリック時に呼び出されるイベントハンドラ 
    '------------------------------------------------------------------------------
    Private Sub MenuButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Try
            Dim btn As Button = CType(sender, Button)
            Dim TitleID As String = btn.Tag.ToString.Substring(1)
            Dim Mode As String = btn.Tag.ToString.Substring(0, 1)
            Dim TitleName As String = btn.Text

            Select Case Mode
                Case "2"
                    Show_Prt(TitleID, TitleName)
                Case Else
                    Show_Disp(TitleID, Mode)             '画面呼出し
            End Select

        Catch ex As Exception
            LOG.Write_Err("ClsExternalMenu.MenuButton_Click", ex)
            MessageBox.Show(EXTR_MSG0004E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    '------------------------------------------------------------------------------
    ' 機能   ： 拡張印刷画面を表示する
    ' 引数   ： PrtID - 帳票ID
    '           PrtTitleName - 帳票タイトル
    ' 戻り値 ： なし
    '------------------------------------------------------------------------------
    Private Sub Show_Prt(ByVal PrtID As String, ByVal PrtTitleName As String)

        '拡張印刷画面クラスを生成する
        Dim frm As New CAstExtendPrint.FrmExtendPrint(PrtID, PrtTitleName)
        '拡張印刷画面を表示する
        CASTCommon.ShowFORM(gstrUSER_ID, CType(frm, Form), ownerFrm)

    End Sub

    '------------------------------------------------------------------------------
    ' 機能   ： 外部画面を表示する
    ' 引数   ： TitleID - 画面ID
    ' 戻り値 ： なし
    '------------------------------------------------------------------------------
    Private Sub Show_Disp(ByVal TitleID As String, ByVal Mode As String)

        Dim asm As System.Reflection.Assembly = Nothing
        Select Case Mode
            Case "0"      ' 0:既存画面呼出
                asm = ownerFrm.GetType.Assembly
            Case Else     ' 1:追加画面呼出
                asm = Me.GetType.Assembly
        End Select
        Dim clsname As String = asm.GetName.Name & "." & TitleID

        Dim cls As Type = asm.GetType(clsname)
        If cls Is Nothing Then
            LOG.Write_Err("ClsExternalMenu.Show_Disp", String.Format(ERRLOG_004, TitleID))
            MessageBox.Show(EXTR_MSG0004E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim instance As Object = Activator.CreateInstance(cls)
        Dim frm As System.Windows.Forms.Form = CType(instance, System.Windows.Forms.Form)

        '外部画面を表示する
        CASTCommon.ShowFORM(gstrUSER_ID, frm, ownerFrm)

    End Sub

End Class
