Imports System
Imports System.IO
Imports CASTCommon
Imports MenteCommon

Public Class KFULOGR010

#Region "宣言"

    Private BatchLog As New CASTCommon.BatchLOG("KFULOGR010", "システムログ参照画面")
    Private Const MsgTitle As String = "システムログ参照画面(KFULOGR010)"

    Private CAST As New CASTCommon.Events
    Private LogToriCode As String = "0000000000-00"
    Private LogFuriDate As String = "00000000"

    Private MyOwnerForm As Form
    Private LogFilePath As String

    'クリックした列の番号
    Dim ClickedColumn As Integer

    'ソートオーダーフラグ
    Dim SortOrderFlag As Boolean = True


#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFULOGR010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            BatchLog.Write(GCom.GetUserID, LogToriCode, LogFuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '処理日にシステム日付を設定
            dtpDate.Value = Date.Now
            dtpDate.CustomFormat = " yyyy 年 MM 月 dd 日 dddd"

            'ログ種別選択コンボボックスを設定
            Call SetCmbLogItems()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(GCom.GetUserID, LogToriCode, LogFuriDate, "(ロード)", "失敗", ex.Message)

        Finally
            BatchLog.Write(GCom.GetUserID, LogToriCode, LogFuriDate, "(ロード)終了", "成功", "")
            btnAction.Focus()
        End Try
    End Sub

    '画面クローズ
    'Private Sub Form1_FormClosing(ByVal sender As Object, _
    '    ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
    '    If e.CloseReason = CloseReason.UserClosing Then
    '        Me.Owner.Show()
    '    End If
    'End Sub

    Private Sub KFULOGR010_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        btnAction.Focus()
    End Sub

#End Region

