Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' しんきんフォーマット（レコード長５３０）クラス
Public Class CFormatShinkin530
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 530

    ' ------------------------------------------
    ' しんきんフォーマット
    ' ------------------------------------------
    ' 各レコード共通部
    Public Structure SINKIN128_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(4)> Public JF1 As String     '組合コード
        <VBFixedString(3)> Public JF2 As String     '店舗コード
        <VBFixedString(1)> Public JF3 As String     'レコード種別
        <VBFixedString(4)> Public JF4 As String     '持込組合コード
        <VBFixedString(1)> Public JF5 As String     '返還区分コード
        <VBFixedString(115)> Public JF6 As String   '予備

        <VBFixedString(402)> Public JF7 As String   '予備

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                           { _
                            SubData(JF1, 4), _
                            SubData(JF2, 3), _
                            SubData(JF3, 1), _
                            SubData(JF4, 4), _
                            SubData(JF5, 1), _
                            SubData(JF6, 115), _
 _
                            SubData(JF7, ModeLen(0)) _
                           })
            End Get
            Set(ByVal value As String)
                JF1 = CuttingData(value, 4)
                JF2 = CuttingData(value, 3)
                JF3 = CuttingData(value, 1)
                JF4 = CuttingData(value, 4)
                JF5 = CuttingData(value, 1)
                JF6 = CuttingData(value, 115)

                JF7 = CuttingData(value, 402)
            End Set
        End Property
    End Structure
    Public SINKIN128_DATA1 As SINKIN128_RECORD1

    ' データレコード
    Structure SINKIN128_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(4)> Public JF1 As String     '組合コード
        <VBFixedString(3)> Public JF2 As String     '店舗コード
        <VBFixedString(1)> Public JF3 As String     'レコード種別
        <VBFixedString(2)> Public JF4 As String     '科目コード
        <VBFixedString(7)> Public JF5 As String     '口座番号
        <VBFixedString(6)> Public JF6 As String     '自振指定日
        <VBFixedString(10)> Public JF7 As String    '金額
        <VBFixedString(1)> Public JF8 As String     '入出金区分
        <VBFixedString(3)> Public JF9 As String     '企業コード
        <VBFixedString(7)> Public JF10 As String    '企業シーケンス
        <VBFixedString(2)> Public JF11 As String    '口座内優先コード
        <VBFixedString(3)> Public JF12 As String    '振替コード
        <VBFixedString(2)> Public JF13 As String    '振替相手科目
        <VBFixedString(7)> Public JF14 As String    '振替相手口番
        <VBFixedString(4)> Public JF15 As String    '開始年月
        <VBFixedString(4)> Public JF16 As String    '終了年月
        <VBFixedString(1)> Public JF17 As String    '摘要設定区分
        <VBFixedString(13)> Public JF18 As String   'カナ摘要
        <VBFixedString(12)> Public JF19 As String   '漢字摘要
        <VBFixedString(24)> Public JF20 As String   '需要家番号
        <VBFixedString(7)> Public JF21 As String    '特定顧客番号
        <VBFixedString(5)> Public JF22 As String    '予備

        <VBFixedString(2)> Public JF23 As String    'フォーマット区分
        <VBFixedString(394)> Public JF24 As String  '予備
        <VBFixedString(3)> Public JF25 As String    '振替コード
        <VBFixedString(3)> Public JF26 As String    '企業コード
        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                        { _
                            SubData(JF1, 4), _
                            SubData(JF2, 3), _
                            SubData(JF3, 1), _
                            SubData(JF4, 2), _
                            SubData(JF5, 7), _
                            SubData(JF6, 6), _
                            SubData(JF7, 10), _
                            SubData(JF8, 1), _
                            SubData(JF9, 3), _
                            SubData(JF10, 7), _
                            SubData(JF11, 2), _
                            SubData(JF12, 3), _
                            SubData(JF13, 2), _
                            SubData(JF14, 7), _
                            SubData(JF15, 4), _
                            SubData(JF16, 4), _
                            SubData(JF17, 1), _
                            SubData(JF18, 13), _
                            SubData(JF19, 12), _
                            SubData(JF20, 24), _
                            SubData(JF21, 7), _
                            SubData(JF22, 5), _
 _
                            SubData(JF23, ModeLen(1)), _
                            SubData(JF24, ModeLen(2)), _
                            SubData(JF25, ModeLen(3)), _
                            SubData(JF26, ModeLen(4)) _
                        })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 1)
                JF4 = CuttingData(Value, 2)
                JF5 = CuttingData(Value, 7)
                JF6 = CuttingData(Value, 6)
                JF7 = CuttingData(Value, 10)
                JF8 = CuttingData(Value, 1)
                JF9 = CuttingData(Value, 3)
                JF10 = CuttingData(Value, 7)
                JF11 = CuttingData(Value, 2)
                JF12 = CuttingData(Value, 3)
                JF13 = CuttingData(Value, 2)
                JF14 = CuttingData(Value, 7)
                JF15 = CuttingData(Value, 4)
                JF16 = CuttingData(Value, 4)
                JF17 = CuttingData(Value, 1)
                JF18 = CuttingData(Value, 13)
                JF19 = CuttingData(Value, 12)
                JF20 = CuttingData(Value, 24)
                JF21 = CuttingData(Value, 7)
                JF22 = CuttingData(Value, 5)

                JF23 = CuttingData(Value, 2)
                JF24 = CuttingData(Value, 394)
                JF25 = CuttingData(Value, 3)
                JF26 = CuttingData(Value, 3)
            End Set
        End Property
    End Structure
    Public SINKIN128_DATA2 As SINKIN128_RECORD2

    ' エンドレコード
    Structure SINKIN128_RECORD9
        Implements CFormat.IFormat

        <VBFixedString(4)> Public JF1 As String     '組合コード
        <VBFixedString(3)> Public JF2 As String     '店舗コード
        <VBFixedString(1)> Public JF3 As String     'レコード種別
        <VBFixedString(120)> Public JF4 As String   '予備

        <VBFixedString(402)> Public JF5 As String   '予備
        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                        { _
                            SubData(JF1, 4), _
                            SubData(JF2, 3), _
                            SubData(JF3, 1), _
                            SubData(JF4, 120), _
 _
                            SubData(JF5, ModeLen(0)) _
                        })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 1)
                JF4 = CuttingData(Value, 120)

                JF5 = CuttingData(Value, 402)
            End Set
        End Property
    End Structure
    Public SINKIN128_DATA9 As SINKIN128_RECORD9

    Private Mode As String = CASTCommon.GetFSKJIni("OTHERSYS", "JIFURILEN")
    Private Shared ModeLen(4) As Integer

    ' New
    Public Sub New()
        MyBase.New()

        If Mode = "530" Then
            ModeLen(0) = 402
            ModeLen(1) = 2
            ModeLen(2) = 394
            ModeLen(3) = 3
            ModeLen(4) = 3
        End If

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"0", "9"}

        FtranPfile = "530.P"

        HeaderKubun = New String() {"0"}
        DataKubun = New String() {"1", "2", "3", "4"}
        TrailerKubun = New String() {"9"}
    End Sub

End Class
