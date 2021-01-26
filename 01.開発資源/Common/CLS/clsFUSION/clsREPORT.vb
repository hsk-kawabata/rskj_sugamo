Option Strict Off '旧VB互換用

Imports System.Windows.Forms
Imports System.Drawing.Printing
Imports System.Drawing
Imports System.IO

Public Class clsREPORT
#Region "宣言"
    Public clsFusion As New clsFUSION.clsMain()
    Public frmREPORT As New clsFUSION.frmREPORT()

    Public gdbcCONNECT As New OracleClient.OracleConnection   'CONNECTION
    Public gdbrREADER As OracleClient.OracleDataReader 'READER
    Public gdbCOMMAND As OracleClient.OracleCommand   'COMMAND関数
    Public gdbTRANS As OracleClient.OracleTransaction 'TRANSACTION

    Private dblRECORD_COUNT As Double
    Private intFILE_SEQ As Integer
    Private intF_REC_LENGTH As Integer
    Private strDATA_KBN As String
    Private dblALL_KEN As Double
    Private dblALL_KIN As Double
    Private intGYO As Integer
    Private intPAGE As Integer
    Private strFURI_DATA As String
    Private strTUUBAN As String
    Private strTORI_CODE As String
    Private strTORIS_CODE As String
    Private strTORIF_CODE As String
    Private strF_FURI_DATE As String
    Private strITAKU_CODE As String
    Private strNS_KBN As String
    Private strITAKU_NNAME As String
    Private strMULTI_KBN As String
    Private strFURIKETU As String
    Private strWRK_FILE As String
    Private strCODE_KBN As String
    Private strF_ITAKU_CODE As String
    Private strTKIN_NO As String
    Private strTSIT_NO As String
    Private strITAKU_KIN_NNAME As String
    Private strITAKU_SIT_NNAME As String
    Private strITAKU_KIN_KNAME As String
    Private strITAKU_SIT_KNAME As String
    Private strKIN_NNAME As String
    Private strSIT_NNAME As String
    Private strKIN_KNAME As String
    Private strSIT_KNAME As String
    Private dblRECORD_SEQ As Double
    'ファイルから取得した値
    Private strF_KEIYAKU_KIN As String
    Private strF_KEIYAKU_SIT As String
    Private strF_KEIYAKU_NO As String
    Private strF_KEIYAKU_KNAME As String
    Private strF_KEIYAKU_KAMOKU As String
    Private strF_KEIYAKU_KOUZA As String
    Private strF_FURIKIN As String
    Private strF_JYUYOKA_NO As String
    Private strF_SINKI_NO As String
    Private strF_IRAI_KEN As String
    Private strF_IRAI_KIN As String

    Private intMOJI_SIZE As Integer = 5


    Private WithEvents pd As New System.Drawing.Printing.PrintDocument()
    Private streamToPrint As StreamReader
    Private ev As PrintPageEventArgs
    Private sender As Object
    Private ev1 As System.Drawing.Printing.PrintEventArgs
    Private PrintDocument As New PrintDocument()
    '印字用
    Private strP_KEIYAKU_KIN(50) As String
    Private strP_KEIYAKU_SIT(50) As String
    Private strP_KEIYAKU_KAMOKU(50) As String
    Private strP_KEIYAKU_KOUZA(50) As String
    Private strP_KEIYAKU_KNAME(50) As String
    Private strP_FURI_KIN(50) As String

    Private dblRECORD_COUNT_WRK As Double
    Private strUKETUKE_KBN As String

    Private strMAE_KEIYAKU_KIN As String
    Private strMAE_KEIYAKU_SIT As String
    Private dblSIT_KEN As Double
    Private dblSIT_KIN As Double
    Private strSORT_FILE As String

    Private intMULTI_FLG As Integer
    Private intKAISUU_FLG As Integer
    '口座・店番読替用
    Private strYOMIKAE_KIN_NO As String
    Private strYOMIKAE_SIT_NO As String
    Private strYOMIKAE_KOUZA As String
    Private strYOMIKAE_SIT_NO2 As String
    Private strYOMIKAE_KOUZA2 As String
    Private intYOMIKAE_FLG As Integer

    '蒲郡信金向け　帳票右下印字用　2007/07/27
    Public strKINKO_NAME As String
    Public intTYOHYO_KBN As Integer '0:企業自振　1：総合振込

#End Region
#Region "共通関数"
    Public Function fn_LOG_WRITE(ByVal astrUSER As String, ByVal astrTORI_CODE As String, ByVal astrFURI_DATE As String, ByVal strSYORI As String, ByVal astrJOB As String, ByVal astrKEKKA As String, ByVal astrERR As String) As Boolean
        '============================================================================
        'NAME           :fn_LOG_WRITE
        'Parameter      :astrUSER：ユーザ名／astrTORI_CODE：取引先コード／astrFURI_DATE：振替日
        '               :strSYORI：処理／astrJOB：処理内容／astrKEKKA：結果／astrERR：エラー内容
        'Description    :ログファイル書き出し
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/07/20
        'Update         :
        '============================================================================
        Dim strFile_Name As String
        strFile_Name = gstrLOG_OPENDIR & "TOUROKU" & Format(System.DateTime.Today, "yyyyMMdd") & ".LOG"

        Dim lngLINECOUNT As Long
        lngLINECOUNT = 0
        intPAGE = 0
        Dim intLOGFILE_NO As Integer
        intLOGFILE_NO = FreeFile()

        If Dir$(strFile_Name) = "" Then
            FileOpen(intLOGFILE_NO, strFile_Name, OpenMode.Output)
        Else
            FileOpen(intLOGFILE_NO, strFile_Name, OpenMode.Input)
            'ファイルの行数を取得
            Do Until EOF(intLOGFILE_NO)
                LineInput(intLOGFILE_NO)
                lngLINECOUNT += 1
            Loop
            FileClose(intLOGFILE_NO)
            FileOpen(intLOGFILE_NO, strFile_Name, OpenMode.Append)
        End If
        If astrUSER = Nothing Then
            astrUSER = ""
        End If
        If strTUUBAN = Nothing Then
            strTUUBAN = ""
        End If
        WriteLine(intLOGFILE_NO, lngLINECOUNT + 1, Format(System.DateTime.Now, "HHmmss"), strTUUBAN.PadLeft(3, "0"), astrUSER, astrTORI_CODE, astrFURI_DATE, strSYORI, astrJOB, astrKEKKA, astrERR)

        FileClose(intLOGFILE_NO)

    End Function
    Public Sub fn_UKETUKE_MEISAI_HEDDA()
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_HEDDA
        'Parameter      :
        'Description    :受付明細表ヘッダー部印刷
        'Return         :
        'Create         :2004/08/03
        'Update         :
        '============================================================================
        ''PrintDocumentオブジェクトの作成
        'Dim pd As New System.Drawing.Printing.PrintDocument()
        pd.PrinterSettings.Copies = 1
        'PrintPageイベントハンドラの追加
        AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_HEDDAPage
        'ページを横に設定
        pd.DefaultPageSettings.Landscape = True

    End Sub
    Private Sub pd_UKETUKE_MEISAI_HEDDAPage(ByVal sender As Object, ByVal ev As PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_PrintHEDDPage
        'Parameter      :
        'Description    :受付明細表ヘッダー部印刷詳細
        'Return         :
        'Create         :2004/08/03
        'Update         :
        '============================================================================
        Dim printingPosition As Integer
        Dim printFont As Font
        'Dim pd1 As New PrintDocument()


        '印刷する文字列と位置を設定する
        printingPosition = 0
        '印刷に使うフォントを指定する
        printFont = New Font("ＭＳ Ｐ明朝", 12)
        ev.Graphics.DrawString("＜ 受 付 明 細 表 ＞", printFont, Brushes.Black, 500, 50)

        printFont = New Font("ＭＳ Ｐ明朝", 10.5)
        ev.Graphics.DrawString("取　引　先　：" & strTORIS_CODE & "-" & strTORIF_CODE & "  " & strITAKU_NNAME, printFont, Brushes.Black, 50, 70)
        ev.Graphics.DrawString("取扱金融機関：" & strTKIN_NO & " " & strITAKU_KIN_NNAME & " " & strTSIT_NO & " " & strITAKU_SIT_NNAME, printFont, Brushes.Black, 50, 85)
        ev.Graphics.DrawString("処理日", printFont, Brushes.Black, 800, 50)
        ev.Graphics.DrawString("振替日", printFont, Brushes.Black, 800, 70)
        ev.Graphics.DrawString(Format(System.DateTime.Today, "yyyy" & "年" & "MM" & "月" & "dd" & "日"), printFont, Brushes.Black, 870, 50)
        ev.Graphics.DrawString(strF_FURI_DATE.Substring(0, 4) & "年" & strF_FURI_DATE.Substring(4, 2) & "月" & strF_FURI_DATE.Substring(6, 2) & "日", printFont, Brushes.Black, 870, 70)
        Dim strCODE_KBN_NAME As String = ""
        If strCODE_KBN = "0" Then
            strCODE_KBN_NAME = "JIS"
        ElseIf strCODE_KBN = "1" Then
            strCODE_KBN_NAME = "EBCDIC"
        ElseIf strCODE_KBN = "2" Then
            strCODE_KBN_NAME = "JIS改"
        End If
        ev.Graphics.DrawString("持込コード：" & strCODE_KBN_NAME, printFont, Brushes.Black, 870, 100)

        ev.Graphics.DrawString("ページ", printFont, Brushes.Black, 1030, 50)
        ev.Graphics.DrawString(intPAGE, printFont, Brushes.Black, 1010 - ((intPAGE.ToString.Length - 1)) * intMOJI_SIZE, 50)



        'Dim X_ITI() As Integer = {70, 150, 280, 400, 600, 670, 780, 1050} '見出し位置
        Dim X_ITI() As Integer = {70, 130, 280, 400, 560, 595, 665, 940, 1020} '見出し位置
        Dim X_MEI() As String = {"ｼｰｹﾝｽ", "ｴﾗｰﾒｯｾｰｼﾞ", "金融機関", "支店", "科目", "口座番号", "受取人", "金額", "備考"} '見出し
        Dim i As Integer
        For i = 0 To 8
            ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 130)
        Next

        '蒲郡信金向け 帳票番号印字 2007/07/27
        If intTYOHYO_KBN = 0 Then '企業自振用
            ev.Graphics.DrawString(strKINKO_NAME & Space(5) & "J-K013" & Space(5) & "保存1年", printFont, Brushes.Black, 830, 770)
        Else '総合振込用
            ev.Graphics.DrawString(strKINKO_NAME & Space(5) & "J-F006" & Space(5) & "保存1年", printFont, Brushes.Black, 830, 770)
        End If

        ev.HasMorePages = True
    End Sub
    Public Function fn_DATA_REC_CHK() As String
        '============================================================================
        'NAME           :fn_DATA_REC_CHK
        'Parameter      :
        'Description    :データレコード項目チェックプログラム
        '               :金融機関コード、科目、口座番号、金額
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/08/28
        'Update         :
        '============================================================================
        fn_DATA_REC_CHK = ""
        Dim intIJYO_FLG As Integer
        intIJYO_FLG = 0
        '金融機関コード数値チェック
        If clsFusion.fn_CHECK_NUM(strF_KEIYAKU_KIN) = False Or strF_KEIYAKU_KIN = "0000" Or strF_KEIYAKU_KIN = "9999" Then
            intIJYO_FLG = 1
            fn_DATA_REC_CHK = "店番異常"
        ElseIf clsFusion.fn_CHECK_NUM(strF_KEIYAKU_SIT) = False Or strF_KEIYAKU_SIT = "000" Or strF_KEIYAKU_SIT = "999" Then
            'ゆうちょ銀行の場合は"000"と"999"を異常としない
            If Not (strF_KEIYAKU_KIN = "9900" And (strF_KEIYAKU_SIT = "000" Or strF_KEIYAKU_SIT = "999")) Then
                fn_DATA_REC_CHK = "店番異常"
            End If
        End If
        '科目コードチェック
        If clsFusion.fn_CHECK_NUM(strF_KEIYAKU_KAMOKU) = False Or strF_KEIYAKU_KAMOKU = "0" Then
            fn_DATA_REC_CHK = "科目異常"
        Else
        End If
        '口座番号チェック
        If clsFusion.fn_CHECK_NUM(strF_KEIYAKU_KOUZA) = False Or strF_KEIYAKU_KOUZA = "0000000" Or strF_KEIYAKU_KOUZA = "9999999" Then
            fn_DATA_REC_CHK = "口番異常"
        End If

        '金融機関コード存在チェック
        If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
            fn_DATA_REC_CHK = "店番異常"
        End If
        '金額チェック
        If clsFusion.fn_CHECK_NUM(strF_FURIKIN.Trim) = False Then
            fn_DATA_REC_CHK = "金額異常"
        End If

    End Function
    Function fn_KOUZA_YOMIKAE(ByVal astrSIT_NO As String, ByVal astrKAMOKU As String, ByVal astrKOUZA As String, ByRef astrNEW_SIT_NO As String, ByRef astrNEW_KOUZA As String) As Boolean
        '=====================================================================================
        'NAME           :fn_KOUZA_YOMIKAE
        'Parameter      :astrKIN_NO：金融機関コード／astrSIT_NO：支店コード／astrKAMOKU：科目
        '               :astrKOUZA：口座番号
        'Description    :KDBMASTから口座読み替えを行う
        'Return         :読み替え後の支店コード、口座番号、True=OK(読み替え済み),False=NG（未読み替え）
        'Create         :2004/07/30
        'Update         :
        '=====================================================================================
        fn_KOUZA_YOMIKAE = False
        Try
start:
            gstrSSQL = "SELECT * FROM KDBMAST WHERE "
            gstrSSQL = gstrSSQL & "OLD_TSIT_NO_D = '" & Val(astrSIT_NO) & "'"
            gstrSSQL = gstrSSQL & " AND KAMOKU_D = '" & Val(astrKAMOKU) & "'"
            gstrSSQL = gstrSSQL & " AND OLD_KOUZA_D = '" & Val(astrKOUZA) & "'"

            gdbCOMMAND = New OracleClient.OracleCommand
            gdbCOMMAND.CommandText = gstrSSQL
            gdbCOMMAND.Connection = gdbcCONNECT

            gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ
            Dim COUNT As Integer
            COUNT = 0
            While (gdbrREADER.Read)
                COUNT += 1
                astrNEW_SIT_NO = Format(gdbrREADER.Item("TSIT_NO_D"), "000")
                astrNEW_KOUZA = Format(gdbrREADER.Item("KOUZA_D"), "0000000")
                fn_KOUZA_YOMIKAE = True
            End While
            If COUNT = 0 Then
                astrNEW_SIT_NO = astrSIT_NO.PadLeft(3, "0")
                astrNEW_KOUZA = astrKOUZA.PadLeft(7, "0")
            End If
        Catch ex As Exception
            If Err.Number = 5 Then
                '-----------------------------------------
                '１秒停止
                '-----------------------------------------
                Dim Start As Double
                Start = CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss"))           ' 中断の開始時刻を設定します。
                Do While CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss")) < Start + 1
                    Application.DoEvents()
                Loop
                Err.Clear()
                GoTo start
            End If
            If Err.Number <> 0 And Err.Number <> 5 Then
                Exit Function
            End If

        End Try
    End Function
    Function fn_TENPO_YOMIKAE(ByVal astrKIN_NO As Object, ByVal astrSIT_NO As Object, ByRef astrNEW_KIN_NO As Object, ByRef astrNEW_SIT_NO As Object) As Boolean
        '=====================================================================================
        'NAME           :fn_TENPO_YOMIKAE
        'Parameter      :astrKIN_NO：金融機関コード／astrSIT_NO：支店コード／
        '               :astrNEW_KIN_NO：読み替え後金融機関コード／astrNEW_SIT_NO：読み替え後支店コード
        'Description    :YOMIKAEMASTから店舗読み替えを行う
        'Return         :読み替え後の金融機関コード、支店コード、True=OK(読み替え済み),False=NG（未読み替え）
        'Create         :2004/07/30
        'Update         :
        '=====================================================================================
        fn_TENPO_YOMIKAE = False
        Try
