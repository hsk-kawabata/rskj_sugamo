Option Explicit Off
Option Strict Off

Imports System.IO
Imports System.Text

Public Class KFGMAIN050

    '手数料計算用　（c:\fskj\txt\手数料テーブル.TXTの読込用）
    Structure TESUU_TABLE
        Public intKIJYUN_KINKO As Integer
        Public intKIJYUN_KINKEN As Integer
        Public KIJYUN_KIJYUN1 As Double
        Public KIJYUN_KIJYUN2 As Double
        Public KIJYUN_KIJYUN3 As Double
        Public KIJYUN_KIJYUN4 As Double
        Public KIJYUN_KIJYUN5 As Double
        Public KIJYUN1_TESUU As Long
        Public KIJYUN2_TESUU As Long
        Public KIJYUN3_TESUU As Long
        Public KIJYUN4_TESUU As Long
        Public KIJYUN5_TESUU As Long
    End Structure
    Private TESUU_TABLE_DATA(10) As TESUU_TABLE

#Region " 共通変数定義 "
    Private Str_Gakkou_Code As String

    Private SNG_ZEIRITU As Single
    Private STR_ZEIRITU As String
    Private Str_Function_Ret As String
    Private Str_Honbu_Code As String
    Private Str_Tesuuryo_Kouza As String
    Private Str_Tesuuryo_Kouza2 As String
    Private Str_Tesuuryo_Kouza3 As String

    Private Str_Utiwake_Code As String
    Private Str_Utiwake_Code2 As String
    Private Str_Utiwake_Code3 As String
    Private Str_Kawase_CenterName As String
    Private Str_IraiNinName As String
    ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- START
    'Private Str_KijyunDate As String
    ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- END
    Private Str_Kekka_Path As String

    Private Str_Ret As String

    Private Str_Ginko(3) As String

    Private Str_Meigi As String
    Private Str_Biko1 As String
    Private Str_Biko2 As String

    Private Str_Prt_GAKKOU_CODE As String
    Private Str_Prt_GAKKOU_KNAME As String
    Private Str_Prt_GAKKOU_NNAME As String
    Private Str_Prt_KESSAI_DATE As String
    Private Str_Prt_HONBU_KOUZA As String
    Private Str_Prt_TESUURYO_JIFURI As String
    Private Str_Prt_TESUURYO_FURI As String
    Private Str_Prt_TESUURYO_ETC As String
    Private Str_Prt_JIFURI_KOUZA As String
    Private Str_Prt_FURI_KOUZA As String
    Private Str_Prt_ETC_KOUZA As String
    Private Str_Prt_HIMOKU_NNAME As String
    Private Str_Prt_KESSAI_PATERN As String
    Private Str_Prt_NYUKIN_KINGAKU As String
    Private Str_Prt_KESSAI_KIN_CODE As String
    Private Str_Prt_KESSAI_KIN_KNAME As String
    Private Str_Prt_KESSAI_KIN_NNAME As String
    Private Str_Prt_KESSAI_TENPO As String
    Private Str_Prt_KESSAI_SIT_KNAME As String
    Private Str_Prt_KESSAI_SIT_NNAME As String
    Private Str_Prt_KESSAI_KAMOKU As String
    Private Str_Prt_KESSAI_KOUZA As String
    Private Str_Prt_KESSAI_DATA As String

    Private Str_Prt_FURI_KBN As String '振替区分 2005/06/28
    Private dblFURIKOMI_TESUU_ALL As Double = 0 '振込手数料合計 2006/04/15
    Private Str_Prt_FURI_DATE As String '振替日 2006/04/18

    Private Str_Err_Gakkou_Name As String
    Private Str_Err_Kessai_Kbn As String
    Private Str_Err_Tesuutyo_Kbn As String

    Private Int_FD_Kbn As Integer

    Private Lng_Upd_Count As Long
    Private Lng_RecordNo As Long

    Private Structure LineData
        <VBFixedStringAttribute(303)> Public Data As String
    End Structure

    Private Line As LineData
    Private NextLine As LineData

    Private Structure Plus
        <VBFixedStringAttribute(280)> Public Data As String
    End Structure

    Private StrLine As Plus

    Public lngSAKI_SUU As Long '取引先数
    Public lngALL_KEN As Long '総合計件数
    Public lngALL_KIN As Long '総合計金額
    Public lngFURI_ALL_KEN As Long '総振替件数
    Public lngFURI_ALL_KIN As Long '総振替金額
    Public lngFUNOU_ALL_KEN As Long '総不能件数
    Public lngFUNOU_ALL_KIN As Long '総不能金額
    Public lngTESUU_ALL As Long '手数料総合計
    Public lngTESUU1 As Long '総手数料1
    Public lngTESUU2 As Long '総手数料2
    Public lngTESUU3 As Long '総手数料3

    Public strTESUUTYO_KBN As String '手数徴求区分
    Public strIN_OUT_KBN As String '入出金区分

    Public lngNYUKIN_SCH_CNT As Long '入金分決済スケジュール数
    Public sSyori_Date As String '移動 2005/06/30

    'ファイル名用変数 2006/04/14
    Structure typ_TITLE
        <VBFixedString(16)> Dim strTITLE_16 As String
    End Structure
    Private strTITLE As typ_TITLE
    '振込手数料用変数 2006/04/17
    Public lngFURI_TESUU1 As Long
    Public lngFURI_TESUU2 As Long
    Public lngFURI_TESUU3 As Long
    Public lngFURI_TESUU4 As Long
    Public lngFURI_TESUU5 As Long

    Public lngG_PRTKESSAI_REC As Long

    Public lngSYORIKEN_TOKUBETU As Long = 0
    Public dblSYORIKIN_TOKUBETU As Double = 0
    Public lngFURIKEN_TOKUBETU As Long = 0
    Public dblFURIKIN_TOKUBETU As Double = 0
    Public lngFUNOUKEN_TOKUBETU As Long = 0
    Public dblFUNOUKIN_TOKUBETU As Double = 0
    Public dblTESUU_TOKUBETU As Double = 0
    Public dblTESUU_A1_TOKUBETU As Double = 0
    Public dblTESUU_A2_TOKUBETU As Double = 0
    Public dblTESUU_A3_TOKUBETU As Double = 0


    Public dblTESUU_KIN1_S As Double = 0
    Public dblTESUU_KIN2_S As Double = 0
    Public dblTESUU_KIN3_S As Double = 0
    Public intTESUUTYO_KBN_T As Integer = 0
    Public strGAKKOU_CODE_S As String = ""
    Public strGAKKOU_KNAME_G As String = ""
    Public strTESUUTYOKIN_NO_T As String = ""
    Public strTESUUTYO_SIT_T As String = ""
    Public strTESUUTYO_KAMOKU_T As String = ""
    Public strTESUUTYO_KOUZA_T As String = ""
    Public dblNYUKIN_GAK As Double = 0 '学校毎の入金金額（処理金額または振替金額)
    Public iGakunen_Flag() As Integer

#End Region

#Region " Form Load "
    Private Sub KFGMAIN050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        With Me
            .WindowState = FormWindowState.Normal
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .ControlBox = True
        End With

        Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

        STR_SYORI_NAME = "資金決済データ作成"
        STR_COMMAND = "Form_Load"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = ""

        MainLog = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

        '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
        Call GSUB_CONNECT()
        '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

        '消費税率取得
        STR_ZEIRITU = CSng(GFUNC_INI_READ("COMMON", "ZEIRITU"))
        '本部コード取得
        Str_Honbu_Code = GFUNC_INI_READ("COMMON", "HONBUCD")

        ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- START
        '和暦変換基準日取得
        'Str_KijyunDate = GFUNC_INI_READ("COMMON", "WAREKIKIJYUNBI")
        ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- END

        '手数料入金口座取得(口振手数料・郵送料）
        Str_Tesuuryo_Kouza = GFUNC_INI_READ("KESSAI", "TESUUKOUZA1")

        '手数料入金口座取得(未使用)
        Str_Tesuuryo_Kouza2 = GFUNC_INI_READ("KESSAI", "TESUUKOUZA2")

        '手数料入金口座取得(振込手数料)
        Str_Tesuuryo_Kouza3 = GFUNC_INI_READ("KESSAI", "TESUUKOUZA3")

        '内訳取得(口振手数料・郵送料)
        Str_Utiwake_Code = GFUNC_INI_READ("KESSAI", "UTIWAKE1")

        '内訳2取得(未使用)
        Str_Utiwake_Code2 = GFUNC_INI_READ("KESSAI", "UTIWAKE2")

        '内訳3取得(郵送料)
        Str_Utiwake_Code3 = GFUNC_INI_READ("KESSAI", "UTIWAKE3")

        '依頼人名取得
        Str_IraiNinName = GFUNC_INI_READ("KESSAI", "IRAININ")

        '為替センター名取得
        Str_Kawase_CenterName = GFUNC_INI_READ("KAWASE", "KAWASECENTER")

        '結果FDPATH取得
        Str_Kekka_Path = GFUNC_INI_READ("KESSAI", "KEKKAFD")

        'FD区分取得
        Int_FD_Kbn = CInt(GFUNC_INI_READ("KESSAI", "FDKBN"))


        'Oracle 接続(Read専用)
        OBJ_CONNECTION_DREAD = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read専用)
        OBJ_CONNECTION_DREAD.Open()

        'Oracle 接続(Read専用)
        OBJ_CONNECTION_DREAD2 = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read専用)
        OBJ_CONNECTION_DREAD2.Open()

        'Oracle 接続(Read専用)
        OBJ_CONNECTION_DREAD3 = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read専用)
        OBJ_CONNECTION_DREAD3.Open()

        'TXT読み込み追加 206/04/15
        Call PSUB_TesuryoKijyun_Input()

    End Sub
#End Region

