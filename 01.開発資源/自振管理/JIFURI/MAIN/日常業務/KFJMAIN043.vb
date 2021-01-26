Imports System.Data.OracleClient
Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJMAIN043
    Inherits System.Windows.Forms.Form
#Region "宣言"
    Dim strFURI_DATE As String
    Dim intCOUNT As String
    Dim strTORI_CODE(50, 15) As String
    Dim strTORIS_CODE(50, 15) As String
    Dim strTORIF_CODE(50, 15) As String
    Dim strTORI_NAME(50, 15) As String
    Dim strKIGYO_CODE(50, 15) As String
    Dim strFURI_CODE(50, 15) As String
    Dim lngSEIKYU_KEN(50, 15) As Long
    Dim lngSEIKYU_KIN(50, 15) As Long
    Dim lngFURI_KEN(50, 15) As Long
    Dim lngFURI_KIN(50, 15) As Long
    Dim lngFUNOU_KEN(50, 15) As Long
    Dim lngFUNOU_KIN(50, 15) As Long

    Dim intMAXPAGE As Integer
    Dim intMAXLINE As Integer
    Dim intPAGE As Integer

    'スケジュールマスタ更新用
    Public gdbcFUNOU_CONNECT As New OracleClient.OracleConnection() 'CONNECTION
    Public gdbrFUNOU_READER As OracleClient.OracleDataReader 'READER
    Public gdbFUNOU_COMMAND As OracleClient.OracleCommand   'COMMAND関数
    Public gdbFUNOU_TRANS As OracleClient.OracleTransaction 'TRANSACTION

    Dim lblIndex(15) As Label
    Dim lblToriCode(15) As Label
    Dim lblToriName(15) As Label
    Dim lblJifuriCode(15) As Label
    Dim lblKigyoCode(15) As Label
    Dim txtSyoriKen(15) As TextBox
    Dim txtSyoriKin(15) As TextBox
    Dim txtFurikaeKen(15) As TextBox
    Dim txtFurikaeKin(15) As TextBox
    Dim txtFunouKen(15) As TextBox
    Dim txtFunouKin(15) As TextBox

    '共通イベントコントロール
    Private CAST As New CASTCommon.Events
    Public clsFUSION As New clsFUSION.clsMain()
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    '' 許可文字指定
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
#End Region
    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN043", "振替結果入力(センター直接持込分)画面")
    Private Const msgTitle As String = "振替結果入力(センター直接持込分)画面(KFJMAIN043)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private strCSV_FILE_NAME As String
    Private Const ThisModuleName As String = "KFJMAIN043.vb"
#Region " ロード"
    Private Sub KFJMAIN043_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = lblFuriDateY.Text + lblFuriDateM.Text + lblFuriDateD.Text
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Me.btnEnd.DialogResult = DialogResult.None

            'ユーザID／システム日付表示
            Call GCom.SetMonitorTopArea(Label3, Label2, lblUser, lblDate)


            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '対象スケジュールの検索
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            strFURI_DATE = lblFuriDateY.Text & lblFuriDateM.Text & lblFuriDateD.Text

            SQL.Append("SELECT *")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" FSYORI_KBN_T = '1'")
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_S =" & strFURI_DATE)
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND MOTIKOMI_KBN_S = '1'")
            SQL.Append("  ORDER BY FURI_CODE_S ASC,KIGYO_CODE_S ASC,TORIS_CODE_S ASC, TORIF_CODE_S ASC")
            Dim P As Integer, L As Integer
            If OraReader.DataReader(SQL) = True Then
                '読込のみ
                P = 1
                L = 0
                Do
                    L += 1
                    If L = 16 Then
                        P += 1
                        L = 1
                    End If
                    intCOUNT += 1
                    strTORIS_CODE(P, L) = GCom.NzStr(OraReader.GetString("TORIS_CODE_T"))
                    strTORIF_CODE(P, L) = GCom.NzStr(OraReader.GetString("TORIF_CODE_T"))
                    strTORI_CODE(P, L) = strTORIS_CODE(P, L) & "-" & strTORIF_CODE(P, L)
                    strTORI_NAME(P, L) = GCom.NzStr(OraReader.GetString("ITAKU_NNAME_T"))
                    strKIGYO_CODE(P, L) = GCom.NzStr(OraReader.GetString("KIGYO_CODE_T"))
                    strFURI_CODE(P, L) = GCom.NzStr(OraReader.GetString("FURI_CODE_T"))
                    lngSEIKYU_KEN(P, L) = OraReader.GetInt64("SYORI_KEN_S")
                    lngSEIKYU_KIN(P, L) = OraReader.GetInt64("SYORI_KIN_S")
                    lngFURI_KEN(P, L) = OraReader.GetInt64("FURI_KEN_S")
                    lngFURI_KIN(P, L) = OraReader.GetInt64("FURI_KIN_S")
                    lngFUNOU_KEN(P, L) = OraReader.GetInt64("FUNOU_KEN_S")
                    lngFUNOU_KIN(P, L) = OraReader.GetInt64("FUNOU_KIN_S")

                Loop Until OraReader.NextRead = False
                OraReader.Close()
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "マスタ検索失敗")
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
            End If

            'オブジェクトのセット
            fn_OBJECT_SET()

            intMAXPAGE = P
            intMAXLINE = L
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '１画面目表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            P = 1
            Call fn_GAMEN_HYOJI(P)
            intPAGE = 1
            lblPage.Text = Format(intPAGE, "00") & "/" & Format(intMAXPAGE, "00")
            txtSyoriKen1.Focus()

            If intMAXPAGE = 1 Then
                '次画面ボタンの非表示
                btnNextGamen.Enabled = False
            End If
            '前画面ボタンの非表示
            btnPreGamen.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region


