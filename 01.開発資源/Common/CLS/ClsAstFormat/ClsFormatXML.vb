'*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応（新規作成） ***
Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Imports CASTCommon.ModPublic
Imports Microsoft.VisualBasic


' 外付け定義 データフォーマットクラス
Public Class CFormatXML

    ' データフォーマット基底クラス
    Inherits CFormat

    ' ログレベル
    Private IS_LEVEL3 As Boolean
    Private IS_LEVEL4 As Boolean

    ' XMLファイル名
    Private mXmlFile As String
    ' XMLフォーマットrootオブジェクト
    Private mXmlRootElement As XmlElement
    Private mXmlRoot As XmlNode

    ' 個別DLL名
    Private mDllName As String = ""
    ' 個別クラス名
    Private mClassName As String = ""
    ' 個別DLLアセンブリ
    Private mDllAsm As Assembly
    ' 個別クラスインスタンス
    Private mClassInstance As Object

    ' 明細データ種別
    Private MEISAI_KIND_1 As Integer = 1
    Private MEISAI_KIND_2 As Integer = 2

    ' エンドレコード区分
    Private EndKubun As String = ""

    ' フォーマットデータ区分リスト
    Private mHeaderList As New List(Of XmlNode)
    Private mDataList As New List(Of XmlNode)
    Private mTrailerList As New List(Of XmlNode)
    Private mEndList As New List(Of XmlNode)

    ' フォーマットデータ数
    Private mHeaderCount As Integer
    Private mDataCount As Integer
    Private mTrailerCount As Integer
    Private mEndCount As Integer

    ' 明細リスト
    Private mMeisaiHeaderList As XmlNodeList
    Private mMeisaiDataList1 As XmlNodeList
    Private mMeisaiDataList2 As XmlNodeList
    Private mMeisaiTrailerList As XmlNodeList
    Private mMeisaiEndList As XmlNodeList

    ' カナ摘要設定リスト
    Private mKanaTekiList As XmlNodeList

    ' レコードチェック個別メソッド
    Private mChkHeaderRecMethod As String = ""
    Private mChkDataRecMethod As String = ""
    Private mChkTrailerRecMethod As String = ""
    Private mChkEndRecMethod As String = ""

    ' 規定文字チェック個別メソッド
    Private mChkHeaderRegularStrMethod As String = ""
    Private mChkDataRegularStrMethod As String = ""
    Private mChkTrailerRegularStrMethod As String = ""
    Private mChkEndRegularStrMethod As String = ""

    ' レコード値補正個別メソッド
    Private mChgHeaderRecMethod As String = ""
    Private mChgDataRecMethod As String = ""
    Private mChgTrailerRecMethod As String = ""
    Private mChgEndRecMethod As String = ""

    ' 返還/返還データ作成@データ区分
    Private mHenkanHeaderNode As XmlNode
    Private mHenkanDataNode As XmlNode
    Private mHenkanTrailerNode As XmlNode

    ' 返還/含0円データ
    Private mInclude0Yen As Integer = 1  ' 1: 0円データを含める  0: 0円データを含めない

	' 再振/再振データ作成@データ区分
    Private mSaifuriHeaderNode As XmlNode
    Private mSaifuriDataNode As XmlNode
    Private mSaifuriTrailerNode As XmlNode



    ' 機能   ： コンストラクタ
    ' 引数   ： フォーマット区分
    '
    Public Sub New(ByVal formatCode As String)

        MyBase.New()

        BLOG = New CASTCommon.BatchLOG("ClsFormatXML", "CFormatXML")
        IS_LEVEL3 = BLOG.IS_LEVEL3()
        IS_LEVEL4 = BLOG.IS_LEVEL4()

        Dim xmlDoc As New ConfigXmlDocument
        Dim node As XmlNode
        Dim nodeList As XmlNodeList
        Dim sWork As String

        'XMLパス作成
        Dim xmlFolderPath As String = CASTCommon.GetFSKJIni("COMMON", "XML_FORMAT_FLD")
        If xmlFolderPath = "err" Or xmlFolderPath = "" then
            Throw New Exception("fskj.iniでXML_FORMAT_FLDが定義されていません。")
        End If
        If xmlFolderPath.EndsWith("\") = False Then
            xmlFolderPath &= "\"
        End If
        mXmlFile = "XML_FORMAT_" & formatCode & ".xml"

        ' XMLフォーマットのrootオブジェクト生成
        xmlDoc.Load(xmlFolderPath & mXmlFile)
        mXmlRootElement = xmlDoc.DocumentElement
        mXmlRoot = mXmlRootElement.SelectSingleNode("/外付け定義")
        If mXmlRoot Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「外付け定義」タグが定義されていません。")
        End If

        ' 個別DLL名（値省略可）
        node = mXmlRoot.SelectSingleNode("共通/個別DLL名")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」タグが定義されていません。")
        End If
        mDllName = node.InnerText.Trim
        ' 個別DLLをロード
        If mDllName <> "" Then
            Try
                mDllAsm = System.Reflection.Assembly.LoadFrom(mDllName & ".dll")
            Catch ex As Exception
                BLOG.Write_Err("CFormatXML.New", ex)

                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」タグで指定された" & mDllName & ".dll" & "が見つかりません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End Try
        End If

        ' 個別クラス名（値省略可）
        node = mXmlRoot.SelectSingleNode("共通/個別クラス名")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別クラス名」タグが定義されていません。")
        End If
        mClassName = node.InnerText.Trim

        If mClassName <> "" Then
            If mDllName = "" Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別クラス名」を指定する場合は「共通/個別DLL名」も指定してください。")
            End If

            ' 個別クラスをインスタンス化
            Try
                mClassInstance = mDllAsm.CreateInstance(mDllName & "." & mClassName)
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別クラス名」タグで指定されたクラスが" & mDllName & ".dll" & "にありません。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If
            Catch ex As Exception
                BLOG.Write_Err("CFormatXML.New", ex)

                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別クラス名」タグで指定されたクラスが" & mDllName & ".dll" & "にありません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End Try
        End If

        ' レコードデータ長（必須）
        node = mXmlRoot.SelectSingleNode("共通/レコードデータ長")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/レコードデータ長」タグが定義されていません。")
        End If
        sWork = node.InnerText.Trim
        If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/レコードデータ長」タグの値（" & sWork & "）が不当です。（" & _
                                ConfigurationErrorsException.GetLineNumber(node) & "行目）")
        End If
        DataInfo.RecoedLen = CInt(sWork)

        ' Ftranパラメータファイル（値省略可）
        node = mXmlRoot.SelectSingleNode("共通/Ftranパラメータファイル")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/Ftranパラメータファイル」タグが定義されていません。")
        End If
        FtranPfile = node.InnerText.Trim

        ' FtranIBMパラメータファイル（値省略可）
        node = mXmlRoot.SelectSingleNode("共通/FtranIBMパラメータファイル")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/FtranIBMパラメータファイル」タグが定義されていません。")
        End If
        FtranIBMPfile = node.InnerText.Trim

        ' FtranIBMバイナリパラメータファイル（値省略可）
        node = mXmlRoot.SelectSingleNode("共通/FtranIBMバイナリパラメータファイル")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/FtranIBMバイナリパラメータファイル」タグが定義されていません。")
        End If
        FtranIBMBinaryPfile = node.InnerText.Trim

        ' CMTブロックサイズ（値省略可）
        node = mXmlRoot.SelectSingleNode("共通/CMTブロックサイズ")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/CMTブロックサイズ」タグが定義されていません。")
        End If
        sWork = node.InnerText.Trim
        If sWork <> "" Then
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/CMTブロックサイズ」タグの値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            CMTBlockSize = CInt(sWork)
        Else
            CMTBlockSize = 0
        End If

        ' レコード区分一覧[@データ区分='ヘッダ']/レコード区分（必須）
        nodeList = mXmlRoot.SelectNodes("共通/レコード区分一覧[@データ区分='ヘッダ']/レコード区分")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/レコード区分一覧[@データ区分='ヘッダ']/レコード区分」タグが定義されていません。")
        End If
        Dim HeaderKubunList As New Generic.List(Of String)
        For Each element As XmlElement In nodeList
            sWork = element.InnerText.Trim
            If sWork = "" Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/レコード区分一覧/レコード区分」タグの値が指定されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "行目）")
            End If
            HeaderKubunList.Add(sWork)
        Next
        HeaderKubun = HeaderKubunList.ToArray

        ' レコード区分一覧[@データ区分='データ']/レコード区分（必須）
        nodeList = mXmlRoot.SelectNodes("共通/レコード区分一覧[@データ区分='データ']/レコード区分")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/レコード区分一覧[@データ区分='データ']/レコード区分」タグが定義されていません。")
        End If
        Dim DataKubunList As New Generic.List(Of String)
        For Each element As XmlElement In nodeList
            sWork = element.InnerText.Trim
            If sWork = "" Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/レコード区分一覧/レコード区分」タグの値が指定されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "行目）")
            End If
            DataKubunList.Add(sWork)
        Next
        DataKubun = DataKubunList.ToArray

        ' レコード区分一覧[@データ区分='トレーラ']/レコード区分（必須）
        nodeList = mXmlRoot.SelectNodes("共通/レコード区分一覧[@データ区分='トレーラ']/レコード区分")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/レコード区分一覧[@データ区分='トレーラ']/レコード区分」タグが定義されていません。")
        End If
        Dim TrailerKubunList As New Generic.List(Of String)
        For Each element As XmlElement In nodeList
            sWork = element.InnerText.Trim
            If sWork = "" Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/レコード区分一覧/レコード区分」タグの値が指定されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "行目）")
            End If
            TrailerKubunList.Add(sWork)
        Next
        TrailerKubun = TrailerKubunList.ToArray

        ' レコード区分一覧[@データ区分='エンド']/レコード区分（必須）
        node = mXmlRoot.SelectSingleNode("共通/レコード区分一覧[@データ区分='エンド']/レコード区分")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/レコード区分一覧[@データ区分='エンド']/レコード区分」タグが定義されていません。")
        End If
        sWork = node.InnerText.Trim
        If sWork = "" Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/レコード区分一覧/レコード区分」タグの値が指定されていません。（" & _
                                ConfigurationErrorsException.GetLineNumber(node) & "行目）")
        End If
        EndKubun = sWork

        ' 最低分のレコード区分（必須）
        nodeList = mXmlRoot.SelectNodes("共通/最低分のレコード区分一覧/レコード区分")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/最低分のレコード区分一覧/レコード区分」タグが定義されていません。")
        End If
        Dim MinRecordCodeList As New Generic.List(Of String)
        Dim count As Integer = 0
        For Each element As XmlElement In nodeList
            count += 1
            sWork = element.InnerText.Trim
            If sWork = "" Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/最低分のレコード区分一覧/レコード区分」タグの値が指定されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "行目）")
            End If

            If count = 1 And sWork <> HeaderKubun(0) Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/最低分のレコード区分一覧/レコード区分」タグの値が「共通/レコード区分一覧[@データ区分='ヘッダ']/レコード区分」の値と異なります。（" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "行目）")
            End If

            If count = nodeList.Count And sWork <> EndKubun Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/最低分のレコード区分一覧/レコード区分」タグの値が「共通/レコード区分一覧[@データ区分='エンド']/レコード区分」の値と異なります。（" & _
                                    ConfigurationErrorsException.GetLineNumber(element) & "行目）")
            End If

            MinRecordCodeList.Add(sWork)
        Next
        DataInfo.MinRecordCode = MinRecordCodeList.ToArray

        ' 共通/フォーマット[@データ区分='']/データ一覧/データ"（必須）
        nodeList = mXmlRoot.SelectNodes("共通/フォーマット[@データ区分='ヘッダ']/データ一覧/データ")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='ヘッダ']/データ一覧/データ」タグが定義されていません。")
        End If
        mHeaderCount = nodeList.Count

        nodeList = mXmlRoot.SelectNodes("共通/フォーマット[@データ区分='データ']/データ一覧/データ")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='データ']/データ一覧/データ」タグが定義されていません。")
        End If
        mDataCount = nodeList.Count

        nodeList = mXmlRoot.SelectNodes("共通/フォーマット[@データ区分='トレーラ']/データ一覧/データ")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='トレーラ']/データ一覧/データ」タグが定義されていません。")
        End If
        mTrailerCount = nodeList.Count

        nodeList = mXmlRoot.SelectNodes("共通/フォーマット[@データ区分='エンド']/データ一覧/データ")
        If nodeList Is Nothing OrElse nodeList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='エンド']/データ一覧/データ」タグが定義されていません。")
        End If
        mEndCount = nodeList.Count

        ' 規定文字チェック個別メソッド名の取得（値省略可）
        node = mXmlRoot.SelectSingleNode("共通/フォーマット[@データ区分='ヘッダ']/規定文字チェックメソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='ヘッダ']/規定文字チェックメソッド」タグが定義されていません。")
        End If
        mChkHeaderRegularStrMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("共通/フォーマット[@データ区分='データ']/規定文字チェックメソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='データ']/規定文字チェックメソッド」タグが定義されていません。")
        End If
        mChkDataRegularStrMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("共通/フォーマット[@データ区分='トレーラ']/規定文字チェックメソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='トレーラ']/規定文字チェックメソッド」タグが定義されていません。")
        End If
        mChkTrailerRegularStrMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("共通/フォーマット[@データ区分='エンド']/規定文字チェックメソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='エンド']/規定文字チェックメソッド」タグが定義されていません。")
        End If
        mChkEndRegularStrMethod = node.InnerText.Trim

        ' 値を補正する個別メソッド名の取得（値省略可）
        node = mXmlRoot.SelectSingleNode("共通/フォーマット[@データ区分='ヘッダ']/値補正メソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='ヘッダ']/値補正メソッド」タグが定義されていません。")
        End If
        mChgHeaderRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("共通/フォーマット[@データ区分='データ']/値補正メソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='データ']/値補正メソッド」タグが定義されていません。")
        End If
        mChgDataRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("共通/フォーマット[@データ区分='トレーラ']/値補正メソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='トレーラ']/値補正メソッド」タグが定義されていません。")
        End If
        mChgTrailerRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("共通/フォーマット[@データ区分='エンド']/値補正メソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='エンド']/値補正メソッド」タグが定義されていません。")
        End If
        mChgEndRecMethod = node.InnerText.Trim


        ' 明細データ項目（必須）
        mMeisaiHeaderList = mXmlRoot.SelectNodes("落し込み[@データ区分='ヘッダ']/明細データ一覧/明細データ")
        If mMeisaiHeaderList Is Nothing OrElse mMeisaiHeaderList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み[@データ区分='ヘッダ']/明細データ一覧/明細データ」タグが定義されていません。")
        End If

        mMeisaiDataList1 = mXmlRoot.SelectNodes("落し込み[@データ区分='データ']/明細データ一覧/明細データ")
        If mMeisaiDataList1 Is Nothing OrElse mMeisaiDataList1.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み[@データ区分='データ']/明細データ一覧/明細データ」タグが定義されていません。")
        End If

        mMeisaiDataList2 = mXmlRoot.SelectNodes("落し込み[@データ区分='データ']/帳票項目一覧/明細データ")
        If mMeisaiDataList2 Is Nothing OrElse mMeisaiDataList2.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み[@データ区分='データ']/帳票項目一覧/明細データ」タグが定義されていません。")
        End If

        mMeisaiTrailerList = mXmlRoot.SelectNodes("落し込み[@データ区分='トレーラ']/明細データ一覧/明細データ")
        If mMeisaiTrailerList Is Nothing OrElse mMeisaiTrailerList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み[@データ区分='トレーラ']/明細データ一覧/明細データ」タグが定義されていません。")
        End If

        mMeisaiEndList = mXmlRoot.SelectNodes("落し込み[@データ区分='エンド']/明細データ一覧/明細データ")
        If mMeisaiEndList Is Nothing OrElse mMeisaiEndList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み[@データ区分='エンド']/明細データ一覧/明細データ」タグが定義されていません。")
        End If

        ' レコードチェック個別メソッド名の取得（値省略可）
        Dim methodname As String = ""
        node = mXmlRoot.SelectSingleNode("落し込み[@データ区分='ヘッダ']/レコードチェックメソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み[@データ区分='ヘッダ']/レコードチェックメソッド」タグが定義されていません。")
        End If
        mChkHeaderRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("落し込み[@データ区分='データ']/レコードチェックメソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み[@データ区分='データ']/レコードチェックメソッド」タグが定義されていません。")
        End If
        mChkDataRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("落し込み[@データ区分='トレーラ']/レコードチェックメソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み[@データ区分='トレーラ']/レコードチェックメソッド」タグが定義されていません。")
        End If
        mChkTrailerRecMethod = node.InnerText.Trim

        node = mXmlRoot.SelectSingleNode("落し込み[@データ区分='エンド']/レコードチェックメソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み[@データ区分='エンド']/レコードチェックメソッド」タグが定義されていません。")
        End If
        mChkEndRecMethod = node.InnerText.Trim

        ' 落し込み/カナ摘要設定（必須）
        mKanaTekiList = mXmlRoot.SelectNodes("落し込み/カナ摘要設定")
        If mKanaTekiList Is Nothing OrElse mKanaTekiList.Count = 0 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み/カナ摘要設定」タグが定義されていません。")
        End If

        ' 返還/返還データ作成[@データ区分='ヘッダ'（必須）
        node = mXmlRoot.SelectSingleNode("返還/返還データ作成[@データ区分='ヘッダ']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成[@データ区分='ヘッダ']」タグが定義されていません。")
        End If
        mHenkanHeaderNode = node

        ' 返還/返還データ作成[@データ区分='データ'（必須）
        node = mXmlRoot.SelectSingleNode("返還/返還データ作成[@データ区分='データ']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成[@データ区分='データ']」タグが定義されていません。")
        End If
        mHenkanDataNode = node

        ' 返還/返還データ作成[@データ区分='トレーラ'（必須）
        node = mXmlRoot.SelectSingleNode("返還/返還データ作成[@データ区分='トレーラ']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成[@データ区分='トレーラ']」タグが定義されていません。")
        End If
        mHenkanTrailerNode = node

        ' 返還/含0円データ（必須）
        node = mXmlRoot.SelectSingleNode("返還/含0円データ")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「返還/含0円データ」タグが定義されていません。")
        End If

        sWork = node.InnerText.Trim
        If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 1 Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「返還/含0円データ」タグの値（" & sWork & "）が不当です。（" & _
                                ConfigurationErrorsException.GetLineNumber(node) & "行目）")
        End If
        mInclude0Yen = CInt(sWork)


        ' 再振/再振データ作成[@データ区分='ヘッダ'（必須）
        node = mXmlRoot.SelectSingleNode("再振/再振データ作成[@データ区分='ヘッダ']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成[@データ区分='ヘッダ'」タグが定義されていません。")
        End If
        mSaifuriHeaderNode = node

        ' 再振/再振データ作成[@データ区分='データ'（必須）
        node = mXmlRoot.SelectSingleNode("再振/再振データ作成[@データ区分='データ']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成[@データ区分='データ'」タグが定義されていません。")
        End If
        mSaifuriDataNode = node

        ' 再振/再振データ作成[@データ区分='トレーラ'（必須）
        node = mXmlRoot.SelectSingleNode("再振/再振データ作成[@データ区分='トレーラ']")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成[@データ区分='トレーラ'」タグが定義されていません。")
        End If
        mSaifuriTrailerNode = node


        ' 登録出口メソッドタグチェック
        node = mXmlRoot.SelectSingleNode("登録出口/落し込み用登録出口メソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「登録出口/落し込み用登録出口メソッド」タグが定義されていません。")
        End If

        node = mXmlRoot.SelectSingleNode("登録出口/返還用登録出口メソッド")
        If node Is Nothing Then
            Throw New Exception(mXmlFile & "定義エラー：" & "「登録出口/返還用登録出口メソッド」タグが定義されていません。")
        End If

    End Sub


    ' 機能　 ： 規定文字チェック ＆　文字置換処理
    '
    ' 戻り値 ： 正常時は、-1
    '           異常時は、不正文字の位置
    ' 備考　 ： RepaceString()関数にて文字置換を実施
    '           置換対象文字は，不正文字にはならない
    '
    Public Overrides Function CheckRegularString() As Long

        Dim nRet As Long = -1

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckRegularString")
        End If

            If IsHeaderRecord() Then
                nRet = CheckRegularStringInternal("ヘッダ")
            ElseIf IsDataRecord() Then
                nRet = CheckRegularStringInternal("データ")
            ElseIf IsTrailerRecord() Then
                nRet = CheckRegularStringInternal("トレーラ")
            ElseIf IsEndRecord() Then
                nRet = CheckRegularStringInternal("エンド")
            End If

            Return nRet

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.CheckRegularString", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                If nRet <> -1 Then
                    BLOG.Write_LEVEL3("CFormatXML.CheckRegularString", "エラーレコード", RecordData)
                End If
                BLOG.Write_Exit3(sw, "CFormatXML.CheckRegularString", "復帰値=" & nRet)
            End If

        End Try

    End Function


    '
    ' 機能　 ： レコードを読み込んでチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー
    ' 備考　 ：
    '
    Public Overrides Function CheckDataFormat() As String

        Dim errPos As Integer = 0
        Dim sRet As String = ""
        Dim datakubun As String = ""    ' データ区分

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckDataFormat")
            End If

            ' 基底クラスのレコードデータチェック
            sRet = MyBase.CheckDataFormat()
            If sRet = "ERR" Then
                ' 規定外文字あり
                errPos = 1
                Return sRet
            End If

            If RecordData.Length = 0 Then
                DataInfo.Message = "ファイル異常"
                mnErrorNumber = 1
                errPos = 2
                sRet = "ERR"
                Return sRet
            End If

            ' ヘッダレコードの場合
            If IsHeaderRecord() Then
                If BeforeRecKbn <> "" And IsBeforeTrailerRecord() = False And IsBeforeEndRecord() = False Then
                    DataInfo.Message = "ファイルレコード（ヘッダ区分）異常"
                    mnErrorNumber = 1
                    errPos = 3
                    sRet = "ERR"
                    Return sRet
                Else
                    datakubun = "ヘッダ"

                    ' ヘッダレコードを明細データに設定
                    sRet = CheckRecord1()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord1()
                    End If
                End If

            ' データレコードの場合
            ElseIf IsDataRecord() Then
                If IsBeforeHeaderRecord() = False And IsBeforeDataRecord() = False Then
                    DataInfo.Message = "ファイルレコード（データ区分）異常"
                    mnErrorNumber = 1
                    errPos = 4
                    sRet = "ERR"
                    Return sRet
                Else
                    datakubun = "データ"

                    ' データレコードを明細データに設定
                    sRet = CheckRecord2()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord2()
                    End If
                End If

            ' トレーラレコードの場合
            ElseIf IsTrailerRecord() Then
                If IsBeforeHeaderRecord() = False And IsBeforeDataRecord() = False Then
                    DataInfo.Message = "ファイルレコード（トレーラ区分）異常"
                    mnErrorNumber = 1
                    errPos = 5
                    sRet = "ERR"
                    Return sRet
                Else
                    datakubun = "トレーラ"

                    ' トレーラレコードを明細データに設定
                    sRet = CheckRecord8()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord8()
                    End If
                End If

            ' エンドレコードの場合
            ElseIf IsEndRecord() Then
                'エンドレコードが複数あってもOK
                If IsBeforeTrailerRecord() = False Then
                    If IsBeforeEndRecord() = False Then
                        DataInfo.Message = "ファイルレコード（エンド区分）異常"
                        mnErrorNumber = 1
                        errPos = 6
                        sRet = "ERR"
                        Return sRet
                    Else
                        datakubun = "エンド"

                        ' エンドレコードを明細データに設定
                        sRet = CheckRecord9()
                        sRet = "99"
                    End If
                Else
                    datakubun = "エンド"

                    ' エンドレコードを明細データに設定
                    sRet = CheckRecord9()
                End If

                If sRet = "ERR" Then
                    errPos = 7
                    Return sRet
                End If

            ' その他のレコードの場合
            Else
                If RecordData.Substring(0, 1) = ChrW(&H1A) Then
                    If IsBeforeEndRecord() = False Then
                        DataInfo.Message = "レコード区分異常（1A）異常"
                        mnErrorNumber = 1
                        errPos = 8
                        sRet = "ERR"
                        Return sRet
                    Else
                        sRet = "1A"
                    End If
                Else
                    DataInfo.Message = "レコード区分異常（" & RecordData.Substring(0, 1) & "）異常"
                    mnErrorNumber = 1
                    errPos = 9
                    sRet = "ERR"
                    Return sRet
                End If
            End If


            If sRet <> "ERR" AndAlso datakubun <> "" Then
                ' レコードチェック個別メソッド名
                Dim methodname As String = ""
                If datakubun = "ヘッダ" Then
                    methodname = mChkHeaderRecMethod
                ElseIf datakubun = "データ" Then
                    methodname = mChkDataRecMethod
                ElseIf datakubun = "トレーラ" Then
                    methodname = mChkTrailerRecMethod
                Else
                    methodname = mChkEndRecMethod
                End If

                If methodname <> "" Then
                    If mClassInstance Is Nothing Then
                        sRet = "Exception"
                        Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」,「共通/個別クラス名」が定義されていません。")
                    End If

                    '----------------------------------------------------------------
                    ' レコードチェック個別メソッド呼出し
                    '----------------------------------------------------------------
                    If IS_LEVEL3 = True Then
                        BLOG.Write_LEVEL3("CFormatXML.CheckDataFormat", "レコードチェック個別メソッド呼出し", methodname)
                    End If

                    Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                    If methodInfo Is Nothing Then
                        sRet = "Exception"
                        Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み/レコードチェックメソッド」タグで指定された" & methodname & "が見つかりません。")
                    End If

                    Dim methodParams() As Object = {Me}
                    If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                        BLOG.Write_Err("CFormatXML.CheckDataFormat", "レコードチェック個別メソッドエラー", methodname)

                        errPos = 10
                        sRet = "ERR"
                        Return sRet
                    End If
                End If
            End If

            ' 基底クラスの後処理
            MyBase.CheckDataFormatAfter()

            Return sRet

        Catch ex As Exception
            ' 本メソッドでの例外の場合
            If sRet = "Exception" Then
                BLOG.Write_Err("CFormatXML.CheckDataFormat", ex)
            End If

            ' フォーマット変換エラーメッセージを設定
            DataInfo.Message = ex.Message

            Throw

        Finally
            If IS_LEVEL3 Then
                If sRet = "ERR" Then
                    BLOG.Write_LEVEL3("CFormatXML.CheckDataFormat", "エラー位置", CStr(errPos))
                    BLOG.Write_LEVEL3("CFormatXML.CheckDataFormat", "エラーレコード", RecordData)
                End If
            End If

            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.CheckDataFormat", "復帰値=" & sRet)
            End If

        End Try

    End Function


    '
    ' 機能　 ： ヘッダレコードチェック
    '
    ' 戻り値 ： "H" - ヘッダ
    '
    Public Overrides Function CheckRecord1() As String

        Dim sRet As String = ""

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckRecord1")
            End If

            ' レコードデータをデータ配列に変換
            Dim SplitedRecordData() As String = SplitRecordData("ヘッダ")

            ' 明細データ情報初期化
            Call InfoMeisaiMast.Init()

            ' 明細データ項目設定
            ' 定義に従い明細データ１に値を設定する
            SetMeisaiInfo(MEISAI_KIND_1, mMeisaiHeaderList, SplitedRecordData)

            sRet = "H"
            Return sRet

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.CheckRecord1", ex)

            ' フォーマット変換エラーメッセージを設定
            DataInfo.Message = ex.Message

            Throw

        Finally
            If sRet <> "" Then
                If IS_LEVEL3 = True Then
                    BLOG.Write_Exit3(sw, "CFormatXML.CheckRecord1", "復帰値=" & sRet)
                End If
            End If

        End Try

    End Function


    '
    ' 機能　 ： データレコードチェック
    '
    ' 戻り値 ： "D" - データ
    '
    Protected Overridable Function CheckRecord2() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckRecord2")
            End If

            ' レコードデータをデータ配列に変換
            Dim SplitedRecordData() As String = SplitRecordData("データ")

            ' 明細データ情報クリア
            Call InfoMeisaiMast.InitData()

            ' 明細データ項目設定
            ' 定義に従い明細データ１に値を設定する
            SetMeisaiInfo(MEISAI_KIND_1, mMeisaiDataList1, SplitedRecordData)

            ' 帳票出力項目設定
            ' 定義に従い明細データ２に値を設定する
            SetMeisaiInfo(MEISAI_KIND_2, mMeisaiDataList2, SplitedRecordData)

            sRet = "D"
            Return sRet

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.CheckRecord2", ex)

            ' フォーマット変換エラーメッセージを設定
            DataInfo.Message = ex.Message

            Throw

        Finally
            If sRet <> "" Then
                If IS_LEVEL3 = True Then
                    BLOG.Write_Exit3(sw, "CFormatXML.CheckRecord2", "復帰値=" & sRet)
                End If
            End If

        End Try

    End Function


    '
    ' 機能　 ： トレーラーレコードチェック
    '
    ' 戻り値 ： "T" - トレーラ
    '
    Protected Function CheckRecord8() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckRecord8")
            End If

            ' レコードデータをデータ配列に変換
            Dim SplitedRecordData() As String = SplitRecordData("トレーラ")

            ' 明細データ情報クリア
            Call InfoMeisaiMast.InitData()

            ' 明細データ項目設定
            ' 定義に従い明細データ１に値を設定する
            SetMeisaiInfo(MEISAI_KIND_1, mMeisaiTrailerList, SplitedRecordData)

            sRet = "T"
            Return sRet

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.CheckRecord8", ex)

            ' フォーマット変換エラーメッセージを設定
            DataInfo.Message = ex.Message

            Throw

        Finally
            If sRet <> "" Then
                If IS_LEVEL3 = True Then
                    BLOG.Write_Exit3(sw, "CFormatXML.CheckRecord8", "復帰値=" & sRet)
                End If
            End If

        End Try

    End Function


    '
    ' 機能　 ： エンドレコードチェック
    '
    ' 戻り値 ： "E" - エンド
    '
    Protected Function CheckRecord9() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.CheckRecord9")
            End If

            ' レコードデータをデータ配列に変換
            Dim SplitedRecordData() As String = SplitRecordData("エンド")

            ' 明細データ情報クリア
            Call InfoMeisaiMast.InitData()

            ' 明細データ項目設定
            ' 定義に従い明細データ１に値を設定する
            SetMeisaiInfo(MEISAI_KIND_1, mMeisaiEndList, SplitedRecordData)

            sRet = "E"
            Return sRet

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.CheckRecord9", ex)

            ' フォーマット変換エラーメッセージを設定
            DataInfo.Message = ex.Message

            Throw

        Finally
            If sRet <> "" Then
                If IS_LEVEL3 = True Then
                    BLOG.Write_Exit3(sw, "CFormatXML.CheckRecord9", "復帰値=" & sRet)
                End If
            End If

        End Try

    End Function


    ' 機能　 ： 明細データを返還ヘッダレコードに設定する
    '
    Public Overrides Sub GetHenkanHeaderRecord()

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetHenkanHeaderRecord")
            End If

            If IsHeaderRecord() = False Then
                Return
            End If

            ' ヘッダレコードを明細データに設定
            If CheckRecord1() = "ERR" Then
                Throw New Exception(Message)
            End If


            ' レコードデータをデータ配列に変換
            Dim SplitedRecordData() As String = SplitRecordData("ヘッダ")

            Dim node As XmlNode
            Dim methodname As String = ""

            ' 返還/返還データ作成@データ区分
            node = mHenkanHeaderNode

            node = node.SelectSingleNode("レコードデータ用個別メソッド")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成[@データ区分='ヘッダ']/レコードデータ用個別メソッド」タグが定義されていません。")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」,「共通/個別クラス名」が定義されていません。")
                End If

                '--------------------------------------------------
                ' レコードデータに対する個別メソッド呼出し
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetHenkanHeaderRecord", "レコードデータ用個別メソッド呼出し", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成/レコードデータ用個別メソッド」タグで指定された" & methodname & "が見つかりません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("レコードデータ用個別メソッドエラー（メソッド名：" & methodname & "）")
                End If
            End If


            ' 分割レコードデータ配列を１レコードにしてプロパティに設定
            RecordData = String.Join("", SplitedRecordData)

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetHenkanHeaderRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetHenkanHeaderRecord")
            End If

        End Try

    End Sub


    ' 機能　 ： 明細データを返還データレコードに設定する
    '
    Public Overrides Sub GetHenkanDataRecord()

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetHenkanDataRecord")
            End If

            If IsDataRecord() = False Then
                Return
            End If

            ' レコードデータをデータ配列に変換
            Dim SplitedRecordData() As String = SplitRecordData("データ")

            Dim node As XmlNode
            Dim attribute As XmlAttribute
            Dim sWork As String
            Dim methodname As String = ""
            Dim IndexFuriketu As Integer = 0

            ' 返還/返還データ作成@データ区分
            node = mHenkanDataNode

            ' 振替結果番号（必須）
            attribute = node.Attributes.ItemOf("振替結果番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成」タグの「振替結果番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成」タグの「振替結果番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexFuriketu = CInt(sWork) - 1

            ' 振替結果をセット
            SplitedRecordData(IndexFuriketu) = InfoMeisaiMast.FURIKETU_KEKKA

            ' EBCDICデータ対応：バイナリデータが存在する場合は書き換える
            If Not ReadByteBin Is Nothing Then
                ' バイナリデータの振替結果を書き換える
                Dim index As Integer = 0
                For i As Integer = 0 To IndexFuriketu - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetHenkanDataRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexFuriketu))
            End If

            node = node.SelectSingleNode("レコードデータ用個別メソッド")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成[@データ区分='データ']/レコードデータ用個別メソッド」タグが定義されていません。")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」,「共通/個別クラス名」が定義されていません。")
                End If

                '--------------------------------------------------
                ' レコードデータに対する個別メソッド呼出し
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetHenkanDataRecord", "レコードデータ用個別メソッド呼出し", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成/レコードデータ用個別メソッド」タグで指定された" & methodname & "が見つかりません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("レコードデータ用個別メソッドエラー（メソッド名：" & methodname & "）")
                End If
            End If


            ' 分割レコードデータ配列を１レコードにしてプロパティに設定
            RecordData = String.Join("", SplitedRecordData)

            ' データレコードを明細データに設定
            If CheckRecord2() = "ERR" Then
                Throw New Exception(Message)
            End If

            ' 基底クラスの後処理
            Call MyBase.GetHenkanDataRecord()

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetHenkanDataRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetHenkanDataRecord")
            End If

        End Try

    End Sub


    ' 機能　 ： 明細データを返還トレーラレコードに設定する
    '
    Public Overrides Sub GetHenkanTrailerRecord()

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetHenkanTrailerRecord")
            End If

            If IsTrailerRecord() = False Then
                Return
            End If

            ' トレーラレコードを明細データに設定
            If CheckRecord8() = "ERR" Then
                Throw New Exception(Message)
            End If

            ' レコードデータをデータ配列に変換
            Dim SplitedRecordData() As String = SplitRecordData("トレーラ")

            Dim node As XmlNode
            Dim attribute As XmlAttribute
            Dim sWork As String
            Dim methodname As String = ""
            Dim IndexNormKen As Integer = 0
            Dim IndexNormKin As Integer = 0
            Dim IndexIjoKen As Integer = 0
            Dim IndexIjoKin As Integer = 0

            ' 返還/返還データ作成@データ区分
            node = mHenkanTrailerNode

            ' 正常件数番号（必須）
            attribute = node.Attributes.ItemOf("正常件数番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成」タグの「正常件数番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成」タグの「正常件数番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexNormKen = CInt(sWork) - 1

            ' 正常金額番号（必須）
            attribute = node.Attributes.ItemOf("正常金額番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成」タグの「正常金額番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成」タグの「正常金額番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexNormKin = CInt(sWork) - 1

            ' 不能件数番号（必須）
            attribute = node.Attributes.ItemOf("不能件数番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成」タグの「不能件数番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成」タグの「不能件数番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexIjoKen = CInt(sWork) - 1

            ' 不能金額番号（必須）
            attribute = node.Attributes.ItemOf("不能金額番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成」タグの「不能金額番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成」タグの「不能金額番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexIjoKin = CInt(sWork) - 1

            ' 振替済み件数をセット
            ' 合計正常件数 計算値に、0円データを含める場合
            If mInclude0Yen = 1 Then
                SplitedRecordData(IndexNormKen) = InfoMeisaiMast.TOTAL_NORM_KEN.ToString.PadLeft(SplitedRecordData(IndexNormKen).Length, "0"c)
            ' 合計正常件数 計算値に、0円データを含めない場合
            Else
                SplitedRecordData(IndexNormKen) = InfoMeisaiMast.TOTAL_NORM_KEN2.ToString.PadLeft(SplitedRecordData(IndexNormKen).Length, "0"c)
            End If

            ' 振替済み金額をセット
            SplitedRecordData(IndexNormKin) = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(SplitedRecordData(IndexNormKin).Length, "0"c)

            ' 振替不能件数をセット
            SplitedRecordData(IndexIjoKen) = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(SplitedRecordData(IndexIjoKen).Length, "0"c)

            ' 振替不能金額をセット
            SplitedRecordData(IndexIjoKin) = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(SplitedRecordData(IndexIjoKin).Length, "0"c)

            'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
            If Not ReadByteBin Is Nothing Then
                ' バイナリデータの振替済み件数を書き換える
                Dim index As Integer = 0
                For i As Integer = 0 To IndexNormKen - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetHenkanTrailerRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexNormKen))

                ' バイナリデータの振替済み金額を書き換える
                index = 0
                For i As Integer = 0 To IndexNormKin - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetHenkanTrailerRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexNormKin))

                ' バイナリデータの振替不能件数を書き換える
                index = 0
                For i As Integer = 0 To IndexIjoKen - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetHenkanTrailerRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexIjoKen))

                ' バイナリデータの振替不能金額を書き換える
                index = 0
                For i As Integer = 0 To IndexIjoKin - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetHenkanTrailerRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexIjoKin))
            End If


            node = node.SelectSingleNode("レコードデータ用個別メソッド")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成[@データ区分='トレーラ']/レコードデータ用個別メソッド」タグが定義されていません。")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」,「共通/個別クラス名」が定義されていません。")
                End If

                '--------------------------------------------------
                ' レコードデータに対する個別メソッド呼出し
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetHenkanTrailerRecord", "レコードデータ用個別メソッド呼出し", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「返還/返還データ作成/レコードデータ用個別メソッド」タグで指定された" & methodname & "が見つかりません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("レコードデータ用個別メソッドエラー（メソッド名：" & methodname & "）")
                End If
            End If

            ' 分割レコードデータ配列を１レコードにしてプロパティに設定
            RecordData = String.Join("", SplitedRecordData)

            ' 基底クラスの後処理
            Call MyBase.GetHenkanTrailerRecord()

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetHenkanTrailerRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetHenkanTrailerRecord")
            End If

        End Try

    End Sub


    ' 機能　 ： 明細データを再振ヘッダレコードに設定する
    ' 引数   ： 振替日
    '
    Public Overrides Sub GetSaifuriHeaderRecord(ByVal SAIFURI_DATE As String)

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetSaifuriHeaderRecord")
            End If

            If IsHeaderRecord() = False Then
                Return
            End If

            ' ヘッダレコードを明細データに設定
            If CheckRecord1() = "ERR" Then
                Throw New Exception(Message)
            End If

            ' レコードデータをデータ配列に変換
            Dim SplitedRecordData() As String = SplitRecordData("ヘッダ")

            Dim node As XmlNode
            Dim attribute As XmlAttribute
            Dim sWork As String
            Dim methodname As String = ""
            Dim IndexFuridate As Integer = 0

            ' 再振/再振データ作成@データ区分
            node = mSaifuriHeaderNode

            ' 振替日番号（必須）
            attribute = node.Attributes.ItemOf("振替日番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「振替日番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「振替日番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexFuridate = CInt(sWork) - 1

            '再振日をセット
            SplitedRecordData(IndexFuridate) = SAIFURI_DATE

            'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
            If Not ReadByteBin Is Nothing Then
                ' バイナリデータの振替結果を書き換える
                Dim index As Integer = 0
                For i As Integer = 0 To IndexFuridate - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriHeaderRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexFuridate))
            End If

            node = node.SelectSingleNode("レコードデータ用個別メソッド")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成[@データ区分='ヘッダ']/レコードデータ用個別メソッド」タグが定義されていません。")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」,「共通/個別クラス名」が定義されていません。")
                End If

                '--------------------------------------------------
                ' レコードデータに対する個別メソッド呼出し
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetSaifuriHeaderRecord", "レコードデータ用個別メソッド呼出し", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成/レコードデータ用個別メソッド」タグで指定された" & methodname & "が見つかりません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("レコードデータ用個別メソッドエラー（メソッド名：" & methodname & "）")
                End If
            End If

            ' 分割レコードデータ配列を１レコードにしてプロパティに設定
            RecordData = String.Join("", SplitedRecordData)

            ' 基底クラスの後処理
            Call MyBase.GetSaifuriHeaderRecord(SAIFURI_DATE)

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetSaifuriHeaderRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetSaifuriHeaderRecord")
            End If

        End Try

    End Sub


    ' 機能　 ： 明細データを再振データレコードに設定する
    '
    Public Overrides Sub GetSaifuriDataRecord()

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetSaifuriDataRecord")
            End If

            If IsDataRecord() = False Then
                Return
            End If

            ' レコードデータをデータ配列に変換
            Dim SplitedRecordData() As String = SplitRecordData("データ")

            Dim node As XmlNode
            Dim attribute As XmlAttribute
            Dim sWork As String
            Dim methodname As String = ""
            Dim IndexFuriketu As Integer = 0

            ' 再振/再振データ作成@データ区分
            node = mSaifuriDataNode

            ' 振替結果番号（必須）
            attribute = node.Attributes.ItemOf("振替結果番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「振替結果番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「振替結果番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexFuriketu = CInt(attribute.Value.Trim) - 1


            ' 振替結果をセット
            SplitedRecordData(IndexFuriketu) = InfoMeisaiMast.FURIKETU_KEKKA

            ' EBCDICデータ対応：バイナリデータが存在する場合は書き換える
            If Not ReadByteBin Is Nothing Then
                ' バイナリデータの振替結果を書き換える
                Dim index As Integer = 0
                For i As Integer = 0 To IndexFuriketu - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriDataRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexFuriketu))
            End If

            node = node.SelectSingleNode("レコードデータ用個別メソッド")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成[@データ区分='データ']/レコードデータ用個別メソッド」タグが定義されていません。")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」,「共通/個別クラス名」が定義されていません。")
                End If

                '--------------------------------------------------
                ' レコードデータに対する個別メソッド呼出し
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetSaifuriDataRecord", "レコードデータ用個別メソッド呼出し", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成/レコードデータ用個別メソッド」タグで指定された" & methodname & "が見つかりません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("レコードデータ用個別メソッドエラー（メソッド名：" & methodname & "）")
                End If
            End If

            ' 分割レコードデータ配列を１レコードにしてプロパティに設定
            RecordData = String.Join("", SplitedRecordData)

            ' データレコードを明細データに設定
            If CheckRecord2() = "ERR" Then
                Throw New Exception(Message)
            End If

            ' 基底クラスの後処理
            Call MyBase.GetSaifuriDataRecord()

            'データレコードの振替結果を0にする
            '振替結果をセット
            SplitedRecordData(IndexFuriketu) = "0"

            'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
            If Not ReadByteBin Is Nothing Then
                ' バイナリデータの振替結果を書き換える
                Dim index As Integer = 0
                For i As Integer = 0 To IndexFuriketu - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriDataRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexFuriketu))
            End If

            ' 分割レコードデータ配列を１レコードにしてプロパティに設定
            RecordData = String.Join("", SplitedRecordData)

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetSaifuriDataRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetSaifuriDataRecord")
            End If

        End Try

    End Sub


    ' 機能　 ： 明細データを再振トレーラレコードに設定する
    ' 引数   ： 依頼合計
    '           金額合計
    '           引数の書込み有無
    '
    Public Overrides Sub GetSaifuriTrailerRecord(Optional ByVal SyoriKen As Long = 0, Optional ByVal SyoriKin As Long = 0, _
                                                       Optional ByVal Write As Boolean = False)

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL3 = True Then
                sw = BLOG.Write_Enter3("CFormatXML.GetSaifuriTrailerRecord")
            End If

            If IsTrailerRecord() = False Then
                Return
            End If

            ' トレーラレコードを明細データに設定
            If CheckRecord8() = "ERR" Then
                Throw New Exception(Message)
            End If

            ' レコードデータをデータ配列に変換
            Dim SplitedRecordData() As String = SplitRecordData("トレーラ")

            Dim node As XmlNode
            Dim attribute As XmlAttribute
            Dim sWork As String
            Dim methodname As String = ""
            Dim IndexKen As Integer = 0
            Dim IndexKin As Integer = 0
            Dim IndexNormKen As Integer = 0
            Dim IndexNormKin As Integer = 0
            Dim IndexIjoKen As Integer = 0
            Dim IndexIjoKin As Integer = 0

            ' 再振/再振データ作成@データ区分
            node = mSaifuriTrailerNode

            ' 依頼合計番号（必須）
            attribute = node.Attributes.ItemOf("依頼合計番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「依頼合計番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「依頼合計番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexKen = CInt(attribute.Value.Trim) - 1

            ' 金額合計番号（必須）
            attribute = node.Attributes.ItemOf("金額合計番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「金額合計番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「金額合計番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexKin = CInt(attribute.Value.Trim) - 1

            ' 正常件数番号（必須）
            attribute = node.Attributes.ItemOf("正常件数番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「正常件数番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「正常件数番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexNormKen = CInt(attribute.Value.Trim) - 1

            ' 正常金額番号（必須）
            attribute = node.Attributes.ItemOf("正常金額番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「正常金額番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「正常金額番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexNormKin = CInt(attribute.Value.Trim) - 1

            ' 不能件数番号（必須）
            attribute = node.Attributes.ItemOf("不能件数番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「不能件数番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「不能件数番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexIjoKen = CInt(attribute.Value.Trim) - 1

            ' 不能金額番号（必須）
            attribute = node.Attributes.ItemOf("不能金額番号")
            If attribute Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「不能金額番号」属性が定義されていません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            sWork = attribute.Value.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成」タグの「不能金額番号」属性の値（" & sWork & "）が不当です。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            IndexIjoKin = CInt(attribute.Value.Trim) - 1


           ' 振替不能件数をセット
            If Write = True Then
                SplitedRecordData(IndexKen) = SyoriKen.ToString.PadLeft(SplitedRecordData(IndexKen).Length, "0"c)
                SplitedRecordData(IndexKin) = SyoriKin.ToString.PadLeft(SplitedRecordData(IndexKin).Length, "0"c)
            Else
                SplitedRecordData(IndexKen) = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(SplitedRecordData(IndexKen).Length, "0"c)
                SplitedRecordData(IndexKin) = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(SplitedRecordData(IndexKin).Length, "0"c)
            End If

            ' 0をセット
            '合計正常件数
            SplitedRecordData(IndexNormKen) = "0".PadLeft(SplitedRecordData(IndexNormKen).Length, "0"c)
            ' 振替済み金額
            SplitedRecordData(IndexNormKin) = "0".PadLeft(SplitedRecordData(IndexNormKin).Length, "0"c)
            ' 振替不能件数
            SplitedRecordData(IndexIjoKen) = "0".PadLeft(SplitedRecordData(IndexIjoKen).Length, "0"c)
            ' 振替不能金額
            SplitedRecordData(IndexIjoKin) = "0".PadLeft(SplitedRecordData(IndexIjoKin).Length, "0"c)

            'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
            If Not ReadByteBin Is Nothing Then
                ' バイナリデータの依頼合計を書き換える
                Dim index As Integer = 0
                For i As Integer = 0 To IndexKen - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexKen))

                ' バイナリデータの金額合計を書き換える
                index = 0
                For i As Integer = 0 To IndexKin - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexKin))

                ' バイナリデータの振替済み件数を書き換える
                index = 0
                For i As Integer = 0 To IndexNormKen - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexNormKen))

                ' バイナリデータの振替済み金額を書き換える
                index = 0
                For i As Integer = 0 To IndexNormKin - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexNormKin))

                ' バイナリデータの振替不能件数を書き換える
                index = 0
                For i As Integer = 0 To IndexIjoKen - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexIjoKen))

                ' バイナリデータの振替不能金額を書き換える
                index = 0
                For i As Integer = 0 To IndexIjoKin - 1
                    index = index + SplitedRecordData(i).Length
                Next

                If IS_LEVEL4 = True Then
                    BLOG.Write_LEVEL4("CFormatXML.GetSaifuriTrailerRecord", "バイナリデータ振替結果書換え", "書換え位置=" & index)
                End If

                ReadByteBin.Insert(index, SplitedRecordData(IndexIjoKin))
            End If

            node = node.SelectSingleNode("レコードデータ用個別メソッド")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成[@データ区分='トレーラ']/レコードデータ用個別メソッド」タグが定義されていません。")
            End If
            methodname = node.InnerText.Trim

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」,「共通/個別クラス名」が定義されていません。")
                End If

                '--------------------------------------------------
                ' レコードデータに対する個別メソッド呼出し
                '--------------------------------------------------
                If IS_LEVEL3 = True Then
                    BLOG.Write_LEVEL3("CFormatXML.GetSaifuriTrailerRecord", "レコードデータ用個別メソッド呼出し", methodname)
                End If

                Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                If methodInfo Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「再振/再振データ作成/レコードデータ用個別メソッド」タグで指定された" & methodname & "が見つかりません。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If

                Dim methodParams() As Object = {Me, SplitedRecordData}
                If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                    Throw New Exception("レコードデータ用個別メソッドエラー（メソッド名：" & methodname & "）")
                End If
            End If

            ' 分割レコードデータ配列を１レコードにしてプロパティに設定
            RecordData = String.Join("", SplitedRecordData)

            ' 基底クラスの後処理
            Call MyBase.GetSaifuriTrailerRecord(SyoriKin, SyoriKin, Write)

        Catch ex As Exception
            BLOG.Write_Err("CFormatXML.GetSaifuriTrailerRecord", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                BLOG.Write_Exit3(sw, "CFormatXML.GetSaifuriTrailerRecord")
            End If

        End Try

    End Sub


    ' 機能　 ： 規定文字チェック ＆　文字置換内部処理
    '
    ' パラメータ ： データ区分
    '
    ' 戻り値 ： 不正文字の位置
    '
    ' 備考　 ： RepaceString()関数にて文字置換を実施
    '           置換対象文字は，不正文字にはならない
    '
    Private Function CheckRegularStringInternal(ByVal DataKubun As String) As Long

        Dim nRet As Long = -1
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.CheckRegularStringInternal",  DataKubun)
            End If

            Dim buff As New StringBuilder(DataInfo.LenOfOneRec)
            Dim RD() As Byte = EncdJ.GetBytes(RecordData)
            Dim sWork As String

            Dim list As List(Of XmlNode) = Nothing
            Dim count As Integer
            If DataKubun = "ヘッダ" Then
                list = mHeaderList
                count = mHeaderCount
            ElseIf DataKubun = "データ" Then
                list = mDataList
                count = mDataCount
            ElseIf DataKubun = "トレーラ" Then
                list = mTrailerList
                count = mTrailerCount
            Else
                list = mEndList
                count = mEndCount
            End If

            Dim copyIndex As Integer = 0 ' バイト配列からの部分文字列生成時のコピー位置

            Dim node As XmlNode = Nothing

            For i As Integer = 1 To count
                If list.Count < count Then
                    ' 共通/フォーマット[@データ区分='']/データ一覧/データ[@番号='']"（必須）
                    node = mXmlRoot.SelectSingleNode("共通/フォーマット[@データ区分='" & DataKubun & "']/データ一覧/データ[@番号='" & i & "']")
                    If node Is Nothing Then
                        Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='" & DataKubun & "']/データ一覧/データ[@番号='" & i & "']」タグが定義されていません。")
                    End If

                    list.Add(node)
                Else
                    node = list.Item(i - 1)
                End IF

                ' データ長の設定値チェック（必須）
                Dim dataLengthAttr As XmlAttribute = node.Attributes.ItemOf("データ長")
                If dataLengthAttr Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/データ一覧/データ」タグの「データ長」属性が定義されていません。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If
                sWork = dataLengthAttr.Value.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/データ一覧/データ」タグの「データ長」属性の値（" & sWork & "）が不当です。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If
                ' データ長の取得
                Dim dataLength As Integer = CInt(sWork)

                ' データ長の合計がレコードデータ長を超える場合
                If (copyIndex + dataLength) > DataInfo.RecoedLen Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/データ一覧/データ」タグの「データ長」属性の値（" & sWork & "）では合計がレコードデータ長を超えます。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If

                ' データ型の設定値チェック（必須）
                Dim dataTypeAttr As XmlAttribute = node.Attributes.ItemOf("データ型")
                If dataTypeAttr Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/データ一覧/データ」タグの「データ型」属性が定義されていません。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If
                sWork = dataTypeAttr.Value.Trim
                If sWork <> "文字列" AndAlso sWork <> "数値" Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/データ一覧/データ」タグの「データ型」属性の値（" & sWork & "）が不当です。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If
                ' データ型の取得
                Dim dataType As String = sWork

                ' 規定外文字判定の設定値チェック（必須）
                Dim dataCheck As Integer = 0 ' チェックしない
                Dim dataCheckAttr As XmlAttribute = node.Attributes.ItemOf("規定外文字判定")
                If dataCheckAttr Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/データ一覧/データ」タグの「規定外文字判定」属性が定義されていません。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If
                sWork = dataCheckAttr.Value.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 1 Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/データ一覧/データ」タグの「規定外文字判定」属性の値（" & sWork & "）が不当です。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If
                dataCheck = CInt(sWork)

                ' バイト配列から部分文字列生成
                Dim value As String = EncdJ.GetString(RD, copyIndex, dataLength)

                If dataType = "数値" Then
                    If value.Trim <> "" AndAlso IsNumeric(value.Trim) = False Then
                        Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='" & DataKubun & "']/データ一覧/データ[@番号='" & i & "' データ型='数値']」タグに対応するデータ（" & value.Trim & "）が数値ではありません。（" & _
                                            ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                    End If

                    buff.Append(value)
                Else
                    ' 文字列の場合、規定外文字列置換（規定外文字変換パターン.txtの置換）を実施
                    buff.Append(MyBase.ReplaceString(value))
                End If

                ' 規定外文字判定の実施（既に規定外文字があった場合はチェックしない＝最初の不正文字の位置を返す）
                If dataCheck = 1 AndAlso nRet = -1 Then
                    ' 規定文字チェック
                    Dim rc As Long = MyBase.CheckRegularStringVerA(value)
                    If rc = -1 Then
                        ' 規定文字チェックでかかるはずだが、全銀ロジックでは、規定文字チェック後に全角チェックが走るので合わせておく
                        rc = MyBase.GetZenkakuPos(value)
                    End If

                    If rc >= 0 Then
                        nRet = copyIndex + rc
                    End If

                End If

                copyIndex += dataLength
            Next

            ' データ長の合計がレコードデータ長に満たない場合
            If copyIndex < DataInfo.RecoedLen Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/データ一覧/データ」タグの「データ長」属性の合計がレコードデータ長より小さいです。（" & _
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            ' 読込んだプロパティのレコードを規定外文字変換後のレコードに置き換え
            If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                mRecordData = buff.ToString(0, RecordLen)
            End If

            ' 規定外文字判定で規定外文字が見つかった場合
            If nRet >= 0 Then
                ' 規定外文字があった場合、行末の改行コードなら正常扱いとする
                If nRet = DataInfo.RecoedLen - 2 Then
                    If mRecordData.Substring(DataInfo.RecoedLen - 2) = Environment.NewLine Then
                        nRet = -1
                    End If
                ElseIf nRet = DataInfo.RecoedLen - 1 Then
                    If mRecordData.Substring(DataInfo.RecoedLen - 1) = vbCr OrElse _
                       mRecordData.Substring(DataInfo.RecoedLen - 1) = vbLf Then
                        nRet = -1
                    End If
                End If

                If nRet >= 0 Then
                    Return nRet
                End If

            End If

            ' 規定文字チェック個別メソッド名
            Dim methodname As String = ""
            If DataKubun = "ヘッダ" Then
                methodname = mChkHeaderRegularStrMethod
            ElseIf DataKubun = "データ" Then
                methodname = mChkDataRegularStrMethod
            ElseIf DataKubun = "トレーラ" Then
                methodname = mChkTrailerRegularStrMethod
            Else
                methodname = mChkEndRegularStrMethod
            End If

            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」,「共通/個別クラス名」が定義されていません。")
                End If

                '----------------------------------------------------------------
                ' 規定文字チェック個別メソッド呼出し
                '----------------------------------------------------------------
                Try
                    If IS_LEVEL4 = True Then
                        BLOG.Write_LEVEL4("CFormatXML.CheckRegularStringInternal", "規定文字個別メソッド呼出し", methodname)
                    End If

                    Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                    If methodInfo Is Nothing Then
                        Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/規定文字チェックメソッド」タグで指定された" & methodname & "が見つかりません。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                    End If

                    Dim methodParams() As Object = {mRecordData}
                    nRet = CLng(methodInfo.Invoke(mClassInstance, methodParams))
                    If nRet >= 0 Then
                        BLOG.Write_Err("CFormatXML.CheckRegularStringInternal", "規定文字個別メソッドエラー", methodname)
                        Return nRet
                    End If
                Catch ex As Exception
                    BLOG.Write_Err("CFormatXML.CheckRegularStringInternal", ex)
                    Throw
                End Try
            End If

            Return nRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.CheckRegularStringInternal", "復帰値=" & nRet)
            End If

        End Try

    End Function


    '
    ' 機能　 ： ヘッダレコードチェック
    '
    ' 戻り値 ： "H" - ヘッダ
    '           "ERR" - エラーあり
    ' 備考　 ：
    '
    Private Function CheckDBRecord1() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.CheckDBRecord1")
            End If

            ' 基底クラスのヘッダレコードチェック
            If MyBase.CheckHeaderRecord() = False Then
                sRet = "ERR"
                Return sRet
            End If

            sRet = "H"
            Return sRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.CheckDBRecord1", "復帰値=" & sRet)
            End If

        End Try

    End Function


    '
    ' 機能　 ： データレコードチェック
    '
    ' 戻り値 ： "D" - データ
    '           "ERR" - エラーあり，"IJO" - インプットエラー
    ' 備考　 ：
    '
    Private Function CheckDBRecord2() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.CheckDBRecord2")
            End If

            Dim CheckRet As Boolean
            Dim sWork As String

            ' 基底クラスのデータレコードチェック
            CheckRet = MyBase.CheckDataRecord()

            ' カナ摘要設定
            InfoMeisaiMast.NTEKIYO = ""
            InfoMeisaiMast.KTEKIYO = ""

            If (Not mInfoComm Is Nothing) Then
                ' レコードデータをデータ配列に変換
                Dim SplitedRecordData() As String = SplitRecordData("データ")

                Dim tekiyou_kbn As String = mInfoComm.INFOToriMast.TEKIYOU_KBN_T
                Select Case tekiyou_kbn
                    Case "0"
                        InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                        InfoMeisaiMast.NTEKIYO = ""
                    Case "1"
                        InfoMeisaiMast.KTEKIYO = ""
                        InfoMeisaiMast.NTEKIYO = mInfoComm.INFOToriMast.NTEKIYOU_T
                    Case Else
                        If IS_LEVEL4 = True Then
                            BLOG.Write_LEVEL4("CFormatXML.CheckDBRecord2", "カナ摘要設定", "摘要区分=" & tekiyou_kbn)
                        End If

                        Dim element As XmlElement = Nothing
                        For Each element In mKanaTekiList
                            sWork = Element.GetAttribute("摘要区分")
                            If sWork Is Nothing OrElse sWork.Trim = "" Then
                                Throw New Exception(mXmlFile & "定義エラー：「落し込み/カナ摘要設定」タグの「摘要区分」属性が定義されていません。（" & _
                                                    ConfigurationErrorsException.GetLineNumber(element) & "行目）")
                            End If
                            sWork = sWork.Trim
                            If IsNumeric(sWork) = False Then
                                Throw New Exception(mXmlFile & "定義エラー：「落し込み/カナ摘要設定」タグの「摘要区分」属性の値（" & sWork & "）が不当です。（" & _
                                        ConfigurationErrorsException.GetLineNumber(element) & "行目）")
                            End If

                            ' 該当"摘要区分の場合
                            IF sWork = tekiyou_kbn Then
                                Exit For
                            End If

                            element = Nothing
                        Next

                        If element Is Nothing Then
                            Throw New Exception(mXmlFile & "定義エラー：「落し込み/カナ摘要設定」タグの「摘要区分」属性に該当する値（" & tekiyou_kbn & "）が定義されていません。")
                        End If

                        Dim datano As Integer = -1
                        Dim datalen As Integer = 0

                        sWork = Element.GetAttribute("データ番号")
                        If Not sWork Is Nothing AndAlso sWork.Trim <> "" Then
                            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData.Length Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み/カナ摘要設定」タグの「データ番号」属性の値（" & sWork & "）が不当です。（" & _
                                                    ConfigurationErrorsException.GetLineNumber(element) & "行目）")
                            End If
                            datano = CInt(sWork) - 1
                        End If

                        sWork = Element.GetAttribute("データ長")
                        If Not sWork Is Nothing AndAlso sWork.Trim <> "" Then
                            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 OrElse CInt(sWork) > SplitedRecordData(datano).Length Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「落し込み/カナ摘要設定」タグの「データ長」属性の値（" & sWork & "）が不当です。（" & _
                                                    ConfigurationErrorsException.GetLineNumber(element) & "行目）")
                            End If
                            datalen = CInt(sWork)
                        End If

                        If datano <> -1 Then
                            If datalen > 0 Then
                                InfoMeisaiMast.KTEKIYO = SplitedRecordData(datano).PadRight(datalen, " "c).Substring(0, datalen).Trim
                            Else
                                InfoMeisaiMast.KTEKIYO = SplitedRecordData(datano)
                            End If
                        End If
                End Select
            End If

            If CheckRet = False Then
                sRet = "IJO"
                Return sRet
            End If

            sRet = "D"
            Return sRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.CheckDBRecord2", "復帰値=" & sRet)
            End If

        End Try

    End Function


    '
    ' 機能　 ： トレーラーレコードチェック
    '
    ' 戻り値 ： "T" - トレーラ
    '           "ERR" - エラーあり
    ' 備考　 ：
    '
    Private Function CheckDBRecord8() As String

        Dim sRet As String = ""
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.CheckDBRecord8")
            End If

            ' 基底クラスのトレーラレコードチェック
            If MyBase.CheckTrailerRecord() = False Then
                sRet = "ERR"
                Return sRet
            End If

            sRet = "T"
            Return sRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.CheckDBRecord8", "復帰値=" & sRet)
            End If

        End Try

    End Function


    ' 機能　 ： 定義に従いレコードデータを分割して文字列配列を作成
    '
    ' 引数   ： DataKubun - データ区分
    '
    ' 戻り値 ： 分割されたレコードデータ
    '
    ' 備考　 ：
    '
    Private Function SplitRecordData(ByVal DataKubun As String) As String()

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.SplitRecordData")
            End If

            Dim node As XmlNode
            Dim sWork As String
            Dim dataLength As Integer = 0

            Dim list As List(Of XmlNode) = Nothing
            Dim count As Integer
            If DataKubun = "ヘッダ" Then
                list = mHeaderList
                count = mHeaderCount
            ElseIf DataKubun = "データ" Then
                list = mDataList
                count = mDataCount
            ElseIf DataKubun = "トレーラ" Then
                list = mTrailerList
                count = mTrailerCount
            Else
                list = mEndList
                count = mEndCount
            End If

            Dim SplitedRecordData(count - 1) As String

            ' レコードデータをデータ配列に変換
            Dim TmpData As String = String.Copy(RecordData)
            For i As Integer = 1 To count
                If list.Count < count Then
                    ' 共通/フォーマット[@データ区分='']/データ一覧/データ[@番号='']"（必須）
                    node = mXmlRoot.SelectSingleNode("共通/フォーマット[@データ区分='" & DataKubun & "']/データ一覧/データ[@番号='" & i & "']")
                    If node Is Nothing Then
                        Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット[@データ区分='" & DataKubun & "']/データ一覧/データ[@番号='" & i & "']」タグが定義されていません。")
                    End If

                    list.Add(node)
                Else
                    node = list.Item(i - 1)
                End IF

                ' データ長の設定値チェック（必須）
                Dim attribute As XmlAttribute = node.Attributes.ItemOf("データ長")
                If attribute Is Nothing 
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/データ一覧/データ」タグの「データ長」属性が定義されていません。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If
                sWork = attribute.Value.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/データ一覧/データ」タグの「データ長」属性の値（" & sWork & "）が不当です。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If

                ' データ長の合計がレコードデータ長を超える場合
                dataLength += CInt(sWork)
                If dataLength > DataInfo.RecoedLen Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/データ一覧/データ」タグの「データ長」属性の値（" & sWork & "）では合計がレコードデータ長を超えます。（" & _
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If

                SplitedRecordData(i - 1) = CuttingData(TmpData, CInt(sWork))
            Next

            ' レコード値補正個別メソッド
            Dim methodname As String = ""
            If DataKubun = "ヘッダ" Then
                methodname = mChgHeaderRecMethod
            ElseIf DataKubun = "データ" Then
                methodname = mChgDataRecMethod
            ElseIf DataKubun = "トレーラ" Then
                methodname = mChgTrailerRecMethod
            Else
                methodname = mChgEndRecMethod
            End If
            If methodname <> "" Then
                If mClassInstance Is Nothing Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」,「共通/個別クラス名」が定義されていません。")
                End If

                '-------------------------------------
                ' 値を補正する個別メソッド呼出し
                '-------------------------------------
                Try
                    If IS_LEVEL4 = True Then
                        BLOG.Write_LEVEL4("CFormatXML.SplitRecordData", "値を補正する個別メソッド呼出し", methodname)
                    End If

                    Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
                    If methodInfo Is Nothing Then
                        Throw New Exception(mXmlFile & "定義エラー：" & "「共通/フォーマット/値補正メソッド」タグで指定された" & methodname & "が見つかりません。")
                    End If

                    Dim methodParams() As Object = {SplitedRecordData}
                    If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = false Then
                        Throw New Exception("値を補正する個別メソッドエラー（メソッド名：" & methodname & "）")
                    End If
                Catch ex As Exception
                    BLOG.Write_Err("CFormatXML.SplitRecordData", ex)
                    Throw
                End Try
            End If

            Return SplitedRecordData

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.SplitRecordData")
            End If
        End Try

    End Function


    ' 機能　 ： 定義に従い明細データに値を設定する
    '
    ' 引数   ： type 明細データ種別（MEISAI_KIND_1: 明細データ一覧,  MEISAI_KIND_2: 帳票項目一覧）
    '        ： nodeList 明細データを定義したXMLノードリスト
    '        ： SplitedRecordData 分割されたレコードデータ
    '
    Private Sub SetMeisaiInfo(ByVal type As Integer, ByRef nodeList As XmlNodeList, ByRef SplitedRecordData() As String)

        Dim Column As String = ""
        Dim sValue As String = ""
        Dim sValue_org As String = ""
        Dim element As XmlElement = Nothing
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.SetMeisaiInfo")
            End If

            Dim sTagName As String
            Dim sWork As String
            Dim iValue As Integer

            Dim InfoType As Type
            Dim Target As Object
            Dim valueType As ValueType
            Dim fieldInfo As FieldInfo


            If type = MEISAI_KIND_1 Then
                sTagName = "「落し込み/明細データ一覧/明細データ」"
            Else
                sTagName = "「落し込み/帳票項目一覧/明細データ」"
            End If

            Dim no As Integer = 0
            For Each element In nodeList
                Column = ""
                sValue = ""
                sValue_org = ""

                ' 明細データの番号（必須）
                no += 1
                sWork = Element.GetAttribute("番号")
                If sWork Is Nothing OrElse sWork.Trim <> CStr(no) Then
                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「番号」属性の値（" & sWork & "）が不当です。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End If

                ' 明細データの列名（必須）
                sWork = Element.GetAttribute("列名")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「列名」属性の値（" & sWork & "）が不当です。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End If
                Column = sWork.Trim

                ' 固定値設定（必須）
                sWork = Element.GetAttribute("固定値設定")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「固定値設定」属性が定義されていません。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End If
                sWork = sWork.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 1 Then
                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「固定値設定」属性の値（" & sWork & "）が不当です。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End If
                iValue = CInt(sWork)

                ' 値（必須）
                sWork = Element.GetAttribute("値")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「値」属性が定義されていません。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End If
                sWork = sWork.Trim

                ' 1行データ内の値を設定する場合
                If iValue = 0 Then
                    Dim Indexes() As String = sWork.Split(","c)
                    For Each index As String In Indexes
                        If Not IsNumeric(index) OrElse CInt(index) <= 0 OrElse CInt(index) > SplitedRecordData.Length Then
                            Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「値」属性の値（" & sWork & "）が不当です。（" & _
                                                ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                        End If

                        sValue = sValue & SplitedRecordData(CInt(index) - 1)
                    Next
               ' 固定値の場合
                Else
                    sValue = sWork
                End If

                sValue_org = sValue
                Dim ValueLength As Integer = sValue.Length

                ' 空白文字の削除（必須）
                sWork = Element.GetAttribute("空白文字削除")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「空白文字削除」属性が定義されていません。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End If
                sWork = sWork.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 3 Then
                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「空白文字削除」属性の値（" & sWork & "）が不当です。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End If
                iValue = CInt(sWork)

                '前後の空白文字を削除する場合
                If iValue = 1 Then
                    sValue = sValue.Trim
                '先頭の空白文字を削除する場合
                ElseIf iValue = 2 Then
                    sValue = sValue.TrimStart
                '末尾の空白文字を削除する場合
                ElseIf iValue = 3 Then
                    sValue = sValue.TrimEnd
                End If

                ' 使用文字（値省略可）
                Dim paddingChar As Char
                sWork = Element.GetAttribute("使用文字")
                If Not sWork Is Nothing AndAlso sWork <> "" Then
                    If System.Text.Encoding.GetEncoding(932).GetByteCount(sWork) <> 1 Then
                        Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「使用文字」属性の値（" & sWork & "）が不当です。（" & _
                                            ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                    End If
                    paddingChar = CChar(sWork)
                Else
                    paddingChar = " "c  ' デフォルト： 半角空白
                End If

                ' 文字埋め（必須）
                sWork = Element.GetAttribute("文字埋め")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「文字埋め」属性が定義されていません。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End If
                sWork = sWork.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 2 Then
                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「文字埋め」属性の値（" & sWork & "）が不当です。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End If
                iValue = CInt(sWork)

                '先頭を文字埋めする場合
                If iValue = 1 Then
                    sValue = sValue.PadLeft(ValueLength, paddingChar)
                '末尾を文字埋めする場合
                ElseIf iValue = 2 Then
                    sValue = sValue.PadRight(ValueLength, paddingChar)
                End If

                ' 日付書式（値省略可）
                Dim fmt As String
                sWork = Element.GetAttribute("日付書式")
                If Not sWork Is Nothing AndAlso sWork.Trim <> "" Then
                    fmt = sWork.Trim
                Else
                    fmt = "yyyyMMdd"  ' デフォルト
                End If

                ' 日付変換（必須）
                sWork = Element.GetAttribute("日付変換")
                If sWork Is Nothing OrElse sWork.Trim = "" Then
                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「日付変換」属性が定義されていません。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End If
                sWork = sWork.Trim
                If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 1 Then
                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「日付変換」属性の値（" & sWork & "）が不当です。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End If
                iValue = CInt(sWork)

                ' 日付変換する場合
                If iValue = 1 Then
                    sValue = CASTCommon.ConvertDate(sValue, fmt)
                End If

                Try
                    If type = MEISAI_KIND_1 Then
                        '明細データの設定対象のメンバ変数を取得
                        InfoType = GetType(MEISAI)
                        Target = InfoType.InvokeMember(Column, _
                            BindingFlags.Public Or BindingFlags.NonPublic Or _
                            BindingFlags.Instance Or BindingFlags.GetField, _
                            Nothing, _
                            InfoMeisaiMast, _
                            Nothing)

                        'メンバ変数へ値を設定
                        valueType = InfoMeisaiMast
                        fieldInfo = InfoMeisaiMast.GetType().GetField(Column)
                        If TypeOf Target Is Integer Then
                            '設定値がIntegerの場合
                            fieldInfo.SetValue(valueType, CInt(sValue))
                        ElseIf TypeOf Target Is Decimal Then
                            '設定値がDecimalの場合
                            fieldInfo.SetValue(valueType, CDec(sValue))
                        Else
                            '設定値がStringの場合
                            fieldInfo.SetValue(valueType, sValue)
                        End If

                        InfoMeisaiMast = DirectCast(valueType, MEISAI)

                    Else
                        '帳票出力項目の設定対象のメンバ変数を取得
                        InfoType = GetType(MEISAI2)
                        Target = InfoType.InvokeMember(Column, _
                            BindingFlags.Public Or BindingFlags.NonPublic Or _
                            BindingFlags.Instance Or BindingFlags.GetField, _
                            Nothing, _
                            InfoMeisaiMast2, _
                            Nothing)

                        'メンバ変数へ値を設定
                        valueType = InfoMeisaiMast2
                        fieldInfo = InfoMeisaiMast2.GetType().GetField(Column)
                        If TypeOf Target Is Integer Then
                            '設定値がIntegerの場合
                            fieldInfo.SetValue(valueType, CInt(sValue))
                        ElseIf TypeOf Target Is Decimal Then
                            '設定値がDecimalの場合
                            fieldInfo.SetValue(valueType, CDec(sValue))
                        Else
                            '設定値がStringの場合
                            fieldInfo.SetValue(valueType, sValue)
                        End If

                        InfoMeisaiMast2 = DirectCast(valueType, MEISAI2)
                    End If
                Catch ex As Exception
                    BLOG.Write_Err("CFormatXML.SetMeisaiInfo", ex)

                    Throw New Exception(mXmlFile & "定義エラー：" & sTagName & "タグの「列名」属性の値（" & Column & "）で指定された明細データへの設定に失敗しました。（" & _
                                        ConfigurationErrorsException.GetLineNumber(Element) & "行目）")
                End Try
            Next

        Catch ex As Exception
            If Not Element Is Nothing AndAlso IS_LEVEL4 Then
                BLOG.Write_LEVEL4("CFormatXML.SetMeisaiInfo", "エラーXMLノード", ConfigurationErrorsException.GetLineNumber(Element) & "行目")
                BLOG.Write_LEVEL4("CFormatXML.SetMeisaiInfo", "エラー列名", Column)
                BLOG.Write_LEVEL4("CFormatXML.SetMeisaiInfo", "エラー設定値（補正前）", sValue_org)
                BLOG.Write_LEVEL4("CFormatXML.SetMeisaiInfo", "エラー設定値", sValue)
            End If

            Throw

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.SetMeisaiInfo")
            End If

        End Try

    End Sub


    '
    ' 機能　 ： 直前のレコード区分がヘッダレコードが判定
    ' 戻り値 ： True - ヘッダレコード
    '
    Private Function IsBeforeHeaderRecord() As Boolean

        Dim bRet As Boolean = False
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.IsBeforeHeaderRecord")
            End If

            For i As Integer = 0 To HeaderKubun.Length - 1
                If BeforeRecKbn = HeaderKubun(i) Then
                    bRet = True
                    Return bRet
                End If
            Next i

            Return bRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.IsBeforeHeaderRecord", "復帰値=" & bRet)
            End If

        End Try

    End Function

    '
    ' 機能　 ： 直前のレコード区分がデータレコードが判定
    ' 戻り値 ： True - データレコード
    '
    Private Function IsBeforeDataRecord() As Boolean

        Dim bRet As Boolean = False
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.IsBeforeDataRecord")
            End If

            For i As Integer = 0 To DataKubun.Length - 1
                If BeforeRecKbn = DataKubun(i) Then
                    bRet = True
                    Return bRet
                End If
            Next i

            Return bRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.IsBeforeDataRecord", "復帰値=" & bRet)
            End If

        End Try

    End Function

    '
    ' 機能　 ： 直前のレコード区分がトレーラレコードが判定
    ' 戻り値 ： True - トレーラーレコード
    '
    Private Function IsBeforeTrailerRecord() As Boolean

        Dim bRet As Boolean = False
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.IsBeforeTrailerRecord")
            End If

            For i As Integer = 0 To TrailerKubun.Length - 1
                If BeforeRecKbn = TrailerKubun(i) Then
                    bRet = True
                    Return bRet
                End If
            Next i

            Return bRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.IsBeforeTrailerRecord", "復帰値=" & bRet)
            End If

        End Try

    End Function

    '
    ' 機能　 ： 直前のレコード区分がエンドレコードが判定
    ' 戻り値 ： True - エンドレコード
    '
    Private Function IsBeforeEndRecord() As Boolean

        Dim bRet As Boolean = False
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            If IS_LEVEL4 = True Then
                sw = BLOG.Write_Enter4("CFormatXML.IsBeforeEndRecord")
            End If

            If BeforeRecKbn = EndKubun Then
                bRet = True
                Return bRet
            End If

            Return bRet

        Finally
            If IS_LEVEL4 = True Then
                BLOG.Write_Exit4(sw, "CFormatXML.IsBeforeEndRecord", "復帰値=" & bRet)
            End If

        End Try

    End Function

End Class
'*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応（新規作成） ***
