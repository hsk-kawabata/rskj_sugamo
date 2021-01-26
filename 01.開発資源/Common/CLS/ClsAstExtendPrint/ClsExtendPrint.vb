'*** Str Add 2015/12/04 SO)沖野 for 拡張印刷対応（新規作成） ***
Imports System.Text.RegularExpressions
Imports System
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Windows.Forms

Public Class CExtendPrint

    Inherits CAstReports.ClsReportBase

    Private LOG As CASTCommon.BatchLOG
    Private IS_LEVEL3 As Boolean
    Private IS_LEVEL4 As Boolean

    Private prtfld As String                    ' 拡張印刷定義の格納フォルダ

    ' CSV定義管理テーブル定義情報
    Private Structure CSV_DEF_TBL
        Dim itemType As Integer                 ' 項目種別
        Dim itemValue As String                 ' 項目値
        Dim itemExt As Object                   ' 拡張項目セクション名
    End Structure

    ' 定義種別3（DB項目名）定義情報
    Private Structure DBItem_INFO
        Dim pos As Integer                      ' [POS]キーの値
        Dim len As Integer                      ' [LEN]キーの値
    End Structure

    ' 定義種別4（文字列マッピング）定義情報
    Private Structure StringMap_INFO
        Dim itemExt As StringMap_INFO_Sub()     ' [VALUE]キー用の構造体
        Dim itemElse As String                  ' [ELSE]キーの値
    End Structure

    Private Structure StringMap_INFO_Sub
        Dim itemCompValue As String             ' 判定値
        Dim itemReplaceValue As String          ' 置き換え文字列
    End Structure

    ' 定義種別5（DBマッピング）定義情報
    Private Structure DBMap_INFO
        Dim itemExt As DBMap_INFO_Sub()         ' [VALUE]キー用の構造体
        Dim itemElse As String                  ' [ELSE]キーの値
    End Structure

    Private Structure DBMap_INFO_Sub
        Dim itemCompValue As String             ' 判定値
        Dim itemSetValue As String              ' 設定するDB項目名
    End Structure

    ' 定義種別7（複数DB項目結合）定義情報
    Private Structure DBUnion_INFO
        Dim itemDBNames As String()             ' DB項目名
        Dim itemSepa As String                  ' 結合時の区切り文字列
    End Structure


    Private Owner As Form = Nothing

    Public WriteOnly Property SetOwner() As Form
        Set(ByVal Value As Form)
            Owner = Value
        End Set
    End Property


    ' コンストラクタ
    Public Sub New()

        LOG = New CASTCommon.BatchLOG("CAstExtendPrint", "CExtendPrint")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()

    End Sub


    ' 機能   ： CSVファイルの作成と帳票印刷を行う
    ' 引数   ： prtID  帳票ID
    '           prtName  帳票名
    '           replaceArray  置換文字列配列
    '           printerArray  プリンタ名配列（通常使うプリンタのみの場合はNothing、配列中の通常使うプリンタ名は空文字）
    '           isAll9  ALL9指定か否か（True: ALL9）
    ' 戻り値 ： 印刷したレコード数（該当レコードがない場合は0、異常時は-1）
    '
    Public Function ExtendPrint(ByVal prtID As String, ByVal prtName As String, _
                               ByVal replaceArray As String(), ByVal printerArray As String(), _
                               ByVal isALL9 As Boolean) As Integer

        Dim ret As Integer = -1   ' 復帰値

        ' 処理開始ログ出力
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        sw = LOG.Write_Enter1("CExtendPrint.ExtendPrint", prtID & "_" & prtName)

        Try
            If IS_LEVEL4 = True Then
                Dim replace As String = "Nothing"
                Dim printer As String = "Nothing"

                If Not replaceArray Is Nothing Then
                    replace = String.Join(",", replaceArray)
                End If

                If Not printerArray Is Nothing Then
                    printer = String.Join(",", printerArray)
                End If

                LOG.Write_LEVEL4("CExtendPrint.ExtendPrint", "引数", _
                                                             "prtID=" & prtID & _
                                                             ", prtName=" & prtName & _
                                                             ", replaceArray=" & replace & _
                                                             ", printerArray=" & printer & _
                                                             ", isALL9=" & isALL9)
            End If

            ' 帳票印刷パラメタファイルを作成し、帳票印刷Exeを実行
            ret = ExecExtendPrint(prtID, prtName, replaceArray, printerArray, isALL9)
            Return ret

        Catch ex As Exception
            LOG.Write_Err("CExtendPrint.ExtendPrint", ex)
            Return ret

        Finally
            ' 処理終了ログ出力
            LOG.Write_Exit1(sw, "CExtendPrint.ExtendPrint", "復帰値=" & ret)
        End Try

    End Function


    ' 機能   ： 業務固有拡張印刷クラスを呼出し帳票印刷を行う
    ' 引数   ： prtID  帳票ID
    '           prtName  帳票名
    '           replaceArray  置換文字列配列
    '           printerArray  プリンタ名配列（通常使うプリンタのみの場合はNothing、配列中の通常使うプリンタ名は空文字）
    '           isAll9  ALL9指定か否か（True: ALL9）
    '           dllName  業務固有CSV作成メソッドの拡張子を除いたDLL名
    '           className  業務固有CSV作成メソッドのクラス名
    '           methodName  業務固有CSV作成メソッド名
    ' 戻り値 ： 印刷したレコード数（該当レコードがない場合は0、異常時は-1）
    '
    Public Function ExtendPrint(ByVal prtID As String, ByVal prtName As String, ByVal replaceArray As String(), _
                                ByVal printerArray As String(), ByVal isAll9 As Boolean, ByVal dllName As String, _
                                ByVal className As String, ByVal methodName As String) As Integer

        Dim ret As Integer = -1   ' 復帰値

        ' 処理開始ログ出力
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        sw = LOG.Write_Enter1("CExtendPrint.ExtendPrint", prtID & "_" & prtName)

        Try
            If IS_LEVEL4 = True Then
                Dim replace As String = "Nothing"
                Dim printer As String = "Nothing"

                If Not replaceArray Is Nothing Then
                    replace = String.Join(",", replaceArray)
                End If

                If Not printerArray Is Nothing Then
                    printer = String.Join(",", printerArray)
                End If

                LOG.Write_LEVEL4("CExtendPrint.ExtendPrint", "引数", _
                                                             "prtID=" & prtID & _
                                                             ", prtName=" & prtName & _
                                                             ", replaceArray=" & replace & _
                                                             ", printerArray=" & printer & _
                                                             ", isALL9=" & isAll9 & _
                                                             ", dllName=" & dllName & _
                                                             ", className=" & className & _
                                                             ", methodName=" & methodName)
            End If

            ' 帳票印刷パラメタファイルを作成し、帳票印刷Exeを実行
            ret = ExecExtendPrint(prtID, prtName, replaceArray, printerArray, isAll9, dllName, className, methodName)
            Return ret

        Catch ex As Exception
            LOG.Write_Err("CExtendPrint.ExtendPrint", ex)
            Return ret

        Finally
            ' 処理終了ログ出力
            LOG.Write_Exit1(sw, "CExtendPrint.ExtendPrint", "復帰値=" & ret)
        End Try

    End Function


    ' 機能   ： CSVファイルの作成と帳票印刷を行う（Exeからの呼出し用）
    ' 引数   ： prtID  帳票ID
    '           prtName  帳票名
    '           replaceArray  置換文字列配列
    '           printerArray  プリンタ名配列（通常使うプリンタのみの場合はNothing、配列中の通常使うプリンタ名は空文字）
    '           isAll9  ALL9指定か否か（True: ALL9）
    ' 戻り値 ： 印刷したレコード数（該当レコードがない場合は0、異常時は-1）
    '
    Public Function ExtendPrint4Exe(ByVal prtID As String, ByVal prtName As String, _
                               ByVal replaceArray As String(), ByVal printerArray As String(), _
                               ByVal isALL9 As Boolean) As Integer

        Dim ret As Integer = -1   ' 復帰値
        Dim retCnt As Integer = 0 ' 印刷レコード数

        ' 処理開始ログ出力
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        sw = LOG.Write_Enter1("CExtendPrint.ExtendPrint4Exe", prtID & "_" & prtName)

        Try
            ' SQL作成クラスの呼出し
            Dim CExtPrintSQL As New ClsExtendPrintSQL()
            Dim sql As String = CExtPrintSQL.Make_SQL(prtID, prtName, replaceArray, isALL9)

            If sql Is Nothing Then
                Return ret
            End If

            prtfld = CASTCommon.GetFSKJIni("COMMON", "PRT_FLD") ' 拡張印刷定義の格納フォルダ
            If prtfld = "err" OrElse prtfld = "" Then
                LOG.Write_Err("CExtendPrint.ExtendPrint4Exe", "fskj.iniで[COMMON]セクション、もしくは[PRT_FLD]キーが定義されていません。")

                Return ret
            End If

            If prtfld.EndsWith("\") = False Then
                prtfld &= "\"
            End If

            ' CSVファイル名用の帳票IDを設定
            InfoReport.ReportName = prtID

            Dim csvpath As String = CreateCsvFile()

            If csvpath Is Nothing Then
                LOG.Write_Err("CExtendPrint.ExtendPrint4Exe", "CSVファイルの作成に失敗しました。fskj.iniの[PRT]キーが定義されていません。")

                Return ret
            End If

            ' 帳票印刷CSV定義ファイル : 拡張印刷定義の格納フォルダ\帳票ID_帳票名\帳票ID_帳票名_CSV.ini
            Dim csvinifile As String = prtfld & prtID & "_" & prtName & "\" & prtID & "_" & prtName & "_CSV.ini"

            ' CSV作成開始ログ出力
            Dim sw2 As System.Diagnostics.Stopwatch
            sw2 = LOG.Write_Enter1("CExtendPrint.ExtendPrint4Exe", "CSV作成")

            ' CSV定義管理テーブルの生成
            Dim csvdefTable As CSV_DEF_TBL()
            csvdefTable = CreateCsvDefTbl(csvinifile, replaceArray)

            ' SQL実行
            Dim oracleDB As CASTCommon.MyOracle = Nothing
            Dim oraReader As CASTCommon.MyOracleReader = Nothing

            Try
                oracleDB = New CASTCommon.MyOracle
                oraReader = New CASTCommon.MyOracleReader(oracleDB)

                If oraReader.DataReader(sql) = False Then
                    ' データなしの場合
                    If oraReader.Message = "" Then
                        ret = 0
                        Return ret
                    End If

                    ' DBアクセス失敗の場合
                    LOG.Write_Err("CExtendPrint.ExtendPrint4Exe", "CSVファイルに出力するデータを取得する際に、DBのアクセスに失敗しました。")

                    Return ret

                End If

                Dim csvtblIndex As Integer
                Dim csvdata As String = ""  ' CSVの出力データ

                While oraReader.EOF = False ' EOFになるまでループ
                    For csvtblIndex = 0 To csvdefTable.Length - 1
                        ' CSVに出力するデータを取得
                        csvdata = getCSVOutputData(csvdefTable, csvtblIndex, oraReader)

                        ' 最後のデータの場合は改行を付ける
                        If csvtblIndex = csvdefTable.Length - 1 Then
                            CSVObject.Output(csvdata, True, True)
                        Else
                            CSVObject.Output(csvdata, True)
                        End If
                    Next

                    oraReader.NextRead()

                    ' レコード数を加算
                    retCnt += 1
                End While

                CSVObject.Close()

            Catch ex As Exception
                LOG.Write_Err("CExtendPrint.ExtendPrint4Exe", ex)
                Return ret

            Finally
                If Not oraReader Is Nothing Then oraReader.Close()
                If Not oracleDB Is Nothing Then oracleDB.Close()
            End Try

            ' CSV作成終了ログ出力
            LOG.Write_Exit1(sw2, "CExtendPrint.ExtendPrint4Exe", "CSV作成")


            Dim prtDspName As String = prtID & "_" & prtName ' 印刷名

            ' 印刷実行
            Dim prtRet As Boolean = ExtendPrintOnly(prtID, prtName, prtDspName, csvpath, printerArray)

            If prtRet = False Then
                Return ret
            End If

            ret = retCnt
            Return ret

        Catch ex As Exception
            LOG.Write_Err("CExtendPrint.ExtendPrint4Exe", ex)
            Return ret

        Finally
            ' 処理終了ログ出力
            LOG.Write_Exit1(sw, "CExtendPrint.ExtendPrint4Exe", "復帰値=" & ret)
        End Try

    End Function

    ' 機能   ： 業務固有拡張印刷クラスを呼出し帳票印刷を行う（Exeからの呼出し用）
    ' 引数   ： prtID  帳票ID
    '           prtName  帳票名
    '           replaceArray  置換文字列配列
    '           printerArray  プリンタ名配列（通常使うプリンタのみの場合はNothing、配列中の通常使うプリンタ名は空文字）
    '           isAll9  ALL9指定か否か（True: ALL9）
    '           dllName  業務固有CSV作成メソッドの拡張子を除いたDLL名
    '           className  業務固有CSV作成メソッドのクラス名
    '           methodName  業務固有CSV作成メソッド名
    ' 戻り値 ： 印刷したレコード数（該当レコードがない場合は0、異常時は-1）
    '
    Public Function ExtendPrint4Exe(ByVal prtID As String, ByVal prtName As String, ByVal replaceArray As String(), _
                                ByVal printerArray As String(), ByVal isAll9 As Boolean, ByVal dllName As String, _
                                ByVal className As String, ByVal methodName As String) As Integer

        Dim ret As Integer = -1 ' 印刷レコード数

        ' 処理開始ログ出力
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        sw = LOG.Write_Enter1("CExtendPrint.ExtendPrint4Exe", prtID & "_" & prtName)

        Try
            ' 業務固有印刷クラス呼出し用
            Dim prtDllName As String = ""     ' DLL名
            Dim prtClassName As String = ""   ' クラス名
            Dim prtDllAsm As Assembly         ' DLLアセンブリ
            Dim prtClassInstance As Object    ' クラスインスタンス

            Try
                ' dllをロード
                prtDllAsm = System.Reflection.Assembly.LoadFrom(dllName & ".dll")

            Catch ex As Exception
                LOG.Write_Err("CExtendPrint.ExtendPrint4Exe", dllName & ".dll が見つかりません。")
                Return ret
            End Try

            ' クラスをインスタンス化
            prtClassInstance = prtDllAsm.CreateInstance(dllName & "." & className)

            If prtClassInstance Is Nothing Then
                LOG.Write_Err("CExtendPrint.ExtendPrint4Exe", "業務固有印刷クラス(" & className & ")が" & dllName & ".dll" & " にありません。")
                Return ret
            End If

            ' メソッド呼出し
            ' 業務固有印刷メソッド開始ログ出力
            Dim sw2 As System.Diagnostics.Stopwatch
            sw2 = LOG.Write_Enter1("CExtendPrint.ExtendPrint4Exe", prtID & "_" & prtName)
            LOG.Write_LEVEL1("CExtendPrint.ExtendPrint4Exe", "業務固有印刷メソッド呼出し", className & "." & methodName)

            Dim methodInfo As MethodInfo = prtClassInstance.GetType.GetMethod(methodName)

            If methodInfo Is Nothing Then
                LOG.Write_Err("CExtendPrint.ExtendPrint4Exe", "業務固有印刷メソッド(" & methodName & ")が見つかりません。")
                Return ret
            End If

            Dim methodParams() As Object = {prtID, prtName, replaceArray, printerArray, isAll9}

            ret = methodInfo.Invoke(prtClassInstance, methodParams)

            ' 業務固有印刷メソッド終了ログ出力
            LOG.Write_Exit1(sw2, "CExtendPrint.ExtendPrint4Exe", "業務固有印刷メソッドの復帰値=" & ret)

            If ret = -1 Then
                LOG.Write_Err("CExtendPrint.ExtendPrint4Exe", "業務固有印刷メソッド(" & methodName & ")が異常終了しました。")
            End If

            Return ret

        Catch ex As Exception
            LOG.Write_Err("CExtendPrint.ExtendPrint4Exe", ex)
            Return ret

        Finally
            ' 処理終了ログ出力
            LOG.Write_Exit1(sw, "CExtendPrint.ExtendPrint4Exe", "復帰値=" & ret)
        End Try

    End Function

    ' 機能   ： 指定されたプリンタ名配列の順番で拡張印刷を行う
    ' 引数   ： prtID  帳票ID
    '           prtName  帳票名
    '           prtDspName  印刷名 （例： 帳票ID_帳票名）
    '           csvpath  RepoAgent用CSVファイルのフルパス名
    '           printerArray  プリンタ名配列（通常使うプリンタのみの場合はNothing、配列中の通常使うプリンタ名は空文字）
    ' 戻り値 ： True-成功, False-失敗
    '
    Public Function ExtendPrintOnly(ByVal prtID As String, ByVal prtName As String, _
                               ByVal prtDspName As String, ByVal csvpath As String, _
                               ByVal printerArray As String()) As Boolean

        Dim bRet As Boolean = False
        Dim repo As CAstReports.RAX

        ' 処理開始ログ出力
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        sw = LOG.Write_Enter1("CExtendPrint.ExtendPrintOnly", prtID & "_" & prtName)

        Try
            If IS_LEVEL4 = True Then
                Dim printer As String = "Nothing"

                If Not printerArray Is Nothing Then
                    printer = String.Join(",", printerArray)
                End If

                LOG.Write_LEVEL4("CExtendPrint.ExtendPrintOnly", "引数", _
                                                             "prtID=" & prtID & _
                                                             ", prtName=" & prtName & _
                                                             ", prtDspName=" & prtDspName & _
                                                             ", csvpath=" & csvpath & _
                                                             ", printerArray=" & printer)
            End If

            If prtfld Is Nothing Then
                prtfld = CASTCommon.GetFSKJIni("COMMON", "PRT_FLD") ' 拡張印刷定義の格納フォルダ
                If prtfld = "err" OrElse prtfld = "" Then
                    LOG.Write_Err("CExtendPrint.ExtendPrintOnly", "fskj.iniで[COMMON]セクション、もしくは[PRT_FLD]キーが定義されていません。")

                    Return bRet
                End If

                If prtfld.EndsWith("\") = False Then
                    prtfld &= "\"
                End If
            End If

            ReportBaseName = prtID & "_" & prtName & ".rpd" ' 帳票定義体

            repo = New CAstReports.RAX(prtfld & prtID & "_" & prtName & "\", ReportBaseName) ' 帳票定義体の格納先：帳票名フォルダ配下
            repo.CsvName = csvpath
            repo.Copies = Copies
            repo.DataCode = DataCode

            ' RepoAgentのログの格納先
            repo.LogPath = prtfld & "LOG"
            If Directory.Exists(repo.LogPath) = False Then
                Directory.CreateDirectory(repo.LogPath)
            End If
            repo.LogName = repo.ReportName & ".LOG"

            ' 印刷実行処理
            bRet = Extend_ReportExecute(repo, prtDspName, printerArray)

            If bRet = False Then
                LOG.Write_Err("CExtendPrint.ExtendPrintOnly", "印刷処理に失敗しました。詳細メッセージ = " & MyBase.ReportMessage)
                Return bRet
            End If

            Return bRet

        Catch ex As Exception
            LOG.Write_Err("CExtendPrint.ExtendPrintOnly", ex)
            Return bRet

        Finally
            repo = Nothing

            ' 処理終了ログ出力
            LOG.Write_Exit1(sw, "CExtendPrint.ExtendPrintOnly", "復帰値=" & bRet)
        End Try

    End Function

    ' 機能   ： CSVファイルを作成する
    ' 戻り値 ： CSVファイルの絶対パス
    '
    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile()
        Return file

    End Function

    ' 機能   ： 帳票印刷CSV定義ファイルの情報を解析して、CSV定義管理テーブルを生成する
    ' 引数   ： csvinifile  帳票印刷CSV定義ファイル
    '           replaceArray  置換文字列配列
    ' 戻り値 :  CSV定義管理テーブル
    ' 異常時 ： 例外をThrow
    '
    Private Function CreateCsvDefTbl(ByVal csvinifile As String, ByVal replaceArray As String()) As CSV_DEF_TBL()

        ' 処理開始ログ出力
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        sw = LOG.Write_Enter3("CExtendPrint.CreateCsvDefTbl")

        Try
            Dim cols As String()

            If File.Exists(csvinifile) Then
                cols = CASTCommon.GetIniFileValues(csvinifile, "COLS", "COL")
                If cols Is Nothing Then
                    Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクション、もしくは[COL]キーが定義されていません。")
                End If
            Else
                Throw New Exception("帳票印刷CSV定義ファイルが見つかりませんでした。ファイル=" & csvinifile)
            End If

            ' CSV定義管理テーブル動的生成
            Dim csvdefTbl As CSV_DEF_TBL() = New CSV_DEF_TBL(cols.Length - 1) {}

            ' 帳票印刷CSV定義ファイルの解析
            For i As Integer = 0 To cols.Length - 1

                Dim strSplits As String() = cols(i).Split(",")

                strSplits(0) = strSplits(0).Trim
                If strSplits(0) = "" Then
                    Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目種別が定義されていません。")

                ' 1（実行時タイムスタンプ）
                '
                ' 定義例)
                ' [COLS]
                ' COL=1, yyyyMMdd
                ' COL=1, HHmmss
                ElseIf strSplits(0) = "1" Then
                    csvdefTbl(i).itemType = 1

                    If strSplits.Length = 1 OrElse strSplits(1).Trim.Length = 0 Then
                        Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目値が定義されていません。")
                    End If

                    csvdefTbl(i).itemValue = CASTCommon.Calendar.Now.ToString(strSplits(1).Trim)

                ' 2（画面入力情報）
                '
                ' 定義例)
                ' [COLS]
                ' COL=2, 1
                ' COL=2, 3
                ElseIf strSplits(0) = "2" Then
                    csvdefTbl(i).itemType = 2

                    If strSplits.Length = 1 OrElse strSplits(1).Trim.Length = 0 Then
                        Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目値が定義されていません。")
                    End If

                    If IsNumeric(strSplits(1).Trim()) = False Then
                        Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目値に誤りがあります。")
                    End If

                    Dim displayInfo As Integer = CInt(strSplits(1).Trim())

                    If displayInfo <= 0 OrElse replaceArray Is Nothing OrElse replaceArray.Length < displayInfo Then
                        Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目値に誤りがあります。")
                    End If

                    ' 2016/01/27 SO)荒木 For IT_D-05_002 SQL置換え番号が跳び番の場合、SQL定義置換でエラーとなる
                    If replaceArray(displayInfo - 1) Is Nothing Then
                        Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目値に誤りがあります。")
                    End If

                    csvdefTbl(i).itemValue = replaceArray(displayInfo - 1)

                ' 3（DB項目名）
                '
                ' 定義例)
                ' [COLS]
                ' COL=3, IRAI_DATA_MEIMAST, 契約者名1
                ' COL=3, IRAI_DATA_MEIMAST, 契約者名2
                '
                ' [契約者名1]
                ' POS=0
                ' LEN=15
                '
                ' [契約者名2]
                ' POS=15
                ElseIf strSplits(0) = "3" Then
                    csvdefTbl(i).itemType = 3

                    If strSplits.Length = 1 OrElse strSplits(1).Trim.Length = 0 Then
                        Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目値が定義されていません。")
                    End If

                    csvdefTbl(i).itemValue = strSplits(1).Trim

                    ' 拡張項目セクション名が定義されている場合
                    If strSplits.Length = 3 Then
                        Dim info As DBItem_INFO = New DBItem_INFO

                        Dim strPos As String = CASTCommon.GetIniFileValue(csvinifile, strSplits(2).Trim, "POS")
                        Dim strLen As String = CASTCommon.GetIniFileValue(csvinifile, strSplits(2).Trim, "LEN")

                        If IsNumeric(strPos) = False OrElse CInt(strPos) < 0 Then
                            Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクション、もしくは[POS]キーが定義されていないか誤りがあります。")
                        End If

                        info.pos = CInt(strPos)

                        If strLen = "err" Then
                            info.len = 0
                        Else
                            If IsNumeric(strLen) = False OrElse CInt(strLen) <= 0 Then
                                Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクションの[LEN]キー値に誤りがあります。")
                            End If
                            info.len = CInt(strLen)
                        End If

                        csvdefTbl(i).itemExt = info
                    End If

                ' 4（文字列マッピング）
                '
                ' 定義例)
                ' [COLS]
                ' COL=4, UMEISAI_KBN_T, 受付明細表出力区分1
                ' COL=4, UMEISAI_KBN_T, 受付明細表出力区分2
                '
                ' [受付明細表出力区分1]
                ' VALUE=0, 未出力
                ' VALUE=1, 店番ｿｰﾄ
                ' VALUE=2, 非ｿｰﾄ
                ' VALUE=3, ｴﾗｰ分
                ' ELSE=
                '
                ' [受付明細表出力区分2]
                ' FILE=C:\RSKJ\TXT\KFJMAST010_受付明細表出力区分.TXT
                ' ELSE=
                ElseIf strSplits(0) = "4" Then
                    csvdefTbl(i).itemType = 4

                    If strSplits.Length < 3 OrElse strSplits(1).Trim.Length = 0 OrElse strSplits(2).Trim.Length = 0 Then
                        Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目情報の形式に誤りがあります。")
                    End If

                    csvdefTbl(i).itemValue = strSplits(1).Trim

                    Dim info As StringMap_INFO = New StringMap_INFO

                    Dim strValues As String() = Nothing

                    ' FILEの値を取得
                    Dim txtFile As String = CASTCommon.GetIniFileValue(csvinifile, strSplits(2).Trim, "FILE")

                    If txtFile = "err" Or txtFile = "" Then
                    ' FILEが定義されていない場合、VALUEの値を取得
                        strValues = CASTCommon.GetIniFileValues(csvinifile, strSplits(2).Trim, "VALUE")
                        If strValues Is Nothing Then
                            Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクション、もしくは[VALUE]キーが定義されていません。")
                        End If
                    Else
                    ' FILEが定義されている場合、txtFileの値を取得
                        Try
                            If Not File.Exists(txtFile) Then
                                Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクションの[FILE]キーに指定したファイルが見つかりません。")
                            End If

                            strValues = File.ReadAllLines(txtFile, Encoding.GetEncoding("SHIFT-JIS"))

                            If strValues Is Nothing Then
                                Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクションの[FILE]キーに指定したファイルの内容に誤りがあります。")
                            End If

                        Catch ex As Exception
                            Throw

                        End Try
                    End If

                    ' ELSEの値を取得
                    Dim strElse As String = CASTCommon.GetIniFileValue(csvinifile, strSplits(2).Trim, "ELSE")

                    ' キー、もしくはキー値を省略した場合は、空文字を設定
                    If strElse = "err" Then
                        strElse = ""
                    End If

                    Dim StringMapSubTbl As StringMap_INFO_Sub()
                    StringMapSubTbl = New StringMap_INFO_Sub(strValues.Length - 1) {}

                    For j As Integer = 0 To strValues.Length - 1
                        Dim StringMapSubInfo As String() = Nothing

                        If Not strValues(j) = "" Then
                            StringMapSubInfo = strValues(j).Split(",")
                            If StringMapSubInfo.Length < 2 Then
                                Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクションの[VALUE]キー値、もしくは[FILE]キーに指定したファイルの内容に誤りがあります。")
                            End If

                            StringMapSubTbl(j).itemCompValue = StringMapSubInfo(0).Trim
                            StringMapSubTbl(j).itemReplaceValue = StringMapSubInfo(1).Trim
                        End If
                    Next

                    info.itemExt = StringMapSubTbl
                    info.itemElse = strElse
                    csvdefTbl(i).itemExt = info

                ' 5（DBマッピング）
                '
                ' 定義例)
                ' [COLS]
                ' COL=5, TEKIYOU_KBN_T, 摘要
                '
                ' [摘要]
                ' VALUE=0, KTEKIYOU_T
                ' VALUE=1, NTEKIYOU_T
                ' ELSE=
                ElseIf strSplits(0) = "5" Then
                    csvdefTbl(i).itemType = 5

                    If strSplits.Length < 3 OrElse strSplits(1).Trim.Length = 0 OrElse strSplits(2).Trim.Length = 0 Then
                        Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目情報の形式に誤りがあります。")
                    End If

                    csvdefTbl(i).itemValue = strSplits(1).Trim

                    Dim info As DBMap_INFO = New DBMap_INFO

                    Dim strValues As String() = Nothing

                    ' VALUEの値を取得
                    strValues = CASTCommon.GetIniFileValues(csvinifile, strSplits(2).Trim, "VALUE")
                    If strValues Is Nothing Then
                        Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクション、もしくは[VALUE]キーが定義されていません。")
                    End If

                    ' ELSEの値を取得
                    Dim strElse As String = CASTCommon.GetIniFileValue(csvinifile, strSplits(2).Trim, "ELSE")

                    ' キー、もしくは、キー値を省略した場合は、空文字を設定
                    If strElse = "err" Then
                        strElse = ""
                    End If

                    Dim DBMapSubTbl As DBMap_INFO_Sub()
                    DBMapSubTbl = New DBMap_INFO_Sub(strValues.Length - 1) {}

                    For j As Integer = 0 To strValues.Length - 1
                        Dim DBMapSubInfo As String() = Nothing

                        If Not strValues(j) = "" Then
                            DBMapSubInfo = strValues(j).Split(",")
                            If DBMapSubInfo.Length < 2 OrElse DBMapSubInfo(1).Trim = "" Then
                                Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクションの[VALUE]キー値に誤りがあります。")
                            End If

                            DBMapSubTbl(j).itemCompValue = DBMapSubInfo(0).Trim
                            DBMapSubTbl(j).itemSetValue = DBMapSubInfo(1).Trim
                        End If

                    Next

                    info.itemExt = DBMapSubTbl
                    info.itemElse = strElse
                    csvdefTbl(i).itemExt = info

                ' 6（INIファイル定義取得）
                '
                ' 定義例)
                ' [COLS]
                ' COL=6, , 金融機関名
                '
                ' [金融機関名]
                ' INISEC=PRINT
                ' INIKEY=KINKONAME
                ElseIf strSplits(0) = "6" Then
                    csvdefTbl(i).itemType = 6

                    If strSplits.Length < 3 OrElse strSplits(2).Trim.Length = 0 Then
                        Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目情報の形式に誤りがあります。")
                    End If

                    Dim strSec As String = CASTCommon.GetIniFileValue(csvinifile, strSplits(2).Trim, "INISEC")
                    Dim strKey As String = CASTCommon.GetIniFileValue(csvinifile, strSplits(2).Trim, "INIKEY")

                    If strSec = "err" OrElse strSec = "" Then
                        Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクション、もしくは[" & strSplits(2).Trim & "]セクションの[INISEC]キーが定義されていません。")
                    End If

                    If strKey = "err" OrElse strKey = "" Then
                        Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクション、もしくは[" & strSplits(2).Trim & "]セクションの[INIKEY]キーが定義されていません。")
                    End If

                    Dim fskjinidata As String = CASTCommon.GetFSKJIni(strSec, strKey)

                    If fskjinidata = "err" Then
                        Throw New Exception("fskj.iniで" & "[" & strSec & "]セクション、もしくは[" & strKey & "]キーが定義されていません。")
                    End If

                    csvdefTbl(i).itemValue = fskjinidata

                ' 7（複数DB項目結合）
                '
                ' 定義例)
                ' [COLS]
                ' COL=7, , 金融機関支店コード
                '
                ' [金融機関支店コード]
                ' ITEM=KEIYAKU_KIN_K
                ' ITEM=KEIYAKU_SIT_K
                ' SEPA=-
                ElseIf strSplits(0) = "7" Then
                    csvdefTbl(i).itemType = 7

                    If strSplits.Length < 3 OrElse strSplits(2).Trim.Length = 0 Then
                        Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目情報の形式に誤りがあります。")
                    End If

                    Dim info As DBUnion_INFO = New DBUnion_INFO
                    Dim strItemNames As String() = CASTCommon.GetIniFileValues(csvinifile, strSplits(2).Trim, "ITEM")
                    Dim strSepa As String = CASTCommon.GetIniFileValue(csvinifile, strSplits(2).Trim, "SEPA")

                    If strItemNames Is Nothing Then
                        Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクション、もしくは[ITEM]キーが定義されていません。")
                    End If


                    For j As Integer = 0 To strItemNames.Length - 1
                        If strItemNames(j) = "" Then
                            Throw New Exception("帳票印刷CSV定義エラー：[" & strSplits(2).Trim & "]セクションの[ITEM]キー値に誤りがあります。")
                        End If
                    Next

                    ' キーを省略すると区切りなし、キー値を省略すると半角空白を区切り文字とする
                    If strSepa = "err" Then
                        strSepa = ""
                    ElseIf strSepa = "" Then
                        strSepa = " "
                    End If

                    info.itemDBNames = strItemNames
                    info.itemSepa = strSepa

                    csvdefTbl(i).itemExt = info

                Else
                    Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (i + 1) & "番目の項目種別は存在しません。")
                End If

            Next

            Return csvdefTbl

        Catch ex As Exception
            Throw

        Finally
            ' 処理終了ログ出力
            LOG.Write_Exit3(sw, "CExtendPrint.CreateCsvDefTbl")
        End Try

    End Function

    ' 機能   ： CSV定義管理テーブルの情報を元にCSVに出力するデータを生成する
    ' 引数   ： csvTableData  CSV定義管理テーブルの配列
    '           csvtblIndex  CSV定義管理テーブルのインデックス
    '           oraReader  OracleReader
    ' 戻り値 ： CSVに出力するデータ
    ' 異常時 ： 例外をThrow
    '
    Private Function getCSVOutputData(ByVal csvTableData As CSV_DEF_TBL(), ByVal csvtblIndex As Integer, _
                                      ByVal oraReader As CASTCommon.MyOracleReader) As String

        ' 処理開始ログ出力
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL4 = True Then
            sw = LOG.Write_Enter4("ClsExtendPrint.getCSVOutputData")
        End If

        Try
            Select Case csvTableData(csvtblIndex).itemType
                Case "1" ' 実行時タイムスタンプ
                    Return csvTableData(csvtblIndex).itemValue

                Case "2" ' 画面入力情報
                    Return csvTableData(csvtblIndex).itemValue

                Case "3" ' DB項目名
                    ' コーディング例）
                    '
                    ' Dim IRAI_DATA As String = OraReader.GetString("IRAI_DATA_MEIMAST", False)
                    ' PrinTuutisyo.OutputCsvData(IRAI_DATA.Substring(0, 15))
                    ' PrinTuutisyo.OutputCsvData(IRAI_DATA.Substring(15))

                    Dim dbitemValue As String = ""

                    ' DB項目値を取得
                    dbitemValue = GetDbItemString(oraReader, csvTableData(csvtblIndex).itemValue)

                    ' DB項目値が空文字、またはDB項目値の分割を行わない場合
                    If dbitemValue = "" OrElse csvTableData(csvtblIndex).itemExt Is Nothing Then
                        Return dbitemValue
                    End If

                    ' キャスト
                    Dim info As DBItem_INFO = CType(csvTableData(csvtblIndex).itemExt, DBItem_INFO)

                    ' info.pos のチェック
                    If info.pos >= dbitemValue.Length Then
                        ' 指定した位置がDB項目名の長さ以上の場合、エラー
                        Throw New Exception("帳票印刷CSV定義エラー：[COLS]セクションの" & (csvtblIndex + 1) & "番目の拡張情報の[POS]キーで指定した値に誤りがあります。")
                    End If

                    ' info.len のチェック
                    If info.len = 0 Then
                        ' LENが指定されていない場合、指定位置以降の文字列を取得
                        Return dbitemValue.Substring(info.pos)
                    Else
                        Dim len As Integer = info.len
                        ' 指定した位置からの文字列長が実際の文字列長を超える場合は、長さを補正する
                        If info.len > dbitemValue.Length - info.pos Then
                            len = dbitemValue.Length - info.pos
                        End If
                        Return dbitemValue.Substring(info.pos, len)
                    End If


                Case "4" ' 文字列マッピング
                    ' キャスト
                    Dim info As StringMap_INFO = CType(csvTableData(csvtblIndex).itemExt, StringMap_INFO)

                    Dim dbitemValue As String

                    ' DB項目値を取得
                    dbitemValue = GetDbItemString(oraReader, csvTableData(csvtblIndex).itemValue)

                    For i As Integer = 0 To info.itemExt.Length - 1
                        If dbitemValue = info.itemExt(i).itemCompValue Then
                            Return info.itemExt(i).itemReplaceValue
                        End If
                    Next

                    ' 判定値が該当しない場合の置換え文字列
                    Return info.itemElse

                Case "5" ' DB項目マッピング
                    ' コーディング例）
                    '
                    ' Select Case GCOM.NzStr(oraReader.GetString("TEKIYOU_KBN_T"))
                    '     Case "0" 'カナ摘要
                    '         OutputCsvData("カナ", True)
                    '         OutputCsvData(GCOM.NzStr(oraReader.GetString("KTEKIYOU_T")), True)
                    '
                    '     Case "1" '漢字摘要
                    '         OutputCsvData("漢字", True)
                    '         OutputCsvData(GCOM.NzStr(oraReader.GetString("NTEKIYOU_T")), True)
                    '
                    '     Case "2", "3"   '可変摘要
                    '         OutputCsvData("ﾃﾞｰﾀ", True)
                    '         OutputCsvData("", True)
                    '     Case Else
                    '         OutputCsvData("", True)
                    '         OutputCsvData("", True)
                    ' End Select

                    ' キャスト
                    Dim info As DBMap_INFO = CType(csvTableData(csvtblIndex).itemExt, DBMap_INFO)

                    Dim dbitemValue As String

                    ' DB項目値を取得
                    dbitemValue = GetDbItemString(oraReader, csvTableData(csvtblIndex).itemValue)

                    For i As Integer = 0 To info.itemExt.Length - 1
                        If dbitemValue = info.itemExt(i).itemCompValue Then
                            Return GetDbItemString(oraReader, info.itemExt(i).itemSetValue)
                        End If
                    Next

                    ' 判定値が該当しない場合の置換え文字列
                    Return info.itemElse

                Case "6" ' INIファイル定義取得
                    ' コーディング例）
                    '
                    ' JIKINKO_NAME = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
                    ' PrnKouzafurikaeSeikyuDataInvoice.OutputCsvData(JIKINKO_NAME)

                    Return csvTableData(csvtblIndex).itemValue

                Case "7" ' 複数DB項目結合
                    ' コーディング例）
                    '
                    ' CSVObject.Output(oraReader.GetItem("KEIYAKU_KIN_K") & oraReader.GetItem("KEIYAKU_SIT_K"))

                    ' キャスト
                    Dim info As DBUnion_INFO = CType(csvTableData(csvtblIndex).itemExt, DBUnion_INFO)

                    Dim dbitemUnion As String() = New String(info.itemDBNames.Length - 1) {}

                    For i As Integer = 0 To info.itemDBNames.Length - 1
                        dbitemUnion(i) = GetDbItemString(oraReader, info.itemDBNames(i))
                    Next

                    Dim unionStr As String
                    unionStr = String.Join(info.itemSepa, dbitemUnion)

                    Return unionStr

                Case Else
                    Return ""

            End Select

        Catch ex As Exception
            Throw

        Finally
            ' 処理終了ログ出力
            If IS_LEVEL4 = True Then
                LOG.Write_Exit4(sw, "CExtendPrint.getCSVOutputData")
            End If
        End Try

    End Function


' Str Add 2016/01/28 SO)荒木 For IT_D-05_004 DB項目名誤りの時の例外メッセージがわかりにくい
'                                       （oraReaderの例外ではDB項目名しか表示されない）
    ' 機能   ： 指定DB項目名のデータを返す
    ' 引数   ： oraReader  OracleReader
    '           DbItemName DB項目名
    ' 戻り値 ： DBの項目データ
    ' 異常時 ： 例外をThrow
    '
    Private Function GetDbItemString(ByVal oraReader As CASTCommon.MyOracleReader, ByVal DbItemName As String) As String

        Try
            Return oraReader.GetString(DbItemName)

        Catch ex As Exception
            LOG.Write_Err("CExtendPrint.GetDbItemString", ex)

            Throw New Exception("帳票印刷CSV定義エラー：DB項目名(" & DbItemName & ")の値取得に失敗しました")

        End Try

    End Function
' End Add 2016/01/28 SO)荒木 For IT_D-05_004 DB項目名誤りの時の例外メッセージがわかりにくい


    ' 機能   ： 帳票印刷パラメタファイルを作成し、帳票印刷Exeを実行する
    ' 引数   ： prtID  帳票ID
    '           prtName  帳票名
    '           replaceArray  置換文字列配列
    '           printerArray  プリンタ名配列（通常使うプリンタのみの場合はNothing、配列中の通常使うプリンタ名は空文字）
    '           isAll9  ALL9指定か否か（True: ALL9）
    '           dllName  業務固有CSV作成メソッドの拡張子を除いたDLL名
    '           className  業務固有CSV作成メソッドのクラス名
    '           methodName  業務固有CSV作成メソッド名
    ' 戻り値 ： 印刷したレコード数（該当レコードがない場合は0、異常時は-1）
    '
    Private Function ExecExtendPrint(ByVal prtID As String, ByVal prtName As String, ByVal replaceArray As String(), _
                                ByVal printerArray As String(), ByVal isAll9 As Boolean, _
                                Optional ByVal dllName As String = Nothing, _
                                Optional ByVal className As String = Nothing, _
                                Optional ByVal methodName As String = Nothing) As Integer

        Dim ret As Integer = -1   ' 復帰値
        Dim paramFilePath As String = Nothing  ' パラメタファイルパス名
        Dim fWriter As StreamWriter = Nothing  ' ファイルライター

        ' 処理開始ログ出力
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        sw = LOG.Write_Enter3("CExtendPrint.ExecExtendPrint", prtID & "_" & prtName)


        Try
            Dim tmpfld As String = CASTCommon.GetFSKJIni("COMMON", "DATBK") ' 拡張印刷パラメタファイルの一時格納フォルダ
            If tmpfld = "err" OrElse tmpfld = "" Then
                LOG.Write_Err("CExtendPrint.ExecExtendPrint", "fskj.iniで[COMMON]セクション、もしくは[DATBK]キーが定義されていません。")
                Return ret
            End If

            If Not Directory.Exists(tmpfld) Then
                LOG.Write_Err("CExtendPrint.ExecExtendPrint", "fskj.iniの[DATBK]キーで定義したディレクトリ(" & tmpfld & ")が存在しません。")
                Return ret
            End If

            If tmpfld.EndsWith("\") = False Then
                tmpfld &= "\"
            End If

            ' 拡張印刷パラメタファイル名（帳票ID_帳票名_グローバル一意識別子(GUID).ini）
            Dim paramFile As String = prtID & "_" & prtName & "_" & System.Guid.NewGuid().ToString() & ".ini"

            ' 拡張印刷Exeとのログ突合せのためにログレベルは１とする
            LOG.Write_LEVEL1("CExtendPrint.ExecExtendPrint", "パラメタファイル：" & paramFile)

            ' 拡張印刷パラメタファイル作成
            paramFilePath = tmpfld & paramFile
            fWriter = New StreamWriter(paramFilePath, False, Encoding.Default)
            fWriter.WriteLine("[COMMON]")
            fWriter.WriteLine("PRTID=" & prtID)
            fWriter.WriteLine("PRTNAME=" & prtName)
            fWriter.WriteLine("ISALL9=" & isALL9)

            ' 置換文字列配列
            fWriter.WriteLine("[REPLACEARRAYS]")
            If Not replaceArray Is Nothing Then
                For i As Integer = 0 To replaceArray.Length - 1
                    ' 2016/01/27 SO)荒木 For IT_D-05_002 SQL置換え番号が跳び番の場合、SQL定義置換でエラーとなる
                    ' 置換文字がない場合は、Nothingを特定する値（制御文字付き）でiniファイルを作成
                    If replaceArray(i) Is Nothing Then
                        fWriter.WriteLine("REPLACE=Nothing" & Chr(1) & "Nothing")
                    Else
                        fWriter.WriteLine("REPLACE=" & replaceArray(i))
                    End If
                Next
            End If

            ' プリンタ名配列
            fWriter.WriteLine("[PRINTERARRAYS]")
            If Not printerArray Is Nothing Then
                For i As Integer = 0 To printerArray.Length - 1
                    fWriter.WriteLine("PRINTER=" & printerArray(i))
                Next
            End If

            ' 業務固有拡張印刷クラス情報
            If Not dllName Is Nothing Then
                fWriter.WriteLine("[EXTERNAL]")
                fWriter.WriteLine("DLLNAME=" & dllName)
                fWriter.WriteLine("CLASSNAME=" & className)
                fWriter.WriteLine("METHODNAME=" & methodName)
            End If

            fWriter.Close()
            fWriter = Nothing

            '-----------------------------------------------
            ' 拡張印刷Exe実行
            '-----------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Owner
            ret = ExeRepo.ExecReport("ExtendPrint.EXE", paramFilePath)

            Return ret

        Catch ex As Exception
            Throw

        Finally
            If Not fWriter Is Nothing Then
                fWriter.Close()
            End If

            If Not paramFilePath Is Nothing AndAlso File.Exists(paramFilePath) Then
                ' パラメタファイル削除
                File.Delete(paramFilePath)
            End If

            ' 処理終了ログ出力
            LOG.Write_Exit3(sw, "CExtendPrint.ExecExtendPrint", "復帰値=" & ret)
        End Try

    End Function

End Class
'*** End Add 2015/12/04 SO)沖野 for 拡張印刷対応（新規作成） ***
