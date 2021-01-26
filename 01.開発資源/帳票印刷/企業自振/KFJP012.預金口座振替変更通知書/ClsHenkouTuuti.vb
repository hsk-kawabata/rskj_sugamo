Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic
' 預金口座振替結果変更通知書印刷クラス
Public Class ClsHenkouTuuti

    ' 引数
    Public FURI_DATE As String                                                  ' 振替日
    Private MainDB As CASTCommon.MyOracle                                       ' パブリックＤＢ
    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻
    Dim Count As Integer = 0

    '自金庫コード
    Dim Jikinko As String
    '自金庫名
    Dim JikinkoNm As String
    '部署名
    Dim Busyo As String
    'NHK コード
    Dim NHKCODE As String

    '
    ' 機能   ：  預金口座振替変更通知書印刷処理
    '
    ' 引数   ： なし
    '
    ' 戻り値 ： 0 - 正常 0以外 - 異常
    '
    ' 備考   ： 
    '
    Public Function PrintHenkouTuuti() As Integer

        Dim SQL As New StringBuilder(128)    ' SQL
        MainDB = New CASTCommon.MyOracle ' オラクル
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim OraReader2 As New CASTCommon.MyOracleReader(MainDB) '2010/10/04.Sakon　追加
        Dim PrinTuutisyo As New KFJP012    ' 口座振替変更通知書
        Try
            'INIファイル取得
            '自金庫コード
            Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If Jikinko.ToUpper = "ERR" Or Jikinko = "" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                Return -300
            End If
            '自金庫名
            JikinkoNm = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
            If JikinkoNm.ToUpper = "ERR" Or JikinkoNm = "" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫名 分類:PRINT 項目:KINKONAME")
                Return -300
            End If
            '部署名
            Busyo = CASTCommon.GetFSKJIni("PRINT", "KINKOBUSYO")
            If Busyo.ToUpper = "ERR" Then '空白は許可する
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:部署名 分類:PRINT 項目:KINKOBUSYO")
                Return -300
            End If
            'NHK コード
            NHKCODE = CASTCommon.GetFSKJIni("COMMON", "NHKCD")
            If NHKCODE.ToUpper = "ERR" Then '空白は許可する
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:NHKコード 分類:COMMON 項目:NHKCD")
                Return -300
            End If

            SQL = New StringBuilder(128)


            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '2010/02/08.Sakon　店舗統廃合考慮－各金融機関・店舗名取得は別途行う

            SQL.Append("SELECT")
            SQL.Append(" TORIMAST.ITAKU_CODE_T      ITAKU_CODE_TORIMAST ")      '会社コード  
            SQL.Append(",TORIMAST.ITAKU_NNAME_T     ITAKU_NNAME_TORIMAST ")     '会社名   
            SQL.Append(",TORIMAST.TSIT_NO_T         TSIT_NO ")                  '取りまとめ店番
            SQL.Append(",TORIMAST.FMT_KBN_T         FMT_KBN_TORIMAST ")         'フォーマット区分 
            SQL.Append(",MEIMAST.JYUYOUKA_NO_K      JYUYOUKA_NO_MEIMAST ")      '契約者番号    
            SQL.Append(",MEIMAST.KEIYAKU_KNAME_K    KEIYAKU_KNAME_MEIMAST ")    '契約者名     
            SQL.Append(",MEIMAST.KEIYAKU_KNAME_K    YOKINSYA_NAME ")            '預金者名    
            SQL.Append(",MEIMAST.KEIYAKU_SIT_K      KEIYAKU_SIT_MEIMAST ")      '変更前店番     
            SQL.Append(",MEIMAST.KEIYAKU_KAMOKU_K   KEIYAKU_KAMOKU_MEIMAST ")   '変更前種目     
            SQL.Append(",MEIMAST.KEIYAKU_KOUZA_K    KEIYAKU_KOUZA_MEIMAST ")    '変更前口座番号    
            SQL.Append(",MEIMAST.TEISEI_SIT_K       TEISEI_SIT_MEIMAST ")       '変更後店番    
            SQL.Append(",MEIMAST.TEISEI_KAMOKU_K    TEISEI_KAMOKU_MEIMAST ")    '変更後種目     
            SQL.Append(",MEIMAST.TEISEI_KOUZA_K     TEISEI_KOUZA_MEIMAST")      '変更後口座番号   
            SQL.Append(",MEIMAST.TORIS_CODE_K       TORIS_CODE_MEIMAST ")       '取引先主コード   
            SQL.Append(",MEIMAST.TORIF_CODE_K       TORIF_CODE_MEIMAST ")       '取引先副コード
            SQL.Append(",MEIMAST.FURI_DATA_K        IRAI_DATA_MEIMAST ")        '依頼データ
            SQL.Append(" FROM ")
            SQL.Append(" MEIMAST ")
            SQL.Append(",TORIMAST ")
            SQL.Append(" WHERE ")
            SQL.Append(" MEIMAST.TORIS_CODE_K = TORIMAST.TORIS_CODE_T ")
            SQL.Append(" AND MEIMAST.TORIF_CODE_K = TORIMAST.TORIF_CODE_T ")
            SQL.Append(" AND MEIMAST.FURI_DATE_K = '" & FURI_DATE & "' ")
            '国税の場合を考慮
            SQL.Append(" AND MEIMAST.DATA_KBN_K = DECODE(FMT_KBN_T,'02','3','2')")
            SQL.Append(" AND (NVL(TRIM(MEIMAST.TEISEI_SIT_K),'000') <> '000' ")
            SQL.Append("      OR NVL(TRIM(MEIMAST.TEISEI_KAMOKU_K),'00') <> '00' ")
            SQL.Append("      OR NVL(TRIM(MEIMAST.TEISEI_KOUZA_K),'0000000') <> '0000000') ")
            SQL.Append(" ORDER BY TORIMAST.TORIMATOME_SIT_T")
            SQL.Append(",MEIMAST.TORIS_CODE_K")
            SQL.Append(",MEIMAST.TORIF_CODE_K")
            SQL.Append(",MEIMAST.RECORD_NO_K")
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            'SQL.Append("SELECT")
            'SQL.Append(" TORIMAST.ITAKU_CODE_T   ITAKU_CODE_TORIMAST ")     ' 会社コード  
            'SQL.Append(",TORIMAST.ITAKU_NNAME_T   ITAKU_NNAME_TORIMAST ")   ' 会社名   
            'SQL.Append(",TORIMATOME.SIT_NNAME_N   SIT_NNAME_TORIMATOME ")   ' 取りまとめ店     
            'SQL.Append(",MEIMAST.JYUYOUKA_NO_K   JYUYOUKA_NO_MEIMAST ")     ' 契約者番号    
            'SQL.Append(",MEIMAST.KEIYAKU_KNAME_K   KEIYAKU_KNAME_MEIMAST ") ' 契約者名     
            'SQL.Append(",MEIMAST.KEIYAKU_KNAME_K   YOKINSYA_NAME ")         ' 預金者名    
            'SQL.Append(",KEIYAKU.SIT_KNAME_N   SIT_KNAME_KEIYAKU ")         ' 変更前店名    
            'SQL.Append(",MEIMAST.KEIYAKU_SIT_K   KEIYAKU_SIT_MEIMAST ")     ' 変更前店番     
            'SQL.Append(",MEIMAST.KEIYAKU_KAMOKU_K   KEIYAKU_KAMOKU_MEIMAST ") ' 変更前種目     
            'SQL.Append(",MEIMAST.KEIYAKU_KOUZA_K   KEIYAKU_KOUZA_MEIMAST ") ' 変更前口座番号    
            'SQL.Append(",TEISEI.SIT_KNAME_N   SIT_KNAME_TEISEI ")           ' 変更後店名   
            'SQL.Append(",MEIMAST.TEISEI_SIT_K   TEISEI_SIT_MEIMAST ")       ' 変更後店番    
            'SQL.Append(",MEIMAST.TEISEI_KAMOKU_K   TEISEI_KAMOKU_MEIMAST ") ' 変更後種目     
            'SQL.Append(",MEIMAST.TEISEI_KOUZA_K    TEISEI_KOUZA_MEIMAST")   ' 変更後口座番号   
            'SQL.Append(",MEIMAST.TORIS_CODE_K   TORIS_CODE_MEIMAST ")       ' 取引先主コード   
            'SQL.Append(",MEIMAST.TORIF_CODE_K   TORIF_CODE_MEIMAST ")       ' 取引先副コード
            'SQL.Append(",TORIMAST.FMT_KBN_T   FMT_KBN_TORIMAST ")           'フォーマット区分 
            'SQL.Append(",MEIMAST.FURI_DATA_K   IRAI_DATA_MEIMAST ")         '依頼データ
            'SQL.Append(" FROM ")
            'SQL.Append(" MEIMAST ")
            'SQL.Append(",TORIMAST ")
            'SQL.Append(",TENMAST TORIMATOME ")
            'SQL.Append(",TENMAST KEIYAKU ")
            'SQL.Append(",TENMAST TEISEI ")
            'SQL.Append(" WHERE ")
            'SQL.Append(" MEIMAST.TORIS_CODE_K = TORIMAST.TORIS_CODE_T ")
            'SQL.Append(" AND MEIMAST.TORIF_CODE_K = TORIMAST.TORIF_CODE_T ")
            'SQL.Append(" AND MEIMAST.FURI_DATE_K = '" & FURI_DATE & "' ")
            ''国税の場合を考慮
            'SQL.Append(" AND MEIMAST.DATA_KBN_K = DECODE(FMT_KBN_T,'02','3','2')")
            'SQL.Append(" AND (MEIMAST.TEISEI_SIT_K <> '000' ")
            'SQL.Append("      OR MEIMAST.TEISEI_KAMOKU_K <> '00' ")
            'SQL.Append("      OR MEIMAST.TEISEI_KOUZA_K <> '0000000') ")
            'SQL.Append(" AND '" & Jikinko & "' = TORIMATOME.KIN_NO_N ")
            'SQL.Append(" AND TORIMAST.TORIMATOME_SIT_T = TORIMATOME.SIT_NO_N ")
            'SQL.Append(" AND '" & Jikinko & "' = KEIYAKU.KIN_NO_N ")
            'SQL.Append(" AND MEIMAST.KEIYAKU_SIT_K = KEIYAKU.SIT_NO_N ")
            'SQL.Append(" AND '" & Jikinko & "' = TEISEI.KIN_NO_N ")
            'SQL.Append(" AND MEIMAST.TEISEI_SIT_K = TEISEI.SIT_NO_N(+) ")
            'SQL.Append(" ORDER BY TORIMAST.TORIMATOME_SIT_T")
            'SQL.Append(",MEIMAST.TORIS_CODE_K")
            'SQL.Append(",MEIMAST.TORIF_CODE_K")
            'SQL.Append(",MEIMAST.RECORD_NO_K")

        Catch ex As Exception
            MainLOG.Write("", "エラー", ex.Message)
        End Try

        Dim name As String = ""

        If OraReader.DataReader(SQL) = True Then
            If name = "" Then
                ' ＣＳＶを作成する
                name = PrinTuutisyo.CreateCsvFile
            End If
            Try
                Do
                    '処理日
                    PrinTuutisyo.OutputCsvData(mMatchingDate)
                    '自金庫コード
                    PrinTuutisyo.OutputCsvData(Jikinko)
                    '自金庫名
                    PrinTuutisyo.OutputCsvData(JikinkoNm)
                    '部署名
                    PrinTuutisyo.OutputCsvData(Busyo)
                    '会社コード	
                    PrinTuutisyo.OutputCsvData(OraReader.GetString("ITAKU_CODE_TORIMAST"))
                    '会社名	
                    PrinTuutisyo.OutputCsvData(OraReader.GetString("ITAKU_NNAME_TORIMAST"))
                    '取りまとめ店	
                    '2010/10/04.Sakon　支店名は別途取得 +++++++++++++++++++++++++++++++++++++++++++++++++++
                    PrinTuutisyo.OutputCsvData(GetTenName(OraReader.GetString("TSIT_NO"), OraReader2))
                    'PrinTuutisyo.OutputCsvData(OraReader.GetString("SIT_NNAME_TORIMATOME"))
                    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    '区分
                    PrinTuutisyo.OutputCsvData("ヘンコウ")
                    '契約者番号	
                    PrinTuutisyo.OutputCsvData(OraReader.GetString("JYUYOUKA_NO_MEIMAST"))
                    Dim IRAI_DATA As String = OraReader.GetString("IRAI_DATA_MEIMAST", False)

                    Select Case OraReader.GetString("FMT_KBN_TORIMAST")
                        Case "01", "06" '地公体
                            '取得した依頼データから、義務者名を取得
                            Dim KEIYAKU_KNAME As String = IRAI_DATA.Substring(198, 30)

                            '契約者名1	
                            PrinTuutisyo.OutputCsvData(KEIYAKU_KNAME.Substring(0, 15))
                            '契約者名2	
                            PrinTuutisyo.OutputCsvData(KEIYAKU_KNAME.Substring(15))
                        Case Else
                            'NHK専用契約者名印字
                            If OraReader.GetString("ITAKU_CODE_TORIMAST").Equals(NHKCODE) Then
                                Dim KEIYAKU_KNAME As String = IRAI_DATA.Substring(65, 15).Trim
                                '契約者名1	
                                PrinTuutisyo.OutputCsvData(KEIYAKU_KNAME)
                                '契約者名2	
                                PrinTuutisyo.OutputCsvData("")
                            Else
                                '契約者名1	
                                PrinTuutisyo.OutputCsvData("")
                                '契約者名2	
                                PrinTuutisyo.OutputCsvData("")
                            End If
                    End Select

                    ' NHK専用預金者名印字
                    If OraReader.GetString("ITAKU_CODE_TORIMAST").Equals(NHKCODE) Then
                        Dim YOKINSYA_NAME As String = IRAI_DATA.Substring(50, 15).Trim
                        '預金者名1	
                        PrinTuutisyo.OutputCsvData(YOKINSYA_NAME)
                        '預金者名2	
                        PrinTuutisyo.OutputCsvData("")
                    Else
                        '預金者名1	
                        PrinTuutisyo.OutputCsvData(OraReader.GetString("YOKINSYA_NAME"))
                        '預金者名2	
                        PrinTuutisyo.OutputCsvData("")
                    End If

                    '変更前店番	
                    PrinTuutisyo.OutputCsvData(OraReader.GetString("KEIYAKU_SIT_MEIMAST"))
                    '変更前店名
                    '2010/10/04.Sakon　支店名は別途取得 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    PrinTuutisyo.OutputCsvData(GetTenName(OraReader.GetString("KEIYAKU_SIT_MEIMAST"), OraReader2))
                    'PrinTuutisyo.OutputCsvData(OraReader.GetString("SIT_KNAME_KEIYAKU"))
                    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    '変更前種目	
                    PrinTuutisyo.OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("KEIYAKU_KAMOKU_MEIMAST")))
                    '変更前口座番号	
                    PrinTuutisyo.OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_MEIMAST"))

                    If OraReader.GetString("TEISEI_SIT_MEIMAST").Equals("000") Then
                        '変更前店番
                        PrinTuutisyo.OutputCsvData(OraReader.GetString("KEIYAKU_SIT_MEIMAST"))
                        '変更前店名
                        '2010/10/04.Sakon　支店名は別途取得 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        PrinTuutisyo.OutputCsvData(GetTenName(OraReader.GetString("KEIYAKU_SIT_MEIMAST"), OraReader2))
                        'PrinTuutisyo.OutputCsvData(OraReader.GetString("SIT_KNAME_KEIYAKU"))
                        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    Else
                        '変更後店番
                        PrinTuutisyo.OutputCsvData(OraReader.GetString("TEISEI_SIT_MEIMAST"))
                        '変更後店名
                        '2010/10/04.Sakon　支店名は別途取得 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        PrinTuutisyo.OutputCsvData(GetTenName(OraReader.GetString("TEISEI_SIT_MEIMAST"), OraReader2))
                        'PrinTuutisyo.OutputCsvData(OraReader.GetString("SIT_KNAME_TEISEI"))
                        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    End If

                    '2011/06/16 標準版修正 店舗のみ変更の場合を考慮 ------------------START
                    '変更後種目
                    '変換後科目・・・00の場合は""何も印字しない。  
                    If OraReader.GetString("TEISEI_KAMOKU_MEIMAST").Equals("00") Then
                        PrinTuutisyo.OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("KEIYAKU_KAMOKU_MEIMAST")))
                    Else
                        PrinTuutisyo.OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("TEISEI_KAMOKU_MEIMAST")))
                    End If

                    '変更後口座番号	
                    If OraReader.GetString("TEISEI_KOUZA_MEIMAST").Equals("0000000") Then
                        PrinTuutisyo.OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_MEIMAST"))
                    Else
                        PrinTuutisyo.OutputCsvData(OraReader.GetString("TEISEI_KOUZA_MEIMAST"))
                    End If

                    ''変更後種目
                    ''変換後科目・・・00の場合は""何も印字しない。  
                    'If OraReader.GetString("TEISEI_KAMOKU_MEIMAST").Equals("00") Then
                    '    PrinTuutisyo.OutputCsvData("")
                    'Else
                    '    PrinTuutisyo.OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("TEISEI_KAMOKU_MEIMAST")))
                    'End If

                    ''変更後口座番号	
                    'PrinTuutisyo.OutputCsvData(OraReader.GetString("TEISEI_KOUZA_MEIMAST"))
                    '2011/06/16 標準版修正 店舗のみ変更の場合を考慮 ------------------END
                    '該当データ有無	
                    PrinTuutisyo.OutputCsvData("")
                    '取引先主副	
                    PrinTuutisyo.OutputCsvData(OraReader.GetString("TORIS_CODE_MEIMAST") & OraReader.GetString("TORIF_CODE_MEIMAST"), False, True)
                    Count += 1
                    OraReader.NextRead()
                Loop Until OraReader.EOF
                OraReader.Close()
            Catch ex As Exception
                MainLOG.Write("データ検索", "失敗", ex.Message)
            End Try
        Else
            '念のため残す ===============================================
            'If name = "" Then
            '    ' ＣＳＶを作成する
            '    name = PrinTuutisyo.CreateCsvFile
            'End If
            'PrinTuutisyo.OutputCsvData("")    '会社コード
            'PrinTuutisyo.OutputCsvData("")    '会社名
            'PrinTuutisyo.OutputCsvData("")    '取りまとめ店
            'PrinTuutisyo.OutputCsvData("")    '区分
            'PrinTuutisyo.OutputCsvData("")    '契約者番号
            ''契約者名、預金者名を二段に分けて印字
            'PrinTuutisyo.OutputCsvData("")    '契約者名1
            'PrinTuutisyo.OutputCsvData("")    '契約者名2
            'PrinTuutisyo.OutputCsvData("")    '預金者名1
            'PrinTuutisyo.OutputCsvData("")    '預金者名2
            'PrinTuutisyo.OutputCsvData("")    '変更前店番
            'PrinTuutisyo.OutputCsvData("")    '変更前店名
            'PrinTuutisyo.OutputCsvData("")    '変更前種目
            'PrinTuutisyo.OutputCsvData("")    '変更前口座番号
            'PrinTuutisyo.OutputCsvData("")    '変更後店番
            'PrinTuutisyo.OutputCsvData("")    '変更後店名
            'PrinTuutisyo.OutputCsvData("")    '変更後種目
            'PrinTuutisyo.OutputCsvData("")    '変更後口座番号
            'PrinTuutisyo.OutputCsvData("該当データなし")    '該当データ有無
            'PrinTuutisyo.OutputCsvData("", False, True)    '取引先主副
            '==============================================================
            MainLOG.Write("印刷", "成功", "印刷対象0件")
            Return -1
        End If
        ' 印刷
        If PrinTuutisyo.ReportExecute() = True Then
            MainLOG.Write("印刷", "成功", Count & "件印刷")
            Return 0
        Else
            MainLOG.Write("印刷", "失敗", PrinTuutisyo.ReportMessage)
            Return -999
        End If
        Return 4
    End Function

    '========================================================================================
    ' 機　能：店舗名取得
    ' 引　数：支店コード
    ' 戻り値：店舗名
    ' 備　考：
    ' 作　成：2010/10/04
    ' 更　新：
    '========================================================================================
    Private Function GetTenName(ByVal pstrSitNo As String, ByVal OraReader2 As CASTCommon.MyOracleReader) As String
        Dim SQL As New StringBuilder(128)    ' SQL

        SQL = New StringBuilder(128)

        SQL.Append("SELECT")
        SQL.Append(" SIT_NNAME_N")
        SQL.Append(" FROM TENMAST")
        SQL.Append(" WHERE KIN_NO_N = '" & Jikinko & "'")
        SQL.Append(" AND SIT_NO_N = '" & pstrSitNo.Trim & "'")

        If OraReader2.DataReader(SQL) = True Then
            Return OraReader2.GetString("SIT_NNAME_N")
        Else
            Return ""
        End If

    End Function

End Class