start:
            gstrSSQL = "SELECT * FROM SITENYOMIKAE WHERE "
            gstrSSQL = gstrSSQL & "OLD_KIN_NO_S = '" & astrKIN_NO & "' AND "
            gstrSSQL = gstrSSQL & "OLD_SIT_NO_S = '" & astrSIT_NO & "'"

            gdbCOMMAND = New OracleClient.OracleCommand
            gdbCOMMAND.CommandText = gstrSSQL
            gdbCOMMAND.Connection = gdbcCONNECT

            gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ
            Dim COUNT As Integer
            COUNT = 0
            While (gdbrREADER.Read)
                COUNT += 1
                astrNEW_KIN_NO = gdbrREADER.Item("NEW_KIN_NO_S")
                astrNEW_SIT_NO = gdbrREADER.Item("NEW_SIT_NO_S")
                fn_TENPO_YOMIKAE = True
            End While
            If COUNT = 0 Then
                astrNEW_KIN_NO = astrKIN_NO
                astrNEW_SIT_NO = astrSIT_NO
            End If
        Catch ex As Exception
            If Err.Number = 5 Then
                '-----------------------------------------
                '１秒停止
                '-----------------------------------------
                Dim Start As Double
                Start = CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss"))           ' 中断の開始時刻を設定します。
                Do While CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss")) < Start + 1
                    Application.DoEvents()
                Loop
                Err.Clear()
                GoTo start
            End If
            If Err.Number <> 0 And Err.Number <> 5 Then
                Exit Function
            End If

        End Try

    End Function
    Function fn_Select_TENMAST(ByVal KIN_NO As String, ByVal SIT_NO As String, ByRef KIN_NNAME As String, ByRef SIT_NNAME As String, ByRef KIN_KNAME As String, ByRef SIT_KNAME As String) As Boolean
        '=====================================================================================
        'NAME           :fn_Select_TENMAST
        'Parameter      :KIN_NO：金融機関コード／SIT_NO：支店コード／KIN_NNAME:金融機関漢字名
        '               :SIT_NNAME:支店漢字名／KIN_KNAME：金融機関カナ名／SIT_KNAME：支店カナ名
        'Description    :金融機関マスタ検索
        'Return         :True=OK(検索ヒット),False=NG（検索失敗）
        'Create         :2004/05/28
        'Update         :
        '=====================================================================================
        fn_Select_TENMAST = False
start:
        Try
            gstrSSQL = "SELECT * FROM TENMAST WHERE KIN_NO_N = '" & Trim(KIN_NO) & "' AND SIT_NO_N = '" & Trim(SIT_NO) & "' AND EDA_N = '01'"

            gdbCOMMAND = New OracleClient.OracleCommand
            gdbCOMMAND.CommandText = gstrSSQL
            gdbCOMMAND.Connection = gdbcCONNECT

            'gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ
            gdbrREADER = gdbCOMMAND.ExecuteReader

            KIN_NNAME = ""
            SIT_NNAME = ""
            KIN_KNAME = ""
            SIT_KNAME = ""

            '読込のみ
            If gdbrREADER.Read = False Then
                fn_Select_TENMAST = False
                Exit Function
            Else
                KIN_NNAME = gdbrREADER.Item("KIN_NNAME_N")
                SIT_NNAME = gdbrREADER.Item("SIT_NNAME_N")
                KIN_KNAME = gdbrREADER.Item("KIN_KNAME_N")
                SIT_KNAME = gdbrREADER.Item("SIT_KNAME_N")
                fn_Select_TENMAST = True
            End If
        Catch ex As Exception
            If Err.Number = 5 Then
                '-----------------------------------------
                '１秒停止
                '-----------------------------------------
                Dim Start As Double
                Start = CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss"))           ' 中断の開始時刻を設定します。
                Do While CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss")) < Start + 1
                    Application.DoEvents()
                Loop
                Err.Clear()
                GoTo start
            End If
            If Err.Number <> 0 And Err.Number <> 5 Then
                Exit Function
            End If

        End Try

    End Function
