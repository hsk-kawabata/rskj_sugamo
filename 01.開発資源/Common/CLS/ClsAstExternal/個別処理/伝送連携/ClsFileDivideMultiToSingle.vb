'========================================================================
'ClsFileDivideMultiToSingle
'依頼ファイル分割ツールクラス
'
'依頼データファイル（128128128・・・9形式）を読み込み、文字コード区分
'（JIS/JIS改あり/EBCDIC）を判定の上、シングル形式ファイルに分割します。
'分割単位は「ヘッダレコード～トレーラレコード」とします。
'
'＜運用ルール＞
'○読み込むファイルの文字コードはSHIFT-JIS、SHIFT-JIS（改行あり）、EBCDICをサポートします。
'○読み込みファイルは任意のディレクトリから選びます。一度に読み込めるファイル数はひとつのみです。
'○分割後のファイルは、そのまま総給振の一括落し込み用フォルダ（iniファイルで、ENTRI-SOUENTRYREAD）へ格納されます。
'
'作成日：2010/04/15
'作成者：m-fukuoka
'
'備考：
'2010/04/15 新規作成
'========================================================================
Imports System
Imports System.IO
Imports System.Text
Imports System.Diagnostics.StackTrace
Imports System.Globalization

Public Class ClsFileDivideMultiToSingle

#Region "クラス変数定義"

    Private Const LOG_RES_STRING_OK As String = "成功"
    Private Const LOG_RES_STRING_ERR As String = "エラー"
    Private Const LOG_RES_STRING_NG As String = "失敗"

    '文字コード区分
    Private iCharCdKbn As Integer
    'システム日付
    Private strSysDate As String = DateTime.Today.ToString("yyyyMMdd")
    'システム日時
    Private strSysTime As String = DateTime.Now.ToString("yyyyMMddHHmmss")
    'エンドレコード保管用（Nothing初期化）
    Private bufEndRecord As Byte() = Nothing
    '出力フォルダパス
    Private strOutDirPath As String = String.Empty

    'ログ出力クラスインスタンス
    Private MainLOG As New CASTCommon.BatchLOG("KFD011", "ファイル分割")

#End Region

#Region "クラス定数定義"

    '文字コード区分（SJIS／SJIS改／EBCDIC）
    Private Const CHARCD_KBN_SJIS As Integer = 1
    Private Const CHARCD_KBN_SJISKAI As Integer = 2
    Private Const CHARCD_KBN_EBCDIC As Integer = 3
    Private Const CHARCD_KBN_OTHER As Integer = 9

    'レコード区分（ヘッダ／データ／トレーラ／エンド）
    Private Const REC_KBN_H As Integer = 1
    Private Const REC_KBN_D As Integer = 2
    Private Const REC_KBN_T As Integer = 8
    Private Const REC_KBN_E As Integer = 9
    Private Const REC_KBN_NG As Integer = 0

    'ログ出力用構造体
    Dim clsUserID As String            'ユーザID
    Dim clsToriCode As String          '取引先主副コード
    Dim clsFuriDate As String          '振替日

#End Region

#Region "プロパティ変数"

    '入力ファイルパス
    Public WriteOnly Property InFilePath() As String
        Set(ByVal Value As String)
            Me.strInFilePath = Value
        End Set
    End Property
    Private strInFilePath As String

    'レコード長（初期値は規定レコード長、後にプログラム内で改行コード分が加味される）
    Public WriteOnly Property RecordLength() As Integer
        Set(ByVal Value As Integer)
            Me.iRecordLength = Value
        End Set
    End Property
    Private iRecordLength As Integer

    'ファイル数
    Public Property FileCount() As Integer
        Get
            Return Me.iFileCount
        End Get
        Set(ByVal Value As Integer)
            Me.iFileCount = Value
        End Set
    End Property
    Private iFileCount As Integer = 0

    'ユーザＩＤ
    Public WriteOnly Property UserId() As String
        Set(ByVal Value As String)
            Me.strUserId = Value
        End Set
    End Property
    Private strUserId As String

    '取引先コード
    Public WriteOnly Property ToriCode() As String
        Set(ByVal Value As String)
            Me.strToriCode = Value
        End Set
    End Property
    Private strToriCode As String

    '振替日
    Public WriteOnly Property FuriDate() As String
        Set(ByVal Value As String)
            Me.strFuriDate = Value
        End Set
    End Property
    Private strFuriDate As String

