Imports Microsoft.VisualBasic.FileIO
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.Drawing


Public Class ClsExtendPrintMenu
    'ログインユーザ用
    Public gstrUSER_ID As String
    Private ownerFrm As System.Windows.Forms.Form
    Private LOG As New CASTCommon.BatchLOG("CAstExtendPrint", "ClsExtendPrintMenu")

    Private Const msgTitle As String = "拡張印刷"
    Private Const ERRLOG_001 As String = "{0}定義エラー：拡張印刷メニューのタブ表示名が定義されていません。"
    Private Const ERRLOG_002 As String = "{0}定義エラー：[{1}]セクションが定義されていません。"
    Private Const ERRLOG_003 As String = "{0}定義エラー：[{1}]セクションの{2}番目のボタン情報に誤りがあります。"
    Private Const ERRLOG_004 As String = "帳票印刷メニュー定義ファイルの帳票IDに指定された'{0}'クラスが見つかりません。"

    '拡張印刷メニュー用業務種別
    Public Const EXPRTMENU_JIFURI As Integer = 1
    Public Const EXPRTMENU_GAKKOU As Integer = 2
    Public Const EXPRTMENU_SOUFURI As Integer = 3
    Public Const EXPRTMENU_KESSAI As Integer = 4
    ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
    Public Const EXPRTMENU_UNYO As Integer = 5
    Public Const EXPRTMENU_WEBDEN As Integer = 6
    Public Const EXPRTMENU_BAITAI As Integer = 7
    Public Const EXPRTMENU_CUST01 As Integer = 8
    Public Const EXPRTMENU_CUST02 As Integer = 9
    ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END
    '2017/01/17 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
    Public Const EXPRTMENU_SSS As Integer = 10
    '2017/01/17 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

    '------------------------------------------------------------------------------
    ' 機能   ： 拡張印刷メニューを作成する
    ' 引数   ： usrID - ログインID
    '           frm   - 親フォームオブジェクト
    '           TAB   - タブコントロールオブジェクト
    '           mode  - 業務種別 1:自振 2:学校 3:総振 4:決済
    ' 戻り値 ： タブコントロールオブジェクト
    '------------------------------------------------------------------------------
    Public Function Read_PrtMenu(ByVal usrID As String, ByVal frm As System.Windows.Forms.Form, _
                                 ByVal TAB As TabControl, ByVal mode As Integer) As TabControl

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter1("ClsExtendPrintMenu.Read_PrtMenu")

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
                    ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
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
                    ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END
                    '2017/01/17 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
                Case 10
                    category = "MENU_SSS.ini"
                    '2017/01/17 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END
                Case Else
                    'NOP
            End Select

            '末尾チェック
            Dim filepath As String = CASTCommon.GetFSKJIni("COMMON", "PRT_FLD")
            If filepath.EndsWith("\") = False Then
                filepath = filepath & "\"
            End If

            '拡張印刷メニュー定義ファイルを読み込む
            Dim filename As String = filepath & category
            If Not System.IO.File.Exists(filename) Then
                Return TAB
            End If

            '===============================================================
            ' 帳票印刷メニュー定義ファイルの解析
            '
            ' 例：MENU_JIFURI.ini
            '------------------------------------------------
            ' [MENUS]
            ' Menu=帳票印刷１                                  ←タブ表示名
            '
            ' [OTHER]
            ' REPLACE=YES                                      ←既存ページの
            '                                                    削除定義
            ' [帳票印刷１]
            ' BUTTON=KFJP001,取引先マスタチェックリスト印刷,0  ←ボタン情報
            '------------------------------------------------
            '
            ' ※ ボタン情報は帳票ID,ボタンタイトル,呼出種別（省略可）
            '    の順に定義する
            ' ※ [OTHER]セクションは省略可（デフォルト:NO）
            ' ※ 拡張印刷メニューページの挿入パターンは以下の通り
            '    1. REPLACE=YES の場合、既存の「帳票印刷」ページを
            '       削除し、その位置に拡張印刷メニューページを表示する
            '    2. REPLACE=NO  の場合、既存の「帳票印刷」ページの
            '       次の位置に拡張印刷メニューページを表示する
            '    3.「帳票印刷」ページが見つからない場合、
            '       メニュータブの末尾に拡張印刷メニューページを追加する
            '===============================================================

            Dim index As Integer = TAB.TabPages.Count
            Dim rmindex As Integer = -1
            Dim rmflag As Boolean = False

            '「帳票印刷」ページ位置を取得する
            For i As Integer = 0 To TAB.TabPages.Count - 1
                If TAB.TabPages(i).Text = "帳票印刷" Then
                    index = i + 1
                    rmindex = i

                    '既存の「帳票印刷」ページを削除するか判定する
                    Dim rp As String = CASTCommon.GetIniFileValue(filename, "OTHER", "REPLACE")
                    If rp = "YES" Then
                        rmflag = True
                    End If

                    Exit For
                End If
            Next

            'タブ表示名の取得
            Dim strSplits As String() = CASTCommon.GetIniFileValues(filename, "MENUS", "MENU")
            If strSplits Is Nothing Then
                LOG.Write_Err("ClsExtendPrintMenu.Read_PrtMenu", String.Format(ERRLOG_001, filename))
                MessageBox.Show(P_MSG0001E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return TAB
            End If

            '定義されているタブ情報の数だけループ
            For Each strValue As String In strSplits

                '新しいタブページを作成する
                Dim tabpage As TabPage = New TabPage(strValue)
                tabpage.BackColor = Color.Transparent
                tabpage.UseVisualStyleBackColor = True

                'ボタン情報の取得
                Dim strSplits2 As String() = CASTCommon.GetIniFileValues(filename, strValue, "BUTTON")
                If strSplits2 Is Nothing Then
                    LOG.Write_Err("ClsExtendPrintMenu.Read_PrtMenu", String.Format(ERRLOG_002, filename, strValue))
                    MessageBox.Show(P_MSG0001E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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
                    '例：btnInf(0)=KFJP001
                    '    btnInf(1)=取引先マスタチェックリスト画面
                    '    btnIng(2)=0
                    Dim btnInf As String() = Split(strValue2, ",")
                    If btnInf.Length < 2 Then
                        LOG.Write_Err("ClsExtendPrintMenu.Read_PrtMenu", String.Format(ERRLOG_003, filename, strValue, i + 1))
                        MessageBox.Show(P_MSG0001E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return TAB
                    End If

                    If btnInf(0).Length = 0 OrElse btnInf(1).Length = 0 Then
                        LOG.Write_Err("ClsExtendPrintMenu.Read_PrtMenu", String.Format(ERRLOG_003, filename, strValue, i + 1))
                        MessageBox.Show(P_MSG0001E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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
                    ' ⇒Tagプロパティ(String型)に帳票IDを設定し、
                    '   文字列の先頭に呼出種別情報を付加する
                    '   (0:拡張印刷画面呼出,1:外部画面呼出)
                    If btnInf.Length < 3 Then
                        Menu_Button.Tag = "0" & btnInf(0).Trim
                    Else
                        If btnInf(2) = "1" Then
                            Menu_Button.Tag = "1" & btnInf(0).Trim
                            '2016/10/18 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- START
                        ElseIf btnInf(2) = "2" Then
                            Menu_Button.Tag = "2" & btnInf(0).Trim
                            '2016/10/18 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- END
                        Else
                            Menu_Button.Tag = "0" & btnInf(0).Trim
                        End If
                    End If
                    tabpage.Controls.Add(Menu_Button)
                    i += 1
                Next

                '作成したタブページを追加する
                TAB.TabPages.Insert(index, tabpage)
                index += 1
            Next

            '既存ページを削除する
            If rmflag = True Then
                TAB.TabPages.RemoveAt(rmindex)
            End If

        Catch ex As Exception
            'エラー処理
            LOG.Write_Err("ClsExtendPrintMenu.Read_PrtMenu", ex)
            MessageBox.Show(P_MSG0002E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            LOG.Write_Exit1(sw, "ClsExtendPrintMenu.Read_PrtMenu")
        End Try

        Return TAB
    End Function

    '------------------------------------------------------------------------------
    ' 機能   ： 拡張印刷画面を表示する
    ' 引数   ： PrtID - 帳票ID
    '           PrtTitleName - 帳票タイトル名
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
    ' 引数   ： PrtID - 帳票ID
    ' 戻り値 ： なし
    '------------------------------------------------------------------------------
    Private Sub Show_Prt_EX(ByVal PrtID As String)

        Dim asm As System.Reflection.Assembly = ownerFrm.GetType.Assembly
        Dim clsname As String = asm.GetName.Name & "." & PrtID

        ' ADD 2016/01/29 SO)山岸 For IT_D-05_001 メニューボタン定義で存在しない帳票IDを外部画面呼出しとした場合のエラーログが不当
        Dim cls As Type = asm.GetType(clsname)
        If cls Is Nothing Then
            LOG.Write_Err("ClsExtendPrintMenu.Show_Prt_EX", String.Format(ERRLOG_004, PrtID))
            MessageBox.Show(P_MSG0009E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim instance As Object = Activator.CreateInstance(cls)
        Dim frm As System.Windows.Forms.Form = CType(instance, System.Windows.Forms.Form)

        '外部画面を表示する
        CASTCommon.ShowFORM(gstrUSER_ID, frm, ownerFrm)

    End Sub

    '2016/10/14 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- START
    '------------------------------------------------------------------------------
    ' 機能   ： 拡張画面を表示する
    ' 引数   ： PrtID - 帳票ID
    '           PrtTitleName - 帳票タイトル名
    ' 戻り値 ： なし
    '------------------------------------------------------------------------------
    Private Sub Show_Prt_External(ByVal PrtID As String, ByVal PrtTitleName As String)

        ''拡張画面クラスを生成する
        Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.LoadFrom("CAstExternal.dll")
        Dim clsname As String = asm.GetName.Name & "." & PrtID

        ' ADD 2016/01/29 SO)山岸 For IT_D-05_001 メニューボタン定義で存在しない帳票IDを外部画面呼出しとした場合のエラーログが不当
        Dim cls As Type = asm.GetType(clsname)
        If cls Is Nothing Then
            LOG.Write_Err("ClsExtendPrintMenu.Show_Prt_External", String.Format(ERRLOG_004, PrtID))
            MessageBox.Show(P_MSG0009E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim instance As Object = Activator.CreateInstance(cls)
        Dim frm As System.Windows.Forms.Form = CType(instance, System.Windows.Forms.Form)

        ''拡張印刷画面を表示する
        CASTCommon.ShowFORM(gstrUSER_ID, frm, ownerFrm)

    End Sub
    '2016/10/14 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- END

    '------------------------------------------------------------------------------
    ' 機能   ： 呼出種別を判定し、拡張印刷画面または外部画面の表示処理を呼び出す
    '           ボタンクリック時に呼び出されるイベントハンドラ 
    '------------------------------------------------------------------------------
    Private Sub MenuButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Try
            Dim btn As Button = CType(sender, Button)
            Dim mode As String = btn.Tag.ToString.Substring(0, 1)
            Dim PrtID As String = btn.Tag.ToString.Substring(1)
            Dim PrtTitleName As String = btn.Text

            '呼出種別を判定する
            If mode = "0" Then
                Show_Prt(PrtID, PrtTitleName)  '拡張印刷画面呼出し
                '2016/10/14 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- START
            ElseIf mode = "2" Then
                Show_Prt_External(PrtID, PrtTitleName)  '拡張画面呼出し
                '2016/10/14 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- END
            Else
                Show_Prt_EX(PrtID)             '外部画面呼出し
            End If

        Catch ex As Exception
            LOG.Write_Err("ClsExtendPrintMenu.MenuButton_Click", ex)
            MessageBox.Show(P_MSG0009E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
End Class
