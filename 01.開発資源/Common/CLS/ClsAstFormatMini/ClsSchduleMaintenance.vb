Option Explicit On 
Option Strict On

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

    '2017/12/05 saitou 広島信金(RSV2標準版) ADD 大規模構築対応 ---------------------------------------- START
    'スケジュールマスタサブ内容
    Public MaxColumn_Sub As Integer = 0
    Public ORANAME_SUB() As String
    Public ORATYPE_SUB() As String
    Public ORASIZE_SUB() As Integer
    Public SCHMAST_SUB() As String
    '2017/12/05 saitou 広島信金(RSV2標準版) ADD ------------------------------------------------------- END

    '一次抽出取引先マスタ配列
    Public Structure TORIMAST_RECORD
        Dim FSYORI_KBN As String                '振込処理区分
        Dim TORIS_CODE As String                '取引先主コード
        Dim TORIF_CODE As String                '取引先副コード
        Dim BAITAI_CODE As String               '媒体コード
        Dim ITAKU_KANRI_CODE As String          '代表委託者コード
        Dim FMT_KBN As Integer                  'フォーマット区分
        Dim SYUBETU As String                   '種別
        Dim ITAKU_CODE As String                '委託者コード
        Dim ITAKU_KNAME As String               '委託者カナ名
        Dim TKIN_NO As String                   '取扱金融機関
        Dim TSIT_NO As String                   '取扱店番
        Dim SOUSIN_KBN As String                '送信区分
        Dim CYCLE As Integer                    'サイクル管理
        Dim KIJITU_KANRI As Integer             '期日管理要否
        Dim FURI_CODE As String                 '振替コード
        Dim KIGYO_CODE As String                '企業コード
        Dim ITAKU_NNAME As String               '委託者漢字名
        Dim FURI_KYU_CODE As Integer            '振込休日コード
        Dim DATEN() As Integer                  'N日の有効／無効
        Dim MONTH_FLG As Integer                '該当月の値？
        Dim KEIYAKU_DATE As String              '契約日
        Dim MOTIKOMI_KIJITSU As Integer         '持込期日
        Dim IRAISHO_YDATE As Integer            '日数／基準日（依頼書）
        Dim IRAISHO_KIJITSU As String           '日付区分（依頼書）
        Dim IRAISHO_KYU_CODE As String          '依頼書休日シフト
        Dim KESSAI_KBN As Integer               '決済区分
        Dim KESSAI_DAY As Integer               '日数／基準日（決済）
        Dim KESSAI_KIJITSU As Integer           '日付区分（決済）
        Dim KESSAI_KYU_CODE As Integer          '決済日休日シフト
        Dim TESUUTYO_KBN As Integer             '手数料徴求区分
        Dim TESUUTYO_PATN As Integer            '手数料徴求方法
        Dim TESUUMAT_NO As Integer              '手数料集計周期
        Dim TESUUTYO_DAY As Integer             '手数料徴求日数／基準日
        Dim TESUUTYO_KIJITSU As Integer         '手数料徴求期日区分
        Dim TESUU_KYU_CODE As Integer           '手数料徴求日休日コード
        Dim TESUUMAT_MONTH As Integer           '集計基準月
        Dim TESUUMAT_ENDDAY As Integer          '集計終了日
        Dim TESUUMAT_KIJYUN As Integer          '集計基準

    End Structure
    Public TR() As TORIMAST_RECORD

    '登録／判定結果情報
    Public Structure SCHMAST_Data
        Dim KFURI_DATE As String            '契約振込日
        Dim FURI_DATE As String             '振込日
        Dim NFURI_DATE As String            '変更振込日
        Dim MOTIKOMI_SEQ As Integer         '持込SEQ
        Dim IRAISYOK_YDATE As String        '依頼書回収予定日
        Dim KAKUHO_YDATE As String          '資金確保予定日
        Dim HASSIN_YDATE As String          '発信処理予定日
        Dim KESSAI_YDATE As String          '決済予定日
        Dim TESUU_YDATE As String           '手数料徴収予定日
        Dim MOTIKOMI_DATE As String         '持込期日
    End Structure
    Public SCH As SCHMAST_Data

    Public Enum OPT
        OptionNothing = 0               '個別登録画面
        OptionAddNew = 1                '新規・再作成
        OptionAppend = 2                '追加作成
    End Enum

    'どの業務から呼び出されるかの識別変数
    Private SchTable As Integer = 0

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
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("SELECT COLUMN_NAME")
            SQL.AppendLine(", DATA_TYPE")
            SQL.AppendLine(", DECODE(DATA_TYPE, 'CHAR', DATA_LENGTH, DATA_PRECISION) DATA_SIZE")
            SQL.AppendLine(" FROM ALL_TAB_COLUMNS")
            SQL.AppendLine(" WHERE UPPER(OWNER) = '" & CASTCommon.DB.USER & "'")
            SQL.AppendLine(" AND TABLE_NAME = 'S_SCHMAST'")
            SQL.AppendLine(" ORDER BY COLUMN_ID ASC")

            If GCom.SetDynaset(SQL.ToString, REC) Then
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
            '2017/12/05 saitou 広島信金(RSV2標準版) ADD 大規模構築対応 ---------------------------------------- START
            If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                SQL.Length = 0
                SQL.AppendLine("SELECT COLUMN_NAME")
                SQL.AppendLine(", DATA_TYPE")
                SQL.AppendLine(", DECODE(DATA_TYPE, 'CHAR', DATA_LENGTH, DATA_PRECISION) DATA_SIZE")
                SQL.AppendLine(" FROM ALL_TAB_COLUMNS")
                SQL.AppendLine(" WHERE UPPER(OWNER) = '" & CASTCommon.DB.USER & "'")
                SQL.AppendLine(" AND TABLE_NAME = 'S_SCHMAST_SUB'")
                SQL.AppendLine(" ORDER BY COLUMN_ID ASC")
                If GCom.SetDynaset(SQL.ToString, REC) Then
                    Do While REC.Read
                        Select Case MaxColumn_Sub
                            Case 0
                                ReDim ORANAME_SUB(0)
                                ReDim ORATYPE_SUB(0)
                                ReDim ORASIZE_SUB(0)
                            Case Else
                                ReDim Preserve ORANAME_SUB(MaxColumn_Sub)
                                ReDim Preserve ORATYPE_SUB(MaxColumn_Sub)
                                ReDim Preserve ORASIZE_SUB(MaxColumn_Sub)
                        End Select
                        ORANAME_SUB(MaxColumn_Sub) = GCom.NzStr(REC.Item("COLUMN_NAME")).Trim.ToUpper
                        ORATYPE_SUB(MaxColumn_Sub) = GCom.NzStr(REC.Item("DATA_TYPE")).Trim.ToUpper
                        ORASIZE_SUB(MaxColumn_Sub) = GCom.NzInt(REC.Item("DATA_SIZE"), 0)
                        MaxColumn_Sub += 1
                    Loop
                    MaxColumn_Sub -= 1
                    ReDim SCHMAST_SUB(MaxColumn_Sub)
                End If
            End If
            '2017/12/05 saitou 広島信金(RSV2標準版) ADD ------------------------------------------------------- END

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
    ' 機能　 ： スケジュールマスタ項目値配列参照
    '
    ' 引数　 ： ARG1 - 項目名=コントロール名
    ' 　　　 　 ARG2 - 返答評価識別
    '
    ' 戻り値 ： 単純文字列値
    '
    ' 備考　 ： なし
    '
    Public Function GetColumnValue(ByVal ColumnName As String, ByVal ReturnType As String) As String
        Dim ReturnValue As String = ReturnType
        For Index As Integer = 0 To MaxColumn Step 1
            If ORANAME(Index) = ColumnName Then
                ReturnValue = GCom.NzStr(SCHMAST(Index)).Trim
                Exit For
            End If
        Next Index
        '2017/12/05 saitou 広島信金(RSV2標準版) ADD 大規模構築対応 ---------------------------------------- START
        If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
            For Index As Integer = 0 To MaxColumn_Sub Step 1
                If ORANAME_SUB(Index) = ColumnName Then
                    ReturnValue = GCom.NzStr(SCHMAST_SUB(Index)).Trim
                    Exit For
                End If
            Next
        End If
        '2017/12/05 saitou 広島信金(RSV2標準版) ADD ------------------------------------------------------- END
        Return ReturnValue
    End Function

    '
    ' 機能　 ： スケジュールマスタ項目値配列参照
    '
    ' 引数　 ： ARG1 - 項目名=コントロール名
    '
    ' 戻り値 ： 文字列型数値情報
    '
    ' 備考　 ： なし
    '
    Public Function GetColumnValue(ByVal ColumnName As String) As String
        Dim ReturnValue As String = ""
        For Index As Integer = 0 To MaxColumn Step 1
            If ORANAME(Index) = ColumnName Then
                ReturnValue = GCom.NzDec(SCHMAST(Index), "")
                Exit For
            End If
        Next Index
        '2017/12/05 saitou 広島信金(RSV2標準版) ADD 大規模構築対応 ---------------------------------------- START
        If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
            For Index As Integer = 0 To MaxColumn_Sub Step 1
                If ORANAME_SUB(Index) = ColumnName Then
                    ReturnValue = GCom.NzDec(SCHMAST_SUB(Index), "")
                    Exit For
                End If
            Next
        End If
        '2017/12/05 saitou 広島信金(RSV2標準版) ADD ------------------------------------------------------- END
        Return ReturnValue
    End Function

    '
    ' 機能　 ： スケジュールマスタ項目値配列参照
    '
    ' 引数　 ： ARG1 - 項目名=コントロール名
    ' 　　　 　 ARG2 - 返答評価識別
    '
    ' 戻り値 ： 数値型項目値
    '
    ' 備考　 ： 整数値(Integer)
    '
    Public Function GetColumnValue(ByVal ColumnName As String, ByVal ReturnType As Integer) As Integer
        Dim ReturnValue As Integer = ReturnType
        For Index As Integer = 0 To MaxColumn Step 1
            If ORANAME(Index) = ColumnName Then
                ReturnValue = GCom.NzInt(SCHMAST(Index), 0)
                Exit For
            End If
        Next Index
        '2017/12/05 saitou 広島信金(RSV2標準版) ADD 大規模構築対応 ---------------------------------------- START
        If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
            For Index As Integer = 0 To MaxColumn_Sub Step 1
                If ORANAME_SUB(Index) = ColumnName Then
                    ReturnValue = GCom.NzInt(SCHMAST_SUB(Index), 0)
                    Exit For
                End If
            Next
        End If
        '2017/12/05 saitou 広島信金(RSV2標準版) ADD ------------------------------------------------------- END
        Return ReturnValue
    End Function

    '
    ' 機能　 ： スケジュールマスタ項目値配列参照
    '
    ' 引数　 ： ARG1 - 項目名=コントロール名
    ' 　　　 　 ARG2 - 返答評価識別
    ' 　　　 　 ARG3 - 桁数の大きい項目識別
    '
    ' 戻り値 ： 数値型項目値
    '
    ' 備考　 ： 十進数値(Decimal)
    '
    Public Function GetColumnValue(ByVal ColumnName As String, _
                ByVal ReturnType As Integer, ByVal DecimalType As Integer) As Decimal
        Dim ReturnValue As Decimal = ReturnType
        For Index As Integer = 0 To MaxColumn Step 1
            If ORANAME(Index) = ColumnName Then
                ReturnValue = GCom.NzDec(SCHMAST(Index), 0)
                Exit For
            End If
        Next Index
        '2017/12/05 saitou 広島信金(RSV2標準版) ADD 大規模構築対応 ---------------------------------------- START
        If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
            For Index As Integer = 0 To MaxColumn_Sub Step 1
                If ORANAME_SUB(Index) = ColumnName Then
                    ReturnValue = GCom.NzDec(SCHMAST_SUB(Index), 0)
                    Exit For
                End If
            Next
        End If
        '2017/12/05 saitou 広島信金(RSV2標準版) ADD ------------------------------------------------------- END
        Return ReturnValue
    End Function

    '
    ' 機能　 ： 休日情報の蓄積
    '
    ' 引数　 ： ARG1 - 振込日データ(画面指定)
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

                    Dim SQL As String = " WHERE SUBSTR(YASUMI_DATE_Y, 1, 6)" & _
                                        " IN ('" & String.Format("{0:yyyyMM}", ProviousMonthDate) & "'" & _
                                        ", '" & String.Format("{0:yyyyMM}", FormFuriDate) & "'" & _
                                        ", '" & String.Format("{0:yyyyMM}", NextMonthDate) & "')"

                    Dim BRet As Boolean = GCom.CheckDateModule(Nothing, 1, SQL)
            End Select
        Catch ex As Exception
            Throw
        End Try
    End Sub

    '
    ' 機能　 ： スケジュール作成対象の取引先コードを抽出
    '
    ' 引数　 ： ARG1 - 振込日年月
    ' 　　　 　 ARG2 - 取引先主コード
    ' 　　　 　 ARG3 - 取引先副コード
    ' 　　　 　 ARG4 - 呼出元識別指定
    '
    ' 戻り値 ： OK = True, NG = False(対象取引先なし)
    '
    ' 備考　 ： 該当の月が対象のもので振込日が開始／終了の間にあるもの
    '
    Public Function GET_SELECT_TORIMAST(ByVal FormFuriDate As Date, _
            ByVal TORIS_CODE As String, ByVal TORIF_CODE As String, _
            Optional ByVal SEL_OPTION As Integer = OPT.OptionNothing) As Boolean

        Dim REC As OracleDataReader = Nothing
        Try
            '該当月
            Dim MonthColumn As String = "TUKI" & FormFuriDate.Month.ToString & "_T"

            Dim SQL As StringBuilder = New StringBuilder(SetToriMastSelectBaseSql(MonthColumn))
            SQL.AppendLine(" WHERE FSYORI_KBN_T = '3'")

            Select Case SEL_OPTION
                Case OPT.OptionNothing
                    '個別登録画面
                    SQL.AppendLine(" AND TORIS_CODE_T = '" & TORIS_CODE & "'")
                    SQL.AppendLine(" AND TORIF_CODE_T = '" & TORIF_CODE & "'")
                Case OPT.OptionAddNew, OPT.OptionAppend
                    '月間スケジュール作成
                    SQL.AppendLine(" AND NOT " & MonthColumn & " = '0'")
                    SQL.AppendLine(" AND '" & String.Format("{0:yyyyMM}", FormFuriDate) & "'")
                    SQL.AppendLine(" BETWEEN SUBSTR(KAISI_DATE_T, 1, 6)")
                    SQL.AppendLine(" AND SUBSTR(SYURYOU_DATE_T, 1, 6)")
                    SQL.AppendLine(" AND KIJITU_KANRI_T = '1'")

                    '追加の場合には取引先マスタの変更があったもの
                    If SEL_OPTION = OPT.OptionAppend Then
                        SQL.AppendLine(" AND NOT KOUSIN_SIKIBETU_T = '2'")
                    End If
            End Select

            If GCom.SetDynaset(SQL.ToString, REC) Then

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
        Try
            Dim SQL As StringBuilder = New StringBuilder(128)
            SQL.AppendLine("SELECT FSYORI_KBN_T")               '振込処理区分
            SQL.AppendLine(", TORIS_CODE_T")                    '取引先主コード
            SQL.AppendLine(", TORIF_CODE_T")                    '取引先副コード
            SQL.Append(", " & MonthColumn & " MONTH")           '設定月
            For Index As Integer = 1 To 31 Step 1
                SQL.Append(", DATE" & Index.ToString & "_T")
            Next Index
            SQL.AppendLine()
            SQL.AppendLine(", SOUSIN_KBN_T")                    '送信区分
            SQL.AppendLine(", FURI_KYU_CODE_T")                 '休日コード
            SQL.AppendLine(", FMT_KBN_T")                       'フォーマット区分
            SQL.AppendLine(", TESUUTYO_KIJITSU_T")              '手数料徴求期日区分
            SQL.AppendLine(", TESUUTYO_KBN_T")                  '手数料徴求区分
            SQL.AppendLine(", TESUUTYO_DAY_T")                  '手数料徴求日数／基準日
            SQL.AppendLine(", TESUU_KYU_CODE_T")                '手数料徴求日休日コード
            SQL.AppendLine(", KESSAI_KIJITSU_T")                '決済日指定区分
            SQL.AppendLine(", KESSAI_DAY_T")                    '決済日日数／基準日
            SQL.AppendLine(", KESSAI_KYU_CODE_T")               '決済休日コード
            SQL.AppendLine(", CYCLE_T")                         'サイクル管理
            SQL.AppendLine(", KIJITU_KANRI_T")                  '期日管理要否
            SQL.AppendLine(", FURI_CODE_T")                     '振替コード
            SQL.AppendLine(", KIGYO_CODE_T")                    '企業コード
            SQL.AppendLine(", ITAKU_CODE_T")                    '委託者コード
            SQL.AppendLine(", TKIN_NO_T")                       '取扱金融機関
            SQL.AppendLine(", TSIT_NO_T")                       '取扱店番
            SQL.AppendLine(", BAITAI_CODE_T")                   '媒体コード
            SQL.AppendLine(", SYUBETU_T")                       '種別コード
            SQL.AppendLine(", ITAKU_KNAME_T")                   '委託者カナ名
            SQL.AppendLine(", ITAKU_NNAME_T")                   '委託者漢字名
            SQL.AppendLine(", MOTIKOMI_KIJITSU_T")              '持込期日
            SQL.AppendLine(", TESUUMAT_NO_T")                   '手数料集計周期
            SQL.AppendLine(", TESUUMAT_MONTH_T")                '集計基準月
            SQL.AppendLine(", TESUUMAT_ENDDAY_T")               '集計終了日
            SQL.AppendLine(", TESUUMAT_KIJYUN_T")               '集計基準
            SQL.AppendLine(", TESUUTYO_PATN_T")                 '手数料徴求方法
            SQL.AppendLine(", KESSAI_KBN_T")                    '決済区分
            SQL.AppendLine(" FROM S_TORIMAST")

            Return SQL.ToString

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
        Try
            Dim Temp As String = ""

            With TR(Counter)
                .FSYORI_KBN = GCom.NzDec(REC.Item("FSYORI_KBN_T"), "")              '振込処理区分
                .TORIS_CODE = GCom.NzStr(REC.Item("TORIS_CODE_T"))              '取引先主コード
                .TORIF_CODE = GCom.NzDec(REC.Item("TORIF_CODE_T"), "")              '取引先副コード
                .ITAKU_CODE = GCom.NzDec(REC.Item("ITAKU_CODE_T"), "")              '委託者コード
                .ITAKU_KNAME = GCom.NzStr(REC.Item("ITAKU_KNAME_T"))                '委託者カナ名
                .ITAKU_NNAME = GCom.NzStr(REC.Item("ITAKU_NNAME_T"))                '委託者漢字名
                .MONTH_FLG = GCom.NzInt(REC.Item("MONTH"), 0)                       '該当指定月の値

                '振込該当日の有効／無効
                ReDim TR(Counter).DATEN(31)
                For Index As Integer = 1 To 31 Step 1
                    Temp = "DATE" & Index.ToString & "_T"
                    TR(Counter).DATEN(Index) = GCom.NzInt(REC.Item(Temp), 0)
                Next Index

                .SOUSIN_KBN = GCom.NzDec(REC.Item("SOUSIN_KBN_T"), "")              '送信区分
                .FURI_KYU_CODE = GCom.NzInt(REC.Item("FURI_KYU_CODE_T"), 0)         '休日コード
                .FMT_KBN = GCom.NzInt(REC.Item("FMT_KBN_T"), 0)                     'フォーマット区分
                .CYCLE = GCom.NzInt(REC.Item("CYCLE_T"), 0)                         'サイクル管理
                .KIJITU_KANRI = GCom.NzInt(REC.Item("KIJITU_KANRI_T"), 0)           '期日管理要否
                .FURI_CODE = GCom.NzDec(REC.Item("FURI_CODE_T"), "")                '振替コード
                .KIGYO_CODE = GCom.NzDec(REC.Item("KIGYO_CODE_T"), "")              '企業コード
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
                .TESUUMAT_NO = GCom.NzInt(REC.Item("TESUUMAT_NO_T"), 0)             '手数料集計周期
                If .TESUUMAT_NO > 12 Then
                    .TESUUMAT_NO = 12
                End If
                .TESUUMAT_MONTH = GCom.NzInt(REC.Item("TESUUMAT_MONTH_T"), 0)       '集計基準月
                .TESUUMAT_ENDDAY = GCom.NzInt(REC.Item("TESUUMAT_ENDDAY_T"), 0)     '集計終了日
                .TESUUMAT_KIJYUN = GCom.NzInt(REC.Item("TESUUMAT_KIJYUN_T"), 0)     '集計基準
                .TESUUTYO_PATN = GCom.NzInt(REC.Item("TESUUTYO_PATN_T"), 0)         '手数料徴求方法
                .KESSAI_KBN = GCom.NzInt(REC.Item("KESSAI_KBN_T"), 0)               '決済区分

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
        Dim SQL As StringBuilder = New StringBuilder(512)
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

            Dim KYUFURI As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "KYUFURI"), 0)
            Dim SOUFURI As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "SOUFURI"), 0)
            Dim HASSHIN As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "HASSIN"), 0)

            '資金確保予定日(後の決済予定日算出処理で書き換える)
            Select Case TR(Index).SYUBETU
                Case Is = "11"
                    '給与振込
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE, KYUFURI, 1)
                Case Is = "12"
                    '賞与振込
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE, KYUFURI, 1)
                Case Is = "21"
                    '総合振込
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KAKUHO_YDATE, SOUFURI, 1)
                Case Else
            End Select

            '発信予定日
            Select Case TR(Index).SYUBETU
                Case Is = "11"
                    '給与振込
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE, HASSHIN, 1)
                Case Is = "12"
                    '賞与振込
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE, HASSHIN, 1)
                Case Is = "21"
                    '総合振込
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.HASSIN_YDATE, HASSHIN, 1)
                Case Else
            End Select

            '------------------------------------------------------------------
            '依頼書回収予定日を求める
            '------------------------------------------------------------------
            BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.IRAISYOK_YDATE, GCom.DayINI.KAISYU, 1)

            '------------------------------------------------------------------
            '決済予定日を求める
            '------------------------------------------------------------------
            'KESSAI_KIJITSU     '決済期日区分(0:営業日数指定, 1:基準日指定, 2:翌月基準日指定)
            'KESSAI_DAY         '決済日数／基準日(1, 2, 3, 4, 6, 12 ※一括徴求の場合のみ有効)
            'KESSAI_KYU_CODE    '決済休日コード(0:翌営業日振込, 1:前営業日振込)

            If TR(Index).KESSAI_DAY = 0 Then
                TR(Index).KESSAI_DAY = 1
            End If

            Select Case TR(Index).KESSAI_KIJITSU
                Case 0
                    '営業日数指定
                    Dim FrontBackType As Integer = 0
                    FrontBackType = 1
                    BRet = GCom.CheckDateModule(SCH.FURI_DATE, SCH.KESSAI_YDATE, TR(Index).KESSAI_DAY, FrontBackType)
                Case 1
                    '基準日指定(月末補正あり)
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

                    '振込日より手前の場合には翌月へ再計算
                    If SCH.FURI_DATE > SCH.KESSAI_YDATE Then
                        '2010.03.01 契約振込日に変更 start
                        'onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                        onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                        '2010.03.01 契約振込日に変更 end
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

            SCH.KAKUHO_YDATE = SCH.KESSAI_YDATE
            SCH.KESSAI_YDATE = SCH.FURI_DATE
            If SCH.HASSIN_YDATE < SCH.KAKUHO_YDATE Then
                SCH.HASSIN_YDATE = SCH.KAKUHO_YDATE
            End If

            '------------------------------------------------------------------
            '手数料徴収予定日を求める
            '------------------------------------------------------------------
            'TESUUTYO_KIJITSU   '手数料徴求期日区分(0:営業日数指定, 1:基準日指定, 2:翌月基準日指定)
            'TESUUTYO_KBN       '手数料徴求区分(0:都度徴求, 1:一括徴求, 2:特別免除, 3:別途徴求(一括) 4:別途徴求(都度))
            'TESUUTYO_NO        '手数料徴求日数／基準日
            'TESUU_KYU_CODE     '手数料徴求日休日コード(0:翌営業日振込, 1:前営業日振込)
            '以下、一括徴求の場合のみ有効
            'TESUUMAT_DAY        '手数料集計周期 (2008.03.06) Ｎヶ月(1,2,3,4,6,12)=12の約数
            'TESUUMAT_MONTH    '集計基準月     (2008.03.06)
            'TESUUMAT_ENDDAY       '集計終了日     (2008.03.06)
            'TESUUMAT_KIJYUN2   '集計基準       (2008.03.06) (0:振込日, 1:決済日)
            'TESUUTYO_PATN      '手数料徴求方法 (2008.03.06) (0:差引入金, 1:直接入金)
            'KESSAI_KBN         '決済区分       (2008.03.06) 
            '決済区分 = (0:預け金, 1:口座入金, 2:為替振込, 3:為替付替, 4:特別企業, 5:決済対象外)
            '2008.03.06 FJH 仕様
            '集計開始基準月は(TESUUMAT_KIJYUN)で観る。
            '集計に入れるか否かは(TESUUMAT_KIJYUN2)で判断。振込日は契約日で、決済日は計算予定日で行う。
            '集計に入れるか否かの基準は(TESUUMAT_END)で行う。

            If TR(Index).TESUUTYO_DAY = 0 Then
                TR(Index).TESUUTYO_DAY = 1
            End If

            Select Case TR(Index).TESUUTYO_KBN

                Case 1

                    '一括徴求(差引入金は絶対にない。→仕様)

                    '手数料徴求月を配列化する
                    Dim ALL_Month(GCom.NzInt(12 \ TR(Index).TESUUMAT_NO)) As Integer

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
                            '振込日の場合
                            onText(0) = GCom.NzInt(SCH.FURI_DATE.Substring(0, 4), 0)
                            onText(1) = GCom.NzInt(SCH.FURI_DATE.Substring(4, 2), 0)
                            onText(2) = GCom.NzInt(SCH.FURI_DATE.Substring(6, 2), 0)
                            CompDay = onText(2)
                        Case Else
                            '決済日の場合
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

                    '手数料徴収月配列内の月値が振込月値を追い越せなかった場合
                    If Cnt > ALL_Month.GetUpperBound(0) Then
                        '次年度の最初の徴収月を設定する
                        onText(0) += 1
                        onText(1) = ALL_Month(1)
                    Else
                        '集計基準が決済日の場合を考慮
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

                    '手数料徴求月の振込日を算出する。
                    Dim Temp_FURI_DATE As String = ""

                    '一括徴求・別途徴求時の徴求予定日計算
                    Select Case TR(Index).TESUUTYO_KBN
                        Case 1, 3
                            Temp = Temp.Substring(0, 6) & "15"
                    End Select

                    Select Case TR(Index).FURI_KYU_CODE
                        Case 0, 1
                            '振込日,営業日判定(土・日・祝祭日判定)
                            BRet = GCom.CheckDateModule(Temp, Temp_FURI_DATE, TR(Index).FURI_KYU_CODE)
                        Case Else
                            '振込日,営業日判定(土・日・祝祭日判定)
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
                            SCH.TESUU_YDATE = SCH.KAKUHO_YDATE
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

                                    '振込日より手前の場合には翌月へ再計算
                                    If SCH.FURI_DATE > SCH.TESUU_YDATE Then
                                        '2010.03.01 契約振込日に変更 start
                                        onDate = GCom.SET_DATE(SCH.KFURI_DATE).AddMonths(1)
                                        'onDate = GCom.SET_DATE(SCH.FURI_DATE).AddMonths(1)
                                        '2010.03.01 契約振込日に変更 end
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
                                    SCH.TESUU_YDATE = SCH.KAKUHO_YDATE

                            End Select

                            '手数料徴求予定日が確保日よりも前の場合
                            If SCH.TESUU_YDATE < SCH.KAKUHO_YDATE Then
                                SCH.TESUU_YDATE = SCH.KAKUHO_YDATE
                            End If

                            '手数料徴求区分が「確保日指定」の場合、手数料徴求予定日を資金確保予定日と同じにします。
                            If TR(Index).TESUUTYO_KIJITSU = 4 Then
                                SCH.TESUU_YDATE = SCH.KAKUHO_YDATE
                            End If

                    End Select
            End Select

            '計算ロジックのみで関数を抜ける場合(2008.03.18 By Astar)
            If CALC_ONLY Then
                Return True
            End If

            '------------------------------------------------------------------
            'マスタ登録項目設定(SQL文)
            '------------------------------------------------------------------
            SQL.AppendLine("INSERT INTO S_SCHMAST")
            
            For ColumnID1 = 0 To MaxColumn Step 1
                Select Case ColumnID1
                    Case 0
                        SQL.Append(" (" & ORANAME(ColumnID1))
                    Case Else
                        SQL.AppendLine(", " & ORANAME(ColumnID1))
                End Select
            Next ColumnID1
            SQL.Append(") VALUES")

            For ColumnID2 = 0 To MaxColumn Step 1

                If ColumnID2 = 0 Then
                    SQL.AppendLine(" (")
                Else
                    SQL.Append(", ")
                End If

                Select Case ORANAME(ColumnID2)
                    Case "FSYORI_KBN_S"
                        '振込処理区分1
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).FSYORI_KBN))
                    Case "TORIS_CODE_S"
                        '取引先主コード2
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).TORIS_CODE))
                    Case "TORIF_CODE_S"
                        '取引先副コード3
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).TORIF_CODE))
                    Case "FURI_DATE_S"
                        '振込日4
                        SQL.AppendLine(SetItem(ColumnID2, SCH.FURI_DATE))
                    Case "KFURI_DATE_S"
                        '契約振込日5
                        SQL.AppendLine(SetItem(ColumnID2, SCH.KFURI_DATE))
                    Case "SYUBETU_S"
                        '種別6
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).SYUBETU))
                    Case "FURI_CODE_S"
                        '振替コード7
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).FURI_CODE))
                    Case "KIGYO_CODE_S"
                        '企業コード8
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).KIGYO_CODE))
                    Case "ITAKU_CODE_S"
                        '委託者コード9
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).ITAKU_CODE))
                    Case "TKIN_NO_S"
                        '取扱金融機関10
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).TKIN_NO))
                    Case "TSIT_NO_S"
                        '取扱店番11
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).TSIT_NO))
                    Case "SOUSIN_KBN_S"
                        '送信区分12
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).SOUSIN_KBN))
                    Case "BAITAI_CODE_S"
                        '媒体コード13
                        SQL.AppendLine(SetItem(ColumnID2, TR(Index).BAITAI_CODE))
                    Case "MOTIKOMI_SEQ_S"
                        '持込SEQ14
                        SQL.AppendLine(SetItem(ColumnID2, SCH.MOTIKOMI_SEQ.ToString))
                    Case "FILE_SEQ_S"
                        'ファイルSEQ15
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUU_KBN_S"
                        '手数料計算区分16
                        Select Case TR(Index).TESUUTYO_KBN
                            Case 0
                                '手数料徴求区分 = 都度徴求
                                SQL.AppendLine(SetItem(ColumnID2, "1"))
                            Case 1
                                '手数料徴求区分 = 一括徴求
                                Select Case TR(Index).MONTH_FLG
                                    Case 1, 3
                                        SQL.AppendLine(SetItem(ColumnID2, "2"))
                                    Case Else
                                        SQL.AppendLine(SetItem(ColumnID2, "3"))
                                End Select
                            Case Else
                                '手数料徴求区分 = (2)特別免除, (3)別途徴求
                                SQL.AppendLine(SetItem(ColumnID2, "0"))
                        End Select
                    Case "MOTIKOMI_DATE_S"
                        '持込予定日17
                        SQL.AppendLine(SetItem(ColumnID2, SCH.MOTIKOMI_DATE))
                    Case "IRAISYO_DATE_S"
                        '依頼書作成日18
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "IRAISYOK_YDATE_S"
                        '依頼書回収予定日19
                        SQL.AppendLine(SetItem(ColumnID2, SCH.IRAISYOK_YDATE))
                    Case "UKETUKE_DATE_S"
                        '受付日20
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "TOUROKU_DATE_S"
                        '登録日21
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "KAKUHO_YDATE_S"
                        '資金確保予定日22
                        SQL.AppendLine(SetItem(ColumnID2, SCH.KAKUHO_YDATE))
                    Case "KAKUHO_DATE_S"
                        '資金確保日23
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "HASSIN_YDATE_S"
                        '発信予定日24
                        SQL.AppendLine(SetItem(ColumnID2, SCH.HASSIN_YDATE))
                    Case "HASSIN_DATE_S"
                        '発信日25
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "SOUSIN_YDATE_S"
                        '送信予定日26
                        SQL.AppendLine(SetItem(ColumnID2, SCH.HASSIN_YDATE))
                    Case "SOUSIN_DATE_S"
                        '送信処理日27
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "KESSAI_YDATE_S"
                        '決済予定日28
                        SQL.AppendLine(SetItem(ColumnID2, SCH.KESSAI_YDATE))
                    Case "KESSAI_DATE_S"
                        '決済日29
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "TESUU_YDATE_S"
                        '手数料徴求予定日30
                        SQL.AppendLine(SetItem(ColumnID2, SCH.TESUU_YDATE))
                    Case "TESUU_DATE_S"
                        '手数料徴求日31
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "UKETORI_DATE_S"
                        '受取書作成日32
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 8)))
                    Case "UKETUKE_FLG_S"
                        '受付済フラグ33
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TOUROKU_FLG_S"
                        '登録済フラグ34
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "KAKUHO_FLG_S"
                        '確保済フラグ35
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "HASSIN_FLG_S"
                        '発信済フラグ36
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "SOUSIN_FLG_S"
                        '送信済フラグ37
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUUKEI_FLG_S"
                        '手数料計算済フラグ38
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUUTYO_FLG_S"
                        '手数料徴求済フラグ39
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "KESSAI_FLG_S"
                        '決済フラグ40
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TYUUDAN_FLG_S"
                        '中断フラグ41
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "ERROR_INF_S"
                        'エラー情報42
                        SQL.AppendLine("NULL")
                    Case "SYORI_KEN_S"
                        '処理件数43
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "SYORI_KIN_S"
                        '処理金額44
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "ERR_KEN_S"
                        'エラー件数45
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "ERR_KIN_S"
                        'エラー金額46
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUU_KIN_S"
                        '手数料金額47
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUU_KIN1_S"
                        '手数料金額１48
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUU_KIN2_S"
                        '手数料金額２49
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "TESUU_KIN3_S"
                        '手数料金額３50
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "FURI_KEN_S"
                        '振込済件数51
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "FURI_KIN_S"
                        '振込済金額52
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "FUNOU_KEN_S"
                        '不能件数53
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "FUNOU_KIN_S"
                        '不能金額54
                        SQL.AppendLine(SetItem(ColumnID2, "0"))
                    Case "UFILE_NAME_S"
                        '受信ファイル名55
                        SQL.AppendLine("NULL")
                    Case "SFILE_NAME_S"
                        '送信ファイル名56
                        SQL.AppendLine("NULL")
                    Case "SAKUSEI_DATE_S"
                        '作成日付57
                        SQL.AppendLine("TO_CHAR(SYSDATE, 'yyyymmdd')")
                    Case "KAKUHO_TIME_STAMP_S"
                        '確保タイムスタンプ58
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 14)))
                    Case "HASSIN_TIME_STAMP_S"
                        '発信タイムスタンプ59
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 14)))
                    Case "KESSAI_TIME_STAMP_S"
                        '決済タイムスタンプ60
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 14)))
                    Case "TESUU_TIME_STAMP_S"
                        '手数料徴求タイムスタンプ61
                        SQL.AppendLine(SetItem(ColumnID2, New String("0"c, 14)))
                    Case "YOBI1_S"
                        '予備１62
                        SQL.AppendLine("NULL")
                    Case "YOBI2_S"
                        '予備２63
                        SQL.AppendLine("NULL")
                    Case "YOBI3_S"
                        '予備３64
                        SQL.AppendLine("NULL")
                    Case "YOBI4_S"
                        '予備４65
                        SQL.AppendLine("NULL")
                    Case "YOBI5_S"
                        '予備５66
                        SQL.AppendLine("NULL")
                    Case "YOBI6_S"
                        '予備６67
                        SQL.AppendLine("NULL")
                    Case "YOBI7_S"
                        '予備７68
                        SQL.AppendLine("NULL")
                    Case "YOBI8_S"
                        '予備８69
                        SQL.AppendLine("NULL")
                    Case "YOBI9_S"
                        '予備９70
                        SQL.AppendLine("NULL")
                    Case "YOBI10_S"
                        '予備１０71
                        SQL.AppendLine("NULL")
                    Case Else
                        SQL.AppendLine("NULL")
                End Select
            Next ColumnID2
            SQL.AppendLine(")")

            Dim SQLCode As Integer = 0
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode, SEL)

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
    ' 機能　 ： スケジュールマスタから削除する
    '
    ' 引数　 ： ARG1 - 振込日指定年月
    '
    ' 戻り値 ： OK = True, NG = False
    '
    ' 備考　 ： 未使用の該当年月スケジュールを削除する(契約振込日条件)
    '
    Public Function DELETE_SCHMAST(ByVal FormFuriDate As Date) As Boolean
        Try
            Dim StartDay As String = String.Format("{0:yyyyMM}", FormFuriDate) & "01"
            Dim onDate As Date = FormFuriDate.AddMonths(1)
            onDate = GCom.SET_DATE(String.Format("{0:yyyyMM}", onDate) & "01")
            onDate = onDate.AddDays(-1)
            Dim EndDay As String = String.Format("{0:yyyyMMdd}", onDate)

            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("DELETE FROM S_SCHMAST")
            SQL.AppendLine(" WHERE KFURI_DATE_S BETWEEN '" & StartDay & "' AND '" & EndDay & "'")
            SQL.AppendLine(" AND UKETUKE_FLG_S = '0'")
            SQL.AppendLine(" AND TOUROKU_FLG_S = '0'")
            SQL.AppendLine(" AND TYUUDAN_FLG_S = '0'")
            
            Dim SQLCode As Integer
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode)

            ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
            If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                If SQLCode = 0 Then
                    Dim Dsql As New StringBuilder(128)
                    Dsql.Length = 0
                    Dsql.Append("DELETE FROM S_SCHMAST_SUB ")
                    Dsql.Append(" WHERE ")
                    Dsql.Append("     NOT EXISTS")
                    Dsql.Append("     (")
                    Dsql.Append("      SELECT")
                    Dsql.Append("          *")
                    Dsql.Append("      FROM")
                    Dsql.Append("          S_SCHMAST")
                    Dsql.Append("      WHERE")
                    Dsql.Append("          TORIS_CODE_S       = TORIS_CODE_SSUB")
                    Dsql.Append("      AND TORIF_CODE_S       = TORIF_CODE_SSUB")
                    Dsql.Append("      AND FURI_DATE_S        = FURI_DATE_SSUB")
                    Dsql.Append("      AND MOTIKOMI_SEQ_S     = MOTIKOMI_SEQ_SSUB")
                    Dsql.Append("     )")

                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, Dsql.ToString, SQLCode)
                End If
            End If
            ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END

            Return (SQLCode = 0)

        Catch ex As Exception
            Throw
        End Try

        Return False
    End Function

    '
    ' 機能　 ： スケジュールマスタ削除
    '
    ' 引数　 ： なし
    '
    ' 戻り値 ： 正常終了 = True, 異常終了 = False
    '
    ' 備考　 ： 個別スケジュール登録用
    '
    Public Function DELETE_SCHMAST(Optional ByVal cFlg As Boolean = True) As Boolean
        Try
            Dim SQL As StringBuilder = New StringBuilder(128)
            SQL.AppendLine("DELETE FROM S_SCHMAST")
            SQL.AppendLine(" WHERE FSYORI_KBN_S = '" & TR(0).FSYORI_KBN & "'")
            SQL.AppendLine(" AND TORIS_CODE_S = '" & TR(0).TORIS_CODE & "'")
            SQL.AppendLine(" AND TORIF_CODE_S = '" & TR(0).TORIF_CODE & "'")
            SQL.AppendLine(" AND FURI_DATE_S = '" & SCH.FURI_DATE & "'")
            SQL.AppendLine(" AND MOTIKOMI_SEQ_S = " & SCH.MOTIKOMI_SEQ)

            Dim SQLCode As Integer = 0
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode)

            ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
            If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                If Ret = 1 AndAlso SQLCode = 0 Then
                    SQL.Length = 0
                    SQL.AppendLine("DELETE FROM S_SCHMAST_SUB")
                    SQL.AppendLine(" WHERE FSYORI_KBN_SSUB = '" & TR(0).FSYORI_KBN & "'")
                    SQL.AppendLine(" AND TORIS_CODE_SSUB   = '" & TR(0).TORIS_CODE & "'")
                    SQL.AppendLine(" AND TORIF_CODE_SSUB   = '" & TR(0).TORIF_CODE & "'")
                    SQL.AppendLine(" AND FURI_DATE_SSUB    = '" & SCH.FURI_DATE & "'")
                    SQL.AppendLine(" AND MOTIKOMI_SEQ_SSUB = " & SCH.MOTIKOMI_SEQ)

                    SQLCode = 0
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode)
                End If
            End If
            ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END

            If cFlg = False Then Return True

            If Ret = 1 AndAlso SQLCode = 0 Then
                Return True
            End If

        Catch ex As Exception
            Throw
        End Try

        Return False
    End Function

    Function SEARCH_NEW_SCHMAST(ByVal TorisCode As String, ByVal TorifCode As String, ByVal NewFuriDate As String) As Boolean
        '============================================================================
        'NAME           :fn_NEW_SCHMAST_KAKUNINN
        'Parameter      :astrTORIS_CODE：取引先主コード／astrTORIF_CODE：取引先副コード／
        '               :astrNEW_FURI_DATE：新振込日
        'Description    :新スケジュールが存在しないことを確認
        'Return         :True=OK(存在する),False=NG（存在しない）
        'Create         :2007/05/28
        'Update         :
        '============================================================================

        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As StringBuilder = New StringBuilder
            Dim BRet As Boolean

            SQL.AppendLine("SELECT COUNT(*) AS COUNTER FROM S_SCHMAST")
            SQL.AppendLine(" WHERE ")
            SQL.AppendLine("TORIS_CODE_S = '" & TorisCode.Trim & "' AND ")
            SQL.AppendLine("TORIF_CODE_S = '" & TorifCode.Trim & "' AND ")
            SQL.AppendLine("FURI_DATE_S = '" & NewFuriDate.Trim & "'")

            BRet = GCom.SetDynaset(SQL.ToString, REC)
            If BRet Then
                Do While REC.Read
                    Dim CNT As Integer = GCom.NzInt(REC.Item("COUNTER"))
                    If CNT = 0 Then
                        Return True
                    Else
                        Return False
                    End If
                Loop
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not REC Is Nothing Then REC.Close()
        End Try
    End Function


    '
    ' 機能　 ： 既存SCHMASTチェック
    '
    ' 引数　 ： ARG1 - 取引先配列位置
    '
    ' 戻り値 ： True = 未処理, False = 処理済
    '
    ' 備考　 ： SCHMASTにスケジュールが存在し、未処理であるか検索
    '
    Public Function CHECK_SELECT_SCHMAST(ByVal Index As Integer) As Boolean
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("SELECT TORIS_CODE_S")
            SQL.AppendLine(" FROM S_SCHMAST")
            SQL.AppendLine(" WHERE FSYORI_KBN_S = '" & TR(Index).FSYORI_KBN & "'")
            SQL.AppendLine(" AND TORIS_CODE_S = '" & TR(Index).TORIS_CODE & "'")
            SQL.AppendLine(" AND TORIF_CODE_S = '" & TR(Index).TORIF_CODE & "'")
            SQL.AppendLine(" AND FURI_DATE_S = '" & SCH.FURI_DATE & "'")
            SQL.AppendLine(" AND MOTIKOMI_SEQ_S = " & SCH.MOTIKOMI_SEQ)

            Dim BRet As Boolean = GCom.SetDynaset(SQL.ToString, REC)
            If BRet AndAlso REC.Read Then
                Return False
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return True
    End Function

    '
    ' 機能　 ： 月間スケジュール作成完了時、取引先マスタの KOUSIN_SIKIBETU_T に"2"を立てる(完了フラグ)
    '
    ' 引数　 ： ARG1 - 取引先情報配列位置
    '
    ' 戻り値 ： True = 未処理, False = 処理済
    '
    ' 備考　 ： なし
    '
    Public Function UPDATE_TORIMAST(ByVal Index As Integer) As Boolean
        Try
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("UPDATE S_TORIMAST")
            SQL.AppendLine(" SET KOUSIN_SIKIBETU_T = '2'")
            SQL.AppendLine(" WHERE FSYORI_KBN_T = '" & TR(Index).FSYORI_KBN & "'")
            SQL.AppendLine(" AND TORIS_CODE_T = '" & TR(Index).TORIS_CODE & "'")
            SQL.AppendLine(" AND TORIF_CODE_T = '" & TR(Index).TORIF_CODE & "'")

            Dim SQLCode As Integer
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode, False)

            Return (Ret = 1)

        Catch ex As Exception
            Throw
        End Try

        Return False

    End Function

    '
    ' 機能　 ： 休日表示領域の描画
    '
    ' 引数　 ： なし
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： なし
    '
    Public Sub Set_Kyuuzitu_Monitor_Area(ByVal ListView1 As ListView)
        Dim REC As OracleDataReader = Nothing
        Try
            With ListView1
                .Clear()
                .Columns.Add("和暦", 210, HorizontalAlignment.Center)
                .Columns.Add("西暦", 100, HorizontalAlignment.Center)
                .Columns.Add(" 名称", 175, HorizontalAlignment.Left)
                .Columns.Add("作成日", 95, HorizontalAlignment.Center)
                .Columns.Add("更新日", 95, HorizontalAlignment.Center)
            End With

            Dim SQL As StringBuilder = New StringBuilder()
            SQL.AppendLine("SELECT YASUMI_DATE_Y")
            SQL.AppendLine(", YASUMI_NAME_Y")
            SQL.AppendLine(", SAKUSEI_DATE_Y")
            SQL.AppendLine(", KOUSIN_DATE_Y")
            SQL.AppendLine(" FROM YASUMIMAST")
            SQL.AppendLine(" ORDER BY YASUMI_DATE_Y ASC")

            Dim Ret As Boolean = GCom.SetDynaset(SQL.ToString, REC)

            If Ret Then
                Dim ROW As Integer = 0

                Do While REC.Read

                    Dim Data(4) As String

                    Dim Temp As String = String.Format("{0:00000000}", GCom.NzDec(REC.Item("YASUMI_DATE_Y"), 0))
                    Dim onText(2) As Integer
                    onText(0) = CType(Temp.Substring(0, 4), Integer)
                    onText(1) = CType(Temp.Substring(4, 2), Integer)
                    onText(2) = CType(Temp.Substring(6), Integer)
                    Dim onDate As Date
                    Call GCom.ChangeDate(onText, Temp, 1, onDate)
                    If Not onDate = MenteCommon.clsCommon.BadResultDate Then

                        '和暦
                        Data(0) = Temp

                        '西暦
                        Data(1) = String.Format("{0:yyyy-MM-dd}", onDate)

                        '休日名称
                        Data(2) = " " & GCom.NzStr(REC.Item("YASUMI_NAME_Y")).Trim

                        '作成日
                        Data(3) = String.Format("{0:0000-00-00}", GCom.NzDec(REC.Item("SAKUSEI_DATE_Y"), 0))

                        '更新日
                        If GCom.NzDec(REC.Item("KOUSIN_DATE_Y")) > 0 Then
                            Data(4) = String.Format("{0:0000-00-00}", GCom.NzDec(REC.Item("KOUSIN_DATE_Y"), 0))
                        End If
                    End If

                    Dim LineColor As Color
                    If ROW Mod 2 = 0 Then
                        LineColor = Color.White
                    Else
                        LineColor = Color.WhiteSmoke
                    End If

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    ROW += 1
                Loop

                If ListView1.Items.Count = 0 Then
                    Dim MSG As String = MSG0003E
                    Call MessageBox.Show(MSG, GCom.GLog.Job1, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
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
    ' 機能　 ： スケジュールマスタの参照
    '
    ' 引数　 ： なし
    '
    ' 戻り値 ： カーソル有無
    '
    ' 備考　 ： なし
    '
    Public Function READ_SCHMAST() As Boolean
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("SELECT * FROM S_SCHMAST")
            SQL.AppendLine(" WHERE FSYORI_KBN_S = '" & TR(0).FSYORI_KBN & "'")
            SQL.AppendLine(" AND TORIS_CODE_S = '" & TR(0).TORIS_CODE & "'")
            SQL.AppendLine(" AND TORIF_CODE_S = '" & TR(0).TORIF_CODE & "'")
            SQL.AppendLine(" AND FURI_DATE_S = '" & SCH.FURI_DATE & "'")
            SQL.AppendLine(" AND MOTIKOMI_SEQ_S = " & SCH.MOTIKOMI_SEQ)

            Dim BRet As Boolean = GCom.SetDynaset(SQL.ToString, REC)
            If BRet AndAlso REC.Read Then

                Array.Clear(SCHMAST, 0, SCHMAST.Length)

                For Index As Integer = 0 To MaxColumn Step 1
                    Select Case ORATYPE(Index)
                        Case "NUMBER"
                            SCHMAST(Index) = GCom.NzDec(REC.Item(Index), "")
                        Case Else
                            SCHMAST(Index) = GCom.NzStr(REC.Item(Index))
                    End Select
                Next Index

                Return True
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

    '2017/12/05 saitou 広島信金(RSV2標準版) ADD 大規模構築対応 ---------------------------------------- START
    ''' <summary>
    ''' スケジュールマスタサブの参照
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function READ_SCHMAST_SUB() As Boolean
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("SELECT * FROM S_SCHMAST_SUB")
            SQL.AppendLine(" WHERE FSYORI_KBN_SSUB = '" & TR(0).FSYORI_KBN & "'")
            SQL.AppendLine(" AND TORIS_CODE_SSUB = '" & TR(0).TORIS_CODE & "'")
            SQL.AppendLine(" AND TORIF_CODE_SSUB = '" & TR(0).TORIF_CODE & "'")
            SQL.AppendLine(" AND FURI_DATE_SSUB = '" & SCH.FURI_DATE & "'")
            SQL.AppendLine(" AND MOTIKOMI_SEQ_SSUB = " & SCH.MOTIKOMI_SEQ)

            Dim BRet As Boolean = GCom.SetDynaset(SQL.ToString, REC)
            If BRet AndAlso REC.Read Then

                Array.Clear(SCHMAST_SUB, 0, SCHMAST_SUB.Length)

                For Index As Integer = 0 To MaxColumn_Sub Step 1
                    Select Case ORATYPE_SUB(Index)
                        Case "NUMBER"
                            SCHMAST_SUB(Index) = GCom.NzDec(REC.Item(Index), "")
                        Case Else
                            SCHMAST_SUB(Index) = GCom.NzStr(REC.Item(Index))
                    End Select
                Next Index

                Return True
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
    '2017/12/05 saitou 広島信金(RSV2標準版) ADD ------------------------------------------------------- END

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

    Public Function GetJobCount() As Integer
        Dim Cnt As Integer = 0
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("SELECT COUNT(*) FROM JOBMAST")
            SQL.AppendLine(" WHERE TOUROKU_DATE_J = '" & Now.ToString("yyyyMMdd") & "'")
            SQL.AppendLine(" AND STATUS_J IN ('0','1')")

            Dim BRet As Boolean = GCom.SetDynaset(SQL.ToString, REC)
            If BRet AndAlso REC.Read Then
                Cnt = REC.GetInt32(0)
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Cnt
    End Function

End Class