#End Region

#Region "メインメソッド"

    '=======================================================================
    'FileDivideMain
    '
    '＜概要＞
    '　ファイル分割ツールメインメソッドです。
    '
    '＜パラメータ＞
    '　なし
    '
    '＜戻り値＞
    '　1：ファイルなし
    '　2：ファイル内容取得エラー
    '  3：不明な文字コード
    '  4：不明なレコード区分
    '  5：ファイル出力エラー
    '  6：iniファイル取得エラー
    '　9：致命的エラー
    '　0：正常終了
    '=======================================================================
    Public Function DivideFileMain(ByRef SplitFileName As ArrayList) As Integer

        Try

            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "メイン", "開始", "")

            '=======================================================
            'ファイル存在確認
            '=======================================================
            If File.Exists(Me.strInFilePath) = False Then
                'ファイルなしエラー
                Return 1
            End If

            '=======================================================
            'iniファイル読み込み
            '=======================================================
            If Me.GetIniData() = False Then
                'iniファイル取得エラー
                Return 6
            End If

            '=======================================================
            'ファイルオープン・初期値取得
            '=======================================================
            'ファイルオープン、バイト配列取得
            SplitFileName = New ArrayList
            SplitFileName.Clear()
            Dim bArray As Byte() = Me.OpenFileAndGetByte()
            If bArray Is Nothing Then
                'ファイル内容取得エラー
                Return 2
            End If

            '=======================================================
            'レコード切り出し
            '=======================================================
            '切り出し開始オフセット
            Dim iOffset As Integer = 0
            '出力用バッファリスト（レコード単位格納）
            Dim alBufArraySJIS As New ArrayList

            '文字コードチェック（SJIS or EBCDIC）
            Dim blRetValue As Boolean = Me.CheckCharCdKbn(bArray(0), Me.iCharCdKbn)
            If Me.iCharCdKbn = CHARCD_KBN_OTHER Then
                '文字コード区分が想定外
                Return 3
            End If

            '文字コードがSHIFT_JISの場合は、改行の存在チェック
            'EBCDICの場合は改行ありを意識しなし
            If Me.iCharCdKbn = CHARCD_KBN_SJIS Then
                If Me.CheckIsIncludeCRLF(bArray, Me.iRecordLength) = True Then
                    'S-JIS改あり 文字コード区分更新
                    iCharCdKbn = CHARCD_KBN_SJISKAI
                    '改行分の2バイトをレコード長に追加
                    Me.iRecordLength += 2
                    MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "メイン", "成功", "文字コード：SJIS改行あり")
                End If
            End If


            'エンドレコードの抽出・保持
            If Me.GetEndRecord(bArray) = False Then
                'エンドレコード抽出結果、エンドレコードではなかった
                Return 4
            End If


            'ファイルの最初から最後まで続ける
            While iOffset < bArray.Length

                'レコード長ずつバイト配列から切り出しを行う
                Dim bTgtArray As Byte() = Me.CutToRecord(bArray, iOffset)

                '=======================================================
                'レコード判定・ファイル出力
                '=======================================================
                '切り出したレコードのレコード区分判定
                Dim iRecKbn As Integer = Me.CheckRecordKbn(bTgtArray(0))
                If iRecKbn = REC_KBN_NG Then
                    '不明なレコード区分
                    Return 4
                End If

                'エンドレコードならば処理を飛ばす
                If iRecKbn = REC_KBN_E Then
                    GoTo NEXT_RECORD
                End If

                'レコードリストに追加
                alBufArraySJIS.Add(bTgtArray)

                If iRecKbn = REC_KBN_T Then
                    'トレーラレコードだった場合、エンドレコードを付加して
                    'ファイル出力を行う

                    alBufArraySJIS.Add(Me.bufEndRecord)

                    If alBufArraySJIS.Count > 0 Then
                        If Me.CreateFile(alBufArraySJIS, CHARCD_KBN_SJIS, SplitFileName) = False Then
                            Return 5
                        End If
                        'バッファクリア
                        alBufArraySJIS.Clear()
                    End If
                End If