#End Region
#Region "全銀"
    Public Sub fn_UKETUKE_MEISAI_PRINT_ZENGIN(ByVal strFILE_NAME As String, ByVal strTORI_CODE As String, ByVal strFURI_DATE As String, ByVal intREC_LENGTH As Integer, ByVal strTUUBAN As String, ByRef blnKEKKA As Boolean)
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_PRINT_ZENGIN
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長／strTUUBAN：通番
        '               :blnKEKKA：結果
        'Description    :受付明細表印刷メイン処理(全銀)
        'Return         :
        'Create         :2004/08/03
        'Update         :
        '============================================================================
        blnKEKKA = False
        'fn_UKETUKE_MEISAI_PRINT = False
        intF_REC_LENGTH = intREC_LENGTH
        strTORIS_CODE = strTORI_CODE.Substring(0, 7)
        strTORIF_CODE = strTORI_CODE.Substring(7, 2)

        gstrSYORI_R = "受付明細表印刷"
        Dim pd As New System.Drawing.Printing.PrintDocument

        '------------------------------------------
        'ログファイル作成ディレクトリの取得
        '------------------------------------------
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "LOG"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのLOGディレクトリの取得に失敗しました", "明細表印刷")
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrLOG_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "DATBK"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのDATBKディレクトリの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrDATBK_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "TENPO"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの支店読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "支店読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrTENPO_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "KOUZA"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの口座読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "口座読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrKOUZA_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "KINKOCD"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの自金庫コードの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "自金庫コード取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrJIKINKO = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "UKETUKECPY"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrUKETUKECPY = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        '蒲郡信金用　2007/07/27
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "KINKONAME"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのKINKONAMEの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            strKINKO_NAME = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If
        '帳票区分セット
        intTYOHYO_KBN = 0

        'ファイルを開き、1企業ごとのファイルを作成し、ソート処理対象の取引先はソート処理を行う。
        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        gdbcCONNECT.Open()
        Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
        dblRECORD_COUNT = 0
        intFILE_SEQ = 0
        strDATA_KBN = "0"
        dblALL_KEN = 0
        dblALL_KIN = 0
        intGYO = 1
        intMULTI_FLG = 0
        intFILE_NO_1 = FreeFile()
        '2006/06/28 マルチ再振データ作成対象のために追加
        gstrFURI_DATE = strFURI_DATE

        FileOpen(intFILE_NO_1, strFILE_NAME, OpenMode.Random, , , intREC_LENGTH)
        If Err.Number = 0 Then
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "成功", Err.Description)
        Else
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "失敗", Err.Description)
            Exit Sub
        End If
        strF_FURI_DATE = strFURI_DATE
        Do Until EOF(intFILE_NO_1)

            dblRECORD_COUNT += 1
            Try
                FileGet(intFILE_NO_1, gstrDATA, dblRECORD_COUNT)
                strFURI_DATA = gstrDATA.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    'ワーク用のファイルを開き、書き込む
                    intFILE_SEQ += 1
                    strWRK_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".DAT"
                    If Dir(strWRK_FILE) <> "" Then
                        Kill(strWRK_FILE)
                    End If
                    intFILE_NO_2 = FreeFile()

                    dblRECORD_SEQ = 0

                    FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Append, , , intREC_LENGTH)   '出力ファイル
                    Print(intFILE_NO_2, gstrDATA.strDATA)

                    If strDATA_KBN <> "0" And strDATA_KBN <> "8" And strDATA_KBN <> "9" Then
                        Exit Sub
                    End If
                Case "2"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA.strDATA)
                Case "8"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA.strDATA)
                    FileClose(intFILE_NO_2)

                    Dim prDialog As New PrintDialog

                    '1企業のファイルが作成されたので、そのファイルを読み込み明細表印刷
                    intKAISUU_FLG = 0
                    If fn_MEISAI_KOBETU_PRINT_ZENGIN() = False Then
                        pd.PrinterSettings.PrintToFile = False
                        gdbcCONNECT.Close()
                        Exit Sub
                    End If

                Case "9"



            End Select
        Loop
        FileClose(intFILE_NO_1)
        gdbcCONNECT.Close()
        blnKEKKA = True

    End Sub
    Public Function fn_MEISAI_KOBETU_PRINT_ZENGIN() As Boolean
        '============================================================================
        'NAME           :fn_MEISAI_KOBETU_PRINT_ZENGIN
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長
        'Description    :受付明細表印刷メイン処理
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2004/08/03
        'Update         :2007/09/13 蒲郡信金向け 他行元受マルチファイル対応
        '============================================================================
        fn_MEISAI_KOBETU_PRINT_ZENGIN = False
        Dim intFILE_NO_2 As Integer
        Dim dblRECORD_COUNT_WRK As Double
        intFILE_NO_2 = FreeFile()
        intGYO = 0
        intPAGE = 0
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)

        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Function
            End Try

            Select Case gstrDATA.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    FileGet(intFILE_NO_2, gZENGIN_REC1, dblRECORD_COUNT_WRK)
                    strF_ITAKU_CODE = gZENGIN_REC1.ZG4
                    '取引先コードの取得
                    gstrSSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND TORIF_CODE_T = '" & Trim(strTORIF_CODE) & "'"

                    gdbCOMMAND = New OracleClient.OracleCommand
                    gdbCOMMAND.CommandText = gstrSSQL
                    gdbCOMMAND.Connection = gdbcCONNECT

                    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                    While (gdbrREADER.Read)

                        strITAKU_CODE = gdbrREADER.Item("ITAKU_CODE_T")
                        strMULTI_KBN = gdbrREADER.Item("MULTI_KBN_T") '2007/09/13
                        If strF_ITAKU_CODE <> strITAKU_CODE Then
                            '2006/06/28 マルチ再振データ作成対象のため、スケジュールも条件に追加
                            'gstrSSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            If strMULTI_KBN = "4" Then '他行元受マルチファイル対応　2007/09/13
                                gstrSSQL = "SELECT * FROM TORIMAST,SCHMAST WHERE ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            Else
                                gstrSSQL = "SELECT * FROM TORIMAST,SCHMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            End If
                            gstrSSQL = gstrSSQL & " AND FURI_DATE_S = '" & Trim(gstrFURI_DATE) & "' AND YUUKOU_FLG_S = '1'"
                            gstrSSQL = gstrSSQL & " AND TORIS_CODE_T = TORIS_CODE_S AND TORIF_CODE_T = TORIF_CODE_S"

                            gdbCOMMAND.CommandText = gstrSSQL
                            gdbCOMMAND.Connection = gdbcCONNECT

                            gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                            If gdbrREADER.Read = False Then
                                '一致する取引先コードが存在しない
                                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                                Exit Function
                            End If
                        End If
                        If strMULTI_KBN = "4" Then '他行元受マルチファイル対応　2007/09/13
                            strTORIS_CODE = gdbrREADER.Item("TORIS_CODE_T")
                        End If
                        strTORIF_CODE = gdbrREADER.Item("TORIF_CODE_T")
                        strITAKU_NNAME = gdbrREADER.Item("ITAKU_NNAME_T")
                        strTKIN_NO = gdbrREADER.Item("TKIN_NO_T")
                        strTSIT_NO = gdbrREADER.Item("TSIT_NO_T")
                        strCODE_KBN = gdbrREADER.Item("CODE_KBN_T")
                        'strMULTI_KBN = gdbrREADER.Item("MOTIKOMI_KBN_T")'2007/09/13
                        strUKETUKE_KBN = gdbrREADER.Item("UMEISAI_KBN_T")
                    End While
                    If fn_Select_TENMAST(strTKIN_NO, strTSIT_NO, strITAKU_KIN_NNAME, strITAKU_SIT_NNAME, strITAKU_KIN_KNAME, strITAKU_SIT_KNAME) = False Then

                    End If
                    If Err.Number <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                        Exit Function
                    End If
                    intPAGE = 1
                    If intFILE_SEQ = 1 Then
                        Call fn_UKETUKE_MEISAI_HEDDA()
                    End If

                    Exit Do


            End Select


        Loop
        FileClose(intFILE_NO_2)
        dblALL_KEN = 0
        dblALL_KIN = 0
        If fn_UKETUKE_MEISAI_DETAIL_ZENGIN() = False Then
            Exit Function
        End If
        fn_MEISAI_KOBETU_PRINT_ZENGIN = True
    End Function
    Public Function fn_UKETUKE_MEISAI_DETAIL_ZENGIN() As Boolean
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_DETAIL_ZENGIN
        'Parameter      :
        'Description    :受付明細表データ部印刷(全銀)
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2004/08/03
        'Update         :
        '============================================================================
        fn_UKETUKE_MEISAI_DETAIL_ZENGIN = False
        '------------------------------------------------------------------
        '受付明細表印刷区分により印刷ロジック分岐
        '------------------------------------------------------------------
        dblRECORD_COUNT_WRK = 0
        Try
            Select Case strUKETUKE_KBN
                Case "0"        '非対称
                    fn_UKETUKE_MEISAI_DETAIL_ZENGIN = True
                    Exit Function
                Case "1"        '対象（店番ソート）
                    '------------------------------------------------
                    'ファイルソート（POWER SORT）
                    '------------------------------------------------

                    strSORT_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".SRT"
                    If Dir(strSORT_FILE) <> "" Then
                        Kill(strSORT_FILE)
                    End If
                    frmREPORT.objAxPowerSORT.DispMessage = False
                    frmREPORT.objAxPowerSORT.DisposalNumber = 0
                    frmREPORT.objAxPowerSORT.FieldDefinition = 1
                    frmREPORT.objAxPowerSORT.KeyCmdStr = "0.1asca 1.4asca 20.3asca 42.1asca 43.7asca"
                    frmREPORT.objAxPowerSORT.InputFiles = strWRK_FILE
                    frmREPORT.objAxPowerSORT.InputFileType = 1
                    frmREPORT.objAxPowerSORT.OutputFile = strSORT_FILE
                    frmREPORT.objAxPowerSORT.OutputFileType = 1
                    frmREPORT.objAxPowerSORT.MaxRecordLength = 120
                    frmREPORT.objAxPowerSORT.Action()

                    If frmREPORT.objAxPowerSORT.ErrorCode <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ファイルソート", "失敗", frmREPORT.objAxPowerSORT.ErrorDetail)
                        Exit Function
                    End If
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZENGIN
                Case "2"        '対象（非ソート）
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAILPage_ZENGIN
                Case "3"        'エラー分のみ
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZENGIN

            End Select
            'ページを横に設定
            pd.DefaultPageSettings.Landscape = True

            '印刷を開始する
            intMULTI_FLG += 1
            pd.DefaultPageSettings.PrinterSettings.Collate = True
            pd.DefaultPageSettings.PrinterSettings.Copies = Val(gstrUKETUKECPY)
            pd.Print()
        Catch ex As Exception
            Exit Function
        End Try
        If Dir(strWRK_FILE) <> "" Then
            Kill(strWRK_FILE)
        End If
        If strUKETUKE_KBN = "1" Then
            If Dir(strSORT_FILE) <> "" Then
                Kill(strSORT_FILE)
            End If
        End If
        fn_UKETUKE_MEISAI_DETAIL_ZENGIN = True

    End Function
    Private Sub pd_UKETUKE_MEISAI_DETAILPage_ZENGIN(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAILPage_ZENGIN
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（ソート非対象）
        'Return         :
        'Create         :2004/08/03
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If
        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        'Dim X_ITI() As Integer = {100, 150, 260, 400, 615, 675, 760, 1070} '見出し位置
        Dim X_ITI() As Integer = {100, 120, 230, 390, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA.strDATA.Substring(0, 1)
                Case "1"
                Case "2"

                    FileGet(intFILE_NO_2, gZENGIN_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gZENGIN_REC2.ZG2
                    strF_KEIYAKU_SIT = gZENGIN_REC2.ZG4
                    strF_KEIYAKU_KAMOKU = gZENGIN_REC2.ZG7
                    strF_KEIYAKU_KOUZA = gZENGIN_REC2.ZG8
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gZENGIN_REC2.ZG9), 30)
                    strF_FURIKIN = gZENGIN_REC2.ZG10
                    strF_SINKI_NO = gZENGIN_REC2.ZG11
                    strF_KEIYAKU_NO = gZENGIN_REC2.ZG12
                    strF_JYUYOKA_NO = gZENGIN_REC2.ZG12 & gZENGIN_REC2.ZG13
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, CObj(strYOMIKAE_KIN_NO), CObj(strYOMIKAE_SIT_NO)) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If

                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '-----------------------------------
                    'エラーチェック
                    '-----------------------------------
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If
                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
    Private Sub pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZENGIN(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZENGIN
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（店番ソート）
        'Return         :
        'Create         :2004/08/09
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If

        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strSORT_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA.strDATA.Substring(0, 1)
                Case "1"
                    strMAE_KEIYAKU_KIN = Nothing
                    strMAE_KEIYAKU_SIT = Nothing
                    dblSIT_KEN = 0
                    dblSIT_KIN = 0
                Case "2"

                    FileGet(intFILE_NO_2, gZENGIN_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gZENGIN_REC2.ZG2
                    strF_KEIYAKU_SIT = gZENGIN_REC2.ZG4
                    strF_KEIYAKU_KAMOKU = gZENGIN_REC2.ZG7
                    strF_KEIYAKU_KOUZA = gZENGIN_REC2.ZG8
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gZENGIN_REC2.ZG9), 30)
                    strF_FURIKIN = gZENGIN_REC2.ZG10
                    strF_SINKI_NO = gZENGIN_REC2.ZG11
                    strF_KEIYAKU_NO = gZENGIN_REC2.ZG12
                    strF_JYUYOKA_NO = gZENGIN_REC2.ZG12 & gZENGIN_REC2.ZG13
                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    If strMAE_KEIYAKU_KIN = Nothing And strMAE_KEIYAKU_SIT = Nothing Then
                        strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                        strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                    End If
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, CObj(strYOMIKAE_KIN_NO), CObj(strYOMIKAE_SIT_NO)) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If
                    If strF_KEIYAKU_KIN <> strMAE_KEIYAKU_KIN Or strF_KEIYAKU_SIT <> strMAE_KEIYAKU_SIT Then
                        dblRECORD_COUNT_WRK -= 1
                        '小計を書き込む
                        ev.Graphics.DrawString("小計", printFont, Brushes.Black, 670, 730)
                        ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 730)
                        ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 730)
                        ev.Graphics.DrawString(Format(dblSIT_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblSIT_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
                        ev.Graphics.DrawString(Format(dblSIT_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblSIT_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
                        dblSIT_KEN = 0
                        dblSIT_KIN = 0
                        strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                        strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                        intGYO = 41
                        Exit Do
                    End If
                    strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                    strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                    dblSIT_KEN += 1
                    dblSIT_KIN = dblSIT_KIN + Val(strF_FURIKIN)
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '-----------------------------------
                    'エラーチェック
                    '-----------------------------------
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If

                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            '小計を書き込む
            ev.Graphics.DrawString("小計", printFont, Brushes.Black, 670, 730)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 730)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 730)
            ev.Graphics.DrawString(Format(dblSIT_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblSIT_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
            ev.Graphics.DrawString(Format(dblSIT_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblSIT_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
    Private Sub pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZENGIN(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZENGIN
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（エラー分のみ）
        'Return         :
        'Create         :2004/08/09
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If
        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA.strDATA.Substring(0, 1)
                Case "1"
                Case "2"

                    FileGet(intFILE_NO_2, gZENGIN_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gZENGIN_REC2.ZG2
                    strF_KEIYAKU_SIT = gZENGIN_REC2.ZG4
                    strF_KEIYAKU_KAMOKU = gZENGIN_REC2.ZG7
                    strF_KEIYAKU_KOUZA = gZENGIN_REC2.ZG8
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gZENGIN_REC2.ZG9), 30)
                    strF_FURIKIN = gZENGIN_REC2.ZG10
                    strF_SINKI_NO = gZENGIN_REC2.ZG11
                    strF_KEIYAKU_NO = gZENGIN_REC2.ZG12
                    strF_JYUYOKA_NO = gZENGIN_REC2.ZG12 & gZENGIN_REC2.ZG13
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, CObj(strYOMIKAE_KIN_NO), CObj(strYOMIKAE_SIT_NO)) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If
                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '☆☆エラーチェックを追加する
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If X_MEI(1) = "" Then
                        Exit Select
                    End If
                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If
                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1030, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
#End Region
#Region "地公体"
    Public Sub fn_UKETUKE_MEISAI_PRINT_ZEIKIN(ByVal strFILE_NAME As String, ByVal strTORI_CODE As String, ByVal strFURI_DATE As String, ByVal intREC_LENGTH As Integer, ByVal strTUUBAN As String, ByRef blnKEKKA As Boolean)
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_PRINT_ZEIKIN
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長／strTUUBAN：通番
        '               :blnKEKKA：結果
        'Description    :受付明細表印刷メイン処理(地公体)
        'Return         :
        'Create         :2004/08/30
        'Update         :
        '============================================================================
        blnKEKKA = False
        intF_REC_LENGTH = intREC_LENGTH
        strTORIS_CODE = strTORI_CODE.Substring(0, 7)
        strTORIF_CODE = strTORI_CODE.Substring(7, 2)

        gstrSYORI_R = "受付明細表印刷"
        Dim pd As New System.Drawing.Printing.PrintDocument

        '------------------------------------------
        'ログファイル作成ディレクトリの取得
        '------------------------------------------
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "LOG"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのLOGディレクトリの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "LOGディレクトリ取得", "失敗", Err.Description)
            'fn_JOBMAST_UPDATE(strSTATUS_ERR, "LOGディレクトリ取得失敗")　　　'通番未取得なのでJOBMAST更新ができない
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrLOG_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "DATBK"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのDATBKディレクトリの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrDATBK_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "TENPO"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの支店読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "支店読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrTENPO_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "KOUZA"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの口座読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "口座読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrKOUZA_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "KINKOCD"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの自金庫コードの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "自金庫コード取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrJIKINKO = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "UKETUKECPY"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrUKETUKECPY = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        '蒲郡信金用　2007/07/27
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "KINKONAME"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのKINKONAMEの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            strKINKO_NAME = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If
        '帳票区分セット
        intTYOHYO_KBN = 0

        'ファイルを開き、1企業ごとのファイルを作成し、ソート処理対象の取引先はソート処理を行う。
        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        gdbcCONNECT.Open()
        Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
        dblRECORD_COUNT = 0
        intFILE_SEQ = 0
        strDATA_KBN = "0"
        dblALL_KEN = 0
        dblALL_KIN = 0
        intGYO = 1
        intMULTI_FLG = 0
        intFILE_NO_1 = FreeFile()
        '2006/06/28 マルチ再振データ作成対象のために追加
        gstrFURI_DATE = strFURI_DATE

        FileOpen(intFILE_NO_1, strFILE_NAME, OpenMode.Random, , , intREC_LENGTH)
        If Err.Number = 0 Then
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "成功", Err.Description)
        Else
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "失敗", Err.Description)
            Exit Sub
        End If
        strF_FURI_DATE = strFURI_DATE
        Do Until EOF(intFILE_NO_1)

            dblRECORD_COUNT += 1
            Try
                FileGet(intFILE_NO_1, gstrDATA_220, dblRECORD_COUNT)
                strFURI_DATA = gstrDATA_220.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_220.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    'ワーク用のファイルを開き、書き込む
                    intFILE_SEQ += 1
                    strWRK_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".DAT"
                    If Dir(strWRK_FILE) <> "" Then
                        Kill(strWRK_FILE)
                    End If
                    intFILE_NO_2 = FreeFile()

                    dblRECORD_SEQ = 0

                    FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Append, , , intREC_LENGTH)   '出力ファイル
                    Print(intFILE_NO_2, gstrDATA_220.strDATA)

                    If strDATA_KBN <> "0" And strDATA_KBN <> "8" And strDATA_KBN <> "9" Then
                        Exit Sub
                    End If
                Case "2"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_220.strDATA)
                Case "8"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_220.strDATA)
                    FileClose(intFILE_NO_2)

                    Dim prDialog As New PrintDialog

                    '1企業のファイルが作成されたので、そのファイルを読み込み明細表印刷
                    intKAISUU_FLG = 0
                    If fn_MEISAI_KOBETU_PRINT_ZEIKIN() = False Then
                        pd.PrinterSettings.PrintToFile = False
                        gdbcCONNECT.Close()
                        Exit Sub
                    End If
                Case "9"



            End Select
        Loop
        FileClose(intFILE_NO_1)
        gdbcCONNECT.Close()
        blnKEKKA = True

    End Sub
    Public Function fn_MEISAI_KOBETU_PRINT_ZEIKIN() As Boolean
        '============================================================================
        'NAME           :fn_MEISAI_KOBETU_PRINT_ZEIKIN
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長
        'Description    :受付明細表印刷メイン処理
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2004/08/30
        'Update         :
        '============================================================================
        fn_MEISAI_KOBETU_PRINT_ZEIKIN = False
        Dim intFILE_NO_2 As Integer
        Dim dblRECORD_COUNT_WRK As Double
        intFILE_NO_2 = FreeFile()
        intGYO = 0
        intPAGE = 0
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)

        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_220, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_220.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Function
            End Try

            Select Case gstrDATA_220.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    FileGet(intFILE_NO_2, gZEIKIN_REC1, dblRECORD_COUNT_WRK)
                    strF_ITAKU_CODE = gZEIKIN_REC1.ZK4
                    '取引先コードの取得
                    gstrSSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND TORIF_CODE_T = '" & Trim(strTORIF_CODE) & "'"

                    gdbCOMMAND = New OracleClient.OracleCommand
                    gdbCOMMAND.CommandText = gstrSSQL
                    gdbCOMMAND.Connection = gdbcCONNECT

                    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                    While (gdbrREADER.Read)

                        strITAKU_CODE = gdbrREADER.Item("ITAKU_CODE_T")
                        If strF_ITAKU_CODE <> strITAKU_CODE Then
                            '2006/06/28 マルチ再振データ作成対象のため、スケジュールも条件に追加
                            'gstrSSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            gstrSSQL = "SELECT * FROM TORIMAST,SCHMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            gstrSSQL = gstrSSQL & " AND FURI_DATE_S = '" & Trim(gstrFURI_DATE) & "' AND YUUKOU_FLG_S = '1'"
                            gstrSSQL = gstrSSQL & " AND TORIS_CODE_T = TORIS_CODE_S AND TORIF_CODE_T = TORIF_CODE_S"
                            gdbCOMMAND.CommandText = gstrSSQL
                            gdbCOMMAND.Connection = gdbcCONNECT

                            gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                            If gdbrREADER.Read = False Then
                                '一致する取引先コードが存在しない
                                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                                Exit Function
                            End If
                        End If
                        strTORIF_CODE = gdbrREADER.Item("TORIF_CODE_T")
                        strITAKU_NNAME = gdbrREADER.Item("ITAKU_NNAME_T")
                        strTKIN_NO = gdbrREADER.Item("TKIN_NO_T")
                        strTSIT_NO = gdbrREADER.Item("TSIT_NO_T")
                        strCODE_KBN = gdbrREADER.Item("CODE_KBN_T")
                        strMULTI_KBN = gdbrREADER.Item("MOTIKOMI_KBN_T")
                        strUKETUKE_KBN = gdbrREADER.Item("UMEISAI_KBN_T")
                    End While
                    'gdbcCONNECT.Close()
                    If fn_Select_TENMAST(strTKIN_NO, strTSIT_NO, strITAKU_KIN_NNAME, strITAKU_SIT_NNAME, strITAKU_KIN_KNAME, strITAKU_SIT_KNAME) = False Then

                    End If
                    If Err.Number <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                        Exit Function
                    End If
                    intPAGE = 1
                    If intFILE_SEQ = 1 Then
                        Call fn_UKETUKE_MEISAI_HEDDA()
                    End If

                    Exit Do


            End Select


        Loop
        FileClose(intFILE_NO_2)
        dblALL_KEN = 0
        dblALL_KIN = 0
        If fn_UKETUKE_MEISAI_DETAIL_ZEIKIN() = False Then
            Exit Function
        End If

        fn_MEISAI_KOBETU_PRINT_ZEIKIN = True

    End Function
    Public Function fn_UKETUKE_MEISAI_DETAIL_ZEIKIN() As Boolean
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_DETAIL_ZENGIN
        'Parameter      :
        'Description    :受付明細表データ部印刷(地公体)
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2004/08/30
        'Update         :
        '============================================================================
        fn_UKETUKE_MEISAI_DETAIL_ZEIKIN = False
        '------------------------------------------------------------------
        '受付明細表印刷区分により印刷ロジック分岐
        '------------------------------------------------------------------
        dblRECORD_COUNT_WRK = 0
        Try
            Select Case strUKETUKE_KBN
                Case "0"        '非対称
                    fn_UKETUKE_MEISAI_DETAIL_ZEIKIN = True
                    Exit Function
                Case "1"        '対象（店番ソート）
                    '------------------------------------------------
                    'ファイルソート（POWER SORT）
                    '------------------------------------------------

                    strSORT_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".SRT"
                    If Dir(strSORT_FILE) <> "" Then
                        Kill(strSORT_FILE)
                    End If
                    frmREPORT.objAxPowerSORT.DispMessage = False
                    frmREPORT.objAxPowerSORT.DisposalNumber = 0
                    frmREPORT.objAxPowerSORT.FieldDefinition = 1
                    frmREPORT.objAxPowerSORT.KeyCmdStr = "0.1asca 1.4asca 20.3asca 42.1asca 43.7asca"
                    frmREPORT.objAxPowerSORT.InputFiles = strWRK_FILE
                    frmREPORT.objAxPowerSORT.InputFileType = 1
                    frmREPORT.objAxPowerSORT.OutputFile = strSORT_FILE
                    frmREPORT.objAxPowerSORT.OutputFileType = 1
                    frmREPORT.objAxPowerSORT.MaxRecordLength = 220
                    frmREPORT.objAxPowerSORT.Action()

                    If frmREPORT.objAxPowerSORT.ErrorCode <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ファイルソート", "失敗", frmREPORT.objAxPowerSORT.ErrorDetail)
                        Exit Function
                    End If
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZEIKIN
                Case "2"        '対象（非ソート）
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAILPage_ZEIKIN
                Case "3"        'エラー分のみ
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZEIKIN

            End Select
            'ページを横に設定
            pd.DefaultPageSettings.Landscape = True

            '印刷を開始する
            intMULTI_FLG += 1
            pd.DefaultPageSettings.PrinterSettings.Collate = True
            pd.DefaultPageSettings.PrinterSettings.Copies = Val(gstrUKETUKECPY)
            pd.Print()
        Catch ex As Exception
            Exit Function
        End Try

        If Dir(strWRK_FILE) <> "" Then
            Kill(strWRK_FILE)
        End If
        If strUKETUKE_KBN = "1" Then
            If Dir(strSORT_FILE) <> "" Then
                Kill(strSORT_FILE)
            End If
        End If

        fn_UKETUKE_MEISAI_DETAIL_ZEIKIN = True
    End Function
    Private Sub pd_UKETUKE_MEISAI_DETAILPage_ZEIKIN(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAILPage_ZEIKIN
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（ソート非対象）（地公体）
        'Return         :
        'Create         :2004/08/30
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If
        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        'Dim X_ITI() As Integer = {100, 150, 260, 400, 615, 675, 760, 1070} '見出し位置
        'Dim X_MEI(7) As String
        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_220, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_220.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_220.strDATA.Substring(0, 1)
                Case "1"
                Case "2"

                    FileGet(intFILE_NO_2, gZEIKIN_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gZEIKIN_REC2.ZK2
                    strF_KEIYAKU_SIT = gZEIKIN_REC2.ZK4
                    strF_KEIYAKU_KAMOKU = gZEIKIN_REC2.ZK6
                    strF_KEIYAKU_KOUZA = gZEIKIN_REC2.ZK7
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gZEIKIN_REC2.ZK8), 30)
                    strF_FURIKIN = gZEIKIN_REC2.ZK9
                    strF_SINKI_NO = gZEIKIN_REC2.ZK14
                    strF_KEIYAKU_NO = gZEIKIN_REC2.ZK15.Substring(0, 15)
                    strF_JYUYOKA_NO = gZEIKIN_REC2.ZK15
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, CObj(strYOMIKAE_KIN_NO), CObj(strYOMIKAE_SIT_NO)) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If
                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '-----------------------------------
                    'エラーチェック
                    '-----------------------------------
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If
                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
    Private Sub pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZEIKIN(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZEIKIN
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（店番ソート）（地公体）
        'Return         :
        'Create         :2004/08/30
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If

        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strSORT_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_220, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_220.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_220.strDATA.Substring(0, 1)
                Case "1"
                    strMAE_KEIYAKU_KIN = Nothing
                    strMAE_KEIYAKU_SIT = Nothing
                    dblSIT_KEN = 0
                    dblSIT_KIN = 0
                Case "2"

                    FileGet(intFILE_NO_2, gZEIKIN_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gZEIKIN_REC2.ZK2
                    strF_KEIYAKU_SIT = gZEIKIN_REC2.ZK4
                    strF_KEIYAKU_KAMOKU = gZEIKIN_REC2.ZK6
                    strF_KEIYAKU_KOUZA = gZEIKIN_REC2.ZK7
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gZEIKIN_REC2.ZK8), 30)
                    strF_FURIKIN = gZEIKIN_REC2.ZK9
                    strF_SINKI_NO = gZEIKIN_REC2.ZK14
                    strF_KEIYAKU_NO = gZEIKIN_REC2.ZK15.Substring(0, 15)
                    strF_JYUYOKA_NO = gZEIKIN_REC2.ZK15
                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    If strMAE_KEIYAKU_KIN = Nothing And strMAE_KEIYAKU_SIT = Nothing Then
                        strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                        strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                    End If
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strYOMIKAE_KIN_NO, strYOMIKAE_SIT_NO) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If
                    If strF_KEIYAKU_KIN <> strMAE_KEIYAKU_KIN Or strF_KEIYAKU_SIT <> strMAE_KEIYAKU_SIT Then
                        dblRECORD_COUNT_WRK -= 1
                        '小計を書き込む
                        ev.Graphics.DrawString("小計", printFont, Brushes.Black, 670, 730)
                        ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 730)
                        ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 730)
                        ev.Graphics.DrawString(Format(dblSIT_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblSIT_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
                        ev.Graphics.DrawString(Format(dblSIT_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblSIT_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
                        dblSIT_KEN = 0
                        dblSIT_KIN = 0
                        strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                        strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                        intGYO = 41
                        Exit Do
                    End If
                    strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                    strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                    dblSIT_KEN += 1
                    dblSIT_KIN = dblSIT_KIN + Val(strF_FURIKIN)
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '-----------------------------------
                    'エラーチェック
                    '-----------------------------------
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If
                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            '小計を書き込む
            ev.Graphics.DrawString("小計", printFont, Brushes.Black, 670, 730)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 730)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 730)
            ev.Graphics.DrawString(Format(dblSIT_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblSIT_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
            ev.Graphics.DrawString(Format(dblSIT_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblSIT_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
    Private Sub pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZEIKIN(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZEIKIN
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（エラー分のみ）（地公体）
        'Return         :
        'Create         :2004/08/30
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If
        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_220, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_220.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_220.strDATA.Substring(0, 1)
                Case "1"
                Case "2"

                    FileGet(intFILE_NO_2, gZEIKIN_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gZEIKIN_REC2.ZK2
                    strF_KEIYAKU_SIT = gZEIKIN_REC2.ZK4
                    strF_KEIYAKU_KAMOKU = gZEIKIN_REC2.ZK6
                    strF_KEIYAKU_KOUZA = gZEIKIN_REC2.ZK7
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gZEIKIN_REC2.ZK8), 30)
                    strF_FURIKIN = gZEIKIN_REC2.ZK9
                    strF_SINKI_NO = gZEIKIN_REC2.ZK14
                    strF_KEIYAKU_NO = gZEIKIN_REC2.ZK15.Substring(0, 15)
                    strF_JYUYOKA_NO = gZEIKIN_REC2.ZK15
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strYOMIKAE_KIN_NO, strYOMIKAE_SIT_NO) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If
                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '-----------------------------------
                    'エラーチェック
                    '-----------------------------------
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If X_MEI(1) = "" Then
                        Exit Select
                    End If
                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If
                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1030, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
#End Region
#Region "国税"
    Public Sub fn_UKETUKE_MEISAI_PRINT_KOKUZEI(ByVal strFILE_NAME As String, ByVal strTORI_CODE As String, ByVal strFURI_DATE As String, ByVal intREC_LENGTH As Integer, ByVal strTUUBAN As String, ByRef blnKEKKA As Boolean)
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_PRINT_KOKUZEI
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長／strTUUBAN：通番
        '               :blnKEKKA：結果
        'Description    :受付明細表印刷メイン処理(国税)
        'Return         :
        'Create         :2004/12/07
        'Update         :
        '============================================================================
        blnKEKKA = False
        intF_REC_LENGTH = intREC_LENGTH
        strTORIS_CODE = strTORI_CODE.Substring(0, 7)
        strTORIF_CODE = strTORI_CODE.Substring(7, 2)

        gstrSYORI_R = "受付明細表印刷"
        Dim pd As New System.Drawing.Printing.PrintDocument

        '------------------------------------------
        'ログファイル作成ディレクトリの取得
        '------------------------------------------
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "LOG"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのLOGディレクトリの取得に失敗しました", "明細表印刷")
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrLOG_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "DATBK"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのDATBKディレクトリの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrDATBK_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "TENPO"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの支店読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "支店読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrTENPO_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "KOUZA"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの口座読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "口座読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrKOUZA_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "KINKOCD"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの自金庫コードの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "自金庫コード取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrJIKINKO = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "UKETUKECPY"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrUKETUKECPY = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        '蒲郡信金用　2007/07/27
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "KINKONAME"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのKINKONAMEの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            strKINKO_NAME = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If
        '帳票区分セット
        intTYOHYO_KBN = 0

        'ファイルを開き、1企業ごとのファイルを作成し、ソート処理対象の取引先はソート処理を行う。
        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        gdbcCONNECT.Open()
        Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
        dblRECORD_COUNT = 0
        intFILE_SEQ = 0
        strDATA_KBN = "0"
        dblALL_KEN = 0
        dblALL_KIN = 0
        intGYO = 1
        intMULTI_FLG = 0
        intFILE_NO_1 = FreeFile()
        '2006/06/28 マルチ再振データ作成対象のために追加
        gstrFURI_DATE = strFURI_DATE

        FileOpen(intFILE_NO_1, strFILE_NAME, OpenMode.Random, , , intREC_LENGTH)
        If Err.Number = 0 Then
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "成功", Err.Description)
        Else
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "失敗", Err.Description)
            Exit Sub
        End If
        strF_FURI_DATE = strFURI_DATE
        Do Until EOF(intFILE_NO_1)

            dblRECORD_COUNT += 1
            Try
                FileGet(intFILE_NO_1, gstrDATA_390, dblRECORD_COUNT)
                strFURI_DATA = gstrDATA_390.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_390.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    'ワーク用のファイルを開き、書き込む
                    intFILE_SEQ += 1
                    strWRK_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".DAT"
                    If Dir(strWRK_FILE) <> "" Then
                        Kill(strWRK_FILE)
                    End If
                    intFILE_NO_2 = FreeFile()

                    dblRECORD_SEQ = 0

                    FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Append, , , intREC_LENGTH)   '出力ファイル
                    Print(intFILE_NO_2, gstrDATA_390.strDATA)

                    If strDATA_KBN <> "0" And strDATA_KBN <> "8" And strDATA_KBN <> "9" Then
                        Exit Sub
                    End If
                Case "3"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_390.strDATA)
                Case "8"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_390.strDATA)
                    FileClose(intFILE_NO_2)

                    Dim prDialog As New PrintDialog

                    '1企業のファイルが作成されたので、そのファイルを読み込み明細表印刷
                    intKAISUU_FLG = 0
                    'Call fn_MEISAI_KOBETU_PRINT_KOKUZEI()
                    If fn_MEISAI_KOBETU_PRINT_KOKUZEI() = False Then
                        gdbcCONNECT.Close()
                        pd.PrinterSettings.PrintToFile = False
                        gdbcCONNECT.Close()
                        Exit Sub
                    End If
                Case "9"



            End Select
        Loop
        FileClose(intFILE_NO_1)
        gdbcCONNECT.Close()
        blnKEKKA = True

    End Sub
    Public Function fn_MEISAI_KOBETU_PRINT_KOKUZEI() As Boolean
        '============================================================================
        'NAME           :fn_MEISAI_KOBETU_PRINT_KOKUZEI
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長
        'Description    :受付明細表印刷メイン処理
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2004/12/07
        'Update         :
        '============================================================================
        fn_MEISAI_KOBETU_PRINT_KOKUZEI = False
        Dim intFILE_NO_2 As Integer
        Dim dblRECORD_COUNT_WRK As Double
        intFILE_NO_2 = FreeFile()
        intGYO = 0
        intPAGE = 0
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)

        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_390, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Function
            End Try

            Select Case gstrDATA_390.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    FileGet(intFILE_NO_2, gKOKUZEI_REC1, dblRECORD_COUNT_WRK)
                    strF_ITAKU_CODE = ""
                    '取引先コードの取得
                    gstrSSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND TORIF_CODE_T = '" & Trim(strTORIF_CODE) & "'"

                    gdbCOMMAND = New OracleClient.OracleCommand
                    gdbCOMMAND.CommandText = gstrSSQL
                    gdbCOMMAND.Connection = gdbcCONNECT

                    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                    While (gdbrREADER.Read)

                        strITAKU_CODE = gdbrREADER.Item("ITAKU_CODE_T")
                        '国税フォーマットに委託者コードなし
                        'If strF_ITAKU_CODE <> strITAKU_CODE Then
                        '    gstrSSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                        '    gdbCOMMAND.CommandText = gstrSSQL
                        '    gdbCOMMAND.Connection = gdbcCONNECT

                        '    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                        '    If gdbrREADER.Read = False Then
                        '        '一致する取引先コードが存在しない
                        '        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                        '        Exit Function
                        '    End If
                        'End If
                        strTORIF_CODE = gdbrREADER.Item("TORIF_CODE_T")
                        strITAKU_NNAME = gdbrREADER.Item("ITAKU_NNAME_T")
                        strTKIN_NO = gdbrREADER.Item("TKIN_NO_T")
                        strTSIT_NO = gdbrREADER.Item("TSIT_NO_T")
                        strCODE_KBN = gdbrREADER.Item("CODE_KBN_T")
                        strMULTI_KBN = gdbrREADER.Item("MOTIKOMI_KBN_T")
                        strUKETUKE_KBN = gdbrREADER.Item("UMEISAI_KBN_T")
                    End While
                    If fn_Select_TENMAST(strTKIN_NO, strTSIT_NO, strITAKU_KIN_NNAME, strITAKU_SIT_NNAME, strITAKU_KIN_KNAME, strITAKU_SIT_KNAME) = False Then

                    End If
                    If Err.Number <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                        Exit Function
                    End If
                    intPAGE = 1
                    If intFILE_SEQ = 1 Then
                        Call fn_UKETUKE_MEISAI_HEDDA()
                    End If

                    Exit Do


            End Select


        Loop
        FileClose(intFILE_NO_2)
        dblALL_KEN = 0
        dblALL_KIN = 0
        If fn_UKETUKE_MEISAI_DETAIL_KOKUZEI() = False Then
            Exit Function
        End If
        fn_MEISAI_KOBETU_PRINT_KOKUZEI = True
    End Function
    Public Function fn_UKETUKE_MEISAI_DETAIL_KOKUZEI() As Boolean
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_DETAIL_KOKUZEI
        'Parameter      :
        'Description    :受付明細表データ部印刷(国税)
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2004/12/07
        'Update         :
        '============================================================================
        fn_UKETUKE_MEISAI_DETAIL_KOKUZEI = False
        '------------------------------------------------------------------
        '受付明細表印刷区分により印刷ロジック分岐
        '------------------------------------------------------------------
        dblRECORD_COUNT_WRK = 0
        Try
            Select Case strUKETUKE_KBN
                Case "0"        '非対称
                    fn_UKETUKE_MEISAI_DETAIL_KOKUZEI = True
                    Exit Function
                Case "1"        '対象（店番ソート）
                    '------------------------------------------------
                    'ファイルソート（POWER SORT）
                    '------------------------------------------------

                    strSORT_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".SRT"
                    If Dir(strSORT_FILE) <> "" Then
                        Kill(strSORT_FILE)
                    End If
                    frmREPORT.objAxPowerSORT.DispMessage = False
                    frmREPORT.objAxPowerSORT.DisposalNumber = 0
                    frmREPORT.objAxPowerSORT.FieldDefinition = 1
                    frmREPORT.objAxPowerSORT.KeyCmdStr = "0.1asca 7.7asca"
                    frmREPORT.objAxPowerSORT.InputFiles = strWRK_FILE
                    frmREPORT.objAxPowerSORT.InputFileType = 1
                    frmREPORT.objAxPowerSORT.OutputFile = strSORT_FILE
                    frmREPORT.objAxPowerSORT.OutputFileType = 1
                    frmREPORT.objAxPowerSORT.MaxRecordLength = 390
                    frmREPORT.objAxPowerSORT.Action()

                    If frmREPORT.objAxPowerSORT.ErrorCode <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ファイルソート", "失敗", frmREPORT.objAxPowerSORT.ErrorDetail)
                        Exit Function
                    End If
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAIL_SORTPage_KOKUZEI
                Case "2"        '対象（非ソート）
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAILPage_KOKUZEI
                Case "3"        'エラー分のみ
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAIL_ERRPage_KOKUZEI
            End Select
            'ページを横に設定
            pd.DefaultPageSettings.Landscape = True

            '印刷を開始する
            intMULTI_FLG += 1
            pd.DefaultPageSettings.PrinterSettings.Collate = True
            pd.DefaultPageSettings.PrinterSettings.Copies = Val(gstrUKETUKECPY)
            pd.Print()
        Catch ex As Exception
            Exit Function
        End Try
        If Dir(strWRK_FILE) <> "" Then
            Kill(strWRK_FILE)
        End If
        If strUKETUKE_KBN = "1" Then
            If Dir(strSORT_FILE) <> "" Then
                Kill(strSORT_FILE)
            End If
        End If
        fn_UKETUKE_MEISAI_DETAIL_KOKUZEI = True

    End Function
    Private Sub pd_UKETUKE_MEISAI_DETAILPage_KOKUZEI(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAILPage_KOKUZEI
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（ソート非対象）
        'Return         :
        'Create         :2004/12/07
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If
        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        'Dim X_ITI() As Integer = {100, 150, 260, 400, 615, 675, 760, 1070} '見出し位置
        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String

        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_390, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_390.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_390.strDATA.Substring(0, 1)
                Case "1"
                Case "3"

                    FileGet(intFILE_NO_2, gKOKUZEI_REC3, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gKOKUZEI_REC3.KZ4.Substring(0, 4)
                    strF_KEIYAKU_SIT = gKOKUZEI_REC3.KZ4.Substring(4, 3)
                    strF_KEIYAKU_KAMOKU = gKOKUZEI_REC3.KZ11
                    strF_KEIYAKU_KOUZA = gKOKUZEI_REC3.KZ12
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gKOKUZEI_REC3.KZ30.Trim & gKOKUZEI_REC3.KZ31.Trim), 30)
                    strF_FURIKIN = gKOKUZEI_REC3.KZ9
                    strF_SINKI_NO = "0"
                    strF_KEIYAKU_NO = gKOKUZEI_REC3.KZ5
                    strF_JYUYOKA_NO = gKOKUZEI_REC3.KZ20 & gKOKUZEI_REC3.KZ21
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strYOMIKAE_KIN_NO, strYOMIKAE_SIT_NO) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If

                    Try

                        If intGYO = 41 Then
                            dblRECORD_COUNT_WRK -= 1
                            Exit Do
                        End If
                        dblALL_KEN += 1
                        dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                        dblRECORD_SEQ += 1
                        '印刷する文字列と位置を設定する
                        printingPosition = 0
                        '印刷に使うフォントを指定する
                        printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                        X_MEI(0) = dblRECORD_SEQ
                        '-----------------------------------
                        'エラーチェック
                        '-----------------------------------
                        X_MEI(1) = fn_DATA_REC_CHK()
                        If intYOMIKAE_FLG = 1 Then
                            If X_MEI(1) <> "" Then
                                X_MEI(1) = X_MEI(1) & " & 読替"
                            Else
                                X_MEI(1) = "読替済み"
                            End If
                        End If

                        If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                            strKIN_NNAME = ""
                            strSIT_NNAME = ""
                        End If
                    Catch ex As Exception
                        Stop
                    End Try
                    Try
                        If strKIN_NNAME.Length > 9 Then
                            strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                        End If
                        X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                        X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                        X_MEI(4) = strF_KEIYAKU_KAMOKU
                        X_MEI(5) = strF_KEIYAKU_KOUZA
                        X_MEI(6) = strF_KEIYAKU_KNAME
                        X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                        If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                            If strYOMIKAE_KIN_NO = "" Then
                                strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                            End If
                            If strYOMIKAE_KOUZA = "" Then
                                strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                            End If
                        End If
                        X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                        Dim i As Integer
                        For i = 0 To 8
                            Select Case i
                                Case 0, 7    '数値項目
                                    ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                                Case Else
                                    ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                            End Select
                        Next
                        intGYO += 1
                    Catch ex As Exception
                        Stop
                    End Try

                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
    Private Sub pd_UKETUKE_MEISAI_DETAIL_SORTPage_KOKUZEI(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAIL_SORTPage_KOKUZEI
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（店番ソート）
        'Return         :
        'Create         :2004/12/07
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If

        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strSORT_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_390, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_390.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_390.strDATA.Substring(0, 1)
                Case "1"
                    strMAE_KEIYAKU_KIN = Nothing
                    strMAE_KEIYAKU_SIT = Nothing
                    dblSIT_KEN = 0
                    dblSIT_KIN = 0
                Case "3"

                    FileGet(intFILE_NO_2, gKOKUZEI_REC3, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gKOKUZEI_REC3.KZ4.Substring(0, 4)
                    strF_KEIYAKU_SIT = gKOKUZEI_REC3.KZ4.Substring(4, 3)
                    strF_KEIYAKU_KAMOKU = gKOKUZEI_REC3.KZ11
                    strF_KEIYAKU_KOUZA = gKOKUZEI_REC3.KZ12
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gKOKUZEI_REC3.KZ30.Trim & gKOKUZEI_REC3.KZ31.Trim), 30)
                    strF_FURIKIN = gKOKUZEI_REC3.KZ9
                    strF_SINKI_NO = "0"
                    strF_KEIYAKU_NO = gKOKUZEI_REC3.KZ5
                    strF_JYUYOKA_NO = gKOKUZEI_REC3.KZ20 & gKOKUZEI_REC3.KZ21
                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    If strMAE_KEIYAKU_KIN = Nothing And strMAE_KEIYAKU_SIT = Nothing Then
                        strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                        strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                    End If
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strYOMIKAE_KIN_NO, strYOMIKAE_SIT_NO) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If
                    If strF_KEIYAKU_KIN <> strMAE_KEIYAKU_KIN Or strF_KEIYAKU_SIT <> strMAE_KEIYAKU_SIT Then
                        dblRECORD_COUNT_WRK -= 1
                        '小計を書き込む
                        ev.Graphics.DrawString("小計", printFont, Brushes.Black, 670, 730)
                        ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 730)
                        ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 730)
                        ev.Graphics.DrawString(Format(dblSIT_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblSIT_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
                        ev.Graphics.DrawString(Format(dblSIT_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblSIT_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
                        dblSIT_KEN = 0
                        dblSIT_KIN = 0
                        strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                        strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                        intGYO = 41
                        Exit Do
                    End If
                    strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                    strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                    dblSIT_KEN += 1
                    dblSIT_KIN = dblSIT_KIN + Val(strF_FURIKIN)
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '-----------------------------------
                    'エラーチェック
                    '-----------------------------------
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If
                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            '小計を書き込む
            ev.Graphics.DrawString("小計", printFont, Brushes.Black, 670, 730)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 730)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 730)
            ev.Graphics.DrawString(Format(dblSIT_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblSIT_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
            ev.Graphics.DrawString(Format(dblSIT_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblSIT_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
    Private Sub pd_UKETUKE_MEISAI_DETAIL_ERRPage_KOKUZEI(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAIL_ERRPage_KOKUZEI
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（エラー分のみ）
        'Return         :
        'Create         :2004/12/07
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If
        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_390, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_390.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_390.strDATA.Substring(0, 1)
                Case "1"
                    '2007/04/24　国税フォーマットのデータ部は'3'
                    'Case "2"
                Case "3"
                    FileGet(intFILE_NO_2, gKOKUZEI_REC3, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gKOKUZEI_REC3.KZ4.Substring(0, 4)
                    strF_KEIYAKU_SIT = gKOKUZEI_REC3.KZ4.Substring(4, 3)
                    strF_KEIYAKU_KAMOKU = gKOKUZEI_REC3.KZ11
                    strF_KEIYAKU_KOUZA = gKOKUZEI_REC3.KZ12
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gKOKUZEI_REC3.KZ30.Trim & gKOKUZEI_REC3.KZ31.Trim), 30)
                    strF_FURIKIN = gKOKUZEI_REC3.KZ9
                    strF_SINKI_NO = "0"
                    strF_KEIYAKU_NO = gKOKUZEI_REC3.KZ5
                    strF_JYUYOKA_NO = gKOKUZEI_REC3.KZ20 & gKOKUZEI_REC3.KZ21
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strYOMIKAE_KIN_NO, strYOMIKAE_SIT_NO) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If
                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '☆☆エラーチェックを追加する
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If X_MEI(1) = "" Then
                        Exit Select
                    End If
                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If
                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1030, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
#End Region

    '蒲郡信金向け
#Region "岡崎市税（300バイト）"
    Public Sub fn_UKETUKE_MEISAI_PRINT_ZEIKIN300(ByVal strFILE_NAME As String, ByVal strTORI_CODE As String, ByVal strFURI_DATE As String, ByVal intREC_LENGTH As Integer, ByVal strTUUBAN As String, ByRef blnKEKKA As Boolean)
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_PRINT_ZEIKIN300
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長／strTUUBAN：通番
        '               :blnKEKKA：結果
        'Description    :受付明細表印刷メイン処理(岡崎市税（300バイト）)
        'Return         :
        'Create         :2007/07/02
        'Update         :
        '============================================================================
        blnKEKKA = False
        'fn_UKETUKE_MEISAI_PRINT = False
        intF_REC_LENGTH = intREC_LENGTH
        strTORIS_CODE = strTORI_CODE.Substring(0, 7)
        strTORIF_CODE = strTORI_CODE.Substring(7, 2)

        gstrSYORI_R = "受付明細表印刷"
        Dim pd As New System.Drawing.Printing.PrintDocument

        '------------------------------------------
        'ログファイル作成ディレクトリの取得
        '------------------------------------------
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "LOG"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのLOGディレクトリの取得に失敗しました", "明細表印刷")
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrLOG_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "DATBK"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのDATBKディレクトリの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrDATBK_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "TENPO"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの支店読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "支店読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrTENPO_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "KOUZA"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの口座読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "口座読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrKOUZA_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "KINKOCD"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの自金庫コードの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "自金庫コード取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrJIKINKO = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "UKETUKECPY"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrUKETUKECPY = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        '蒲郡信金用　2007/07/27
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "KINKONAME"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのKINKONAMEの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            strKINKO_NAME = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If
        '帳票区分セット
        intTYOHYO_KBN = 0


        'ファイルを開き、1企業ごとのファイルを作成し、ソート処理対象の取引先はソート処理を行う。
        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        gdbcCONNECT.Open()
        Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
        dblRECORD_COUNT = 0
        intFILE_SEQ = 0
        strDATA_KBN = "0"
        dblALL_KEN = 0
        dblALL_KIN = 0
        intGYO = 1
        intMULTI_FLG = 0
        intFILE_NO_1 = FreeFile()
        '2006/06/28 マルチ再振データ作成対象のために追加
        gstrFURI_DATE = strFURI_DATE

        FileOpen(intFILE_NO_1, strFILE_NAME, OpenMode.Random, , , intREC_LENGTH)
        If Err.Number = 0 Then
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "成功", Err.Description)
        Else
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "失敗", Err.Description)
            Exit Sub
        End If
        strF_FURI_DATE = strFURI_DATE
        Do Until EOF(intFILE_NO_1)

            dblRECORD_COUNT += 1
            Try
                FileGet(intFILE_NO_1, gstrDATA_300, dblRECORD_COUNT)
                strFURI_DATA = gstrDATA_300.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_300.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    'ワーク用のファイルを開き、書き込む
                    intFILE_SEQ += 1
                    strWRK_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".DAT"
                    If Dir(strWRK_FILE) <> "" Then
                        Kill(strWRK_FILE)
                    End If
                    intFILE_NO_2 = FreeFile()

                    dblRECORD_SEQ = 0

                    FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Append, , , intREC_LENGTH)   '出力ファイル
                    Print(intFILE_NO_2, gstrDATA_300.strDATA)

                    If strDATA_KBN <> "0" And strDATA_KBN <> "8" And strDATA_KBN <> "9" Then
                        Exit Sub
                    End If
                Case "2"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_300.strDATA)
                Case "8"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_300.strDATA)
                    FileClose(intFILE_NO_2)

                    Dim prDialog As New PrintDialog

                    '1企業のファイルが作成されたので、そのファイルを読み込み明細表印刷
                    intKAISUU_FLG = 0
                    If fn_MEISAI_KOBETU_PRINT_ZEIKIN300() = False Then
                        pd.PrinterSettings.PrintToFile = False
                        gdbcCONNECT.Close()
                        Exit Sub
                    End If

                Case "9"



            End Select
        Loop
        FileClose(intFILE_NO_1)
        gdbcCONNECT.Close()
        blnKEKKA = True

    End Sub
    Public Function fn_MEISAI_KOBETU_PRINT_ZEIKIN300() As Boolean
        '============================================================================
        'NAME           :fn_MEISAI_KOBETU_PRINT_ZEIKIN300
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長
        'Description    :受付明細表印刷メイン処理(岡崎市税（300バイト）)
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2007/07/02
        'Update         :2007/09/13 蒲郡信金向け 他行元受マルチファイル対応
        '============================================================================
        fn_MEISAI_KOBETU_PRINT_ZEIKIN300 = False
        Dim intFILE_NO_2 As Integer
        Dim dblRECORD_COUNT_WRK As Double
        intFILE_NO_2 = FreeFile()
        intGYO = 0
        intPAGE = 0
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)

        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_300, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_300.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Function
            End Try

            Select Case gstrDATA_300.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    FileGet(intFILE_NO_2, gSHIZEI_REC1, dblRECORD_COUNT_WRK)
                    strF_ITAKU_CODE = gSHIZEI_REC1.SZ4
                    '取引先コードの取得
                    gstrSSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND TORIF_CODE_T = '" & Trim(strTORIF_CODE) & "'"

                    gdbCOMMAND = New OracleClient.OracleCommand
                    gdbCOMMAND.CommandText = gstrSSQL
                    gdbCOMMAND.Connection = gdbcCONNECT

                    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                    While (gdbrREADER.Read)

                        strITAKU_CODE = gdbrREADER.Item("ITAKU_CODE_T")
                        strMULTI_KBN = gdbrREADER.Item("MULTI_KBN_T") '2007/09/13
                        If strF_ITAKU_CODE <> strITAKU_CODE Then
                            If strMULTI_KBN = "4" Then '他行元受マルチファイル対応　2007/09/13
                                gstrSSQL = "SELECT * FROM TORIMAST,SCHMAST WHERE ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            Else
                                gstrSSQL = "SELECT * FROM TORIMAST,SCHMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            End If
                            gstrSSQL = gstrSSQL & " AND FURI_DATE_S = '" & Trim(gstrFURI_DATE) & "' AND YUUKOU_FLG_S = '1'"
                            gstrSSQL = gstrSSQL & " AND TORIS_CODE_T = TORIS_CODE_S AND TORIF_CODE_T = TORIF_CODE_S"

                            'gstrSSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            gdbCOMMAND.CommandText = gstrSSQL
                            gdbCOMMAND.Connection = gdbcCONNECT

                            gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                            If gdbrREADER.Read = False Then
                                '一致する取引先コードが存在しない
                                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                                Exit Function
                            End If
                        End If
                        If strMULTI_KBN = "4" Then '他行元受マルチファイル対応　2007/09/13
                            strTORIS_CODE = gdbrREADER.Item("TORIS_CODE_T")
                        End If
                        strTORIF_CODE = gdbrREADER.Item("TORIF_CODE_T")
                        strITAKU_NNAME = gdbrREADER.Item("ITAKU_NNAME_T")
                        strTKIN_NO = gdbrREADER.Item("TKIN_NO_T")
                        strTSIT_NO = gdbrREADER.Item("TSIT_NO_T")
                        strCODE_KBN = gdbrREADER.Item("CODE_KBN_T")
                        'strMULTI_KBN = gdbrREADER.Item("MOTIKOMI_KBN_T")
                        strUKETUKE_KBN = gdbrREADER.Item("UMEISAI_KBN_T")
                    End While
                    If fn_Select_TENMAST(strTKIN_NO, strTSIT_NO, strITAKU_KIN_NNAME, strITAKU_SIT_NNAME, strITAKU_KIN_KNAME, strITAKU_SIT_KNAME) = False Then

                    End If
                    If Err.Number <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                        Exit Function
                    End If
                    intPAGE = 1
                    If intFILE_SEQ = 1 Then
                        Call fn_UKETUKE_MEISAI_HEDDA()
                    End If

                    Exit Do


            End Select


        Loop
        FileClose(intFILE_NO_2)
        dblALL_KEN = 0
        dblALL_KIN = 0
        If fn_UKETUKE_MEISAI_DETAIL_ZEIKIN300() = False Then
            Exit Function
        End If
        fn_MEISAI_KOBETU_PRINT_ZEIKIN300 = True
    End Function
    Public Function fn_UKETUKE_MEISAI_DETAIL_ZEIKIN300() As Boolean
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_DETAIL_ZEIKIN300
        'Parameter      :
        'Description    :受付明細表データ部印刷(岡崎市税・300バイト)
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2007/07/02
        'Update         :
        '============================================================================
        fn_UKETUKE_MEISAI_DETAIL_ZEIKIN300 = False
        '------------------------------------------------------------------
        '受付明細表印刷区分により印刷ロジック分岐
        '------------------------------------------------------------------
        dblRECORD_COUNT_WRK = 0
        Try
            Select Case strUKETUKE_KBN
                Case "0"        '非対称
                    fn_UKETUKE_MEISAI_DETAIL_ZEIKIN300 = True
                    Exit Function
                Case "1"        '対象（店番ソート）
                    '------------------------------------------------
                    'ファイルソート（POWER SORT）
                    '------------------------------------------------

                    strSORT_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".SRT"
                    If Dir(strSORT_FILE) <> "" Then
                        Kill(strSORT_FILE)
                    End If
                    frmREPORT.objAxPowerSORT.DispMessage = False
                    frmREPORT.objAxPowerSORT.DisposalNumber = 0
                    frmREPORT.objAxPowerSORT.FieldDefinition = 1
                    frmREPORT.objAxPowerSORT.KeyCmdStr = "0.1asca 1.4asca 20.3asca 42.1asca 43.7asca"
                    frmREPORT.objAxPowerSORT.InputFiles = strWRK_FILE
                    frmREPORT.objAxPowerSORT.InputFileType = 1
                    frmREPORT.objAxPowerSORT.OutputFile = strSORT_FILE
                    frmREPORT.objAxPowerSORT.OutputFileType = 1
                    frmREPORT.objAxPowerSORT.MaxRecordLength = 300
                    frmREPORT.objAxPowerSORT.Action()

                    If frmREPORT.objAxPowerSORT.ErrorCode <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ファイルソート", "失敗", frmREPORT.objAxPowerSORT.ErrorDetail)
                        Exit Function
                    End If
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZEIKIN300
                Case "2"        '対象（非ソート）
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAILPage_ZEIKIN300
                Case "3"        'エラー分のみ
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZEIKIN300

            End Select
            'ページを横に設定
            pd.DefaultPageSettings.Landscape = True

            '印刷を開始する
            intMULTI_FLG += 1
            pd.DefaultPageSettings.PrinterSettings.Collate = True
            pd.DefaultPageSettings.PrinterSettings.Copies = Val(gstrUKETUKECPY)
            pd.Print()
        Catch ex As Exception
            Exit Function
        End Try
        If Dir(strWRK_FILE) <> "" Then
            Kill(strWRK_FILE)
        End If
        If strUKETUKE_KBN = "1" Then
            If Dir(strSORT_FILE) <> "" Then
                Kill(strSORT_FILE)
            End If
        End If
        fn_UKETUKE_MEISAI_DETAIL_ZEIKIN300 = True

    End Function
    Private Sub pd_UKETUKE_MEISAI_DETAILPage_ZEIKIN300(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAILPage_ZEIKIN300
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（ソート非対象）
        'Return         :
        'Create         :2007/07/02
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If
        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        'Dim X_ITI() As Integer = {100, 150, 260, 400, 615, 675, 760, 1070} '見出し位置
        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_300, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_300.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_300.strDATA.Substring(0, 1)
                Case "1"
                Case "2"

                    FileGet(intFILE_NO_2, gSHIZEI_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gSHIZEI_REC2.SZ2
                    strF_KEIYAKU_SIT = gSHIZEI_REC2.SZ4
                    strF_KEIYAKU_KAMOKU = gSHIZEI_REC2.SZ7
                    strF_KEIYAKU_KOUZA = gSHIZEI_REC2.SZ8
                    If Trim(gSHIZEI_REC2.SZ9).Length > 30 Then
                        strF_KEIYAKU_KNAME = Trim(gSHIZEI_REC2.SZ9).Substring(0, 30)
                    Else
                        strF_KEIYAKU_KNAME = Trim(gSHIZEI_REC2.SZ9)
                    End If
                    strF_FURIKIN = gSHIZEI_REC2.SZ10
                    strF_SINKI_NO = gSHIZEI_REC2.SZ11
                    strF_KEIYAKU_NO = gSHIZEI_REC2.SZ14 '暫定対応
                    strF_JYUYOKA_NO = gSHIZEI_REC2.SZ14
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strYOMIKAE_KIN_NO, strYOMIKAE_SIT_NO) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If

                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '-----------------------------------
                    'エラーチェック
                    '-----------------------------------
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If

                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
    Private Sub pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZEIKIN300(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZEIKIN300
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（店番ソート）
        'Return         :
        'Create         :2007/07/02
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If

        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strSORT_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_300, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_300.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_300.strDATA.Substring(0, 1)
                Case "1"
                    strMAE_KEIYAKU_KIN = Nothing
                    strMAE_KEIYAKU_SIT = Nothing
                    dblSIT_KEN = 0
                    dblSIT_KIN = 0
                Case "2"

                    FileGet(intFILE_NO_2, gSHIZEI_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gSHIZEI_REC2.SZ2
                    strF_KEIYAKU_SIT = gSHIZEI_REC2.SZ4
                    strF_KEIYAKU_KAMOKU = gSHIZEI_REC2.SZ7
                    strF_KEIYAKU_KOUZA = gSHIZEI_REC2.SZ8
                    If Trim(gSHIZEI_REC2.SZ9).Length > 30 Then
                        strF_KEIYAKU_KNAME = Trim(gSHIZEI_REC2.SZ9).Substring(0, 30)
                    Else
                        strF_KEIYAKU_KNAME = Trim(gSHIZEI_REC2.SZ9)
                    End If
                    strF_FURIKIN = gSHIZEI_REC2.SZ10
                    strF_SINKI_NO = gSHIZEI_REC2.SZ11
                    strF_KEIYAKU_NO = gSHIZEI_REC2.SZ14 '暫定対応
                    strF_JYUYOKA_NO = gSHIZEI_REC2.SZ14
                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    If strMAE_KEIYAKU_KIN = Nothing And strMAE_KEIYAKU_SIT = Nothing Then
                        strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                        strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                    End If
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strYOMIKAE_KIN_NO, strYOMIKAE_SIT_NO) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If
                    If strF_KEIYAKU_KIN <> strMAE_KEIYAKU_KIN Or strF_KEIYAKU_SIT <> strMAE_KEIYAKU_SIT Then
                        dblRECORD_COUNT_WRK -= 1
                        '小計を書き込む
                        ev.Graphics.DrawString("小計", printFont, Brushes.Black, 670, 730)
                        ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 730)
                        ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 730)
                        ev.Graphics.DrawString(Format(dblSIT_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblSIT_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
                        ev.Graphics.DrawString(Format(dblSIT_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblSIT_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
                        dblSIT_KEN = 0
                        dblSIT_KIN = 0
                        strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                        strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                        intGYO = 41
                        Exit Do
                    End If
                    strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                    strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                    dblSIT_KEN += 1
                    dblSIT_KIN = dblSIT_KIN + Val(strF_FURIKIN)
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '-----------------------------------
                    'エラーチェック
                    '-----------------------------------
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If

                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            '小計を書き込む
            ev.Graphics.DrawString("小計", printFont, Brushes.Black, 670, 730)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 730)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 730)
            ev.Graphics.DrawString(Format(dblSIT_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblSIT_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
            ev.Graphics.DrawString(Format(dblSIT_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblSIT_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
    Private Sub pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZEIKIN300(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZEIKIN300
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（エラー分のみ）
        'Return         :
        'Create         :2007/07/02
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If
        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_300, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_300.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_300.strDATA.Substring(0, 1)
                Case "1"
                Case "2"

                    FileGet(intFILE_NO_2, gSHIZEI_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gSHIZEI_REC2.SZ2
                    strF_KEIYAKU_SIT = gSHIZEI_REC2.SZ4
                    strF_KEIYAKU_KAMOKU = gSHIZEI_REC2.SZ7
                    strF_KEIYAKU_KOUZA = gSHIZEI_REC2.SZ8
                    If Trim(gSHIZEI_REC2.SZ9).Length > 30 Then
                        strF_KEIYAKU_KNAME = Trim(gSHIZEI_REC2.SZ9).Substring(0, 30)
                    Else
                        strF_KEIYAKU_KNAME = Trim(gSHIZEI_REC2.SZ9)
                    End If
                    strF_FURIKIN = gSHIZEI_REC2.SZ10
                    strF_SINKI_NO = gSHIZEI_REC2.SZ11
                    strF_KEIYAKU_NO = gSHIZEI_REC2.SZ14 '暫定対応
                    strF_JYUYOKA_NO = gSHIZEI_REC2.SZ14
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strYOMIKAE_KIN_NO, strYOMIKAE_SIT_NO) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If
                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '☆☆エラーチェックを追加する
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If X_MEI(1) = "" Then
                        Exit Select
                    End If
                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If

                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1030, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
#End Region

#Region "地公体（350バイト）"
    Public Sub fn_UKETUKE_MEISAI_PRINT_ZEIKIN350(ByVal strFILE_NAME As String, ByVal strTORI_CODE As String, ByVal strFURI_DATE As String, ByVal intREC_LENGTH As Integer, ByVal strTUUBAN As String, ByRef blnKEKKA As Boolean)
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_PRINT_ZEIKIN350
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長／strTUUBAN：通番
        '               :blnKEKKA：結果
        'Description    :受付明細表印刷メイン処理(地公体（350バイト）)
        'Return         :
        'Create         :2005/07/26
        'Update         :
        '============================================================================
        blnKEKKA = False
        'fn_UKETUKE_MEISAI_PRINT = False
        intF_REC_LENGTH = intREC_LENGTH
        strTORIS_CODE = strTORI_CODE.Substring(0, 7)
        strTORIF_CODE = strTORI_CODE.Substring(7, 2)

        gstrSYORI_R = "受付明細表印刷"
        Dim pd As New System.Drawing.Printing.PrintDocument

        '------------------------------------------
        'ログファイル作成ディレクトリの取得
        '------------------------------------------
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "LOG"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのLOGディレクトリの取得に失敗しました", "明細表印刷")
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrLOG_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "DATBK"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのDATBKディレクトリの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrDATBK_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "TENPO"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの支店読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "支店読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrTENPO_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "KOUZA"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの口座読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "口座読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrKOUZA_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "KINKOCD"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの自金庫コードの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "自金庫コード取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrJIKINKO = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "UKETUKECPY"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrUKETUKECPY = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        '蒲郡信金用　2007/07/27
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "KINKONAME"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのKINKONAMEの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            strKINKO_NAME = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If
        '帳票区分セット
        intTYOHYO_KBN = 0

        'ファイルを開き、1企業ごとのファイルを作成し、ソート処理対象の取引先はソート処理を行う。
        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        gdbcCONNECT.Open()
        Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
        dblRECORD_COUNT = 0
        intFILE_SEQ = 0
        strDATA_KBN = "0"
        dblALL_KEN = 0
        dblALL_KIN = 0
        intGYO = 1
        intMULTI_FLG = 0
        intFILE_NO_1 = FreeFile()
        '2006/06/28 マルチ再振データ作成対象のために追加
        gstrFURI_DATE = strFURI_DATE

        FileOpen(intFILE_NO_1, strFILE_NAME, OpenMode.Random, , , intREC_LENGTH)
        If Err.Number = 0 Then
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "成功", Err.Description)
        Else
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "失敗", Err.Description)
            Exit Sub
        End If
        strF_FURI_DATE = strFURI_DATE
        Do Until EOF(intFILE_NO_1)

            dblRECORD_COUNT += 1
            Try
                FileGet(intFILE_NO_1, gstrDATA_350, dblRECORD_COUNT)
                strFURI_DATA = gstrDATA_350.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_350.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    'ワーク用のファイルを開き、書き込む
                    intFILE_SEQ += 1
                    strWRK_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".DAT"
                    If Dir(strWRK_FILE) <> "" Then
                        Kill(strWRK_FILE)
                    End If
                    intFILE_NO_2 = FreeFile()

                    dblRECORD_SEQ = 0

                    FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Append, , , intREC_LENGTH)   '出力ファイル
                    Print(intFILE_NO_2, gstrDATA_350.strDATA)

                    If strDATA_KBN <> "0" And strDATA_KBN <> "8" And strDATA_KBN <> "9" Then
                        Exit Sub
                    End If
                Case "2"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_350.strDATA)
                Case "8"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_350.strDATA)
                    FileClose(intFILE_NO_2)

                    Dim prDialog As New PrintDialog

                    '1企業のファイルが作成されたので、そのファイルを読み込み明細表印刷
                    intKAISUU_FLG = 0
                    If fn_MEISAI_KOBETU_PRINT_ZEIKIN350() = False Then
                        pd.PrinterSettings.PrintToFile = False
                        gdbcCONNECT.Close()
                        Exit Sub
                    End If

                Case "9"



            End Select
        Loop
        FileClose(intFILE_NO_1)
        gdbcCONNECT.Close()
        blnKEKKA = True

    End Sub
    Public Function fn_MEISAI_KOBETU_PRINT_ZEIKIN350() As Boolean
        '============================================================================
        'NAME           :fn_MEISAI_KOBETU_PRINT_ZEIKIN350
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長
        'Description    :受付明細表印刷メイン処理(地公体（350バイト）)
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2005/07/26
        'Update         :2007/09/13 蒲郡信金向け 他行元受マルチファイル対応
        '============================================================================
        fn_MEISAI_KOBETU_PRINT_ZEIKIN350 = False
        Dim intFILE_NO_2 As Integer
        Dim dblRECORD_COUNT_WRK As Double
        intFILE_NO_2 = FreeFile()
        intGYO = 0
        intPAGE = 0
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)

        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_350, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_350.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Function
            End Try

            Select Case gstrDATA_350.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    FileGet(intFILE_NO_2, gZEIKIN350_REC1, dblRECORD_COUNT_WRK)
                    strF_ITAKU_CODE = gZEIKIN350_REC1.ZK4 & gZEIKIN350_REC1.ZK5
                    '取引先コードの取得
                    gstrSSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND TORIF_CODE_T = '" & Trim(strTORIF_CODE) & "'"

                    gdbCOMMAND = New OracleClient.OracleCommand
                    gdbCOMMAND.CommandText = gstrSSQL
                    gdbCOMMAND.Connection = gdbcCONNECT

                    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                    While (gdbrREADER.Read)

                        strITAKU_CODE = gdbrREADER.Item("ITAKU_CODE_T")
                        strMULTI_KBN = gdbrREADER.Item("MULTI_KBN_T") '2007/09/13
                        If strF_ITAKU_CODE <> strITAKU_CODE Then
                            If strMULTI_KBN = "4" Then '他行元受マルチファイル対応　2007/09/13
                                gstrSSQL = "SELECT * FROM TORIMAST,SCHMAST WHERE ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            Else
                                gstrSSQL = "SELECT * FROM TORIMAST,SCHMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            End If
                            gstrSSQL = gstrSSQL & " AND FURI_DATE_S = '" & Trim(gstrFURI_DATE) & "' AND YUUKOU_FLG_S = '1'"
                            gstrSSQL = gstrSSQL & " AND TORIS_CODE_T = TORIS_CODE_S AND TORIF_CODE_T = TORIF_CODE_S"

                            'gstrSSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            gdbCOMMAND.CommandText = gstrSSQL
                            gdbCOMMAND.Connection = gdbcCONNECT

                            gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                            If gdbrREADER.Read = False Then
                                '一致する取引先コードが存在しない
                                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                                Exit Function
                            End If
                        End If
                        If strMULTI_KBN = "4" Then '他行元受マルチファイル対応　2007/09/13
                            strTORIS_CODE = gdbrREADER.Item("TORIS_CODE_T")
                        End If
                        strTORIF_CODE = gdbrREADER.Item("TORIF_CODE_T")
                        strITAKU_NNAME = gdbrREADER.Item("ITAKU_NNAME_T")
                        strTKIN_NO = gdbrREADER.Item("TKIN_NO_T")
                        strTSIT_NO = gdbrREADER.Item("TSIT_NO_T")
                        strCODE_KBN = gdbrREADER.Item("CODE_KBN_T")
                        'strMULTI_KBN = gdbrREADER.Item("MOTIKOMI_KBN_T")
                        strUKETUKE_KBN = gdbrREADER.Item("UMEISAI_KBN_T")
                    End While
                    If fn_Select_TENMAST(strTKIN_NO, strTSIT_NO, strITAKU_KIN_NNAME, strITAKU_SIT_NNAME, strITAKU_KIN_KNAME, strITAKU_SIT_KNAME) = False Then

                    End If
                    If Err.Number <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                        Exit Function
                    End If
                    intPAGE = 1
                    If intFILE_SEQ = 1 Then
                        Call fn_UKETUKE_MEISAI_HEDDA()
                    End If

                    Exit Do


            End Select


        Loop
        FileClose(intFILE_NO_2)
        dblALL_KEN = 0
        dblALL_KIN = 0
        If fn_UKETUKE_MEISAI_DETAIL_ZEIKIN350() = False Then
            Exit Function
        End If
        fn_MEISAI_KOBETU_PRINT_ZEIKIN350 = True
    End Function
    Public Function fn_UKETUKE_MEISAI_DETAIL_ZEIKIN350() As Boolean
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_DETAIL_ZEIKIN350
        'Parameter      :
        'Description    :受付明細表データ部印刷(全銀)
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2005/07/26
        'Update         :
        '============================================================================
        fn_UKETUKE_MEISAI_DETAIL_ZEIKIN350 = False
        '------------------------------------------------------------------
        '受付明細表印刷区分により印刷ロジック分岐
        '------------------------------------------------------------------
        dblRECORD_COUNT_WRK = 0
        Try
            Select Case strUKETUKE_KBN
                Case "0"        '非対称
                    fn_UKETUKE_MEISAI_DETAIL_ZEIKIN350 = True
                    Exit Function
                Case "1"        '対象（店番ソート）
                    '------------------------------------------------
                    'ファイルソート（POWER SORT）
                    '------------------------------------------------

                    strSORT_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".SRT"
                    If Dir(strSORT_FILE) <> "" Then
                        Kill(strSORT_FILE)
                    End If
                    frmREPORT.objAxPowerSORT.DispMessage = False
                    frmREPORT.objAxPowerSORT.DisposalNumber = 0
                    frmREPORT.objAxPowerSORT.FieldDefinition = 1
                    frmREPORT.objAxPowerSORT.KeyCmdStr = "0.1asca 1.4asca 20.3asca 42.1asca 43.7asca"
                    frmREPORT.objAxPowerSORT.InputFiles = strWRK_FILE
                    frmREPORT.objAxPowerSORT.InputFileType = 1
                    frmREPORT.objAxPowerSORT.OutputFile = strSORT_FILE
                    frmREPORT.objAxPowerSORT.OutputFileType = 1
                    frmREPORT.objAxPowerSORT.MaxRecordLength = 350
                    frmREPORT.objAxPowerSORT.Action()

                    If frmREPORT.objAxPowerSORT.ErrorCode <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ファイルソート", "失敗", frmREPORT.objAxPowerSORT.ErrorDetail)
                        Exit Function
                    End If
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZEIKIN350
                Case "2"        '対象（非ソート）
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAILPage_ZEIKIN350
                Case "3"        'エラー分のみ
                    'イベントハンドラの追加
                    AddHandler pd.PrintPage, AddressOf pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZEIKIN350

            End Select
            'ページを横に設定
            pd.DefaultPageSettings.Landscape = True

            '印刷を開始する
            intMULTI_FLG += 1
            pd.DefaultPageSettings.PrinterSettings.Collate = True
            pd.DefaultPageSettings.PrinterSettings.Copies = Val(gstrUKETUKECPY)
            pd.Print()
        Catch ex As Exception
            Exit Function
        End Try
        If Dir(strWRK_FILE) <> "" Then
            Kill(strWRK_FILE)
        End If
        If strUKETUKE_KBN = "1" Then
            If Dir(strSORT_FILE) <> "" Then
                Kill(strSORT_FILE)
            End If
        End If
        fn_UKETUKE_MEISAI_DETAIL_ZEIKIN350 = True

    End Function
    Private Sub pd_UKETUKE_MEISAI_DETAILPage_ZEIKIN350(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAILPage_ZEIKIN350
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（ソート非対象）
        'Return         :
        'Create         :2005/07/26
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If
        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        'Dim X_ITI() As Integer = {100, 150, 260, 400, 615, 675, 760, 1070} '見出し位置
        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_350, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_350.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_350.strDATA.Substring(0, 1)
                Case "1"
                Case "2"

                    FileGet(intFILE_NO_2, gZEIKIN350_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gZEIKIN350_REC2.ZK2
                    strF_KEIYAKU_SIT = gZEIKIN350_REC2.ZK4
                    strF_KEIYAKU_KAMOKU = gZEIKIN350_REC2.ZK7
                    strF_KEIYAKU_KOUZA = gZEIKIN350_REC2.ZK8
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gZEIKIN350_REC2.ZK9), 30)
                    strF_FURIKIN = gZEIKIN350_REC2.ZK10
                    strF_SINKI_NO = gZEIKIN350_REC2.ZK11
                    strF_KEIYAKU_NO = gZEIKIN350_REC2.ZK21
                    strF_JYUYOKA_NO = gZEIKIN350_REC2.ZK16
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strYOMIKAE_KIN_NO, strYOMIKAE_SIT_NO) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If

                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '-----------------------------------
                    'エラーチェック
                    '-----------------------------------
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If

                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
    Private Sub pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZEIKIN350(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAIL_SORTPage_ZEIKIN350
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（店番ソート）
        'Return         :
        'Create         :2005/07/26
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If

        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strSORT_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_350, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_350.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_350.strDATA.Substring(0, 1)
                Case "1"
                    strMAE_KEIYAKU_KIN = Nothing
                    strMAE_KEIYAKU_SIT = Nothing
                    dblSIT_KEN = 0
                    dblSIT_KIN = 0
                Case "2"

                    FileGet(intFILE_NO_2, gZEIKIN350_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gZEIKIN350_REC2.ZK2
                    strF_KEIYAKU_SIT = gZEIKIN350_REC2.ZK4
                    strF_KEIYAKU_KAMOKU = gZEIKIN350_REC2.ZK7
                    strF_KEIYAKU_KOUZA = gZEIKIN350_REC2.ZK8
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gZEIKIN350_REC2.ZK9), 30)
                    strF_FURIKIN = gZEIKIN350_REC2.ZK10
                    strF_SINKI_NO = gZEIKIN350_REC2.ZK11
                    strF_KEIYAKU_NO = gZEIKIN350_REC2.ZK21
                    strF_JYUYOKA_NO = gZEIKIN350_REC2.ZK16
                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    If strMAE_KEIYAKU_KIN = Nothing And strMAE_KEIYAKU_SIT = Nothing Then
                        strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                        strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                    End If
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strYOMIKAE_KIN_NO, strYOMIKAE_SIT_NO) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If
                    If strF_KEIYAKU_KIN <> strMAE_KEIYAKU_KIN Or strF_KEIYAKU_SIT <> strMAE_KEIYAKU_SIT Then
                        dblRECORD_COUNT_WRK -= 1
                        '小計を書き込む
                        ev.Graphics.DrawString("小計", printFont, Brushes.Black, 670, 730)
                        ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 730)
                        ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 730)
                        ev.Graphics.DrawString(Format(dblSIT_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblSIT_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
                        ev.Graphics.DrawString(Format(dblSIT_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblSIT_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
                        dblSIT_KEN = 0
                        dblSIT_KIN = 0
                        strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                        strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                        intGYO = 41
                        Exit Do
                    End If
                    strMAE_KEIYAKU_KIN = strF_KEIYAKU_KIN
                    strMAE_KEIYAKU_SIT = strF_KEIYAKU_SIT
                    dblSIT_KEN += 1
                    dblSIT_KIN = dblSIT_KIN + Val(strF_FURIKIN)
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '-----------------------------------
                    'エラーチェック
                    '-----------------------------------
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If

                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            '小計を書き込む
            ev.Graphics.DrawString("小計", printFont, Brushes.Black, 670, 730)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 730)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 730)
            ev.Graphics.DrawString(Format(dblSIT_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblSIT_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
            ev.Graphics.DrawString(Format(dblSIT_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblSIT_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 730)
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1000, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
    Private Sub pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZEIKIN350(ByVal sender As Object, ByVal ev As System.Drawing.Printing.PrintPageEventArgs)
        '============================================================================
        'NAME           :pd_UKETUKE_MEISAI_DETAIL_ERRPage_ZEIKIN350
        'Parameter      :
        'Description    :受付明細表データ部印刷詳細（エラー分のみ）
        'Return         :
        'Create         :2005/07/26
        'Update         :
        '============================================================================
        If intMULTI_FLG <> 1 Then
            If intKAISUU_FLG <> 0 Then
                If intKAISUU_FLG = intMULTI_FLG Then
                    intKAISUU_FLG = 1
                Else
                    intKAISUU_FLG += 1
                    Exit Sub
                End If
            Else
                intKAISUU_FLG += 1
            End If
        End If
        Dim intFILE_NO_2 As Integer
        intFILE_NO_2 = FreeFile()
        intGYO = 1
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)
        Dim printingPosition As Integer
        Dim printFont As Font

        printFont = New Font("ＭＳ Ｐ明朝", 10)

        Dim X_ITI() As Integer = {100, 120, 230, 370, 575, 595, 660, 970, 1000} '見出し位置
        Dim X_MEI(8) As String
        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_350, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_350.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_350.strDATA.Substring(0, 1)
                Case "1"
                Case "2"

                    FileGet(intFILE_NO_2, gZEIKIN350_REC2, dblRECORD_COUNT_WRK)
                    strF_KEIYAKU_KIN = gZEIKIN350_REC2.ZK2
                    strF_KEIYAKU_SIT = gZEIKIN350_REC2.ZK4
                    strF_KEIYAKU_KAMOKU = gZEIKIN350_REC2.ZK7
                    strF_KEIYAKU_KOUZA = gZEIKIN350_REC2.ZK8
                    strF_KEIYAKU_KNAME = Microsoft.VisualBasic.Left(Trim(gZEIKIN350_REC2.ZK9), 30)
                    strF_FURIKIN = gZEIKIN350_REC2.ZK10
                    strF_SINKI_NO = gZEIKIN350_REC2.ZK11
                    strF_KEIYAKU_NO = gZEIKIN350_REC2.ZK21
                    strF_JYUYOKA_NO = gZEIKIN350_REC2.ZK16
                    strYOMIKAE_KIN_NO = ""
                    strYOMIKAE_SIT_NO = ""
                    strYOMIKAE_KOUZA = ""
                    intYOMIKAE_FLG = 0
                    If gstrTENPO_YOMIKAE = 1 Then     '支店読み替え対象
                        If fn_TENPO_YOMIKAE(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strYOMIKAE_KIN_NO, strYOMIKAE_SIT_NO) = True Then
                            intYOMIKAE_FLG = 1
                        Else
                            strYOMIKAE_KIN_NO = ""
                            strYOMIKAE_SIT_NO = ""
                        End If
                    End If
                    If gstrKOUZA_YOMIKAE = 1 Then    '口座読み替え対象
                        If intYOMIKAE_FLG = 0 And strF_KEIYAKU_KIN = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strF_KEIYAKU_SIT, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO, strYOMIKAE_KOUZA) = True Then
                                intYOMIKAE_FLG = 1
                            Else
                                strYOMIKAE_SIT_NO = ""
                                strYOMIKAE_KOUZA = ""
                            End If
                        ElseIf intYOMIKAE_FLG = 1 And strYOMIKAE_KIN_NO = gstrJIKINKO Then
                            If fn_KOUZA_YOMIKAE(strYOMIKAE_SIT_NO, clsFusion.fn_CHG_KAMOKU1TO2(strF_KEIYAKU_KAMOKU), strF_KEIYAKU_KOUZA, strYOMIKAE_SIT_NO2, strYOMIKAE_KOUZA2) = True Then
                                intYOMIKAE_FLG = 1
                                strYOMIKAE_SIT_NO = strYOMIKAE_SIT_NO2
                                strYOMIKAE_KOUZA = strYOMIKAE_KOUZA2
                            End If
                        End If
                    End If
                    If intGYO = 41 Then
                        dblRECORD_COUNT_WRK -= 1
                        Exit Do
                    End If
                    dblALL_KEN += 1
                    dblALL_KIN = dblALL_KIN + Val(strF_FURIKIN)

                    dblRECORD_SEQ += 1
                    '印刷する文字列と位置を設定する
                    printingPosition = 0
                    '印刷に使うフォントを指定する
                    printFont = New Font("ＭＳ Ｐ明朝", 9.5)


                    X_MEI(0) = dblRECORD_SEQ
                    '☆☆エラーチェックを追加する
                    X_MEI(1) = fn_DATA_REC_CHK()
                    If intYOMIKAE_FLG = 1 Then
                        If X_MEI(1) <> "" Then
                            X_MEI(1) = X_MEI(1) & " & 読替"
                        Else
                            X_MEI(1) = "読替済み"
                        End If
                    End If

                    If X_MEI(1) = "" Then
                        Exit Select
                    End If
                    If fn_Select_TENMAST(strF_KEIYAKU_KIN, strF_KEIYAKU_SIT, strKIN_NNAME, strSIT_NNAME, strKIN_KNAME, strSIT_KNAME) = False Then
                        strKIN_NNAME = ""
                        strSIT_NNAME = ""
                    End If

                    If strKIN_NNAME.Length > 9 Then
                        strKIN_NNAME = strKIN_NNAME.Substring(0, 9)
                    End If
                    X_MEI(2) = strF_KEIYAKU_KIN & " " & strKIN_NNAME
                    X_MEI(3) = strF_KEIYAKU_SIT & " " & strSIT_NNAME
                    X_MEI(4) = strF_KEIYAKU_KAMOKU
                    X_MEI(5) = strF_KEIYAKU_KOUZA
                    X_MEI(6) = strF_KEIYAKU_KNAME
                    X_MEI(7) = Format(Val(strF_FURIKIN), "#,##0")
                    If intYOMIKAE_FLG = 1 And strYOMIKAE_SIT_NO <> "" Then
                        If strYOMIKAE_KIN_NO = "" Then
                            strYOMIKAE_KIN_NO = strF_KEIYAKU_KIN
                        End If
                        If strYOMIKAE_KOUZA = "" Then
                            strYOMIKAE_KOUZA = strF_KEIYAKU_KOUZA
                        End If
                    End If
                    X_MEI(8) = strYOMIKAE_KIN_NO & " " & strYOMIKAE_SIT_NO & " " & strYOMIKAE_KOUZA
                    Dim i As Integer
                    For i = 0 To 8
                        Select Case i
                            Case 0, 7    '数値項目
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i) - ((X_MEI(i).Length - 1)) * intMOJI_SIZE, 150 + intGYO * 14)
                            Case Else
                                ev.Graphics.DrawString(X_MEI(i), printFont, Brushes.Black, X_ITI(i), 150 + intGYO * 14)
                        End Select
                    Next
                    intGYO += 1
                Case "8"
                    intGYO = 1
            End Select

        Loop

        If intGYO = 41 Then
            intGYO = 1
            intPAGE += 1
            ev.HasMorePages = True
        Else
            'トレーラを書き込む
            ev.Graphics.DrawString("合計", printFont, Brushes.Black, 670, 750)
            ev.Graphics.DrawString("件", printFont, Brushes.Black, 800, 750)
            ev.Graphics.DrawString("円", printFont, Brushes.Black, 1030, 750)
            ev.Graphics.DrawString(Format(dblALL_KEN, "#,##0"), printFont, Brushes.Black, 780 - ((Format(dblALL_KEN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)
            ev.Graphics.DrawString(Format(dblALL_KIN, "#,##0"), printFont, Brushes.Black, 980 - ((Format(dblALL_KIN, "#,##0").Length - 1)) * intMOJI_SIZE, 750)

            ev.HasMorePages = False
        End If
        FileClose(intFILE_NO_2)

    End Sub
#End Region




    '総振用 2004/12/09
#Region "全銀"
    Public Sub fn_UKETUKE_MEISAI_PRINT_ZENGIN_K(ByVal strFILE_NAME As String, ByVal strTORI_CODE As String, ByVal strFURI_DATE As String, ByVal intREC_LENGTH As Integer, ByVal strTUUBAN As String, ByRef blnKEKKA As Boolean)
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_PRINT_ZENGIN_K
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長／strTUUBAN：通番
        '               :blnKEKKA：結果
        'Description    :受付明細表印刷メイン処理(全銀)&総振用
        'Return         :
        'Create         :2004/08/03
        'Update         :2004/12/09
        '============================================================================
        blnKEKKA = False
        'fn_UKETUKE_MEISAI_PRINT = False
        intF_REC_LENGTH = intREC_LENGTH
        strTORIS_CODE = strTORI_CODE.Substring(0, 7)
        strTORIF_CODE = strTORI_CODE.Substring(7, 2)

        gstrSYORI_R = "受付明細表印刷"
        Dim pd As New System.Drawing.Printing.PrintDocument

        '------------------------------------------
        'ログファイル作成ディレクトリの取得
        '------------------------------------------
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "LOG"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのLOGディレクトリの取得に失敗しました", "明細表印刷")
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrLOG_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "DATBK"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのDATBKディレクトリの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrDATBK_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "TENPO"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの支店読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "支店読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrTENPO_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "KOUZA"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの口座読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "口座読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrKOUZA_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "KINKOCD"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの自金庫コードの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "自金庫コード取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrJIKINKO = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "UKETUKECPY"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrUKETUKECPY = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        '蒲郡信金用　2007/07/27
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "KINKONAME"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのKINKONAMEの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            strKINKO_NAME = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If
        '帳票区分セット
        intTYOHYO_KBN = 1

        'ファイルを開き、1企業ごとのファイルを作成し、ソート処理対象の取引先はソート処理を行う。
        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        gdbcCONNECT.Open()
        Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
        dblRECORD_COUNT = 0
        intFILE_SEQ = 0
        strDATA_KBN = "0"
        dblALL_KEN = 0
        dblALL_KIN = 0
        intGYO = 1
        intMULTI_FLG = 0
        intFILE_NO_1 = FreeFile()
        '2006/06/28 マルチ再振データ作成対象のために追加
        gstrFURI_DATE = strFURI_DATE

        FileOpen(intFILE_NO_1, strFILE_NAME, OpenMode.Random, , , intREC_LENGTH)
        If Err.Number = 0 Then
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "成功", Err.Description)
        Else
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "失敗", Err.Description)
            Exit Sub
        End If
        strF_FURI_DATE = strFURI_DATE
        Do Until EOF(intFILE_NO_1)

            dblRECORD_COUNT += 1
            Try
                FileGet(intFILE_NO_1, gstrDATA, dblRECORD_COUNT)
                strFURI_DATA = gstrDATA.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    'ワーク用のファイルを開き、書き込む
                    intFILE_SEQ += 1
                    strWRK_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".DAT"
                    If Dir(strWRK_FILE) <> "" Then
                        Kill(strWRK_FILE)
                    End If
                    intFILE_NO_2 = FreeFile()

                    dblRECORD_SEQ = 0

                    FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Append, , , intREC_LENGTH)   '出力ファイル
                    Print(intFILE_NO_2, gstrDATA.strDATA)

                    If strDATA_KBN <> "0" And strDATA_KBN <> "8" And strDATA_KBN <> "9" Then
                        Exit Sub
                    End If
                Case "2"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA.strDATA)
                Case "8"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA.strDATA)
                    FileClose(intFILE_NO_2)

                    Dim prDialog As New PrintDialog

                    '1企業のファイルが作成されたので、そのファイルを読み込み明細表印刷
                    intKAISUU_FLG = 0

                    If fn_MEISAI_KOBETU_PRINT_ZENGIN_K() = False Then
                        pd.PrinterSettings.PrintToFile = False
                        gdbcCONNECT.Close()
                        Exit Sub
                    End If

                Case "9"



            End Select
        Loop
        FileClose(intFILE_NO_1)
        gdbcCONNECT.Close()
        blnKEKKA = True

    End Sub
    Public Function fn_MEISAI_KOBETU_PRINT_ZENGIN_K() As Boolean
        '============================================================================
        'NAME           :fn_MEISAI_KOBETU_PRINT_ZENGIN_K
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長
        'Description    :受付明細表印刷メイン処理&総振用
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2004/08/03
        'Update         :2004/12/09
        'Update         :2007/09/21 蒲郡信金向け 蒲郡市マルチファイル対応
        '============================================================================
        fn_MEISAI_KOBETU_PRINT_ZENGIN_K = False
        Dim intFILE_NO_2 As Integer
        Dim dblRECORD_COUNT_WRK As Double
        intFILE_NO_2 = FreeFile()
        intGYO = 0
        intPAGE = 0
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)

        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Function
            End Try

            Select Case gstrDATA.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    FileGet(intFILE_NO_2, gZENGIN_REC1, dblRECORD_COUNT_WRK)
                    strF_ITAKU_CODE = gZENGIN_REC1.ZG4
                    '取引先コードの取得
                    gstrSSQL = "SELECT * FROM S_TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND TORIF_CODE_T = '" & Trim(strTORIF_CODE) & "'"

                    gdbCOMMAND = New OracleClient.OracleCommand
                    gdbCOMMAND.CommandText = gstrSSQL
                    gdbCOMMAND.Connection = gdbcCONNECT

                    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                    While (gdbrREADER.Read)

                        strITAKU_CODE = gdbrREADER.Item("ITAKU_CODE_T")
                        strMULTI_KBN = gdbrREADER.Item("MULTI_KBN_T") '2007/09/13
                        If strF_ITAKU_CODE <> strITAKU_CODE Then
                            If strMULTI_KBN = "4" Then '蒲郡市マルチファイル対応　2007/09/21
                                gstrSSQL = "SELECT * FROM S_TORIMAST,S_SCHMAST WHERE ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            Else
                                gstrSSQL = "SELECT * FROM S_TORIMAST,S_SCHMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            End If
                            gstrSSQL = gstrSSQL & " AND FURI_DATE_S = '" & Trim(gstrFURI_DATE) & "' AND YUUKOU_FLG_S = '1'"
                            gstrSSQL = gstrSSQL & " AND TORIS_CODE_T = TORIS_CODE_S AND TORIF_CODE_T = TORIF_CODE_S"

                            'gstrSSQL = "SELECT * FROM S_TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            gdbCOMMAND.CommandText = gstrSSQL
                            gdbCOMMAND.Connection = gdbcCONNECT

                            gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                            If gdbrREADER.Read = False Then
                                '一致する取引先コードが存在しない
                                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                                Exit Function
                            End If
                        End If
                        If strMULTI_KBN = "4" Then '蒲郡市マルチファイル対応　2007/09/21
                            strTORIS_CODE = gdbrREADER.Item("TORIS_CODE_T")
                        End If
                        strTORIF_CODE = gdbrREADER.Item("TORIF_CODE_T")
                        strITAKU_NNAME = gdbrREADER.Item("ITAKU_NNAME_T")
                        strTKIN_NO = gdbrREADER.Item("TKIN_NO_T")
                        strTSIT_NO = gdbrREADER.Item("TSIT_NO_T")
                        strCODE_KBN = gdbrREADER.Item("CODE_KBN_T")
                        'strMULTI_KBN = gdbrREADER.Item("MOTIKOMI_KBN_T")
                        strUKETUKE_KBN = gdbrREADER.Item("UMEISAI_KBN_T")
                    End While
                    If fn_Select_TENMAST(strTKIN_NO, strTSIT_NO, strITAKU_KIN_NNAME, strITAKU_SIT_NNAME, strITAKU_KIN_KNAME, strITAKU_SIT_KNAME) = False Then

                    End If
                    If Err.Number <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                        Exit Function
                    End If
                    intPAGE = 1
                    If intFILE_SEQ = 1 Then
                        Call fn_UKETUKE_MEISAI_HEDDA()
                    End If

                    Exit Do


            End Select


        Loop
        FileClose(intFILE_NO_2)
        dblALL_KEN = 0
        dblALL_KIN = 0
        If fn_UKETUKE_MEISAI_DETAIL_ZENGIN() = False Then
            Exit Function
        End If
        fn_MEISAI_KOBETU_PRINT_ZENGIN_K = True
    End Function