#Region " Button Click "
    Private Sub btnCreate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreate.Click
        '確認メッセージ
        If GFUNC_MESSAGE_QUESTION("作成しますか？") <> vbOK Then
            Exit Sub
        End If

        Cursor.Current = Cursors.WaitCursor()

        '帳票ワークの削除
        If PFUNC_DeletePrtKessai() = False Then
            Exit Sub
        End If

        '入力値チェック
        If PFUNC_Nyuryoku_Check() = False Then
            Exit Sub
        End If

        STR_COMMAND = "資金決済データ作成"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = STR_FURIKAE_DATE(1)

        If PFUNC_Himoku_Create() = False Then
            Exit Sub
        End If

        '入金について電文は作成しない(標準版) 2006/04/17
        ''入金のスケジュールが存在するかチェック
        'If fn_select_G_SCHMAST_NYU() = False Then
        '    Exit Sub
        'End If

        'Select case CInt(STR_FSKJ_FLG)
        '    Case 0
        '学校自振単独
        '手数料計算
        If PFUNC_Tesuuryo_Keisan() = False Then

            Exit Sub
        End If
        'Case 1
        '        '企業自振連携
        '        '手数料計算
        'End Select

        'リエンタ(決済)データ作成
        'ワークファイル作成
        If PFUNC_Write_WorkD() = False Then

            Exit Sub
        End If

        'ヘッダ部、データ部ファイル作成
        If PFUNC_Split_WorkD() = False Then
            MessageBox.Show("リエンタFD作成失敗", "資金決済データ作成", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        Else
            MessageBox.Show("リエンタFD作成完了", "資金決済データ作成", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        btnEnd.Focus()
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        If OBJ_CONNECTION_DREAD Is Nothing Then
        Else
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD.Close()
            OBJ_CONNECTION_DREAD = Nothing
        End If

        If OBJ_CONNECTION_DREAD2 Is Nothing Then
        Else
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD2.Close()
            OBJ_CONNECTION_DREAD2 = Nothing
        End If

        If OBJ_CONNECTION_DREAD3 Is Nothing Then
        Else
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD3.Close()
            OBJ_CONNECTION_DREAD3 = Nothing
        End If

        Me.Close()

    End Sub
#End Region

#Region " Private Sub "
    Private Sub PSUB_TesuryoKijyun_Input()

        Dim iFileNo As Integer
        Dim iFileCount As Integer

        Dim sFileIndex As String = ""
        Dim sFileItemName As String = ""

        '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        '手数料テーブルファイルの読込
        '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        iFileNo = FreeFile()
        FileOpen(iFileNo, STR_TXT_PATH & STR_TESUURYO_TXT, OpenMode.Input)

        iFileCount = -1

        Do Until EOF(iFileNo)
            iFileCount += 1
            Input(iFileNo, sFileIndex)
            Input(iFileNo, sFileItemName)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).intKIJYUN_KINKO)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).intKIJYUN_KINKEN)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).KIJYUN_KIJYUN1)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).KIJYUN_KIJYUN2)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).KIJYUN_KIJYUN3)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).KIJYUN_KIJYUN4)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).KIJYUN_KIJYUN5)
        Loop

        FileClose(iFileNo)

    End Sub
    Private Sub PSUB_Put_Data(ByVal pFileNo As Integer, ByVal pValue As String)

        Line.Data = pValue
        Lng_RecordNo += 1

        FilePut(pFileNo, Line, Lng_RecordNo)

    End Sub
    Private Sub PSUB_GET_GINKONAME(ByVal pGinko_Code As String, ByVal pSiten_Code As String)

        '金融機関コードと支店コードから金融機関名、支店名を抽出

        Str_Ginko(0) = ""
        Str_Ginko(1) = ""
        Str_Ginko(2) = ""
        Str_Ginko(3) = ""

        If Trim(pGinko_Code) = "" Or Trim(pSiten_Code) = "" Then
            Exit Sub
        End If

        STR_SQL = "SELECT KIN_KNAME_N , KIN_NNAME_N , SIT_KNAME_N , SIT_NNAME_N  FROM TENMAST "
        STR_SQL += " WHERE KIN_NO_N = '" & pGinko_Code & "'"
        STR_SQL += " AND SIT_NO_N = '" & pSiten_Code & "'"

        If GFUNC_SELECT_SQL5(STR_SQL, 0) = False Then
            Exit Sub
        End If

        If OBJ_DATAREADER_DREAD3.HasRows = False Then
            If GFUNC_SELECT_SQL5("", 1) = False Then
                Exit Sub
            End If

            Exit Sub
        End If
        With OBJ_DATAREADER_DREAD3
            .Read()

            Str_Ginko(0) = .Item("KIN_KNAME_N")
            Str_Ginko(1) = .Item("SIT_KNAME_N")
            Str_Ginko(2) = .Item("KIN_NNAME_N")
            Str_Ginko(3) = .Item("SIT_NNAME_N")
        End With

        If GFUNC_SELECT_SQL5("", 1) = False Then
            Exit Sub
        End If

    End Sub
    '2005/06/20　追加
    Private Sub PSUB_GET_TESUUTYO_KBN_T(ByVal pGAKKOU_Code As String)

        '学校コードから手数徴求区分を抽出

        If Trim(pGAKKOU_Code) = "" Then
            Exit Sub
        End If

        STR_SQL = "SELECT TESUUTYO_KBN_T  FROM GAKMAST2 "
        STR_SQL += " WHERE GAKKOU_CODE_T = '" & pGAKKOU_Code & "'"

        If GFUNC_SELECT_SQL5(STR_SQL, 0) = False Then
            Exit Sub
        End If

        If OBJ_DATAREADER_DREAD3.HasRows = False Then
            If GFUNC_SELECT_SQL5("", 1) = False Then
                Exit Sub
            End If

            Exit Sub
        End If
        With OBJ_DATAREADER_DREAD3
            .Read()

            strTESUUTYO_KBN = .Item("TESUUTYO_KBN_T")
        End With

        If GFUNC_SELECT_SQL5("", 1) = False Then
            Exit Sub
        End If

    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " Private Function "
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False

        '決済日
        If Trim(txtKessaiDateY.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("決済日(年)が未入力です。")
            txtKessaiDateY.Focus()

            Exit Function
        Else
            Select Case (CInt(txtKessaiDateY.Text))
                Case Is >= 2004
                Case Else
                    Call GSUB_MESSAGE_WARNING("決済日(年)は２００４年以降を設定してください。")
                    txtKessaiDateY.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtKessaiDateM.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("決済日(月)が未入力です。")
            txtKessaiDateM.Focus()

            Exit Function
        Else
            Select Case (CInt(txtKessaiDateM.Text))
                Case 1 To 12
                Case Else
                    Call GSUB_MESSAGE_WARNING("決済月は１月～１２月を設定してください。")
                    txtKessaiDateM.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtKessaiDateD.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("決済日(日)が未入力です。")
            txtKessaiDateD.Focus()

            Exit Function
        End If

        STR_FURIKAE_DATE(0) = Trim(txtKessaiDateY.Text) & "/" & Trim(txtKessaiDateM.Text) & "/" & Trim(txtKessaiDateD.Text)
        STR_FURIKAE_DATE(1) = Trim(txtKessaiDateY.Text) & Format(CInt(txtKessaiDateM.Text), "00") & Format(CInt(txtKessaiDateD.Text), "00")

        If Not IsDate(STR_FURIKAE_DATE(0)) Then
            Call GSUB_MESSAGE_WARNING("決済日が正しくありません")

            txtKessaiDateY.Focus()

            Exit Function
        End If

        'スケジュール存在チェック
        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " KESSAI_YDATE_S ='" & STR_FURIKAE_DATE(1) & "'"


        If GFUNC_ISEXIST(STR_SQL) = False Then
            Call GSUB_MESSAGE_WARNING("入力した決済日に該当するスケジュールが存在しません")

            Exit Function
        End If

        PFUNC_Nyuryoku_Check = True

    End Function

    Private Function PFUNC_Tesuuryo_Keisan() As Boolean

        Dim iTableID As Integer

        Dim lKingaku1 As Long

        Dim lTesuuryo1 As Long
        Dim lTesuuryo2 As Long
        Dim lTesuuryo3 As Long
        '追加 2006/04/17
        Dim strGAKUNEN1 As String
        Dim strGAKUNEN2 As String
        Dim strGAKUNEN3 As String
        Dim strGAKUNEN4 As String
        Dim strGAKUNEN5 As String
        Dim strGAKUNEN6 As String
        Dim strGAKUNEN7 As String
        Dim strGAKUNEN8 As String
        Dim strGAKUNEN9 As String

        PFUNC_Tesuuryo_Keisan = False

        STR_SQL = " SELECT "
        STR_SQL += " G_SCHMAST.*"
        STR_SQL += ",GAKMAST2.*"
        STR_SQL += " FROM "
        STR_SQL += " G_SCHMAST"
        STR_SQL += ",GAKMAST2"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " AND"
        STR_SQL += " KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND"
        STR_SQL += " FUNOU_FLG_S = '1'"
        STR_SQL += " AND"
        STR_SQL += " TYUUDAN_FLG_S = '0'"
        STR_SQL += " AND"
        STR_SQL += " KESSAI_FLG_S = '0'"
        STR_SQL += " AND"
        STR_SQL += " KESSAI_KBN_T <> '99'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S <> '2'" '入金・臨時出金データについては手数料計算しない 2007/02/10
        'STR_SQL += " FURI_KBN_S <> '2'" '入金データについては手数料計算しない 2005/06/30
        STR_SQL += " ORDER BY GAKKOU_CODE_S,FURI_KBN_S"


        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
                Exit Function
            End If

            If lngNYUKIN_SCH_CNT > 0 Then
                PFUNC_Tesuuryo_Keisan = True
            Else
                Call GSUB_MESSAGE_WARNING("指定日の処理対象先はありません")
            End If

            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD

                '学年フラグ取得 2006/04/17
                strGAKUNEN1 = .Item("GAKUNEN1_FLG_S")
                strGAKUNEN2 = .Item("GAKUNEN2_FLG_S")
                strGAKUNEN3 = .Item("GAKUNEN3_FLG_S")
                strGAKUNEN4 = .Item("GAKUNEN4_FLG_S")
                strGAKUNEN5 = .Item("GAKUNEN5_FLG_S")
                strGAKUNEN6 = .Item("GAKUNEN6_FLG_S")
                strGAKUNEN7 = .Item("GAKUNEN7_FLG_S")
                strGAKUNEN8 = .Item("GAKUNEN8_FLG_S")
                strGAKUNEN9 = .Item("GAKUNEN9_FLG_S")


                iTableID = .Item("TESUU_TABLE_ID_T")
                TESUU_TABLE_DATA(iTableID).KIJYUN1_TESUU = .Item("TESUU_A1_T")
                TESUU_TABLE_DATA(iTableID).KIJYUN2_TESUU = .Item("TESUU_A2_T")
                TESUU_TABLE_DATA(iTableID).KIJYUN3_TESUU = .Item("TESUU_A3_T")
                TESUU_TABLE_DATA(iTableID).KIJYUN4_TESUU = .Item("TESUU_B1_T")
                TESUU_TABLE_DATA(iTableID).KIJYUN5_TESUU = .Item("TESUU_B2_T")

                '手数料徴求区分→全ての徴求区分で手数料計算を行う 2005/06/20
                'Select case CInt(.Item("TESUUTYO_KBN_T"))

                'Case 0, 1
                '都度徴求
                '一括徴求

                '請求引落区分
                Select Case (CInt(.Item("SEIKYU_KBN_T")))
                    Case 0
                        '請求分徴求
                        '①金額 = 振替手数料単価 * 処理件数
                        lKingaku1 = .Item("TESUU_TANKA_T") * .Item("SYORI_KEN_S")
                    Case 1
                        '振替分徴求
                        '①金額 = 振替手数料単価 * 振替済件数
                        lKingaku1 = .Item("TESUU_TANKA_T") * .Item("FURI_KEN_S")
                End Select

                '消費税区分
                Select Case (CInt(.Item("SYOUHI_KBN_T")))
                    Case 0
                        '外税
                        '手数料１ = 少数切捨(①金額 * 税率(INIﾌｧｲﾙより取得))
                        'lTesuuryo1 = Fix(lKingaku1 * Sng_Zeiritu)
                        lTesuuryo1 = Fix(CDbl(lKingaku1) * Val(STR_ZEIRITU))
                    Case 1
                        '内税
                        '手数料１ = ①金額
                        lTesuuryo1 = lKingaku1
                End Select

                '送料計上区分
                Select Case (CInt(.Item("SOURYO_KBN_T")))
                    Case 0
                        '計上しない
                        '手数料２ = 0
                        lTesuuryo2 = 0
                    Case 1
                        '計上する
                        '手数料２ = 送料
                        lTesuuryo2 = CInt(.Item("SOURYO_T"))
                End Select

                '学校自振は費目の決済口座毎に振り込み手数料の計算が必要なのでここでは行わない 2006/04/15
                lTesuuryo3 = 0
                ''手数料３ = 振込手数料計算
                'lTesuuryo3 = PFUNC_Get_FuriTesuryo(Str_Jikou_Ginko_Code, (.Item("FURI_KIN_S") - (lTesuuryo1 + lTesuuryo2)), .Item("FURI_KEN_S"), iTableID)


                'Case 2
                '    '特別免除

                '    '手数料１ = 0
                '    '手数料２ = 0
                '    '手数料３ = 0
                '    lTesuuryo1 = 0
                '    lTesuuryo2 = 0
                '    lTesuuryo3 = 0
                'End Select

                '算出した手数料をスケジュール(現在参照中のレコード)に更新する
                STR_SQL = " UPDATE  G_SCHMAST SET "
                STR_SQL += " TESUU_KIN_S = " & (lTesuuryo1 + lTesuuryo2 + lTesuuryo3)
                STR_SQL += ",TESUU_KIN1_S = " & lTesuuryo1
                STR_SQL += ",TESUU_KIN2_S = " & lTesuuryo2
                STR_SQL += ",TESUU_KIN3_S = " & lTesuuryo3
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_S = '" & Trim(.Item("GAKKOU_CODE_S")) & "'"
                STR_SQL += " AND"
                STR_SQL += " SCH_KBN_S = '" & Trim(.Item("SCH_KBN_S")) & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_S = '" & Trim(.Item("FURI_KBN_S")) & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_S = '" & Trim(.Item("FURI_DATE_S")) & "'"
                '学年コードの指定
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN1_FLG_S = '" & strGAKUNEN1 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN2_FLG_S = '" & strGAKUNEN2 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN3_FLG_S = '" & strGAKUNEN3 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN4_FLG_S = '" & strGAKUNEN4 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN5_FLG_S = '" & strGAKUNEN5 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN6_FLG_S = '" & strGAKUNEN6 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN7_FLG_S = '" & strGAKUNEN7 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN8_FLG_S = '" & strGAKUNEN8 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN9_FLG_S = '" & strGAKUNEN9 & "'"


                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                    Exit Function
                End If
            End With
        End While

        If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_Tesuuryo_Keisan = True

    End Function

    Private Function PFUNC_Get_FuriTesuryo(ByVal pGinko_Code As String, _
                                           ByVal pKingaku As Long, _
                                           ByVal pKensuu As Long, _
                                           ByVal pIndex As Integer) As Long

        PFUNC_Get_FuriTesuryo = 0

        If pKingaku <= 0 Then
            Exit Function
        End If

        If pKensuu <= 0 Then
            Exit Function
        End If

        With TESUU_TABLE_DATA(pIndex)
            Select Case .intKIJYUN_KINKO
                Case 0
                    '僚店・他行を判定しない

                    Select Case .intKIJYUN_KINKEN
                        Case 0
                            '振替済金額の値で手数料を算出
                            Select Case (pKingaku)
                                Case .KIJYUN_KIJYUN1 To .KIJYUN_KIJYUN2 - 1
                                    '振替別手数料１ =< X > 振替別手数料２
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU1
                                Case .KIJYUN_KIJYUN2 To .KIJYUN_KIJYUN3 - 1
                                    '振替別手数料２ =< X > 振替別手数料３
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU2
                                Case Is >= .KIJYUN_KIJYUN3
                                    '振替別手数料３ =< X 
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU3
                                Case Else
                                    PFUNC_Get_FuriTesuryo = 0
                            End Select
                        Case Else
                            '振替済件数の値で手数料を算出
                            Select Case (pKensuu)
                                Case .KIJYUN_KIJYUN1 To .KIJYUN_KIJYUN2 - 1
                                    '振替別手数料１ =< X > 振替別手数料２
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU1
                                Case .KIJYUN_KIJYUN2 To .KIJYUN_KIJYUN3 - 1
                                    '振替別手数料２ =< X > 振替別手数料３
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU2
                                Case Is >= .KIJYUN_KIJYUN3
                                    '振替別手数料３ =< X 
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU3
                                Case Else
                                    PFUNC_Get_FuriTesuryo = 0
                            End Select
                    End Select
                Case Else
                    '僚店・他行を判定する

                    '自行か他行かの判定
                    Select Case (pGinko_Code = STR_JIKINKO_CODE)
                        Case True
                            '自行

                            '振替済金額の値で手数料を算出
                            Select Case (pKingaku)
                                Case .KIJYUN_KIJYUN1 To .KIJYUN_KIJYUN2 - 1
                                    '振替別手数料１ =< X > 振替別手数料２
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU1
                                Case Is >= .KIJYUN_KIJYUN2
                                    '振替別手数料２ < X
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU2
                                Case Else
                                    PFUNC_Get_FuriTesuryo = 0
                            End Select
                        Case False
                            '他行

                            '振替済金額の値で手数料を算出
                            Select Case (pKingaku)
                                Case .KIJYUN_KIJYUN1 To .KIJYUN_KIJYUN2 - 1
                                    '振替別手数料１ =< X > 振替別手数料２
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU3
                                Case Is >= .KIJYUN_KIJYUN2
                                    '振替別手数料２ < X
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU4
                                Case Else
                                    PFUNC_Get_FuriTesuryo = 0
                            End Select
                    End Select
            End Select
        End With

    End Function

    Private Function PFUNC_Write_WorkD() As Boolean

        Dim iFileNo As Integer

        Dim sErrorMessage As String

        Dim sEsc_Gakkou_Code As String = ""
        Dim sEsc_Gakkou_Code2 As String = ""
        Dim strFURI_CHK_GAKKOU As String = "" '振り込み手数料用学校コードチェック 2006/04/19
        Dim sEsc_Furi_Kbn As String = "" '比較振替区分 2005/06/30
        Dim sEsc_Furi_Kbn2 As String = "" '比較振替区分 2005/06/30
        Dim sLine_Data As String = ""
        Dim sSortData As String = ""

        Dim lBetudan_Count As Long
        Dim lFutuu_Count As Long
        Dim lTouza_Count As Long
        Dim lKawase_Count As Long
        Dim lTesuuKano_Count As Long

        Dim lBetudan_Nyu_Count As Long '別段入金データ件数
        Dim lKawase_Seikyu_Count As Long '為替請求データ件数

        Dim lRecord_Count As Long
        Dim lCreateRecord_Count As Long

        Dim lTesuuFuka_Count As Long
        Dim lTesuuFuka_Kingaku As Long

        Dim lngKESSAIREC As Long '決済データ作成対象スケジュール数

        Dim strGAKKOU_NAME As String = ""
        Dim strTESUU_KIN As String = ""
        Dim strTESUU_SIT As String = ""
        Dim strTESUU_KAMOKU As String = ""
        Dim strTESUU_KOUZA As String = ""


        PFUNC_Write_WorkD = False

        sSyori_Date = Format(Now, "yyyyMMddHHmmss")
        sErrorMessage = ""

        STR_SQL = " SELECT "
        STR_SQL += " G_SCHMAST.*"
        STR_SQL += ",GAKMAST1.*"
        STR_SQL += ",GAKMAST2.*"
        STR_SQL += ",MAIN0500G_WORK.*"
        STR_SQL += " FROM "
        STR_SQL += " G_SCHMAST"
        STR_SQL += ",GAKMAST1"
        STR_SQL += ",GAKMAST2"
        STR_SQL += ",MAIN0500G_WORK"
        STR_SQL += " WHERE GAKKOU_CODE_S = GAKKOU_CODE_H"
        STR_SQL += " AND GAKKOU_CODE_H = GAKKOU_CODE_G"
        STR_SQL += " AND GAKUNEN_CODE_G = 1"
        STR_SQL += " AND GAKKOU_CODE_G = GAKKOU_CODE_T"
        STR_SQL += " AND FURI_KBN_S = FURI_KBN_H" '振替区分追加 2005/06/28
        STR_SQL += " AND KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND KESSAI_FLG_S = '0'"
        STR_SQL += " AND TYUUDAN_FLG_S = '0'"
        STR_SQL += " AND DATA_FLG_S = '1'"
        STR_SQL += " AND FUNOU_FLG_S = '1'" '臨時入金の場合はここをコメント
        STR_SQL += " AND SYORI_KIN_S > 0"
        '費目金額が0のものは作成しない 2005/06/20
        STR_SQL += " AND HIMOKU_FURI_KIN_H > 0" 'HIMOKU_KINGAKU_HからHIMOKU_FURI_KIN_Hに変更 2006/04/14
        STR_SQL += " AND KESSAI_KBN_T <> '99'" '決済対象外はデータ作成しない 2005/06/30
        STR_SQL += " AND KESSAI_KBN_T <> '02'" '企業自振連携時、企業自振で決済のものは作成しない 2005/11/02
        '臨時入金・出金については作成しない 2006/04/13
        STR_SQL += " AND SCH_KBN_S <> '2'"
        '並替条件 決済予定日・振替日・学校コード・決済口座情報（金・店・科・口)順 2006/10/19
        STR_SQL += " ORDER BY KESSAI_YDATE_S,FURI_DATE_S,GAKKOU_CODE_S,KESSAI_KIN_CODE_H,KESSAI_TENPO_H,KESSAI_KAMOKU_H,KESSAI_KOUZA_H"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
                Exit Function
            End If

            Call GSUB_MESSAGE_WARNING("指定日の処理対象先はありません")

            Exit Function
        End If

        sEsc_Gakkou_Code = ""
        sEsc_Furi_Kbn = ""

        '追加 2006/04/17
        lngTESUU_ALL = 0 '手数料総合計
        lngTESUU1 = 0 '総手数料1
        lngTESUU2 = 0 '総手数料2
        lngTESUU3 = 0 '総手数料3

        lngFURI_TESUU1 = 0
        lngFURI_TESUU2 = 0
        lngFURI_TESUU3 = 0
        lngFURI_TESUU4 = 0
        lngFURI_TESUU5 = 0

        dblFURIKOMI_TESUU_ALL = 0 '合計振込手数料初期化 2006/04/15

        iFileNo = FreeFile()

        If Dir$(STR_DAT_PATH & "FD_WORK1_D.DAT") <> "" Then
            Kill(STR_DAT_PATH & "FD_WORK1_D.DAT")
        End If

        FileOpen(iFileNo, STR_DAT_PATH & "FD_WORK1_D.DAT", OpenMode.Random, , , 303)
        lngKESSAIREC = 0
        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                If .Item("FUNOU_FLG_S") = "0" And .Item("FURI_KBN_S") <> "2" Then
                    GoTo NEXT_RECORD
                Else
                    lngKESSAIREC = lngKESSAIREC + 1
                End If

                Str_Err_Gakkou_Name = .Item("GAKKOU_NNAME_G")
                Str_Err_Kessai_Kbn = .Item("KESSAI_KBN_T")
                Str_Err_Tesuutyo_Kbn = .Item("TESUUTYO_PATN_T")

                '追加 2006/04/17
                lngFURI_TESUU1 = CLng(.Item("TESUU_A1_T"))
                lngFURI_TESUU2 = CLng(.Item("TESUU_A2_T"))
                lngFURI_TESUU3 = CLng(.Item("TESUU_A3_T"))
                lngFURI_TESUU4 = CLng(.Item("TESUU_B1_T"))
                lngFURI_TESUU5 = CLng(.Item("TESUU_B2_T"))


                '手数料徴求方法判定
                Select Case (CInt(.Item("TESUUTYO_PATN_T")))
                    Case 0, 1
                    Case Else
                        'Goto マスタ設定エラー
                        sErrorMessage = "決済情報に誤りがあります(手数料徴求方法)" & vbCrLf

                        GoTo ERROR_MAST_SET
                End Select

                '決済区分判定
                Select Case (CInt(.Item("KESSAI_KBN_T")))
                    Case 99
                        'Goto Next While
                        GoTo NEXT_RECORD
                    Case Else
                        lRecord_Count += 1
                End Select

                '別段出金電文作成
                '但し入金の場合は別段入金
                If Trim(sEsc_Gakkou_Code) <> Trim(.Item("GAKKOU_CODE_S")) Then
                    sEsc_Gakkou_Code = Trim(.Item("GAKKOU_CODE_S"))
                    sEsc_Furi_Kbn = Trim(.Item("FURI_KBN_S")) '比較振替区分

                    If strFURI_CHK_GAKKOU = "" Then '一回のみ
                        strFURI_CHK_GAKKOU = .Item("GAKKOU_CODE_S")
                        strGAKKOU_NAME = .Item("GAKKOU_KNAME_G")
                        strTESUU_KIN = .Item("TESUUTYOKIN_NO_T")
                        strTESUU_SIT = .Item("TESUUTYO_SIT_T")
                        strTESUU_KAMOKU = .Item("TESUUTYO_KAMOKU_T  ")
                        strTESUU_KOUZA = .Item("TESUUTYO_KOUZA_T ")
                    End If


                    '↓↓↓振替・郵送料および振込手数料発生時(一つ前の学校コード用電文)
                    If Trim(strFURI_CHK_GAKKOU) <> Trim(.Item("GAKKOU_CODE_S")) Then

                        '振込手数料更新 2006/04/17
                        If PFUNC_UPDATE_PrtKessai(Trim(sEsc_Gakkou_Code2), CLng(dblFURIKOMI_TESUU_ALL)) = False Then
                            'Goto 書き込みエラー
                            GoTo ERROR_RIENT_WRITE

                        End If
                        lngTESUU3 = lngTESUU3 + CLng(dblFURIKOMI_TESUU_ALL)

                        '入金金額より手数料（振替・郵送手数料＋振込手数料)が多い場合は手数料は引かない
                        If dblNYUKIN_GAK < (dblTESUU_KIN1_S + dblTESUU_KIN2_S + dblFURIKOMI_TESUU_ALL) Then
                            lngTESUU_ALL = lngTESUU_ALL - (dblTESUU_KIN1_S + dblTESUU_KIN2_S + dblFURIKOMI_TESUU_ALL)
                        Else
                            If dblTESUU_KIN1_S + dblTESUU_KIN2_S > 0 Then '手数料+郵送料
                                Select Case (intTESUUTYO_KBN_T)
                                    Case 0
                                        sLine_Data = PFUNC_Syokanjo_Nyuukin(strGAKKOU_CODE_S, _
                                                                            strGAKKOU_KNAME_G, _
                                                                            CLng(dblTESUU_KIN1_S + dblTESUU_KIN2_S), _
                                                                            strTESUUTYO_SIT_T, _
                                                                            strTESUUTYO_KAMOKU_T, _
                                                                            strTESUUTYO_KOUZA_T)


                                        sSortData = strGAKKOU_CODE_S & _
                                                    "8" & _
                                                    strTESUUTYOKIN_NO_T & _
                                                    strTESUUTYO_SIT_T & _
                                                    CInt(strTESUUTYO_KAMOKU_T) & _
                                                    strTESUUTYO_KOUZA_T

                                        sLine_Data = sSortData & sLine_Data

                                        Select Case (Str_Function_Ret)
                                            Case "OK"
                                                Call PSUB_Put_Data(iFileNo, sLine_Data)

                                                lTesuuKano_Count += 1
                                                lCreateRecord_Count += 1
                                            Case Else
                                                'Goto 書き込みエラー
                                                GoTo ERROR_RIENT_WRITE
                                        End Select
                                    Case Else
                                        'Goto マスタ設定エラー
                                        'sErrorMessage = "決済情報に誤りがあります(手数料徴求区分)" & vbCrLf

                                        'GoTo ERROR_MAST_SET
                                End Select
                            End If

                            '↓↓↓振込手数料発生時
                            If CLng(dblFURIKOMI_TESUU_ALL) > 0 Then  '振込手数料
                                ''振込手数料更新 2006/04/17
                                sLine_Data = PFUNC_Syokanjo_Nyuukin_FURIKOMI(Trim(strFURI_CHK_GAKKOU), _
                                                                    Trim(strGAKKOU_NAME), _
                                                                    CLng(dblFURIKOMI_TESUU_ALL), _
                                                                    Trim(strTESUU_SIT), _
                                                                    Trim(strTESUU_KAMOKU), _
                                                                    Trim(strTESUU_KOUZA))

                                sSortData = Trim(sEsc_Gakkou_Code2) & _
                                            "8" & _
                                            Trim(strTESUU_KIN) & _
                                            Trim(strTESUU_SIT) & _
                                            CInt(Trim(strTESUU_KAMOKU)) & _
                                            Trim(strTESUU_KOUZA)

                                sLine_Data = sSortData & sLine_Data

                                Select Case (Str_Function_Ret)
                                    Case "OK"
                                        Call PSUB_Put_Data(iFileNo, sLine_Data)

                                        lTesuuKano_Count += 1
                                        lCreateRecord_Count += 1
                                    Case Else
                                        'Goto 書き込みエラー
                                        GoTo ERROR_RIENT_WRITE
                                End Select
                            End If '振込手数料電文作成


                            '↑↑↑
                        End If

                    End If

                    dblTESUU_KIN1_S = 0
                    dblTESUU_KIN2_S = 0
                    dblTESUU_KIN3_S = 0
                    intTESUUTYO_KBN_T = 0
                    strGAKKOU_CODE_S = ""
                    strGAKKOU_KNAME_G = ""
                    strTESUUTYOKIN_NO_T = ""
                    strTESUUTYO_SIT_T = ""
                    strTESUUTYO_KAMOKU_T = ""
                    strTESUUTYO_KOUZA_T = ""


                    dblFURIKOMI_TESUU_ALL = 0 '振込手数料初期化 2006/04/15 
                    strGAKKOU_NAME = ""
                    strTESUU_KIN = ""
                    strTESUU_SIT = ""
                    strTESUU_KAMOKU = ""
                    strTESUU_KOUZA = ""

                    strFURI_CHK_GAKKOU = .Item("GAKKOU_CODE_S")
                    strGAKKOU_NAME = .Item("GAKKOU_KNAME_G")
                    strTESUU_KIN = .Item("TESUUTYOKIN_NO_T")
                    strTESUU_SIT = .Item("TESUUTYO_SIT_T")
                    strTESUU_KAMOKU = .Item("TESUUTYO_KAMOKU_T  ")
                    strTESUU_KOUZA = .Item("TESUUTYO_KOUZA_T ")

                    dblNYUKIN_GAK = 0 '初期化

                    'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then '随時追加 2006/08/11
                    If .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                        If .Item("FURI_KBN_S") <> "2" Then '初振・再振・臨時出金
                            sLine_Data = PFUNC_Betudan_Syukkin(Trim(.Item("GAKKOU_CODE_S")), _
                                                            Trim(.Item("GAKKOU_KNAME_G")), _
                                                            Trim(.Item("HONBU_KOUZA_T")), _
                                                            CDbl(Trim(.Item("FURI_KIN_S"))))

                            dblNYUKIN_GAK = CDbl(Trim(.Item("FURI_KIN_S")))

                        Else '自振入金時
                            sLine_Data = PFUNC_Betudan_Nyukin(Trim(.Item("GAKKOU_CODE_S")), _
                                                               Trim(.Item("GAKKOU_KNAME_G")), _
                                                               Trim(.Item("KESSAI_TENPO_H")), _
                                                               Trim(.Item("HONBU_KOUZA_T")), _
                                                               CDbl(Trim(.Item("SYORI_KIN_S"))))

                            dblNYUKIN_GAK = CDbl(Trim(.Item("SYORI_KIN_S")))
                        End If

                        lngSYORIKEN_TOKUBETU = .Item("SYORI_KEN_S")
                        dblSYORIKIN_TOKUBETU = .Item("SYORI_KIN_S")
                        lngFURIKEN_TOKUBETU = .Item("FURI_KEN_S")
                        dblFURIKIN_TOKUBETU = .Item("FURI_KIN_S")
                        lngFUNOUKEN_TOKUBETU = .Item("FUNOU_KEN_S")
                        dblFUNOUKIN_TOKUBETU = .Item("FUNOU_KIN_S")
                        dblTESUU_A1_TOKUBETU = .Item("TESUU_KIN1_S")
                        dblTESUU_A2_TOKUBETU = .Item("TESUU_KIN2_S")

                    Else
                        '複数に分かれたG_SCHMASTの処理件・金、済件・金、不件・金の合計をカウント 2006/04/17
                        If fn_COUNT_G_SCHMAST(Trim(.Item("GAKKOU_CODE_S"))) = False Then
                            'Goto 書き込みエラー
                            GoTo ERROR_RIENT_WRITE
                        End If
                        sLine_Data = PFUNC_Betudan_Syukkin(Trim(.Item("GAKKOU_CODE_S")), _
                                                           Trim(.Item("GAKKOU_KNAME_G")), _
                                                           Trim(.Item("HONBU_KOUZA_T")), _
                                                           dblFURIKIN_TOKUBETU)

                        dblNYUKIN_GAK = dblFURIKIN_TOKUBETU
                    End If

                    sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                 "1" & _
                                 STR_JIKINKO_CODE & _
                                 Trim(.Item("TSIT_NO_T")) & _
                                 "0" & _
                                 Trim(.Item("HONBU_KOUZA_T"))

                    sLine_Data = sSortData & sLine_Data

                    Select Case (Str_Function_Ret)
                        Case "OK"
                            Call PSUB_Put_Data(iFileNo, sLine_Data)
                            If .Item("FURI_KBN_S") <> "2" Then '初振・再振・臨時出金
                                lBetudan_Count += 1
                            Else
                                lBetudan_Nyu_Count += 1
                            End If
                            lCreateRecord_Count += 1
                        Case "ZERO"
                            '特別スケジュールの場合は合計値をセット 2006/04/17
                            'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                            If .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                                If .Item("TESUU_KIN_S") > 0 Then
                                    lTesuuFuka_Count += 1
                                    lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                                End If
                            Else
                                If dblTESUU_TOKUBETU > 0 Then
                                    lTesuuFuka_Count += 1
                                    lTesuuFuka_Kingaku += dblTESUU_TOKUBETU
                                End If
                            End If

                            'Goto スケジュールの更新
                            GoTo UPD_SCHEDULE
                        Case Else
                            'Goto 書き込みエラー
                            GoTo ERROR_RIENT_WRITE
                    End Select
                Else '学校コードが同一で振替区分が異なる時
                    If sEsc_Furi_Kbn <> Trim(.Item("FURI_KBN_S")) Then
                        sEsc_Gakkou_Code = Trim(.Item("GAKKOU_CODE_S"))
                        sEsc_Furi_Kbn = Trim(.Item("FURI_KBN_S")) '比較振替区分

                        'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                        If .Item("SCH_KBN_S") = "2" Then   '随時追加 2006/08/11
                            If .Item("FURI_KBN_S") <> "2" Then '初振・再振・臨時出金

                                '振替分徴求
                                sLine_Data = PFUNC_Betudan_Syukkin(Trim(.Item("GAKKOU_CODE_S")), _
                                                                    Trim(.Item("GAKKOU_KNAME_G")), _
                                                                    Trim(.Item("HONBU_KOUZA_T")), _
                                                                    CDbl(Trim(.Item("FURI_KIN_S"))))
                                dblNYUKIN_GAK = CDbl(Trim(.Item("FURI_KIN_S")))
                            Else '自振入金時
                                sLine_Data = PFUNC_Betudan_Nyukin(Trim(.Item("GAKKOU_CODE_S")), _
                                                                   Trim(.Item("GAKKOU_KNAME_G")), _
                                                                   Trim(.Item("KESSAI_TENPO_H")), _
                                                                   Trim(.Item("HONBU_KOUZA_T")), _
                                                                   CDbl(Trim(.Item("SYORI_KIN_S"))))
                                dblNYUKIN_GAK = CDbl(Trim(.Item("SYORI_KIN_S")))
                            End If

                            lngSYORIKEN_TOKUBETU = .Item("SYORI_KEN_S")
                            dblSYORIKIN_TOKUBETU = .Item("SYORI_KIN_S")
                            lngFURIKEN_TOKUBETU = .Item("FURI_KEN_S")
                            dblFURIKIN_TOKUBETU = .Item("FURI_KIN_S")
                            lngFUNOUKEN_TOKUBETU = .Item("FUNOU_KEN_S")
                            dblFUNOUKIN_TOKUBETU = .Item("FUNOU_KIN_S")


                        Else
                            '複数に分かれたG_SCHMASTの処理件・金、済件・金、不件・金の合計をカウント 2006/04/17
                            If fn_COUNT_G_SCHMAST(Trim(.Item("GAKKOU_CODE_S"))) = False Then
                                'Goto 書き込みエラー
                                GoTo ERROR_RIENT_WRITE
                            End If
                            sLine_Data = PFUNC_Betudan_Syukkin(Trim(.Item("GAKKOU_CODE_S")), _
                                                               Trim(.Item("GAKKOU_KNAME_G")), _
                                                               Trim(.Item("HONBU_KOUZA_T")), _
                                                               dblFURIKIN_TOKUBETU)
                            dblNYUKIN_GAK = dblFURIKIN_TOKUBETU
                        End If


                        sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                     "1" & _
                                     STR_JIKINKO_CODE & _
                                     Trim(.Item("TSIT_NO_T")) & _
                                     "0" & _
                                     Trim(.Item("HONBU_KOUZA_T"))

                        sLine_Data = sSortData & sLine_Data

                        Select Case (Str_Function_Ret)
                            Case "OK"
                                Call PSUB_Put_Data(iFileNo, sLine_Data)
                                If .Item("FURI_KBN_S") <> "2" Then '初振・再振・臨時出金
                                    lBetudan_Count += 1
                                Else
                                    lBetudan_Nyu_Count += 1
                                End If
                                lCreateRecord_Count += 1
                            Case "ZERO"
                                '特別スケジュールの場合は合計値をセット 2006/04/17
                                'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                                If .Item("SCH_KBN_S") = "2" Then   '随時追加 2006/08/11
                                    If .Item("TESUU_KIN_S") > 0 Then
                                        lTesuuFuka_Count += 1
                                        lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                                    End If
                                Else
                                    If dblTESUU_TOKUBETU > 0 Then
                                        lTesuuFuka_Count += 1
                                        lTesuuFuka_Kingaku += dblTESUU_TOKUBETU
                                    End If
                                End If

                                'Goto スケジュールの更新
                                GoTo UPD_SCHEDULE
                            Case Else
                                'Goto 書き込みエラー
                                GoTo ERROR_RIENT_WRITE
                        End Select
                    End If
                End If 'END 別段出金電文作成条件

                If IsDBNull(.Item("KESSAI_MEIGI_H")) = False Then
                    Str_Meigi = Trim(.Item("KESSAI_MEIGI_H"))
                Else
                    Str_Meigi = ""
                End If

                If IsDBNull(.Item("DENPYO_BIKOU1_T")) = False Then
                    Str_Biko1 = Trim(.Item("DENPYO_BIKOU1_T"))
                Else
                    Str_Biko1 = ""
                End If

                If IsDBNull(.Item("DENPYO_BIKOU2_T")) = False Then
                    Str_Biko2 = Trim(.Item("DENPYO_BIKOU2_T"))
                Else
                    Str_Biko2 = ""
                End If

                Call PSUB_GET_GINKONAME(.Item("KESSAI_KIN_CODE_H"), .Item("KESSAI_TENPO_H"))

                Str_Prt_KESSAI_KIN_KNAME = Str_Ginko(0)
                Str_Prt_KESSAI_KIN_NNAME = Str_Ginko(2)
                Str_Prt_KESSAI_SIT_KNAME = Str_Ginko(1)
                Str_Prt_KESSAI_SIT_NNAME = Str_Ginko(3)

                Str_Prt_GAKKOU_CODE = .Item("GAKKOU_CODE_S")
                Str_Prt_GAKKOU_KNAME = .Item("GAKKOU_KNAME_G")
                Str_Prt_GAKKOU_NNAME = .Item("GAKKOU_NNAME_G")

                Str_Prt_HONBU_KOUZA = .Item("HONBU_KOUZA_T")

                Str_Prt_KESSAI_DATE = .Item("KESSAI_YDATE_S")
                '振替日 2006/04/18
                Str_Prt_FURI_DATE = .Item("FURI_DATE_S")

                '特別スケジュールの場合は合計値をセット 2006/04/17
                'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                If .Item("SCH_KBN_S") = "2" Then   '随時追加 2006/08/11
                    Str_Prt_TESUURYO_JIFURI = .Item("TESUU_KIN1_S")
                    Str_Prt_TESUURYO_FURI = .Item("TESUU_KIN3_S") '振込手数料
                    Str_Prt_TESUURYO_ETC = .Item("TESUU_KIN2_S") '郵送料
                Else
                    Str_Prt_TESUURYO_JIFURI = dblTESUU_A1_TOKUBETU
                    Str_Prt_TESUURYO_FURI = dblTESUU_A3_TOKUBETU '振込手数料
                    Str_Prt_TESUURYO_ETC = dblTESUU_A2_TOKUBETU '郵送料
                End If
                Str_Prt_JIFURI_KOUZA = Str_Tesuuryo_Kouza
                Str_Prt_FURI_KOUZA = Str_Tesuuryo_Kouza3 '振込手数料
                Str_Prt_ETC_KOUZA = Str_Tesuuryo_Kouza2 '郵送料

                Str_Prt_HIMOKU_NNAME = .Item("HIMOKU_NAME_H")

                '手数料徴求区分求める 2005/06/20
                'Call PSUB_GET_TESUUTYO_KBN_T(Str_Prt_GAKKOU_CODE)

                Str_Prt_KESSAI_KIN_CODE = .Item("KESSAI_KIN_CODE_H")
                Str_Prt_KESSAI_TENPO = .Item("KESSAI_TENPO_H")
                Str_Prt_KESSAI_KAMOKU = CInt(Trim(.Item("KESSAI_KAMOKU_H")))
                Str_Prt_KESSAI_KOUZA = .Item("KESSAI_KOUZA_H")

                Str_Prt_FURI_KBN = CStr(.Item("FURI_KBN_H")) '振替区分追加 2005/06/28

                'G_SCHMASTで入出金区分が入金の場合は帳票には処理金額・件数を表示
                If CStr(.Item("FURI_KBN_S")) = "2" Then '入金
                    Str_Prt_NYUKIN_KINGAKU = CLng(.Item("HIMOKU_KINGAKU_H"))
                    '自振入金時の電文作成後、スケジュール更新処理を行う
                    '為替請求→スケジュール更新
                    '為替請求
                    Str_Prt_KESSAI_PATERN = 5

                    sLine_Data = PFUNC_Kawase_Seikyu(Trim(.Item("GAKKOU_CODE_S")), _
                                                    Trim(.Item("GAKKOU_KNAME_G")), _
                                                    Trim(.Item("KESSAI_KIN_CODE_H")), _
                                                    Trim(.Item("KESSAI_TENPO_H")), _
                                                    Str_Prt_NYUKIN_KINGAKU, _
                                                    Trim(Str_Biko1), _
                                                    Trim(Str_Biko2))

                    sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                "2" & _
                                Trim(.Item("KESSAI_KIN_CODE_H")) & _
                                Trim(.Item("KESSAI_TENPO_H")) & _
                                CInt(Trim(.Item("KESSAI_KAMOKU_H"))) & _
                                Trim(.Item("KESSAI_KOUZA_H"))

                    Str_Prt_KESSAI_DATA = sLine_Data

                    sLine_Data = sSortData & sLine_Data

                    sLine_Data = sLine_Data

                    Select Case (Str_Function_Ret)
                        Case "OK"
                            If fn_select_G_PRTKESSAI() = False Then '登録チェック 2006/04/17
                                GoTo ERROR_RIENT_WRITE
                            Else
                                If lngG_PRTKESSAI_REC = 0 Then '登録されていなかったら 2006/04/17
                                    Call PSUB_Put_Data(iFileNo, sLine_Data)

                                    '為替振込以外は手数料0円固定 2006/04/15
                                    Str_Prt_TESUURYO_FURI = 0

                                    '決済帳票データ登録
                                    If PFUNC_InsertPrtKessai() = False Then
                                        GoTo ERROR_RIENT_WRITE
                                    End If

                                    lKawase_Seikyu_Count += 1
                                    lCreateRecord_Count += 1

                                End If
                            End If

                        Case "ZERO"
                            lKawase_Seikyu_Count += 1
                            lCreateRecord_Count += 1

                            'lTesuuFuka_Count += 1
                            'lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                        Case Else
                            'Goto 書き込みエラー
                            GoTo ERROR_RIENT_WRITE
                    End Select


                    GoTo UPD_SCHEDULE '自振入金の電文は別段入金&為替請求のみ
                Else '入金以外
                    '振替済み金額で入金取引データを作成する
                    Str_Prt_NYUKIN_KINGAKU = CLng(.Item("HIMOKU_FURI_KIN_H"))
                End If

                '費目に金融機関が設定されているもののみを処理対象とする
                If Trim(.Item("KESSAI_KIN_CODE_H")) <> "" Then
                    '費目に設定されている金融機関が自行かどうかで決済区分を判定する
                    If Trim(.Item("KESSAI_KIN_CODE_H")) = STR_JIKINKO_CODE Then
                        Select Case (CInt(Trim(.Item("KESSAI_KAMOKU_H"))))
                            Case 1
                                '普通

                                Str_Prt_KESSAI_PATERN = 1

                                sLine_Data = PFUNC_Futuu_Nyuukin(Trim(.Item("GAKKOU_CODE_S")), _
                                                                Trim(.Item("GAKKOU_KNAME_G")), _
                                                                Trim(.Item("KESSAI_KOUZA_H")), _
                                                                Str_Prt_NYUKIN_KINGAKU, _
                                                                Trim(.Item("KESSAI_TENPO_H")), _
                                                                Trim(Str_Meigi), _
                                                                Trim(.Item("FURI_DATE_S")), _
                                                                Trim(.Item("TESUUTYO_PATN_T")))


                                sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                            "2" & _
                                            Trim(.Item("KESSAI_KIN_CODE_H")) & _
                                            Trim(.Item("KESSAI_TENPO_H")) & _
                                            CInt(Trim(.Item("KESSAI_KAMOKU_H"))) & _
                                            Trim(.Item("KESSAI_KOUZA_H"))

                                Str_Prt_KESSAI_DATA = sLine_Data

                                sLine_Data = sSortData & sLine_Data

                                sLine_Data = sLine_Data

                                Select Case (Str_Function_Ret)
                                    Case "OK"

                                        If fn_select_G_PRTKESSAI() = False Then '登録チェック 2006/04/17
                                            GoTo ERROR_RIENT_WRITE
                                        Else
                                            If lngG_PRTKESSAI_REC = 0 Then '登録されていなかったら 2006/04/17
                                                Call PSUB_Put_Data(iFileNo, sLine_Data)

                                                '為替振込以外は手数料0円固定 2006/04/15
                                                Str_Prt_TESUURYO_FURI = 0

                                                '決済帳票データ登録
                                                If PFUNC_InsertPrtKessai() = False Then
                                                    GoTo ERROR_RIENT_WRITE
                                                End If

                                                lFutuu_Count += 1
                                                lCreateRecord_Count += 1

                                            End If
                                        End If

                                    Case "ZERO"
                                        lFutuu_Count += 1
                                        lCreateRecord_Count += 1

                                        '特別スケジュールの場合は合計値をセット 2006/04/17
                                        'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                                        If .Item("SCH_KBN_S") = "2" Then   '随時追加 2006/08/11
                                            If .Item("TESUU_KIN_S") > 0 Then
                                                lTesuuFuka_Count += 1
                                                lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                                            End If
                                        Else
                                            If dblTESUU_TOKUBETU > 0 Then
                                                lTesuuFuka_Count += 1
                                                lTesuuFuka_Kingaku += dblTESUU_TOKUBETU
                                            End If
                                        End If

                                    Case Else
                                        'Goto 書き込みエラー
                                        GoTo ERROR_RIENT_WRITE
                                End Select


                            Case 2
                                '当座
                                Str_Prt_KESSAI_PATERN = 2

                                sLine_Data = PFUNC_Touza_Nyuukin(Trim(.Item("GAKKOU_CODE_S")), _
                                          Trim(.Item("GAKKOU_KNAME_G")), _
                                          Trim(.Item("KESSAI_KOUZA_H")), _
                                          Str_Prt_NYUKIN_KINGAKU, _
                                          Trim(.Item("KESSAI_TENPO_H")), _
                                          Trim(Str_Meigi), _
                                          Trim(.Item("FURI_DATE_S")), _
                                          Trim(.Item("TESUUTYO_PATN_T")))

                                sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                            "2" & _
                                            Trim(.Item("KESSAI_KIN_CODE_H")) & _
                                            Trim(.Item("KESSAI_TENPO_H")) & _
                                            CInt(Trim(.Item("KESSAI_KAMOKU_H"))) & _
                                            Trim(.Item("KESSAI_KOUZA_H"))

                                Str_Prt_KESSAI_DATA = sLine_Data

                                sLine_Data = sSortData & sLine_Data

                                Select Case (Str_Function_Ret)
                                    Case "OK"

                                        If fn_select_G_PRTKESSAI() = False Then '登録チェック 2006/04/17
                                            GoTo ERROR_RIENT_WRITE
                                        Else
                                            If lngG_PRTKESSAI_REC = 0 Then '登録されていなかったら 2006/04/17
                                                Call PSUB_Put_Data(iFileNo, sLine_Data)

                                                '為替振込以外は手数料0円固定 2006/04/15
                                                Str_Prt_TESUURYO_FURI = 0

                                                '決済帳票データ登録
                                                If PFUNC_InsertPrtKessai() = False Then
                                                    GoTo ERROR_RIENT_WRITE
                                                End If

                                                lTouza_Count += 1
                                                lCreateRecord_Count += 1

                                            End If
                                        End If


                                    Case "ZERO"
                                        lTouza_Count += 1
                                        lCreateRecord_Count += 1

                                        '特別スケジュールの場合は合計値をセット 2006/04/17
                                        'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then '随時追加 2006/08/11
                                        If .Item("SCH_KBN_S") = "2" Then '随時追加 2006/08/11
                                            If .Item("TESUU_KIN_S") > 0 Then
                                                lTesuuFuka_Count += 1
                                                lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                                            End If
                                        Else
                                            If dblTESUU_TOKUBETU > 0 Then
                                                lTesuuFuka_Count += 1
                                                lTesuuFuka_Kingaku += dblTESUU_TOKUBETU
                                            End If
                                        End If
                                    Case Else
                                        'Goto 書き込みエラー
                                        GoTo ERROR_RIENT_WRITE
                                End Select
                        End Select
                    Else

                        '為替振込
                        Str_Prt_KESSAI_PATERN = 3

                        '決済口座番号追加 2006/04/24
                        sLine_Data = PFUNC_Kawase_FuriKomi(Trim(.Item("GAKKOU_CODE_S")), _
                                                            Trim(.Item("GAKKOU_KNAME_G")), _
                                                            Trim(.Item("KESSAI_KIN_CODE_H")), _
                                                            Trim(.Item("KESSAI_TENPO_H")), _
                                                            Trim(.Item("KESSAI_KAMOKU_H")), _
                                                            Trim(.Item("KESSAI_KOUZA_H")), _
                                                            Trim(Str_Meigi), _
                                                            Str_Prt_NYUKIN_KINGAKU, _
                                                            Trim(Str_Biko1), _
                                                            Trim(Str_Biko2))

                        sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                    "2" & _
                                    Trim(.Item("KESSAI_KIN_CODE_H")) & _
                                    Trim(.Item("KESSAI_TENPO_H")) & _
                                    CInt(Trim(.Item("KESSAI_KAMOKU_H"))) & _
                                    Trim(.Item("KESSAI_KOUZA_H"))

                        Str_Prt_KESSAI_DATA = sLine_Data

                        sLine_Data = sSortData & sLine_Data

                        Select Case (Str_Function_Ret)
                            Case "OK"

                                If fn_select_G_PRTKESSAI() = False Then '登録チェック 2006/04/17
                                    GoTo ERROR_RIENT_WRITE
                                Else
                                    If lngG_PRTKESSAI_REC = 0 Then '登録されていなかったら 2006/04/17
                                        Call PSUB_Put_Data(iFileNo, sLine_Data)

                                        '為替振込の時は振り込み手数料計算 2006/04/15
                                        Str_Prt_TESUURYO_FURI = 0 '初期化

                                        'iTableID=0(基準値しか設定できないので)固定
                                        Str_Prt_TESUURYO_FURI = PFUNC_Get_FuriTesuryo(STR_JIKINKO_CODE, Str_Prt_NYUKIN_KINGAKU, .Item("FURI_KEN_S"), 0)
                                        dblFURIKOMI_TESUU_ALL = dblFURIKOMI_TESUU_ALL + CDbl(Str_Prt_TESUURYO_FURI)

                                        '決済帳票データ登録
                                        If PFUNC_InsertPrtKessai() = False Then
                                            GoTo ERROR_RIENT_WRITE
                                        End If

                                        lKawase_Count += 1
                                        lCreateRecord_Count += 1
                                    End If
                                End If

                            Case "ZERO"
                                lKawase_Count += 1
                                lCreateRecord_Count += 1

                                '特別スケジュールの場合は合計値をセット 2006/04/17
                                'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                                If .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                                    If .Item("TESUU_KIN_S") > 0 Then
                                        lTesuuFuka_Count += 1
                                        lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                                    End If
                                Else
                                    If dblTESUU_TOKUBETU > 0 Then
                                        lTesuuFuka_Count += 1
                                        lTesuuFuka_Kingaku += dblTESUU_TOKUBETU
                                    End If
                                End If

                            Case Else
                                'Goto 書き込みエラー
                                GoTo ERROR_RIENT_WRITE
                        End Select

                    End If
                End If


                '諸勘定連動入金（振替手数料＋郵送料)
                If Trim(sEsc_Gakkou_Code2) <> Trim(.Item("GAKKOU_CODE_S")) Then

                    sEsc_Gakkou_Code2 = Trim(.Item("GAKKOU_CODE_S"))
                    sEsc_Furi_Kbn2 = Trim(.Item("FURI_KBN_S")) '比較振替区分

                    '特別スケジュールの場合は合計値をセット 2006/04/17
                    'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                    If .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                        If CLng(.Item("TESUU_KIN1_S")) + CLng(.Item("TESUU_KIN2_S")) > 0 Then '手数料+郵送料
                            '帳票出力用手数料２の合計 2006/04/18
                            lngTESUU2 = lngTESUU2 + CLng(.Item("TESUU_KIN2_S"))

                            dblTESUU_KIN1_S = CDbl(.Item("TESUU_KIN1_S"))
                            dblTESUU_KIN2_S = CDbl(.Item("TESUU_KIN2_S"))
                        Else
                            dblTESUU_KIN1_S = 0
                            dblTESUU_KIN2_S = 0
                        End If
                    Else
                        If CLng(dblTESUU_A1_TOKUBETU) + CLng(dblTESUU_A2_TOKUBETU) > 0 Then '手数料+郵送料
                            '帳票出力用手数料２の合計 2006/04/18
                            lngTESUU2 = lngTESUU2 + CLng(dblTESUU_A2_TOKUBETU)

                            dblTESUU_KIN1_S = CDbl(dblTESUU_A1_TOKUBETU)
                            dblTESUU_KIN2_S = CDbl(dblTESUU_A2_TOKUBETU)
                        Else
                            dblTESUU_KIN1_S = 0
                            dblTESUU_KIN2_S = 0
                        End If
                    End If

                    intTESUUTYO_KBN_T = CInt(.Item("TESUUTYO_KBN_T"))
                    strGAKKOU_CODE_S = Trim(.Item("GAKKOU_CODE_S"))
                    strGAKKOU_KNAME_G = Trim(.Item("GAKKOU_KNAME_G"))
                    strTESUUTYOKIN_NO_T = Trim(.Item("TESUUTYOKIN_NO_T"))
                    strTESUUTYO_SIT_T = Trim(.Item("TESUUTYO_SIT_T"))
                    strTESUUTYO_KAMOKU_T = Trim(.Item("TESUUTYO_KAMOKU_T  "))
                    strTESUUTYO_KOUZA_T = Trim(.Item("TESUUTYO_KOUZA_T "))

                Else '学校コードが同一で振替区分が異なる時
                    If sEsc_Furi_Kbn2 <> Trim(.Item("FURI_KBN_S")) Then
                        sEsc_Gakkou_Code2 = Trim(.Item("GAKKOU_CODE_S"))
                        sEsc_Furi_Kbn2 = Trim(.Item("FURI_KBN_S")) '比較振替区分

                        '特別スケジュールの場合は合計値をセット 2006/04/17
                        'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                        If .Item("SCH_KBN_S") = "2" Then  '随時追加 2006/08/11
                            If CLng(.Item("TESUU_KIN1_S")) + CLng(.Item("TESUU_KIN2_S")) > 0 Then '手数料+郵送料
                                '帳票出力用手数料２の合計 2006/04/18
                                lngTESUU2 = lngTESUU2 + CLng(.Item("TESUU_KIN2_S"))

                                dblTESUU_KIN1_S = CDbl(.Item("TESUU_KIN1_S"))
                                dblTESUU_KIN2_S = CDbl(.Item("TESUU_KIN2_S"))
                            Else
                                dblTESUU_KIN1_S = 0
                                dblTESUU_KIN2_S = 0
                            End If
                        Else
                            If CLng(dblTESUU_A1_TOKUBETU) + CLng(dblTESUU_A2_TOKUBETU) > 0 Then '手数料+郵送料
                                '帳票出力用手数料２の合計 2006/04/18
                                lngTESUU2 = lngTESUU2 + CLng(dblTESUU_A2_TOKUBETU)

                                dblTESUU_KIN1_S = CDbl(dblTESUU_A1_TOKUBETU)
                                dblTESUU_KIN2_S = CDbl(dblTESUU_A2_TOKUBETU)
                            Else
                                dblTESUU_KIN1_S = 0
                                dblTESUU_KIN2_S = 0
                            End If
                        End If

                        intTESUUTYO_KBN_T = CInt(.Item("TESUUTYO_KBN_T"))
                        strGAKKOU_CODE_S = Trim(.Item("GAKKOU_CODE_S"))
                        strGAKKOU_KNAME_G = Trim(.Item("GAKKOU_KNAME_G"))
                        strTESUUTYOKIN_NO_T = Trim(.Item("TESUUTYOKIN_NO_T"))
                        strTESUUTYO_SIT_T = Trim(.Item("TESUUTYO_SIT_T"))
                        strTESUUTYO_KAMOKU_T = Trim(.Item("TESUUTYO_KAMOKU_T  "))
                        strTESUUTYO_KOUZA_T = Trim(.Item("TESUUTYO_KOUZA_T "))
                    End If

                End If 'END 諸勘定連動入金(振替手数料＋郵送料)

UPD_SCHEDULE:
                'スケジュール更新
                If PFUNC_Update_Schedule(sEsc_Gakkou_Code, Trim(.Item("FURI_DATE_S")), sSyori_Date, Trim(.Item("FURI_KBN_S"))) = False Then
                    'Goto スケジュール更新エラー
                    GoTo ERROR_UPD_SCHEDULE
                End If
NEXT_RECORD:
            End With
        End While


        '振込手数料更新 2006/04/17
        If PFUNC_UPDATE_PrtKessai(Trim(sEsc_Gakkou_Code2), CLng(dblFURIKOMI_TESUU_ALL)) = False Then
            'Goto 書き込みエラー
            GoTo ERROR_RIENT_WRITE

        End If
        lngTESUU3 = lngTESUU3 + CLng(dblFURIKOMI_TESUU_ALL)

        '入金金額より手数料（振替・郵送手数料＋振込手数料)が多い場合は手数料は引かない
        If dblNYUKIN_GAK < (dblTESUU_KIN1_S + dblTESUU_KIN2_S + dblFURIKOMI_TESUU_ALL) Then
            lngTESUU_ALL = lngTESUU_ALL - (dblTESUU_KIN1_S + dblTESUU_KIN2_S + dblFURIKOMI_TESUU_ALL)
        Else

            '↓↓↓振替・郵送料および振込手数料発生時(一つ前の学校コード用電文)
            If dblTESUU_KIN1_S + dblTESUU_KIN2_S > 0 Then '手数料+郵送料
                Select Case (intTESUUTYO_KBN_T)
                    Case 0
                        sLine_Data = PFUNC_Syokanjo_Nyuukin(strGAKKOU_CODE_S, _
                                                            strGAKKOU_KNAME_G, _
                                                            CLng(dblTESUU_KIN1_S + dblTESUU_KIN2_S), _
                                                            strTESUUTYO_SIT_T, _
                                                            strTESUUTYO_KAMOKU_T, _
                                                            strTESUUTYO_KOUZA_T)


                        sSortData = strGAKKOU_CODE_S & _
                                    "8" & _
                                    strTESUUTYOKIN_NO_T & _
                                    strTESUUTYO_SIT_T & _
                                    CInt(strTESUUTYO_KAMOKU_T) & _
                                    strTESUUTYO_KOUZA_T

                        sLine_Data = sSortData & sLine_Data

                        Select Case (Str_Function_Ret)
                            Case "OK"
                                Call PSUB_Put_Data(iFileNo, sLine_Data)

                                lTesuuKano_Count += 1
                                lCreateRecord_Count += 1
                            Case Else
                                'Goto 書き込みエラー
                                GoTo ERROR_RIENT_WRITE
                        End Select
                    Case Else
                        'Goto マスタ設定エラー
                        'sErrorMessage = "決済情報に誤りがあります(手数料徴求区分)" & vbCrLf

                        'GoTo ERROR_MAST_SET
                End Select
            End If


            '振り込み手数料分の電文　各学校に１電文作成
            If CLng(dblFURIKOMI_TESUU_ALL) > 0 Then '振込手数料

                ''振込手数料更新 2006/04/17
                'If PFUNC_UPDATE_PrtKessai(Trim(sEsc_Gakkou_Code2), CLng(dblFURIKOMI_TESUU_ALL)) = False Then
                '    'Goto 書き込みエラー
                '    GoTo ERROR_RIENT_WRITE

                'End If

                'lngTESUU3 = lngTESUU3 + CLng(dblFURIKOMI_TESUU_ALL)

                sLine_Data = PFUNC_Syokanjo_Nyuukin_FURIKOMI(Trim(sEsc_Gakkou_Code2), _
                                                    Trim(strGAKKOU_NAME), _
                                                    CLng(dblFURIKOMI_TESUU_ALL), _
                                                    Trim(strTESUU_SIT), _
                                                    Trim(strTESUU_KAMOKU), _
                                                    Trim(strTESUU_KOUZA))

                sSortData = Trim(sEsc_Gakkou_Code2) & _
                            "8" & _
                            Trim(strTESUU_KIN) & _
                            Trim(strTESUU_SIT) & _
                            CInt(Trim(strTESUU_KAMOKU)) & _
                            Trim(strTESUU_KOUZA)

                sLine_Data = sSortData & sLine_Data

                Select Case (Str_Function_Ret)
                    Case "OK"
                        Call PSUB_Put_Data(iFileNo, sLine_Data)

                        lTesuuKano_Count += 1
                        lCreateRecord_Count += 1
                    Case Else
                        'Goto 書き込みエラー
                        GoTo ERROR_RIENT_WRITE
                End Select
            End If '振込手数料電文作成
            '↑↑↑
        End If

        '決済スケジュール数チェック
        If lngKESSAIREC = 0 Then
            Call GSUB_MESSAGE_WARNING("処理するデータがありません")
            GoTo ERROR_HANDLER
        End If

        'カウントチェック
        If lRecord_Count = 0 Then
            Call GSUB_MESSAGE_WARNING("処理するデータがありません")
            GoTo ERROR_HANDLER
        End If

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        FileClose(iFileNo)

        '総合計件数・金額等を取得
        If fn_select_G_SCHMAST() = False Then
            Exit Function
        Else
            '手数料総合計算出 2006/04/18
            lngTESUU_ALL = lngTESUU_ALL + (lngTESUU1 + lngTESUU2 + lngTESUU3)
        End If

        Dim strDIR As String
        strDIR = CurDir()

        'Dim CRXApplication As New CRAXDDRT.Application
        'Dim CRXReport As CRAXDDRT.Report
        'Dim CPProperty As CRAXDDRT.ConnectionProperty
        'Dim DBTable As CRAXDDRT.DatabaseTable

        'CRXReport = CRXApplication.OpenReport(STR_LST_PATH & "資金決済学校一覧表.RPT", 1)

        'DBTable = CRXReport.Database.Tables(1)

        'CPProperty = DBTable.ConnectionProperties("Password")
        'CPProperty.Value = "KZFMAST"
        'CRXReport.RecordSelectionFormula = ""

        'Dim CRX_FORMULA As CRAXDDRT.FormulaFieldDefinition
        'Dim strFORMULA_NAME As String
        'For i As Integer = 1 To CRXReport.FormulaFields.Count
        '    CRX_FORMULA = CRXReport.FormulaFields.Item(i)
        '    strFORMULA_NAME = CRX_FORMULA.FormulaFieldName
        '    Select Case (strFORMULA_NAME)
        '        Case "総合計件数"
        '            CRX_FORMULA.Text = CStr(lngALL_KEN)
        '        Case "総合計金額"
        '            CRX_FORMULA.Text = CStr(lngALL_KIN)
        '        Case "総振替件数"
        '            CRX_FORMULA.Text = CStr(lngFURI_ALL_KEN)
        '        Case "総振替金額"
        '            CRX_FORMULA.Text = CStr(lngFURI_ALL_KIN)
        '        Case "総不能件数"
        '            CRX_FORMULA.Text = CStr(lngFUNOU_ALL_KEN)
        '        Case "総不能金額"
        '            CRX_FORMULA.Text = CStr(lngFUNOU_ALL_KIN)
        '        Case "総手数料合計"
        '            CRX_FORMULA.Text = CStr(lngTESUU_ALL)
        '        Case "総手数料1"
        '            CRX_FORMULA.Text = CStr(lngTESUU1)
        '        Case "総手数料3" '郵送料
        '            CRX_FORMULA.Text = CStr(lngTESUU2)
        '        Case "総手数料2" '振込手数料
        '            CRX_FORMULA.Text = CStr(lngTESUU3)

        '    End Select
        'Next

        'CRXReport.PrintOut(False, 1)

        ChDir(strDIR)

        PFUNC_Write_WorkD = True

        ''完了メッセージ
        'Call GSUB_MESSAGE_INFOMATION("リエンタＦＤを作成しました" & vbCrLf & _
        '                                 "作成レコード件数  = " & lCreateRecord_Count & vbCrLf & _
        '                                 "  別段出金          = " & lBetudan_Count & vbCrLf & _
        '                                 "  預け金             = " & "0" & vbCrLf & _
        '                                 "  普通入金          = " & lFutuu_Count & vbCrLf & _
        '                                 "  当座入金          = " & lTouza_Count & vbCrLf & _
        '                                 "  別段入金          = " & "0" & vbCrLf & _
        '                                 "  為替振込          = " & lKawase_Count & vbCrLf & _
        '                                 "  為替付替          = " & "0" & vbCrLf & _
        '                                 "  為替請求          = " & "0" & vbCrLf & _
        '                                 "  手数料徴求       = " & lTesuuKano_Count & vbCrLf & _
        '                                 "  手数料徴求不可 = " & lTesuuFuka_Count)

        Exit Function

ERROR_RIENT_WRITE:
        sErrorMessage = "リエンタデータ書き込みに失敗しました" & vbCrLf
        sErrorMessage += " 学校名　　　　 = " & Str_Err_Gakkou_Name & vbCrLf
        sErrorMessage += " 決済区分　　　 = " & Str_Err_Kessai_Kbn & vbCrLf
        sErrorMessage += " 手数料徴求方法 = " & Str_Err_Tesuutyo_Kbn & vbCrLf
        sErrorMessage += " 手数料徴求区分 = " & Str_Err_Tesuutyo_Kbn & vbCrLf
        sErrorMessage += " 請求金額　　　 = " & "" & vbCrLf
        sErrorMessage += " 振替済金額　　 = " & "" & vbCrLf
        sErrorMessage += " 手数料金額　　 = " & "" & vbCrLf
        sErrorMessage += " エラー情報　　 = " & Err.Description

        GoTo ERROR_HANDLER
ERROR_MAST_SET:
        sErrorMessage += " 学校名　　　　 = " & Str_Err_Gakkou_Name & vbCrLf
        sErrorMessage += " 決済区分　　　 = " & Str_Err_Kessai_Kbn & vbCrLf
        sErrorMessage += " 手数料徴求方法 = " & Str_Err_Tesuutyo_Kbn & vbCrLf
        sErrorMessage += " 手数料徴求区分 = " & Str_Err_Tesuutyo_Kbn & vbCrLf
        sErrorMessage += " 口座科目　　　 = " & "" & vbCrLf

        GoTo ERROR_HANDLER
ERROR_UPD_SCHEDULE:
        sErrorMessage = "スケジュール更新に失敗しました" & vbCrLf
        sErrorMessage += " 学校名　　　　 = " & Str_Err_Gakkou_Name & vbCrLf
        sErrorMessage += " 振替日　　　　 = " & "" & vbCrLf
        sErrorMessage += " エラー情報　　 = " & Err.Description

        GoTo ERROR_HANDLER
ERROR_HANDLER:
        If Trim(sErrorMessage) <> "" Then
            Call GSUB_MESSAGE_WARNING(sErrorMessage)
        End If

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        FileClose(iFileNo)

    End Function
    Private Function PFUNC_Betudan_Syukkin(ByVal pGakkou_Code As String, _
                                           ByVal pGakkou_Name As String, _
                                           ByVal pHonbu_Kouza As String, _
                                           ByVal pFurikae_Kingaku As Double) As String

        Dim FD_WORK_D As String
        'Betudan_Syukkin = INDEX内
        ' 0 = 科目コード
        ' 1 = 取引コード
        ' 2 = 口座番号
        ' 3 = 金額
        ' 4 = 振替コード
        ' 5 = 企業コード
        ' 6 = 摘要
        ' 7 = 取扱番号1
        ' 8 = 件数
        ' 9 = 手形小切手番号
        '10 = 原店番号
        '11 = 起算日
        '12 = 予備1
        Dim Betudan_Syukkin(12) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Betudan_Syukkin(0) = "04"
        Betudan_Syukkin(1) = "099"

        If Format(pHonbu_Kouza) = 0 Then
            Call GSUB_MESSAGE_WARNING("別段口座番号が設定されていません" & vbCrLf & "学校コード = " & pGakkou_Code & vbCrLf & "資金決済用リエンタＦＤ作成（別段処理）")
            Return Str_Function_Ret
        End If

        Betudan_Syukkin(2) = Format(CInt(pHonbu_Kouza), "0000000")
        Betudan_Syukkin(3) = Format(pFurikae_Kingaku, "0000000000")

        '振替済金額が０円時はここで処理を終了
        If CLng(Betudan_Syukkin(3)) = 0 Then
            Str_Function_Ret = "ZERO"
            Return Str_Function_Ret
        End If

        Betudan_Syukkin(4) = Space(3)

        If Trim(pGakkou_Name) = "" Then
            Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "学校コード = " & pGakkou_Code & vbCrLf & "学校名 = " & pGakkou_Name & vbCrLf & "摘要がありません")
            Return Str_Function_Ret
        End If

        Betudan_Syukkin(5) = Space(3)
        Betudan_Syukkin(6) = Mid(Trim(pGakkou_Name) & Space(16), 1, 16)
        Betudan_Syukkin(7) = Space(5)
        Betudan_Syukkin(8) = Space(4)
        Betudan_Syukkin(9) = Space(6)
        Betudan_Syukkin(10) = Space(3)
        Betudan_Syukkin(11) = Space(6)
        Betudan_Syukkin(12) = Space(212)

        For iCount = LBound(Betudan_Syukkin) To UBound(Betudan_Syukkin)
            FD_WORK_D += Betudan_Syukkin(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Betudan_Syukkin = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "学校名 : " & pGakkou_Name & vbCrLf & "作成したデータサイズが違います : " & Len(FD_WORK_D) & "バイト")
            Return Str_Function_Ret
        End If

        Str_Function_Ret = "OK"

        Return Str_Function_Ret

    End Function
    '別段入金 2005/06/28
    Private Function PFUNC_Betudan_Nyukin(ByVal pGakkou_Code As String, _
                                           ByVal pGakkou_Name As String, _
                                           ByVal pTukekae_Sit As String, _
                                           ByVal pHonbu_Kouza As String, _
                                           ByVal pNyukin_Kingaku As Double) As String

        Dim FD_WORK_D As String
        'PFUNC_Betudan_Nyukin = INDEX内
        ' 0 = 科目コード*2
        ' 1 = 取引コード*3
        ' 2 = 口座番号*7
        ' 3 = 金額*10
        ' 4 = 資金化区分コード*1
        ' 5 = 他店券摘要*2
        ' 6 = 振替コード*3
        ' 7 = 企業コード*3
        ' 8 = 摘要*16
        ' 9 = 取扱番号1*5
        '10 = 件数*4
        '11 = 窓口収納区分*1
        '12 = 印紙件数*3
        '13 = 手形小切手番号*6
        '14 = 発行先顧客番号*7
        '15 = 手数料徴求区分*1
        '16 = 手数料額*5
        '17 = 起算日*6
        '18 = 先日付予定日*6
        '19 = 原店番号*3
        '20 = 予備1*186
        Dim Betudan_Nyukin(20) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Betudan_Nyukin(0) = "04"
        Betudan_Nyukin(1) = "019"

        If Format(pHonbu_Kouza) = 0 Then
            Call GSUB_MESSAGE_WARNING("本部口座番号が設定されていません" & vbCrLf & "学校コード = " & pGakkou_Code & vbCrLf & "資金決済用リエンタＦＤ作成（別段処理）")
            Return Str_Function_Ret
        End If

        Betudan_Nyukin(2) = Format(CInt(pHonbu_Kouza), "0000000")
        Betudan_Nyukin(3) = Format(pNyukin_Kingaku, "0000000000")

        '企業口座入金金額が０円時はここで処理を終了
        If CLng(Betudan_Nyukin(3)) = 0 Then
            Str_Function_Ret = "ZERO"
            Return Str_Function_Ret
        End If
        '資金化区分コード
        Betudan_Nyukin(4) = Space(1)

        If Trim(pGakkou_Name) = "" Then
            Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "学校コード = " & pGakkou_Code & vbCrLf & "学校名 = " & pGakkou_Name & vbCrLf & "摘要がありません")

            Return Str_Function_Ret
        End If

        Betudan_Nyukin(5) = Space(2) '他店券摘要
        Betudan_Nyukin(6) = Space(3) '振替コード
        Betudan_Nyukin(7) = Space(3) '企業コード
        Betudan_Nyukin(8) = CStr(pGakkou_Name.Trim & Space(16)).Substring(0, 16) '摘要：委託者名カナ
        Betudan_Nyukin(9) = Space(5) '取扱番号1
        Betudan_Nyukin(10) = Space(4) '件数
        Betudan_Nyukin(11) = Space(1) '窓口収納区分
        Betudan_Nyukin(12) = Space(3) '印紙件数
        Betudan_Nyukin(13) = Space(6) '手形小切手番号
        Betudan_Nyukin(14) = Space(7) '発行元顧客番号
        Betudan_Nyukin(15) = Space(1) '手数料徴求区分
        Betudan_Nyukin(16) = Space(5) '手数料額
        Betudan_Nyukin(17) = Space(6) '起算日
        Betudan_Nyukin(18) = Space(6) '先日付予定日
        If pTukekae_Sit.Trim = "" Then
            Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "学校コード = " & pGakkou_Code & vbCrLf & "学校名 = " & pGakkou_Name & vbCrLf & "原店番号がありません")
            Return Str_Function_Ret
        Else
            If pTukekae_Sit = Str_Honbu_Code Then
                Betudan_Nyukin(19) = Space(3) '原点番号
            Else
                Betudan_Nyukin(19) = pTukekae_Sit '原点番号
            End If

        End If
        Betudan_Nyukin(20) = Space(186) '予備１

        For iCount = LBound(Betudan_Nyukin) To UBound(Betudan_Nyukin)
            FD_WORK_D += Betudan_Nyukin(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Betudan_Nyukin = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "学校名 : " & pGakkou_Name & vbCrLf & "作成したデータサイズが違います : " & Len(FD_WORK_D) & "バイト")
            Return Str_Function_Ret
        End If

        Str_Function_Ret = "OK"

        Return Str_Function_Ret

    End Function

    Private Function PFUNC_Futuu_Nyuukin(ByVal pGakkou_Code As String, _
                                         ByVal pGakkou_Name As String, _
                                         ByVal pHimoku_Kouza As String, _
                                         ByVal pHimoku_Kingaku As Long, _
                                         ByVal pHimoku_Tenpo As String, _
                                         ByVal pHimoku_Meigi As String, _
                                         ByVal pFurikae_Date As String, _
                                         ByVal pTesuuryo_kbn As String) As String
        '*********************普通入金********************************
        'UPDATE:2005/10/19 新フォーマット対応 引数にpHimoku_Meigi追加
        '*************************************************************

        Dim FD_WORK_D As String
        'Futuu_Nyuukin = INDEX内
        ' 0 = 科目コード * 2
        ' 1 = 取引コード * 3
        ' 2 = 口座番号 * 7
        ' 3 = 行 * 2
        ' 4 = 金額 * 10
        ' 5 = 資金化区分コード * 1
        ' 6 = 他店券摘要 * 2
        ' 7 = 振替コード * 3
        ' 8 = 摘要 * 13
        ' 9 = 手数料徴求区分 * 1
        '10 = 手数料額 * 5
        '11 = 起算日 * 6
        '12 = 先日付予定日 * 6
        '13 = 振込依頼人名 * 48 2005/10/19　追加
        '14 = 原店番号 * 3
        '15 = 金額1 * 10
        '16 = 資金化区分コード1 * 1
        '17 = 他店券摘要1 * 2
        '18 = 金額2 * 10
        '19 = 資金化区分コード2 * 1
        '20 = 他店券摘要2 * 2
        '21 = 金額3 * 10
        '22 = 資金化区分コード3 * 1
        '23 = 他店券摘要3 * 2
        '24 = 予備1 * 129  2005/10/19　177→129に変更
        Dim Futuu_Nyuukin(24) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Futuu_Nyuukin(0) = "02"
        Futuu_Nyuukin(1) = "019"

        If Trim(pHimoku_Kouza) = "" Then
            Call GSUB_MESSAGE_WARNING("口座番号がありません : " & Trim(pHimoku_Kouza) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Futuu_Nyuukin(2) = Format(CInt(pHimoku_Kouza), "0000000")
        Futuu_Nyuukin(3) = "01"

        Select Case (Trim(pTesuuryo_kbn))
            Case "0"
                Futuu_Nyuukin(4) = Format((pHimoku_Kingaku), "0000000000")

                If CLng(Futuu_Nyuukin(4)) <= 0 Then
                    Futuu_Nyuukin(4) = Format(pHimoku_Kingaku, "0000000000")
                    Str_Function_Ret = "ZERO"
                End If
            Case "1"
                Futuu_Nyuukin(4) = Format(pHimoku_Kingaku, "0000000000")
        End Select

        Futuu_Nyuukin(5) = Space(1)
        Futuu_Nyuukin(6) = Space(2)
        Futuu_Nyuukin(7) = "040"

        If Trim(pGakkou_Name) = "" Then
            Call GSUB_MESSAGE_WARNING("摘要がありません" & vbCrLf & "資金決済用リエンタＦＤ作成（普通入金）")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If
        '摘要
        Futuu_Nyuukin(8) = Mid(Mid(pFurikae_Date, 5, 2) & "/" & Mid(pFurikae_Date, 7, 2) & Space(1) & Trim(pGakkou_Name) & Space(13), 1, 13)
        Futuu_Nyuukin(9) = Space(1)
        Futuu_Nyuukin(10) = Space(5)
        Futuu_Nyuukin(11) = Space(6)
        Futuu_Nyuukin(12) = Space(6)

        '新フォーマット対応 2005/10/19
        Select Case (Futuu_Nyuukin(7))
            Case "040", "041", "044"
                If Futuu_Nyuukin(8) = "" Then
                    Futuu_Nyuukin(13) = pHimoku_Meigi
                Else
                    Futuu_Nyuukin(13) = Space(48)
                End If
            Case Else
                Futuu_Nyuukin(13) = Space(48)
        End Select

        If Trim(pHimoku_Tenpo) = "" Then
            Call GSUB_MESSAGE_WARNING("原店番号がありません" & vbCrLf & "資金決済用リエンタＦＤ作成（普通入金）")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If

        If Str_Honbu_Code = pHimoku_Tenpo Then
            Futuu_Nyuukin(14) = Space(3)
        Else
            Futuu_Nyuukin(14) = Format(CInt(pHimoku_Tenpo), "000")
        End If

        Futuu_Nyuukin(15) = Space(10)
        Futuu_Nyuukin(16) = Space(1)
        Futuu_Nyuukin(17) = Space(2)
        Futuu_Nyuukin(18) = Space(10)
        Futuu_Nyuukin(19) = Space(1)
        Futuu_Nyuukin(20) = Space(2)
        Futuu_Nyuukin(21) = Space(10)
        Futuu_Nyuukin(22) = Space(1)
        Futuu_Nyuukin(23) = Space(2)
        Futuu_Nyuukin(24) = Space(129)

        For iCount = LBound(Futuu_Nyuukin) To UBound(Futuu_Nyuukin)
            FD_WORK_D += Futuu_Nyuukin(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Futuu_Nyuukin = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name & vbCrLf & "作成したデータサイズが違います : " & Len(FD_WORK_D) & "バイト")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function
    Private Function PFUNC_Touza_Nyuukin(ByVal pGakkou_Code As String, _
                                         ByVal pGakkou_Name As String, _
                                         ByVal pHimoku_Kouza As String, _
                                         ByVal pHimoku_Kingaku As Long, _
                                         ByVal pHimoku_Tenpo As String, _
                                         ByVal pHimoku_Meigi As String, _
                                         ByVal pFurikae_Date As String, _
                                         ByVal pTesuuryo_kbn As String) As String

        '*********************当座入金********************************
        'UPDATE:2005/10/19 新フォーマット対応 引数にpHimoku_Meigi追加
        '*************************************************************

        Dim FD_WORK_D As String
        'Touza_Nyuukin = INDEX内
        ' 0 = 科目コード * 2
        ' 1 = 取引コード * 3
        ' 2 = 口座番号 * 7
        ' 3 = 金額 * 10
        ' 4 = 資金化区分コード * 1
        ' 5 = 他店券摘要 * 2
        ' 6 = 振替コード * 3
        ' 7 = 摘要 * 13
        ' 8 = 手数料徴求区分 * 1
        ' 9 = 手数料額 * 5
        '10 = 起算日 * 6
        '11 = 先日付予定日 * 6
        '12 = 振込依頼人名 * 48
        '13 = 原店番号 * 3
        '14 = 金額1 * 10
        '15 = 資金化区分コード1 * 1
        '16 = 他店券摘要1 * 2
        '17 = 金額2 * 10
        '18 = 資金化区分コード2 * 1
        '19 = 他店券摘要2 * 2
        '20 = 金額3 * 10
        '21 = 資金化区分コード3 * 1
        '22 = 他店券摘要3 * 2
        '23 = 予備1 * 131
        Dim Touza_Nyuukin(23) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Touza_Nyuukin(0) = "01"
        Touza_Nyuukin(1) = "010"

        If Trim(pHimoku_Kouza) = "" Then
            Call GSUB_MESSAGE_WARNING("口座番号がありません : " & Trim(pHimoku_Kouza) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)

            Return Str_Function_Ret
        End If

        Touza_Nyuukin(2) = Format(CInt(pHimoku_Kouza), "0000000")

        Select Case (Trim(pTesuuryo_kbn))
            Case "0"
                Touza_Nyuukin(3) = Format((pHimoku_Kingaku), "0000000000")

                If CLng(Touza_Nyuukin(4)) <= 0 Then
                    Touza_Nyuukin(3) = Format(pHimoku_Kingaku, "0000000000")
                    Str_Function_Ret = "ZERO"
                End If
            Case "1"
                Touza_Nyuukin(3) = Format(pHimoku_Kingaku, "0000000000")
        End Select

        Touza_Nyuukin(4) = Space(1)
        Touza_Nyuukin(5) = Space(2)
        Touza_Nyuukin(6) = "040"

        If Trim(pGakkou_Name) = "" Then
            Call GSUB_MESSAGE_WARNING("摘要がありません" & vbCrLf & "資金決済用リエンタＦＤ作成（普通入金）")
            Str_Function_Ret = "NG"

            Return Str_Function_Ret
        End If

        Touza_Nyuukin(7) = Mid(Mid(pFurikae_Date, 5, 2) & "/" & Mid(pFurikae_Date, 7, 2) & Space(1) & Trim(pGakkou_Name) & Space(13), 1, 13)
        Touza_Nyuukin(8) = Space(1)
        Touza_Nyuukin(9) = Space(5)
        Touza_Nyuukin(10) = Space(6)
        Touza_Nyuukin(11) = Space(6)

        '新フォーマット対応 2005/10/19
        Select Case (Touza_Nyuukin(6))
            Case "040", "041", "044"
                If Touza_Nyuukin(7) = "" Then
                    Touza_Nyuukin(12) = pHimoku_Meigi
                Else
                    Touza_Nyuukin(12) = Space(48)
                End If
            Case Else
                Touza_Nyuukin(12) = Space(48)
        End Select

        If Trim(pHimoku_Tenpo) = "" Then
            Call GSUB_MESSAGE_WARNING("原店番号がありません" & vbCrLf & "資金決済用リエンタＦＤ作成（普通入金）")
            Str_Function_Ret = "NG"

            Return Str_Function_Ret
        End If

        If Str_Honbu_Code = pHimoku_Tenpo Then
            Touza_Nyuukin(13) = Space(3)
        Else
            Touza_Nyuukin(13) = Format(CInt(pHimoku_Tenpo), "000")
        End If

        Touza_Nyuukin(14) = Space(10)
        Touza_Nyuukin(15) = Space(1)
        Touza_Nyuukin(16) = Space(2)
        Touza_Nyuukin(17) = Space(10)
        Touza_Nyuukin(18) = Space(1)
        Touza_Nyuukin(19) = Space(2)
        Touza_Nyuukin(20) = Space(10)
        Touza_Nyuukin(21) = Space(1)
        Touza_Nyuukin(22) = Space(2)
        Touza_Nyuukin(23) = Space(131)

        For iCount = LBound(Touza_Nyuukin) To UBound(Touza_Nyuukin)
            FD_WORK_D += Touza_Nyuukin(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Touza_Nyuukin = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name & vbCrLf & "作成したデータサイズが違います : " & Len(FD_WORK_D) & "バイト")
            Str_Function_Ret = "NG"

            Return Str_Function_Ret
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function
    Private Function PFUNC_Kawase_FuriKomi(ByVal pGakkou_Code As String, _
                                           ByVal pGakkou_Name As String, _
                                           ByVal pHimoku_Ginko As String, _
                                           ByVal pHimoku_Siten As String, _
                                           ByVal pHimoku_Kamoku As String, _
                                           ByVal pHimoku_Kouza As String, _
                                           ByVal pHimoku_Meigi As String, _
                                           ByVal pHimoku_Kingaku As Long, _
                                           ByVal pDenpyo_Biko1 As String, _
                                           ByVal pDenpyo_Biko2 As String) As String

        Dim FD_WORK_D As String
        'TKawase_Furikae = INDEX内
        ' 0 = 科目コード * 2
        ' 1 = 取引コード * 3
        ' 2 = 受信店名 * 30
        ' 3 = 発信店名 * 20
        ' 4 = 取扱日 * 6
        ' 5 = 種目 * 4
        ' 6 = 顧客手数料 * 4
        ' 7 = 金額 * 12
        ' 8 = 金額複記符号 * 15
        ' 9 = 受取人科目口番 * 15
        '10 = 受取人名 * 29
        '11 = 依頼人名 * 29
        '12 = 備考1 * 29
        '13 = 備考2 * 29
        '14 = 予備1 * 53

        Dim Kawase_Furikae(14) As String

        Dim iCount As Integer

        Dim sAngo As String = ""

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Kawase_Furikae(0) = "48"
        Kawase_Furikae(1) = "100"

        If PFUNC_GET_GINKONAME(pHimoku_Ginko, pHimoku_Siten) = False Then
            Call GSUB_MESSAGE_WARNING("金融機関コード取込エラー : " & Trim(pHimoku_Ginko) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        If STR_JIKINKO_CODE = pHimoku_Ginko Then
            Kawase_Furikae(2) = Mid(Chr(223) & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Furikae(3) = Mid(Chr(223) & Space(1) & "ｾﾝﾀｰ" & Space(20), 1, 20)
        Else
            Kawase_Furikae(2) = Mid(Str_Ginko(0).Trim & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Furikae(3) = Mid(Str_Kawase_CenterName.Trim & Space(20), 1, 20)
        End If

        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- START
        'Kawase_Furikae(4) = Trim(Str(Val(Mid(STR_FURIKAE_DATE(1), 1, 4)) - CInt(Mid(Str_KijyunDate, 1, 4)))) & Mid(STR_FURIKAE_DATE(1), 5, 4)
        Kawase_Furikae(4) = CASTCommon.GetWAREKI(Trim(STR_FURIKAE_DATE(1)), "yyMMdd")
        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- END
        Kawase_Furikae(5) = "ﾌﾘｺﾐ"
        Kawase_Furikae(6) = Space(4)
        Kawase_Furikae(7) = Format(CDbl(pHimoku_Kingaku), "000000000000")

        Select Case (pHimoku_Kingaku)
            Case Is <= 0
                Str_Function_Ret = "ZERO"
        End Select

        If PFUNC_Set_FukukiKigo(Format(CLng(Kawase_Furikae(7)), "#,##0"), sAngo) = False Then
            Call GSUB_MESSAGE_WARNING("複記符号設定処理エラー : " & Trim(Kawase_Furikae(8)) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Kawase_Furikae(8) = Mid(Trim(sAngo) & Space(15), 1, 15)

        '決済口座番号を設定 2006/04/24
        Select Case (Format(CInt(pHimoku_Kamoku), "00"))
            Case "02" '費目は02：当座 2006/04/24
                Kawase_Furikae(9) = Mid("ﾄ" & Format(Val(pHimoku_Kouza), "0000000") & Space(15), 1, 15)
            Case "01" '費目は01：普通 2006/04/24
                Kawase_Furikae(9) = Mid("ﾌ" & Format(Val(pHimoku_Kouza), "0000000") & Space(15), 1, 15)
            Case Else
                Kawase_Furikae(9) = Mid("ｿ" & Format(Val(pHimoku_Kouza), "0000000") & Space(15), 1, 15)
        End Select

        If Trim(pHimoku_Meigi) = "" Then
            Call GSUB_MESSAGE_WARNING("受取人名がありません : " & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Kawase_Furikae(10) = Mid(Trim(pHimoku_Meigi) & Space(29), 1, 29)
        Kawase_Furikae(11) = Mid(Str_IraiNinName & Space(29), 1, 29)

        If Mid(pDenpyo_Biko1, 1, 7) = "@MM-DD@" Then
            Kawase_Furikae(12) = Mid(Mid(STR_FURIKAE_DATE(1), 5, 2) & "-" & Mid(Kawase_Furikae(4), 7, 2) & Mid(pDenpyo_Biko1, 8) & Space(29), 1, 29)
        Else
            Kawase_Furikae(12) = Mid(Trim(pDenpyo_Biko1) & Space(29), 1, 29)
        End If
        If Mid(pDenpyo_Biko2, 1, 7) = "@MM-DD@" Then
            Kawase_Furikae(13) = Mid(Mid(STR_FURIKAE_DATE(1), 5, 2) & "-" & Mid(STR_FURIKAE_DATE(1), 7, 2) & Mid(pDenpyo_Biko2, 8) & Space(29), 1, 29)
        Else
            Kawase_Furikae(13) = Mid(Trim(pDenpyo_Biko2) & Space(29), 1, 29)
        End If

        Kawase_Furikae(14) = Space(53)

        For iCount = LBound(Kawase_Furikae) To UBound(Kawase_Furikae)
            FD_WORK_D += Kawase_Furikae(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Kawase_FuriKomi = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name & vbCrLf & "作成したデータサイズが違います : " & Len(FD_WORK_D) & "バイト")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function

    Private Function PFUNC_Kawase_Tukekae(ByVal pGakkou_Code As String, _
                                           ByVal pGakkou_Name As String, _
                                           ByVal pHimoku_Ginko As String, _
                                           ByVal pHimoku_Siten As String, _
                                           ByVal pHimoku_Kamoku As String, _
                                           ByVal pHimoku_Meigi As String, _
                                           ByVal pHimoku_Kingaku As Long, _
                                           ByVal pDenpyo_Biko1 As String, _
                                           ByVal pDenpyo_Biko2 As String) As String

        Dim FD_WORK_D As String
        'PFUNC_Kawase_Tukekae = INDEX内
        ' 0 = 科目コード * 2
        ' 1 = 取引コード * 3
        ' 2 = 受信店名 * 30
        ' 3 = 発信店名 * 20
        ' 4 = 取扱日 * 6
        ' 5 = 種目 * 4
        ' 6 = 金額 * 12
        ' 7 = 金額複記符号 * 15
        ' 8 = 番号 * 16
        ' 9 = 資金付替事由1 * 48
        '10 = 資金付替事由2 * 48
        '11 = 資金付替事由3 * 20
        '12 = 資金付替事由4 * 20
        '13 = 照会番号 * 15
        '14 = 予備1 * 21

        Dim Kawase_Tukekae(14) As String

        Dim iCount As Integer

        Dim sAngo As String = ""

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Kawase_Tukekae(0) = "48"
        Kawase_Tukekae(1) = "500"
        '------------------------------
        '受信店名取得
        '------------------------------
        If PFUNC_GET_GINKONAME(pHimoku_Ginko, pHimoku_Siten) = False Then
            Call GSUB_MESSAGE_WARNING("金融機関コード取込エラー : " & Trim(pHimoku_Ginko) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        If STR_JIKINKO_CODE = pHimoku_Ginko Then
            Kawase_Tukekae(2) = Mid(Chr(223) & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Tukekae(3) = Mid(Chr(223) & Space(1) & "ｾﾝﾀｰ" & Space(20), 1, 20)
        Else
            Kawase_Tukekae(2) = Mid(Str_Ginko(0).Trim & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Tukekae(3) = Mid(Str_Kawase_CenterName.Trim & Space(20), 1, 20)
        End If

        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- START
        'Kawase_Tukekae(4) = Trim(Str(Val(Mid(STR_FURIKAE_DATE(1), 1, 4)) - CInt(Mid(Str_KijyunDate, 1, 4)))) & Mid(STR_FURIKAE_DATE(1), 5, 4)
        Kawase_Tukekae(4) = CASTCommon.GetWAREKI(Trim(STR_FURIKAE_DATE(1)), "yyMMdd")
        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- END
        Kawase_Tukekae(5) = "ﾂｹｶｴ"
        Kawase_Tukekae(6) = Format(CDbl(pHimoku_Kingaku), "000000000000")

        Select Case (pHimoku_Kingaku)
            Case Is <= 0
                Str_Function_Ret = "ZERO"
        End Select

        If PFUNC_Set_FukukiKigo(Format(CLng(Kawase_Tukekae(6)), "#,##0"), sAngo) = False Then
            Call GSUB_MESSAGE_WARNING("複記符号設定処理エラー : " & Trim(Kawase_Tukekae(7)) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If
        Kawase_Tukekae(7) = Mid(Trim(sAngo) & Space(15), 1, 15)

        '------------------------------
        '番号判定・取得
        '------------------------------
        If pHimoku_Ginko = "1000" Then           '全信連の場合
            Kawase_Tukekae(8) = "ﾁｳｹｲ-1"
        Else
            Kawase_Tukekae(8) = Space(16)              '全信連以外の場合
        End If

        '----------------------------------------
        ' 資金付替事由欄設定
        ' 資金付替事由1=受取人名（口座名義人名）
        ' 資金付替事由2=依頼人名（INIファイル)
        ' 資金付替事由3=備考１（伝票用備考１）
        ' 資金付替事由4=備考２（伝票用備考１）
        '----------------------------------------
        If ConvNullToString(pHimoku_Meigi) = "" Or pHimoku_Meigi.Trim = "" Then
            Call GSUB_MESSAGE_WARNING("受取人名がありません" & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        Else
            Kawase_Tukekae(9) = Mid(Trim(pHimoku_Meigi) & Space(48), 1, 48)
        End If
        '依頼人
        Kawase_Tukekae(10) = Mid(Str_IraiNinName & Space(48), 1, 48)

        '備考欄に振替日を設定する場合は "@" で囲む
        If pDenpyo_Biko1.Length >= 7 Then
            If pDenpyo_Biko1.Substring(0, 7) = "@MM-DD@" Then
                Kawase_Tukekae(11) = Mid(Mid(STR_FURIKAE_DATE(1), 5, 2) & "-" & Mid(Kawase_Tukekae(4), 7, 2) & Mid(pDenpyo_Biko1, 8) & Space(20), 1, 20)
            Else
                Kawase_Tukekae(11) = Mid(Trim(pDenpyo_Biko1) & Space(20), 1, 20)
            End If
        Else
            Kawase_Tukekae(11) = pDenpyo_Biko1.Trim
        End If

        If pDenpyo_Biko2.Trim.Length > 0 Then
            If pDenpyo_Biko2.Trim.Length >= 7 Then
                If pDenpyo_Biko2.Substring(0, 7) = "@MM-DD@" Then
                    Kawase_Tukekae(12) = Mid(Mid(STR_FURIKAE_DATE(1), 5, 2) & "-" & Mid(Kawase_Tukekae(4), 7, 2) & Mid(pDenpyo_Biko2, 8) & Space(20), 1, 20)
                ElseIf pDenpyo_Biko2.Substring(0, 5) = "@TEN@" Then
                    '取引先マスタ備考２に「@TEN@」とあったら
                    '支店名を自動編集する機能追加
                    Kawase_Tukekae(12) = Str_Ginko(1).Trim & "ｱﾂｶｲ" & pDenpyo_Biko2.Substring(5, pDenpyo_Biko2.Length - 5)
                Else
                    Kawase_Tukekae(12) = pDenpyo_Biko2.Trim
                End If
            ElseIf pDenpyo_Biko2.Trim.Length >= 5 Then '追加 2005/03/22
                If pDenpyo_Biko2.Substring(0, 5) = "@TEN@" Then
                    '取引先マスタ備考２に「@TEN@」とあったら
                    '支店名を自動編集する機能追加
                    Kawase_Tukekae(12) = Str_Ginko(0).Trim & "ｱﾂｶｲ" & pDenpyo_Biko2.Substring(5, pDenpyo_Biko2.Length - 5)
                Else
                    Kawase_Tukekae(12) = pDenpyo_Biko2.Trim
                End If
            Else
                Kawase_Tukekae(12) = pDenpyo_Biko2.Trim
            End If
        Else
            Kawase_Tukekae(12) = pDenpyo_Biko2.Trim
        End If

        Kawase_Tukekae(11) = Kawase_Tukekae(11).PadRight(20, " ") '追加 2005/03/22
        Kawase_Tukekae(12) = Kawase_Tukekae(12).PadRight(20, " ") '追加 2005/03/22

        Kawase_Tukekae(13) = Space(15)
        Kawase_Tukekae(14) = Space(21)

        For iCount = LBound(Kawase_Tukekae) To UBound(Kawase_Tukekae)
            FD_WORK_D += Kawase_Tukekae(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Kawase_Tukekae = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name & vbCrLf & "作成したデータサイズが違います : " & Len(FD_WORK_D) & "バイト")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function

    '為替請求 2005/06/28
    Private Function PFUNC_Kawase_Seikyu(ByVal pGakkou_Code As String, _
                                           ByVal pGakkou_Name As String, _
                                           ByVal pHimoku_Ginko As String, _
                                           ByVal pHimoku_Siten As String, _
                                           ByVal pHimoku_Kingaku As Long, _
                                           ByVal pDenpyo_Biko1 As String, _
                                           ByVal pDenpyo_Biko2 As String) As String

        Dim FD_WORK_D As String
        'Kawase_Seikyu = INDEX内
        ' 0 = 科目コード * 2
        ' 1 = 取引コード * 3
        ' 2 = 受信店名 * 30
        ' 3 = 発信店名 * 20
        ' 4 = 取扱日 * 6
        ' 5 = 種目 * 4
        ' 6 = 金額 * 12
        ' 7 = 金額複記符号 * 15
        ' 8 = 番号 * 16
        ' 9 = 資金付替事由 * 48
        '10 = 資金付替事由2 * 48
        '11 = 備考1 * 29
        '12 = 備考2 * 29
        '13 = 照会番号 * 15
        '14 = 予備1 * 3

        Dim Kawase_Seikyu(14) As String

        Dim iCount As Integer

        Dim sAngo As String = ""

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Kawase_Seikyu(0) = "48"
        Kawase_Seikyu(1) = "600"

        '------------------------------
        '受信店名取得
        '------------------------------
        If PFUNC_GET_GINKONAME(pHimoku_Ginko, pHimoku_Siten) = False Then
            Call GSUB_MESSAGE_WARNING("金融機関コード取込エラー : " & Trim(pHimoku_Ginko) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        If STR_JIKINKO_CODE = pHimoku_Ginko Then
            Kawase_Seikyu(2) = Mid(Chr(223) & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Seikyu(3) = Mid(Chr(223) & Space(1) & "ｾﾝﾀｰ" & Space(20), 1, 20)
        Else
            Kawase_Seikyu(2) = Mid(Str_Ginko(0).Trim & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Seikyu(3) = Mid(Str_Kawase_CenterName.Trim & Space(20), 1, 20)
        End If
        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- START
        'Kawase_Seikyu(4) = Trim(Str(Val(Mid(STR_FURIKAE_DATE(1), 1, 4)) - CInt(Mid(Str_KijyunDate, 1, 4)))) & Mid(STR_FURIKAE_DATE(1), 5, 4)
        Kawase_Seikyu(4) = CASTCommon.GetWAREKI(Trim(STR_FURIKAE_DATE(1)), "yyMMdd")
        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- END

        Kawase_Seikyu(5) = "ｾｲｷｳ"
        Kawase_Seikyu(6) = Format(CDbl(pHimoku_Kingaku), "000000000000")

        Select Case (pHimoku_Kingaku)
            Case Is <= 0
                Str_Function_Ret = "ZERO"
        End Select

        If PFUNC_Set_FukukiKigo(Format(CLng(Kawase_Seikyu(6)), "#,##0"), sAngo) = False Then
            Call GSUB_MESSAGE_WARNING("複記符号設定処理エラー : " & Trim(Kawase_Seikyu(7)) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If
        Kawase_Seikyu(7) = Mid(Trim(sAngo) & Space(15), 1, 15)

        '------------------------------
        '番号判定・取得
        '------------------------------
        Kawase_Seikyu(8) = Space(16)              '番号

        '----------------------------------------
        ' 資金付替事由欄設定
        ' 資金付替事由=Space(48)
        ' 資金付替事由2=Space(48)
        ' 資金付替事由3=備考１（伝票用備考１）
        ' 資金付替事由4=備考２（伝票用備考１）
        '----------------------------------------
        '資金付替事由
        Kawase_Seikyu(9) = Space(48)
        '資金付替事由2
        Kawase_Seikyu(10) = Space(48)

        '備考１＆２　とりあえず空白
        Kawase_Seikyu(11) = Space(29)
        Kawase_Seikyu(12) = Space(29)

        Kawase_Seikyu(13) = Space(15)
        Kawase_Seikyu(14) = Space(3)


        For iCount = LBound(Kawase_Seikyu) To UBound(Kawase_Seikyu)
            FD_WORK_D += Kawase_Seikyu(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Kawase_Seikyu = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name & vbCrLf & "作成したデータサイズが違います : " & Len(FD_WORK_D) & "バイト")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function

    '諸勘定連動入金(自振手数料)
    Private Function PFUNC_Syokanjo_Nyuukin(ByVal pGakkou_Code As String, _
                                            ByVal pGakkou_Name As String, _
                                            ByVal pTesuuryo_Kingaku As Long, _
                                            ByVal pTSiten_Code As String, _
                                            ByVal pTKamoku_Code As String, _
                                            ByVal pTKouza As String) As String


        Dim FD_WORK_D As String
        'Syokanjo_Nyuukin = INDEX内
        ' 0 = 科目コード * 2
        ' 1 = 取引コード * 3
        ' 2 = 口座番号 * 7
        ' 3 = 行 * 2
        ' 4 = 内訳コード * 2
        ' 5 = 前残 * 12
        ' 6 = 符号コード * 1
        ' 7 = 金額 * 12
        ' 8 = 件数 * 5
        ' 9 = 振替コード * 3
        '10 = 取扱番号1 * 3
        '11 = 人格コード * 2
        '12 = 課税コード * 1
        '13 = 摘要 * 20
        '14 = 連動店番 * 3
        '15 = 連動科目口番 * 9
        '16 = 相手内訳コード * 2
        '17 = 連動ソフト機番 * 1
        '18 = 取扱番号2 * 5
        '19 = 相手摘要 * 20
        '20 = 起算日 * 6
        '21 = 予備1 * 159
        Dim Syokanjo_Nyuukin(21) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Syokanjo_Nyuukin(0) = "99"
        Syokanjo_Nyuukin(1) = "419"

        If CLng(Str_Tesuuryo_Kouza) = 0 Then
            Call GSUB_MESSAGE_WARNING("口座番号がありません : " & Trim(Str_Tesuuryo_Kouza) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(2) = Format(CLng(Str_Tesuuryo_Kouza), "0000000")
        Syokanjo_Nyuukin(3) = "01"

        If CInt(Str_Utiwake_Code) = 0 Then
            Call GSUB_MESSAGE_WARNING("内訳コードがありません : " & Trim(Str_Utiwake_Code) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(4) = Format(CInt(Str_Utiwake_Code), "00")
        Syokanjo_Nyuukin(5) = "000000000000"
        Syokanjo_Nyuukin(6) = "1"
        Syokanjo_Nyuukin(7) = Format(pTesuuryo_Kingaku, "000000000000")
        Syokanjo_Nyuukin(8) = "00001"
        Syokanjo_Nyuukin(9) = Space(3)
        Syokanjo_Nyuukin(10) = Space(3)
        Syokanjo_Nyuukin(11) = Space(2)
        Syokanjo_Nyuukin(12) = Space(1)
        Syokanjo_Nyuukin(13) = Mid("ﾃｽｳﾘﾖｳ" & Space(20), 1, 20)

        If Trim(pTSiten_Code) = "" Then
            Call GSUB_MESSAGE_WARNING("連動店番がありません : " & Trim(pTSiten_Code) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(14) = Format(CInt(pTSiten_Code), "000")

        If Trim(pTKamoku_Code) = "" Or Trim(pTKouza) = "" Then
            Call GSUB_MESSAGE_WARNING("連動科目口番がありません : " & Trim(pTKamoku_Code) & " & " & Trim(pTKouza) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(15) = Format(CInt(pTKamoku_Code), "00") & Format(CInt(pTKouza), "0000000")
        Syokanjo_Nyuukin(16) = Space(2)
        Syokanjo_Nyuukin(17) = Space(1)
        Syokanjo_Nyuukin(18) = Space(5)
        Syokanjo_Nyuukin(19) = Space(20)
        Syokanjo_Nyuukin(20) = Space(6)
        Syokanjo_Nyuukin(21) = Space(159)

        If CLng(Syokanjo_Nyuukin(7)) = 0 Then
            Str_Function_Ret = "ZERO"
        Else
            For iCount = LBound(Syokanjo_Nyuukin) To UBound(Syokanjo_Nyuukin)
                FD_WORK_D += Syokanjo_Nyuukin(iCount)
            Next iCount

            If Len(FD_WORK_D) = 280 Then
                PFUNC_Syokanjo_Nyuukin = FD_WORK_D
            Else
                Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name & vbCrLf & "作成したデータサイズが違います : " & Len(FD_WORK_D) & "バイト")
                Str_Function_Ret = "NG"
                Return Str_Function_Ret
            End If
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function

    '諸勘定連動入金(郵送料)
    Private Function PFUNC_Syokanjo_Nyuukin_SOURYO(ByVal pGakkou_Code As String, _
                                            ByVal pGakkou_Name As String, _
                                            ByVal pTesuuryo_Kingaku As Long, _
                                            ByVal pTSiten_Code As String, _
                                            ByVal pTKamoku_Code As String, _
                                            ByVal pTKouza As String) As String


        Dim FD_WORK_D As String
        'Syokanjo_Nyuukin_SOURYO = INDEX内
        ' 0 = 科目コード * 2
        ' 1 = 取引コード * 3
        ' 2 = 口座番号 * 7
        ' 3 = 行 * 2
        ' 4 = 内訳コード * 2
        ' 5 = 前残 * 12
        ' 6 = 符号コード * 1
        ' 7 = 金額 * 12
        ' 8 = 件数 * 5
        ' 9 = 振替コード * 3
        '10 = 取扱番号1 * 3
        '11 = 人格コード * 2
        '12 = 課税コード * 1
        '13 = 摘要 * 20
        '14 = 連動店番 * 3
        '15 = 連動科目口番 * 9
        '16 = 相手内訳コード * 2
        '17 = 連動ソフト機番 * 1
        '18 = 取扱番号2 * 5
        '19 = 相手摘要 * 20
        '20 = 起算日 * 6
        '21 = 予備1 * 159
        Dim Syokanjo_Nyuukin(21) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Syokanjo_Nyuukin(0) = "99"
        Syokanjo_Nyuukin(1) = "419"

        If CLng(Str_Tesuuryo_Kouza2) = 0 Then
            Call GSUB_MESSAGE_WARNING("口座番号がありません : " & Trim(Str_Tesuuryo_Kouza2) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)

            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(2) = Format(CLng(Str_Tesuuryo_Kouza2), "0000000")
        Syokanjo_Nyuukin(3) = "01"

        If CInt(Str_Utiwake_Code2) = 0 Then
            Call GSUB_MESSAGE_WARNING("内訳コードがありません : " & Trim(Str_Utiwake_Code2) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(4) = Format(CInt(Str_Utiwake_Code2), "00")
        Syokanjo_Nyuukin(5) = "000000000000"
        Syokanjo_Nyuukin(6) = "1"
        Syokanjo_Nyuukin(7) = Format(pTesuuryo_Kingaku, "000000000000")
        Syokanjo_Nyuukin(8) = "00001"
        Syokanjo_Nyuukin(9) = Space(3)
        Syokanjo_Nyuukin(10) = Space(3)
        Syokanjo_Nyuukin(11) = Space(2)
        Syokanjo_Nyuukin(12) = Space(1)
        Syokanjo_Nyuukin(13) = Mid("ｿｳﾘﾖｳ" & Space(20), 1, 20)

        If Trim(pTSiten_Code) = "" Then
            Call GSUB_MESSAGE_WARNING("連動店番がありません : " & Trim(pTSiten_Code) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)

            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(14) = Format(CInt(pTSiten_Code), "000")

        If Trim(pTKamoku_Code) = "" Or Trim(pTKouza) = "" Then
            Call GSUB_MESSAGE_WARNING("連動科目口番がありません : " & Trim(pTKamoku_Code) & " & " & Trim(pTKouza) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)

            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(15) = Format(CInt(pTKamoku_Code), "00") & Format(CInt(pTKouza), "0000000")
        Syokanjo_Nyuukin(16) = Space(2)
        Syokanjo_Nyuukin(17) = Space(1)
        Syokanjo_Nyuukin(18) = Space(5)
        Syokanjo_Nyuukin(19) = Space(20)
        Syokanjo_Nyuukin(20) = Space(6)
        Syokanjo_Nyuukin(21) = Space(159)

        If CLng(Syokanjo_Nyuukin(7)) = 0 Then
            Str_Function_Ret = "ZERO"
        Else
            For iCount = LBound(Syokanjo_Nyuukin) To UBound(Syokanjo_Nyuukin)
                FD_WORK_D += Syokanjo_Nyuukin(iCount)
            Next iCount

            If Len(FD_WORK_D) = 280 Then
                PFUNC_Syokanjo_Nyuukin_SOURYO = FD_WORK_D
            Else
                Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name & vbCrLf & "作成したデータサイズが違います : " & Len(FD_WORK_D) & "バイト")
                Str_Function_Ret = "NG"

                Return Str_Function_Ret
            End If
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function

    '諸勘定連動入金(振込手数料)
    Private Function PFUNC_Syokanjo_Nyuukin_FURIKOMI(ByVal pGakkou_Code As String, _
                                            ByVal pGakkou_Name As String, _
                                            ByVal pTesuuryo_Kingaku As Long, _
                                            ByVal pTSiten_Code As String, _
                                            ByVal pTKamoku_Code As String, _
                                            ByVal pTKouza As String) As String


        Dim FD_WORK_D As String
        'PFUNC_Syokanjo_Nyuukin_FURIKOMI = INDEX内
        ' 0 = 科目コード * 2
        ' 1 = 取引コード * 3
        ' 2 = 口座番号 * 7
        ' 3 = 行 * 2
        ' 4 = 内訳コード * 2
        ' 5 = 前残 * 12
        ' 6 = 符号コード * 1
        ' 7 = 金額 * 12
        ' 8 = 件数 * 5
        ' 9 = 振替コード * 3
        '10 = 取扱番号1 * 3
        '11 = 人格コード * 2
        '12 = 課税コード * 1
        '13 = 摘要 * 20
        '14 = 連動店番 * 3
        '15 = 連動科目口番 * 9
        '16 = 相手内訳コード * 2
        '17 = 連動ソフト機番 * 1
        '18 = 取扱番号2 * 5
        '19 = 相手摘要 * 20
        '20 = 起算日 * 6
        '21 = 予備1 * 159
        Dim Syokanjo_Nyuukin(21) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Syokanjo_Nyuukin(0) = "99"
        Syokanjo_Nyuukin(1) = "419"

        If CLng(Str_Tesuuryo_Kouza3) = 0 Then
            Call GSUB_MESSAGE_WARNING("口座番号がありません : " & Trim(Str_Tesuuryo_Kouza3) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(2) = Format(CLng(Str_Tesuuryo_Kouza3), "0000000")
        Syokanjo_Nyuukin(3) = "01"

        If CInt(Str_Utiwake_Code3) = 0 Then
            Call GSUB_MESSAGE_WARNING("内訳コードがありません : " & Trim(Str_Utiwake_Code3) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(4) = Format(CInt(Str_Utiwake_Code3), "00")
        Syokanjo_Nyuukin(5) = "000000000000"
        Syokanjo_Nyuukin(6) = "1"
        Syokanjo_Nyuukin(7) = Format(pTesuuryo_Kingaku, "000000000000")
        Syokanjo_Nyuukin(8) = "00001"
        Syokanjo_Nyuukin(9) = Space(3)
        Syokanjo_Nyuukin(10) = Space(3)
        Syokanjo_Nyuukin(11) = Space(2)
        Syokanjo_Nyuukin(12) = Space(1)
        Syokanjo_Nyuukin(13) = Mid("ﾌﾘｺﾐﾃｽｳﾘﾖｳ" & Space(20), 1, 20)

        If Trim(pTSiten_Code) = "" Then
            Call GSUB_MESSAGE_WARNING("連動店番がありません : " & Trim(pTSiten_Code) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(14) = Format(CInt(pTSiten_Code), "000")

        If Trim(pTKamoku_Code) = "" Or Trim(pTKouza) = "" Then
            Call GSUB_MESSAGE_WARNING("連動科目口番がありません : " & Trim(pTKamoku_Code) & " & " & Trim(pTKouza) & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(15) = Format(CInt(pTKamoku_Code), "00") & Format(CInt(pTKouza), "0000000")
        Syokanjo_Nyuukin(16) = Space(2)
        Syokanjo_Nyuukin(17) = Space(1)
        Syokanjo_Nyuukin(18) = Space(5)
        Syokanjo_Nyuukin(19) = Space(20)
        Syokanjo_Nyuukin(20) = Space(6)
        Syokanjo_Nyuukin(21) = Space(159)

        If CLng(Syokanjo_Nyuukin(7)) = 0 Then
            Str_Function_Ret = "ZERO"
        Else
            For iCount = LBound(Syokanjo_Nyuukin) To UBound(Syokanjo_Nyuukin)
                FD_WORK_D += Syokanjo_Nyuukin(iCount)
            Next iCount

            If Len(FD_WORK_D) = 280 Then
                PFUNC_Syokanjo_Nyuukin_FURIKOMI = FD_WORK_D
            Else
                Call GSUB_MESSAGE_WARNING("決済情報に誤りがあります" & vbCrLf & "取引先コード = " & pGakkou_Code & vbCrLf & "取引先名 = " & pGakkou_Name & vbCrLf & "作成したデータサイズが違います : " & Len(FD_WORK_D) & "バイト")
                Str_Function_Ret = "NG"
                Return Str_Function_Ret
            End If
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function


    Private Function PFUNC_Update_Schedule(ByVal pGakkou_Code As String, _
                                           ByVal pFurikae_Date As String, _
                                           ByVal pTimeStamp As String, _
                                           ByVal pFuri_KBN_S As String) As Boolean

        PFUNC_Update_Schedule = False

        STR_SQL = " UPDATE  G_SCHMAST SET "
        STR_SQL += " KESSAI_FLG_S = '1'"
        STR_SQL += ",KESSAI_DATE_S = '" & pTimeStamp.Substring(0, 8) & "'" '追加2005/06/16
        STR_SQL += ",TIME_STAMP_S = '" & pTimeStamp & "'"
        STR_SQL += ",TESUU_KIN3_S = " & CLng(dblFURIKOMI_TESUU_ALL)  '振込手数料更新 2006/04/15
        STR_SQL += ",TESUU_KIN_S = TESUU_KIN1_S + TESUU_KIN2_S + " & CLng(dblFURIKOMI_TESUU_ALL)   '振込手数料更新 2006/04/15
        'STR_SQL += ",TESUU_KIN_S = TESUU_KIN_S + " & CLng(dblFURIKOMI_TESUU_ALL)   '振込手数料更新 2006/04/15
        STR_SQL += " WHERE"
        STR_SQL += " FURI_DATE_S = '" & pFurikae_Date & "'"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_S ='" & pGakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S ='" & pFuri_KBN_S & "'" '追加 2005/06/28
        'STR_SQL += " SCH_KBN_S <> '2'"

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        Lng_Upd_Count += 1

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        PFUNC_Update_Schedule = True

    End Function
    Private Function PFUNC_Set_FukukiKigo(ByVal pValue1 As String, ByRef pValue2 As String) As Boolean

        Dim iLoopCount As Integer

        Dim sLZH As String
        Dim sFugo(14) As String

        PFUNC_Set_FukukiKigo = False

        sLZH = "Y"
        pValue2 = ""

        '渡された金額情報を暗号化する
        For iLoopCount = 0 To 14
            sFugo(iLoopCount) = Space(1)
            Select Case (Mid(Format(CLng(pValue1), "000,000,000,000"), iLoopCount + 1, 1))
                Case "0"
                    If sLZH = "Y" Then
                        sFugo(iLoopCount) = Space(1)
                    Else
                        sFugo(iLoopCount) = "ﾄ"
                    End If
                Case "1"
                    sLZH = "N"
                    sFugo(iLoopCount) = "ﾋ"
                Case "2"
                    sLZH = "N"
                    sFugo(iLoopCount) = "ﾌ"
                Case "3"
                    sLZH = "N"
                    sFugo(iLoopCount) = "ﾐ"
                Case "4"
                    sLZH = "N"
                    sFugo(iLoopCount) = "ﾖ"
                Case "5"
                    sLZH = "N"
                    sFugo(iLoopCount) = "ｲ"
                Case "6"
                    sLZH = "N"
                    sFugo(iLoopCount) = "ﾙ"
                Case "7"
                    sLZH = "N"
                    sFugo(iLoopCount) = "ﾅ"
                Case "8"
                    sLZH = "N"
                    sFugo(iLoopCount) = "ﾔ"
                Case "9"
                    sLZH = "N"
                    sFugo(iLoopCount) = "ｺ"
                Case ","
                    sFugo(iLoopCount) = Space(1)
            End Select
        Next iLoopCount


        For iLoopCount = 0 To 14
            pValue2 += sFugo(iLoopCount)
        Next

        pValue2 = Trim(pValue2)

        PFUNC_Set_FukukiKigo = True

    End Function
    Private Function PFUNC_GET_GINKONAME(ByVal pGinko_Code As String, ByVal pSiten_Code As String) As Boolean

        Dim ret As Boolean = False

        '金融機関コードと支店コードから金融機関名、支店名を抽出
        Str_Ginko(0) = ""
        Str_Ginko(1) = ""

        If Trim(pGinko_Code) = "" Or Trim(pSiten_Code) = "" Then
            Exit Function
        End If

        Dim SQL As String = ""

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            SQL = "SELECT KIN_KNAME_N , SIT_KNAME_N  FROM TENMAST "
            SQL &= " WHERE KIN_NO_N = '" & pGinko_Code & "'"
            SQL &= " AND SIT_NO_N = '" & pSiten_Code & "'"

            OraReader = New CASTCommon.MyOracleReader

            If OraReader.DataReader(SQL) Then
                Str_Ginko(0) = OraReader.GetItem("KIN_KNAME_N")
                Str_Ginko(1) = OraReader.GetItem("SIT_KNAME_N")
            End If

            OraReader.Close()
            OraReader = Nothing

            ret = True

        Catch ex As Exception
            Throw New Exception("TENMAST取得失敗", ex)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function

    Private Function PFUNC_Split_WorkD() As Boolean

        Dim oFileSys As Object
        Dim oDrive As Object

        Dim sFileName As String
        Dim sHedder As String
        Dim sOut As String

        Dim iFileNo As Integer
        Dim iFileNo2 As Integer
        Dim iFileNo3 As Integer

        Dim sFILE_NAME1 As String 'データベースより作成されたファイル
        Dim sFILE_NAME2 As String 'FILE1をPowerSortを使用して並び替えたもの
        Dim sFILE_NAME3 As String '

        Dim iCnt As Integer
        Dim iSCount As Integer
        Dim iLCount As Integer

        Dim Rient_Rec_Cnt As Integer

        Dim HEAD_DATA(1007) As Byte
        Dim OUT_DATA(255) As Byte

        Dim lRead_Point As Long
        Dim lWrite_Point As Long

        PFUNC_Split_WorkD = False

        On Error Resume Next

        sFILE_NAME1 = STR_DAT_PATH & "FD_WORK1_D.DAT"
        sFILE_NAME2 = STR_DAT_PATH & "FD_WORK2_D.DAT"
        sFILE_NAME3 = STR_DAT_PATH & "FD_WORK_D.DAT"

        'PowerSortでワークファイルの内容をソートする
        'ソートキー・フィールド(学校コード、レコード区分、金庫、店舗、科目、口座)を指定
        With AxPowerSORT1
            'エラーメッセージを表示しない旨を指定します。
            'True :メッセージ表示
            'False:メッセージ非表示
            .DispMessage = False

            'ソート処理を指定します。
            '0	(省略値) ソート機能を実行します。
            '1	マージ機能を実行します。
            '2	コピー機能を実行します。
            .DisposalNumber = 0

            'フィールドはカラム位置で指定
            '0	(省略値) 浮動フィールドで指定します。
            '1	固定フィールドで指定します。
            .FieldDefinition = 1

            'ソートキー・フィールド(学校コード、レコード区分、金庫、店舗、科目、口座)を指定
            .KeyCmdStr = "0.7asca 7.1asca 8.4asca 12.3asca 15.1asca 16.7asca"

            '入力ファイル名を指定します。
            .InputFiles = sFILE_NAME1
            '入力ファイル種別にバイナリ固定長ファイル（1）を指定します。
            .InputFileType = 1

            '出力ファイル名を指定します。
            .OutputFile = sFILE_NAME2
            '出力ファイル種別にバイナリ固定長ファイル（1）を指定します。
            .OutputFileType = 1

            .MaxRecordLength = 303
            .Action()                     'レコード選択処理を実行します。

            If .ErrorCode <> 0 Then       'エラー検出時の処理。
                Call GSUB_MESSAGE_WARNING("PowerSORTでエラーを検出しました。 ErrorDetail=" & .ErrorDetail)

                Exit Function
            End If
        End With

        iFileNo2 = FreeFile()
        FileOpen(iFileNo2, sFILE_NAME2, OpenMode.Random, , , 303)    'ワークファイルFD_WORK2_D.DAT

        If Err.Number <> 0 Then
            Call GSUB_MESSAGE_WARNING("中間ファイルのオープンに失敗しました")
            FileClose(iFileNo2)

            Exit Function
        End If

        If Dir$(sFILE_NAME3) <> "" Then
            Kill(sFILE_NAME3)
        End If

        Err.Clear()

        iFileNo3 = FreeFile()
        FileOpen(iFileNo3, sFILE_NAME3, OpenMode.Random, , , 280)    'ワークファイルFD_WORK_D.DAT

        If Err.Number <> 0 Then
            Call GSUB_MESSAGE_WARNING("中間ファイルのオープンに失敗しました")
            FileClose(iFileNo3)
            Exit Function
        End If


        Dim Data As String
        Dim Next_Data As String
        Dim OUT_DATA1 As String
        Dim OUT_Data2 As String
        Dim KBN As String
        Dim Cnt As Long
        Dim Next_Cnt As Long
        Dim recHIMOKU As String = ""
        Dim recTESUU As String = ""
        Dim ALL_KINGAKU As Long
        Dim ALL_TESUU As Long
        Dim DATA_TYPE As String = ""

        Dim WriteCnt As Long

        Dim FILE_REC As Long

        ALL_KINGAKU = 0
        ALL_TESUU = 0
        Cnt = 1
        WriteCnt = 0
        FILE_REC = 0
        '-------------------------------------------------------------------------------
        'FD_WORK2_D.DATの行数カウント
        '-------------------------------------------------------------------------------
        Do Until EOF(iFileNo2) = True
            FILE_REC = FILE_REC + 1
            FileGet(iFileNo2, Line, FILE_REC)
        Loop

        For Cnt = 1 To FILE_REC
            FileGet(iFileNo2, Line, Cnt)

            KBN = Line.Data.Substring(7, 1)
            Select Case (KBN)
                Case "1"    '<---レコード区分"1"
                    recHIMOKU = ""
                    recTESUU = ""

                    StrLine.Data = Line.Data.Substring(23, 280)

                    WriteCnt += 1

                    FilePut(iFileNo3, StrLine, WriteCnt)
                Case "2"    '<---レコード区分"2"
                    DATA_TYPE = Line.Data.Substring(23, 5)
                    Next_Cnt = Cnt + 1

                    WriteCnt += 1

                    FileGet(iFileNo2, NextLine, Next_Cnt)

                    If Line.Data.Substring(8, 15) <> NextLine.Data.Substring(8, 15) Then
                        '<---金・店・科・口が違ってた場合
                        Select Case (DATA_TYPE)
                            Case "02019"
                                '<---普通入金の場合 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(38, 10)
                            Case "01010"
                                '<---当座入金の場合 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(36, 10)
                            Case "48100"
                                '<---為替振込の場合 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(92, 12)
                            Case "48500"
                                '<---為替付替の場合 2005/06/20
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(88, 12)
                            Case "48600"
                                '<---為替請求の場合 2005/06/28
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(88, 12)
                        End Select

                        OUT_DATA1 = Line.Data.Substring(23, 280)
                        OUT_Data2 = ""

                        Select Case (DATA_TYPE)
                            Case "02019"
                                '<---普通入金の場合 2003/08/27
                                StrLine.Data = Mid(OUT_DATA1, 1, 14) & Format(ALL_KINGAKU, "0000000000") & Mid(OUT_DATA1, 25, 256)
                            Case "01010"
                                '<---当座入金の場合 2003/08/27
                                StrLine.Data = Mid(OUT_DATA1, 1, 12) & Format(ALL_KINGAKU, "0000000000") & Mid(OUT_DATA1, 23, 256)
                            Case "48100"
                                '<---為替振込の場合 2003/08/27
                                StrLine.Data = Mid(OUT_DATA1, 1, 69) & Format(ALL_KINGAKU, "000000000000") & Mid(OUT_DATA1, 82, 256)
                            Case "48500"
                                '<---為替付替の場合 2005/06/20
                                StrLine.Data = Mid(OUT_DATA1, 1, 65) & Format(ALL_KINGAKU, "000000000000") & Mid(OUT_DATA1, 78, 256)
                            Case "48600"
                                '<---為替請求の場合 2005/06/28
                                StrLine.Data = Mid(OUT_DATA1, 1, 65) & Format(ALL_KINGAKU, "000000000000") & Mid(OUT_DATA1, 78, 256)
                        End Select

                        FilePut(iFileNo3, StrLine, WriteCnt)

                        ALL_KINGAKU = 0
                    Else
                        Select Case (DATA_TYPE)
                            Case "02019"
                                '<---普通入金の場合 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(38, 10)
                            Case "01010"
                                '<---当座入金の場合 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(36, 10)
                            Case "48100"
                                '<---為替振込の場合 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(92, 12)
                            Case "48500"
                                '<---為替付替の場合 2005/06/20
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(88, 12)
                            Case "48600"
                                '<---為替請求の場合 2005/06/28
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(88, 12)
                        End Select

                        GoTo NEXT_LOOP
                    End If
                Case "8"    '<---レコード区分"8"
                    If recTESUU <> Line.Data.Substring(28, 7) Then 'iniファイルの手数口座が違う場合
                        recTESUU = Line.Data.Substring(28, 7)
                        ALL_TESUU = ALL_TESUU + Line.Data.Substring(52, 12)
                        StrLine.Data = Line.Data.Substring(23, 280)
                        WriteCnt += 1

                        FilePut(iFileNo3, StrLine, WriteCnt)

                        ALL_TESUU = 0
                    Else
                        ALL_TESUU = ALL_TESUU + Line.Data.Substring(52, 12)

                        GoTo NEXT_LOOP
                    End If
            End Select
NEXT_LOOP:

        Next

        FileClose(iFileNo2)
        FileClose(iFileNo3)

        '************************************************************
        '一時的にコメントする
        'リエンタは作らない(資金決済画面から作成するため)
        ''リエンタ用データ作成
        'If clsFUSION.fn_MAKE_RIENTDATA("", _
        '                               STR_DAT_PATH & "FD_WORK_D.DAT", _
        '                               STR_DAT_PATH & "FD_WORK_O.DAT", _
        '                               STR_DAT_PATH & "FD_WORK_H.DAT", _
        '                               Str_Jikou_Ginko_Code & Str_Honbu_Code, _
        '                               STR_FURIKAE_DATE(1), _
        '                               Rient_Rec_Cnt) = False Then

        '    Err.Clear()
        '    Exit Function
        'End If
        '************************************************************


        '作成した２つのファイルからリエンタファイルを作成する
Retry:
        If GFUNC_MESSAGE_QUESTION("ＦＤをセットして下さい") <> vbOK Then

            Exit Function
        End If

        Select Case (Int_FD_Kbn)
            Case 1
                'FD区分がIBM形式の場合

            Case Else
                Str_Ret = InputBox("ファイル名を入力してください", , "RIENTER.RNT")

                If Trim(Str_Ret) <> "" Then
                    sFileName = Str_Kekka_Path & Str_Ret

                    '取得したPATHがFDﾄﾞﾗｲﾌﾞの場合
                    Select Case (StrConv(Mid(Str_Kekka_Path, 1, 1), vbProperCase))
                        Case "A", "B"
                            'ＦＤ読み取り処理
                            oFileSys = CreateObject("Scripting.FileSystemObject")
                            oDrive = oFileSys.GetDrive(Str_Kekka_Path)

                            If oDrive.IsReady <> True Then
                                If GFUNC_MESSAGE_QUESTION("処理フロッピーを差し込んでください") <> vbOK Then
                                    Exit Function
                                End If
                                GoTo Retry

                            End If
                    End Select

                    If Dir$(sFileName) <> "" Then
                        Kill(sFileName)
                    End If

                    iFileNo = FreeFile()

                    Err.Number = 0

                    FileOpen(iFileNo, sFileName, OpenMode.Binary, OpenAccess.Write, , )     'ワークファイル

                    If Err.Number <> 0 Then
                        Exit Function
                    End If

                    'FilePut(iFileNo, Str_Ret, )
                    'ファイル名設定 2006/04/14
                    strTITLE.strTITLE_16 = Str_Ret
                    FilePut(iFileNo, strTITLE, )

                    iFileNo2 = FreeFile()
                    '作成したヘッダファイルを読み込みヘッダデータを書き込む
                    FileOpen(iFileNo2, STR_DAT_PATH & "FD_WORK_H.DAT", OpenMode.Binary, OpenAccess.Read, , 1008)

                    FileGet(iFileNo2, HEAD_DATA, 1)
                    FilePut(iFileNo, HEAD_DATA, )

                    FileClose(iFileNo2)

                    iFileNo3 = FreeFile()
                    '作成した出力ファイルを読み込み出力データを書き込む
                    FileOpen(iFileNo3, STR_DAT_PATH & "FD_WORK_O.DAT", OpenMode.Binary, OpenAccess.Read, , 256)

                    iCnt = 1

                    Do Until EOF(iFileNo3)
                        lRead_Point = ((iCnt - 1) * 256) + 1
                        FileGet(iFileNo3, OUT_DATA, lRead_Point)

                        iSCount = 32 + CInt(OUT_DATA(5))

                        For iLCount = iSCount To 255
                            OUT_DATA(iLCount) = 0
                        Next iLCount

                        lWrite_Point = 1025 + ((iCnt - 1) * 256)
                        FilePut(iFileNo, OUT_DATA, lWrite_Point)

                        iCnt += 1
                    Loop

                    FileClose(iFileNo)
                    FileClose(iFileNo3)

                End If
        End Select

        PFUNC_Split_WorkD = True

    End Function

    Private Function PFUNC_DeletePrtKessai() As Boolean

        PFUNC_DeletePrtKessai = False

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then

            Exit Function
        End If

        STR_SQL = " DELETE   FROM G_PRTKESSAI"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then

            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then

            Exit Function
        End If

        PFUNC_DeletePrtKessai = True


    End Function
    Private Function PFUNC_InsertPrtKessai() As Boolean

        PFUNC_InsertPrtKessai = False

        STR_SQL = " INSERT INTO G_PRTKESSAI"
        STR_SQL += " values("
        STR_SQL += "'" & Str_Prt_GAKKOU_CODE & "'"
        STR_SQL += ",'" & Str_Prt_GAKKOU_KNAME & "'"
        STR_SQL += ",'" & Str_Prt_GAKKOU_NNAME & "'"
        STR_SQL += ",'" & Str_Prt_HONBU_KOUZA & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_DATE & "'"
        STR_SQL += "," & CInt(Str_Prt_TESUURYO_JIFURI)
        STR_SQL += "," & CInt(Str_Prt_TESUURYO_FURI)
        STR_SQL += "," & CInt(Str_Prt_TESUURYO_ETC)
        STR_SQL += ",'" & Str_Utiwake_Code & "'" '口振内訳コード追加 2006/04/17
        STR_SQL += ",'" & Str_Prt_JIFURI_KOUZA & "'"
        STR_SQL += ",'" & Str_Utiwake_Code3 & "'" '振込内訳コード追加 2006/04/17
        STR_SQL += ",'" & Str_Prt_FURI_KOUZA & "'"
        STR_SQL += ",'" & Str_Utiwake_Code2 & "'" 'その他内訳コード追加 2006/04/17
        STR_SQL += ",'" & Str_Prt_ETC_KOUZA & "'"
        STR_SQL += ",'" & Str_Prt_HIMOKU_NNAME & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_PATERN & "'"
        STR_SQL += "," & CLng(Str_Prt_NYUKIN_KINGAKU)
        STR_SQL += ",'" & Str_Prt_KESSAI_KIN_CODE & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_KIN_KNAME & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_KIN_NNAME & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_TENPO & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_SIT_KNAME & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_SIT_NNAME & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_KAMOKU & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_KOUZA & "'"
        STR_SQL += ",'" & Str_Prt_FURI_KBN & "'"
        STR_SQL += "," & Str_Prt_TESUURYO_FURI '振込手数料追加 2006/04/15
        STR_SQL += "," & lngSYORIKEN_TOKUBETU '処理件数
        STR_SQL += "," & dblSYORIKIN_TOKUBETU '処理金額
        STR_SQL += "," & lngFURIKEN_TOKUBETU '振替済件数
        STR_SQL += "," & dblFURIKIN_TOKUBETU '振替済金額
        STR_SQL += "," & lngFUNOUKEN_TOKUBETU '不能件数
        STR_SQL += "," & dblFUNOUKIN_TOKUBETU '不能金額
        STR_SQL += ",'" & sSyori_Date & "'" 'タイムスタンプ
        STR_SQL += ",'" & Str_Prt_FURI_DATE & "'"   '振替日
        STR_SQL += " )"

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        PFUNC_InsertPrtKessai = True

    End Function

    Private Function PFUNC_Himoku_Create() As Boolean

        Dim iNo As Integer

        '追加
        Dim CHK_GAKUNEN As Integer
        Dim CHK_HIMOKU_NO As Integer
        Dim iLcount As Integer
        Dim bFlg As Boolean = False
        Dim bLoopFlg As Boolean = False
        Dim iLoopCount As Integer

        Dim strFURI_KBN_LOCAL As String = "0"
        Dim strFURI_DATE_LOCAL As String = "00000000"
        Dim strNENGETUDO_LOCAL As String = "000000"
        Dim strGAKKOU_CODE_LOCAL As String = ""

        PFUNC_Himoku_Create = False

        STR_SQL = " DELETE  FROM MAIN0500G_WORK"

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If
        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        STR_SQL = " SELECT * FROM G_SCHMAST WHERE"
        STR_SQL += " KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND"
        STR_SQL += " KESSAI_FLG_S = '0'" '決済フラグ=0のもの 2005/06/30
        STR_SQL += " AND"
        STR_SQL += " TYUUDAN_FLG_S = '0'" '中断フラグ=0のもの 2006/04/17
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S <> '2'" 'スケジュール区分<>2のもの(標準版) 2006/04/13
        STR_SQL += " ORDER BY GAKKOU_CODE_S,NENGETUDO_S,FURI_DATE_S,FURI_KBN_S"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If
        ReDim iGakunen_Flag(9)
        While (OBJ_DATAREADER_DREAD.Read = True)
            '学年フラグ取得 2006/10/27
            For iLoopCount = 1 To 9
                'If OBJ_DATAREADER_DREAD.Item("GAKUNEN" & iLoopCount & "_FLG_S") = "1" Then
                iGakunen_Flag(iLoopCount) = OBJ_DATAREADER_DREAD.Item("GAKUNEN" & iLoopCount & "_FLG_S")
                'End If
            Next
            strFURI_KBN_LOCAL = OBJ_DATAREADER_DREAD.Item("FURI_KBN_S")
            strFURI_DATE_LOCAL = OBJ_DATAREADER_DREAD.Item("FURI_DATE_S")
            strNENGETUDO_LOCAL = OBJ_DATAREADER_DREAD.Item("NENGETUDO_S")
            strGAKKOU_CODE_LOCAL = OBJ_DATAREADER_DREAD.Item("GAKKOU_CODE_S")

            STR_SQL = " SELECT "
            STR_SQL += " G_MEIMAST.*"
            STR_SQL += ",HIMOMAST.*"

            If strFURI_KBN_LOCAL = "2" Or strFURI_KBN_LOCAL = "3" Then
                STR_SQL += " FROM G_MEIMAST ,HIMOMAST "
                STR_SQL += " WHERE"
                STR_SQL += " FURI_DATE_M ='" & strFURI_DATE_LOCAL & "'"
                STR_SQL += " AND"
                STR_SQL += " HIMOKU_ID_H ='000'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN_CODE_M = GAKUNEN_CODE_H"
                STR_SQL += " AND"
                STR_SQL += " GAKKOU_CODE_M = GAKKOU_CODE_H"
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_M = '" & strFURI_KBN_LOCAL & "'"
            Else
                STR_SQL += " FROM G_MEIMAST LEFT JOIN HIMOMAST ON (G_MEIMAST.HIMOKU_ID_M = HIMOMAST.HIMOKU_ID_H) AND (G_MEIMAST.GAKUNEN_CODE_M = HIMOMAST.GAKUNEN_CODE_H) AND (G_MEIMAST.GAKKOU_CODE_M = HIMOMAST.GAKKOU_CODE_H)"
                STR_SQL += " WHERE"
                STR_SQL += " FURI_DATE_M ='" & strFURI_DATE_LOCAL & "'"
                STR_SQL += " AND"
                STR_SQL += " TUKI_NO_H ='" & strNENGETUDO_LOCAL.Substring(4, 2) & "'"
                '入金・臨時出金データは除外 2007/02/10
                STR_SQL += " AND"
                STR_SQL += " (FURI_KBN_M <> '2' OR FURI_KBN_M <> '3')"

            End If

            '学校コード追加 2006/04/18 
            STR_SQL += " AND"
            STR_SQL += " GAKKOU_CODE_M = '" & strGAKKOU_CODE_LOCAL & "'"
            '↓2006/10/26
            STR_SQL += " AND ("
            bLoopFlg = False
            For iLcount = 1 To 9
                If iGakunen_Flag(iLcount) = 1 Then
                    If bLoopFlg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_M=" & iLcount
                    bLoopFlg = True
                End If
            Next iLcount
            STR_SQL += " )"

            If GFUNC_SELECT_SQL4(STR_SQL, 0) = False Then
                Exit Function
            End If

            While (OBJ_DATAREADER_DREAD2.Read = True)
                For iNo = 1 To 15

                    If IsDBNull(OBJ_DATAREADER_DREAD2.Item("HIMOKU_NAME" & Format(iNo, "00") & "_H")) = False Then
                        CHK_HIMOKU_NO = iNo
                        CHK_GAKUNEN = OBJ_DATAREADER_DREAD2.Item("GAKUNEN_CODE_M")

                        '振替区分=2(入金)または3(臨時出金)時、
                        '費目マスタのYOBI1_H～YOBI3_Hに値が無い場合エラーとする 2005/07/01
                        If strFURI_KBN_LOCAL = "2" Then '入金
                            If ConvNullToString(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Trim.Length <> 7 Then
                                MessageBox.Show("入金時の決済金融機関が登録されていません", STR_COMMAND, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                If GFUNC_SELECT_SQL4("", 1) = False Then
                                    Exit Function
                                End If

                                Exit Function
                            End If
                        ElseIf strFURI_KBN_LOCAL = "3" Then '臨時出金
                            If ConvNullToString(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Trim.Length <> 17 Then
                                MessageBox.Show("臨時出金時の決済金融機関が登録されていません", STR_COMMAND, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                If GFUNC_SELECT_SQL4("", 1) = False Then
                                    Exit Function
                                End If

                                Exit Function
                            End If
                        End If

                        STR_SQL = " SELECT * FROM MAIN0500G_WORK"
                        STR_SQL += " WHERE"
                        STR_SQL += " GAKKOU_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("GAKKOU_CODE_H") & "'"
                        '特別時と通常時は決済金融機関情報は別項目から取得
                        '臨時データはYOBI1_HまたはYOBI2_Hから取得 2005/06/28
                        If strFURI_KBN_LOCAL = "2" Then '入金
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KIN_CODE_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(0, 4) & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_TENPO_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(4, 3) & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KAMOKU_H ='01'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KOUZA_H = '9999999'"

                        ElseIf strFURI_KBN_LOCAL = "3" Then '臨時出金
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KIN_CODE_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(0, 4) & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_TENPO_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(4, 3) & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KAMOKU_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(7, 2) & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KOUZA_H = '" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(9, 7) & "'"

                        Else
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KIN_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KIN_CODE" & Format(iNo, "00") & "_H") & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_TENPO_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_TENPO" & Format(iNo, "00") & "_H") & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KAMOKU_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KAMOKU" & Format(iNo, "00") & "_H") & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KOUZA_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KOUZA" & Format(iNo, "00") & "_H") & "'"

                        End If

                        If GFUNC_SELECT_SQL5(STR_SQL, 0) = False Then
                            Exit Function
                        End If

                        If OBJ_DATAREADER_DREAD3.HasRows = True Then
                            OBJ_DATAREADER_DREAD3.Read()

                            STR_SQL = " UPDATE  MAIN0500G_WORK SET "

                            If CHK_HIMOKU_NO <> OBJ_DATAREADER_DREAD3.Item("HIMOKU_NO_H") Then
                                STR_SQL += " HIMOKU_NAME_H = '*** 合算 ***',"
                            End If

                            '臨時データ時・通常時の決済金融機関情報は別項目から取得
                            '臨時データはYOBI1_HまたはYOBI2_H＆YOBI3_Hから取得 2005/06/28
                            If strFURI_KBN_LOCAL = "2" Then '入金
                                STR_SQL += " HIMOKU_KINGAKU_H = HIMOKU_KINGAKU_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += ", HIMOKU_FURI_KIN_H = HIMOKU_FURI_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                Else
                                    STR_SQL += ", HIMOKU_FUNOU_KIN_H = HIMOKU_FUNOU_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                End If
                                STR_SQL += " WHERE"
                                STR_SQL += " GAKKOU_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("GAKKOU_CODE_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KIN_CODE_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(0, 4) & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_TENPO_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(4, 3) & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KAMOKU_H ='01'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KOUZA_H = '9999999'"

                                iNo = 15 '費目ごとの金額は設定されていないのでiNO=15にしてループ終了
                            ElseIf strFURI_KBN_LOCAL = "3" Then '臨時出金
                                STR_SQL += " HIMOKU_KINGAKU_H = HIMOKU_KINGAKU_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += ", HIMOKU_FURI_KIN_H = HIMOKU_FURI_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                Else
                                    STR_SQL += ", HIMOKU_FUNOU_KIN_H = HIMOKU_FUNOU_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                End If
                                STR_SQL += " WHERE"
                                STR_SQL += " GAKKOU_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("GAKKOU_CODE_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KIN_CODE_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(0, 4) & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_TENPO_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(4, 3) & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KAMOKU_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(7, 2) & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KOUZA_H = '" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(9, 7) & "'"
                                iNo = 15 '費目ごとの金額は設定されていないのでiNO=15にしてループ終了
                            Else
                                STR_SQL += " HIMOKU_KINGAKU_H = HIMOKU_KINGAKU_H + " & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += ", HIMOKU_FURI_KIN_H = HIMOKU_FURI_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                Else
                                    STR_SQL += ", HIMOKU_FUNOU_KIN_H = HIMOKU_FUNOU_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                End If
                                STR_SQL += " WHERE"
                                STR_SQL += " GAKKOU_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("GAKKOU_CODE_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KIN_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KIN_CODE" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_TENPO_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_TENPO" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KAMOKU_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KAMOKU" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KOUZA_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KOUZA" & Format(iNo, "00") & "_H") & "'"

                            End If

                        Else
                            STR_SQL = "insert into MAIN0500G_WORK values("
                            STR_SQL += "'" & OBJ_DATAREADER_DREAD2.Item("GAKKOU_CODE_H") & "'"
                            '特別時と通常時は決済金融機関情報は別項目から取得
                            '臨時データはYOBI1_HまたはYOBI2_Hから取得 2005/06/28
                            If strFURI_KBN_LOCAL = "2" Then '入金

                                STR_SQL += ",'*** 合算 ***'"
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(0, 4) & "'"
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(4, 3) & "'"
                                STR_SQL += ",'01'"
                                'If IsDBNull(OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H")) = False Then
                                '    STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H") & "'"
                                'Else
                                STR_SQL += ",''" '受け取り名義人
                                'End If
                                STR_SQL += ",'9999999'"
                                STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                    STR_SQL += ",0"
                                Else
                                    STR_SQL += ",0"
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                End If
                                iNo = 15 '費目ごとの金額は設定されていないのでiNO=15にしてループ終了
                            ElseIf strFURI_KBN_LOCAL = "3" Then '臨時出金
                                STR_SQL += ",'*** 合算 ***'"
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(0, 4) & "'"
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(4, 3) & "'"
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(7, 2) & "'"
                                'If IsDBNull(OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H")) = False Then
                                '    STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H") & "'"
                                'Else
                                STR_SQL += ",'" & ConvNullToString(OBJ_DATAREADER_DREAD2.Item("YOBI3_H")) & "'" '受け取り名義人
                                'End If
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(9, 7) & "'"
                                STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                    STR_SQL += ",0"
                                Else
                                    STR_SQL += ",0"
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                End If
                                iNo = 15 '費目ごとの金額は設定されていないのでiNO=15にしてループ終了
                            Else
                                STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("HIMOKU_NAME" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KIN_CODE" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_TENPO" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KAMOKU" & Format(iNo, "00") & "_H") & "'"
                                If IsDBNull(OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H")) = False Then
                                    STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H") & "'"
                                Else
                                    STR_SQL += ",''"
                                End If
                                STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KOUZA" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                    STR_SQL += ",0"
                                Else
                                    STR_SQL += ",0"
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                End If
                            End If

                            STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("GAKUNEN_CODE_M") '追加 2005/06/16
                            STR_SQL += "," & iNo '追加 2005/06/16
                            STR_SQL += "," & CInt(strFURI_KBN_LOCAL) '追加 2005/06/28
                            STR_SQL += ",'" & Space(10) & "'" '追加 2005/06/28
                            STR_SQL += ",'" & Space(10) & "'" '追加 2005/06/28
                            STR_SQL += ",'" & Space(10) & "'" '追加 2005/06/28
                            STR_SQL += ")"
                        End If

                        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                            Exit Function
                        End If
                        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                            Exit Function
                        End If

                        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                            Exit Function
                        End If

                        If GFUNC_SELECT_SQL5("", 1) = False Then
                            Exit Function
                        End If
                    End If
                Next iNo
            End While
            If GFUNC_SELECT_SQL4(STR_SQL, 1) = False Then
                Exit Function
            End If
            '2006/12/08
        End While
        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        PFUNC_Himoku_Create = True

    End Function
    Function fn_select_G_SCHMAST() As Boolean
        '============================================================================
        'NAME           :fn_select_G_SCHMAST
        'Parameter      :
        'Description    :G_SCHMASTから出力項目抽出
        'Return         :True=OK,False=NG
        'Create         :2005/06/16
        'UPDATE         :
        '============================================================================
        fn_select_G_SCHMAST = False

        Dim SQL As New StringBuilder(128)

        SQL.Append(" SELECT ")
        SQL.Append(" COUNT(GAKKOU_CODE_S)")
        SQL.Append(",SUM(SYORI_KEN_S)")
        SQL.Append(",SUM(SYORI_KIN_S)")
        SQL.Append(",SUM(FURI_KEN_S)")
        SQL.Append(",SUM(FURI_KIN_S)")
        SQL.Append(",SUM(FUNOU_KEN_S)")
        SQL.Append(",SUM(FUNOU_KIN_S)")
        SQL.Append(",SUM(TESUU_KIN_S)")
        SQL.Append(",SUM(TESUU_KIN1_S)")
        SQL.Append(",SUM(TESUU_KIN2_S)")
        SQL.Append(",SUM(TESUU_KIN3_S)")
        SQL.Append(" FROM G_SCHMAST")
        SQL.Append(" WHERE KESSAI_FLG_S = '1'")
        SQL.Append(" AND TYUUDAN_FLG_S = '0'")
        SQL.Append(" AND KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'")
        SQL.Append(" AND TIME_STAMP_S = '" & sSyori_Date & "'")
        SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

        gdbcCONNECT.ConnectionString = STR_CONNECTION
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = SQL.ToString
        gdbCOMMAND.Connection = gdbcCONNECT
        gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ


        While (gdbrREADER.Read)
            lngSAKI_SUU = CLng(gdbrREADER.Item(0))
            lngALL_KEN = CLng(gdbrREADER.Item(1))
            lngALL_KIN = CLng(gdbrREADER.Item(2))
            lngFURI_ALL_KEN = CLng(gdbrREADER.Item(3))
            lngFURI_ALL_KIN = CLng(gdbrREADER.Item(4))
            lngFUNOU_ALL_KEN = CLng(gdbrREADER.Item(5))
            lngFUNOU_ALL_KIN = CLng(gdbrREADER.Item(6))
            'lngTESUU_ALL = CLng(gdbrREADER.Item(7))
            lngTESUU1 = CLng(gdbrREADER.Item(8))
            'lngTESUU2 = CLng(gdbrREADER.Item(9)) '郵送料
            'lngTESUU3 = CLng(gdbrREADER.Item(10)) '振込手数料

        End While

        If Err.Number = 0 Then
            fn_select_G_SCHMAST = True
            gdbcCONNECT.Close()
        Else
            fn_select_G_SCHMAST = False
            MessageBox.Show("検索中にエラーが発生しました" & vbCrLf & CStr(Err.Number) & " :" & Err.Description, STR_SYORI_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            gdbcCONNECT.Close()
        End If

    End Function

    Function fn_select_G_SCHMAST_NYU() As Boolean
        '============================================================================
        'NAME           :fn_select_G_SCHMAST_NYU
        'Parameter      :
        'Description    :G_SCHMASTから入金のスケジュール抽出
        'Return         :True=OK,False=NG
        'Create         :2005/06/28
        'UPDATE         :
        '============================================================================
        fn_select_G_SCHMAST_NYU = False

        Dim SQL As New StringBuilder(128)

        SQL.Append("SELECT ")
        SQL.Append(" * ")
        SQL.Append(" FROM G_SCHMAST")
        SQL.Append(" WHERE KESSAI_FLG_S = '0'")
        SQL.Append(" AND TYUUDAN_FLG_S = '0'")
        SQL.Append(" AND DATA_FLG_S = '1'")
        SQL.Append(" AND KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'")
        SQL.Append(" AND FURI_KBN_S = '2'")
        SQL.Append(" AND SYORI_KIN_S > 0 ")
        SQL.Append(" ORDER BY GAKKOU_CODE_S ASC ")


        gdbcCONNECT.ConnectionString = STR_CONNECTION
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = SQL.ToString
        gdbCOMMAND.Connection = gdbcCONNECT
        gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

        lngNYUKIN_SCH_CNT = 0

        While (gdbrREADER.Read)
            lngNYUKIN_SCH_CNT = lngNYUKIN_SCH_CNT + 1

        End While

        If Err.Number = 0 Then
            fn_select_G_SCHMAST_NYU = True
            gdbcCONNECT.Close()
        Else
            fn_select_G_SCHMAST_NYU = False
            MessageBox.Show("検索中にエラーが発生しました" & vbCrLf & CStr(Err.Number) & " :" & Err.Description, STR_SYORI_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            gdbcCONNECT.Close()
        End If

    End Function

    Function fn_select_G_PRTKESSAI() As Boolean
        '============================================================================
        'NAME           :fn_select_G_PRTKESSAI
        'Parameter      :
        'Description    :G_PRTKESSAIの存在チェック
        'Return         :True=OK,False=NG
        'Create         :2006/04/17
        'UPDATE         :2006/04/18 振替日を抽出条件に追加
        '============================================================================
        fn_select_G_PRTKESSAI = False
        Dim SQL As New StringBuilder(128)
        SQL.Append(" SELECT * FROM G_PRTKESSAI")
        SQL.Append(" WHERE GAKKOU_CODE_P = '" & Str_Prt_GAKKOU_CODE & "'")
        SQL.Append(" AND KESSAI_DATE_P = '" & Str_Prt_KESSAI_DATE & "'")
        SQL.Append(" AND KESSAI_KIN_CODE_P = '" & Str_Prt_KESSAI_KIN_CODE & "'")
        SQL.Append(" AND KESSAI_TENPO_P = '" & Str_Prt_KESSAI_TENPO & "'")
        SQL.Append(" AND KESSAI_KAMOKU_P = '" & Str_Prt_KESSAI_KAMOKU & "'")
        SQL.Append(" AND KESSAI_KOUZA_P = '" & Str_Prt_KESSAI_KOUZA & "'")
        SQL.Append(" AND FURI_DATE_P = '" & Str_Prt_FURI_DATE & "'")

        gdbcCONNECT.ConnectionString = STR_CONNECTION
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = SQL.ToString
        gdbCOMMAND.Connection = gdbcCONNECT
        gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

        lngG_PRTKESSAI_REC = 0
        While (gdbrREADER.Read)
            lngG_PRTKESSAI_REC = lngG_PRTKESSAI_REC + 1
        End While

        If Err.Number = 0 Then
            fn_select_G_PRTKESSAI = True
            gdbcCONNECT.Close()
        Else
            fn_select_G_PRTKESSAI = False
            MessageBox.Show("検索中にエラーが発生しました" & vbCrLf & CStr(Err.Number) & " :" & Err.Description, STR_SYORI_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            gdbcCONNECT.Close()
        End If

    End Function

    Function fn_COUNT_G_SCHMAST(ByVal astrGAKKOU_CODE As String) As Boolean
        '============================================================================
        'NAME           :fn_COUNT_G_SCHMAST
        'Parameter      :astrGAKKOU_CODE:学校コード
        'Description    :G_SCHMASTから出力項目抽出
        'Return         :True=OK,False=NG
        'Create         :2006/04/17
        'UPDATE         :
        '============================================================================
        Dim lngYUUSOU_CNT As Long

        fn_COUNT_G_SCHMAST = False

        Dim SQL As New StringBuilder(128)

        SQL.Append("SELECT SYORI_KEN_S,SYORI_KIN_S")
        SQL.Append(" ,FURI_KEN_S,FURI_KIN_S,FUNOU_KEN_S,FUNOU_KIN_S")
        SQL.Append(" ,TESUU_KIN_S,TESUU_KIN1_S,TESUU_KIN2_S,TESUU_KIN3_S")
        SQL.Append(" FROM G_SCHMAST")
        SQL.Append(" WHERE TYUUDAN_FLG_S = '0'")
        SQL.Append(" AND KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'")
        SQL.Append(" AND GAKKOU_CODE_S = '" & astrGAKKOU_CODE & "'")
        SQL.Append(" AND SCH_KBN_S <> '2'") '2007/02/10
        SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

        gdbcCONNECT.ConnectionString = STR_CONNECTION
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = SQL.ToString
        gdbCOMMAND.Connection = gdbcCONNECT
        gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

        lngSYORIKEN_TOKUBETU = 0
        dblSYORIKIN_TOKUBETU = 0
        lngFURIKEN_TOKUBETU = 0
        dblFURIKIN_TOKUBETU = 0
        lngFUNOUKEN_TOKUBETU = 0
        dblFUNOUKIN_TOKUBETU = 0
        dblTESUU_TOKUBETU = 0
        dblTESUU_A1_TOKUBETU = 0
        dblTESUU_A2_TOKUBETU = 0
        dblTESUU_A3_TOKUBETU = 0
        lngYUUSOU_CNT = 0

        While (gdbrREADER.Read)
            lngSYORIKEN_TOKUBETU = lngSYORIKEN_TOKUBETU + CLng(gdbrREADER.Item(0))
            dblSYORIKIN_TOKUBETU = dblSYORIKIN_TOKUBETU + CDbl(gdbrREADER.Item(1))
            lngFURIKEN_TOKUBETU = lngFURIKEN_TOKUBETU + CLng(gdbrREADER.Item(2))
            dblFURIKIN_TOKUBETU = dblFURIKIN_TOKUBETU + CDbl(gdbrREADER.Item(3))
            lngFUNOUKEN_TOKUBETU = lngFUNOUKEN_TOKUBETU + CLng(gdbrREADER.Item(4))
            dblFUNOUKIN_TOKUBETU = dblFUNOUKIN_TOKUBETU + CDbl(gdbrREADER.Item(5))

            dblTESUU_TOKUBETU = dblTESUU_TOKUBETU + CDbl(gdbrREADER.Item(6))
            dblTESUU_A1_TOKUBETU = dblTESUU_A1_TOKUBETU + CDbl(gdbrREADER.Item(7))
            dblTESUU_A2_TOKUBETU = dblTESUU_A2_TOKUBETU + CDbl(gdbrREADER.Item(8))
            dblTESUU_A3_TOKUBETU = dblTESUU_A3_TOKUBETU + CDbl(gdbrREADER.Item(9))
            lngYUUSOU_CNT += 1
        End While
        '郵送料は一振替日で一回取るので、スケジュール分で割ったものが郵送料
        dblTESUU_A2_TOKUBETU = dblTESUU_A2_TOKUBETU / lngYUUSOU_CNT

        If Err.Number = 0 Then
            fn_COUNT_G_SCHMAST = True
            gdbcCONNECT.Close()
        Else
            fn_COUNT_G_SCHMAST = False
            MessageBox.Show("検索中にエラーが発生しました" & vbCrLf & CStr(Err.Number) & " :" & Err.Description, STR_SYORI_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            gdbcCONNECT.Close()
        End If

    End Function

    Private Function PFUNC_UPDATE_PrtKessai(ByVal astrGAKKOU_CODE As String, ByVal alngFURI_TESUU As Long) As Boolean

        PFUNC_UPDATE_PrtKessai = False

        STR_SQL = " UPDATE  G_PRTKESSAI SET "
        STR_SQL += " FURIKOMI_TESUU_P = " & alngFURI_TESUU
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_P  = '" & astrGAKKOU_CODE & "'"

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        PFUNC_UPDATE_PrtKessai = True

    End Function

#End Region

End Class