NEXT_RECORD:
                'オフセット加算（レコード長を加える）
                iOffset += Me.iRecordLength
            End While

            '正常終了
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "メイン", "成功", "")
            Return 0

        Catch ex As Exception
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "メイン", "失敗", ex.Message)
            Return 9
        Finally
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "メイン", "終了", "")
        End Try

    End Function

    '=======================================================================
    'GetIniData
    '＜概要＞
    '　設定ファイルから情報を取得し、グローバル変数へ格納します。
    '
    '＜パラメータ＞
    '　なし
    '
    '＜戻り値＞
    '　なし
    '
    '＜備考＞
    '=======================================================================
    Private Function GetIniData() As Boolean

        Try

            Me.strOutDirPath = CASTCommon.GetFSKJIni("COMMON", "DENBK")
            If Me.strOutDirPath = "err" OrElse Me.strOutDirPath = "" Then
                MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "iniファイル情報取得", "失敗", "[COMMON]-[DENBK]")
                Return False
            Else
                MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "iniファイル情報取得", "成功", Me.strOutDirPath)
            End If

            Return True

        Catch ex As Exception
            '異常ログ
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "iniファイル情報取得", "失敗", ex.Message)
            Return False
        End Try

    End Function

    '=======================================================================
    'OpenFileAndGetByte
    '
    '＜概要＞
    '　ファイルを開き、内容をバイト配列で返します。
    '
    '＜パラメータ＞
    '　なし
    '
    '＜戻り値＞
    '　ファイル内容バイト配列
    '=======================================================================
    Private Function OpenFileAndGetByte() As Byte()

        Dim fs As FileStream = Nothing

        Try
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "ファイル読込", "成功", "")

            'ファイルオープン
            fs = New FileStream(Me.strInFilePath, FileMode.Open, FileAccess.Read)
            If fs.Length = 0 Then
                'ファイル内容なしエラー
                MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "ファイル読込", "失敗", "空ファイル")
                Return Nothing
            End If

            'ファイル内容をバイト配列に格納
            Dim byteArray(fs.Length - 1) As Byte
            fs.Read(byteArray, 0, fs.Length)

            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "ファイル読込", "成功", "")
            Return byteArray

        Catch ex As Exception
            '例外時はNothingを返す
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "ファイル読込", "失敗", ex.Message)
            Return Nothing
        Finally
            If Not fs Is Nothing Then
                'ファイルクローズ
                fs.Close()
            End If
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "ファイル読込", "終了", "")
        End Try

    End Function

    '=======================================================================
    'CheckCharCd
    '
    '＜概要＞
    '　文字コードをチェックし、結果を返します。渡される文字はレコード区分を意味する
    '　「1/2/8/9」である必要があります。
    '
    '＜パラメータ＞
    '　byteCheckTgt：チェック対象文字（バイト型）
    '　iRetValue：文字コード区分（1：SHIFT_JIS／3：EBCDIC／9：判定対象外）
    '
    '＜戻り値＞
    '　TRUE（ヘッダレコードである） or FALSE（ヘッダレコードでない　または　エラー）
    '=======================================================================
    Private Function CheckCharCdKbn(ByVal byteCheckTgt As Byte, _
                                    ByRef iRetValue As Integer) As Boolean

        Try
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "レコード情報取得", "成功", "文字コード：SJIS")

            Select Case Hex(byteCheckTgt)
                Case "31"
                    'ヘッダレコード・JIS
                    iRetValue = CHARCD_KBN_SJIS
                    'ログ出力
                    MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "レコード情報取得", "成功", "文字コード：SJIS")
                    Return True
                Case "F1"
                    'ヘッダレコード・EBCDIC
                    iRetValue = CHARCD_KBN_EBCDIC
                    'ログ出力
                    MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "レコード情報取得", "成功", "文字コード：EBCDIC")
                    Return True
                Case "32", "38", "39"
                    'ヘッダレコードでない　JIS
                    iRetValue = CHARCD_KBN_SJIS
                    Return False
                Case "F2", "F8", "F9"
                    'ヘッダレコードでない　EBCDIC
                    iRetValue = CHARCD_KBN_EBCDIC
                    Return False
                Case Else
                    'レコード区分ではない
                    iRetValue = CHARCD_KBN_OTHER
                    MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "レコード情報取得", "失敗", "文字コード：不明")
                    Return False
            End Select

        Catch ex As Exception
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "レコード情報取得", "失敗", ex.Message)
            Return False
        End Try

    End Function

    '=======================================================================
    'CheckIsIncludeCRLF
    '
    '＜概要＞
    '　改行の有無を判定します。渡されたバイト配列を所定のレコード長（改行付きの場合は
    '　改行を含めない長さ）で割り切れるかどうかで判断します。
    '
    '＜パラメータ＞
    '　bArray：チェック対象文字列（バイト配列）
    '　iRecLength：フォーマットの規定長（全銀依頼ファイルの場合は120）
    '
    '＜戻り値＞
    '　TRUE（改行あり） or FALSE（改行なし）
    '=======================================================================
    Private Function CheckIsIncludeCRLF(ByVal bArray As Byte(), _
                                        ByVal iRecLength As Integer) As Boolean

        Dim iArrayLength As Integer = bArray.Length

        '規定文字数でも割り切れ、かつ規定文字数＋２（改行分）でも割り切れる
        'つまり、改行ありかなしか文字列長で判定できない場合
        If iArrayLength Mod iRecLength = 0 And _
            iArrayLength Mod (iRecLength + 2) = 0 Then

            '改行コードがあるべき場所の文字を取得
            Dim strCheckString As String = Me.GetCrlfString(bArray, iRecLength)
            '改行コードが否かをチェック
            Return Me.CheckIsCRLF(strCheckString, Me.iCharCdKbn)
        End If

        '規定文字数で割り切れる＝改行なし
        If iArrayLength Mod iRecLength = 0 Then
            Return False
        End If

        '規定文字数＋２で割り切れる＝改行あり
        If iArrayLength Mod (iRecLength + 2) = 0 Then
            Return True
        End If

        Return False

    End Function

    '=======================================================================
    'GetCrlfString
    '
    '＜概要＞
    '　改行コードが存在する位置の値を返します。
    '
    '＜パラメータ＞
    '　byteArray：チェック対象文字（バイト配列）
    '　iRecLength：フォーマットの規定長（全銀依頼ファイルの場合は120）
    '
    '＜戻り値＞
    '　改行コード位置の文字
    '=======================================================================
    Private Function GetCrlfString(ByVal bArray As Byte(), _
                                   ByVal iRecLength As Integer) As String

        'CRLFが代入される位置の値取得
        Dim sb As New StringBuilder
        With sb
            .Length = 0
            .Append(Hex(bArray(iRecLength)))
            .Append(Hex(bArray(iRecLength + 1)))
        End With

        Return sb.ToString

    End Function

    '=======================================================================
    'CheckIsCRLF
    '
    '＜概要＞
    '　渡された文字が改行コードか否かの判定を行います。
    '
    '＜パラメータ＞
    '　strCheckString：チェック対象文字
    '　iCharCd：文字コード区分
    '
    '＜戻り値＞
    '　改行コード位置の文字
    '=======================================================================
    Private Function CheckIsCRLF(ByVal strCheckString As String, _
                                 ByVal iCharCd As Integer) _
                                 As Boolean

        Select Case iCharCd
            Case 1
                'SHIFT_JIS
                If strCheckString <> "DA" Then
                    Return False
                End If
            Case 3
                'EBCDIC
                If strCheckString <> "D25" Then
                    Return False
                End If
            Case Else
                Return False
        End Select

        Return True

    End Function

    '=======================================================================
    'CutToRecord
    '
    '＜概要＞
    '　渡されたバイト配列の開始位置から、レコード長分の文字列をバイト配列から
    '　切り出します。
    '
    '＜パラメータ＞
    '　bArray：文字列（バイト配列）
    '　iOffset：切り出し開始位置
    '
    '＜戻り値＞
    '　切り出したバイト配列（レコード単位）
    '=======================================================================
    Private Function CutToRecord(ByVal bArray As Byte(), _
                                 ByVal iOffset As Integer)

        'レコード長で配列初期化
        Dim bRetArray(Me.iRecordLength - 1) As Byte

        For index As Integer = 0 To Me.iRecordLength - 1 Step 1
            '取得元配列より、新規配列へ格納
            bRetArray(index) = bArray(index + iOffset)
        Next

        Return bRetArray

    End Function

    '=======================================================================
    'CheckRecordKbn
    '
    '＜概要＞
    '　レコード区分を判定します。渡されたバイト文字で判定します。
    '
    '＜パラメータ＞
    '　bCheckTgt：チェック対象文字（バイト型）
    '
    '＜戻り値＞
    '　レコード区分（1/2/8/9/0（不明））
    '=======================================================================
    Private Function CheckRecordKbn(ByVal bCheckTgt As Byte) As Integer

        Try
            Dim str As String = Hex(bCheckTgt)

            Select Case Me.iCharCdKbn
                Case CHARCD_KBN_SJIS, CHARCD_KBN_SJISKAI
                    'SJISまたはSJIS改あり
                    Select Case str
                        Case "31"
                            'ヘッダ
                            Return REC_KBN_H
                        Case "32"
                            'データ
                            Return REC_KBN_D
                        Case "38"
                            'トレーラ
                            Return REC_KBN_T
                        Case "39"
                            'エンド
                            Return REC_KBN_E
                        Case Else
                            '不明
                            Return REC_KBN_NG
                    End Select

                Case CHARCD_KBN_EBCDIC
                    'EBCDIC
                    Select Case str
                        Case "F1"
                            'ヘッダ
                            Return REC_KBN_H
                        Case "F2"
                            'データ
                            Return REC_KBN_D
                        Case "F8"
                            'トレーラ
                            Return REC_KBN_T
                        Case "F9"
                            'エンド
                            Return REC_KBN_E
                        Case Else
                            '不明
                            Return REC_KBN_NG
                    End Select

                Case Else
                    '不明
                    Return REC_KBN_NG

            End Select

        Catch ex As Exception
            '不明
            Return REC_KBN_NG
        End Try

    End Function

    '=======================================================================
    'CheckRecordKbn
    '
    '＜概要＞
    '　レコード区分を判定します。渡されたバイト文字で判定します。
    '
    '＜パラメータ＞
    '　alBufferList：レコード単位にバイト配列を格納したArrayList
    '　iMode：ファイル作成モード（文字コード区分と同じ）
    '
    '＜戻り値＞
    '=======================================================================
    Private Function CreateFile(ByVal alBufferList As ArrayList, _
                                ByVal iMode As Integer, _
                                ByRef SplitFileName As ArrayList) As Boolean

        '出力用バイト配列
        Dim bOutputArray As Byte() = Nothing
        'ファイルカウント加算
        Me.iFileCount += 1

        Try
            '======================================================
            '出力用バイト配列作成
            '======================================================
            For i As Integer = 0 To alBufferList.Count - 1 Step 1

                'ArrayListより配列抽出
                Dim bCurArray As Byte() = alBufferList(i)

                '出力用配列の要素を追加
                ReDim Preserve bOutputArray((i + 1) * Me.iRecordLength - 1)

                'ArrayListの配列から、要素単位で出力用バイト配列へ格納
                Dim index As Integer = 0
                For Each bt As Byte In bCurArray
                    bOutputArray((i * Me.iRecordLength) + index) = bt
                    index += 1
                Next

            Next

            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "出力用バッファ作成", "成功", "")

        Catch ex As Exception
            '異常ログ
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "出力用バッファ作成", "失敗", ex.Message)
            Return False
        End Try

        '======================================================
        '出力先指定
        '======================================================
        'サフィックス文字（モード＆システム日時 & ファイル連番（9999）
        Dim strMode As String = String.Empty
        Select Case iMode
            Case CHARCD_KBN_SJIS
                strMode = "SJIS"
            Case CHARCD_KBN_SJISKAI
                strMode = "SJISK"
            Case CHARCD_KBN_EBCDIC
                strMode = "EBC"
        End Select

        Dim strAddStr As String = "_" & Me.iFileCount.ToString.PadLeft(3, "0"c)
        '出力ファイル名（入力ファイルのファイル名部分に上記のサフィックス文字を追加）
        Dim strOutFileName As String = _
            Path.GetFileNameWithoutExtension(Me.strInFilePath) & strAddStr & ".DAT"

        'ディレクトリがない場合は作成
        If Directory.Exists(Me.strOutDirPath) = False Then
            Directory.CreateDirectory(Me.strOutDirPath)
        End If

        '出力パス
        Dim strOutputFilePath As String = Path.Combine(Me.strOutDirPath, strOutFileName)

        Dim fs As FileStream = Nothing
        Try
            'ファイル出力ストリーム
            fs = New FileStream(strOutputFilePath, FileMode.Create, FileAccess.Write)
            'ファイル書き込み
            fs.Write(bOutputArray, 0, bOutputArray.Length)
            SplitFileName.Add(strOutputFilePath)

            '正常ログ
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "ファイル出力", "成功", "")
            Return True

        Catch ex As Exception
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "ファイル出力", "失敗", ex.Message)
            Return False
        Finally
            'ファイルクローズ
            If Not fs Is Nothing Then
                'ファイルクローズ
                fs.Close()
            End If
        End Try

    End Function

    '=======================================================================
    'GetEndRecord
    '
    '＜概要＞
    '　ファイルの最終レコード（エンドレコード）をバイト配列で取得します。
    '
    '＜パラメータ＞
    '　bArray：ファイル内容を格納したバイト配列
    '　iMode：ファイル作成モード（文字コード区分と同じ）
    '
    '＜戻り値＞
    '=======================================================================
    Private Function GetEndRecord(ByVal bArray As Byte()) As Boolean

        Try
            'エンドレコード格納用配列
            Dim bEndRecArray(Me.iRecordLength - 1) As Byte
            Dim j As Integer = 0

            'ファイルのバイト配列より、最終のレコードを抽出
            For i As Integer = bArray.Length - Me.iRecordLength To bArray.Length - 1 Step 1
                bEndRecArray(j) = bArray(i)
                j += 1
            Next

            If Me.CheckRecordKbn(bEndRecArray(0)) = REC_KBN_E Then
                '抽出結果がエンドレコードだったら、正常終了
                'グローバルへ格納
                Me.bufEndRecord = bEndRecArray

                MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "エンドレコード取得", "成功", "")
                Return True
            Else
                '抽出結果がエンドレコードではないため、異常終了
                MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "エンドレコード取得", "失敗", "データ内最終レコード要確認")
                Return False
            End If

        Catch ex As Exception
            '異常ログ
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "エンドレコード取得", "失敗", ex.Message)
            Return False
        End Try

    End Function

#End Region

End Class