#Region "ボタン"

    '参照ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            BatchLog.Write(GCom.GetUserID, LogToriCode, LogFuriDate, "(参照)開始", "成功", "")

            '対象ログなし
            If cmbLog.SelectedIndex < 0 Then

                MessageBox.Show(MSG0017W, _
                                MsgTitle, _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Warning)
                Exit Sub
            End If

            Dim ROW As Integer = 0
            Dim LineColor As Color
            Dim LineData As String
            Dim CommonFileName As String

            Me.SuspendLayout()
            Me.ListView1.Items.Clear()

            '--------------------------------------------------
            '対象処理のログファイル一覧を取得
            '--------------------------------------------------
            '[vshost]ファイル対応
            CommonFileName = Replace(cmbLog.SelectedItem.ToString, "VSHOST", ".VSHOST")
            Dim LogFiles() As String = Directory.GetFiles(LogFilePath, CommonFileName & "*.Log")
            Dim logCount As String = "0"

            For Each FL As String In LogFiles

                '巨大ファイルチェック
                If New FileInfo(FL).Length >= 10000000 Then '10MB
                    If MessageBox.Show(MSG0022I, _
                       MsgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                        Exit Sub
                    End If
                End If

                If logCount = "1" Then
                Else
                    'If MessageBox.Show(MSG0024I, _
                    '                   MsgTitle, _
                    '                   MessageBoxButtons.YesNo, _
                    '                   MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                    '    Exit Sub
                    'End If
                    logCount = "1"
                End If

                Dim FR As New StreamReader(FL, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
                LineData = FR.ReadLine

                Do While Not LineData Is Nothing AndAlso Not LineData = ""

                    'ログの項目数異常
                    LineData = LineData.Replace("#ERROR 448#", ",""""") '#ERROR 448#を,""に置換

                    '置換しても項目が足りない場合はダミーを付与
                    While LineData.Split(",").Length < 13
                        LineData &= ","""""
                    End While

                    '--------------------------------------------------
                    'リストヴューへデータセット
                    '--------------------------------------------------
                    Dim Data() As String = Replace(LineData, """", "").Split(",")
                    Dim SetData(Data.Length) As String

                    For Index As Integer = 1 To Data.Length Step 1
                        Select Case Index
                            Case 1
                                '項番
                                SetData(Index) = (ROW + 1).ToString
                            Case 2
                                '時間 
                                SetData(Index) = String.Format("{0:00:00:00}", GCom.NzInt(Data(0), 0))
                            Case 3
                                'プロセスＩＤ
                                SetData(Index) = GCom.NzStr(Data(1))
                            Case 4
                                'ジョブ通番
                                SetData(Index) = GCom.NzStr(Data(2))
                            Case 5
                                '端末名
                                SetData(Index) = GCom.NzStr(Data(3))
                            Case 6
                                'ユーザ名
                                SetData(Index) = GCom.NzStr(Data(4))
                            Case 7
                                '取引先コード
                                SetData(Index) = GCom.NzStr(Data(5))
                            Case 8
                                '振替日
                                If GCom.NzDec(Data(6), 0) = 0 Then
                                    SetData(Index) = "0000/00/00"
                                Else
                                    Dim onDate As Date = GCom.SET_DATE(GCom.NzDec(Data(6), ""))
                                    SetData(Index) = String.Format("{0:yyyy/MM/dd}", onDate)
                                End If
                            Case 9
                                '処理モジュール
                                SetData(Index) = GCom.NzStr(Data(7))
                            Case 10
                                '処理名
                                SetData(Index) = GCom.NzStr(Data(8))
                            Case 11
                                '処理内容
                                SetData(Index) = GCom.NzStr(Data(9))
                            Case 12
                                '結果
                                SetData(Index) = GCom.NzStr(Data(10))
                            Case 13
                                '備考
                                SetData(Index) = GCom.NzStr(Data(11))
                        End Select
                    Next

                    '--------------------------------------------------
                    '背景色設定(縞模様)
                    '--------------------------------------------------
                    If ROW Mod 2 = 0 Then
                        LineColor = Color.White
                    Else
                        LineColor = Color.WhiteSmoke
                    End If

                    '--------------------------------------------------
                    'リストヴューにデータを設定
                    '--------------------------------------------------
                    Dim vLstItem As New ListViewItem(SetData, -1, Color.Black, LineColor, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    ROW += 1
                    LineData = FR.ReadLine
                Loop

                FR.Close()

            Next

            If ListView1.Items.Count > 0 Then
                ListView1.Items(0).Selected = True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(GCom.GetUserID, LogToriCode, LogFuriDate, "(参照)", "失敗", ex.Message)

        Finally
            BatchLog.Write(GCom.GetUserID, LogToriCode, LogFuriDate, "(参照)終了", "成功", "")
            Me.ResumeLayout()
        End Try
    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Me.Close()
    End Sub

#End Region

#Region "データタイムピッカー"

    '処理日変更
    Private Sub dtpDate_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpDate.ValueChanged
        'ファイル名コンボ蓄積
        Call SetCmbLogItems()
    End Sub

#End Region

#Region "関数"

    Private Sub SetCmbLogItems()
        '==============================================================================
        '機能   : ログ種別選択ラベル設定
        '引数   : なし
        '備考   :   
        '作成日 : 2009/10/04
        '更新日 :
        '==============================================================================
        Dim DateString As String
        Dim Ret As String
        Dim NumberString As String

        Try
            ListView1.Items.Clear()
            cmbLog.Items.Clear()

            DateString = String.Format("{0:yyyyMMdd}", dtpDate.Value)
            LogFilePath = GCom.GetLogFolder & DateString

            '--------------------------------------------------
            'フォルダ存在チェック
            '--------------------------------------------------
            If Directory.Exists(GCom.GetLogFolder & DateString) = True Then
                LogFilePath &= "\"
            Else
                cmbLog.Enabled = False
                Exit Sub
            End If

            '--------------------------------------------------
            'フォルダ読込／ファイル一覧セット処理
            '--------------------------------------------------
            Dim LogFiles() As String = Directory.GetFiles(LogFilePath, "*.Log")
            Dim memName As String = ""
            Dim Temp As String
            Dim SyubetuName As Boolean = True 'ログ種別名と通番の区分け

            For Each FL As String In LogFiles
                Temp = System.IO.Path.GetFileNameWithoutExtension(FL)
                Dim CharArray() As Char = Temp.ToString.ToCharArray()
                Ret = ""
                NumberString = ""
                SyubetuName = True

                'ログ種別名を作成(ファイル名から記号・通番を取り除く)
                For Each c As Char In CharArray
                    If Char.IsNumber(c) AndAlso SyubetuName = False Then
                        NumberString &= c.ToString
                    Else
                        Select Case c
                            Case "."c, "-"c, "_"c
                                SyubetuName = False
                            Case Else
                                Ret &= StrConv(c.ToString, VbStrConv.Narrow).ToUpper
                        End Select
                    End If
                Next

                'ログ種別選択に値を設定
                If Not Ret = memName AndAlso Not Ret = "ERRORSQL" AndAlso Ret.Length > 0 Then
                    cmbLog.Items.Add(Ret)
                    memName = Ret
                End If
            Next FL

            cmbLog.Enabled = (cmbLog.Items.Count > 0)
            If cmbLog.Enabled Then
                cmbLog.SelectedIndex = 0
                cmbLog.Focus()
            Else
                dtpDate.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(GCom.GetUserID, LogToriCode, LogFuriDate, "(ファイル名コンボ蓄積)", "失敗", ex.Message)
        End Try
    End Sub

    '一覧表示領域のソート
    Private Sub LetSort_ListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
    Handles ListView1.ColumnClick

        Try

            With CType(sender, ListView)

                If ClickedColumn = e.Column Then
                    ' 同じ列をクリックした場合は，逆順にする 
                    SortOrderFlag = Not SortOrderFlag
                End If

                ' 列番号設定
                ClickedColumn = e.Column

                ' 列水平方向配置
                Dim ColAlignment As HorizontalAlignment = .Columns(e.Column).TextAlign

                ' ソート
                .ListViewItemSorter = New ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)

                ' ソート実行
                .Sort()

            End With

        Catch ex As Exception
            Throw
        End Try

    End Sub
#End Region

End Class

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

        Try

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

        Catch ex As Exception
            Throw
        End Try
    End Function
End Class
