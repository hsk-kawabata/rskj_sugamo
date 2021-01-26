Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKMAIN050

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKMAIN050", "資金決済データ作成画面")
    Private Const msgTitle As String = "資金決済データ作成画面(KFKMAIN050)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private Const ThisModuleName As String = "KFKMAIN050.vb"

    Private KESSAI_DATE As String             '決済日

    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース
    'クリックした列の番号
    Dim ClickedColumn As Integer
    'ソートオーダーフラグ
    Dim SortOrderFlag As Boolean = True

    Private BATCHSV As String            '2010.01.27 追加
    Private RIENTASV As String           '2010.01.27 追加

    Structure KeyInfo

        Dim TORIS_CODE As String            '取引先主コード
        Dim TORIF_CODE As String            '取引先副コード
        Dim FURI_DATE As String             '振替日
        Dim KESSAI_YDATE As String          '決済予定日
        Dim TESUU_YDATE As String           '手数料徴求予定日
        Dim TESUU_KIN As String             '手数料金額　：手数料合計
        Dim TESUU_KIN1 As String            '手数料金額１：引落手数料
        Dim TESUU_KIN2 As String            '手数料金額２：送料
        Dim TESUU_KIN3 As String            '手数料金額３：振込手数料
        Dim FURI_KEN As String              '振込済件数
        Dim FURI_KIN As String              '振込済金額
        Dim KIGYO_CODE As String            '企業コード
        Dim BAITAI_CODE As String           '媒体コード
        Dim SYUBETU_CODE As String          '種別コード
        Dim ITAKU_CODE As String            '委託者コード
        Dim ITAKU_NNAME As String           '委託者名漢字
        Dim ITAKU_KNAME As String           '委託者名カナ
        Dim FURI_CODE As String             '振替コード
        Dim NS_KBN As String                '入出金区分
        Dim TESUUTYO_PATN As String         '手数料徴求方法
        Dim TESUUTYO_KBN As String          '手数料徴求区分
        Dim KESSAI_KBN As String            '決済区分
        Dim TORIMATOME_SIT_NO As String     'とりまとめ店
        Dim HONBU_KOUZA As String           '本部別段口座番号
        Dim TUKEKIN_NO As String            '決済金融機関
        Dim TUKESIT_NO As String            '決済支店
        Dim TUKEKAMOKU As String            '決済科目
        Dim TUKEKOUZA As String             '決済口座番号
        Dim TUKEMEIGI As String             '決済名義人（カナ）
        Dim BIKOU1 As String                '備考１
        Dim BIKOU2 As String                '備考２
        Dim TSUUTYOSIT_NO As String         '手数料徴求支店
        Dim TSUUTYOKAMOKU As String         '手数料徴求科目
        Dim TSUUTYOKOUZA As String          '手数料徴求口座番号

        Dim KESSAI_TORI_KBN As String       '0：資金決済と手数料徴求の両方
        '                                    1：資金決済のみ
        '                                    2:手数料徴求のみ
        Dim TESUUTYO_FLG As String          '手数料徴求済フラグ
        Dim RECTUUBAN As Long               'レコード番号

        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
        End Sub

        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_SV1").PadRight(7)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_SV1").PadRight(2)
            FURI_DATE = oraReader.GetString("FURI_DATE_SV1").PadRight(8)
            KESSAI_YDATE = oraReader.GetString("KESSAI_YDATE_SV1")
            TESUU_YDATE = oraReader.GetString("TESUU_YDATE_SV1")
            TESUU_KIN = oraReader.GetString("TESUU_KIN_SV1")
            TESUU_KIN1 = oraReader.GetString("TESUU_KIN1_SV1")
            TESUU_KIN2 = oraReader.GetString("TESUU_KIN2_SV1")
            TESUU_KIN3 = oraReader.GetString("TESUU_KIN3_SV1")
            FURI_KEN = oraReader.GetString("FURI_KEN_SV1")
            FURI_KIN = oraReader.GetString("FURI_KIN_SV1")
            KIGYO_CODE = oraReader.GetString("KIGYO_CODE_TV1")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_TV1")
            SYUBETU_CODE = oraReader.GetString("SYUBETU_CODE_TV1")
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_TV1")
            ITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_TV1")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_TV1")
            FURI_CODE = oraReader.GetString("FURI_CODE_TV1")
            NS_KBN = oraReader.GetString("NS_KBN_TV1")
            TESUUTYO_PATN = oraReader.GetString("TESUUTYO_PATN_TV1")
            TESUUTYO_KBN = oraReader.GetString("TESUUTYO_KBN_TV1")
            KESSAI_KBN = oraReader.GetString("KESSAI_KBN_TV1")
            TORIMATOME_SIT_NO = oraReader.GetString("TORIMATOME_SIT_NO_TV1")
            HONBU_KOUZA = oraReader.GetString("HONBU_KOUZA_TV1")
            TUKEKIN_NO = oraReader.GetString("TUKEKIN_NO_TV1")
            TUKESIT_NO = oraReader.GetString("TUKESIT_NO_TV1")
            TUKEKAMOKU = oraReader.GetString("TUKEKAMOKU_TV1")
            TUKEKOUZA = oraReader.GetString("TUKEKOUZA_TV1")
            TUKEMEIGI = oraReader.GetString("TUKEMEIGI_TV1")
            BIKOU1 = oraReader.GetString("BIKOU1_TV1")
            BIKOU2 = oraReader.GetString("BIKOU2_TV1")
            TSUUTYOSIT_NO = oraReader.GetString("TSUUTYOSIT_NO_TV1")
            TSUUTYOKAMOKU = oraReader.GetString("TSUUTYOKAMOKU_TV1")
            TSUUTYOKOUZA = oraReader.GetString("TSUUTYOKOUZA_TV1")
            KESSAI_TORI_KBN = oraReader.GetString("KESSAI_TORI_KBN")

        End Sub

    End Structure
    '