#End Region
#Region "地公体"
    Public Sub fn_UKETUKE_MEISAI_PRINT_ZEIKIN_K(ByVal strFILE_NAME As String, ByVal strTORI_CODE As String, ByVal strFURI_DATE As String, ByVal intREC_LENGTH As Integer, ByVal strTUUBAN As String, ByRef blnKEKKA As Boolean)
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_PRINT_ZEIKIN_K
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長／strTUUBAN：通番
        '               :blnKEKKA：結果
        'Description    :受付明細表印刷メイン処理(地公体)&総振用
        'Return         :
        'Create         :2004/08/30
        'Update         :2004/12/09
        '============================================================================
        blnKEKKA = False
        intF_REC_LENGTH = intREC_LENGTH
        strTORIS_CODE = strTORI_CODE.Substring(0, 7)
        strTORIF_CODE = strTORI_CODE.Substring(7, 2)

        gstrSYORI_R = "受付明細表印刷"
        Dim pd As New System.Drawing.Printing.PrintDocument

        '------------------------------------------
        'ログファイル作成ディレクトリの取得
        '------------------------------------------
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "LOG"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのLOGディレクトリの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "LOGディレクトリ取得", "失敗", Err.Description)
            'fn_JOBMAST_UPDATE(strSTATUS_ERR, "LOGディレクトリ取得失敗")　　　'通番未取得なのでJOBMAST更新ができない
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrLOG_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "DATBK"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのDATBKディレクトリの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrDATBK_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "TENPO"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの支店読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "支店読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrTENPO_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "KOUZA"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの口座読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "口座読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrKOUZA_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "KINKOCD"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの自金庫コードの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "自金庫コード取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrJIKINKO = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "UKETUKECPY"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrUKETUKECPY = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If
        '蒲郡信金用　2007/07/27
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "KINKONAME"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのKINKONAMEの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            strKINKO_NAME = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If
        '帳票区分セット
        intTYOHYO_KBN = 1

        'ファイルを開き、1企業ごとのファイルを作成し、ソート処理対象の取引先はソート処理を行う。
        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        gdbcCONNECT.Open()
        Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
        dblRECORD_COUNT = 0
        intFILE_SEQ = 0
        strDATA_KBN = "0"
        dblALL_KEN = 0
        dblALL_KIN = 0
        intGYO = 1
        intMULTI_FLG = 0
        intFILE_NO_1 = FreeFile()
        '2006/06/28 マルチ再振データ作成対象のために追加
        gstrFURI_DATE = strFURI_DATE

        FileOpen(intFILE_NO_1, strFILE_NAME, OpenMode.Random, , , intREC_LENGTH)
        If Err.Number = 0 Then
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "成功", Err.Description)
        Else
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "失敗", Err.Description)
            Exit Sub
        End If
        strF_FURI_DATE = strFURI_DATE
        Do Until EOF(intFILE_NO_1)

            dblRECORD_COUNT += 1
            Try
                FileGet(intFILE_NO_1, gstrDATA_220, dblRECORD_COUNT)
                strFURI_DATA = gstrDATA_220.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_220.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    'ワーク用のファイルを開き、書き込む
                    intFILE_SEQ += 1
                    strWRK_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".DAT"
                    If Dir(strWRK_FILE) <> "" Then
                        Kill(strWRK_FILE)
                    End If
                    intFILE_NO_2 = FreeFile()

                    dblRECORD_SEQ = 0

                    FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Append, , , intREC_LENGTH)   '出力ファイル
                    Print(intFILE_NO_2, gstrDATA_220.strDATA)

                    If strDATA_KBN <> "0" And strDATA_KBN <> "8" And strDATA_KBN <> "9" Then
                        Exit Sub
                    End If
                Case "2"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_220.strDATA)
                Case "8"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_220.strDATA)
                    FileClose(intFILE_NO_2)

                    Dim prDialog As New PrintDialog

                    '1企業のファイルが作成されたので、そのファイルを読み込み明細表印刷
                    intKAISUU_FLG = 0
                    If fn_MEISAI_KOBETU_PRINT_ZEIKIN_K() = False Then
                        pd.PrinterSettings.PrintToFile = False
                        gdbcCONNECT.Close()
                        Exit Sub
                    End If
                Case "9"



            End Select
        Loop
        FileClose(intFILE_NO_1)
        gdbcCONNECT.Close()
        blnKEKKA = True

    End Sub
    Public Function fn_MEISAI_KOBETU_PRINT_ZEIKIN_K() As Boolean
        '============================================================================
        'NAME           :fn_MEISAI_KOBETU_PRINT_ZEIKIN_K
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長
        'Description    :受付明細表印刷メイン処理
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2004/08/30
        'Update         :
        '============================================================================
        fn_MEISAI_KOBETU_PRINT_ZEIKIN_K = False
        Dim intFILE_NO_2 As Integer
        Dim dblRECORD_COUNT_WRK As Double
        intFILE_NO_2 = FreeFile()
        intGYO = 0
        intPAGE = 0
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)

        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_220, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA_220.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Function
            End Try

            Select Case gstrDATA_220.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    FileGet(intFILE_NO_2, gZEIKIN_REC1, dblRECORD_COUNT_WRK)
                    strF_ITAKU_CODE = gZEIKIN_REC1.ZK4
                    '取引先コードの取得
                    gstrSSQL = "SELECT * FROM S_TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND TORIF_CODE_T = '" & Trim(strTORIF_CODE) & "'"

                    gdbCOMMAND = New OracleClient.OracleCommand
                    gdbCOMMAND.CommandText = gstrSSQL
                    gdbCOMMAND.Connection = gdbcCONNECT

                    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                    While (gdbrREADER.Read)

                        strITAKU_CODE = gdbrREADER.Item("ITAKU_CODE_T")
                        If strF_ITAKU_CODE <> strITAKU_CODE Then
                            gstrSSQL = "SELECT * FROM S_TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                            gdbCOMMAND.CommandText = gstrSSQL
                            gdbCOMMAND.Connection = gdbcCONNECT

                            gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                            If gdbrREADER.Read = False Then
                                '一致する取引先コードが存在しない
                                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                                Exit Function
                            End If
                        End If
                        strTORIF_CODE = gdbrREADER.Item("TORIF_CODE_T")
                        strITAKU_NNAME = gdbrREADER.Item("ITAKU_NNAME_T")
                        strTKIN_NO = gdbrREADER.Item("TKIN_NO_T")
                        strTSIT_NO = gdbrREADER.Item("TSIT_NO_T")
                        strCODE_KBN = gdbrREADER.Item("CODE_KBN_T")
                        strMULTI_KBN = gdbrREADER.Item("MOTIKOMI_KBN_T")
                        strUKETUKE_KBN = gdbrREADER.Item("UMEISAI_KBN_T")
                    End While
                    'gdbcCONNECT.Close()
                    If fn_Select_TENMAST(strTKIN_NO, strTSIT_NO, strITAKU_KIN_NNAME, strITAKU_SIT_NNAME, strITAKU_KIN_KNAME, strITAKU_SIT_KNAME) = False Then

                    End If
                    If Err.Number <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                        Exit Function
                    End If
                    intPAGE = 1
                    If intFILE_SEQ = 1 Then
                        Call fn_UKETUKE_MEISAI_HEDDA()
                    End If

                    Exit Do


            End Select


        Loop
        FileClose(intFILE_NO_2)
        dblALL_KEN = 0
        dblALL_KIN = 0
        If fn_UKETUKE_MEISAI_DETAIL_ZEIKIN() = False Then
            Exit Function
        End If

        fn_MEISAI_KOBETU_PRINT_ZEIKIN_K = True

    End Function
