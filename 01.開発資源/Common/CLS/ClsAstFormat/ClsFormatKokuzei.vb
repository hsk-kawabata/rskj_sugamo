Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' 国税 データフォーマットクラス
Public Class CFormatKokuzei
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 390

    '------------------------------------------
    '国税フォーマット
    '------------------------------------------
    'ファイル先頭レコード（Ａ）
    Structure KOKUZEI_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public KZ1 As String         'データ区分(=1)
        <VBFixedString(2)> Public KZ2 As String         'ファイル区分
        <VBFixedString(19)> Public KZ3 As String        'ダミー
        <VBFixedString(3)> Public KZ4 As String         '科目コード
        <VBFixedString(2)> Public KZ5 As String         '年度
        <VBFixedString(2)> Public KZ6 As String         '課税年分
        <VBFixedString(1)> Public KZ7 As String         '納期区分
        <VBFixedString(7)> Public KZ8 As String         '納期カナ文字
        <VBFixedString(2)> Public KZ9 As String         '徴定区分
        <VBFixedString(6)> Public KZ10 As String        '発送年月日
        <VBFixedString(6)> Public KZ11 As String        '振替日
        <VBFixedString(6)> Public KZ12 As String        '課税期間（自）
        <VBFixedString(6)> Public KZ13 As String        '課税期間（至）
        <VBFixedString(325)> Public KZ14 As String      'ダミー
        <VBFixedString(2)> Public KZ15 As String        '依頼ファイルＮＯ
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(KZ1, 1), _
                            SubData(KZ2, 2), _
                            SubData(KZ3, 19), _
                            SubData(KZ4, 3), _
                            SubData(KZ5, 2), _
                            SubData(KZ6, 2), _
                            SubData(KZ7, 1), _
                            SubData(KZ8, 7), _
                            SubData(KZ9, 2), _
                            SubData(KZ10, 6), _
                            SubData(KZ11, 6), _
                            SubData(KZ12, 6), _
                            SubData(KZ13, 6), _
                            SubData(KZ14, 325), _
                            SubData(KZ15, 2) _
                            })
            End Get
            Set(ByVal Value As String)
                KZ1 = CuttingData(Value, 1)
                KZ2 = CuttingData(Value, 2)
                KZ3 = CuttingData(Value, 19)
                KZ4 = CuttingData(Value, 3)
                KZ5 = CuttingData(Value, 2)
                KZ6 = CuttingData(Value, 2)
                KZ7 = CuttingData(Value, 1)
                KZ8 = CuttingData(Value, 7)
                KZ9 = CuttingData(Value, 2)
                KZ10 = CuttingData(Value, 6)
                KZ11 = CuttingData(Value, 6).Replace(" "c, "0")
                KZ12 = CuttingData(Value, 6)
                KZ13 = CuttingData(Value, 6)
                KZ14 = CuttingData(Value, 325)
                KZ15 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure

    Public KOKUZEI_REC1 As KOKUZEI_RECORD1

    '署別金融機関店舗別名称レコード（Ｂ）
    '署別金融機関店舗別トータルレコード（Ｄ）
    '署別金融機関別トータルレコード（Ｅ）
    Structure KOKUZEI_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public KZ1 As String         'データ区分(=2)
        <VBFixedString(2)> Public KZ2 As String         'ファイル区分
        <VBFixedString(5)> Public KZ3 As String         '局署番号
        <VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
        <VBFixedString(9)> Public KZ5 As String         'ダミー
        <VBFixedString(5)> Public KZ6 As String         '日銀コード
        <VBFixedString(10)> Public KZ7 As String        '税務署名
        <VBFixedString(7)> Public KZ8 As String         'ダミー
        <VBFixedString(5)> Public KZ9 As String         '税務署郵便番号
        <VBFixedString(7)> Public KZ10 As String        '取扱金融機関番号
        <VBFixedString(5)> Public KZ11 As String        '金融機関郵便番号
        <VBFixedString(7)> Public KZ12 As String        'ダミー
        <VBFixedString(6)> Public KZ13 As String        '送付分件数
        <VBFixedString(12)> Public KZ14 As String       '送付分合計金額
        <VBFixedString(6)> Public KZ15 As String        '振替納付不能件数
        <VBFixedString(12)> Public KZ16 As String       '振替納付不能合計
        <VBFixedString(6)> Public KZ17 As String        '振替納付件数
        <VBFixedString(12)> Public KZ18 As String       '振替納付合計金額
        <VBFixedString(5)> Public KZ19 As String        'ダミー
        <VBFixedString(8)> Public KZ20 As String        '税務署電話番号
        <VBFixedString(8)> Public KZ21 As String        '金融機関電話番
        <VBFixedString(27)> Public KZ22 As String       'ダミー
        <VBFixedString(30)> Public KZ23 As String       '都市区名
        <VBFixedString(30)> Public KZ24 As String       '所在地Ⅰ
        <VBFixedString(30)> Public KZ25 As String       '所在地Ⅱ
        <VBFixedString(30)> Public KZ26 As String       '肩書
        <VBFixedString(30)> Public KZ27 As String       '金融機関名称Ⅰ
        <VBFixedString(30)> Public KZ28 As String       '金融機関名称Ⅱ
        <VBFixedString(30)> Public KZ29 As String       '店舗名称
        <VBFixedString(1)> Public KZ30 As String        '補充記入
        <VBFixedString(5)> Public KZ31 As String        'ダミー
        <VBFixedString(2)> Public KZ32 As String        '依頼ファイルＮＯ
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(KZ1, 1), _
                            SubData(KZ2, 2), _
                            SubData(KZ3, 5), _
                            SubData(KZ4, 7), _
                            SubData(KZ5, 9), _
                            SubData(KZ6, 5), _
                            SubData(KZ7, 10), _
                            SubData(KZ8, 7), _
                            SubData(KZ9, 5), _
                            SubData(KZ10, 7), _
                            SubData(KZ11, 5), _
                            SubData(KZ12, 7), _
                            SubData(KZ13, 6), _
                            SubData(KZ14, 12), _
                            SubData(KZ15, 6), _
                            SubData(KZ16, 12), _
                            SubData(KZ17, 6), _
                            SubData(KZ18, 12), _
                            SubData(KZ19, 5), _
                            SubData(KZ20, 8), _
                            SubData(KZ21, 8), _
                            SubData(KZ22, 27), _
                            SubData(KZ23, 30), _
                            SubData(KZ24, 30), _
                            SubData(KZ25, 30), _
                            SubData(KZ26, 30), _
                            SubData(KZ27, 30), _
                            SubData(KZ28, 30), _
                            SubData(KZ29, 30), _
                            SubData(KZ30, 1), _
                            SubData(KZ31, 5), _
                            SubData(KZ32, 2) _
                            })
            End Get
            Set(ByVal Value As String)
                KZ1 = CuttingData(Value, 1)
                KZ2 = CuttingData(Value, 2)
                KZ3 = CuttingData(Value, 5)
                KZ4 = CuttingData(Value, 7)
                KZ5 = CuttingData(Value, 9)
                KZ6 = CuttingData(Value, 5)
                KZ7 = CuttingData(Value, 10)
                KZ8 = CuttingData(Value, 7)
                KZ9 = CuttingData(Value, 5)
                KZ10 = CuttingData(Value, 7)
                KZ11 = CuttingData(Value, 5)
                KZ12 = CuttingData(Value, 7)
                KZ13 = CuttingData(Value, 6)
                KZ14 = CuttingData(Value, 12)
                KZ15 = CuttingData(Value, 6)
                KZ16 = CuttingData(Value, 12)
                KZ17 = CuttingData(Value, 6)
                KZ18 = CuttingData(Value, 12)
                KZ19 = CuttingData(Value, 5)
                KZ20 = CuttingData(Value, 8)
                KZ21 = CuttingData(Value, 8)
                KZ22 = CuttingData(Value, 27)
                KZ23 = CuttingData(Value, 30)
                KZ24 = CuttingData(Value, 30)
                KZ25 = CuttingData(Value, 30)
                KZ26 = CuttingData(Value, 30)
                KZ27 = CuttingData(Value, 30)
                KZ28 = CuttingData(Value, 30)
                KZ29 = CuttingData(Value, 30)
                KZ30 = CuttingData(Value, 1)
                KZ31 = CuttingData(Value, 5)
                KZ32 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure
    Public KOKUZEI_REC2 As KOKUZEI_RECORD2

    '個別明細レコード（Ｃ）
    Structure KOKUZEI_RECORD3
        Implements CFormat.IFormat

        <VBFixedString(1)> Public KZ1 As String         'データ区分(=3)
        <VBFixedString(2)> Public KZ2 As String         'ファイル区分(=91)
        <VBFixedString(5)> Public KZ3 As String         '局署番号
        <VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
        <VBFixedString(7)> Public KZ5 As String         '納税者番号
        <VBFixedString(1)> Public KZ6 As String         '継承区分
        <VBFixedString(1)> Public KZ7 As String         '補完表示区分
        <VBFixedString(1)> Public KZ8 As String         '振替結果コード
        <VBFixedString(10)> Public KZ9 As String        '納付税額
        <VBFixedString(9)> Public KZ10 As String        '内利子税
        <VBFixedString(1)> Public KZ11 As String        '預金種別
        <VBFixedString(7)> Public KZ12 As String        '口座番号
        <VBFixedString(8)> Public KZ13 As String        '整理番号
        <VBFixedString(69)> Public KZ14 As String       'ダミー
        <VBFixedString(7)> Public KZ15 As String        '郵便番号（7桁）
        <VBFixedString(5)> Public KZ16 As String        '郵便番号（5桁）
        <VBFixedString(1)> Public KZ17 As String        '補完表示
        <VBFixedString(7)> Public KZ18 As String        '取扱金融機関番号
        <VBFixedString(7)> Public KZ19 As String        'ダミー
        <VBFixedString(6)> Public KZ20 As String        '市外局番(納税者)
        <VBFixedString(8)> Public KZ21 As String        '電話番号(納税者)
        <VBFixedString(2)> Public KZ22 As String        'ダミー
        <VBFixedString(23)> Public KZ23 As String       '都市区分
        <VBFixedString(23)> Public KZ24 As String       '住所Ⅰ
        <VBFixedString(23)> Public KZ25 As String       '住所Ⅱ
        <VBFixedString(23)> Public KZ26 As String       '住所Ⅲ
        <VBFixedString(23)> Public KZ27 As String       '肩書Ⅰ
        <VBFixedString(23)> Public KZ28 As String       '肩書Ⅱ
        <VBFixedString(23)> Public KZ29 As String       '肩書Ⅲ
        <VBFixedString(23)> Public KZ30 As String       '納税者名Ⅰ
        <VBFixedString(23)> Public KZ31 As String       '納税者名Ⅱ
        <VBFixedString(5)> Public KZ32 As String        '納貯番号
        <VBFixedString(3)> Public KZ33 As String        '口座番号
        <VBFixedString(1)> Public KZ34 As String        '継続区分
        <VBFixedString(2)> Public KZ35 As String        '依頼ファイルＮＯ
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(KZ1, 1), _
                            SubData(KZ2, 2), _
                            SubData(KZ3, 5), _
                            SubData(KZ4, 7), _
                            SubData(KZ5, 7), _
                            SubData(KZ6, 1), _
                            SubData(KZ7, 1), _
                            SubData(KZ8, 1), _
                            SubData(KZ9, 10), _
                            SubData(KZ10, 9), _
                            SubData(KZ11, 1), _
                            SubData(KZ12, 7), _
                            SubData(KZ13, 8), _
                            SubData(KZ14, 69), _
                            SubData(KZ15, 7), _
                            SubData(KZ16, 5), _
                            SubData(KZ17, 1), _
                            SubData(KZ18, 7), _
                            SubData(KZ19, 7), _
                            SubData(KZ20, 6), _
                            SubData(KZ21, 8), _
                            SubData(KZ22, 2), _
                            SubData(KZ23, 23), _
                            SubData(KZ24, 23), _
                            SubData(KZ25, 23), _
                            SubData(KZ26, 23), _
                            SubData(KZ27, 23), _
                            SubData(KZ28, 23), _
                            SubData(KZ29, 23), _
                            SubData(KZ30, 23), _
                            SubData(KZ31, 23), _
                            SubData(KZ32, 5), _
                            SubData(KZ33, 3), _
                            SubData(KZ34, 1), _
                            SubData(KZ35, 2) _
                            })
            End Get
            Set(ByVal Value As String)
                KZ1 = CuttingData(Value, 1)
                KZ2 = CuttingData(Value, 2)
                KZ3 = CuttingData(Value, 5)
                KZ4 = CuttingData(Value, 7)
                KZ5 = CuttingData(Value, 7)
                KZ6 = CuttingData(Value, 1)
                KZ7 = CuttingData(Value, 1)
                KZ8 = CuttingData(Value, 1)
                KZ9 = CuttingData(Value, 10)
                KZ10 = CuttingData(Value, 9)
                KZ11 = CuttingData(Value, 1)
                KZ12 = CuttingData(Value, 7)
                KZ13 = CuttingData(Value, 8)
                KZ14 = CuttingData(Value, 69)
                KZ15 = CuttingData(Value, 7)
                KZ16 = CuttingData(Value, 5)
                KZ17 = CuttingData(Value, 1)
                KZ18 = CuttingData(Value, 7)
                KZ19 = CuttingData(Value, 7)
                KZ20 = CuttingData(Value, 6)
                KZ21 = CuttingData(Value, 8)
                KZ22 = CuttingData(Value, 2)
                KZ23 = CuttingData(Value, 23)
                KZ24 = CuttingData(Value, 23)
                KZ25 = CuttingData(Value, 23)
                KZ26 = CuttingData(Value, 23)
                KZ27 = CuttingData(Value, 23)
                KZ28 = CuttingData(Value, 23)
                KZ29 = CuttingData(Value, 23)
                KZ30 = CuttingData(Value, 23)
                KZ31 = CuttingData(Value, 23)
                KZ32 = CuttingData(Value, 5)
                KZ33 = CuttingData(Value, 3)
                KZ34 = CuttingData(Value, 1)
                KZ35 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure
    Public KOKUZEI_REC3 As KOKUZEI_RECORD3

    'ファイル合計レコード（Ｆ）
    Structure KOKUZEI_RECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public KZ1 As String         'データ区分(=8)
        <VBFixedString(2)> Public KZ2 As String         'ファイル区分(=91)
        <VBFixedString(5)> Public KZ3 As String         '局署番号
        <VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
        <VBFixedString(10)> Public KZ5 As String        'ダミー
        <VBFixedString(45)> Public KZ6 As String        'ダミー
        <VBFixedString(6)> Public KZ7 As String         '送付分件数
        <VBFixedString(12)> Public KZ8 As String        '送付分合計金額
        <VBFixedString(6)> Public KZ9 As String         '振替納付不能件数
        <VBFixedString(12)> Public KZ10 As String       '振替納付不能合計金額
        <VBFixedString(6)> Public KZ11 As String        '振替納付件数
        <VBFixedString(12)> Public KZ12 As String       '振替納付合計金額
        <VBFixedString(264)> Public KZ13 As String      'ダミー
        <VBFixedString(2)> Public KZ14 As String        '依頼ファイルＮＯ
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(KZ1, 1), _
                            SubData(KZ2, 2), _
                            SubData(KZ3, 5), _
                            SubData(KZ4, 7), _
                            SubData(KZ5, 10), _
                            SubData(KZ6, 45), _
                            SubData(KZ7, 6), _
                            SubData(KZ8, 12), _
                            SubData(KZ9, 6), _
                            SubData(KZ10, 12), _
                            SubData(KZ11, 6), _
                            SubData(KZ12, 12), _
                            SubData(KZ13, 264), _
                            SubData(KZ14, 2) _
                            })
            End Get
            Set(ByVal Value As String)
                KZ1 = CuttingData(Value, 1)
                KZ2 = CuttingData(Value, 2)
                KZ3 = CuttingData(Value, 5)
                KZ4 = CuttingData(Value, 7)
                KZ5 = CuttingData(Value, 10)
                KZ6 = CuttingData(Value, 45)
                KZ7 = CuttingData(Value, 6)
                KZ8 = CuttingData(Value, 12)
                KZ9 = CuttingData(Value, 6)
                KZ10 = CuttingData(Value, 12)
                KZ11 = CuttingData(Value, 6)
                KZ12 = CuttingData(Value, 12)
                KZ13 = CuttingData(Value, 264)
                KZ14 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure
    Public KOKUZEI_REC8 As KOKUZEI_RECORD8

    'ファイルエンドレコード（Ｇ）
    Structure KOKUZEI_RECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public KZ1 As String         'データ区分(=9)
        <VBFixedString(2)> Public KZ2 As String         'ファイル区分(=91)
        <VBFixedString(5)> Public KZ3 As String         '局署番号
        <VBFixedString(7)> Public KZ4 As String         '全銀協統一コード
        <VBFixedString(10)> Public KZ5 As String        'ダミー
        <VBFixedString(45)> Public KZ6 As String        'ダミー
        <VBFixedString(6)> Public KZ7 As String         '送付分件数
        <VBFixedString(12)> Public KZ8 As String        '送付分合計金額
        <VBFixedString(6)> Public KZ9 As String         '振替納付不能件数
        <VBFixedString(12)> Public KZ10 As String       '振替納付不能金額
        <VBFixedString(6)> Public KZ11 As String        '振替納付件数
        <VBFixedString(12)> Public KZ12 As String       '振替納付金額
        <VBFixedString(264)> Public KZ13 As String      'ダミー
        <VBFixedString(2)> Public KZ14 As String        '依頼ファイルＮＯ
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(KZ1, 1), _
                            SubData(KZ2, 2), _
                            SubData(KZ3, 5), _
                            SubData(KZ4, 7), _
                            SubData(KZ5, 10), _
                            SubData(KZ6, 45), _
                            SubData(KZ7, 6), _
                            SubData(KZ8, 12), _
                            SubData(KZ9, 6), _
                            SubData(KZ10, 12), _
                            SubData(KZ11, 6), _
                            SubData(KZ12, 12), _
                            SubData(KZ13, 264), _
                            SubData(KZ14, 2) _
                            })
            End Get
            Set(ByVal Value As String)
                KZ1 = CuttingData(Value, 1)
                KZ2 = CuttingData(Value, 2)
                KZ3 = CuttingData(Value, 5)
                KZ4 = CuttingData(Value, 7)
                KZ5 = CuttingData(Value, 10)
                KZ6 = CuttingData(Value, 45)
                KZ7 = CuttingData(Value, 6)
                KZ8 = CuttingData(Value, 12)
                KZ9 = CuttingData(Value, 6)
                KZ10 = CuttingData(Value, 12)
                KZ11 = CuttingData(Value, 6)
                KZ12 = CuttingData(Value, 12)
                KZ13 = CuttingData(Value, 264)
                KZ14 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure
    Public KOKUZEI_REC9 As KOKUZEI_RECORD9

    Private TOTAL_SIT_NORM_KEN As Decimal = 0           ' 金融機関店舗別 振替納付件数
    Private TOTAL_SIT_NORM_KIN As Decimal = 0           ' 金融機関店舗別 振替納付金額
    Private TOTAL_SIT_IJO_KEN As Decimal = 0            ' 金融機関店舗別 振替納付不能件数
    Private TOTAL_SIT_IJO_KIN As Decimal = 0            ' 金融機関店舗別 振替納付不能金額
    Private TOTAL_KIN_NORM_KEN As Decimal = 0           ' 金融機関別 振替納付件数
    Private TOTAL_KIN_NORM_KIN As Decimal = 0           ' 金融機関別 振替納付金額
    Private TOTAL_KIN_IJO_KEN As Decimal = 0            ' 金融機関別 振替納付不能件数
    Private TOTAL_KIN_IJO_KIN As Decimal = 0            ' 金融機関別 振替納付不能金額

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"1", "2", "3", "4", "5", "8", "9"}

        FtranPfile = "390.P"
        FtranIBMPfile = "390IBM.P"
        FtranIBMBinaryPfile = "390READ.P"

        CMTBlockSize = 3900

        HeaderKubun = New String() {"1"}
        DataKubun = New String() {"2", "3", "4", "5"}
        TrailerKubun = New String() {"8"}
    End Sub

    '
    ' 機能　 ： レコードチェック
    '
    ' 戻り値 ： 不正文字の位置
    '
    ' 備考　 ：
    '
    Public Overrides Function CheckRegularString() As Long
        Return MyBase.CheckRegularString()
    End Function

    '
    ' 機能　 ： レコードを読み込んでチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー
    '
    ' 備考　 ：
    '
    Public Overrides Function CheckDataFormat() As String
        Dim ErrString As String = ""

        ' 基本クラス チェック
        Dim sRet As String = MyBase.CheckDataFormat()
        If sRet = "ERR" Then
            '' 規定外文字あり
            'Return "ERR"
            ErrString = DataInfo.Message
            If ErrString = "" Then
                ErrString = "ERR"
            End If
        End If

        If RecordData.Length = 0 Then
            DataInfo.Message = "ファイル異常"
            mnErrorNumber = 1
            Return "ERR"
        End If

        Select Case RecordData.Substring(0, 1)
            Case "1"
                If BeforeRecKbn <> "" And BeforeRecKbn <> "8" And BeforeRecKbn <> "9" Then
                    DataInfo.Message = "ファイルレコード（ヘッダ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord1()
                    '*** 修正 mitsu 2008/05/21 ヘッダに委託者情報がないためチェックしない ***
                    'If sRet <> "ERR" Then
                    '    sRet = CheckDBRecord1()
                    'End If
                    '************************************************************************
                End If
            Case "2"
                If BeforeRecKbn <> "1" And BeforeRecKbn <> "4" And BeforeRecKbn <> "5" Then
                    DataInfo.Message = "ファイルレコード（データ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord2()
                End If
            Case "3"
                If BeforeRecKbn <> "2" And BeforeRecKbn <> "3" Then
                    DataInfo.Message = "ファイルレコード（データ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord3()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord3()
                    End If
                End If
            Case "4"
                If BeforeRecKbn <> "3" Then
                    DataInfo.Message = "ファイルレコード（データ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord4()
                End If
            Case "5"
                If BeforeRecKbn <> "4" Then
                    DataInfo.Message = "ファイルレコード（データ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord5()
                End If
            Case "8"
                If BeforeRecKbn <> "5" Then
                    DataInfo.Message = "ファイルレコード（トレーラ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord8()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord8()
                    End If
                End If
            Case "9"
                If BeforeRecKbn <> "8" Then
                    DataInfo.Message = "ファイルレコード（エンド区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord9()
                End If
            Case Else
                DataInfo.Message = "レコード区分異常（" & RecordData.Substring(0, 1) & "）異常"
                mnErrorNumber = 1
                Return "ERR"
        End Select

        ' 各フォーマット　共通後処理
        MyBase.CheckDataFormatAfter()

        If ErrString <> "" Then
            ' 規定外文字あり
            Return ErrString
        End If

        Return sRet
    End Function

    '
    ' 機能　 ： ヘッダレコードチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー

    ' 備考　 ：
    '
    Public Overrides Function CheckRecord1() As String
        KOKUZEI_REC1.Data = RecordData

        ' 明細マスタ情報初期化
        Call InfoMeisaiMast.Init()

        ' 明細マスタ項目設定
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC1.KZ1
        InfoMeisaiMast.SYUBETU_CODE = KOKUZEI_REC1.KZ2
        InfoMeisaiMast.ITAKU_CODE = ""

        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(KOKUZEI_REC1.KZ11, "yyyyMMdd")
        If InfoMeisaiMast.FURIKAE_DATE = "" Then
            InfoMeisaiMast.FURIKAE_DATE = New String("0"c, 8)
        End If
        InfoMeisaiMast.FURIKAE_DATE_MOTO = KOKUZEI_REC1.KZ11

        InfoMeisaiMast.ITAKU_KIN = ""
        InfoMeisaiMast.ITAKU_SIT = ""
        InfoMeisaiMast.ITAKU_KAMOKU = ""
        InfoMeisaiMast.ITAKU_KOUZA = ""
        InfoMeisaiMast.ITAKU_KNAME = ""

        InfoMeisaiMast.I_SIT_NNAME = ""

        '***
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader
        Dim strRet As String = ""

        ' ＤＢ接続が存在しない場合，正常値を返す
        If OraDB Is Nothing Then
            Return ""
        End If

        ' 口振
        SQL.Append("SELECT")
        SQL.Append(" TORIS_CODE_T,TORIF_CODE_T,TYUUDAN_FLG_S,TOUROKU_FLG_S")
        SQL.Append(" FROM TORIMAST,SCHMAST")
        SQL.Append(" WHERE FURI_DATE_S  = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
        SQL.Append("   AND TORIS_CODE_S =  " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
        SQL.Append("   AND TORIF_CODE_S = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
        SQL.Append("   AND '1'          = FSYORI_KBN_T")
        SQL.Append("   AND TORIS_CODE_S = TORIS_CODE_T")
        SQL.Append("   AND TORIF_CODE_S = TORIF_CODE_T")

        OraReader = New CASTCommon.MyOracleReader(OraDB.OracleConnection, OraDB.OracleTransaction)
        If OraReader.DataReader(SQL) = False Then

            WriteBLog("取引先またはスケジュール検索", "失敗", "委託者コード：" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"))
            DataInfo.Message = "取引先またはｽｹｼﾞｭｰﾙ検索失敗"

            OraReader.Close()

            ' 取引先マスタ情報をクリアする
            Call mInfoComm.GetTORIMAST("", "")

            Return "ERR"

        Else
            ' 取引先マスタを取得
            Call mInfoComm.GetTORIMAST(OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))

        End If

        If OraReader.EOF = False AndAlso OraReader.GetItem("TYUUDAN_FLG_S") <> "0" Then
            WriteBLog("スケジュール:落し込み処理済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"), "中断")
            DataInfo.Message = "中断フラグ設定済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
            OraReader.Close()
            Return "ERR"
        End If

        If OraReader.EOF = False AndAlso OraReader.GetItem("TOUROKU_FLG_S") <> "0" Then
            WriteBLog("スケジュール:落し込み処理済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"), "中断")
            DataInfo.Message = "落し込み処理済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
            OraReader.Close()
            Return "ERR"
        End If

        ' オラクルReaderクローズ
        OraReader.Close()
        WriteBLog("取引先検索", "成功", "取引先コード：" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
        Return "H"

    End Function

    Private Function CheckDBRecord1() As String

        'データチェック
        If MyBase.CheckHeaderRecord() = False Then
            Return "ERR"
        End If

        Return "H"
    End Function

    '
    ' 機能　 ： データレコードチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー

    ' 備考　 ：
    '
    Protected Overridable Function CheckRecord2() As String
        KOKUZEI_REC2.Data = RecordData

        '明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC2.KZ1
        InfoMeisaiMast.ITAKU_KIN = KOKUZEI_REC2.KZ4.Substring(0, 4)
        InfoMeisaiMast.ITAKU_SIT = KOKUZEI_REC2.KZ4.Substring(4, 3)

        '*** 修正 maeda 2008/05/12************************************************
        '帳票出力項目用に金融機関名、店舗名を追加
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = KOKUZEI_REC2.KZ27
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = KOKUZEI_REC2.KZ28
        '*************************************************************************


        Return "D"
    End Function

    '
    ' 機能　 ： データレコードチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー

    ' 備考　 ：
    '
    Protected Overridable Function CheckRecord3() As String
        KOKUZEI_REC3.Data = RecordData

        '明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC3.KZ1
        InfoMeisaiMast.KEIYAKU_KIN = KOKUZEI_REC3.KZ4.Substring(0, 4)
        InfoMeisaiMast.KEIYAKU_SIT = KOKUZEI_REC3.KZ4.Substring(4, 3)
        InfoMeisaiMast.KEIYAKU_KAMOKU = KOKUZEI_REC3.KZ11
        InfoMeisaiMast.KEIYAKU_KOUZA = KOKUZEI_REC3.KZ12.Trim.PadLeft(7, "0"c)
        InfoMeisaiMast.KEIYAKU_KNAME = KOKUZEI_REC3.KZ30.Trim & KOKUZEI_REC3.KZ31.Trim
        If InfoMeisaiMast.KEIYAKU_KNAME.Length > 40 Then
            InfoMeisaiMast.KEIYAKU_KNAME = InfoMeisaiMast.KEIYAKU_KNAME.Substring(0, 40)
        End If
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(KOKUZEI_REC3.KZ9.TrimStart)
        InfoMeisaiMast.FURIKIN_MOTO = KOKUZEI_REC3.KZ9.TrimStart
        InfoMeisaiMast.SINKI_CODE = "0"
        InfoMeisaiMast.KEIYAKU_NO = KOKUZEI_REC3.KZ5
        InfoMeisaiMast.JYUYOKA_NO = KOKUZEI_REC3.KZ20 & KOKUZEI_REC3.KZ21
        '2011/06/16 標準版修正 国税の摘要をMEIMASTに反映 ------------------START

        ' 摘要
        InfoMeisaiMast.NTEKIYO = ""
        InfoMeisaiMast.KTEKIYO = ""
        Try
            If (Not mInfoComm Is Nothing) Then
                Select Case mInfoComm.INFOToriMast.TEKIYOU_KBN_T
                    Case "0"
                        InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                        InfoMeisaiMast.NTEKIYO = ""
                    Case "1"
                        InfoMeisaiMast.KTEKIYO = ""
                        InfoMeisaiMast.NTEKIYO = mInfoComm.INFOToriMast.NTEKIYOU_T
                    Case Else
                        Return "ERR"
                End Select
            End If
        Catch ex As Exception
        End Try
        '2011/06/16 標準版修正 国税の摘要をMEIMASTに反映 ------------------END

        InfoMeisaiMast.FURIKETU_CODE = CASTCommon.CAInt32(KOKUZEI_REC3.KZ8)
        InfoMeisaiMast.FURIKETU_MOTO = KOKUZEI_REC3.KZ8

        '*** 修正 maeda 2008/05/12************************************************
        '帳票出力項目用に金融機関名、店舗名を追加
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ""
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ""
        '*************************************************************************


        ' 依頼件数，依頼金額 カウント対象レコード
        InfoMeisaiMast.FURIKEN = 1

        Return "D"
    End Function

    Private Function CheckDBRecord3() As String

        'データチェック
        If MyBase.CheckDataRecord() = False Then
            Return "IJO"
        End If

        Return "D"
    End Function

    Protected Overridable Function CheckRecord4() As String
        KOKUZEI_REC2.Data = RecordData

        '明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC2.KZ1

        '*** 修正 maeda 2008/05/12************************************************
        '帳票出力項目用に金融機関名、店舗名を追加
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ""
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ""
        '*************************************************************************


        Return "D"
    End Function

    Protected Overridable Function CheckRecord5() As String
        KOKUZEI_REC2.Data = RecordData

        '明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC2.KZ1

        '*** 修正 maeda 2008/05/12************************************************
        '帳票出力項目用に金融機関名、店舗名を追加
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ""
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ""
        '*************************************************************************

        Return "D"
    End Function

    '
    ' 機能　 ： トレーラーレコードチェック
    '
    ' 戻り値 ： True - 成功， False - 失敗
    '
    ' 備考　 ：
    '
    Protected Function CheckRecord8() As String
        KOKUZEI_REC8.Data = RecordData

        ' 明細マスタ項目設定 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC8.KZ1
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ7.TrimStart)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ8.TrimStart)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ11.TrimStart)
        InfoMeisaiMast.TOTAL_ZUMI_KIN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ12.TrimStart)
        InfoMeisaiMast.TOTAL_FUNO_KEN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ9.TrimStart)
        InfoMeisaiMast.TOTAL_FUNO_KIN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ10.TrimStart)

        InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = KOKUZEI_REC8.KZ7.TrimStart
        InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = KOKUZEI_REC8.KZ8.TrimStart
        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = KOKUZEI_REC8.KZ11.TrimStart
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = KOKUZEI_REC8.KZ12.TrimStart
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = KOKUZEI_REC8.KZ9.TrimStart
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = KOKUZEI_REC8.KZ10.TrimStart

        Return "T"
    End Function

    Private Function CheckDBRecord8() As String

        'データチェック
        If MyBase.CheckTrailerRecord() = False Then
            Return "ERR"
        End If

        Return "T"
    End Function

    '
    ' 機能　 ： エンドレコードチェック
    '
    ' 戻り値 ： True - 成功， False - 失敗
    '
    ' 備考　 ：
    '
    Protected Function CheckRecord9() As String
        KOKUZEI_REC9.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC9.KZ1

        Return "E"
    End Function

    ' 機能　 ： 返還データレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overrides Sub GetHenkanDataRecord()
        If mRecordData.StartsWith("3") = False Then
            ' 個別明細レコード以外の場合は，処理しない
            Return
        End If

        KOKUZEI_REC3.Data = RecordData

        ' 振替結果をセット
        KOKUZEI_REC3.KZ8 = InfoMeisaiMast.FURIKETU_KEKKA

        '*** 修正 mitsu 2008/09/30 EBCDICデータ対応 ***
        'バイナリデータが存在する場合は書き換える
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(24, KOKUZEI_REC3.KZ8)
        End If
        '**********************************************

        RecordData = KOKUZEI_REC3.Data

        ' レコードデータを分析
        Call CheckRecord3()

        '　署別金融機関店舗別，署別金融機関別 集計
        Try
            If InfoMeisaiMast.FURIKETU_CODE = 0 Then
                ' 振替済み件数，振替済み金額 
                TOTAL_SIT_NORM_KEN += InfoMeisaiMast.FURIKEN
                TOTAL_KIN_NORM_KEN += InfoMeisaiMast.FURIKEN
                If InfoMeisaiMast.FURIKEN = 1 Then
                    TOTAL_SIT_NORM_KIN += InfoMeisaiMast.FURIKIN
                    TOTAL_KIN_NORM_KIN += InfoMeisaiMast.FURIKIN
                End If
            Else
                ' 異常件数，異常金額
                TOTAL_SIT_IJO_KEN += InfoMeisaiMast.FURIKEN
                TOTAL_KIN_IJO_KEN += InfoMeisaiMast.FURIKEN
                If InfoMeisaiMast.FURIKEN = 1 Then
                    TOTAL_SIT_IJO_KIN += InfoMeisaiMast.FURIKIN
                    TOTAL_KIN_IJO_KIN += InfoMeisaiMast.FURIKIN
                End If
            End If

            ' レコード区分を保存
            BeforeRecKbn = RecordData.Substring(0, 1)
        Catch ex As Exception

        End Try

        Call MyBase.GetHenkanDataRecord()
    End Sub

    ' 機能　 ： 返還トレーラレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overrides Sub GetHenkanTrailerRecord()
        ' レコードデータを分析
        If mRecordData.StartsWith("4") = True Then
            ' 署別金融機関店舗別トータルレコード
            Call CheckRecord4()

            ' 振替不能件数をセット
            KOKUZEI_REC2.KZ15 = TOTAL_SIT_IJO_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC2.KZ16 = TOTAL_SIT_IJO_KIN.ToString.PadLeft(12, " "c)
            ' 振替済み件数をセット
            KOKUZEI_REC2.KZ17 = TOTAL_SIT_NORM_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC2.KZ18 = TOTAL_SIT_NORM_KIN.ToString.PadLeft(12, " "c)

            '*** 修正 mitsu 2008/09/30 EBCDICデータ対応 ***
            'バイナリデータが存在する場合は書き換える
            If Not ReadByteBin Is Nothing Then
                ReadByteBin.Insert(88, KOKUZEI_REC2.KZ15)
                ReadByteBin.Insert(94, KOKUZEI_REC2.KZ16)
                ReadByteBin.Insert(106, KOKUZEI_REC2.KZ17)
                ReadByteBin.Insert(112, KOKUZEI_REC2.KZ18)
            End If
            '**********************************************

            RecordData = KOKUZEI_REC2.Data

            TOTAL_SIT_IJO_KEN = 0
            TOTAL_SIT_IJO_KIN = 0
            TOTAL_SIT_NORM_KEN = 0
            TOTAL_SIT_NORM_KIN = 0
        ElseIf mRecordData.StartsWith("5") = True Then
            ' 署別金融機関別トータルレコード
            Call CheckRecord5()

            ' 振替不能件数をセット
            KOKUZEI_REC2.KZ15 = TOTAL_KIN_IJO_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC2.KZ16 = TOTAL_KIN_IJO_KIN.ToString.PadLeft(12, " "c)
            ' 振替済み件数をセット
            KOKUZEI_REC2.KZ17 = TOTAL_KIN_NORM_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC2.KZ18 = TOTAL_KIN_NORM_KIN.ToString.PadLeft(12, " "c)

            '*** 修正 mitsu 2008/09/30 EBCDICデータ対応 ***
            'バイナリデータが存在する場合は書き換える
            If Not ReadByteBin Is Nothing Then
                ReadByteBin.Insert(88, KOKUZEI_REC2.KZ15)
                ReadByteBin.Insert(94, KOKUZEI_REC2.KZ16)
                ReadByteBin.Insert(106, KOKUZEI_REC2.KZ17)
                ReadByteBin.Insert(112, KOKUZEI_REC2.KZ18)
            End If
            '**********************************************

            RecordData = KOKUZEI_REC2.Data

            TOTAL_KIN_IJO_KEN = 0
            TOTAL_KIN_IJO_KIN = 0
            TOTAL_KIN_NORM_KEN = 0
            TOTAL_KIN_NORM_KIN = 0
        ElseIf mRecordData.StartsWith("8") = True Then
            ' ファイル合計レコード
            Call CheckRecord8()

            ' 振替済み件数をセット
            KOKUZEI_REC8.KZ11 = InfoMeisaiMast.TOTAL_NORM_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC8.KZ12 = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(12, " "c)
            ' 振替不能件数をセット
            KOKUZEI_REC8.KZ9 = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC8.KZ10 = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(12, " "c)

            '*** 修正 mitsu 2008/09/30 EBCDICデータ対応 ***
            'バイナリデータが存在する場合は書き換える
            If Not ReadByteBin Is Nothing Then
                ReadByteBin.Insert(88, KOKUZEI_REC8.KZ9)
                ReadByteBin.Insert(94, KOKUZEI_REC8.KZ10)
                ReadByteBin.Insert(106, KOKUZEI_REC8.KZ11)
                ReadByteBin.Insert(112, KOKUZEI_REC8.KZ12)
            End If
            '**********************************************

            RecordData = KOKUZEI_REC8.Data
        End If

        Call MyBase.GetHenkanTrailerRecord()
    End Sub
End Class