#Region "関数"
    ''' <summary>
    ''' 決済データの抽出
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function KessaiDataList() As Integer

        Dim OraKesReader As New CASTCommon.MyOracleReader(MainDB)
        Dim ROW As Integer = 0
        Dim Data(7) As String


        Try

            Dim SQL As StringBuilder
            Dim CommonSQL As StringBuilder          '抽出条件（共通）
            Dim TudoWhereSQL As StringBuilder       '抽出条件（決済・手数料同時）
            Dim KessaiOnlyWhereSQL As StringBuilder '抽出条件（決済のみ）
            Dim TesuuOnlyWhereSQL As StringBuilder  '抽出条件（手数料のみ）
            Dim IkkatuWhereSQL As StringBuilder     '抽出条件（決済・手数料同時(一括)）

            SQL = New StringBuilder(128)                    '文字列を扱うクラスStringBuilder（高速に文字列の追加、挿入を行う）
            CommonSQL = New StringBuilder(128)
            TudoWhereSQL = New StringBuilder(128)
            KessaiOnlyWhereSQL = New StringBuilder(128)
            TesuuOnlyWhereSQL = New StringBuilder(128)
            IkkatuWhereSQL = New StringBuilder(128)

            CommonSQL.Append(" SELECT")
            CommonSQL.Append(" TORIS_CODE_SV1")
            CommonSQL.Append(", TORIF_CODE_SV1")
            CommonSQL.Append(", FURI_DATE_SV1")
            CommonSQL.Append(", KESSAI_DATE_SV1")
            CommonSQL.Append(", KESSAI_YDATE_SV1")
            CommonSQL.Append(", TESUU_DATE_SV1")
            CommonSQL.Append(", TESUU_YDATE_SV1")
            CommonSQL.Append(", TESUUKEI_FLG_SV1")
            CommonSQL.Append(", KESSAI_FLG_SV1")
            CommonSQL.Append(", TESUUTYO_FLG_SV1")
            CommonSQL.Append(", TYUUDAN_FLG_SV1")
            CommonSQL.Append(", FURI_KIN_SV1")
            CommonSQL.Append(", TESUUTYO_KBN_TV1")
            CommonSQL.Append(", ITAKU_NNAME_TV1")
            CommonSQL.Append(", KIGYO_CODE_SV1")
            CommonSQL.Append(", FURI_CODE_SV1")
            CommonSQL.Append(", ' ' TEIKEI_KBN")
            CommonSQL.Append(", KESSAI_KBN_TV1")

            ' 2017/07/25 タスク）綾部 ADD (RSV2標準対応 No.8(学校諸会費 随時出金対象外)) -------------------- START
            CommonSQL.Append(", BAITAI_CODE_TV1")
            ' 2017/07/25 タスク）綾部 ADD (RSV2標準対応 No.8(学校諸会費 随時出金対象外)) -------------------- END

            ' ★資金決済と手数料徴求を同時に行う条件SQL
            TudoWhereSQL.Append(" FROM V1_KESMAST")
            TudoWhereSQL.Append(" WHERE KESSAI_YDATE_SV1 = " & SQ(KESSAI_DATE))                                         '決済予定日（=入力日）
            TudoWhereSQL.Append(" AND TESUU_YDATE_SV1 = " & SQ(KESSAI_DATE))                                             '手数料予定日（=入力日）
            TudoWhereSQL.Append(" AND TESUUKEI_FLG_SV1 = '1'")                                                          '手数料計算フラグ（手数料計算済み）
            TudoWhereSQL.Append(" AND ( (KESSAI_FLG_SV1 = '0' AND TESUUTYO_FLG_SV1 = '0') ")                            '決済フラグ（未決済）
            TudoWhereSQL.Append(" OR (KESSAI_FLG_SV1 = '2' AND TESUUTYO_FLG_SV1 = '2') )")                              '手数料徴求フラグ（手数料未徴求）
            TudoWhereSQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")                                                           '中断フラグ（中断なし）
            TudoWhereSQL.Append(" AND FURI_KIN_SV1 > 0")                                                                '振替済み金額（振替済金額あり）
            TudoWhereSQL.Append(" AND KESSAI_KBN_TV1 <> '99'")                                                          '決済対象外ではない 2009.10.28

            ' ★資金決済だけを行う条件SQL
            KessaiOnlyWhereSQL.Append(" FROM V1_KESMAST")
            KessaiOnlyWhereSQL.Append(" WHERE KESSAI_YDATE_SV1 = " & SQ(KESSAI_DATE))                                    '決済予定日（=入力日）
            'KessaiOnlyWhereSQL.Append(" AND (TESUU_YDATE_SV1 != " & SQ(KESSAI_DATE))
            KessaiOnlyWhereSQL.Append(" AND (TESUUTYO_KBN_TV1 = '1' OR TESUUTYO_KBN_TV1 = '2' OR TESUUTYO_KBN_TV1 = '3') ")
            KessaiOnlyWhereSQL.Append(" AND TESUUKEI_FLG_SV1 = '1'")                                                    '手数料計算フラグ（手数料計算済み）
            KessaiOnlyWhereSQL.Append(" AND ((KESSAI_FLG_SV1 = '0' AND TESUUTYO_FLG_SV1 = '0') ")                      '決済フラグ（未決済）
            KessaiOnlyWhereSQL.Append(" OR (KESSAI_FLG_SV1 = '2' AND TESUUTYO_FLG_SV1 = '0') )")                        '決済フラグ（手数料はなし） 
            KessaiOnlyWhereSQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")                                                     '中断フラグ（中断なし）
            KessaiOnlyWhereSQL.Append(" AND FURI_KIN_SV1 > 0")                                                          '振替済み金額（振替済金額あり）
            KessaiOnlyWhereSQL.Append(" AND KESSAI_KBN_TV1 <> '99'")                                                     '決済対象外ではない 2009.10.28

            '***手数料のみは抽出しない 2009.10.28 start
            '' ★手数料徴求だけを行う条件SQL
            'TesuuOnlyWhereSQL.Append(" FROM V1_KESMAST")
            'TesuuOnlyWhereSQL.Append(" WHERE KESSAI_YDATE_SV1 != " & SQ(KESSAI_DATE))                                   '決済予定日（=入力日）
            'TesuuOnlyWhereSQL.Append(" AND TESUU_YDATE_SV1 = " & SQ(KESSAI_DATE))                                       '手数料予定日（≠入力日）
            'TesuuOnlyWhereSQL.Append(" AND TESUUKEI_FLG_SV1 = '1'")                                                    '手数料計算フラグ（手数料計算済み）
            'TesuuOnlyWhereSQL.Append(" AND ( ( KESSAI_FLG_SV1 = '1'")                                                   '決済フラグ（未決済）            
            'TesuuOnlyWhereSQL.Append(" AND TESUUTYO_FLG_SV1 = '0') ")                                                   '手数料徴求フラグ（手数料未徴求）
            'TesuuOnlyWhereSQL.Append(" OR (KESSAI_FLG_SV1 = '1' AND TESUUTYO_FLG_SV1 = '2') )")                         '決済フラグ（手数料はなし） '2008/08/08 浜松信金　tesuutyo_FLG='2'追加
            'TesuuOnlyWhereSQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")                                                     '中断フラグ（中断なし）
            'TesuuOnlyWhereSQL.Append(" AND FURI_KIN_SV1 > 0")                                                          '振替済み金額（振替済金額あり）
            '***手数料のみは抽出しない 2009.10.28 end

            ' ★手数料徴求区分が1:一括徴求の場合、資金決済と手数料徴求を同時に行う条件SQL
            'IkkatuWhereSQL.Append(" FROM V1_KESMAST")
            'IkkatuWhereSQL.Append(" WHERE KESSAI_YDATE_SV1 = " & SQ(KESSAI_DATE))                                       '決済予定日（=入力日）
            'IkkatuWhereSQL.Append(" AND TESUU_YDATE_SV1 = " & SQ(KESSAI_DATE))                                          '手数料予定日（=入力日）
            'IkkatuWhereSQL.Append(" AND TESUUKEI_FLG_SV1 = '1'")                                                        '手数料計算フラグ（手数料計算済み）
            'IkkatuWhereSQL.Append(" AND ( (KESSAI_FLG_SV1 = '0' AND TESUUTYO_FLG_SV1 = '0') ")                          '決済フラグ（未決済）
            'IkkatuWhereSQL.Append(" OR (KESSAI_FLG_SV1 = '2' AND TESUUTYO_FLG_SV1 = '2') ) ")                           '手数料徴求フラグ（手数料未徴求）
            'IkkatuWhereSQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")                                                         '中断フラグ（中断なし）
            'IkkatuWhereSQL.Append(" AND FURI_KIN_SV1 > 0")                                                              '振替済み金額（振替済金額あり）
            'IkkatuWhereSQL.Append(" AND TESUUTYO_KBN_TV1 = '1'")                                                        '手数料徴求区分（一括徴求）
            'IkkatuWhereSQL.Append(" AND KESSAI_KBN_TV1 <> '99'")                                                        '決済対象外ではない 2009.10.28

            'SQL文の作成
            SQL.Append(CommonSQL)
            SQL.Append(TudoWhereSQL)
            SQL.Append(" UNION")
            SQL.Append(CommonSQL)
            SQL.Append(KessaiOnlyWhereSQL)
            '***手数料のみは抽出しない 2009.10.28 start
            'SQL.Append(" UNION")
            'SQL.Append(CommonSQL)
            'SQL.Append(TesuuOnlyWhereSQL)
            'SQL.Append(" UNION")
            'SQL.Append(CommonSQL)
            'SQL.Append(IkkatuWhereSQL)
            '***手数料のみは抽出しない 2009.10.28 end

            SQL.Append(" ORDER BY KIGYO_CODE_SV1 ASC, FURI_CODE_SV1 ASC")

            If OraKesReader.DataReader(SQL) = True Then

                Do While OraKesReader.EOF = False

                    ' 2017/07/25 タスク）綾部 ADD (RSV2標準対応 No.8(学校諸会費 随時出金対象外)) -------------------- START
                    If GCom.NzStr(OraKesReader.GetItem("BAITAI_CODE_TV1")) = "07" And
                       GCom.NzDec(OraKesReader.GetItem("TORIF_CODE_SV1"), 0).ToString("00") = "04" Then
                        '--------------------------------------------------------------------------------
                        ' 学校諸会費の「随時出金」は資金決済対象外のため、次レコードへ
                        '--------------------------------------------------------------------------------
                    Else
                        ' 2017/07/25 タスク）綾部 ADD (RSV2標準対応 No.8(学校諸会費 随時出金対象外)) -------------------- END

                        Dim Temp As String

                        ROW += 1

                        'ListViewの値設定
                        Dim strTESUUTYO_KBN_TV1 As String   '手数料徴求区分
                        Dim strKESSAI_FLG_SV1 As String     '決済フラグ
                        Dim strTESUU_YDATE_SV1 As String    '手数料予定日

                        strTESUUTYO_KBN_TV1 = GCom.NzStr(OraKesReader.GetItem("TESUUTYO_KBN_TV1")).Trim '手数料徴求区分
                        strKESSAI_FLG_SV1 = GCom.NzStr(OraKesReader.GetItem("KESSAI_FLG_SV1")).Trim
                        strTESUU_YDATE_SV1 = GCom.NzStr(OraKesReader.GetItem("TESUU_YDATE_SV1")).Trim

                        If strTESUUTYO_KBN_TV1 <> "0" Then
                            Data(1) = "2"           '抽出条件２(決済のみ)
                        Else
                            Data(1) = "1"           '抽出条件１(決済、手数料同時)
                        End If
                        '================================================================================================================================================================================

                        '取引先名
                        Temp = GCom.NzStr(OraKesReader.GetItem("ITAKU_NNAME_TV1")).Trim
                        Data(2) = GCom.GetLimitString(Temp, 40)
                        '取引先コード（主コード + "-" + 副コード）
                        Data(3) = GCom.NzDec(OraKesReader.GetItem("TORIS_CODE_SV1"), 0).ToString("0000000000")
                        Data(3) &= "-"
                        Data(3) &= GCom.NzDec(OraKesReader.GetItem("TORIF_CODE_SV1"), 0).ToString("00")
                        '2014/04/15 saitou 標準修正 UPD -------------------------------------------------->>>>
                        '発信データ作成画面と日付の表示方法を合わせる
                        '振替日
                        Dim strTempFuriDate As String = GCom.NzDec(OraKesReader.GetItem("FURI_DATE_SV1"), "")
                        Data(4) = strTempFuriDate.Substring(0, 4) & "/" & strTempFuriDate.Substring(4, 2) & "/" & strTempFuriDate.Substring(6, 2)
                        '決済予定日
                        Dim strTempKessaiYDate As String = GCom.NzStr(OraKesReader.GetItem("KESSAI_YDATE_SV1")).Trim.Substring(0, 8)
                        Data(5) = strTempKessaiYDate.Substring(0, 4) & "/" & strTempKessaiYDate.Substring(4, 2) & "/" & strTempKessaiYDate.Substring(6, 2)
                        '手数料徴求予定日
                        Dim strTempTesuuYDate As String = GCom.NzStr(OraKesReader.GetItem("TESUU_YDATE_SV1")).Trim.Substring(0, 8)
                        Data(6) = strTempTesuuYDate.Substring(0, 4) & "/" & strTempTesuuYDate.Substring(4, 2) & "/" & strTempTesuuYDate.Substring(6, 2)

                        ''振替日
                        'Data(4) = GCom.NzDec(OraKesReader.GetItem("FURI_DATE_SV1"), "")
                        ''決済予定日
                        'Data(5) = GCom.NzStr(OraKesReader.GetItem("KESSAI_YDATE_SV1")).Trim.Substring(0, 8)
                        ''手数料決済予定日
                        'Data(6) = GCom.NzStr(OraKesReader.GetItem("TESUU_YDATE_SV1")).Trim.Substring(0, 8)
                        '2014/04/15 saitou 標準修正 UPD --------------------------------------------------<<<<

                        '決済区分取得 決済区分
                        Select Case GCom.NzStr(OraKesReader.GetItem("KESSAI_KBN_TV1"))
                            Case "00"
                                Data(7) = "預け金"
                            Case "01"
                                Data(7) = "口座入金"
                            Case "02"
                                Data(7) = "為替振込"
                            Case "03"
                                Data(7) = "為替付替"
                            Case "04"
                                Data(7) = "別段出金のみ"
                            Case "05"
                                Data(7) = "特別企業"
                            Case "99"
                                Data(7) = "決済対象外"
                        End Select

                        Dim vLstItem As New ListViewItem(Data, -1, Color.Black, Color.White, Nothing)
                        ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                        ' 2017/07/25 タスク）綾部 ADD (RSV2標準対応 No.8(学校諸会費 随時出金対象外)) -------------------- START
                    End If
                    ' 2017/07/25 タスク）綾部 ADD (RSV2標準対応 No.8(学校諸会費 随時出金対象外)) -------------------- END

                    OraKesReader.NextRead()
                Loop
            Else
                Return 0    '0件
            End If

            Return ROW      '正常 件数

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(データ検索)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return -1

        Finally
            If Not OraKesReader Is Nothing Then OraKesReader.Close()
        End Try

    End Function