#End Region
#Region "国税"
    Public Sub fn_UKETUKE_MEISAI_PRINT_KOKUZEI_K(ByVal strFILE_NAME As String, ByVal strTORI_CODE As String, ByVal strFURI_DATE As String, ByVal intREC_LENGTH As Integer, ByVal strTUUBAN As String, ByRef blnKEKKA As Boolean)
        '============================================================================
        'NAME           :fn_UKETUKE_MEISAI_PRINT_KOKUZEI_K
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長／strTUUBAN：通番
        '               :blnKEKKA：結果
        'Description    :受付明細表印刷メイン処理(国税) & 総振用
        'Return         :
        'Create         :2004/12/07
        'Update         :2004/12/09
        '============================================================================
        blnKEKKA = False
        intF_REC_LENGTH = intREC_LENGTH
        strTORIS_CODE = strTORI_CODE.Substring(0, 7)
        strTORIF_CODE = strTORI_CODE.Substring(7, 2)

        gstrSYORI_R = "受付明細表印刷"
        Dim pd As New System.Drawing.Printing.PrintDocument

        '------------------------------------------
        'ログファイル作成ディレクトリの取得
        '------------------------------------------
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "LOG"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのLOGディレクトリの取得に失敗しました", "明細表印刷")
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrLOG_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "DATBK"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのDATBKディレクトリの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrDATBK_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "TENPO"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの支店読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "支店読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrTENPO_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "YOMIKAE"
        gstrIKeyName = "KOUZA"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの口座読替有無の取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "口座読替有無取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrKOUZA_YOMIKAE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "COMMON"
        gstrIKeyName = "KINKOCD"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからの自金庫コードの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "自金庫コード取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrJIKINKO = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "UKETUKECPY"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            gstrUKETUKECPY = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If

        '蒲郡信金用　2007/07/27
        gstrIFileName = CurDir()     'カレントディレクトリの取得
        gstrIFileName = gstrIFileName & "\FSKJ.INI"
        gstrIAppName = "PRINT"
        gstrIKeyName = "KINKONAME"
        gstrIDefault = "err"
        gintTEMP_LEN = 0
        gstrTEMP = Space(100)

        gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            MessageBox.Show("イニシャルファイルからのKINKONAMEの取得に失敗しました", "明細表印刷")
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
            Exit Sub
        Else
            '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            strKINKO_NAME = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        End If
        '帳票区分セット
        intTYOHYO_KBN = 1

        'ファイルを開き、1企業ごとのファイルを作成し、ソート処理対象の取引先はソート処理を行う。
        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        gdbcCONNECT.Open()
        Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
        dblRECORD_COUNT = 0
        intFILE_SEQ = 0
        strDATA_KBN = "0"
        dblALL_KEN = 0
        dblALL_KIN = 0
        intGYO = 1
        intMULTI_FLG = 0
        intFILE_NO_1 = FreeFile()
        '2006/06/28 マルチ再振データ作成対象のために追加
        gstrFURI_DATE = strFURI_DATE

        FileOpen(intFILE_NO_1, strFILE_NAME, OpenMode.Random, , , intREC_LENGTH)
        If Err.Number = 0 Then
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "成功", Err.Description)
        Else
            fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイルオープン", "失敗", Err.Description)
            Exit Sub
        End If
        strF_FURI_DATE = strFURI_DATE
        Do Until EOF(intFILE_NO_1)

            dblRECORD_COUNT += 1
            Try
                FileGet(intFILE_NO_1, gstrDATA_390, dblRECORD_COUNT)
                strFURI_DATA = gstrDATA_390.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strFURI_DATE, gstrSYORI_R, "ファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT & Err.Description)
                Exit Sub
            End Try

            Select Case gstrDATA_390.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    'ワーク用のファイルを開き、書き込む
                    intFILE_SEQ += 1
                    strWRK_FILE = gstrDATBK_OPENDIR & strTORIS_CODE & intFILE_SEQ & ".DAT"
                    If Dir(strWRK_FILE) <> "" Then
                        Kill(strWRK_FILE)
                    End If
                    intFILE_NO_2 = FreeFile()

                    dblRECORD_SEQ = 0

                    FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Append, , , intREC_LENGTH)   '出力ファイル
                    Print(intFILE_NO_2, gstrDATA_390.strDATA)

                    If strDATA_KBN <> "0" And strDATA_KBN <> "8" And strDATA_KBN <> "9" Then
                        Exit Sub
                    End If
                Case "3"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_390.strDATA)
                Case "8"
                    'ワーク用のファイルに書き込む
                    Print(intFILE_NO_2, gstrDATA_390.strDATA)
                    FileClose(intFILE_NO_2)

                    Dim prDialog As New PrintDialog

                    '1企業のファイルが作成されたので、そのファイルを読み込み明細表印刷
                    intKAISUU_FLG = 0
                    'Call fn_MEISAI_KOBETU_PRINT_KOKUZEI()
                    If fn_MEISAI_KOBETU_PRINT_KOKUZEI_K() = False Then
                        gdbcCONNECT.Close()
                        pd.PrinterSettings.PrintToFile = False
                        gdbcCONNECT.Close()
                        Exit Sub
                    End If
                Case "9"



            End Select
        Loop
        FileClose(intFILE_NO_1)
        gdbcCONNECT.Close()
        blnKEKKA = True

    End Sub
    Public Function fn_MEISAI_KOBETU_PRINT_KOKUZEI_K() As Boolean
        '============================================================================
        'NAME           :fn_MEISAI_KOBETU_PRINT_KOKUZEI_K
        'Parameter      :strFILE_NAME：読み込みファイル名／strTORIS_CODE：取引先主コード
        '               :strFURI_DATE：振替日／intREC_LENGTH：レコード長
        'Description    :受付明細表印刷メイン処理 & 総振用
        'Return         :True=OK,False=NG（異常発生）
        'Create         :2004/12/07
        'Update         :2004/12/09
        '============================================================================
        fn_MEISAI_KOBETU_PRINT_KOKUZEI_K = False
        Dim intFILE_NO_2 As Integer
        Dim dblRECORD_COUNT_WRK As Double
        intFILE_NO_2 = FreeFile()
        intGYO = 0
        intPAGE = 0
        FileOpen(intFILE_NO_2, strWRK_FILE, OpenMode.Random, , , intF_REC_LENGTH)

        Do Until EOF(intFILE_NO_2)

            dblRECORD_COUNT_WRK += 1
            Try
                FileGet(intFILE_NO_2, gstrDATA_390, dblRECORD_COUNT_WRK)
                strFURI_DATA = gstrDATA.strDATA
            Catch EX As Exception
                fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "ワークファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT_WRK & Err.Description)
                Exit Function
            End Try

            Select Case gstrDATA_390.strDATA.Substring(0, 1)
                Case "1"     'ヘッダーレコード
                    FileGet(intFILE_NO_2, gKOKUZEI_REC1, dblRECORD_COUNT_WRK)
                    strF_ITAKU_CODE = ""
                    '取引先コードの取得
                    gstrSSQL = "SELECT * FROM S_TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND TORIF_CODE_T = '" & Trim(strTORIF_CODE) & "'"

                    gdbCOMMAND = New OracleClient.OracleCommand
                    gdbCOMMAND.CommandText = gstrSSQL
                    gdbCOMMAND.Connection = gdbcCONNECT

                    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                    While (gdbrREADER.Read)

                        strITAKU_CODE = gdbrREADER.Item("ITAKU_CODE_T")
                        '国税フォーマットに委託者コードなし
                        'If strF_ITAKU_CODE <> strITAKU_CODE Then
                        '    gstrSSQL = "SELECT * FROM S_TORIMAST WHERE TORIS_CODE_T = '" & Trim(strTORIS_CODE) & "' AND ITAKU_CODE_T = '" & Trim(strF_ITAKU_CODE) & "'"
                        '    gdbCOMMAND.CommandText = gstrSSQL
                        '    gdbCOMMAND.Connection = gdbcCONNECT

                        '    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

                        '    If gdbrREADER.Read = False Then
                        '        '一致する取引先コードが存在しない
                        '        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                        '        Exit Function
                        '    End If
                        'End If
                        strTORIF_CODE = gdbrREADER.Item("TORIF_CODE_T")
                        strITAKU_NNAME = gdbrREADER.Item("ITAKU_NNAME_T")
                        strTKIN_NO = gdbrREADER.Item("TKIN_NO_T")
                        strTSIT_NO = gdbrREADER.Item("TSIT_NO_T")
                        strCODE_KBN = gdbrREADER.Item("CODE_KBN_T")
                        strMULTI_KBN = gdbrREADER.Item("MOTIKOMI_KBN_T")
                        strUKETUKE_KBN = gdbrREADER.Item("UMEISAI_KBN_T")
                    End While
                    If fn_Select_TENMAST(strTKIN_NO, strTSIT_NO, strITAKU_KIN_NNAME, strITAKU_SIT_NNAME, strITAKU_KIN_KNAME, strITAKU_SIT_KNAME) = False Then

                    End If
                    If Err.Number <> 0 Then
                        fn_LOG_WRITE(gstrUSER_ID, strTORIS_CODE, strF_FURI_DATE, gstrSYORI_R, "取引先検索", "失敗", "委託者コード：" & strF_ITAKU_CODE & Err.Description)
                        Exit Function
                    End If
                    intPAGE = 1
                    If intFILE_SEQ = 1 Then
                        Call fn_UKETUKE_MEISAI_HEDDA()
                    End If

                    Exit Do


            End Select


        Loop
        FileClose(intFILE_NO_2)
        dblALL_KEN = 0
        dblALL_KIN = 0
        If fn_UKETUKE_MEISAI_DETAIL_KOKUZEI() = False Then
            Exit Function
        End If
        fn_MEISAI_KOBETU_PRINT_KOKUZEI_K = True
    End Function
#End Region


End Class
