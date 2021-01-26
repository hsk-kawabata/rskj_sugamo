' 2016/01/14 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
Option Explicit On
Option Strict On

Imports System
Imports System.Data.OracleClient
Imports System.Text
Imports CASTCommon

Public Class ClsSchduleMaintenanceClass

    'スケジュールマスタ内容   
    Public MaxColumn As Integer = 0
    Public ORANAME() As String         'ORACLE Column Name
    Public ORATYPE() As String         'ORACLE Column TYPE
    Public ORASIZE() As Integer        'ORACLE Column SIZE
    Public SCHMAST() As String         'SCHMAST Current Column Value

    '一次抽出取引先マスタ配列
    Public Structure TORIMAST_RECORD
        Dim FSYORI_KBN As String                '振替処理区分
        Dim TORIS_CODE As String                '取引先主コード
        Dim TORIF_CODE As String                '取引先副コード
        Dim BAITAI_CODE As String               '媒体コード
        Dim ITAKU_KANRI_CODE As String          '代表委託者コード
        Dim FMT_KBN As Integer                  'フォーマット区分
        Dim SYUBETU As String                   '種別コード
        Dim ITAKU_CODE As String                '委託者コード
        Dim ITAKU_KNAME As String               '委託者カナ名
        Dim TKIN_NO As String                   '取扱金融機関
        Dim TSIT_NO As String                   '取扱店番
        Dim MOTIKOMI_KBN As String              '持込区分
        Dim SOUSIN_KBN As String                '送信区分
        Dim TAKO_KBN As Integer                 '他行区分
        Dim FURI_CODE As String                 '振替コード
        Dim KIGYO_CODE As String                '企業コード
        Dim ITAKU_NNAME As String               '委託者漢字名
        Dim FURI_KYU_CODE As Integer            '振替休日コード
        Dim DATEN() As Integer                  'N日の有効／無効
        Dim MONTH_FLG As Integer                '該当月の値？
        Dim SFURI_FLG As Integer                '再振契約
        Dim SFURI_FCODE As String               '再振副コード
        Dim SFURI_DAY As Integer                '日数／基準日（再振）
        Dim SFURI_KIJITSU As Integer             '日付区分（再振）
        Dim SFURI_KYU_CODE As Integer            '再振休日シフト
        Dim KEIYAKU_DATE As String              '契約日
        Dim MOTIKOMI_KIJITSU As Integer         '持込期日
        Dim IRAISHO_YDATE As Integer            '日数／基準日（依頼書）
        Dim IRAISHO_KIJITSU As String           '日付区分（依頼書）
        Dim IRAISHO_KYU_CODE As String          '依頼書休日シフト
        Dim KESSAI_KBN As Integer               '決済区分
        Dim KESSAI_DAY As Integer               '日数／基準日（決済）
        Dim KESSAI_KIJITSU As Integer            '日付区分（決済）
        Dim KESSAI_KYU_CODE As Integer           '決済日休日シフト
        Dim TESUUTYO_KBN As Integer             '手数料徴求区分
        Dim TESUUTYO_PATN As Integer            '手数料徴求方法(2008.03.06)
        Dim TESUUMAT_NO As Integer              '手数料集計周期
        Dim TESUUTYO_DAY As Integer             '手数料徴求日数／基準日
        Dim TESUUTYO_KIJITSU As Integer         '手数料徴求期日区分
        Dim TESUU_KYU_CODE As Integer           '手数料徴求日休日コード
        Dim TESUUMAT_MONTH As Integer           '集計基準月(2008.03.06)
        Dim TESUUMAT_ENDDAY As Integer          '集計終了日(2008.03.06)
        Dim TESUUMAT_KIJYUN As Integer          '集計基準(2008.03.06)

    End Structure
    Public TR() As TORIMAST_RECORD

    '登録／判定結果情報
    Public Structure SCHMAST_Data
        Dim YUUKOU_FLG As Integer           '有効フラグ判定
        Dim KFURI_DATE As String            '契約振替日
        Dim FURI_DATE As String             '振替日
        Dim NFURI_DATE As String            '変更振替日
        Dim MOTIKOMI_SEQ As Integer         '持込SEQ
        Dim IRAISYOK_YDATE As String        '依頼書回収予定日
        Dim HAISIN_YDATE As String          '配信処理予定日自行分
        Dim HAISIN_T1YDATE As String        '配信処理予定日他行提携内分
        Dim HAISIN_T2YDATE As String        '配信処理予定日他行提携外分
        Dim FUNOU_YDATE As String           '不能処理予定日自行分
        Dim FUNOU_T1YDATE As String         '不能処理予定日他行提携内分
        Dim FUNOU_T2YDATE As String         '不能処理予定日他行提携外分
        Dim HENKAN_YDATE As String          '返還予定日
        Dim KESSAI_YDATE As String          '決済予定日
        Dim TESUU_YDATE As String           '手数料徴収予定日
        Dim KSAIFURI_DATE As String         '再振予定日
        Dim MOTIKOMI_DATE As String         '持込期日

        'FJHからの仕様待ち
        Dim KAKUHO_YDATE_S As String        '資金確保予定日
        Dim HASSIN_YDATE_S As String        '発信予定日

        Dim WRK_SFURI_YDATE As String            '再振日2009.10.05
    End Structure
    Public SCH As SCHMAST_Data

    Public Enum OPT
        OptionNothing = 0               '個別登録画面
        OptionAddNew = 1                '新規・再作成
        OptionAppend = 2                '追加作成
    End Enum

    Private Const ThisModuleName As String = "ClsSchduleMaintenance.vb"

    'どの業務から呼び出されるかの識別変数
    Private SchTable As Integer = 0

    Public Enum APL
        JifuriApplication = 1
        SofuriApplication = 3
    End Enum

    Public WriteOnly Property SetSchTable() As Integer
        Set(ByVal Value As Integer)
            SchTable = Value
        End Set
    End Property

    '
    ' 機能　 ： SCHMAST項目名の蓄積
    '
    ' 引数　 ： なし
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： 2007.12.12 共通化
    '
    Public Sub SetSchMastInformation()

        Dim REC As OracleDataReader = Nothing

        Try
            Dim SQL As String = "SELECT COLUMN_NAME"
            SQL &= ", DATA_TYPE"
            SQL &= ", DECODE(DATA_TYPE, 'CHAR', DATA_LENGTH, DATA_PRECISION) DATA_SIZE"
            SQL &= " FROM ALL_TAB_COLUMNS"
            SQL &= " WHERE UPPER(OWNER) = 'KZFMAST'"        '注意 2009.09.15 とりあえずこのまま
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL &= " AND TABLE_NAME = 'SCHMAST'"
                Case Is = APL.SofuriApplication
                    SQL &= " AND TABLE_NAME = 'S_SCHMAST'"
            End Select
            SQL &= " ORDER BY COLUMN_ID ASC"
            Dim Ret As Boolean = GCom.SetDynaset(SQL, REC)
            If Ret Then
                Do While REC.Read
                    Select Case MaxColumn
                        Case 0
                            ReDim ORANAME(0)
                            ReDim ORATYPE(0)
                            ReDim ORASIZE(0)
                        Case Else
                            ReDim Preserve ORANAME(MaxColumn)
                            ReDim Preserve ORATYPE(MaxColumn)
                            ReDim Preserve ORASIZE(MaxColumn)
                    End Select
                    ORANAME(MaxColumn) = GCom.NzStr(REC.Item("COLUMN_NAME")).Trim.ToUpper
                    ORATYPE(MaxColumn) = GCom.NzStr(REC.Item("DATA_TYPE")).Trim.ToUpper
                    ORASIZE(MaxColumn) = GCom.NzInt(REC.Item("DATA_SIZE"), 0)
                    MaxColumn += 1
                Loop
                MaxColumn -= 1
                ReDim SCHMAST(MaxColumn)
            End If
        Catch ex As Exception
            Throw
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
    End Sub

    '
    ' 機能　 ： 休日情報の蓄積
    '
    ' 引数　 ： ARG1 - 振替日データ(画面指定)
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： 判定前に蓄積すること
    '
    Public Sub SetKyuzituInformation(Optional ByVal FormFuriDate As Date = MenteCommon.clsCommon.BadResultDate)

        Try
            Select Case FormFuriDate
                Case MenteCommon.clsCommon.BadResultDate
                    '全休日情報を蓄積する。
                    Dim BRet As Boolean = GCom.CheckDateModule(Nothing, 1)
                Case Else
                    '該当月前後の休日情報のだけを蓄積する。
                    Dim ProviousMonthDate As Date = FormFuriDate.AddMonths(-1)
                    Dim NextMonthDate As Date = FormFuriDate.AddMonths(1)

                    Dim SQL As String = " WHERE SUBSTR(YASUMI_DATE_Y, 1, 6)"
                    SQL &= " IN ('" & String.Format("{0:yyyyMM}", ProviousMonthDate) & "'"
                    SQL &= ", '" & String.Format("{0:yyyyMM}", FormFuriDate) & "'"
                    SQL &= ", '" & String.Format("{0:yyyyMM}", NextMonthDate) & "')"
                    Dim BRet As Boolean = GCom.CheckDateModule(Nothing, 1, SQL)
            End Select
        Catch ex As Exception

            Throw

        End Try
    End Sub

    '
    ' 機能　 ： スケジュール作成対象の取引先コードを抽出
    '
    ' 引数　 ： ARG1 - 振替日年月
    ' 　　　 　 ARG2 - 取引先主コード
    ' 　　　 　 ARG3 - 取引先副コード
    ' 　　　 　 ARG4 - 呼出元識別指定
    '
    ' 戻り値 ： OK = True, NG = False(対象取引先なし)
    '
    ' 備考　 ： 該当の月が対象のもので振替日が開始／終了の間にあるもの
    '
    Public Function GET_SELECT_TORIMAST(ByVal FormFuriDate As Date, _
            ByVal TORIS_CODE As String, ByVal TORIF_CODE As String, _
            Optional ByVal SEL_OPTION As Integer = OPT.OptionNothing) As Boolean

        With GCom.GLog
            .Job2 = "スケジュール作成対象取引先コード抽出"
        End With
        Dim REC As OracleDataReader = Nothing
        Try
            '該当月
            Dim MonthColumn As String = "TUKI" & FormFuriDate.Month.ToString & "_T"

            Dim SQL As String = SetToriMastSelectBaseSql(MonthColumn)
            Select Case SchTable
                Case Is = APL.JifuriApplication
                    SQL &= " WHERE FSYORI_KBN_T = '1'"
                Case Is = APL.SofuriApplication
                    SQL &= " WHERE FSYORI_KBN_T = '3'"
            End Select

            Select Case SEL_OPTION
                Case OPT.OptionNothing
                    '個別登録画面
                    SQL &= " AND TORIS_CODE_T = '" & TORIS_CODE & "'"
                    SQL &= " AND TORIF_CODE_T = '" & TORIF_CODE & "'"
                Case OPT.OptionAddNew, OPT.OptionAppend
                    '月間スケジュール作成
                    SQL &= " AND NOT " & MonthColumn & " = '0'"
                    SQL &= " AND '" & String.Format("{0:yyyyMM}", FormFuriDate) & "'"
                    SQL &= " BETWEEN SUBSTR(KAISI_DATE_T, 1, 6)"
                    SQL &= " AND SUBSTR(SYURYOU_DATE_T, 1, 6)"
                    SQL &= " AND NOT TO_NUMBER(NVL(BAITAI_CODE_T, '0')) = 7"

                    Select Case SchTable
                        Case Is = APL.JifuriApplication
                        Case Is = APL.SofuriApplication
                            SQL &= " AND KIJITU_KANRI_T = '1'"
                    End Select

                    '追加の場合には取引先マスタの変更があったもの
                    If SEL_OPTION = OPT.OptionAppend Then

                        SQL &= " AND NOT KOUSIN_SIKIBETU_T = '2'"
                    End If

            End Select

            If GCom.SetDynaset(SQL, REC) Then

                Dim Counter As Integer = 0

                Select Case SEL_OPTION
                    Case OPT.OptionNothing
                        '個別登録画面
                        If REC.Read Then

                            ReDim TR(0)

                            '配列変数に取り敢えず対象取引先を蓄積
                            Call GET_SELECT_TORIMAST_Sub(REC, Counter)

                            Return True
                        End If
                    Case Else
                        '月間スケジュール作成
                        Do While REC.Read

                            Select Case Counter
                                Case Is = 0
                                    ReDim TR(0)
                                Case Else
                                    ReDim Preserve TR(Counter)
                            End Select

                            '配列変数に取り敢えず対象取引先を蓄積
                            Call GET_SELECT_TORIMAST_Sub(REC, Counter)

                            Counter += 1
                        Loop
                        Return (Counter > 0)
                End Select
            End If
        Catch ex As Exception

            Throw

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return False
    End Function

    '
    ' 機能　 ： 取引先マスタ検索SQL文の基本部をセットする。
    '
    ' 引数　 ： ARG1 - 設定月の項目名
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： 共通化
    '
    Private Function SetToriMastSelectBaseSql(ByVal MonthColumn As String) As String
        With GCom.GLog
            .Job2 = "取引先マスタ検索SQL作成"
        End With
        Try
            Dim SQL As String = "SELECT FSYORI_KBN_T"   '振替処理区分
            SQL &= ", TORIS_CODE_T"                     '取引先主コード
            SQL &= ", TORIF_CODE_T"                     '取引先副コード
            SQL &= ", " & MonthColumn & " MONTH"        '設定月
            For Index As Integer = 1 To 31 Step 1
                SQL &= ", DATE" & Index.ToString & "_T"
            Next Index
            SQL &= ", SOUSIN_KBN_T"                     '送信区分
            SQL &= ", FURI_KYU_CODE_T"                       '休日コード
            SQL &= ", FMT_KBN_T"                        'フォーマット区分
            SQL &= ", TESUUTYO_KIJITSU_T"               '手数料徴求期日区分
            SQL &= ", TESUUTYO_KBN_T"                   '手数料徴求区分
            SQL &= ", TESUUTYO_DAY_T"                    '手数料徴求日数／基準日
            SQL &= ", TESUU_KYU_CODE_T"                 '手数料徴求日休日コード
            SQL &= ", KESSAI_KIJITSU_T"                 '決済日指定区分
            SQL &= ", KESSAI_DAY_T"                      '決済日日数／基準日
            SQL &= ", KESSAI_KYU_CODE_T"                '決済休日コード
            SQL &= ", FURI_CODE_T"                      '振替コード
            SQL &= ", ITAKU_CODE_T"                     '委託者コード
            SQL &= ", TKIN_NO_T"                        '取扱金融機関
            SQL &= ", TSIT_NO_T"                        '取扱店番
            SQL &= ", BAITAI_CODE_T"                    '媒体コード
            SQL &= ", SYUBETU_T"                   '種別コード
            SQL &= ", ITAKU_KNAME_T"                    '委託者カナ名
            SQL &= ", ITAKU_NNAME_T"                    '委託者漢字名
            SQL &= ", MOTIKOMI_KIJITSU_T"               '持込期日
            SQL &= ", TESUUMAT_NO_T"                    '手数料集計周期(2008.03.06)
            SQL &= ", TESUUMAT_MONTH_T"                '集計基準月(2008.03.06)
            SQL &= ", TESUUMAT_ENDDAY_T"                   '集計終了日(2008.03.06)
            SQL &= ", TESUUMAT_KIJYUN_T"               '集計基準(2008.03.06)
            SQL &= ", TESUUTYO_PATN_T"                  '手数料徴求方法(2008.03.06)
            SQL &= ", KESSAI_KBN_T"                     '決済区分(2008.03.06)

            Select Case SchTable
                Case Is = APL.JifuriApplication
                    '自振固有の項目
                    SQL &= ", TAKO_KBN_T"                       '他行区分
                    SQL &= ", SFURI_FLG_T"                      '再振フラグ
                    SQL &= ", SFURI_KIJITSU_T"                      '再振区分
                    SQL &= ", SFURI_DAY_T"                     '再振日
                    SQL &= ", SFURI_FCODE_T"                     '再振日
                    SQL &= ", SFURI_KYU_CODE_T"                 '再振休日コード
                    SQL &= ", KIGYO_CODE_T"                     '企業コード
                    SQL &= ", MOTIKOMI_KBN_T"                   '持込区分
                    'SQL &= ", TOHO_CNT_CODE_T"                  '伝送当方センター確認コード

                    SQL &= " FROM TORIMAST"

                Case Is = APL.SofuriApplication
                    '総給振に項目がない

                    SQL &= " FROM K_TORIMAST"
            End Select

            Return SQL
        Catch ex As Exception

            Throw

        End Try
        Return ""
    End Function

    '
    ' 機能　 ： 取引先マスタ検索結果を配列へセットする。
    '
    ' 引数　 ： ARG1 - ORACLE Data Reader Object
    ' 　　　 　 ARG2 - カウンター変数
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： 共通化(配列変数に取り敢えず対象取引先を蓄積)
    '
    Public Sub GET_SELECT_TORIMAST_Sub(ByVal REC As OracleDataReader, ByRef Counter As Integer)
        With GCom.GLog
            .Job2 = "取引先マスタ検索結果格納"
        End With
        Try
            Dim Temp As String = ""

            With TR(Counter)
                .FSYORI_KBN = GCom.NzDec(REC.Item("FSYORI_KBN_T"), "")              '振替処理区分
                .TORIS_CODE = GCom.NzDec(REC.Item("TORIS_CODE_T"), "")              '取引先主コード
                .TORIF_CODE = GCom.NzDec(REC.Item("TORIF_CODE_T"), "")              '取引先副コード
                .ITAKU_CODE = GCom.NzDec(REC.Item("ITAKU_CODE_T"), "")              '委託者コード
                .ITAKU_KNAME = GCom.NzStr(REC.Item("ITAKU_KNAME_T"))                '委託者カナ名
                .ITAKU_NNAME = GCom.NzStr(REC.Item("ITAKU_NNAME_T"))                '委託者漢字名
                .MONTH_FLG = GCom.NzInt(REC.Item("MONTH"), 0)                       '該当指定月の値

                '振替該当日の有効／無効
                ReDim TR(Counter).DATEN(31)
                For Index As Integer = 1 To 31 Step 1
                    Temp = "DATE" & Index.ToString & "_T"
                    TR(Counter).DATEN(Index) = GCom.NzInt(REC.Item(Temp), 0)
                Next Index

                .SOUSIN_KBN = GCom.NzDec(REC.Item("SOUSIN_KBN_T"), "")              '送信区分
                .FURI_KYU_CODE = GCom.NzInt(REC.Item("FURI_KYU_CODE_T"), 0)         '休日コード
                .FMT_KBN = GCom.NzInt(REC.Item("FMT_KBN_T"), 0)                     'フォーマット区分
                .FURI_CODE = GCom.NzDec(REC.Item("FURI_CODE_T"), "")                '振替コード
                .TKIN_NO = GCom.NzDec(REC.Item("TKIN_NO_T"), "")                    '取扱金融機関
                .TSIT_NO = GCom.NzDec(REC.Item("TSIT_NO_T"), "")                    '取扱店番
                .BAITAI_CODE = GCom.NzDec(REC.Item("BAITAI_CODE_T"), "")            '媒体コード
                .SYUBETU = GCom.NzDec(REC.Item("SYUBETU_T"), "")                    '種別コード
                .TESUUTYO_KIJITSU = GCom.NzInt(REC.Item("TESUUTYO_KIJITSU_T"), 0)   '手数料徴求期日区分
                .TESUUTYO_KBN = GCom.NzInt(REC.Item("TESUUTYO_KBN_T"), 0)           '手数料徴求区分
                .TESUUTYO_DAY = GCom.NzInt(REC.Item("TESUUTYO_DAY_T"), 0)           '手数料徴求日数／基準日
                .TESUU_KYU_CODE = GCom.NzInt(REC.Item("TESUU_KYU_CODE_T"), 0)       '手数料徴求日休日コード
                .KESSAI_KIJITSU = GCom.NzInt(REC.Item("KESSAI_KIJITSU_T"), 0)       '決済日指定区分
                .KESSAI_DAY = GCom.NzInt(REC.Item("KESSAI_DAY_T"), 0)               '決済日日数／基準日
                .KESSAI_KYU_CODE = GCom.NzInt(REC.Item("KESSAI_KYU_CODE_T"), 0)     '決済休日コード
                .MOTIKOMI_KIJITSU = GCom.NzInt(REC.Item("MOTIKOMI_KIJITSU_T"), 0)   '持込期日
                .TESUUMAT_NO = GCom.NzInt(REC.Item("TESUUMAT_NO_T"), 0)             '手数料集計周期(2008.03.06)
                If .TESUUMAT_NO > 12 Then
                    .TESUUMAT_NO = 12
                End If
                .TESUUMAT_MONTH = GCom.NzInt(REC.Item("TESUUMAT_MONTH_T"), 0)     '集計基準月(2008.03.06)
                .TESUUMAT_ENDDAY = GCom.NzInt(REC.Item("TESUUMAT_ENDDAY_T"), 0)           '集計終了日(2008.03.06)
                .TESUUMAT_KIJYUN = GCom.NzInt(REC.Item("TESUUMAT_KIJYUN_T"), 0)   '集計基準(2008.03.06)
                .TESUUTYO_PATN = GCom.NzInt(REC.Item("TESUUTYO_PATN_T"), 0)         '手数料徴求方法(2008.03.06)
                .KESSAI_KBN = GCom.NzInt(REC.Item("KESSAI_KBN_T"), 0)               '決済区分(2008.03.06)

                Select Case SchTable
                    Case Is = APL.JifuriApplication
                        .TAKO_KBN = GCom.NzInt(REC.Item("TAKO_KBN_T"), 0)               '他行区分
                        .KIGYO_CODE = GCom.NzDec(REC.Item("KIGYO_CODE_T"), "")          '企業コード
                        .MOTIKOMI_KBN = GCom.NzDec(REC.Item("MOTIKOMI_KBN_T"), "")      '持込区分
                        .SFURI_FLG = GCom.NzInt(REC.Item("SFURI_FLG_T"), 0)             '再振フラグ
                        .SFURI_FCODE = GCom.NzStr(REC.Item("SFURI_FCODE_T"))            '
                        .SFURI_DAY = GCom.NzInt(REC.Item("SFURI_DAY_T"), 0)           '再振日
                        .SFURI_KYU_CODE = GCom.NzInt(REC.Item("SFURI_KYU_CODE_T"), 0)   '再振休日コード
                    Case Is = APL.SofuriApplication
                        ' NOP
                End Select
            End With
        Catch ex As Exception
            Throw
        End Try
    End Sub

    '
    ' 機能　 ： スケジュールマスタへの登録
    '
    ' 引数　 ： ARG1 - 0 = 個別スケジュール登録, Else = 月間スケジュール作成
    '           ARG2 - ログ出力識別
    '           ARG3 - 諸計算だけの機能を使用する場合=True
    '
    ' 戻り値 ： 正常終了 = True, 異常終了 = False
    '
    ' 備考　 ： 再振スケジュールの月末補正
    '
    Public Function INSERT_NEW_SCHMAST(ByVal Index As Integer, _
            Optional ByVal SEL As Boolean = True, Optional ByVal CALC_ONLY As Boolean = False) As Boolean

        Dim Ret As Integer
        Dim Cnt As Integer
        Dim SQL As String = ""
        Dim BRet As Boolean             '日付評価関数の戻り値受け皿
        Dim Temp As String              '汎用文字列変数
        Dim onDate As Date              '汎用日付変数
        Dim onText(2) As Integer        '汎用日付評価時配列
        Dim ColumnID1 As Integer
        Dim ColumnID2 As Integer
        Try
            '------------------------------------------------------------------
            '持込締切日(期日)を求める
            '------------------------------------------------------------------
            BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.MOTIKOMI_DATE, TR(Index).MOTIKOMI_KIJITSU, 1)

            '------------------------------------------------------------------
            '振分／不能更新／返却予定日
            '------------------------------------------------------------------
            '自行分配信予定日
            BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HAISIN_YDATE, GCom.DayINI.HAISIN, 1)
            '自行分不能結果予定日
            BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.FUNOU_YDATE, GCom.DayINI.FUNOU, 0)
            '------------------------------------------------------------------
            '配信予定日を求める
            '------------------------------------------------------------------
            '他行区分    : 非対象    :          対象
            '------------------------------------------------------------------
            'ＦＭＴ区分  : 全部      : SSS(内)    : SSS(外)    :   それ以外
            '------------------------------------------------------------------
            '自          :       パターン１
            '------------------------------------------------------------------
            '他行１      :    ×     :       パターン４        : パターン２
            '------------------------------------------------------------------
            '他行２      :    ×     :     ×     : パターン５ : パターン３
            '------------------------------------------------------------------
            '              振り分け予定       不能予定
            'パターン  1        - 3              + 1
            '          2        - 5              + 2
            '          3        - 6              + 3        全てINIファイル指定
            '          4        - 5              + 3
            '          5        - 7              + 5
            '------------------------------------------------------------------
            '[JIFURI]  fskj.ini
            'FUNOU = 1
            'FUNOU_TAKOU_1 = 1
            'FUNOU_TAKOU_2 = 1
            'FUNOU_SSS_1 = 3
            'FUNOU_SSS_2 = 3
            'HAISIN = 3
            'HAISIN_TAKOU_1 = 3
            'HAISIN_TAKOU_2 = 3
            'HAISIN_SSS_1 = 6
            'HAISIN_SSS_2 = 7
            'KAISYU = 10
            'HITS = 4
            'HITQ = 3
            '------------------------------------------------------------------

            '他行仕向け有無(他行区分), フォーマット区分 で決定
            Select Case TR(Index).FMT_KBN
                Case 20, 21
                    'SSS は他行区分に関係なく作成する。
                    '20, ＳＳＳ(提携内)
                    '21, ＳＳＳ(提携外)
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                            SCH.HAISIN_T1YDATE, GCom.DayINI.HAISIN_SSS_1, 1)
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                            SCH.HAISIN_T2YDATE, GCom.DayINI.HAISIN_SSS_2, 1)

                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                            SCH.FUNOU_T1YDATE, GCom.DayINI.FUNOU_SSS_1, 0)
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                            SCH.FUNOU_T2YDATE, GCom.DayINI.FUNOU_SSS_2, 0)

                    Select Case TR(Index).FMT_KBN
                        Case 20
                            SCH.HENKAN_YDATE = SCH.FUNOU_T1YDATE    '返還予定日
                            SCH.HAISIN_T2YDATE = New String("0"c, 8)
                            SCH.FUNOU_T2YDATE = New String("0"c, 8)
                        Case 21
                            SCH.HENKAN_YDATE = SCH.FUNOU_T2YDATE    '返還予定日
                    End Select

                Case Else  '0, 1, 2, 3, 4, 5, 6
                    'SSS 以外のフォーマットは他行区分で判定する
                    '00, 全銀
                    '01, 地公体(350)
                    '02, 国税
                    '03, 年金
                    '04, 依頼書
                    '05, 伝票
                    '06, 地公体(300)
                    If TR(Index).TAKO_KBN = 0 Then
                        '0, 他行データ作成非対象
                        SCH.HAISIN_T1YDATE = New String("0"c, 8)
                        SCH.HAISIN_T2YDATE = New String("0"c, 8)
                        SCH.FUNOU_T1YDATE = New String("0"c, 8)
                        SCH.FUNOU_T2YDATE = New String("0"c, 8)

                        SCH.HENKAN_YDATE = SCH.FUNOU_YDATE          '返還予定日
                    Else
                        '1, 他行データ作成対象
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                                SCH.HAISIN_T1YDATE, GCom.DayINI.HAISIN_TAKOU_1, 1)
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                                SCH.HAISIN_T2YDATE, GCom.DayINI.HAISIN_TAKOU_2, 1)

                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                                SCH.FUNOU_T1YDATE, GCom.DayINI.FUNOU_TAKOU_1, 0)
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, _
                                SCH.FUNOU_T2YDATE, GCom.DayINI.FUNOU_TAKOU_2, 0)

                        '他行マスタに伝送以外があるか否か
                        Select Case CheckTakouMast(Index)
                            Case 0
                                '伝送のみ
                                SCH.HENKAN_YDATE = SCH.FUNOU_T1YDATE    '返還予定日
                                SCH.HAISIN_T2YDATE = New String("0"c, 8)
                                SCH.FUNOU_T2YDATE = New String("0"c, 8)
                            Case Else
                                '伝送以外混在
                                SCH.HENKAN_YDATE = SCH.FUNOU_T2YDATE    '返還予定日
                        End Select
                    End If
            End Select

            '総給振の場合の処理
            If SchTable = APL.SofuriApplication Then

                Dim KYUFURI As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "KYUFURI"), 0)
                Dim SOUFURI As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "SOUFURI"), 0)
                Dim HASSHIN As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "HASSIN"), 0)

                '資金確保予定日(後の決済予定日算出処理で書き換える)2008.04.04 By Astar FJH西田氏指示
                Select Case TR(Index).SYUBETU
                    Case Is = "11"
                        '給与振込
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE_S, KYUFURI, 1)
                    Case Is = "12"
                        '賞与振込
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE_S, KYUFURI, 1)
                    Case Is = "21"
                        '総合振込
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE_S, SOUFURI, 1)
                End Select

                '発信予定日
                Select Case TR(Index).SYUBETU
                    Case Is = "11"
                        '給与振込
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE_S, HASSHIN, 1)
                    Case Is = "12"
                        '賞与振込
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE_S, HASSHIN, 1)
                    Case Is = "21"
                        '総合振込
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE_S, HASSHIN, 1)
                End Select

                '返還予定日
                BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HENKAN_YDATE, 1, 0)
                SCH.HENKAN_YDATE = SCH.FURI_DATE
            End If

            '------------------------------------------------------------------
            '依頼書回収予定日を求める
            '------------------------------------------------------------------
            BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.IRAISYOK_YDATE, GCom.DayINI.KAISYU, 1)

            '------------------------------------------------------------------
            '決済予定日を求める
            '------------------------------------------------------------------
            'KESSAI_KIJITSU     '決済期日区分(0:営業日数指定, 1:基準日指定, 2:翌月基準日指定)
            'KESSAI_DAY         '決済日数／基準日(1, 2, 3, 4, 6, 12 ※一括徴求の場合のみ有効)
            'KESSAI_KYU_CODE    '決済休日コード(0:翌営業日振替, 1:前営業日振替)

            '2008.02.27 Insert By Astar
            If TR(Index).KESSAI_DAY = 0 Then
                TR(Index).KESSAI_DAY = 1
            End If

            Select Case TR(Index).KESSAI_KIJITSU
                Case 0
                    '営業日数指定
                    Dim FrontBackType As Integer = 0
                    Select Case SchTable
                        Case APL.JifuriApplication
                            FrontBackType = 0
                        Case APL.SofuriApplication
                            FrontBackType = 1
                    End Select
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KESSAI_YDATE, TR(Index).KESSAI_DAY, FrontBackType)
                Case 1
                    '基準日指定(月末補正あり)
                    '2008.04.21 Update By Astar
                    'onDate = GCom.SET_DATE(SCH.FURI_DATE)
                    onDate = GCom.SET_DATE(SCH.KFURI_DATE)
                    onText(0) = onDate.Year
                    onText(1) = onDate.Month
                    onText(2) = TR(Index).KESSAI_DAY

                    '基準日で歴日判定
                    Ret = GCom.SET_DATE(onDate, onText)
                    If Not Ret = -1 Then
                        '歴日でない場合には月末を算出
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = 1
                        Ret = GCom.SET_DATE(onDate, onText)
                        onDate = onDate.AddDays(-1)
                    End If
                    Temp = String.Format("{0:yyyyMMdd}", onDate)
                    BRet = GCom.CheckDateModule(Temp, SCH.KESSAI_YDATE, TR(Index).KESSAI_KYU_CODE)

                    '振替日より手前の場合には翌月へ再計算
                    If SCH.FURI_DATE > SCH.KESSAI_YDATE Then
                        '2010.03.01 契約振替日に変更 start
                        'onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1) 
                        onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                        '2010.03.01 契約振替日に変更 end
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = TR(Index).KESSAI_DAY
                        Ret = GCom.SET_DATE(onDate, onText)
                        If Not Ret = -1 Then
                            onText(0) = onDate.Year
                            onText(1) = onDate.Month
                            onText(2) = 1
                            Ret = GCom.SET_DATE(onDate, onText)
                            onDate = onDate.AddDays(-1)
                        End If
                        Temp = String.Format("{0:yyyyMMdd}", onDate)
                        BRet = GCom.CheckDateModule(Temp, SCH.KESSAI_YDATE, TR(Index).KESSAI_KYU_CODE)
                    End If
                Case 2
                    '翌月基準日指定
                    '2008.04.21 Update By Astar
                    'onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                    onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                    onText(0) = onDate.Year
                    onText(1) = onDate.Month
                    onText(2) = TR(Index).KESSAI_DAY
                    Ret = GCom.SET_DATE(onDate, onText)
                    If Not Ret = -1 Then
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = 1
                        Ret = GCom.SET_DATE(onDate, onText)
                        onDate = onDate.AddDays(-1)
                    End If
                    Temp = String.Format("{0:yyyyMMdd}", onDate)
                    BRet = GCom.CheckDateModule(Temp, SCH.KESSAI_YDATE, TR(Index).KESSAI_KYU_CODE)
                Case 3
                    '振込日指定
                    SCH.KESSAI_YDATE = SCH.FURI_DATE
            End Select

            If SchTable = APL.SofuriApplication Then

                SCH.KAKUHO_YDATE_S = SCH.KESSAI_YDATE
                SCH.KESSAI_YDATE = SCH.FURI_DATE
                If SCH.HASSIN_YDATE_S < SCH.KAKUHO_YDATE_S Then

                    SCH.HASSIN_YDATE_S = SCH.KAKUHO_YDATE_S
                End If
            End If

            '------------------------------------------------------------------
            '手数料徴収予定日を求める
            '------------------------------------------------------------------
            'TESUUTYO_KIJITSU   '手数料徴求期日区分(0:営業日数指定, 1:基準日指定, 2:翌月基準日指定)
            'TESUUTYO_KBN       '手数料徴求区分(0:都度徴求, 1:一括徴求, 2:特別免除, 3:別途徴求(一括) 4:別途徴求(都度))
            'TESUUTYO_NO        '手数料徴求日数／基準日
            'TESUU_KYU_CODE     '手数料徴求日休日コード(0:翌営業日振替, 1:前営業日振替)
            '以下、一括徴求の場合のみ有効
            'TESUUMAT_DAY        '手数料集計周期 (2008.03.06) Ｎヶ月(1,2,3,4,6,12)=12の約数
            'TESUUMAT_MONTH    '集計基準月     (2008.03.06)
            'TESUUMAT_ENDDAY       '集計終了日     (2008.03.06)
            'TESUUMAT_KIJYUN2   '集計基準       (2008.03.06) (0:振替日, 1:決済日)
            'TESUUTYO_PATN      '手数料徴求方法 (2008.03.06) (0:差引入金, 1:直接入金)
            'KESSAI_KBN         '決済区分       (2008.03.06) 
            '決済区分 = (0:預け金, 1:口座入金, 2:為替振込, 3:為替付替, 4:特別企業, 5:決済対象外)
            '集計開始基準月は(TESUUMAT_KIJYUN)で観る。
            '集計に入れるか否かは(TESUUMAT_KIJYUN2)で判断。振替日は契約日で、決済日は計算予定日で行う。
            '集計に入れるか否かの基準は(TESUUMAT_END)で行う。

            If TR(Index).TESUUTYO_DAY = 0 Then
                TR(Index).TESUUTYO_DAY = 1
            End If

            Select Case TR(Index).TESUUTYO_KBN

                Case 1

                    '一括徴求(差引入金は絶対にない。→仕様)

                    '手数料徴求月を配列化する
                    Dim ALL_Month(GCom.NzInt(12 \ TR(Index).TESUUMAT_NO)) As Integer

                    '2008.04.12 Update By Astar
                    'あくまでも年間を通したサイクルで考える
                    Select Case ALL_Month.GetUpperBound(0)
                        Case 2, 3, 4, 6
                            onText(1) = TR(Index).TESUUMAT_MONTH - 1
                            Do While onText(1) - TR(Index).TESUUMAT_NO > 0
                                onText(1) -= TR(Index).TESUUMAT_NO
                            Loop
                            If onText(1) <= 0 Then
                                onText(1) = TR(Index).TESUUMAT_NO
                            End If
                            ALL_Month(1) = onText(1)

                            For Cnt = 2 To ALL_Month.GetUpperBound(0) Step 1
                                onText(1) = ALL_Month(Cnt - 1) + TR(Index).TESUUMAT_NO
                                ALL_Month(Cnt) = onText(1)
                            Next Cnt
                        Case 12
                            For Cnt = 1 To ALL_Month.GetUpperBound(0) Step 1
                                ALL_Month(Cnt) = Cnt
                            Next Cnt
                        Case Else
                            If TR(Index).TESUUMAT_MONTH = 1 Then
                                ALL_Month(1) = 12
                            Else
                                ALL_Month(1) = TR(Index).TESUUMAT_MONTH - 1
                            End If
                    End Select

                    Dim CompDay As Integer
                    Select Case TR(Index).TESUUMAT_KIJYUN
                        Case 0
                            '振替日の場合(振替日で判断する。)
                            onText(0) = GCom.NzInt(SCH.FURI_DATE.Substring(0, 4), 0)
                            onText(1) = GCom.NzInt(SCH.FURI_DATE.Substring(4, 2), 0)
                            onText(2) = GCom.NzInt(SCH.FURI_DATE.Substring(6, 2), 0)
                            CompDay = onText(2)
                        Case Else
                            '決済日の場合(決済予定日で判断する。)
                            onText(0) = GCom.NzInt(SCH.KESSAI_YDATE.Substring(0, 4), 0)
                            onText(1) = GCom.NzInt(SCH.KESSAI_YDATE.Substring(4, 2), 0)
                            onText(2) = GCom.NzInt(SCH.KESSAI_YDATE.Substring(6, 2), 0)
                            CompDay = onText(2)
                    End Select
                    Dim memMonth As Integer = onText(1)

                    '次回手数料徴求月を求める
                    For Cnt = 1 To ALL_Month.GetUpperBound(0) Step 1

                        If onText(1) <= ALL_Month(Cnt) Then

                            onText(1) = ALL_Month(Cnt)
                            Exit For
                        End If
                    Next

                    '手数料徴収月配列内の月値が振替月値を追い越せなかった場合(2008.04.16 By Astar)
                    If Cnt > ALL_Month.GetUpperBound(0) Then
                        '次年度の最初の徴収月を設定する
                        onText(0) += 1
                        onText(1) = ALL_Month(1)
                    Else
                        If (TR(Index).TESUUMAT_KIJYUN = 0 AndAlso GCom.NzInt(SCH.KFURI_DATE.Substring(4, 2), 0) = memMonth) OrElse _
                           (TR(Index).TESUUMAT_KIJYUN <> 0 AndAlso GCom.NzInt(SCH.KESSAI_YDATE.Substring(4, 2), 0) = memMonth) Then
                            '終了日の関係で当月内に設定できない場合には１サイクルずらす
                            If onText(1) <= memMonth AndAlso TR(Index).TESUUMAT_ENDDAY < CompDay Then
                                onText(1) += TR(Index).TESUUMAT_NO
                                If onText(1) > 12 Then
                                    onText(0) += 1
                                    onText(1) = onText(1) - 12
                                End If
                            End If
                        End If
                    End If

                    Ret = SET_DATE(onDate, onText)
                    If Not Ret = -1 Then
                        '歴日でない場合には月末を算出
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = 1
                        Ret = GCom.SET_DATE(onDate, onText)
                        onDate = onDate.AddDays(-1)
                    End If
                    Temp = String.Format("{0:yyyyMMdd}", onDate)

                    '手数料徴求月の振替日を算出する。
                    Dim Temp_FURI_DATE As String = ""

                    Select Case TR(Index).TESUUTYO_KBN
                        Case 1, 3
                            Temp = Temp.Substring(0, 6) & "15"
                    End Select

                    Select Case TR(Index).FURI_KYU_CODE
                        Case 0, 1
                            '振替日,営業日判定(土・日・祝祭日判定)
                            BRet = GCom.CheckDateModule(Temp, Temp_FURI_DATE, TR(Index).FURI_KYU_CODE)
                        Case Else
                            '振替日,営業日判定(土・日・祝祭日判定)
                            BRet = GCom.CheckDateModule(Temp, Temp_FURI_DATE, 0)
                            If GCom.NzInt(Temp.Substring(0, 6), 0) < _
                                GCom.NzInt(Temp_FURI_DATE.Substring(0, 6), 0) Then

                                BRet = GCom.CheckDateModule(Temp, Temp_FURI_DATE, 1, 1)
                            End If
                    End Select

                    '手数料徴求関連指定項目に従って算出する
                    Select Case TR(Index).TESUUTYO_KIJITSU
                        Case 0
                            '営業日数指定
                            Select Case TR(Index).TESUUTYO_KBN
                                Case 1, 3
                                    '集計終了日(月末補正あり)
                                    onText(0) = Integer.Parse(Temp_FURI_DATE.Substring(0, 4))
                                    onText(1) = Integer.Parse(Temp_FURI_DATE.Substring(4, 2))
                                    onText(2) = TR(Index).TESUUMAT_ENDDAY
                                    '歴日判定
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    If Not Ret = -1 Then
                                        '歴日でない場合には月末を算出
                                        onText(0) = onDate.Year
                                        onText(1) = onDate.Month
                                        onText(2) = 1
                                        Ret = GCom.SET_DATE(onDate, onText)
                                        onDate = onDate.AddDays(-1)
                                    End If

                                    '集計終了日を手数料徴求日休日コードで営業日補正する
                                    BRet = GCom.CheckDateModule(onDate.ToString("yyyyMMdd"), Temp_FURI_DATE, TR(Index).TESUU_KYU_CODE)
                                    '集計終了日から営業日日数指定する
                                    BRet = GCom.CheckDateModule(Temp_FURI_DATE, SCH.TESUU_YDATE, TR(Index).TESUUTYO_DAY, 0)
                                Case Else
                                    BRet = GCom.CheckDateModule(Temp_FURI_DATE, SCH.TESUU_YDATE, TR(Index).TESUUTYO_DAY, 0)
                            End Select
                            '*****************************************************************************
                        Case 1
                            '基準日指定(月末補正あり)
                            onDate = GCom.SET_DATE(Temp_FURI_DATE)
                            onText(0) = onDate.Year
                            onText(1) = onDate.Month
                            onText(2) = TR(Index).TESUUTYO_DAY

                            '基準日で歴日判定
                            Ret = GCom.SET_DATE(onDate, onText)
                            If Not Ret = -1 Then
                                '歴日でない場合には月末を算出
                                onText(0) = onDate.Year
                                onText(1) = onDate.Month
                                onText(2) = 1
                                Ret = GCom.SET_DATE(onDate, onText)
                                onDate = onDate.AddDays(-1)
                            End If
                            Temp = String.Format("{0:yyyyMMdd}", onDate)
                            BRet = GCom.CheckDateModule(Temp, SCH.TESUU_YDATE, TR(Index).TESUU_KYU_CODE)
                        Case 2
                            '翌月基準日指定
                            onDate = GCom.SET_DATE(Temp_FURI_DATE).AddMonths(1)
                            onText(0) = onDate.Year
                            onText(1) = onDate.Month
                            onText(2) = TR(Index).TESUUTYO_DAY
                            Ret = GCom.SET_DATE(onDate, onText)
                            If Not Ret = -1 Then
                                onText(0) = onDate.Year
                                onText(1) = onDate.Month
                                onText(2) = 1
                                Ret = GCom.SET_DATE(onDate, onText)
                                onDate = onDate.AddDays(-1)
                            End If
                            Temp = String.Format("{0:yyyyMMdd}", onDate)
                            BRet = GCom.CheckDateModule(Temp, SCH.TESUU_YDATE, TR(Index).TESUU_KYU_CODE)
                        Case 3
                            '振込日指定
                            SCH.TESUU_YDATE = SCH.FURI_DATE
                        Case 4
                            '確保日指定
                            SCH.TESUU_YDATE = SCH.KESSAI_YDATE
                    End Select
                Case 2  '特別免除

                    SCH.TESUU_YDATE = "00000000"

                Case 3  '別途徴求

                    SCH.TESUU_YDATE = SCH.KESSAI_YDATE

                Case Else
                    '都度請求
                    Select Case TR(Index).TESUUTYO_PATN
                        Case 0
                            '差引入金
                            SCH.TESUU_YDATE = SCH.KESSAI_YDATE
                        Case Else

                            '直接入金
                            Select Case TR(Index).TESUUTYO_KIJITSU
                                Case 0
                                    '営業日数指定
                                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.TESUU_YDATE, TR(Index).TESUUTYO_DAY, 0)
                                Case 1
                                    '基準日指定(月末補正あり)
                                    onDate = GCom.SET_DATE(SCH.FURI_DATE)
                                    onText(0) = onDate.Year
                                    onText(1) = onDate.Month
                                    onText(2) = TR(Index).TESUUTYO_DAY

                                    '基準日で歴日判定
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    If Not Ret = -1 Then
                                        '歴日でない場合には月末を算出
                                        onText(0) = onDate.Year
                                        onText(1) = onDate.Month
                                        onText(2) = 1
                                        Ret = GCom.SET_DATE(onDate, onText)
                                        onDate = onDate.AddDays(-1)
                                    End If
                                    Temp = String.Format("{0:yyyyMMdd}", onDate)
                                    BRet = GCom.CheckDateModule(Temp, SCH.TESUU_YDATE, TR(Index).TESUU_KYU_CODE)

                                    '振替日より手前の場合には翌月へ再計算
                                    If SCH.FURI_DATE > SCH.TESUU_YDATE Then
                                        onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                                        onText(0) = onDate.Year
                                        onText(1) = onDate.Month
                                        onText(2) = TR(Index).TESUUTYO_DAY
                                        Ret = GCom.SET_DATE(onDate, onText)
                                        If Not Ret = -1 Then
                                            onText(0) = onDate.Year
                                            onText(1) = onDate.Month
                                            onText(2) = 1
                                            Ret = GCom.SET_DATE(onDate, onText)
                                            onDate = onDate.AddDays(-1)
                                        End If
                                        Temp = String.Format("{0:yyyyMMdd}", onDate)
                                        BRet = GCom.CheckDateModule(Temp, SCH.TESUU_YDATE, TR(Index).TESUU_KYU_CODE)
                                    End If
                                Case 2
                                    '翌月基準日指定
                                    onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                                    onText(0) = onDate.Year
                                    onText(1) = onDate.Month
                                    onText(2) = TR(Index).TESUUTYO_DAY
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    If Not Ret = -1 Then
                                        onText(0) = onDate.Year
                                        onText(1) = onDate.Month
                                        onText(2) = 1
                                        Ret = GCom.SET_DATE(onDate, onText)
                                        onDate = onDate.AddDays(-1)
                                    End If
                                    Temp = String.Format("{0:yyyyMMdd}", onDate)
                                    BRet = GCom.CheckDateModule(Temp, SCH.TESUU_YDATE, TR(Index).TESUU_KYU_CODE)
                                Case 3
                                    '振込日指定
                                    SCH.TESUU_YDATE = SCH.FURI_DATE
                                Case 4
                                    '確保日指定
                                    SCH.TESUU_YDATE = SCH.KESSAI_YDATE

                            End Select

                            If SCH.TESUU_YDATE < SCH.KESSAI_YDATE Then
                                SCH.TESUU_YDATE = SCH.KESSAI_YDATE
                            End If

                            If SchTable = APL.SofuriApplication Then
                                If TR(Index).TESUUTYO_KIJITSU = 4 Then
                                    SCH.TESUU_YDATE = SCH.KAKUHO_YDATE_S
                                End If
                            End If
                    End Select
            End Select

            '------------------------------------------------------------------
            '再振予定日を求める
            '------------------------------------------------------------------
            'SFURI_FLG      '再振フラグ(0:再振契約なし, 1:再振契約あり)
            'SFURI_KBN      '再振区分(0:営業日数指定, 1:基準日指定)
            'SFURI_DAY      '再振契約日(再振ありの場合、再振替日を指定　振替日からの営業日数　or 基準日指定)
            'SFURI_KYU_CODE '再振休日コード(0:翌営業日振替, 1:前営業日振替)
            If TR(Index).SFURI_FLG = 1 Then
                '再振フラグ(再振有) 
                If TR(Index).SFURI_DAY = 0 Then
                    TR(Index).SFURI_DAY = 1
                End If

                Select Case TR(Index).SFURI_KIJITSU
                    Case 0
                        '営業日数指定
                        BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KSAIFURI_DATE, TR(Index).SFURI_DAY, 0)
                    Case 1
                        '基準日指定(月末補正あり)
                        onDate = GCom.SET_DATE(SCH.KFURI_DATE)
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = TR(Index).SFURI_DAY

                        '基準日で歴日判定
                        Ret = GCom.SET_DATE(onDate, onText)
                        If Not Ret = -1 Then
                            '歴日でない場合には月末を算出
                            onText(0) = onDate.Year
                            onText(1) = onDate.Month
                            onText(2) = 1
                            Ret = GCom.SET_DATE(onDate, onText)
                            onDate = onDate.AddDays(-1)
                        End If
                        Temp = String.Format("{0:yyyyMMdd}", onDate)
                        Select Case TR(Index).SFURI_KYU_CODE
                            Case 0, 1
                                '振替日,営業日判定(土・日・祝祭日判定)
                                BRet = GCom.CheckDateModule(Temp, _
                                                SCH.KSAIFURI_DATE, TR(Index).SFURI_KYU_CODE)

                                '振替日より手前の場合には翌月へ再計算
                                If SCH.FURI_DATE >= SCH.KSAIFURI_DATE Then
                                    onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                                    onText(0) = onDate.Year
                                    onText(1) = onDate.Month
                                    onText(2) = TR(Index).SFURI_DAY
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    If Not Ret = -1 Then
                                        onText(0) = onDate.Year
                                        onText(1) = onDate.Month
                                        onText(2) = 1
                                        Ret = GCom.SET_DATE(onDate, onText)
                                        onDate = onDate.AddDays(-1)
                                    End If
                                    Temp = String.Format("{0:yyyyMMdd}", onDate)

                                    '振替日,営業日判定(土・日・祝祭日判定)
                                    BRet = GCom.CheckDateModule(Temp, _
                                                    SCH.KSAIFURI_DATE, TR(Index).SFURI_KYU_CODE)
                                End If
                            Case Else
                                '振替日,営業日判定(土・日・祝祭日判定)
                                BRet = GCom.CheckDateModule(Temp, SCH.KSAIFURI_DATE, 0)

                                '再振休日コードが「2」翌営業日振替(月跨ぎ時前営業日)では同月を原則とする。
                                If GCom.NzInt(SCH.FURI_DATE.Substring(0, 6), 0) < _
                                   GCom.NzInt(SCH.KSAIFURI_DATE.Substring(0, 6), 0) Then

                                    Temp = SCH.KSAIFURI_DATE

                                    '月が異なる場合に前月最終営業日へ補正する。
                                    onText(0) = GCom.NzInt(SCH.KSAIFURI_DATE.Substring(0, 4), 0)
                                    onText(1) = GCom.NzInt(SCH.KSAIFURI_DATE.Substring(4, 2), 0)
                                    onText(2) = 1
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    BRet = GCom.CheckDateModule(onDate.AddDays(-1).ToString("yyyyMMdd"), _
                                                                    SCH.KSAIFURI_DATE, 1)

                                    If SCH.FURI_DATE >= SCH.KSAIFURI_DATE Then
                                        '振替日より手前の場合には再計算を破棄する

                                        SCH.KSAIFURI_DATE = Temp
                                    End If
                                End If
                        End Select
                    Case 2
                        '翌月基準日指定
                        onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                        onText(0) = onDate.Year
                        onText(1) = onDate.Month
                        onText(2) = TR(Index).SFURI_DAY
                        Ret = GCom.SET_DATE(onDate, onText)
                        If Not Ret = -1 Then
                            onText(0) = onDate.Year
                            onText(1) = onDate.Month
                            onText(2) = 1
                            Ret = GCom.SET_DATE(onDate, onText)
                            onDate = onDate.AddDays(-1)
                        End If
                        Temp = String.Format("{0:yyyyMMdd}", onDate)
                        Select Case TR(Index).SFURI_KYU_CODE
                            Case 0, 1
                                '振替日,営業日判定(土・日・祝祭日判定)
                                BRet = GCom.CheckDateModule(Temp, _
                                                SCH.KSAIFURI_DATE, TR(Index).SFURI_KYU_CODE)
                            Case Else
                                '振替日,営業日判定(土・日・祝祭日判定)
                                BRet = GCom.CheckDateModule(Temp, SCH.KSAIFURI_DATE, 0)

                                '再振休日コードが「2」翌営業日振替(月跨ぎ時前営業日)では同月を原則とする。
                                onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                                If GCom.NzInt(onDate.ToString("yyyyMM"), 0) < _
                                   GCom.NzInt(SCH.KSAIFURI_DATE.Substring(0, 6), 0) Then

                                    Temp = SCH.KSAIFURI_DATE

                                    '月が異なる場合に前月最終営業日へ補正する。
                                    onText(0) = GCom.NzInt(SCH.KSAIFURI_DATE.Substring(0, 4), 0)
                                    onText(1) = GCom.NzInt(SCH.KSAIFURI_DATE.Substring(4, 2), 0)
                                    onText(2) = 1
                                    Ret = GCom.SET_DATE(onDate, onText)
                                    Temp = onDate.ToString("yyyyMMdd")
                                    BRet = GCom.CheckDateModule(Temp, SCH.KSAIFURI_DATE, 1, 1)

                                    If SCH.FURI_DATE >= SCH.KSAIFURI_DATE Then
                                        '振替日より手前の場合には再計算を破棄する

                                        SCH.KSAIFURI_DATE = Temp
                                    End If
                                End If

                        End Select
                End Select
            Else
                '再振なし
                SCH.KSAIFURI_DATE = New String("0"c, 8)
            End If

            '計算ロジックのみで関数を抜ける場合
            If CALC_ONLY Then
                Return True
            End If

            '------------------------------------------------------------------
            'マスタ登録項目設定(SQL文)
            '------------------------------------------------------------------
            Select Case SchTable
                Case Is = APL.JifuriApplication

                    SQL = "INSERT INTO SCHMAST"

                Case Is = APL.SofuriApplication

                    SQL = "INSERT INTO S_SCHMAST"
            End Select

            For ColumnID1 = 0 To MaxColumn Step 1
                Select Case ColumnID1
                    Case 0
                        SQL &= " (" & ORANAME(ColumnID1)
                    Case Else
                        SQL &= ", " & ORANAME(ColumnID1)
                End Select
            Next ColumnID1
            SQL &= ") VALUES"

            For ColumnID2 = 0 To MaxColumn Step 1

                If ColumnID2 = 0 Then
                    SQL &= " ("
                Else
                    SQL &= ", "
                End If

                Select Case ORANAME(ColumnID2)
                    Case "FSYORI_KBN_S"
                        '振替処理区分1
                        SQL &= SetItem(ColumnID2, TR(Index).FSYORI_KBN)
                    Case "TORIS_CODE_S"
                        '取引先主コード2
                        SQL &= SetItem(ColumnID2, TR(Index).TORIS_CODE)
                    Case "TORIF_CODE_S"
                        '取引先副コード3
                        SQL &= SetItem(ColumnID2, TR(Index).TORIF_CODE)
                    Case "FURI_DATE_S"
                        '振替日4
                        SQL &= SetItem(ColumnID2, SCH.FURI_DATE)
                    Case "KFURI_DATE_S"
                        '契約振替日5
                        SQL &= SetItem(ColumnID2, SCH.KFURI_DATE)
                    Case "SAIFURI_DATE_S"
                        '再振替日6
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "KSAIFURI_DATE_S"
                        '契約再振替予日7
                        SQL &= SetItem(ColumnID2, SCH.KSAIFURI_DATE)
                    Case "FURI_CODE_S"
                        '振替コード8
                        SQL &= SetItem(ColumnID2, TR(Index).FURI_CODE)
                    Case "KIGYO_CODE_S"
                        '企業コード9
                        SQL &= SetItem(ColumnID2, TR(Index).KIGYO_CODE)
                    Case "ITAKU_CODE_S"
                        '委託者コード10
                        SQL &= SetItem(ColumnID2, TR(Index).ITAKU_CODE)
                    Case "TKIN_NO_S"
                        '取扱金融機関11
                        SQL &= SetItem(ColumnID2, TR(Index).TKIN_NO)
                    Case "TSIT_NO_S"
                        '取扱店番12
                        SQL &= SetItem(ColumnID2, TR(Index).TSIT_NO)
                    Case "SOUSIN_KBN_S"
                        '送信区分13
                        SQL &= SetItem(ColumnID2, TR(Index).SOUSIN_KBN)
                    Case "MOTIKOMI_KBN_S"
                        '持込区分14
                        SQL &= SetItem(ColumnID2, TR(Index).MOTIKOMI_KBN)
                    Case "BAITAI_CODE_S"
                        '媒体コード15
                        SQL &= SetItem(ColumnID2, TR(Index).BAITAI_CODE)
                    Case "MOTIKOMI_SEQ_S"
                        '持込ＳＥＱ16
                        SQL &= SetItem(ColumnID2, SCH.MOTIKOMI_SEQ.ToString)
                    Case "FILE_SEQ_S"
                        'ファイルＳＥＱ17
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUU_KBN_S"
                        '手数料計算区分18
                        Select Case TR(Index).TESUUTYO_KBN
                            Case 0
                                '手数料徴求区分 = 都度徴求
                                SQL &= SetItem(ColumnID2, "1")
                            Case 1, 3
                                '手数料徴求区分 = 一括徴求
                                Select Case TR(Index).MONTH_FLG
                                    Case 1, 3
                                        SQL &= SetItem(ColumnID2, "2")
                                    Case Else
                                        SQL &= SetItem(ColumnID2, "3")
                                End Select
                            Case Else
                                '手数料徴求区分 = (2)特別免除, (3)別途徴求
                                SQL &= SetItem(ColumnID2, "0")
                        End Select
                    Case "MOTIKOMI_DATE_S"
                        '持込予定日
                        SQL &= SetItem(ColumnID2, SCH.MOTIKOMI_DATE)
                    Case "IRAISYO_DATE_S"
                        '依頼書作成日19
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "IRAISYOK_YDATE_S"
                        '依頼書回収予定日20
                        SQL &= SetItem(ColumnID2, SCH.IRAISYOK_YDATE)
                    Case "UKETUKE_DATE_S"
                        '受付日21
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "TOUROKU_DATE_S"
                        '登録日22
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "HAISIN_YDATE_S"
                        '配信処理予定日23
                        SQL &= SetItem(ColumnID2, SCH.HAISIN_YDATE)
                    Case "HAISIN_DATE_S"
                        '配信処理日24
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "SOUSIN_YDATE_S"
                        '送信予定日25
                        SQL &= SetItem(ColumnID2, SCH.HAISIN_YDATE)
                    Case "SOUSIN_DATE_S"
                        '送信処理日26
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "FUNOU_YDATE_S"
                        '不能処理予定日27
                        SQL &= SetItem(ColumnID2, SCH.FUNOU_YDATE)
                    Case "FUNOU_DATE_S"
                        '不能処理日28
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "KESSAI_YDATE_S"
                        '決済予定日29
                        SQL &= SetItem(ColumnID2, SCH.KESSAI_YDATE)
                    Case "KESSAI_DATE_S"
                        '決済日30
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "TESUU_YDATE_S"
                        '手数料徴求予定日31
                        SQL &= SetItem(ColumnID2, SCH.TESUU_YDATE)
                    Case "TESUU_DATE_S"
                        '手数料徴求日32
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "HENKAN_YDATE_S"
                        '返還処理予定日33
                        SQL &= SetItem(ColumnID2, SCH.HENKAN_YDATE)
                    Case "HENKAN_DATE_S"
                        '返還処理日34
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "UKETORI_DATE_S"
                        '受取書作成日35
                        SQL &= SetItem(ColumnID2, New String("0"c, 8))
                    Case "UKETUKE_FLG_S"
                        '受付済フラグ36
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TOUROKU_FLG_S"
                        '登録済フラグ37
                        SQL &= SetItem(ColumnID2, "0")
                    Case "HAISIN_FLG_S"
                        '配信済フラグ38
                        SQL &= SetItem(ColumnID2, "0")
                    Case "SAIFURI_FLG_S"
                        '再振済フラグ39
                        SQL &= SetItem(ColumnID2, "0")
                    Case "SOUSIN_FLG_S"
                        '送信済フラグ40
                        SQL &= SetItem(ColumnID2, "0")
                    Case "FUNOU_FLG_S"
                        '不能済フラグ41
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUUKEI_FLG_S"
                        '手数料計算済フラグ42
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUUTYO_FLG_S"
                        '手数料徴求済フラグ43
                        SQL &= SetItem(ColumnID2, "0")
                    Case "KESSAI_FLG_S"
                        '決済フラグ44
                        SQL &= SetItem(ColumnID2, "0")
                    Case "HENKAN_FLG_S"
                        '返還済フラグ45
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TYUUDAN_FLG_S"
                        '中断フラグ46
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TAKOU_FLG_S"
                        '他行フラグ47
                        SQL &= SetItem(ColumnID2, "0")
                    Case "NIPPO_FLG_S"
                        '日報フラグ48
                        SQL &= SetItem(ColumnID2, "0")
                    Case "ERROR_INF_S"
                        'エラー情報49
                        SQL &= "NULL"
                    Case "SYORI_KEN_S"
                        '処理件数50
                        SQL &= SetItem(ColumnID2, "0")
                    Case "SYORI_KIN_S"
                        '処理金額51
                        SQL &= SetItem(ColumnID2, "0")
                    Case "ERR_KEN_S"
                        'エラー件数52
                        SQL &= SetItem(ColumnID2, "0")
                    Case "ERR_KIN_S"
                        'エラー金額53
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUU_KIN_S"
                        '手数料金額54
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUU_KIN1_S"
                        '手数料金額１55
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUU_KIN2_S"
                        '手数料金額２56
                        SQL &= SetItem(ColumnID2, "0")
                    Case "TESUU_KIN3_S"
                        '手数料金額３57
                        SQL &= SetItem(ColumnID2, "0")
                    Case "FURI_KEN_S"
                        '振替済件数58
                        SQL &= SetItem(ColumnID2, "0")
                    Case "FURI_KIN_S"
                        '振替済金額59
                        SQL &= SetItem(ColumnID2, "0")
                    Case "FUNOU_KEN_S"
                        '不能件数60
                        SQL &= SetItem(ColumnID2, "0")
                    Case "FUNOU_KIN_S"
                        '不能金額61
                        SQL &= SetItem(ColumnID2, "0")
                    Case "UFILE_NAME_S"
                        '受信ファイル名62
                        SQL &= "NULL"
                    Case "SFILE_NAME_S"
                        '送信ファイル名63
                        SQL &= "NULL"
                    Case "SAKUSEI_DATE_S"
                        '作成日付64
                        SQL &= "TO_CHAR(SYSDATE, 'yyyymmdd')"
                    Case "JIFURI_TIME_STAMP_S"
                        '配信タイムスタンプ65
                        SQL &= SetItem(ColumnID2, New String("0"c, 14))
                    Case "KESSAI_TIME_STAMP_S"
                        '決済タイムスタンプ66
                        SQL &= SetItem(ColumnID2, New String("0"c, 14))
                    Case "TESUU_TIME_STAMP_S"
                        '手数料徴求タイムスタンプ67
                        SQL &= SetItem(ColumnID2, New String("0"c, 14))
                    Case "YOBI1_S"
                        '予備１68
                        SQL &= "NULL"
                    Case "YOBI2_S"
                        '予備２69
                        SQL &= "NULL"
                    Case "YOBI3_S"
                        '予備３70
                        SQL &= "NULL"
                    Case "YOBI4_S"
                        '予備４71
                        SQL &= "NULL"
                    Case "YOBI5_S"
                        '予備５72
                        SQL &= "NULL"
                    Case "YOBI6_S"
                        '予備６73
                        SQL &= "NULL"
                    Case "YOBI7_S"
                        '予備７74
                        SQL &= "NULL"
                    Case "YOBI8_S"
                        '予備８75
                        SQL &= "NULL"
                    Case "YOBI9_S"
                        '予備９76
                        SQL &= "NULL"
                    Case "YOBI10_S"
                        '予備１０77
                        SQL &= "NULL"
                    Case Else
                        SQL &= "NULL"
                End Select
            Next ColumnID2
            SQL &= ")"

            Dim SQLCode As Integer = 0
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode, SEL)
            If Ret = 1 AndAlso SQLCode = 0 Then
                ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
                If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                    Dim ReturnMessage As String = String.Empty
                    Dim SubMastSQL As New StringBuilder
                    Call CAstExternal.ModExternal.Ex_InsertSchmastSub_RetSQL(TR(Index).FSYORI_KBN, _
                                                                             TR(Index).TORIS_CODE, _
                                                                             TR(Index).TORIF_CODE, _
                                                                             SCH.FURI_DATE, _
                                                                             SCH.MOTIKOMI_SEQ, _
                                                                             ReturnMessage, _
                                                                             SubMastSQL)
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SubMastSQL.ToString, SQLCode, SEL)
                End If
                ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END
                Return True
            End If
        Catch ex As Exception

            Throw

        End Try

        Return False
    End Function

    '
    ' 機能　 ： 挿入文支援関数
    '
    ' 引数　 ： ARG1 - ORACLE COLUMN_ID Index
    ' 　　　 　 ARG2 - 設定値
    '
    ' 戻り値 ： SQL文挿入文字列
    '
    ' 備考　 ： なし
    '
    Private Function SetItem(ByVal Index As Integer, ByVal aData As String) As String
        If aData.Length = 0 Then
            Return "NULL"
        Else
            Select Case ORATYPE(Index)
                Case "CHAR"
                    Return "'" & aData & "'"
                Case "NUMBER"
                    Return aData
                Case Else
                    Return "'" & aData & "'"
            End Select
        End If
    End Function

    '
    ' 機能　 ： 他行マスタのチェック
    '
    ' 引数　 ： ARG1 - 蓄積 TORIMAST-Index
    '
    ' 戻り値 ： 伝送以外の相手件数
    '
    ' 備考　 ： ２種類の方法を準備(True／Falseを逆転すれば方法が変わるよ)
    '
    Private Function CheckTakouMast(ByVal Index As Integer) As Integer

        Dim Ret As Integer = 0
        Dim SQL As String
        Dim BRet As Boolean
        Dim REC As OracleDataReader = Nothing

        Try
            Select Case True
                Case True
                    '伝送以外の媒体コードが混入しているか否か
                    SQL = "SELECT COUNT(*) COUNTER"
                    SQL &= " FROM TAKOUMAST"
                    SQL &= " WHERE TORIS_CODE_V = '" & TR(Index).TORIS_CODE & "'"
                    SQL &= " AND TORIF_CODE_V = '" & TR(Index).TORIF_CODE & "'"
                    SQL &= " AND NOT TO_NUMBER(NVL(BAITAI_CODE_V, '0')) = 0"
                    BRet = GCom.SetDynaset(SQL, REC)
                    If BRet AndAlso REC.Read Then
                        Ret = GCom.NzInt(REC.Item("COUNTER"), 0)
                    End If
                Case False
                    '伝送以外の振り分け媒体の有無検出
                    SQL = "SELECT TKIN_NO_V"
                    SQL &= " FROM TAKOUMAST"
                    SQL &= " WHERE TORIS_CODE_V = '" & TR(Index).TORIS_CODE & "'"
                    SQL &= " AND TORIF_CODE_V = '" & TR(Index).TORIF_CODE & "'"
                    BRet = GCom.SetDynaset(SQL, REC)
                    If BRet Then
                        Do While REC.Read
                            Dim BKCode As String = GCom.NzDec(REC.Item("TKIN_NO_V"), "")
                            Dim BAITAI_CODE As String = CASTCommon.GetIni("FURIWAKE.INI", BKCode, "Baitai")
                            If Not GCom.NzInt(BAITAI_CODE, 0) = 0 Then
                                Ret += 1
                            End If
                        Loop
                    End If
            End Select

        Catch ex As Exception
            Throw
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Ret

    End Function

    '
    ' 機能　　　: テキストボックスで入力された日付データを評価する
    '
    ' 戻り値　　: 正常日付 = OK(-1)
    ' 　　　　　  異常日付 = 誤った箇所 (0=年, 1=月, 2=日)
    '
    ' 引き数　　: ARG1(onDate) - 日付
    ' 　　　　　  ARG2(onData) - 年月日情報(配列) (0=年, 1=月, 2=日)
    '
    ' 機能説明　: 日付チェック関数
    '
    ' 備考　　　: 共通関数では使いにくい為、若干改造して専用関数とする。
    '
    Private Function SET_DATE(ByRef onDate As Date, ByRef onData() As Integer) As Integer

        Try
            onDate = DateSerial(onData(0), onData(1), onData(2))

            Select Case False
                Case onDate.Year = onData(0)
                    Return 0
                Case onDate.Month = onData(1)
                    Return 1
                Case onDate.Day = onData(2)
                    Return 2
            End Select

            Return -1

        Catch ex As Exception
            Throw
        End Try

        Return 9

    End Function

End Class
' 2016/01/14 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END