#End Region

    '画面LOAD時
    Private Sub KFKMAIN050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim strSysDate As String

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '------------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '休日マスタ取り込み
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '2010.01.27 追加 ********
            If Not SetIniFIle() Then
                Return
            End If
            '************************

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時にシステム日付を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            'リスト画面の初期化
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    '2010.01.27 追加 ********
    Private Function SetIniFIle() As Boolean
        Try

            BATCHSV = CASTCommon.GetFSKJIni("COMMON", "BATCHSV")
            If BATCHSV.ToUpper = "ERR" OrElse BATCHSV = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "バッチサーバ", "COMMON", "BATCHSV"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:BATCHSV")
                Return False
            End If

            RIENTASV = CASTCommon.GetFSKJIni("COMMON", "RIENTASV")
            If RIENTASV.ToUpper = "ERR" OrElse RIENTASV = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "リエンタサーバ", "COMMON", "RIENTASV"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:センター区分 分類:COMMON 項目:RIENTASV")
                Return False
            End If
 
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(設定ファイル取得)", "失敗", ex.ToString)
        End Try
    End Function
    '*************************

    '実行ボタン押下時
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            Dim nSelectItems As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            'リストに１件も表示されていないとき
            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                'リストに１件以上表示されているが、チェックされていないとき
                If nSelectItems.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            Dim dRet As DialogResult

            If RIENTASV = BATCHSV Then  'ジョブ起動
                dRet = MessageBox.Show(MSG0023I.Replace("{0}", "資金決済データ作成"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            Else                        'バッチ起動
                dRet = MessageBox.Show(MSG0015I.Replace("{0}", "資金決済データ作成"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            End If

            If Not dRet = DialogResult.OK Then
                Return
            End If

            MainDB = New CASTCommon.MyOracle
            Dim SQL As StringBuilder


            '***ｽｹｼﾞｭｰﾙﾏｽﾀの更新
            Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items

            For Each item As ListViewItem In nItems

                Dim FURI_DATE As String = GCom.NzDec(item.SubItems(4).Text, "")
                Dim Temp As String = GCom.NzDec(item.SubItems(3).Text, "")
                Dim TORIS_CODE As String = Temp.Substring(0, 10)
                Dim TORIF_CODE As String = Temp.Substring(10)
                Dim Jyouken As String = GCom.NzDec(item.SubItems(1).Text, "")
                SQL = New StringBuilder(128)
                LW.ToriCode = TORIS_CODE & "-" & TORIF_CODE
                LW.FuriDate = FURI_DATE

                SQL.Append("UPDATE SCHMAST")

                If item.Checked = True Then
                    Select Case Jyouken
                        Case "1"                                    '(決済、手数料同時)
                            SQL.Append(" SET KESSAI_FLG_S = '2'")
                            SQL.Append(", TESUUTYO_FLG_S = '2'")
                        Case "2"                                    '(決済のみ)
                            SQL.Append(" SET KESSAI_FLG_S = '2'")
                    End Select
                Else
                    Select Case Jyouken
                        Case "1"                                    '(決済、手数料同時)
                            SQL.Append(" SET KESSAI_FLG_S = '0'")
                            SQL.Append(", TESUUTYO_FLG_S = '0'")
                        Case "2"                                    '(決済のみ)
                            SQL.Append(" SET KESSAI_FLG_S = '0'")
                    End Select
                End If

                SQL.Append(" WHERE TORIS_CODE_S = '" & TORIS_CODE & "'")
                SQL.Append(" AND TORIF_CODE_S = '" & TORIF_CODE & "'")
                SQL.Append(" AND FURI_DATE_S = '" & FURI_DATE & "'")

                Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
                If nRet = 0 Then
                    MainDB.Rollback()
                    Return
                ElseIf nRet < 0 Then
                    Throw New Exception(MSG0002E.Replace("{0}", "登録"))
                End If
            Next

            Dim jobid As String
            Dim para As String

            ''2010.01.27 追加 *******************
            'iniファイルのRIENTASVを見て
            'ジョブ起動かバッチ起動か切り替える
            '************************************
            If RIENTASV = BATCHSV Then  'ジョブ起動

                'ジョブマスタに登録
                jobid = "K030"                      '..\Batch\資金決済データ作成\
                para = KESSAI_DATE                  '決済日をパラメタとして設定

                '#########################
                'job検索
                '#########################
                Dim iRet As Integer
                iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                If iRet = 1 Then
                    MainDB.Rollback()
                    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                ElseIf iRet = -1 Then
                    Throw New Exception(MSG0002E.Replace("{0}", "検索"))
                End If

                '#########################
                'job登録
                '#########################
                If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then
                    Throw New Exception(MSG0005E)
                End If

                MainDB.Commit()

                MessageBox.Show(MSG0021I.Replace("{0}", "資金決済データ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Else                        'バッチ起動

                MainDB.Commit()

                Dim ExePath As String = CASTCommon.GetFSKJIni("COMMON", "EXE")
                Dim l_exitcode As Long

                Dim myProcess As New Process
                Dim SInfo As New ProcessStartInfo(Path.Combine(ExePath, "KFK030.EXE"), KESSAI_DATE)

                SInfo.WorkingDirectory = ExePath
                SInfo.CreateNoWindow = True
                SInfo.UseShellExecute = False
                myProcess = Process.Start(SInfo)
                myProcess.WaitForExit()
                l_exitcode = myProcess.ExitCode

                If l_exitcode <> 0 Then

                    MessageBox.Show(MSG0034E.Replace("{0}", "資金決済データ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", "エラーコード： " & l_exitcode)

                    Return
                Else

                    MessageBox.Show(MSG0016I.Replace("{0}", "資金決済データ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                End If


            End If

            'リスト画面の初期化
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False

            Me.txtKijyunDateY.Enabled = True
            Me.txtKijyunDateM.Enabled = True
            Me.txtKijyunDateD.Enabled = True

        Catch ex As Exception

            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.Message)

        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try
    End Sub

    '終了ボタン押下時
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

#Region "KFJMAIN030用関数"

    Function fn_check_TEXT() As Boolean
        '============================================================================
        'NAME           :fn_check_TEXT
        'Parameter      :
        'Description    :実行ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :
        'Update         :
        '============================================================================

        Try
            '------------------------------------------------
            '決済年チェック
            '------------------------------------------------
            '必須チェック
            If txtKijyunDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '決済月チェック
            '------------------------------------------------
            '必須チェック
            If txtKijyunDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtKijyunDateM.Text >= 1 And txtKijyunDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '決済日チェック
            '------------------------------------------------
            '必須チェック
            If txtKijyunDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtKijyunDateD.Text >= 1 And txtKijyunDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = txtKijyunDateY.Text & "/" & txtKijyunDateM.Text & "/" & txtKijyunDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(fn_check_TEXT)", "失敗", ex.Message)
            Return False

        Finally

        End Try

        Return True

    End Function

    'ゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKijyunDateY.Validating, txtKijyunDateM.Validating, txtKijyunDateD.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub
    '一覧表示領域のソート(2009/10/09)追加
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
                .ListViewItemSorter = New CASTCommon.ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)

                ' ソート実行
                .Sort()

            End With

        Catch ex As Exception
            Throw
        End Try

    End Sub
#End Region

    Private Sub btnAllon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOn.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)開始", "成功", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = True '((CType(sender, Button).Name.ToUpper = "BTNALLON")) OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)", "失敗", ex.Message)
        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)終了", "成功", "")

        End Try

    End Sub

    Private Sub btnAlloff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOff.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)開始", "成功", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = False 'OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)", "失敗", ex.Message)
        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)終了", "成功", "")

        End Try

    End Sub

    Private Sub btnRef_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRef.Click

        Dim iRet As Integer

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")


            'テキストボックス入力チェック
            If fn_check_TEXT() = False Then
                Return
            End If

            'リスト画面の初期化
            Me.ListView1.Items.Clear()

            KESSAI_DATE = txtKijyunDateY.Text & txtKijyunDateM.Text & txtKijyunDateD.Text

            MainDB = New CASTCommon.MyOracle

            iRet = KessaiDataList()

            If iRet = 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return
            ElseIf iRet = -1 Then

                Return

            Else

                Me.btnRef.Enabled = False
                Me.btnClear.Enabled = True
                Me.btnAllOn.Enabled = True
                Me.btnAllOff.Enabled = True
                Me.btnAction.Enabled = True
                Me.txtKijyunDateY.Enabled = False
                Me.txtKijyunDateM.Enabled = False
                Me.txtKijyunDateD.Enabled = False
                Me.btnAction.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)", "失敗", ex.Message)

        Finally

            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")

        End Try

    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)開始", "成功", "")

            '基準日にシステム日付を表示
            '休日マスタ取り込み
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時に振替日に前営業日を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            'リスト画面の初期化
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False
            Me.txtKijyunDateY.Enabled = True
            Me.txtKijyunDateM.Enabled = True
            Me.txtKijyunDateD.Enabled = True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)終了", "成功", "")
        End Try

    End Sub

End Class
