Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' ＳＫＣフォーマット（レコード長１６０）クラス
Public Class CFormatSKC160
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 160

    ' ------------------------------------------
    ' ＳＫＣフォーマット
    ' ------------------------------------------

    ' ヘッダレコード
    Public Structure SKC160_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public JF1 As String     'データ区分
        <VBFixedString(4)> Public JF2 As String     '組合コード
        <VBFixedString(3)> Public JF3 As String     '店舗コード
        <VBFixedString(1)> Public JF4 As String     'レコード種別
        <VBFixedString(4)> Public JF5 As String     '持込組合コード
        <VBFixedString(1)> Public JF6 As String     '返還区分
        <VBFixedString(1)> Public JF7 As String     '職域信組区分
        <VBFixedString(145)> Public JF8 As String   '予備

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                           { _
                            SubData(JF1, 1), _
                            SubData(JF2, 4), _
                            SubData(JF3, 3), _
                            SubData(JF4, 1), _
                            SubData(JF5, 4), _
                            SubData(JF6, 1), _
                            SubData(JF7, 1), _
                            SubData(JF8, 145) _
                           })
            End Get
            Set(ByVal value As String)
                JF1 = CuttingData(value, 1)
                JF2 = CuttingData(value, 4)
                JF3 = CuttingData(value, 3)
                JF4 = CuttingData(value, 1)
                JF5 = CuttingData(value, 4)
                JF6 = CuttingData(value, 1)
                JF7 = CuttingData(value, 1)
                JF8 = CuttingData(value, 145)
            End Set
        End Property
    End Structure
    Public SKC160_DATA1 As SKC160_RECORD1

    ' データレコード
    Structure SKC160_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public JF1 As String     'データ区分
        <VBFixedString(4)> Public JF2 As String     '組合コード
        <VBFixedString(3)> Public JF3 As String     '店舗コード
        <VBFixedString(1)> Public JF4 As String     'レコード種別
        <VBFixedString(2)> Public JF5 As String     '科目コード
        <VBFixedString(7)> Public JF6 As String     '口座番号
        <VBFixedString(8)> Public JF7 As String     '自振指定日
        <VBFixedString(10)> Public JF8 As String    '金額
        <VBFixedString(1)> Public JF9 As String     '入出金区分
        <VBFixedString(4)> Public JF10 As String    '企業コード
        <VBFixedString(7)> Public JF11 As String    '企業シーケンス
        <VBFixedString(2)> Public JF12 As String    '口座内優先コード
        <VBFixedString(3)> Public JF13 As String    '振替コード
        <VBFixedString(2)> Public JF14 As String    '振替相手科目
        <VBFixedString(7)> Public JF15 As String    '振替相手口番
        <VBFixedString(6)> Public JF16 As String    '開始年月
        <VBFixedString(6)> Public JF17 As String    '終了年月
        <VBFixedString(1)> Public JF18 As String    '摘要設定区分
        <VBFixedString(13)> Public JF19 As String   'カナ摘要
        <VBFixedString(12)> Public JF20 As String   '漢字摘要
        <VBFixedString(24)> Public JF21 As String   '需要家番号
        <VBFixedString(7)> Public JF22 As String    '特定顧客番号
        <VBFixedString(1)> Public JF23 As String    '振替結果
        <VBFixedString(11)> Public JF24 As String   '職員番号
        <VBFixedString(10)> Public JF25 As String   '所属コード
        <VBFixedString(1)> Public JF26 As String    '休日指定コード
        <VBFixedString(4)> Public JF27 As String    '予備
        <VBFixedString(2)> Public JF28 As String    '持込ＭＴチェックエラーコード

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                        { _
                            SubData(JF1, 1), _
                            SubData(JF2, 4), _
                            SubData(JF3, 3), _
                            SubData(JF4, 1), _
                            SubData(JF5, 2), _
                            SubData(JF6, 7), _
                            SubData(JF7, 8), _
                            SubData(JF8, 10), _
                            SubData(JF9, 1), _
                            SubData(JF10, 4), _
                            SubData(JF11, 7), _
                            SubData(JF12, 2), _
                            SubData(JF13, 3), _
                            SubData(JF14, 2), _
                            SubData(JF15, 7), _
                            SubData(JF16, 6), _
                            SubData(JF17, 6), _
                            SubData(JF18, 1), _
                            SubData(JF19, 13), _
                            SubData(JF20, 12), _
                            SubData(JF21, 24), _
                            SubData(JF22, 7), _
                            SubData(JF23, 1), _
                            SubData(JF24, 11), _
                            SubData(JF25, 10), _
                            SubData(JF26, 1), _
                            SubData(JF27, 4), _
                            SubData(JF28, 2) _
                        })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 1)
                JF2 = CuttingData(Value, 4)
                JF3 = CuttingData(Value, 3)
                JF4 = CuttingData(Value, 1)
                JF5 = CuttingData(Value, 2)
                JF6 = CuttingData(Value, 7)
                JF7 = CuttingData(Value, 8)
                JF8 = CuttingData(Value, 10)
                JF9 = CuttingData(Value, 1)
                JF10 = CuttingData(Value, 4)
                JF11 = CuttingData(Value, 7)
                JF12 = CuttingData(Value, 2)
                JF13 = CuttingData(Value, 3)
                JF14 = CuttingData(Value, 2)
                JF15 = CuttingData(Value, 7)
                JF16 = CuttingData(Value, 6)
                JF17 = CuttingData(Value, 6)
                JF18 = CuttingData(Value, 1)
                JF19 = CuttingData(Value, 13)
                JF20 = CuttingData(Value, 12)
                JF21 = CuttingData(Value, 24)
                JF22 = CuttingData(Value, 7)
                JF23 = CuttingData(Value, 1)
                JF24 = CuttingData(Value, 11)
                JF25 = CuttingData(Value, 10)
                JF26 = CuttingData(Value, 1)
                JF27 = CuttingData(Value, 4)
                JF28 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure
    Public SKC160_DATA2 As SKC160_RECORD2

    ' トレーラレコード
    Structure SKC160_RECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public JF1 As String     'データ区分
        <VBFixedString(4)> Public JF2 As String     '組合コード
        <VBFixedString(3)> Public JF3 As String     '店舗コード
        <VBFixedString(1)> Public JF4 As String     'レコード種別
        <VBFixedString(6)> Public JF5 As String     '合計件数
        <VBFixedString(12)> Public JF6 As String    '合計金額
        <VBFixedString(1)> Public JF7 As String     '振替済件数符号
        <VBFixedString(6)> Public JF8 As String     '振替済件数
        <VBFixedString(1)> Public JF9 As String     '振替済金額符号
        <VBFixedString(12)> Public JF10 As String   '振替済金額
        <VBFixedString(6)> Public JF11 As String    '不能件数
        <VBFixedString(12)> Public JF12 As String   '不能金額
        <VBFixedString(95)> Public JF13 As String   '予備

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                        { _
                            SubData(JF1, 1), _
                            SubData(JF2, 4), _
                            SubData(JF3, 3), _
                            SubData(JF4, 1), _
                            SubData(JF5, 6), _
                            SubData(JF6, 12), _
                            SubData(JF7, 1), _
                            SubData(JF8, 6), _
                            SubData(JF9, 1), _
                            SubData(JF10, 12), _
                            SubData(JF11, 6), _
                            SubData(JF12, 12), _
                            SubData(JF13, 95) _
                        })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 1)
                JF2 = CuttingData(Value, 4)
                JF3 = CuttingData(Value, 3)
                JF4 = CuttingData(Value, 1)
                JF5 = CuttingData(Value, 6)
                JF6 = CuttingData(Value, 12)
                JF7 = CuttingData(Value, 1)
                JF8 = CuttingData(Value, 6)
                JF9 = CuttingData(Value, 1)
                JF10 = CuttingData(Value, 12)
                JF11 = CuttingData(Value, 6)
                JF12 = CuttingData(Value, 12)
                JF13 = CuttingData(Value, 95)
            End Set
        End Property
    End Structure
    Public SKC160_DATA8 As SKC160_RECORD8

    ' エンドレコード
    Structure SKC160_RECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public JF1 As String     'データ区分
        <VBFixedString(4)> Public JF2 As String     '組合コード
        <VBFixedString(3)> Public JF3 As String     '店舗コード
        <VBFixedString(1)> Public JF4 As String     'レコード種別
        <VBFixedString(151)> Public JF5 As String   '予備

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                        { _
                            SubData(JF1, 1), _
                            SubData(JF2, 4), _
                            SubData(JF3, 3), _
                            SubData(JF4, 1), _
                            SubData(JF5, 151) _
                        })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 1)
                JF2 = CuttingData(Value, 4)
                JF3 = CuttingData(Value, 3)
                JF4 = CuttingData(Value, 1)
                JF5 = CuttingData(Value, 151)
            End Set
        End Property
    End Structure
    Public SKC160_DATA9 As SKC160_RECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"1", "2", "8", "9"}

        FtranPfile = "HAISIN_SKC.P"

        HeaderKubun = New String() {"1"}
        DataKubun = New String() {"2"}
        TrailerKubun = New String() {"8"}
    End Sub

End Class