#Region " 次画面"
    Private Sub btnNextGamen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNextGamen.Click
        '============================================================================
        'NAME           :btnNextGamen_Click
        'Parameter      :
        'Description    :次画面ボタンを押下時の処理
        'Return         :
        'Create         :2009/09/14
        'Update         :
        '============================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(次画面)開始", "成功", "")

            If intPAGE = intMAXPAGE Then
                MessageBox.Show(MSG0266W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '-----------------------------------
            '画面内容の入力値チェック
            '-----------------------------------
            If fn_check_text(intPAGE) = False Then
                Exit Sub
            End If
            '-----------------------------------
            '画面内容の一時保存
            '-----------------------------------
            fn_GAMEN_HOZON(intPAGE)

            '-----------------------------------
            '画面の初期化
            '-----------------------------------
            fn_GAMEN_CLEAR()

            '-----------------------------------
            '次画面の表示
            '-----------------------------------
            intPAGE += 1
            btnPreGamen.Enabled = True
            If intPAGE = intMAXPAGE Then
                btnNextGamen.Enabled = False
            Else
                btnNextGamen.Enabled = True
            End If

            fn_GAMEN_HYOJI(intPAGE)
            lblPage.Text = Format(intPAGE, "00") & "/" & Format(intMAXPAGE, "00")
            txtSyoriKen1.Focus()

            If intPAGE = intMAXPAGE Then
                '次画面ボタンの非表示
                btnNextGamen.Enabled = False
            End If
            If intPAGE = 1 Then
                '前画面ボタンの非表示
                btnPreGamen.Enabled = False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(次画面)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(次画面)終了", "成功", "")
        End Try
    End Sub

#End Region
#Region " 前画面"
    Private Sub btnPreGamen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPreGamen.Click
        '============================================================================
        'NAME           :btnNextGamen_Click
        'Parameter      :
        'Description    :次画面ボタンを押下時の処理
        'Return         :
        'Create         :2009/09/14
        'Update         :
        '============================================================================
        If intPAGE = 1 Then
            MessageBox.Show(MSG0264W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        '-----------------------------------
        '画面内容の入力値チェック
        '-----------------------------------
        If fn_check_text(intPAGE) = False Then
            Exit Sub
        End If
        '-----------------------------------
        '画面内容の一時保存
        '-----------------------------------
        fn_GAMEN_HOZON(intPAGE)

        '-----------------------------------
        '画面の初期化
        '-----------------------------------
        fn_GAMEN_CLEAR()

        '-----------------------------------
        '前画面の表示
        '-----------------------------------
        intPAGE -= 1

        '次画面ボタンの表示
        btnNextGamen.Enabled = True
        If intPAGE = 1 Then
            btnPreGamen.Enabled = False
        Else
            btnPreGamen.Enabled = True
        End If


        fn_GAMEN_HYOJI(intPAGE)
        lblPage.Text = Format(intPAGE, "00") & "/" & Format(intMAXPAGE, "00")
        txtSyoriKen1.Focus()



    End Sub

#End Region
#Region " 更新"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '============================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :更新ボタンを押下時の処理
        'Return         :
        'Create         :2009/09/25
        'Update         :
        '============================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            Dim MainDB As New CASTCommon.MyOracle
            Dim SQL As New StringBuilder(128)
            '-----------------------------------
            '画面内容の入力値チェック
            '-----------------------------------
            If fn_check_text(intPAGE) = False Then
                Exit Sub
            End If
            '-----------------------------------
            '画面内容の一時保存
            '-----------------------------------
            If fn_GAMEN_HOZON(intPAGE) = False Then
                Exit Sub
            End If

            '-----------------------------------
            '更新確認メッセージの表示
            '-----------------------------------
            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, _
                               MessageBoxIcon.Information) = DialogResult.No Then
                Exit Sub
            End If

            '-----------------------------------
            'スケジュールマスタ（SCHMAST）の更新
            '-----------------------------------
            MainDB.BeginTrans()

            Dim I As Integer, J As Integer, L As Integer
            For I = 1 To intMAXPAGE
                If I = intMAXPAGE Then
                    L = intMAXLINE     '最終ページのみ
                Else
                    L = 15
                End If
                For J = 1 To L
                    SQL = New StringBuilder(128)
                    SQL.Append("UPDATE SCHMAST SET ")
                    SQL.Append(" SYORI_KEN_S =" & lngSEIKYU_KEN(I, J))
                    SQL.Append(",SYORI_KIN_S =" & lngSEIKYU_KIN(I, J))
                    SQL.Append(",FURI_KEN_S =" & lngFURI_KEN(I, J))
                    SQL.Append(",FURI_KIN_S =" & lngFURI_KIN(I, J))
                    SQL.Append(",FUNOU_KEN_S =" & lngFUNOU_KEN(I, J))
                    SQL.Append(",FUNOU_KIN_S =" & lngFUNOU_KIN(I, J))
                    SQL.Append(",FUNOU_FLG_S ='1'")
                    SQL.Append(" WHERE FURI_DATE_S = " & SQ(strFURI_DATE))
                    SQL.Append(" AND TORIS_CODE_S = " & SQ(strTORIS_CODE(I, J)))
                    SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE(I, J)))
                    SQL.Append(" AND MOTIKOMI_KBN_S ='1'")


                    MainDB.ExecuteNonQuery(SQL)

                Next J
            Next I

            MainDB.Commit()
            MessageBox.Show(MSG0006I, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", ex.ToString)
            Exit Sub
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
        End Try
    End Sub

#End Region
#Region " 終了"

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try

    End Sub
#End Region
#Region " 関数"
    Function fn_OBJECT_SET() As Boolean
        '============================================================================
        'NAME           :fn_OBJECT_SET
        'Parameter      :
        'Description    :画面のテキストボックスを配列型オブジェクトにセットする
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/15
        'Update         :
        '============================================================================
        fn_OBJECT_SET = False
        Try
            lblIndex(1) = lblIndex1
            lblIndex(2) = lblIndex2
            lblIndex(3) = lblIndex3
            lblIndex(4) = lblIndex4
            lblIndex(5) = lblIndex5
            lblIndex(6) = lblIndex6
            lblIndex(7) = lblIndex7
            lblIndex(8) = lblIndex8
            lblIndex(9) = lblIndex9
            lblIndex(10) = lblIndex10
            lblIndex(11) = lblIndex11
            lblIndex(12) = lblIndex12
            lblIndex(13) = lblIndex13
            lblIndex(14) = lblIndex14
            lblIndex(15) = lblIndex15

            lblToriCode(1) = lblToriCode1
            lblToriCode(2) = lblToriCode2
            lblToriCode(3) = lblToriCode3
            lblToriCode(4) = lblToriCode4
            lblToriCode(5) = lblToriCode5
            lblToriCode(6) = lblToriCode6
            lblToriCode(7) = lblToriCode7
            lblToriCode(8) = lblToriCode8
            lblToriCode(9) = lblToriCode9
            lblToriCode(10) = lblToriCode10
            lblToriCode(11) = lblToriCode11
            lblToriCode(12) = lblToriCode12
            lblToriCode(13) = lblToriCode13
            lblToriCode(14) = lblToriCode14
            lblToriCode(15) = lblToriCode15

            lblToriName(1) = lblToriName1
            lblToriName(2) = lblToriName2
            lblToriName(3) = lblToriName3
            lblToriName(4) = lblToriName4
            lblToriName(5) = lblToriName5
            lblToriName(6) = lblToriName6
            lblToriName(7) = lblToriName7
            lblToriName(8) = lblToriName8
            lblToriName(9) = lblToriName9
            lblToriName(10) = lblToriName10
            lblToriName(11) = lblToriName11
            lblToriName(12) = lblToriName12
            lblToriName(13) = lblToriName13
            lblToriName(14) = lblToriName14
            lblToriName(15) = lblToriName15

            lblJifuriCode(1) = lblJifuriCode1
            lblJifuriCode(2) = lblJifuriCode2
            lblJifuriCode(3) = lblJifuriCode3
            lblJifuriCode(4) = lblJifuriCode4
            lblJifuriCode(5) = lblJifuriCode5
            lblJifuriCode(6) = lblJifuriCode6
            lblJifuriCode(7) = lblJifuriCode7
            lblJifuriCode(8) = lblJifuriCode8
            lblJifuriCode(9) = lblJifuriCode9
            lblJifuriCode(10) = lblJifuriCode10
            lblJifuriCode(11) = lblJifuriCode11
            lblJifuriCode(12) = lblJifuriCode12
            lblJifuriCode(13) = lblJifuriCode13
            lblJifuriCode(14) = lblJifuriCode14
            lblJifuriCode(15) = lblJifuriCode15

            lblKigyoCode(1) = lblKigyoCode1
            lblKigyoCode(2) = lblKigyoCode2
            lblKigyoCode(3) = lblKigyoCode3
            lblKigyoCode(4) = lblKigyoCode4
            lblKigyoCode(5) = lblKigyoCode5
            lblKigyoCode(6) = lblKigyoCode6
            lblKigyoCode(7) = lblKigyoCode7
            lblKigyoCode(8) = lblKigyoCode8
            lblKigyoCode(9) = lblKigyoCode9
            lblKigyoCode(10) = lblKigyoCode10
            lblKigyoCode(11) = lblKigyoCode11
            lblKigyoCode(12) = lblKigyoCode12
            lblKigyoCode(13) = lblKigyoCode13
            lblKigyoCode(14) = lblKigyoCode14
            lblKigyoCode(15) = lblKigyoCode15

            txtSyoriKen(1) = txtSyoriKen1
            txtSyoriKen(2) = txtSyoriKen2
            txtSyoriKen(3) = txtSyoriKen3
            txtSyoriKen(4) = txtSyoriKen4
            txtSyoriKen(5) = txtSyoriKen5
            txtSyoriKen(6) = txtSyoriKen6
            txtSyoriKen(7) = txtSyoriKen7
            txtSyoriKen(8) = txtSyoriKen8
            txtSyoriKen(9) = txtSyoriKen9
            txtSyoriKen(10) = txtSyoriKen10
            txtSyoriKen(11) = txtSyoriKen11
            txtSyoriKen(12) = txtSyoriKen12
            txtSyoriKen(13) = txtSyoriKen13
            txtSyoriKen(14) = txtSyoriKen14
            txtSyoriKen(15) = txtSyoriKen15

            txtSyoriKin(1) = txtSyoriKin1
            txtSyoriKin(2) = txtSyoriKin2
            txtSyoriKin(3) = txtSyoriKin3
            txtSyoriKin(4) = txtSyoriKin4
            txtSyoriKin(5) = txtSyoriKin5
            txtSyoriKin(6) = txtSyoriKin6
            txtSyoriKin(7) = txtSyoriKin7
            txtSyoriKin(8) = txtSyoriKin8
            txtSyoriKin(9) = txtSyoriKin9
            txtSyoriKin(10) = txtSyoriKin10
            txtSyoriKin(11) = txtSyoriKin11
            txtSyoriKin(12) = txtSyoriKin12
            txtSyoriKin(13) = txtSyoriKin13
            txtSyoriKin(14) = txtSyoriKin14
            txtSyoriKin(15) = txtSyoriKin15

            txtFurikaeKen(1) = txtFurikaeKen1
            txtFurikaeKen(2) = txtFurikaeKen2
            txtFurikaeKen(3) = txtFurikaeKen3
            txtFurikaeKen(4) = txtFurikaeKen4
            txtFurikaeKen(5) = txtFurikaeKen5
            txtFurikaeKen(6) = txtFurikaeKen6
            txtFurikaeKen(7) = txtFurikaeKen7
            txtFurikaeKen(8) = txtFurikaeKen8
            txtFurikaeKen(9) = txtFurikaeKen9
            txtFurikaeKen(10) = txtFurikaeKen10
            txtFurikaeKen(11) = txtFurikaeKen11
            txtFurikaeKen(12) = txtFurikaeKen12
            txtFurikaeKen(13) = txtFurikaeKen13
            txtFurikaeKen(14) = txtFurikaeKen14
            txtFurikaeKen(15) = txtFurikaeKen15

            txtFurikaeKin(1) = txtFurikaeKin1
            txtFurikaeKin(2) = txtFurikaeKin2
            txtFurikaeKin(3) = txtFurikaeKin3
            txtFurikaeKin(4) = txtFurikaeKin4
            txtFurikaeKin(5) = txtFurikaeKin5
            txtFurikaeKin(6) = txtFurikaeKin6
            txtFurikaeKin(7) = txtFurikaeKin7
            txtFurikaeKin(8) = txtFurikaeKin8
            txtFurikaeKin(9) = txtFurikaeKin9
            txtFurikaeKin(10) = txtFurikaeKin10
            txtFurikaeKin(11) = txtFurikaeKin11
            txtFurikaeKin(12) = txtFurikaeKin12
            txtFurikaeKin(13) = txtFurikaeKin13
            txtFurikaeKin(14) = txtFurikaeKin14
            txtFurikaeKin(15) = txtFurikaeKin15

            txtFunouKen(1) = txtFunouKen1
            txtFunouKen(2) = txtFunouKen2
            txtFunouKen(3) = txtFunouKen3
            txtFunouKen(4) = txtFunouKen4
            txtFunouKen(5) = txtFunouKen5
            txtFunouKen(6) = txtFunouKen6
            txtFunouKen(7) = txtFunouKen7
            txtFunouKen(8) = txtFunouKen8
            txtFunouKen(9) = txtFunouKen9
            txtFunouKen(10) = txtFunouKen10
            txtFunouKen(11) = txtFunouKen11
            txtFunouKen(12) = txtFunouKen12
            txtFunouKen(13) = txtFunouKen13
            txtFunouKen(14) = txtFunouKen14
            txtFunouKen(15) = txtFunouKen15

            txtFunouKin(1) = txtFunouKin1
            txtFunouKin(2) = txtFunouKin2
            txtFunouKin(3) = txtFunouKin3
            txtFunouKin(4) = txtFunouKin4
            txtFunouKin(5) = txtFunouKin5
            txtFunouKin(6) = txtFunouKin6
            txtFunouKin(7) = txtFunouKin7
            txtFunouKin(8) = txtFunouKin8
            txtFunouKin(9) = txtFunouKin9
            txtFunouKin(10) = txtFunouKin10
            txtFunouKin(11) = txtFunouKin11
            txtFunouKin(12) = txtFunouKin12
            txtFunouKin(13) = txtFunouKin13
            txtFunouKin(14) = txtFunouKin14
            txtFunouKin(15) = txtFunouKin15
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(オブジェクトセット)", "失敗", ex.ToString)
            Return False
        End Try


        fn_OBJECT_SET = True
    End Function
    Function fn_GAMEN_CLEAR() As Boolean
        '============================================================================
        'NAME           :fn_GAMEN_CLEAR
        'Parameter      :
        'Description    :画面を初期化する
        'Return         :
        'Create         :2009/09/15
        'Update         :
        '============================================================================
        fn_GAMEN_CLEAR = False
        Try
            Dim L As Integer
            For L = 1 To 15
                lblIndex(L).Text = ""
                lblToriCode(L).Text = ""
                lblToriName(L).Text = ""
                lblJifuriCode(L).Text = ""
                lblKigyoCode(L).Text = ""
                txtSyoriKen(L).Text = ""
                txtSyoriKin(L).Text = ""
                txtFurikaeKen(L).Text = ""
                txtFurikaeKin(L).Text = ""
                txtFunouKen(L).Text = ""
                txtFunouKin(L).Text = ""
            Next L
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(画面初期化)", "失敗", ex.ToString)
            Return False
        End Try
        fn_GAMEN_CLEAR = True
    End Function
    Private Function fn_GAMEN_HYOJI(ByVal aintPAGE As Integer) As Boolean
        '============================================================================
        'NAME           :fn_GAMEN_HYOJI
        'Parameter      :aintPAGE:ページ番号
        'Description    :aintPAGEページの画面内容をセットする
        'Return         :
        'Create         :2009/09/15
        'Update         :
        '============================================================================
        fn_GAMEN_HYOJI = False
        Try
            Dim L As Integer
            For L = 1 To 15
                If aintPAGE = intMAXPAGE And L = intMAXLINE + 1 Then
                    Exit For
                End If
                lblIndex(L).Text = ((aintPAGE - 1) * 15 + L).ToString
                lblToriCode(L).Text = strTORI_CODE(aintPAGE, L).Trim
                lblToriName(L).Text = strTORI_NAME(aintPAGE, L).Trim
                lblJifuriCode(L).Text = strFURI_CODE(aintPAGE, L).Trim
                lblKigyoCode(L).Text = strKIGYO_CODE(aintPAGE, L).Trim
                txtSyoriKen(L).Text = lngSEIKYU_KEN(aintPAGE, L).ToString("#,##0")
                txtSyoriKin(L).Text = lngSEIKYU_KIN(aintPAGE, L).ToString("#,##0")
                txtFurikaeKen(L).Text = lngFURI_KEN(aintPAGE, L).ToString("#,##0")
                txtFurikaeKin(L).Text = lngFURI_KIN(aintPAGE, L).ToString("#,##0")
                txtFunouKen(L).Text = lngFUNOU_KEN(aintPAGE, L).ToString("#,##0")
                txtFunouKin(L).Text = lngFUNOU_KIN(aintPAGE, L).ToString("#,##0")

            Next
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ページセット)", "失敗", ex.ToString)
            Return False
        End Try
        fn_GAMEN_HYOJI = True
    End Function
    Private Function fn_GAMEN_HOZON(ByVal aintPAGE As Integer) As Boolean
        '============================================================================
        'NAME           :fn_GAMEN_HOZON
        'Parameter      :aintPAGE:ページ番号
        'Description    :aintPAGEページの画面内容を変数に格納する
        'Return         :
        'Create         :2009/09/15
        'Update         :
        '============================================================================
        fn_GAMEN_HOZON = False
        Try
            Dim L As Integer
            For L = 1 To 15
                If aintPAGE = intMAXPAGE And L = intMAXLINE + 1 Then
                    Exit For
                End If
                lngSEIKYU_KEN(aintPAGE, L) = fn_DEL_KANMA(txtSyoriKen(L).Text)
                lngSEIKYU_KIN(aintPAGE, L) = fn_DEL_KANMA(txtSyoriKin(L).Text)
                lngFURI_KEN(aintPAGE, L) = fn_DEL_KANMA(txtFurikaeKen(L).Text)
                lngFURI_KIN(aintPAGE, L) = fn_DEL_KANMA(txtFurikaeKin(L).Text)
                lngFUNOU_KEN(aintPAGE, L) = fn_DEL_KANMA(txtFunouKen(L).Text)
                lngFUNOU_KIN(aintPAGE, L) = fn_DEL_KANMA(txtFunouKin(L).Text)
            Next
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ページ保存)", "失敗", ex.ToString)
            Return False
        End Try
        fn_GAMEN_HOZON = True
    End Function
    Public Function fn_check_text(ByVal aintPAGE As Integer) As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :aintPAGE:ページ番号
        'Description    :aintPAGEページのテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/15
        'Update         :
        '============================================================================
        fn_check_text = False
        Try
            Dim L As Integer
            For L = 1 To 15
                '請求件数と不能件数・振替件数の一致チェック
                If GCom.NzLong(fn_DEL_KANMA(txtSyoriKen(L).Text)) <> _
                   GCom.NzLong(fn_DEL_KANMA(txtFunouKen(L).Text)) + GCom.NzLong(fn_DEL_KANMA(txtFurikaeKen(L).Text)) Then
                    MessageBox.Show(String.Format(MSG0337W, "入力件数"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSyoriKen(L).Focus()
                    Return False
                End If

                '請求金額と不能金額・振替金額の一致チェック
                If GCom.NzLong(fn_DEL_KANMA(txtSyoriKin(L).Text)) <> _
                   GCom.NzLong(fn_DEL_KANMA(txtFunouKin(L).Text)) + GCom.NzLong(fn_DEL_KANMA(txtFurikaeKin(L).Text)) Then
                    MessageBox.Show(String.Format(MSG0337W, "入力金額"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSyoriKin(L).Focus()
                    Return False
                End If
                '件数･金額矛盾チェック(処理金額)
                If GCom.NzLong(fn_DEL_KANMA(txtSyoriKin(L).Text)) > 0 AndAlso GCom.NzLong(fn_DEL_KANMA(txtSyoriKen(L).Text)) = 0 Then
                    MessageBox.Show(MSG0338W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSyoriKen(L).Focus()
                    Return False
                End If
                '件数･金額矛盾チェック(振替金額)
                If GCom.NzLong(fn_DEL_KANMA(txtFurikaeKin(L).Text)) > 0 AndAlso GCom.NzLong(fn_DEL_KANMA(txtFurikaeKen(L).Text)) = 0 Then
                    MessageBox.Show(MSG0338W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFurikaeKen(L).Focus()
                    Return False
                End If
                '件数･金額矛盾チェック(不能金額)
                If GCom.NzLong(fn_DEL_KANMA(txtFunouKin(L).Text)) > 0 AndAlso GCom.NzLong(fn_DEL_KANMA(txtFunouKen(L).Text)) = 0 Then
                    MessageBox.Show(MSG0338W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFunouKin(L).Focus()
                    Return False
                End If
            Next
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try
        fn_check_text = True
    End Function
    Function fn_DEL_KANMA(ByVal aINTEXT As String) As Long
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_DEL_KANMA
        'Parameter  :aINTEXT:入力値
        'Description:カンマ編集(カンマ削除）関数
        'Return     :置換後の文字(LONG)
        'Create     :2009/09/15
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        Try
            Dim I As Integer
            Dim strO_SYOKITI As String = ""
            If aINTEXT.Trim.Length = 0 Then
                aINTEXT = "0"
                Exit Function
            End If
            For I = 0 To aINTEXT.Trim.Length - 1        'ｶﾝﾏ&ｽﾍﾟｰｽとる
                If aINTEXT.Substring(I, 1) = "," Or aINTEXT.Substring(I, 1) = " " Then
                    strO_SYOKITI = strO_SYOKITI & ""
                Else
                    strO_SYOKITI = strO_SYOKITI & aINTEXT.Substring(I, 1)
                End If
            Next I
            fn_DEL_KANMA = CLng(strO_SYOKITI)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(カンマ削除)", "失敗", ex.ToString)
        End Try
    End Function

#End Region
#Region "テキストボックス"
    '#Region "処理件数"
    '    Private Sub txtSyoriKen_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
    '                txtSyoriKen1.TextChanged, txtSyoriKen2.TextChanged, txtSyoriKen3.TextChanged, txtSyoriKen4.TextChanged, txtSyoriKen5.TextChanged, _
    '                txtSyoriKen6.TextChanged, txtSyoriKen7.TextChanged, txtSyoriKen8.TextChanged, txtSyoriKen9.TextChanged, txtSyoriKen10.TextChanged, _
    '                txtSyoriKen11.TextChanged, txtSyoriKen12.TextChanged, txtSyoriKen13.TextChanged, txtSyoriKen14.TextChanged, txtSyoriKen15.TextChanged

    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 12 Then
    '            strINDEX = strSENDER_NAME.Substring(11, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(11, 2)
    '        End If

    '        If fn_TEXT_CHANGED(txtSyoriKen(strINDEX), "処理件数") = False Then
    '            MessageBox.Show("数値チェックに失敗しました", gstrSYORI_R)
    '        End If

    '    End Sub
    '    Private Sub txtSyoriKen_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles _
    '    txtSyoriKen1.KeyPress, txtSyoriKen2.KeyPress, txtSyoriKen3.KeyPress, txtSyoriKen4.KeyPress, txtSyoriKen5.KeyPress, _
    '    txtSyoriKen6.KeyPress, txtSyoriKen7.KeyPress, txtSyoriKen8.KeyPress, txtSyoriKen9.KeyPress, txtSyoriKen10.KeyPress, _
    '    txtSyoriKen11.KeyPress, txtSyoriKen12.KeyPress, txtSyoriKen13.KeyPress, txtSyoriKen14.KeyPress, txtSyoriKen15.KeyPress

    '        Dim KeyAscii As Short = Asc(e.KeyChar)
    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String
    '        Dim objNEXT_OBJ As Object

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 12 Then
    '            strINDEX = strSENDER_NAME.Substring(11, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(11, 2)
    '        End If

    '        If KeyAscii = 13 Then   '13:Enterｷｰ
    '            fn_TEXT_KEYPRESS(txtSyoriKen(strINDEX), "処理件数", txtSyoriKin(strINDEX), e)
    '        End If
    '    End Sub
    '#End Region
    '#Region "処理金額"
    '    Private Sub txtSyoriKin_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
    '                txtSyoriKin1.TextChanged, txtSyoriKin2.TextChanged, txtSyoriKin3.TextChanged, txtSyoriKin4.TextChanged, txtSyoriKin5.TextChanged, _
    '                txtSyoriKin6.TextChanged, txtSyoriKin7.TextChanged, txtSyoriKin8.TextChanged, txtSyoriKin9.TextChanged, txtSyoriKin10.TextChanged, _
    '                txtSyoriKin11.TextChanged, txtSyoriKin12.TextChanged, txtSyoriKin13.TextChanged, txtSyoriKin14.TextChanged, txtSyoriKin15.TextChanged

    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 12 Then
    '            strINDEX = strSENDER_NAME.Substring(11, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(11, 2)
    '        End If

    '        If fn_TEXT_CHANGED(txtSyoriKin(strINDEX), "処理金額") = False Then
    '            MessageBox.Show("数値チェックに失敗しました", gstrSYORI_R)
    '        End If

    '    End Sub

    '    Private Sub txtSyoriKin_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles _
    '    txtSyoriKin1.KeyPress, txtSyoriKin2.KeyPress, txtSyoriKin3.KeyPress, txtSyoriKin4.KeyPress, txtSyoriKin5.KeyPress, _
    '    txtSyoriKin6.KeyPress, txtSyoriKin7.KeyPress, txtSyoriKin8.KeyPress, txtSyoriKin9.KeyPress, txtSyoriKin10.KeyPress, _
    '    txtSyoriKin11.KeyPress, txtSyoriKin12.KeyPress, txtSyoriKin13.KeyPress, txtSyoriKin14.KeyPress, txtSyoriKin15.KeyPress

    '        Dim KeyAscii As Short = Asc(e.KeyChar)
    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String
    '        Dim objNEXT_OBJ As Object

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 12 Then
    '            strINDEX = strSENDER_NAME.Substring(11, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(11, 2)
    '        End If

    '        If KeyAscii = 13 Then   '13:Enterｷｰ
    '            fn_TEXT_KEYPRESS(txtSyoriKin(strINDEX), "処理金額", txtFurikaeKen(strINDEX), e)
    '        End If
    '    End Sub

    '#End Region
    '#Region "振替済件数"
    '    Private Sub txtFurikaeKen_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
    '                txtFurikaeKen1.TextChanged, txtFurikaeKen2.TextChanged, txtFurikaeKen3.TextChanged, txtFurikaeKen4.TextChanged, txtFurikaeKen5.TextChanged, _
    '                txtFurikaeKen6.TextChanged, txtFurikaeKen7.TextChanged, txtFurikaeKen8.TextChanged, txtFurikaeKen9.TextChanged, txtFurikaeKen10.TextChanged, _
    '                txtFurikaeKen11.TextChanged, txtFurikaeKen12.TextChanged, txtFurikaeKen13.TextChanged, txtFurikaeKen14.TextChanged, txtFurikaeKen15.TextChanged

    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 14 Then
    '            strINDEX = strSENDER_NAME.Substring(13, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(13, 2)
    '        End If

    '        If fn_TEXT_CHANGED(txtFurikaeKen(strINDEX), "振替済件数") = False Then
    '            MessageBox.Show("数値チェックに失敗しました", gstrSYORI_R)
    '        End If

    '    End Sub

    '    Private Sub txtFurikaeKen_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles _
    '    txtFurikaeKen1.KeyPress, txtFurikaeKen2.KeyPress, txtFurikaeKen3.KeyPress, txtFurikaeKen4.KeyPress, txtFurikaeKen5.KeyPress, _
    '    txtFurikaeKen6.KeyPress, txtFurikaeKen7.KeyPress, txtFurikaeKen8.KeyPress, txtFurikaeKen9.KeyPress, txtFurikaeKen10.KeyPress, _
    '    txtFurikaeKen11.KeyPress, txtFurikaeKen12.KeyPress, txtFurikaeKen13.KeyPress, txtFurikaeKen14.KeyPress, txtFurikaeKen15.KeyPress

    '        Dim KeyAscii As Short = Asc(e.KeyChar)
    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String
    '        Dim objNEXT_OBJ As Object

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 14 Then
    '            strINDEX = strSENDER_NAME.Substring(13, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(13, 2)
    '        End If

    '        If KeyAscii = 13 Then   '13:Enterｷｰ
    '            fn_TEXT_KEYPRESS(txtFurikaeKen(strINDEX), "振替済件数", txtFurikaeKin(strINDEX), e)
    '        End If
    '    End Sub
    '#End Region
    '#Region "振替済金額"
    '    Private Sub txtFurikaeKin_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
    '                txtFurikaeKin1.TextChanged, txtFurikaeKin2.TextChanged, txtFurikaeKin3.TextChanged, txtFurikaeKin4.TextChanged, txtFurikaeKin5.TextChanged, _
    '                txtFurikaeKin6.TextChanged, txtFurikaeKin7.TextChanged, txtFurikaeKin8.TextChanged, txtFurikaeKin9.TextChanged, txtFurikaeKin10.TextChanged, _
    '                txtFurikaeKin11.TextChanged, txtFurikaeKin12.TextChanged, txtFurikaeKin13.TextChanged, txtFurikaeKin14.TextChanged, txtFurikaeKin15.TextChanged

    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 14 Then
    '            strINDEX = strSENDER_NAME.Substring(13, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(13, 2)
    '        End If

    '        If fn_TEXT_CHANGED(txtFurikaeKin(strINDEX), "振替済金額") = False Then
    '            MessageBox.Show("数値チェックに失敗しました", gstrSYORI_R)
    '        End If

    '    End Sub

    '    Private Sub txtFurikaeKin_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles _
    '    txtFurikaeKin1.KeyPress, txtFurikaeKin2.KeyPress, txtFurikaeKin3.KeyPress, txtFurikaeKin4.KeyPress, txtFurikaeKin5.KeyPress, _
    '    txtFurikaeKin6.KeyPress, txtFurikaeKin7.KeyPress, txtFurikaeKin8.KeyPress, txtFurikaeKin9.KeyPress, txtFurikaeKin10.KeyPress, _
    '    txtFurikaeKin11.KeyPress, txtFurikaeKin12.KeyPress, txtFurikaeKin13.KeyPress, txtFurikaeKin14.KeyPress, txtFurikaeKin15.KeyPress

    '        Dim KeyAscii As Short = Asc(e.KeyChar)
    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String
    '        Dim objNEXT_OBJ As Object

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 14 Then
    '            strINDEX = strSENDER_NAME.Substring(13, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(13, 2)
    '        End If

    '        If KeyAscii = 13 Then   '13:Enterｷｰ
    '            fn_TEXT_KEYPRESS(txtFurikaeKin(strINDEX), "振替済金額", txtFunouKen(strINDEX), e)
    '        End If
    '    End Sub
    '#End Region
    '#Region "不能件数"
    '    Private Sub txtFunouKen_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
    '                txtFunouKen1.TextChanged, txtFunouKen2.TextChanged, txtFunouKen3.TextChanged, txtFunouKen4.TextChanged, txtFunouKen5.TextChanged, _
    '                txtFunouKen6.TextChanged, txtFunouKen7.TextChanged, txtFunouKen8.TextChanged, txtFunouKen9.TextChanged, txtFunouKen10.TextChanged, _
    '                txtFunouKen11.TextChanged, txtFunouKen12.TextChanged, txtFunouKen13.TextChanged, txtFunouKen14.TextChanged, txtFunouKen15.TextChanged

    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 12 Then
    '            strINDEX = strSENDER_NAME.Substring(11, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(11, 2)
    '        End If

    '        If fn_TEXT_CHANGED(txtFunouKen(strINDEX), "不能件数") = False Then
    '            MessageBox.Show("数値チェックに失敗しました", gstrSYORI_R)
    '        End If

    '    End Sub

    '    Private Sub txtFunouKen_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles _
    '    txtFunouKen1.KeyPress, txtFunouKen2.KeyPress, txtFunouKen3.KeyPress, txtFunouKen4.KeyPress, txtFunouKen5.KeyPress, _
    '    txtFunouKen6.KeyPress, txtFunouKen7.KeyPress, txtFunouKen8.KeyPress, txtFunouKen9.KeyPress, txtFunouKen10.KeyPress, _
    '    txtFunouKen11.KeyPress, txtFunouKen12.KeyPress, txtFunouKen13.KeyPress, txtFunouKen14.KeyPress, txtFunouKen15.KeyPress

    '        Dim KeyAscii As Short = Asc(e.KeyChar)
    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String
    '        Dim objNEXT_OBJ As Object

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 12 Then
    '            strINDEX = strSENDER_NAME.Substring(11, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(11, 2)
    '        End If

    '        If KeyAscii = 13 Then   '13:Enterｷｰ
    '            fn_TEXT_KEYPRESS(txtFunouKen(strINDEX), "不能件数", txtFunouKin(strINDEX), e)
    '        End If
    '    End Sub
    '#End Region
    '#Region "不能金額"
    '    Private Sub txtFunouKin_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
    '                txtFunouKin1.TextChanged, txtFunouKin2.TextChanged, txtFunouKin3.TextChanged, txtFunouKin4.TextChanged, txtFunouKin5.TextChanged, _
    '                txtFunouKin6.TextChanged, txtFunouKin7.TextChanged, txtFunouKin8.TextChanged, txtFunouKin9.TextChanged, txtFunouKin10.TextChanged, _
    '                txtFunouKin11.TextChanged, txtFunouKin12.TextChanged, txtFunouKin13.TextChanged, txtFunouKin14.TextChanged, txtFunouKin15.TextChanged

    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 12 Then
    '            strINDEX = strSENDER_NAME.Substring(11, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(11, 2)
    '        End If

    '        If fn_TEXT_CHANGED(txtFunouKin(strINDEX), "不能金額") = False Then
    '            MessageBox.Show("数値チェックに失敗しました", gstrSYORI_R)
    '        End If

    '    End Sub

    '    Private Sub txtFunouKin_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles _
    '    txtFunouKin1.KeyPress, txtFunouKin2.KeyPress, txtFunouKin3.KeyPress, txtFunouKin4.KeyPress, txtFunouKin5.KeyPress, _
    '    txtFunouKin6.KeyPress, txtFunouKin7.KeyPress, txtFunouKin8.KeyPress, txtFunouKin9.KeyPress, txtFunouKin10.KeyPress, _
    '    txtFunouKin11.KeyPress, txtFunouKin12.KeyPress, txtFunouKin13.KeyPress, txtFunouKin14.KeyPress, txtFunouKin15.KeyPress

    '        Dim KeyAscii As Short = Asc(e.KeyChar)
    '        Dim strSENDER_NAME As String
    '        Dim strINDEX As String
    '        Dim objNEXT_OBJ As Object

    '        strSENDER_NAME = sender.Name

    '        If strSENDER_NAME.Length = 12 Then
    '            strINDEX = strSENDER_NAME.Substring(11, 1)
    '        Else
    '            strINDEX = strSENDER_NAME.Substring(11, 2)
    '        End If

    '        If KeyAscii = 13 Then   '13:Enterｷｰ
    '            If strINDEX <> 15 Then
    '                fn_TEXT_KEYPRESS(txtFunouKin(strINDEX), "不能金額", txtSyoriKen(strINDEX + 1), e)
    '            Else
    '                fn_TEXT_KEYPRESS_OBJ(txtFunouKin(strINDEX), "不能金額", btnAction, e)
    '            End If
    '        End If
    '    End Sub
    '#End Region
#End Region
#Region " 不要な関数"
    'Function fn_TEXT_CHANGED(ByVal atxtOBJ As TextBox, ByVal astrNAME As String) As Boolean
    '    '============================================================================
    '    'NAME           :fn_TEXT_CHANGED
    '    'Parameter      :atxtOBJ：テキストボックス／astrNAME：テキストボックス名
    '    'Description    :テキストボックスのテキストチェンジ時の処理（数値チェック、カンマ編集）
    '    'Return         :True=OK(成功),False=NG（失敗）
    '    'Create         :2004/08/25
    '    'Update         :
    '    '============================================================================
    '    fn_TEXT_CHANGED = False
    '    Try
    '        Dim I As Integer
    '        Dim strSUUTI As String = ""
    '        If atxtOBJ.Text.Trim.Length > 0 Then
    '            For I = 0 To atxtOBJ.Text.Length - 1
    '                If atxtOBJ.Text.Chars(I) = "," Then
    '                Else
    '                    strSUUTI += atxtOBJ.Text.Chars(I)
    '                End If
    '            Next
    '            If clsFUSION.fn_CHECK_NUM_MSG(strSUUTI, astrNAME, gstrSYORI_R) = True Then
    '                atxtOBJ.Text = Format(CInt(strSUUTI), "#,##0")
    '                atxtOBJ.SelectionStart = atxtOBJ.Text.Length
    '            Else
    '                atxtOBJ.Focus()
    '                atxtOBJ.SelectionStart = 0
    '                atxtOBJ.SelectionLength = atxtOBJ.Text.Length
    '            End If
    '        End If
    '    Catch ex As Exception
    '        MessageBox.Show(errMsg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テキストボックス編集)", "失敗", ex.ToString)
    '        Return False
    '    End Try

    '    fn_TEXT_CHANGED = True
    'End Function
    'Function fn_TEXT_KEYPRESS(ByVal atxtOBJ As TextBox, ByVal astrNAME As String, ByVal aobjNEXT_OBJ As TextBox, ByVal e As System.Windows.Forms.KeyPressEventArgs) As Boolean
    '    '============================================================================
    '    'NAME           :fn_TEXT_KEYPRESS
    '    'Parameter      :atxtOBJ：テキストボックス／astrNAME：テキストボックス名
    '    '               :aobjNEXT_OBJ：次にカーソル移動をするオブジェクト
    '    'Description    :テキストボックスのエンターキー押下時の処理（数値チェック、カンマ編集、カーソル移動）
    '    'Return         :True=OK(成功),False=NG（失敗）
    '    'Create         :2004/11/19
    '    'Update         :
    '    '============================================================================
    '    fn_TEXT_KEYPRESS = False

    '    Dim I As Integer
    '    Dim strKINGAKU As String = ""
    '    Dim KeyAscii As Short = Asc(e.KeyChar)

    '    If atxtOBJ.Text.Trim.Length <> 0 Then '未入力可
    '        '数値ﾁｪｯｸ
    '        For I = 0 To atxtOBJ.Text.Length - 1
    '            If atxtOBJ.Text.Chars(I) = "," Then
    '            Else
    '                strKINGAKU += atxtOBJ.Text.Chars(I)
    '            End If
    '        Next
    '        If clsFUSION.fn_CHECK_NUM_MSG(strKINGAKU, astrNAME, gstrSYORI_R) = False Then
    '            KeyAscii = 0
    '            atxtOBJ.SelectionStart = 0
    '            atxtOBJ.SelectionLength = atxtOBJ.SelectionLength
    '            Exit Function
    '        Else
    '            atxtOBJ.Text = Format(CInt(strKINGAKU), "#,##0")
    '        End If
    '    Else
    '        atxtOBJ.Text = "0"
    '    End If
    '    KeyAscii = 0
    '    aobjNEXT_OBJ.Focus()
    '    fn_TEXT_KEYPRESS = True
    'End Function
    'Function fn_TEXT_KEYPRESS_OBJ(ByVal atxtOBJ As TextBox, ByVal astrNAME As String, ByVal aobjNEXT_OBJ As Button, ByVal e As System.Windows.Forms.KeyPressEventArgs) As Boolean
    '    '============================================================================
    '    'NAME           :fn_TEXT_KEYPRESS
    '    'Parameter      :atxtOBJ：テキストボックス／astrNAME：テキストボックス名
    '    '               :aobjNEXT_OBJ：次にカーソル移動をするオブジェクト
    '    'Description    :テキストボックスのエンターキー押下時の処理（数値チェック、カンマ編集、カーソル移動）
    '    'Return         :True=OK(成功),False=NG（失敗）
    '    'Create         :2004/11/19
    '    'Update         :
    '    '============================================================================
    '    fn_TEXT_KEYPRESS_OBJ = False

    '    Dim I As Integer
    '    Dim strKINGAKU As String = ""
    '    Dim KeyAscii As Short = Asc(e.KeyChar)

    '    If atxtOBJ.Text.Trim.Length <> 0 Then '未入力可
    '        '数値ﾁｪｯｸ
    '        For I = 0 To atxtOBJ.Text.Length - 1
    '            If atxtOBJ.Text.Chars(I) = "," Then
    '            Else
    '                strKINGAKU += atxtOBJ.Text.Chars(I)
    '            End If
    '        Next
    '        If clsFUSION.fn_CHECK_NUM_MSG(strKINGAKU, astrNAME, gstrSYORI_R) = False Then
    '            KeyAscii = 0
    '            atxtOBJ.SelectionStart = 0
    '            atxtOBJ.SelectionLength = atxtOBJ.SelectionLength
    '            Exit Function
    '        Else
    '            atxtOBJ.Text = Format(CInt(strKINGAKU), "#,##0")
    '        End If
    '    End If
    '    KeyAscii = 0
    '    aobjNEXT_OBJ.Focus()
    '    fn_TEXT_KEYPRESS_OBJ = True
    'End Function
#End Region
End Class