Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGNENJ021

#Region " 共通変数定義 "
    Dim STR帳票ソート順 As String
    Dim INTCNT01 As Integer
    Dim INTCNT02 As Integer
    '追加 2006/03/29
    Public strNENDO As String
    Public intTUUBAN As Integer

    Dim flg As Boolean = False

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGNENJ021", "クラス替え処理画面")
    Private Const msgTitle As String = "クラス替え処理画面(KFGNENJ021)"
    Private MainDB As MyOracle
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGNENJ021_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim strOLD_GAKKOU_NAME As String = "" '2008/03/12　追加
        Dim OraReader As MyOracleReader = Nothing
        Dim OraReader2 As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Try
            'ログ用
            LW.UserID = GCom.GetUserID
            LW.ToriCode = STR_クラス替学校コード
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            MainDB = New MyOracle


            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            labGAKKOU_CODE.Text = STR_クラス替学校コード
            lab学校名.Text = STR_クラス替学校名
            labGAKUNEN.Text = STR_クラス替学年コード
            labGAKUNENMEI.Text = STR_クラス替学年名

            If PFUNC_GAKMAST2_GET() = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "学校マスタ２に未登録")
                Exit Sub
            End If

            '2007/08/27　スプレッド項目表示設定
            DataGridView1.Columns(7).Visible = False
            DataGridView1.Columns(8).Visible = False
            DataGridView1.Columns(9).Visible = False
            DataGridView1.Columns(10).Visible = False
            DataGridView1.Columns(11).Visible = False

            OraReader = New MyOracleReader(MainDB)
            OraReader2 = New MyOracleReader(MainDB)

            '生徒マスタの読込み
            '2007/08/27　学校名取得を追加
            SQL.Append(" SELECT SEITOMAST.*,GAKKOU_NNAME_G,GAKKOU_KNAME_G FROM SEITOMAST,GAKMAST1")
            SQL.Append(" WHERE GAKKOU_CODE_O = GAKKOU_CODE_G")
            SQL.Append(" AND GAKUNEN_CODE_O = GAKUNEN_CODE_G")
            SQL.Append(" AND GAKKOU_CODE_O =" & SQ(STR_クラス替学校コード))
            SQL.Append(" AND GAKUNEN_CODE_O = " & GCom.NzInt(STR_クラス替学年コード))
            '月を４月固定で抽出
            SQL.Append(" AND TUKI_NO_O ='04'")
            '---------------------------------------------------------------------------------------

            Select Case (STR帳票ソート順)
                Case "0"
                    '学年、クラス、生徒番号
                    SQL.Append(" ORDER BY GAKUNEN_CODE_O asc , CLASS_CODE_O asc , SEITO_NO_O ASC")
                Case "1"
                    '入学年度、通番
                    SQL.Append(" ORDER BY NENDO_O asc , TUUBAN_O ASC")
                Case "2"
                    '生徒名のアイウエオ順
                    SQL.Append(" ORDER BY SEITO_KNAME_O ASC")
            End Select

            If OraReader.DataReader(SQL) = False Then
                Exit Sub
            End If

            INTCNT01 = 0

            While OraReader.EOF = False
                Dim RowItem As New DataGridViewRow
                RowItem.CreateCells(DataGridView1)

                '生徒番号の格納
                RowItem.Cells(1).Value = OraReader.GetString("SEITO_NO_O")

                '生徒名の格納
                Select Case OraReader.GetString("SEITO_NNAME_O").Trim
                    Case ""
                        '生徒氏名カナを格納
                        RowItem.Cells(2).Value = OraReader.GetString("SEITO_KNAME_O")
                    Case Else
                        '生徒名漢字の格納
                        RowItem.Cells(2).Value = OraReader.GetString("SEITO_NNAME_O")
                End Select

                '性別の格納
                Select Case OraReader.GetString("SEIBETU_O")
                    Case "0"
                        RowItem.Cells(3).Value = "男"
                    Case "1"
                        RowItem.Cells(3).Value = "女"
                    Case "2"
                        RowItem.Cells(3).Value = "－"
                End Select

                '旧クラスの格納
                RowItem.Cells(4).Value = GCom.NzStr(OraReader.GetInt("CLASS_CODE_O"))
                '新クラスの初期化
                RowItem.Cells(5).Value = ""
                '新生徒番号の初期化
                RowItem.Cells(6).Value = ""
                '入学年度
                RowItem.Cells(10).Value = GCom.NzStr(OraReader.GetString("NENDO_O"))
                '通番
                RowItem.Cells(11).Value = GCom.NzInt(OraReader.GetString("TUUBAN_O"))

                DataGridView1.Rows.Add(RowItem)

                INTCNT01 += 1
                OraReader.NextRead()
            End While

            '入力ボタン制御
            btnUpd.Enabled = True
            btnEnd.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
            If Not MainDB Is Nothing Then MainDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not OraReader2 Is Nothing Then OraReader2.Close()
        End Try

    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnUpd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpd.Click

        '更新ボタン

        Try
            MainDB = New MyOracle

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            LW.ToriCode = STR_クラス替学校コード

            '新クラスチェック
            For INTCNT02 = 0 To INTCNT01 - 1
                '削除チェックボックスがOFF でかつ　新クラスが入力されている場合
                If DataGridView1.Rows(INTCNT02).Cells(0).Value = False And Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value) <> "" Then
                    If PFUNC_CLASS_CHK(Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value)) = False Then
                        Exit Sub
                    End If
                End If
            Next INTCNT02

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons. _
                           YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Return
            End If
            'トランザクション開始
            MainDB.BeginTrans()

            'スプレッドの内容を生徒マスタを更新する
            For INTCNT02 = 0 To INTCNT01 - 1 Step 1

                '削除チェックボックス
                If DataGridView1.Rows(INTCNT02).Cells(0).Value = True Then
                    '削除指示ありの場合
                    If PFUNC_SEITOMAST_DEL() = False Then
                        Exit Sub
                    End If
                Else
                    '削除指示なしの場合
                    If PFUNC_SEITOMAST_UPD() = False Then
                        Exit Sub
                    End If
                End If
            Next INTCNT02

            'トランザクション終了（ＣＯＭＭＩＴ）
            MainDB.Commit()

            Call MessageBox.Show(MSG0006I, msgTitle, _
                                     MessageBoxButtons.OK, MessageBoxIcon.Information)


            '入力ボタン制御
            btnUpd.Enabled = False
            btnEnd.Enabled = True
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKMAST2_GET() As Boolean

        '学校マスタ２の取得

        PFUNC_GAKMAST2_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_T =" & SQ(STR_クラス替学校コード))

            If OraReader.DataReader(SQL) = False Then
                STR帳票ソート順 = "0"
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            Else
                STR帳票ソート順 = OraReader.GetString("MEISAI_OUT_T")
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

        PFUNC_GAKMAST2_GET = True
    End Function
    Private Function PFUNC_SEITOMAST_DEL() As Boolean

        PFUNC_SEITOMAST_DEL = False
        Dim SQL As New StringBuilder
        Dim Ret As Integer
        Try
            '生徒マスタの削除
            SQL.Append(" DELETE  FROM SEITOMAST")
            SQL.Append(" WHERE GAKKOU_CODE_O =" & SQ(STR_クラス替学校コード))
            SQL.Append(" AND GAKUNEN_CODE_O = " & GCom.NzInt(STR_クラス替学年コード))
            '条件変更 2006/03/29
            SQL.Append(" AND NENDO_O = " & SQ(GCom.NzStr(DataGridView1.Rows(INTCNT02).Cells(10).Value)))
            SQL.Append(" AND TUUBAN_O =" & DataGridView1.Rows(INTCNT02).Cells(11).Value)

            Ret = MainDB.ExecuteNonQuery(SQL)
            Select Case Ret
                '削除処理エラー
                Case Is <= 0
                    Throw New Exception("生徒マスタの削除処理に失敗しました。")
                    Exit Function
            End Select
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(生徒削除)", "失敗", ex.ToString)
            Return False
        End Try

        PFUNC_SEITOMAST_DEL = True

    End Function
    Private Function PFUNC_SEITOMAST_UPD() As Boolean

        PFUNC_SEITOMAST_UPD = False
        Dim SQL As New StringBuilder
        Dim Ret As Integer
        Try
            '生徒マスタのクラス、生徒番号の更新
            Select Case (True)
                Case (Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value) = "" And Trim(DataGridView1.Rows(INTCNT02).Cells(6).Value) = "")
                    '入力なし(更新なし)

                    PFUNC_SEITOMAST_UPD = True

                    Exit Function
                Case (Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value) <> "" And Trim(DataGridView1.Rows(INTCNT02).Cells(6).Value) <> "")
                    '新クラス、新生徒番号の入力あり
                    SQL.Append(" UPDATE  SEITOMAST SET ")
                    SQL.Append(" CLASS_CODE_O = " & GCom.NzInt(DataGridView1.Rows(INTCNT02).Cells(5).Value))
                    SQL.Append(",SEITO_NO_O = " & SQ(Format(GCom.NzInt(DataGridView1.Rows(INTCNT02).Cells(6).Value), "0000000")))
                Case (Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value) <> "" And Trim(DataGridView1.Rows(INTCNT02).Cells(6).Value) = "")
                    '新クラスのみ入力あり
                    SQL.Append(" UPDATE  SEITOMAST SET ")
                    SQL.Append(" CLASS_CODE_O = " & GCom.NzInt(DataGridView1.Rows(INTCNT02).Cells(5).Value))
                Case (Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value) = "" And Trim(DataGridView1.Rows(INTCNT02).Cells(6).Value) <> "")
                    '新生徒番号のみ入力あり
                    SQL.Append(" UPDATE  SEITOMAST SET ")
                    SQL.Append(" SEITO_NO_O = " & SQ(Format(GCom.NzInt(DataGridView1.Rows(INTCNT02).Cells(6).Value), "0000000")))
            End Select
            SQL.Append(",KOUSIN_DATE_O =" & SQ(Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd")))
            SQL.Append(" WHERE GAKKOU_CODE_O = " & SQ(STR_クラス替学校コード))
            SQL.Append(" AND GAKUNEN_CODE_O = " & GCom.NzInt(STR_クラス替学年コード))
            '条件変更 2006/03/29
            SQL.Append(" AND NENDO_O = " & SQ(GCom.NzStr(DataGridView1.Rows(INTCNT02).Cells(10).Value)))
            SQL.Append(" AND TUUBAN_O =" & DataGridView1.Rows(INTCNT02).Cells(11).Value)
          

            Ret = MainDB.ExecuteNonQuery(SQL)
            Select Case Ret
                '削除処理エラー
                Case Is <= 0
                    Throw New Exception("生徒マスタの更新処理に失敗しました。")
                    Exit Function
            End Select

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(生徒更新)", "失敗", ex.ToString)
            Return False
        End Try
        PFUNC_SEITOMAST_UPD = True

    End Function
    Private Function PFUNC_CLASS_CHK(ByVal STR新クラス As String) As Boolean

        Dim iCount As Integer

        'クラスチェック
        PFUNC_CLASS_CHK = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM GAKMAST1")
            SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(STR_クラス替学校コード))
            SQL.Append(" AND GAKUNEN_CODE_G = " & GCom.NzInt(STR_クラス替学年コード))

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return False
            End If

            While OraReader.EOF = False
                For iCount = 1 To 20
                    'クラスコード
                    '2006/10/19　新クラスが登録されていた場合のみクラス替え処理を行なう
                    If OraReader.GetInt("CLASS_CODE1" & Format(iCount, "00") & "_G") = GCom.NzInt(STR新クラス) Then
                        PFUNC_CLASS_CHK = True
                        Exit While
                    End If
                Next iCount
                OraReader.NextRead()
            End While
            If PFUNC_CLASS_CHK = False Then
                MessageBox.Show(String.Format(G_MSG0045W, DataGridView1.Rows(INTCNT02).Cells(1).Value), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return False
            End If
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(生徒更新)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

#End Region

    Private colNo, rowNo As Integer
    Private TextEditCtrl As DataGridViewTextBoxEditingControl

    Private Sub CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        colNo = e.ColumnIndex
        rowNo = e.RowIndex

        CType(sender, DataGridView).ImeMode = ImeMode.Disable
    End Sub
    Private Sub CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        With CType(sender, DataGridView)
            Dim str_Value As String = .Rows(e.RowIndex).Cells(e.ColumnIndex).Value
            If Not str_Value Is Nothing Then
                If IsNumeric(str_Value) Then
                    Select Case colNo
                        Case 0 '削除
                        Case 1 '生徒番号
                        Case 2 '生徒氏名
                        Case 3 '性別
                        Case 4 '旧クラス
                        Case 5 '新クラス
                            .Rows(e.RowIndex).Cells(e.ColumnIndex).Value = str_Value.Trim.PadLeft(2, "0"c)
                        Case 6 '新生徒番号
                            .Rows(e.RowIndex).Cells(e.ColumnIndex).Value = str_Value.Trim.PadLeft(7, "0"c)
                        Case 7 '進学前学校名
                        Case 8 '進学前クラス
                        Case 9 '進学前生徒番号
                        Case 10 '入学年度
                        Case 11 '通番
                    End Select
                End If
            End If
        End With

    End Sub
    Private Sub EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs)
        TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)

        Select Case colNo
            Case 0
            Case Else
                AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocus
                AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressNum
        End Select
    End Sub
    Private Sub CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Select Case colNo
            Case 0
            Case Else
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocus
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressNum
        End Select

        Call CellLeave(sender, e)
    End Sub

End Class
