Imports System
Imports System.Text
Imports CASTCommon

''' <summary>
''' 口座振替店別集計表　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class ClsTenbetushuukei

#Region "クラス定数"
    '引数
    Public TORI_CODE As String          '取引先コード
    Public FURI_DATE As String          '振替日
    Public HENKAN_FLG As String         '返還フラグ
    Public PRINTERNAME As String        '出力プリンタ
    Public INVOKE_KBN As String = ""    ' 呼び出し区分      '2012/06/30 標準版　WEB伝送対応

#End Region

#Region "クラス変数"

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    Private Structure strcIniInfo
        Dim KINKOCD As String
        Dim ZEIRITU As String
    End Structure
    Private IniInfo As strcIniInfo

    Private Structure strcSchInfo
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
        Dim ITAKU_NNAME As String
        Dim ITAKU_CODE As String
        Dim TKIN_NO As String
        Dim TKIN_NNAME As String
        Dim TSIT_NO As String
        Dim TSIT_NNAME As String
        Dim TORIMATOME_SIT As String
        Dim TORIMATOME_SIT_NNAME As String
        Dim FURI_DATE As String
        Dim FURI_CODE As String
        Dim KIGYO_CODE As String
        Dim KIHTESUU As Integer
        Dim SEIKYU_KBN As String
        Dim TESUUTYO_KBN As String

        '2013/04/24 saitou 標準版 WEB伝送対応 -------------------------------------------------->>>>
        Dim BAITAI_CODE As String
        Dim MULTI_KBN As String
        Dim ITAKU_KANRI_CODE As String
        '2013/04/24 saitou 標準版 WEB伝送対応 --------------------------------------------------<<<<

        Public Sub Init()
            TORIS_CODE = String.Empty
            TORIF_CODE = String.Empty
            ITAKU_NNAME = String.Empty
            ITAKU_CODE = String.Empty
            TKIN_NO = String.Empty
            TKIN_NNAME = String.Empty
            TSIT_NO = String.Empty
            TSIT_NNAME = String.Empty
            TORIMATOME_SIT = String.Empty
            TORIMATOME_SIT_NNAME = String.Empty
            FURI_DATE = String.Empty
            FURI_CODE = String.Empty
            KIGYO_CODE = String.Empty
            KIHTESUU = 0
            SEIKYU_KBN = String.Empty
            TESUUTYO_KBN = String.Empty

            '2013/04/24 saitou 標準版 WEB伝送対応 -------------------------------------------------->>>>
            BAITAI_CODE = String.Empty
            MULTI_KBN = String.Empty
            ITAKU_KANRI_CODE = String.Empty
            '2013/04/24 saitou 標準版 WEB伝送対応 --------------------------------------------------<<<<

        End Sub

        Friend Sub SetOraData(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_T")
            TORIF_CODE = oraReader.GetString("TORIF_CODE_T")
            ITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_T")
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
            TKIN_NO = oraReader.GetString("TKIN_NO_T")
            TKIN_NNAME = oraReader.GetString("TKIN_NNAME_N")
            TORIMATOME_SIT = oraReader.GetString("TORIMATOME_SIT_T")
            TORIMATOME_SIT_NNAME = oraReader.GetString("TORIMATOME_SIT_NNAME_N")
            FURI_DATE = oraReader.GetString("FURI_DATE_S")
            FURI_CODE = oraReader.GetString("FURI_CODE_T")
            KIGYO_CODE = oraReader.GetString("KIGYO_CODE_T")
            KIHTESUU = oraReader.GetInt("KIHTESUU_T")
            SEIKYU_KBN = oraReader.GetString("SEIKYU_KBN_T")
            TESUUTYO_KBN = oraReader.GetString("TESUUTYO_KBN_T")

            '2013/04/24 saitou 標準版 WEB伝送対応 -------------------------------------------------->>>>
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_T")
            MULTI_KBN = oraReader.GetString("MULTI_KBN_T")
            ITAKU_KANRI_CODE = oraReader.GetString("ITAKU_KANRI_CODE_T")
            '2013/04/24 saitou 標準版 WEB伝送対応 --------------------------------------------------<<<<

        End Sub
    End Structure
    Private Key As strcSchInfo

    Private lngTesuuKin As Long

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 口座振替店別集計表　メイン処理
    ''' </summary>
    ''' <returns>0 - 正常 , 0以外 - 異常</returns>
    ''' <remarks></remarks>
    Public Function Main() As Integer
        ' オラクル
        MainDB = New CASTCommon.MyOracle

        Dim bRet As Boolean
        Try
            ' 印刷処理
            bRet = Printshuukei()

        Catch ex As Exception
           MainLOG.Write("口座振替店別集計表", "失敗", ex.Message & ":" & ex.StackTrace)
            Return -1
        Finally
            MainDB.Close()
        End Try

        If bRet Then
            Return 0
        Else
            Return 2
        End If

    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 口座振替店別集計表を出力します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function Printshuukei() As Boolean
        Dim oraSchReader As New CASTCommon.MyOracleReader(MainDB)
        Dim oraTenReader As New CASTCommon.MyOracleReader(MainDB)
        Dim oraMeiReader As New CASTCommon.MyOracleReader(MainDB)

        Dim PrnTenbetu As New ClsPrnTenbetushuukei    ' 口座振替店別集計表
        Dim name As String = ""

        ' CSVを作成する
        name = PrnTenbetu.CreateCsvFile

        Try
            'INIファイル読み込み
            If GetIniInfo() = False Then
                Return False
            End If

            Dim strTorisCode As String = TORI_CODE.Substring(0, 10)
            Dim strTorifCode As String = TORI_CODE.Substring(10, 2)
            Dim strFuriDate As String = FURI_DATE

            '2012/06/30 標準版　WEB伝送対応
            Dim User_ID As String = ""  'ユーザＩＤ
            Dim WEB_PRINTERNAME As String = "" 'WEB伝送用プリンタ名
            Dim WEB_PRINT As String = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_PRINT")    'WEB伝送プリント区分(0:PDFのみ、1:PDFと紙）

            LW.ToriCode = TORI_CODE
            LW.FuriDate = FURI_DATE

            '対象のスケジュールが存在するかチェック
            If oraSchReader.DataReader(CreateGetSchmastSQL(strTorisCode, strTorifCode, strFuriDate)) = True Then
                While oraSchReader.EOF = False
                    Key.Init()
                    Call Key.SetOraData(oraSchReader)

                    ' 2012/06/30 標準版　WEB伝送対応------------------------------------->
                    If Key.BAITAI_CODE = "10" And HENKAN_FLG <> "0" And INVOKE_KBN = "1" Then   '返還時
                        WEB_PRINTERNAME = CASTCommon.GetFSKJIni("WEB_DEN", "PRINTER")  'PDFを作成するプリンタ名を設定
                        Dim Sql = New StringBuilder(128)
                        Dim OraWebReader As New CASTCommon.MyOracleReader(MainDB)
                        Sql.Append(" SELECT USER_ID_W ")
                        Sql.Append(" FROM WEB_RIREKIMAST ")
                        If Key.MULTI_KBN = "0" Then
                            Sql.Append(" WHERE TORIS_CODE_W = '" & Key.TORIS_CODE & "'")
                            Sql.Append(" AND TORIF_CODE_W  = '" & Key.TORIF_CODE & "'")
                        Else
                            Sql.Append(" WHERE ITAKU_KANRI_CODE_W = '" & Key.ITAKU_KANRI_CODE & "'")
                        End If
                        Sql.Append(" AND FURI_DATE_W = '" & FURI_DATE & "'")
                        Sql.Append(" AND FSYORI_KBN_W = '1'")

                        If OraWebReader.DataReader(Sql) Then
                            User_ID = OraWebReader.GetString("USER_ID_W")
                        Else
                            User_ID = ""
                        End If

                        OraWebReader.Close()

                    End If
                    ' 2012/06/30 標準版　WEB伝送対応-------------------------------------<

                    '自金庫支店分
                    If oraTenReader.DataReader(CreateGetJikinkoListSQL) = True Then
                        While oraTenReader.EOF = False
                            If oraMeiReader.DataReader(CreateGetMeimastSQL(oraTenReader.GetString("KIN_NO_N"), oraTenReader.GetString("SIT_NO_N"))) = True Then
                                '手数料計算
                                lngTesuuKin = CalcTesuuKin(oraMeiReader.GetInt("SYORI_KEN"), oraMeiReader.GetInt("FURI_KEN"))
                                'CSV出力
                                Call SetListCsvData(oraTenReader, oraMeiReader, PrnTenbetu)

                            Else
                                '異常終了(カウント系なので必ず取得できる)
                                MainLOG.Write(LW.ToriCode, LW.FuriDate, "レコード作成", "失敗", "件数金額カウント失敗")
                                Return False
                            End If

                            oraMeiReader.Close()

                            oraTenReader.NextRead()
                        End While
                    End If

                    oraTenReader.Close()

                    '支店不明分
                    If oraMeiReader.DataReader(CreateGetMeimastUnknown) = True Then
                        '手数料計算
                        lngTesuuKin = CalcTesuuKin(oraMeiReader.GetInt("SYORI_KEN"), oraMeiReader.GetInt("FURI_KEN"))
                        'CSV出力
                        Call SetListCsvData(oraMeiReader, PrnTenbetu, True)
                    End If

                    oraMeiReader.Close()

                    '他行分
                    If oraMeiReader.DataReader(CreateGetMeimastTakou) = True Then
                        While oraMeiReader.EOF = False
                            '手数料計算
                            lngTesuuKin = CalcTesuuKin(oraMeiReader.GetInt("SYORI_KEN"), oraMeiReader.GetInt("FURI_KEN"))
                            'CSV出力
                            Call SetListCsvData(oraMeiReader, PrnTenbetu, False)

                            oraMeiReader.NextRead()
                        End While
                    End If

                    oraSchReader.NextRead()
                End While


                '印刷
                'If PrnTenbetu.ReportExecute(PRINTERNAME) Then
                '    Return True
                'Else
                '    Return False
                'End If

                '2012/06/30 標準版　WEB伝送対応
                If User_ID <> "" And HENKAN_FLG <> "0" Then '返還時
                    If PrnTenbetu.ReportExecute(WEB_PRINTERNAME, User_ID, Key.ITAKU_CODE) = True Then

                    Else
                        Return False
                    End If

                    If WEB_PRINT = "1" Then '区分が１の場合、通常使うプリンタでも印刷する
                        If PrnTenbetu.ReportExecute(PRINTERNAME) Then
                            Return True
                        Else
                            Return False
                        End If
                    Else
                        Return True
                    End If

                Else
                    If PrnTenbetu.ReportExecute(PRINTERNAME) Then
                        Return True
                    Else
                        Return False
                    End If
                End If

            Else
                MainLOG.Write(LW.ToriCode, LW.FuriDate, "レコード作成", "失敗", "スケジュールなし")
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.ToriCode, LW.FuriDate, "レコード作成", "失敗", ex.Message)
            Return False
        Finally
            If Not oraSchReader Is Nothing Then
                oraSchReader.Close()
                oraSchReader = Nothing
            End If
            If Not oraTenReader Is Nothing Then
                oraTenReader.Close()
                oraTenReader = Nothing
            End If
            If Not oraMeiReader Is Nothing Then
                oraMeiReader.Close()
                oraMeiReader = Nothing
            End If
        End Try

    End Function

    ''' <summary>
    ''' 印刷対象のスケジュールを取得するSQLを作成します。
    ''' </summary>
    ''' <param name="strTorisCode">取引先主コード</param>
    ''' <param name="strTorifCode">取引先副コード</param>
    ''' <param name="strFuriDate">振替日</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetSchmastSQL(ByVal strTorisCode As String, _
                                         ByVal strTorifCode As String, _
                                         ByVal strFuriDate As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select ")
            .Append(" TORIS_CODE_T")
            .Append(",TORIF_CODE_T")
            .Append(",ITAKU_NNAME_T")
            .Append(",ITAKU_CODE_T")
            .Append(",TKIN_NO_T")
            .Append(",KIN_NNAME_N as TKIN_NNAME_N")
            .Append(",TORIMATOME_SIT_T")
            .Append(",SIT_NNAME_N as TORIMATOME_SIT_NNAME_N")
            .Append(",FURI_DATE_S")
            .Append(",FURI_CODE_T")
            .Append(",KIGYO_CODE_T")
            .Append(",KIHTESUU_T")
            .Append(",SEIKYU_KBN_T")
            .Append(",TESUUTYO_KBN_T")
            ' 2012/06/30 標準版　WEB伝送対応------------------------------------->
            .Append(", TORIMAST.BAITAI_CODE_T")      '媒体コード
            .Append(", TORIMAST.MULTI_KBN_T")        'マルチ区分
            .Append(", TORIMAST.ITAKU_KANRI_CODE_T") '代表委託者コード
            ' 2012/06/30 標準版　WEB伝送対応-------------------------------------<
            .Append(" from TORIMAST")
            .Append(" inner join SCHMAST")
            .Append(" on TORIS_CODE_T = TORIS_CODE_S")
            .Append(" and TORIF_CODE_T = TORIF_CODE_S")
            .Append(" left outer join TENMAST")
            .Append(" on TKIN_NO_T = KIN_NO_N")
            .Append(" and TORIMATOME_SIT_T = SIT_NO_N")
            .Append(" where TORIS_CODE_T = " & SQ(strTorisCode))
            .Append(" and TORIF_CODE_T = " & SQ(strTorifCode))
            .Append(" and FURI_DATE_S = " & SQ(strFuriDate))
            .Append(" order by TORIS_CODE_T, TORIF_CODE_T")
        End With

        Return SQL
    End Function

    ''' <summary>
    ''' 自金庫の支店を取得するするSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetJikinkoListSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select ")
            .Append(" KIN_NO_N")
            .Append(",SIT_NO_N")
            .Append(",KIN_NNAME_N")
            .Append(",SIT_NNAME_N")
            .Append(" from TENMAST")
            .Append(" where KIN_NO_N = " & SQ(IniInfo.KINKOCD))
            .Append(" order by SIT_NO_N ")
        End With

        Return SQL
    End Function

    ''' <summary>
    ''' 明細マスタから指定したスケジュールと金融機関・支店の振替件数・金額を取得するSQLを作成します。
    ''' </summary>
    ''' <param name="strKinNo">金融機関コード</param>
    ''' <param name="strSitNo">支店コード</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetMeimastSQL(ByVal strKinNo As String, _
                                         ByVal strSitNo As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select ")
            .Append(" count(FURIKIN_K) as SYORI_KEN")
            .Append(",sum(FURIKIN_K) as SYORI_KIN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 1, 0)) as FURI_KEN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, FURIKIN_K, 0)) as FURI_KIN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, 1)) as FUNOU_KEN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, FURIKIN_K)) as FUNOU_KIN")
            .Append(" from MEIMAST")
            .Append(" inner join TORIMAST")
            .Append(" on TORIS_CODE_K = TORIS_CODE_T")
            .Append(" and TORIF_CODE_K = TORIF_CODE_T")
            .Append(" where TORIS_CODE_K = " & SQ(Key.TORIS_CODE))
            .Append(" and TORIF_CODE_K = " & SQ(Key.TORIF_CODE))
            .Append(" and FURI_DATE_K = " & SQ(Key.FURI_DATE))
            .Append(" and KEIYAKU_KIN_K = " & SQ(strKinNo))
            .Append(" and KEIYAKU_SIT_K = " & SQ(strSitNo))
            .Append(" and DATA_KBN_K = decode(FMT_KBN_T, '02', '3', '2')")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 明細マスタから指定したスケジュールと金融機関の振替件数・金額を取得するSQLを作成します。
    ''' </summary>
    ''' <param name="strKinNo">金融機関コード</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetMeimastSQL(ByVal strKinNo As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select ")
            .Append(" count(FURIKIN_K) as SYORI_KEN")
            .Append(",sum(FURIKIN_K) as SYORI_KIN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 1, 0)) as FURI_KEN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, FURIKIN_K, 0)) as FURI_KIN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, 1)) as FUNOU_KEN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, FURIKIN_K)) as FUNOU_KIN")
            .Append(" from MEIMAST")
            .Append(" inner join TORIMAST")
            .Append(" on TORIS_CODE_K = TORIS_CODE_T")
            .Append(" and TORIF_CODE_K = TORIF_CODE_T")
            .Append(" where TORIS_CODE_K = " & SQ(Key.TORIS_CODE))
            .Append(" and TORIF_CODE_K = " & SQ(Key.TORIF_CODE))
            .Append(" and FURI_DATE_K = " & SQ(Key.FURI_DATE))
            .Append(" and KEIYAKU_KIN_K = " & SQ(strKinNo))
            .Append(" and DATA_KBN_K = decode(FMT_KBN_T, '02', '3', '2')")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 明細マスタから指定したスケジュールの不明支店の振替件数・金額を取得するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetMeimastUnknown() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select ")
            .Append(" count(FURIKIN_K) as SYORI_KEN")
            .Append(",sum(FURIKIN_K) as SYORI_KIN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 1, 0)) as FURI_KEN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, FURIKIN_K, 0)) as FURI_KIN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, 1)) as FUNOU_KEN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, FURIKIN_K)) as FUNOU_KIN")
            .Append(" from MEIMAST")
            .Append(" inner join TORIMAST")
            .Append(" on TORIS_CODE_K = TORIS_CODE_T")
            .Append(" and TORIF_CODE_K = TORIF_CODE_T")
            .Append(" where TORIS_CODE_K = " & SQ(Key.TORIS_CODE))
            .Append(" and TORIF_CODE_K = " & SQ(Key.TORIF_CODE))
            .Append(" and FURI_DATE_K = " & SQ(Key.FURI_DATE))
            .Append(" and KEIYAKU_KIN_K = " & SQ(IniInfo.KINKOCD))
            .Append(" and DATA_KBN_K = decode(FMT_KBN_T, '02', '3', '2')")
            .Append(" and not exists (")
            .Append("   select KIN_NO_N from TENMAST")
            .Append("   where KEIYAKU_KIN_K = KIN_NO_N")
            .Append("   and KEIYAKU_SIT_K = SIT_NO_N")
            .Append(" )")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 明細マスタから指定したスケジュールの他行分の振替件数・金額を取得するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetMeimastTakou() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select ")
            .Append(" KEIYAKU_KIN_K")
            .Append(",KIN_NNAME_N")
            .Append(",count(FURIKIN_K) as SYORI_KEN")
            .Append(",sum(FURIKIN_K) as SYORI_KIN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 1, 0)) as FURI_KEN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, FURIKIN_K, 0)) as FURI_KIN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, 1)) as FUNOU_KEN")
            .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, FURIKIN_K)) as FUNOU_KIN")
            .Append(" from MEIMAST")
            .Append(" inner join TORIMAST")
            .Append(" on TORIS_CODE_K = TORIS_CODE_T")
            .Append(" and TORIF_CODE_K = TORIF_CODE_T")
            .Append(" left outer join (")
            .Append("   select KIN_NO_N, KIN_NNAME_N from TENMAST group by KIN_NO_N, KIN_NNAME_N")
            .Append(" ) TENMAST")
            .Append(" on KEIYAKU_KIN_K = KIN_NO_N")
            .Append(" where TORIS_CODE_K = " & SQ(Key.TORIS_CODE))
            .Append(" and TORIF_CODE_K = " & SQ(Key.TORIF_CODE))
            .Append(" and FURI_DATE_K = " & SQ(Key.FURI_DATE))
            .Append(" and KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
            .Append(" and DATA_KBN_K = decode(FMT_KBN_T, '02', '3', '2')")
            .Append(" group by KEIYAKU_KIN_K, KIN_NNAME_N")
            .Append(" order by KEIYAKU_KIN_K")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 手数料を計算します。
    ''' </summary>
    ''' <param name="intSyoriKen">依頼件数</param>
    ''' <param name="intFuriKen">振替件数</param>
    ''' <returns>手数料</returns>
    ''' <remarks></remarks>
    Private Function CalcTesuuKin(ByVal intSyoriKen As Integer, _
                                  ByVal intFuriKen As Integer) As Long

        If HENKAN_FLG = "0" Then        '落込時
            Return 0
        Else
            If Key.SEIKYU_KBN = "0" Then
                '依頼件数分／基本手数料計算
                Return intSyoriKen * Key.KIHTESUU
            ElseIf Key.SEIKYU_KBN = "1" Then
                '振替件数分／基本手数料計算
                Return intFuriKen * Key.KIHTESUU
            Else
                '請求区分異常
                Return 0
            End If
        End If
    End Function

    ''' <summary>
    ''' CSVに項目を設定します。（自金庫支店分）
    ''' </summary>
    ''' <param name="oraTenReader"></param>
    ''' <param name="oraMeiReader"></param>
    ''' <param name="List"></param>
    ''' <remarks></remarks>
    Private Sub SetListCsvData(ByVal oraTenReader As CASTCommon.MyOracleReader, _
                               ByVal oraMeiReader As CASTCommon.MyOracleReader, _
                               ByRef List As ClsPrnTenbetushuukei)
        With List
            .OutputCsvData(Key.FURI_DATE, True)                             '振替日
            .OutputCsvData(mMatchingDate, True)                             '処理日
            .OutputCsvData(Key.TORIS_CODE, True)                            '取引先主コード
            .OutputCsvData(Key.TORIF_CODE, True)                            '取引先副コード
            .OutputCsvData(Key.ITAKU_NNAME, True)                           '委託者名漢字
            .OutputCsvData(Key.ITAKU_CODE, True)                            '委託者コード
            .OutputCsvData(Key.TKIN_NO, True)                               '取扱金融機関コード
            .OutputCsvData(Key.TKIN_NNAME, True)                            '取扱金融機関名
            .OutputCsvData(Key.TORIMATOME_SIT, True)                        'とりまとめ店コード
            .OutputCsvData(Key.TORIMATOME_SIT_NNAME, True)                  'とりまとめ店名漢字
            .OutputCsvData(oraTenReader.GetString("SIT_NO_N"), True)        '支店コード
            .OutputCsvData(oraTenReader.GetString("SIT_NNAME_N"), True)     '支店名
            .OutputCsvData(oraMeiReader.GetInt("SYORI_KEN"))                '振替依頼件数
            .OutputCsvData(oraMeiReader.GetInt64("SYORI_KIN"))              '振替依頼金額

            If HENKAN_FLG = "0" Then
                .OutputCsvData("0")                                         '振替件数
                .OutputCsvData("0")                                         '振替金額
                .OutputCsvData("0")                                         '不能件数
                .OutputCsvData("0")                                         '不能金額
            Else
                .OutputCsvData(oraMeiReader.GetInt("FURI_KEN"))             '振替件数
                .OutputCsvData(oraMeiReader.GetInt64("FURI_KIN"))           '振替金額
                .OutputCsvData(oraMeiReader.GetInt("FUNOU_KEN"))            '不能件数
                .OutputCsvData(oraMeiReader.GetInt64("FUNOU_KIN"))          '不能金額
            End If

            If Key.TESUUTYO_KBN = "2" Then
                .OutputCsvData("")                                          '手数料
            Else
                .OutputCsvData(lngTesuuKin)                                 '手数料
            End If

            .OutputCsvData(Key.FURI_CODE, True)                             '振替コード
            .OutputCsvData(Key.KIGYO_CODE, True, True)                      '企業コード

        End With
    End Sub

    ''' <summary>
    ''' CSVに項目を設定します。（支店不明分、他行分）
    ''' </summary>
    ''' <param name="oraMeiReader"></param>
    ''' <param name="List"></param>
    ''' <param name="bUnknownFlg"></param>
    ''' <remarks></remarks>
    Private Sub SetListCsvData(ByVal oraMeiReader As CASTCommon.MyOracleReader, _
                               ByRef List As ClsPrnTenbetushuukei, _
                               ByVal bUnknownFlg As Boolean)
        With List
            .OutputCsvData(Key.FURI_DATE, True)                             '振替日
            .OutputCsvData(mMatchingDate, True)                             '処理日
            .OutputCsvData(Key.TORIS_CODE, True)                            '取引先主コード
            .OutputCsvData(Key.TORIF_CODE, True)                            '取引先副コード
            .OutputCsvData(Key.ITAKU_NNAME, True)                           '委託者名漢字
            .OutputCsvData(Key.ITAKU_CODE, True)                            '委託者コード
            .OutputCsvData(Key.TKIN_NO, True)                               '取扱金融機関コード
            .OutputCsvData(Key.TKIN_NNAME, True)                            '取扱金融機関名
            .OutputCsvData(Key.TORIMATOME_SIT, True)                        'とりまとめ店コード
            .OutputCsvData(Key.TORIMATOME_SIT_NNAME, True)                  'とりまとめ店名漢字
            If bUnknownFlg = True Then
                '不明分
                .OutputCsvData("***", True)                                 '支店コード
                .OutputCsvData("その他", True)                              '支店名
            Else
                '他行分
                .OutputCsvData(oraMeiReader.GetString("KEIYAKU_KIN_K"), True)
                .OutputCsvData(oraMeiReader.GetString("KIN_NNAME_N"), True)
            End If

            .OutputCsvData(oraMeiReader.GetInt("SYORI_KEN"))                '振替依頼件数
            .OutputCsvData(oraMeiReader.GetInt64("SYORI_KIN"))              '振替依頼金額

            If HENKAN_FLG = "0" Then
                .OutputCsvData("0")                                         '振替件数
                .OutputCsvData("0")                                         '振替金額
                .OutputCsvData("0")                                         '不能件数
                .OutputCsvData("0")                                         '不能金額
            Else
                .OutputCsvData(oraMeiReader.GetInt("FURI_KEN"))             '振替件数
                .OutputCsvData(oraMeiReader.GetInt64("FURI_KIN"))           '振替金額
                .OutputCsvData(oraMeiReader.GetInt("FUNOU_KEN"))            '不能件数
                .OutputCsvData(oraMeiReader.GetInt64("FUNOU_KIN"))          '不能金額
            End If

            If Key.TESUUTYO_KBN = "2" Then
                .OutputCsvData("")                                          '手数料
            Else
                .OutputCsvData(lngTesuuKin)                                 '手数料
            End If

            .OutputCsvData(Key.FURI_CODE, True)                             '振替コード
            .OutputCsvData(Key.KIGYO_CODE, True, True)                      '企業コード

        End With

    End Sub

    ''' <summary>
    ''' INIファイルを読み込みます。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function GetIniInfo() As Boolean

        IniInfo.KINKOCD = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
        If IniInfo.KINKOCD.Equals("err") = True OrElse IniInfo.KINKOCD = String.Empty Then
            MainLOG.Write("設定ファイル取得", "失敗", "[COMMON]KINKOCD 設定なし")
            Return False
        End If

        IniInfo.ZEIRITU = CASTCommon.GetFSKJIni("COMMON", "ZEIRITU")
        If IniInfo.ZEIRITU.Equals("err") = True OrElse IniInfo.ZEIRITU = String.Empty Then
            MainLOG.Write("設定ファイル取得", "失敗", "[COMMON]ZEIRITU 設定なし")
            Return False
        End If

        Return True

    End Function

#End Region

End Class

#Region "過去の遺産"
'Option Strict On

'Imports System
'Imports System.IO
'Imports System.Text
'Imports System.Collections
'Imports CAstBatch
'Imports CASTCommon.ModPublic

'Public Class ClsTenbetushuukei

'    ' ログ処理クラス
'    'Private MainLOG As CASTCommon.BatchLOG

'    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
'    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

'    '引数
'    Public TORI_CODE As String          '取引先コード
'    Public FURI_DATE As String          '振替日
'    Public HENKAN_FLG As String         '返還フラグ
'    Public PRINTERNAME As String        '出力プリンタ


'    Dim strITAKU_NNAME As String
'    Dim strTORI_NNAME As String
'    Dim strTORI_KIN As String
'    Dim strTORI_KIN_NNAME As String
'    Dim strTORI_SIT_NO As String
'    Dim strTORIS_CODE As String = ""
'    Dim strTORIF_CODE As String = ""
'    Dim strITAKU_CODE As String = ""
'    Dim intSIT_NUMBER As Integer
'    Dim TesuuTyo_Kbn As String = "" '2010/01/25 追加

'    Dim strSIT_NO As New ArrayList
'    Dim strSIT_NAME As New ArrayList
'    Dim dblSIT_FURI_KEN As New ArrayList
'    Dim dblSIT_FURI_KIN As New ArrayList
'    Dim dblSIT_FUNOU_KEN As New ArrayList
'    Dim dblSIT_FUNOU_KIN As New ArrayList
'    Dim dblSIT_IRAI_KEN As New ArrayList
'    Dim dblSIT_IRAI_KIN As New ArrayList
'    Dim dblSIT_TESUU_KIN As New ArrayList

'    Dim gstrJIKINKO_NO As String

'    Dim strKIN_NNAME As String
'    Dim strSIT_NNAME As String
'    Dim strKIN_KNAME As String
'    Dim strSIT_KNAME As String

'    '支店不明をカウント
'    Public lngSIT_FUMEI_KEN As Decimal
'    Public dblSIT_FUMEI_KIN As Decimal
'    Public lngSIT_FUMEI_FURIKEN As Decimal
'    Public dblSIT_FUMEI_FURIKIN As Decimal
'    Public lngSIT_FUMEI_FUNOUKEN As Decimal
'    Public dblSIT_FUMEI_FUNOUKIN As Decimal

'    '他行をカウント
'    Public lngSIT_TAKOU_KEN As Decimal
'    Public dblSIT_TAKOU_KIN As Decimal
'    Public lngSIT_TAKOU_FURIKEN As Decimal
'    Public dblSIT_TAKOU_FURIKIN As Decimal
'    Public lngSIT_TAKOU_FUNOUKEN As Decimal
'    Public dblSIT_TAKOU_FUNOUKIN As Decimal

'    ' パブリックＤＢ
'    Public MainDB As CASTCommon.MyOracle

'    ' FSKJ.INI セクション名
'    Private ReadOnly AppTOUROKU As String = "REPORTS"

'    '==============================================================================
'    ' 機能   ： 口座振替店別集計表メイン処理
'    ' 引数   ： なし
'    ' 戻り値 ： 0 - 正常 0以外 - 異常
'    ' 備考   ：  
'    ' 作成　 ： 2009/09/12
'    ' 更新　 ： 
'    '==============================================================================
'    Function Main() As Integer
'        ' オラクル
'        MainDB = New CASTCommon.MyOracle

'        Dim bRet As Boolean
'        Try
'            'MainLOG = New CASTCommon.BatchLOG("KFJP019", "口座振替店別集計表")

'            ' 印刷処理
'            bRet = Printshuukei()

'        Catch ex As Exception
'            MainLOG.Write("口座振替店別集計表", "失敗", ex.Message & ":" & ex.StackTrace)

'            Return -1
'        Finally
'            MainDB.Close()
'        End Try

'        If bRet Then
'            Return 0
'        Else
'            Return 2
'        End If

'    End Function

'    '==============================================================================
'    ' 機能   ： 口座振替店別集計帳票出力処理
'    ' 引数   ： なし
'    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
'    ' 備考   ： 
'    ' 作成　 ： 2009/09/12
'    ' 更新　 ： 
'    '==============================================================================
'    Private Function Printshuukei() As Boolean
'        Dim SQL As New StringBuilder(128)
'        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
'        Dim PartReader As New CASTCommon.MyOracleReader(MainDB)
'        Dim TenReader As New CASTCommon.MyOracleReader(MainDB)
'        Dim CSV_LOOP As Integer = 0

'        Dim strSeikyuKbn As String = "0"    '手数料請求区分
'        Dim intKihonTesuu As Integer = 0    '基本手数料単価
'        Dim dblTesuuKin As Double = 0      '手数料

'        strTORIS_CODE = TORI_CODE.Substring(0, 10)
'        strTORIF_CODE = TORI_CODE.Substring(10, 2)

'        Dim PrnTenbetu As New ClsPrnTenbetushuukei    ' 口座振替店別集計表

'        Try
'            SQL = New StringBuilder(128)
'            SQL.Append("SELECT")
'            SQL.Append(" TORIMAST.FURI_CODE_T")
'            'SQL.Append(",TORIMAST.TORIS_CODE_T")
'            'SQL.Append(",TORIMAST.TORIF_CODE_T")
'            SQL.Append(",TORIMAST.ITAKU_CODE_T")
'            SQL.Append(",TORIMAST.ITAKU_NNAME_T")
'            '2010/01/25 手数料徴求区分を取得
'            SQL.Append(",TORIMAST.TESUUTYO_KBN_T")
'            '===============================
'            'SQL.Append(",MEIMAST.TORIS_CODE_K")
'            'SQL.Append(",MEIMAST.TORIF_CODE_K")
'            SQL.Append(",MEIMAST.ITAKU_KIN_K")
'            SQL.Append(",MEIMAST.ITAKU_SIT_K")
'            SQL.Append(",MEIMAST.KEIYAKU_KIN_K")
'            SQL.Append(",MEIMAST.KEIYAKU_SIT_K")
'            SQL.Append(",MEIMAST.FURIKIN_K")
'            SQL.Append(",TENMAST.KIN_NO_N")
'            SQL.Append(",TENMAST.SIT_NO_N")
'            SQL.Append(",TENMAST.KIN_NNAME_N")
'            'SQL.Append(",TENMAST.EDA_N")
'            SQL.Append(",TENMAST_KEI.KIN_NO_N")
'            SQL.Append(",TENMAST_KEI.SIT_NO_N")
'            SQL.Append(",TENMAST_KEI.SIT_NNAME_N")
'            'SQL.Append(",TENMAST_KEI.EDA_N")
'            SQL.Append(",TENMAST_TORI.SIT_NNAME_N TORI_SIT_NNAME_N")
'            'SQL.Append(",TENMAST_TORI.EDA_N")
'            SQL.Append(",TORIMAST.ITAKU_NNAME_T")
'            SQL.Append(",TORIMAST.TORIMATOME_SIT_T")
'            SQL.Append(",TORIMAST.TUKEKIN_NO_T")
'            SQL.Append(",TORIMAST.TUKESIT_NO_T")
'            SQL.Append(",TORIMAST.TUKEKAMOKU_T")
'            SQL.Append(",TORIMAST.TUKEKOUZA_T")
'            'SQL.Append(",TORIMAST.TUKEMEIGI_KNAME_T")
'            SQL.Append(",TORIMAST.SEIKYU_KBN_T")
'            SQL.Append(",TORIMAST.KIHTESUU_T")
'            ' 2010/09/14 TASK)saitou 振替/企業コード印字対応 -------------------->
'            SQL.Append(", TORIMAST.FURI_CODE_T")        '取引先マスタ.振替コード
'            SQL.Append(", TORIMAST.KIGYO_CODE_T")       '取引先マスタ.企業コード
'            ' 2010/09/14 TASK)saitou 振替/企業コード印字対応 --------------------<
'            SQL.Append(" FROM MEIMAST")
'            SQL.Append(",TENMAST")
'            SQL.Append(",TENMAST TENMAST_KEI")

'            'SQL.Append(",(SELECT * FROM TENMAST WHERE EDA_N = '01') TENMAST_TORI")      '金融機関異常対応
'            SQL.Append(",TENMAST TENMAST_TORI")

'            SQL.Append(",TORIMAST")
'            SQL.Append(" WHERE  '" & strTORIS_CODE & "' = MEIMAST.TORIS_CODE_K")
'            SQL.Append(" AND   '" & strTORIF_CODE & "' = MEIMAST.TORIF_CODE_K")
'            SQL.Append(" AND MEIMAST.TORIS_CODE_K = TORIMAST.TORIS_CODE_T")
'            SQL.Append(" AND MEIMAST.TORIF_CODE_K = TORIMAST.TORIF_CODE_T")
'            SQL.Append(" AND MEIMAST.ITAKU_KIN_K = TENMAST.KIN_NO_N")
'            SQL.Append(" AND MEIMAST.ITAKU_SIT_K = TENMAST.SIT_NO_N")
'            SQL.Append(" AND MEIMAST.KEIYAKU_KIN_K = TENMAST_KEI.KIN_NO_N")
'            SQL.Append(" AND MEIMAST.KEIYAKU_SIT_K = TENMAST_KEI.SIT_NO_N")
'            SQL.Append(" AND TORIMAST.TORIMATOME_SIT_T = TENMAST_TORI.SIT_NO_N(+)")     '金融機関異常対応
'            SQL.Append(" AND TORIMAST.TKIN_NO_T = TENMAST_TORI.KIN_NO_N(+)")
'            '2011/06/16 標準版修正 処理速度向上 ------------------START
'            SQL.Append(" AND MEIMAST.FURI_DATE_K = '" & FURI_DATE & "'")
'            '2011/06/16 標準版修正 処理速度向上 ------------------END
'            'SQL.Append(" AND TENMAST.EDA_N = '01'")                                     '枝番='01'を条件に追加。

'            'SQL.Append(" AND TENMAST_KEI.EDA_N = '01'")

'            SQL.Append("ORDER BY TENMAST.SIT_NO_N")

'            Dim aaaSQL As Boolean

'            aaaSQL = OraReader.DataReader(SQL)
'            If aaaSQL = False Then
'            End If


'            MainLOG.Write("パラメータ取得", "成功", CStr(aaaSQL))


'            strITAKU_CODE = OraReader.GetItem("ITAKU_CODE_T")
'            TesuuTyo_Kbn = OraReader.GetItem("TESUUTYO_KBN_T")
'            ' 2010/09/14 TASK)saitou 振替/企業コード印字対応 -------------------->
'            Dim strFuriCodeT As String = OraReader.GetItem("FURI_CODE_T")
'            Dim strKigyoCodeT As String = OraReader.GetItem("KIGYO_CODE_T")
'            ' 2010/09/14 TASK)saitou 振替/企業コード印字対応 --------------------<

'            '-----------------------------------
'            '金融機関マスタ（TENMAST）の検索
'            '-----------------------------------
'            intSIT_NUMBER = 0
'            gstrJIKINKO_NO = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

'            SQL = New StringBuilder(128)
'            SQL.Append(" SELECT TENMAST.KIN_NO_N")
'            SQL.Append(",TENMAST.SIT_NO_N")
'            SQL.Append(",TENMAST.SIT_NNAME_N")
'            SQL.Append(",TENMAST.KIN_NNAME_N")
'            'SQL.Append(",TENMAST.EDA_N")
'            SQL.Append(" FROM TENMAST")
'            SQL.Append(" WHERE TENMAST.KIN_NO_N = '" & gstrJIKINKO_NO & "'")

'            'SQL.Append(" AND TENMAST.EDA_N = '01'")


'            SQL.Append(" ORDER BY TENMAST.SIT_NO_N")

'            Dim aaSQL As Boolean

'            aaSQL = TenReader.DataReader(SQL)
'            If aaSQL = False Then
'            End If

'            strITAKU_NNAME = OraReader.GetItem("ITAKU_NNAME_T")     '取引先名
'            strTORI_KIN = OraReader.GetItem("KIN_NO_N")             '取扱金融機関コード
'            strTORI_KIN_NNAME = OraReader.GetItem("KIN_NNAME_N")    '取扱金融機関名漢字
'            strTORI_SIT_NO = OraReader.GetItem("TORIMATOME_SIT_T")  'とりまとめ店コード
'            strTORI_NNAME = OraReader.GetItem("TORI_SIT_NNAME_N")   'とりまとめ店名漢字
'            strSeikyuKbn = OraReader.GetItem("SEIKYU_KBN_T")            '手数料請求区分
'            intKihonTesuu = OraReader.GetInt("KIHTESUU_T")          '基本手数料単価

'            If aaSQL = True Then
'                While (TenReader.EOF = False)
'                    'intGYO = 1
'                    intSIT_NUMBER += 1
'                    strSIT_NO.Add(TenReader.GetItem("SIT_NO_N"))
'                    strSIT_NAME.Add(TenReader.GetItem("SIT_NNAME_N"))
'                    'dblSIT_FURI_KEN.Add(0)
'                    'dblSIT_FURI_KIN.Add(0)
'                    'dblSIT_FUNOU_KEN.Add(0)
'                    'dblSIT_FUNOU_KIN.Add(0)
'                    'dblSIT_IRAI_KEN.Add(0)
'                    'dblSIT_IRAI_KIN.Add(0)

'                    '--------------------------
'                    '依頼件数、依頼金額の検索
'                    '--------------------------
'                    SQL = New StringBuilder(128)
'                    SQL.Append("SELECT")
'                    SQL.Append(" COUNT(FURIKIN_K) FURIKENSUU")
'                    SQL.Append(",SUM(FURIKIN_K) FURIKINGAKU")
'                    SQL.Append(" FROM MEIMAST")
'                    SQL.Append(" WHERE TORIS_CODE_K = '" & strTORIS_CODE & "'")
'                    SQL.Append(" AND   TORIF_CODE_K = '" & strTORIF_CODE & "'")
'                    SQL.Append(" AND   FURI_DATE_K = '" & FURI_DATE & "'")
'                    SQL.Append(" AND   KEIYAKU_KIN_K = '" & TenReader.GetItem("KIN_NO_N") & "'")
'                    SQL.Append(" AND   KEIYAKU_SIT_K = '" & TenReader.GetItem("SIT_NO_N") & "'")

'                    Dim baSQL As Boolean

'                    baSQL = PartReader.DataReader(SQL)
'                    If baSQL = False Then
'                    End If

'                    If PartReader.GetItem("FURIKENSUU") = Nothing Then
'                        dblSIT_IRAI_KEN.Add(0)
'                        dblSIT_IRAI_KIN.Add(0)
'                    Else
'                        dblSIT_IRAI_KEN.Add(PartReader.GetInt64("FURIKENSUU"))
'                        dblSIT_IRAI_KIN.Add(PartReader.GetInt64("FURIKINGAKU"))
'                    End If
'                    PartReader.Close()

'                    '-------------------------
'                    '振替件数、振替金額の検索
'                    '-------------------------
'                    SQL = New StringBuilder(128)
'                    SQL.Append("SELECT")
'                    SQL.Append(" COUNT(FURIKIN_K) FURIKENSUU")
'                    SQL.Append(",SUM(FURIKIN_K) FURIKINGAKU")
'                    SQL.Append(" FROM MEIMAST")
'                    SQL.Append(" WHERE TORIS_CODE_K = '" & strTORIS_CODE & "'")
'                    SQL.Append(" AND   TORIF_CODE_K = '" & strTORIF_CODE & "'")
'                    SQL.Append(" AND   FURI_DATE_K = '" & FURI_DATE & "'")
'                    SQL.Append(" AND   KEIYAKU_KIN_K = '" & TenReader.GetItem("KIN_NO_N") & "'")
'                    SQL.Append(" AND   KEIYAKU_SIT_K = '" & TenReader.GetItem("SIT_NO_N") & "'")
'                    SQL.Append(" AND   FURIKETU_CODE_K = '0'")

'                    Dim bbSQL As Boolean
'                    bbSQL = PartReader.DataReader(SQL)
'                    If bbSQL = False Then
'                    End If

'                    If PartReader.GetItem("FURIKENSUU") = Nothing Then
'                        dblSIT_FURI_KEN.Add(0)
'                        dblSIT_FURI_KIN.Add(0)
'                    Else
'                        dblSIT_FURI_KEN.Add(PartReader.GetInt64("FURIKENSUU"))
'                        dblSIT_FURI_KIN.Add(PartReader.GetInt64("FURIKINGAKU"))
'                    End If
'                    PartReader.Close()

'                    '--------------------------
'                    '不能件数、不能金額の検索
'                    '--------------------------
'                    SQL = New StringBuilder(128)
'                    SQL.Append("SELECT")
'                    SQL.Append(" COUNT(FURIKIN_K) FURIKENSUU")
'                    SQL.Append(",SUM(FURIKIN_K) FURIKINGAKU")
'                    SQL.Append(" FROM MEIMAST")
'                    SQL.Append(" WHERE TORIS_CODE_K = '" & strTORIS_CODE & "'")
'                    SQL.Append(" AND   TORIF_CODE_K = '" & strTORIF_CODE & "'")
'                    SQL.Append(" AND   FURI_DATE_K = '" & FURI_DATE & "'")
'                    SQL.Append(" AND   KEIYAKU_KIN_K = '" & TenReader.GetItem("KIN_NO_N") & "' ")
'                    SQL.Append(" AND   KEIYAKU_SIT_K = '" & TenReader.GetItem("SIT_NO_N") & "' ")
'                    SQL.Append(" AND   FURIKETU_CODE_K <> '0'")

'                    Dim bcSQL As Boolean
'                    bcSQL = PartReader.DataReader(SQL)
'                    If bbSQL = False Then
'                    End If

'                    If PartReader.GetInt64("FURIKENSUU") = Nothing Then
'                        dblSIT_FUNOU_KEN.Add(0)
'                        dblSIT_FUNOU_KIN.Add(0)
'                    Else
'                        dblSIT_FUNOU_KEN.Add(PartReader.GetInt64("FURIKENSUU"))
'                        dblSIT_FUNOU_KIN.Add(PartReader.GetInt64("FURIKINGAKU"))
'                    End If
'                    PartReader.Close()

'                    '手数料計算
'                    If HENKAN_FLG = "0" Then    '落込時
'                        '計算しない
'                        dblTesuuKin = 0
'                    Else                        '返還時
'                        If strSeikyuKbn = "0" Then
'                            '   依頼件数分／基本手数料計算
'                            '   dblTesuuKin = CDbl(dblSIT_IRAI_KEN.Item(intSIT_NUMBER).ToString) * intKihonTesuu
'                            '
'                            'インデックス範囲外になるので -1    2009/09/18 kakiwaki
'                            dblTesuuKin = CDbl(dblSIT_IRAI_KEN.Item(intSIT_NUMBER - 1).ToString) * intKihonTesuu
'                        ElseIf strSeikyuKbn = "1" Then
'                            '   振替件数分／基本手数料計算
'                            '   dblTesuuKin = CDbl(dblSIT_FURI_KEN.Item(intSIT_NUMBER).ToString) * intKihonTesuu

'                            'インデックス範囲外になるので -1    2009/09/18 kakiwaki
'                            dblTesuuKin = CDbl(dblSIT_FURI_KEN.Item(intSIT_NUMBER - 1).ToString) * intKihonTesuu
'                        Else
'                            '請求区分異常
'                            dblTesuuKin = 0
'                        End If
'                    End If
'                    dblSIT_TESUU_KIN.Add(dblTesuuKin)


'                    TenReader.NextRead()
'                End While

'            Else
'            End If
'            TenReader.Close()

'            ''支店不明分カウント
'            ''SQL文で明細マスト→店マスト

'            lngSIT_FUMEI_KEN = 0
'            dblSIT_FUMEI_KIN = 0
'            lngSIT_FUMEI_FURIKEN = 0
'            dblSIT_FUMEI_FURIKIN = 0
'            lngSIT_FUMEI_FUNOUKEN = 0
'            dblSIT_FUMEI_FUNOUKIN = 0

'            SQL = New StringBuilder(128)
'            SQL.Append(" SELECT MEIMAST.KEIYAKU_KIN_K")
'            SQL.Append(",MEIMAST.FURIKIN_K")
'            SQL.Append(",MEIMAST.FURIKETU_CODE_K")
'            SQL.Append(",MEIMAST.KEIYAKU_SIT_K")
'            SQL.Append(",MEIMAST.FURI_DATE_K")
'            SQL.Append(",MEIMAST.TORIS_CODE_K")
'            SQL.Append(",MEIMAST.TORIF_CODE_K")
'            SQL.Append(" FROM MEIMAST")
'            SQL.Append(" WHERE TORIS_CODE_K = '" & strTORIS_CODE & "'")
'            SQL.Append(" AND TORIF_CODE_K = '" & strTORIF_CODE & "'")
'            SQL.Append(" AND FURI_DATE_K = '" & FURI_DATE & "'")
'            SQL.Append(" AND KEIYAKU_KIN_K <> '" & Space(4) & "'")  'ヘッダ／トレーラ／エンド以外
'            SQL.Append(" AND KEIYAKU_KIN_K = '" & gstrJIKINKO_NO & "'")

'            Dim ccSQL As Boolean
'            ccSQL = PartReader.DataReader(SQL)

'            '*** 2008/6/4  kaki            ***
'            '***異常支店あるなしフラグ追加 ***
'            Dim SIT_TEN_FLG As Boolean = False
'            '*** 2008/6/4  kaki            ***

'            Try
'                'If ccSQL = False Then
'                '    '■2009/03/11.Sakon　依頼件数０件考慮追加
'                '    LOG.Write("店別集計", "成功", "対象０件")
'                '    Return True
'                'End If


'                '2008/10/07 sakon　最後の自店舗が出力されない ***********
'                '   ループ処理を [   While 条件 ～ End While
'                '                 ⇒ Do ～ Loop While 条件   ] に変更
'                '********************************************************
'                Do Until PartReader.EOF
'                    'Select_TENMAST の引数を明細マスタの実データで検索する
'                    If fn_Select_TENMAST(PartReader.GetItem("KEIYAKU_KIN_K"), PartReader.GetItem("KEIYAKU_SIT_K"), strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
'                        lngSIT_FUMEI_KEN = lngSIT_FUMEI_KEN + 1
'                        dblSIT_FUMEI_KIN = dblSIT_FUMEI_KIN + PartReader.GetInt64("FURIKIN_K")
'                        SIT_TEN_FLG = True
'                        If PartReader.GetItem("FURIKETU_CODE_K") <> "0" Then
'                            lngSIT_FUMEI_FUNOUKEN += 1
'                            dblSIT_FUMEI_FUNOUKIN += PartReader.GetInt64("FURIKIN_K")
'                        Else
'                            lngSIT_FUMEI_FURIKEN += 1
'                            dblSIT_FUMEI_FURIKIN += PartReader.GetInt64("FURIKIN_K")
'                        End If
'                    End If
'                    PartReader.NextRead()
'                Loop
'                'Loop While (PartReader.EOF = False)

'                PartReader.Close()

'            Catch ex As Exception
'                PartReader.Close()
'                Return False
'            End Try

'            'If ccSQL = True Then
'            'intSIT_NUMBER += 1
'            'End If

'            '※「*** その他」の行分カウントアップする
'            intSIT_NUMBER += 1

'            strSIT_NO.Add("***")
'            strSIT_NAME.Add("その他")
'            dblSIT_IRAI_KEN.Add(lngSIT_FUMEI_KEN)
'            dblSIT_IRAI_KIN.Add(dblSIT_FUMEI_KIN)
'            dblSIT_FUNOU_KEN.Add(lngSIT_FUMEI_FUNOUKEN)
'            dblSIT_FUNOU_KIN.Add(dblSIT_FUMEI_FUNOUKIN)
'            dblSIT_FURI_KEN.Add(lngSIT_FUMEI_FURIKEN)
'            dblSIT_FURI_KIN.Add(dblSIT_FUMEI_FURIKIN)

'            '手数料計算
'            If HENKAN_FLG = "0" Then    '落込時
'                '計算しない
'                dblTesuuKin = 0
'            Else                        '返還時
'                If strSeikyuKbn = "0" Then
'                    '       '依頼件数分／基本手数料計算
'                    '       dblTesuuKin = CDbl(dblSIT_IRAI_KEN.Item(intSIT_NUMBER).ToString) * intKihonTesuu

'                    'インデックス範囲外になるので           2009/09/18 kakiwaki
'                    If ccSQL = True Then
'                        dblTesuuKin = CDbl(dblSIT_IRAI_KEN.Item(intSIT_NUMBER - 2).ToString) * intKihonTesuu
'                    Else
'                        dblTesuuKin = CDbl(dblSIT_IRAI_KEN.Item(intSIT_NUMBER - 1).ToString) * intKihonTesuu
'                    End If
'                ElseIf strSeikyuKbn = "1" Then
'                    '       '振替件数分／基本手数料計算
'                    '       dblTesuuKin = CDbl(dblSIT_FURI_KEN.Item(intSIT_NUMBER).ToString) * intKihonTesuu

'                    'インデックス範囲外になるので           2009/09/18 kakiwaki
'                    If ccSQL = True Then
'                        dblTesuuKin = CDbl(dblSIT_FURI_KEN.Item(intSIT_NUMBER - 2).ToString) * intKihonTesuu
'                    Else
'                        dblTesuuKin = CDbl(dblSIT_FURI_KEN.Item(intSIT_NUMBER - 1).ToString) * intKihonTesuu
'                    End If
'                Else
'                    '請求区分異常
'                    dblTesuuKin = 0
'                End If
'            End If
'            dblSIT_TESUU_KIN.Add(dblTesuuKin)

'            '他行カウント
'            lngSIT_TAKOU_KEN = 0
'            dblSIT_TAKOU_KIN = 0
'            lngSIT_TAKOU_FURIKEN = 0
'            dblSIT_TAKOU_FURIKIN = 0
'            lngSIT_TAKOU_FUNOUKEN = 0
'            dblSIT_TAKOU_FUNOUKIN = 0

'            SQL = New StringBuilder(128)
'            SQL.Append(" SELECT KEIYAKU_KIN_K")
'            SQL.Append(",SUM(FURIKIN_K) FURIKIN")
'            SQL.Append(",COUNT(FURIKIN_K) FURIKEN")
'            SQL.Append(",SUM(DECODE(FURIKETU_CODE_K,0,FURIKIN_K,0)) TAKOU_FURIKIN")
'            SQL.Append(",SUM(DECODE(FURIKETU_CODE_K,0,1,0)) TAKOU_FURIKEN")
'            SQL.Append(",SUM(DECODE(FURIKETU_CODE_K,0,0,FURIKIN_K)) TAKOU_FUNOUKIN")
'            SQL.Append(",SUM(DECODE(FURIKETU_CODE_K,0,0,1)) TAKOU_FUNOUKEN")
'            SQL.Append(" FROM MEIMAST")
'            SQL.Append(" WHERE TORIS_CODE_K = '" & strTORIS_CODE & "'")
'            SQL.Append(" AND TORIF_CODE_K = '" & strTORIF_CODE & "'")
'            SQL.Append(" AND FURI_DATE_K = '" & FURI_DATE & "'")
'            SQL.Append(" AND KEIYAKU_KIN_K <> '" & gstrJIKINKO_NO & "'")
'            SQL.Append(" GROUP BY KEIYAKU_KIN_K")
'            SQL.Append(" ORDER BY KEIYAKU_KIN_K")

'            Dim DDSQL As Boolean
'            DDSQL = PartReader.DataReader(SQL)
'            If DDSQL = False Then
'            End If


'            While (PartReader.EOF = False)
'                '金融機関コードが空白の場合はスルー
'                If PartReader.GetItem("KEIYAKU_KIN_K").Trim <> "" Then
'                    lngSIT_TAKOU_KEN = PartReader.GetInt64("FURIKEN")
'                    dblSIT_TAKOU_KIN = PartReader.GetInt64("FURIKIN")
'                    lngSIT_TAKOU_FURIKEN = PartReader.GetInt64("TAKOU_FURIKEN")
'                    dblSIT_TAKOU_FURIKIN = PartReader.GetInt64("TAKOU_FURIKIN")
'                    lngSIT_TAKOU_FUNOUKEN = PartReader.GetInt64("TAKOU_FUNOUKEN")
'                    dblSIT_TAKOU_FUNOUKIN = PartReader.GetInt64("TAKOU_FUNOUKIN")

'                    intSIT_NUMBER += 1

'                    SQL = New StringBuilder(128)
'                    SQL.Append(" SELECT TENMAST.KIN_NO_N")
'                    SQL.Append(",TENMAST.SIT_NO_N")
'                    SQL.Append(",TENMAST.SIT_NNAME_N")
'                    SQL.Append(",TENMAST.KIN_NNAME_N")
'                    'SQL.Append(",TENMAST.EDA_N")
'                    SQL.Append(" FROM TENMAST")
'                    SQL.Append(" WHERE TENMAST.KIN_NO_N = '" & PartReader.GetItem("KEIYAKU_KIN_K") & "'")
'                    'SQL.Append(" AND TENMAST.SIT_NO_N = '" & PartReader.GetItem("KEIYAKU_SIT_K") & "'")

'                    'SQL.Append(" AND TENMAST.EDA_N = '01'")

'                    SQL.Append(" ORDER BY TENMAST.SIT_NO_N")
'                    Dim SitReader As New CASTCommon.MyOracleReader(MainDB)
'                    If SitReader.DataReader(SQL) = True Then
'                        strSIT_NO.Add(SitReader.GetItem("KIN_NO_N"))
'                        strSIT_NAME.Add(SitReader.GetItem("KIN_NNAME_N"))
'                    Else
'                        strSIT_NO.Add(PartReader.GetItem("KEIYAKU_KIN_K"))
'                        strSIT_NAME.Add("")
'                    End If
'                    SitReader.Close()

'                    dblSIT_IRAI_KEN.Add(lngSIT_TAKOU_KEN)
'                    dblSIT_IRAI_KIN.Add(dblSIT_TAKOU_KIN)

'                    If HENKAN_FLG = "0" Then
'                        dblSIT_FURI_KEN.Add(0)
'                        dblSIT_FURI_KIN.Add(0)
'                        dblSIT_FUNOU_KEN.Add(0)
'                        dblSIT_FUNOU_KIN.Add(0)
'                        dblSIT_TESUU_KIN.Add(0)
'                    Else
'                        dblSIT_FURI_KEN.Add(lngSIT_TAKOU_FURIKEN)
'                        dblSIT_FURI_KIN.Add(dblSIT_TAKOU_FURIKIN)
'                        dblSIT_FUNOU_KEN.Add(lngSIT_TAKOU_FUNOUKEN)
'                        dblSIT_FUNOU_KIN.Add(dblSIT_TAKOU_FUNOUKIN)

'                        '手数料計算
'                        If HENKAN_FLG = "0" Then    '落込時
'                            '計算しない
'                            dblTesuuKin = 0
'                        Else                        '返還時
'                            If strSeikyuKbn = "0" Then
'                                '       '依頼件数分／基本手数料計算
'                                '       dblTesuuKin = CDbl(dblSIT_IRAI_KEN.Item(intSIT_NUMBER).ToString) * intKihonTesuu

'                                'インデックス範囲外になるので           2009/09/18 kakiwaki
'                                If ccSQL = True Then
'                                    dblTesuuKin = CDbl(dblSIT_IRAI_KEN.Item(intSIT_NUMBER - 2).ToString) * intKihonTesuu
'                                Else
'                                    dblTesuuKin = CDbl(dblSIT_IRAI_KEN.Item(intSIT_NUMBER - 1).ToString) * intKihonTesuu
'                                End If
'                            ElseIf strSeikyuKbn = "1" Then
'                                '       '振替件数分／基本手数料計算
'                                '       dblTesuuKin = CDbl(dblSIT_FURI_KEN.Item(intSIT_NUMBER).ToString) * intKihonTesuu

'                                'インデックス範囲外になるので           2009/09/18 kakiwaki
'                                If ccSQL = True Then
'                                    dblTesuuKin = CDbl(dblSIT_FURI_KEN.Item(intSIT_NUMBER - 2).ToString) * intKihonTesuu
'                                Else
'                                    dblTesuuKin = CDbl(dblSIT_FURI_KEN.Item(intSIT_NUMBER - 1).ToString) * intKihonTesuu
'                                End If
'                            Else
'                                '請求区分異常
'                                dblTesuuKin = 0
'                            End If
'                        End If
'                        dblSIT_TESUU_KIN.Add(dblTesuuKin)

'                    End If


'                End If

'                PartReader.NextRead()
'            End While

'            PartReader.Close()
'            OraReader.Close()

'            'strSIT_NO(intSIT_NUMBER + 2) = "***"
'            'strSIT_NAME(intSIT_NUMBER + 2) = "他行"
'            'dblSIT_IRAI_KEN(intSIT_NUMBER + 2) = lngSIT_TAKOU_KEN
'            'dblSIT_IRAI_KIN(intSIT_NUMBER + 2) = dblSIT_TAKOU_KIN
'            'dblSIT_FURI_KEN(intSIT_NUMBER + 2) = lngSIT_TAKOU_FURIKEN
'            'dblSIT_FURI_KIN(intSIT_NUMBER + 2) = dblSIT_TAKOU_FURIKIN
'            'dblSIT_FUNOU_KEN(intSIT_NUMBER + 2) = lngSIT_TAKOU_FUNOUKEN
'            'dblSIT_FUNOU_KIN(intSIT_NUMBER + 2) = dblSIT_TAKOU_FUNOUKIN

'            Dim name As String = ""

'            ' CSVを作成する
'            name = PrnTenbetu.CreateCsvFile

'            'If name = "" Then
'            'End If

'            Do While CSV_LOOP < intSIT_NUMBER

'                PrnTenbetu.OutputCsvData(FURI_DATE)                                             '振替日
'                PrnTenbetu.OutputCsvData(mMatchingDate)                                         '処理日
'                PrnTenbetu.OutputCsvData(strTORIS_CODE)                                         '取引先主コード
'                PrnTenbetu.OutputCsvData(strTORIF_CODE)                                         '取引先副コード
'                PrnTenbetu.OutputCsvData(strITAKU_NNAME, True)                                  '取引先名漢字
'                PrnTenbetu.OutputCsvData(strITAKU_CODE)                                         '委託者コード
'                PrnTenbetu.OutputCsvData(strTORI_KIN)                                           '取扱金融機関コード
'                PrnTenbetu.OutputCsvData(strTORI_KIN_NNAME, True)                               '取扱金融機関名
'                PrnTenbetu.OutputCsvData(strTORI_SIT_NO)                                        '取りまとめ店コード
'                PrnTenbetu.OutputCsvData(strTORI_NNAME, True)                                   '取りまとめ店名漢字
'                PrnTenbetu.OutputCsvData(strSIT_NO.Item(CSV_LOOP).ToString)                     '支店コード
'                PrnTenbetu.OutputCsvData(strSIT_NAME.Item(CSV_LOOP).ToString, True)             '支店名
'                PrnTenbetu.OutputCsvData(dblSIT_IRAI_KEN.Item(CSV_LOOP).ToString)               '振替依頼件数
'                PrnTenbetu.OutputCsvData(dblSIT_IRAI_KIN.Item(CSV_LOOP).ToString)               '振替依頼金額

'                If HENKAN_FLG = "0" Then
'                    PrnTenbetu.OutputCsvData("0")                                               '振替件数
'                    PrnTenbetu.OutputCsvData("0")                                               '振替金額
'                    PrnTenbetu.OutputCsvData("0")                                               '振替不能件数
'                    PrnTenbetu.OutputCsvData("0")                                               '振替不能金額
'                Else
'                    PrnTenbetu.OutputCsvData(dblSIT_FURI_KEN.Item(CSV_LOOP).ToString)           '振替件数
'                    PrnTenbetu.OutputCsvData(dblSIT_FURI_KIN.Item(CSV_LOOP).ToString)           '振替金額
'                    PrnTenbetu.OutputCsvData(dblSIT_FUNOU_KEN.Item(CSV_LOOP).ToString)          '振替不能件数
'                    PrnTenbetu.OutputCsvData(dblSIT_FUNOU_KIN.Item(CSV_LOOP).ToString)          '振替不能金額
'                End If

'                '2010/01/25 手数料表示判定追加
'                'PrnTenbetu.OutputCsvData(dblSIT_TESUU_KIN.Item(CSV_LOOP).ToString, False, True) '手数料
'                If TesuuTyo_Kbn = "2" Then
'                    ' 2010/09/14 TASK)saitou 振替/企業コード印字対応 -------------------->
'                    ' 手数料は最終項目でなくなったため改行を削除
'                    'PrnTenbetu.OutputCsvData("", False, True) '手数料
'                    PrnTenbetu.OutputCsvData("") '手数料
'                    ' 2010/09/14 TASK)saitou 振替/企業コード印字対応 --------------------<
'                Else
'                    ' 2010/09/14 TASK)saitou 振替/企業コード印字対応 -------------------->
'                    ' 手数料は最終項目でなくなったため改行を削除
'                    'PrnTenbetu.OutputCsvData(dblSIT_TESUU_KIN.Item(CSV_LOOP).ToString, False, True) '手数料
'                    PrnTenbetu.OutputCsvData(dblSIT_TESUU_KIN.Item(CSV_LOOP).ToString) '手数料
'                    ' 2010/09/14 TASK)saitou 振替/企業コード印字対応 --------------------<
'                End If

'                ' 2010/09/14 TASK)saitou 振替/企業コード印字対応 -------------------->
'                PrnTenbetu.OutputCsvData(strFuriCodeT)                  '振替コード
'                PrnTenbetu.OutputCsvData(strKigyoCodeT, False, True)    '企業コード
'                ' 2010/09/14 TASK)saitou 振替/企業コード印字対応 --------------------<

'                CSV_LOOP += 1

'                'Loop While CSV_LOOP < intSIT_NUMBER - 1
'            Loop

'            If PrnTenbetu.ReportExecute(PRINTERNAME) Then
'                Return True
'            Else
'                Return False
'            End If

'        Catch ex As Exception
'            MainLOG.Write("対象データ設定", "失敗", ex.Message)
'        End Try

'    End Function
'    Function fn_Select_TENMAST(ByVal KIN_NO As String, ByVal SIT_NO As String, ByRef KIN_NNAME As String, ByRef SIT_NNAME As String, ByRef KIN_KNAME As String, ByRef SIT_KNAME As String) As Boolean
'        '=====================================================================================
'        'NAME           :fn_Select_TENMAST
'        'Parameter      :KIN_NO－金融機関コード／SIT_NO－支店コード／KIN_NNAME－金融機関漢字名
'        '               :SIT_NNAME－支店漢字名／KIN_KNAME－金融機関カナ名／SIT_KNAME－支店カナ名
'        'Description    :金融機関マスタ検索
'        'Return         :True=OK(検索ヒット),False=NG（検索失敗）
'        'Create         :2009/09/12
'        'Update         :
'        '=====================================================================================
'        Dim PartReader As New CASTCommon.MyOracleReader(MainDB)
'        Dim SQL As New StringBuilder(128)
'        fn_Select_TENMAST = False

'        Try
'            SQL = New StringBuilder(128)
'            SQL.Append(" SELECT TENMAST.KIN_NO_N")
'            SQL.Append(",TENMAST.SIT_NO_N")
'            'SQL.Append(",TENMAST.EDA_N")
'            SQL.Append(",KIN_NNAME_N")
'            SQL.Append(",SIT_NNAME_N")
'            SQL.Append(",KIN_KNAME_N")
'            SQL.Append(",SIT_KNAME_N")
'            SQL.Append(" FROM TENMAST")

'            SQL.Append(" WHERE TENMAST.KIN_NO_N = '" & KIN_NO & "'")
'            SQL.Append(" AND  TENMAST.SIT_NO_N = '" & SIT_NO & "'")

'            'SQL.Append(" AND EDA_N = '01'")


'            KIN_NNAME = ""
'            SIT_NNAME = ""
'            KIN_KNAME = ""
'            SIT_KNAME = ""

'            '読込のみ
'            If PartReader.DataReader(SQL) = False Then
'                fn_Select_TENMAST = False
'                Exit Function
'            Else
'                KIN_NNAME = PartReader.GetItem("KIN_NNAME_N")
'                SIT_NNAME = PartReader.GetItem("SIT_NNAME_N")
'                KIN_KNAME = PartReader.GetItem("KIN_KNAME_N")
'                SIT_KNAME = PartReader.GetItem("SIT_KNAME_N")
'                fn_Select_TENMAST = True
'            End If
'        Catch ex As Exception
'            MainLOG.Write("例外発生", "失敗", ex.Message)
'            Return False

'        Finally
'            PartReader.Close()
'        End Try

'        Return True
'    End Function

'End Class


#End Region
