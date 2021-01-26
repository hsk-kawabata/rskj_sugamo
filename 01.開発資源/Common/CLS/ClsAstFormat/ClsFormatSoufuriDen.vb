Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

''' <summary>
''' MT代行データ　フォーマット定義クラス
''' </summary>
''' <remarks>
''' 2016/10/19 saitou RSV2 added for 信組対応
''' 笠岡信組に導入されているMT代行フォーマットクラスをRSV2に移植
''' </remarks>
Public Class CFormatSoufuriDen
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 400

    '------------------------------------------
    '総振伝送データフォーマット
    '------------------------------------------
    'ヘッダレコード
    Public Structure SDRECORD1
        Implements CFormat.IFormat

        <VBFixedString(8)> Public SD1 As String     ' ブロック番号
        <VBFixedString(8)> Public SD2 As String     ' 発信指定日
        <VBFixedString(6)> Public SD3 As String     ' MT番号             自金庫コード　＆　（統一番号+1）
        <VBFixedString(15)> Public SD4 As String    ' 依頼組合名         組合名
        <VBFixedString(4)> Public SD5 As String     ' 依頼組合コード     自金庫コード
        <VBFixedString(359)> Public SD6 As String   ' 予備

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(SD1, 8), _
                            SubData(SD2, 8), _
                            SubData(SD3, 6), _
                            SubData(SD4, 15), _
                            SubData(SD5, 4), _
                            SubData(SD6, 359) _
                            })
            End Get
            Set(ByVal value As String)
                SD1 = CuttingData(value, 8)
                SD2 = CuttingData(value, 8)
                SD3 = CuttingData(value, 6)
                SD4 = CuttingData(value, 15)
                SD5 = CuttingData(value, 4)
                SD6 = CuttingData(value, 359)
            End Set
        End Property
    End Structure
    Public SOUDEN_REC1 As SDRECORD1

    'データレコード
    Structure SDRECORD2
        Implements CFormat.IFormat

        <VBFixedString(8)> Public SD1 As String     ' ブロック番号
        <VBFixedString(1)> Public SD2 As String     ' 決済回数
        <VBFixedString(1)> Public SD3 As String     ' 予備1
        <VBFixedString(8)> Public SD4 As String     ' 取扱日
        <VBFixedString(4)> Public SD5 As String     ' 取扱日通信種目コード
        <VBFixedString(3)> Public SD6 As String     ' 付加コード
        <VBFixedString(15)> Public SD7 As String    ' 受信金融機関名
        <VBFixedString(1)> Public SD8 As String     ' 区切1
        <VBFixedString(15)> Public SD9 As String    ' 受信店舗名
        <VBFixedString(15)> Public SD10 As String   ' 金額
        <VBFixedString(15)> Public SD11 As String   ' 発信組合名
        <VBFixedString(1)> Public SD12 As String    ' 区切2
        <VBFixedString(15)> Public SD13 As String   ' 発信店舗名
        <VBFixedString(7)> Public SD14 As String    ' 銀行間手数料
        <VBFixedString(16)> Public SD15 As String   ' 番号欄
        <VBFixedString(20)> Public SD16 As String   ' EDI情報
        <VBFixedString(48)> Public SD17 As String   ' 受取人欄
        <VBFixedString(48)> Public SD18 As String   ' 依頼人欄
        <VBFixedString(48)> Public SD19 As String   ' 備考1
        <VBFixedString(48)> Public SD20 As String   ' 備考2
        <VBFixedString(15)> Public SD21 As String   ' 発信番号
        <VBFixedString(15)> Public SD22 As String   ' 照会番号
        <VBFixedString(33)> Public SD23 As String   ' 予備2

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(SD1, 8), _
                            SubData(SD2, 1), _
                            SubData(SD3, 1), _
                            SubData(SD4, 8), _
                            SubData(SD5, 4), _
                            SubData(SD6, 3), _
                            SubData(SD7, 15), _
                            SubData(SD8, 1), _
                            SubData(SD9, 15), _
                            SubData(SD10, 15), _
                            SubData(SD11, 15), _
                            SubData(SD12, 1), _
                            SubData(SD13, 15), _
                            SubData(SD14, 7), _
                            SubData(SD15, 16), _
                            SubData(SD16, 20), _
                            SubData(SD17, 48), _
                            SubData(SD18, 48), _
                            SubData(SD19, 48), _
                            SubData(SD20, 48), _
                            SubData(SD21, 15), _
                            SubData(SD22, 15), _
                            SubData(SD23, 33) _
                            })
            End Get
            Set(ByVal Value As String)
                SD1 = CuttingData(Value, 8)
                SD2 = CuttingData(Value, 1)
                SD3 = CuttingData(Value, 1)
                SD4 = CuttingData(Value, 8)
                SD5 = CuttingData(Value, 4)
                SD6 = CuttingData(Value, 3)
                SD7 = CuttingData(Value, 15)
                SD8 = CuttingData(Value, 1)
                SD9 = CuttingData(Value, 15)
                SD10 = CuttingData(Value, 15)
                SD11 = CuttingData(Value, 15)
                SD12 = CuttingData(Value, 1)
                SD13 = CuttingData(Value, 15)
                SD14 = CuttingData(Value, 7)
                SD15 = CuttingData(Value, 16)
                SD16 = CuttingData(Value, 20)
                SD17 = CuttingData(Value, 48)
                SD18 = CuttingData(Value, 48)
                SD19 = CuttingData(Value, 48)
                SD20 = CuttingData(Value, 48)
                SD21 = CuttingData(Value, 15)
                SD22 = CuttingData(Value, 15)
                SD23 = CuttingData(Value, 33)
            End Set
        End Property
    End Structure
    Public SOUDEN_REC2 As SDRECORD2

    'フッターレコード
    Public Structure SDRECORD9
        Implements CFormat.IFormat

        <VBFixedString(8)> Public SD1 As String     ' ブロック番号
        <VBFixedString(6)> Public SD2 As String     ' 順電文数
        <VBFixedString(12)> Public SD3 As String    ' 順電文合計金額
        <VBFixedString(6)> Public SD4 As String     ' 先振電文数
        <VBFixedString(12)> Public SD5 As String    ' 先振電文合計金額
        <VBFixedString(6)> Public SD6 As String     ' 逆電文数
        <VBFixedString(12)> Public SD7 As String    ' 逆電文合計金額
        <VBFixedString(6)> Public SD8 As String     ' 取り立て\0電文数
        <VBFixedString(6)> Public SD9 As String     ' 通信電文数
        <VBFixedString(6)> Public SD10 As String    ' 合計電文数
        <VBFixedString(320)> Public SD11 As String  ' 予備

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(SD1, 8), _
                            SubData(SD2, 6), _
                            SubData(SD3, 12), _
                            SubData(SD4, 6), _
                            SubData(SD5, 12), _
                            SubData(SD6, 6), _
                            SubData(SD7, 12), _
                            SubData(SD8, 6), _
                            SubData(SD9, 6), _
                            SubData(SD10, 6), _
                            SubData(SD11, 320) _
                            })
            End Get
            Set(ByVal Value As String)
                SD1 = CuttingData(Value, 8)
                SD2 = CuttingData(Value, 6)
                SD3 = CuttingData(Value, 12)
                SD4 = CuttingData(Value, 6)
                SD5 = CuttingData(Value, 12)
                SD6 = CuttingData(Value, 6)
                SD7 = CuttingData(Value, 12)
                SD8 = CuttingData(Value, 6)
                SD9 = CuttingData(Value, 6)
                SD10 = CuttingData(Value, 6)
                SD11 = CuttingData(Value, 320)
            End Set
        End Property
    End Structure
    Public SOUDEN_REC9 As SDRECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        FtranPfile = "400.P"

    End Sub
End Class

' 資金決済データフォーマット
Public Class ClsFormSOU

    ' 固定長構造体用インターフェース
    Protected Interface IFormat
        ' データ
        Sub Init()
        Property Data() As String
    End Interface

    ' SHIT-JISエンコーディング
    Protected Friend Shared EncdJ As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")

#Region "総振伝送為替明細用"
    Public Structure PrnSoufuriData
        Implements ClsFormSOU.IFormat

        '--------ヘッダ--------
        Dim HassinDate As String        '発信日
        '--------グループ------
        Dim TorisCode As String         '取引先主コード
        Dim TorifCode As String         '取引先副コード
        Dim ToriNName As String         '取引先名(日本語)
        Dim FuriDate As String          '振込日
        Dim Baitai As String            '媒体
        Dim Syubetu As String           '種別
        Dim TekiyouSyubetu As String    '適用種別
        Dim HonbuTenCode As String      '本部支店コード
        Dim HonbuTenName As String      '本部支店名
        Dim KeiyakuName As String       '受取人名
        Dim FurikomiKinCode As String   '振込金融機関コード
        Dim FurikomiKinName As String   '振込金融機関名
        Dim FurikomiSitCode As String   '振込支店コード
        Dim FurikomiSitName As String   '振込支店名
        Dim Kamoku As String            '科目
        Dim KouzaNo As String           '口座番号
        Dim FurikomiKIN As String       '振込金
        Dim Bikou1 As String            '備考１
        Dim Bikou2 As String            '備考２
        Dim TukekaeKinCode As String    '付替金融機関コード
        Dim TukekaeKinName As String    '付替金融機関名
        Dim TukekaeSitCode As String    '付替支店コード
        Dim TukekaeSitName As String    '付替支店名

        Public Sub Init() Implements IFormat.Init
            HassinDate = ""         '発信日

            TorisCode = ""          '取引先主コード
            TorifCode = ""          '取引先副コード
            ToriNName = ""          '取引先名(日本語)
            FuriDate = ""           '振込日
            Baitai = ""             '媒体
            Syubetu = ""            '種別
            TekiyouSyubetu = ""     '適用種別
            HonbuTenCode = ""       '本部支店コード
            HonbuTenName = ""       '本部支店名
            KeiyakuName = ""        '受取人名
            FurikomiKinCode = ""    '振込金融機関コード
            FurikomiKinName = ""    '振込金融機関名
            FurikomiSitCode = ""    '振込支店コード
            FurikomiSitName = ""    '振込支店名
            Kamoku = ""             '科目
            KouzaNo = ""            '口座番号
            FurikomiKIN = ""        '振込金
            Bikou1 = ""             '備考１
            Bikou2 = ""             '備考２
            TukekaeKinCode = ""     '付替金融機関コード
            TukekaeKinName = ""     '付替金融機関名
            TukekaeSitCode = ""     '付替支店コード
            TukekaeSitName = ""     '付替支店名

        End Sub

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements IFormat.Data
            Get
                Dim record As String = ""

                record = String.Concat(New String() _
                            { _
                            SubData(HassinDate, 8), _
                            SubData(TorisCode, 10), _
                            SubData(TorifCode, 2), _
                            SubData(ToriNName, 50), _
                            SubData(FuriDate, 8), _
                            SubData(Baitai, 2), _
                            SubData(Syubetu, 4), _
                            SubData(TekiyouSyubetu, 8), _
                            SubData(HonbuTenCode, 3), _
                            SubData(HonbuTenName, 15), _
                            SubData(KeiyakuName, 40), _
                            SubData(FurikomiKinCode, 4), _
                            SubData(FurikomiKinName, 15), _
                            SubData(FurikomiSitCode, 3), _
                            SubData(FurikomiSitName, 15), _
                            SubData(Kamoku, 1), _
                            SubData(KouzaNo, 7), _
                            SubData(FurikomiKIN, 15), _
                            SubData(Bikou1, 48), _
                            SubData(Bikou2, 48), _
                            SubData(TukekaeKinCode, 4), _
                            SubData(TukekaeKinName, 15), _
                            SubData(TukekaeSitCode, 4), _
                            SubData(TukekaeSitName, 15) _
                            })
                Return record
            End Get
            Set(ByVal value As String)
                HassinDate = CuttingData(value, 8)
                TorisCode = CuttingData(value, 10)
                TorifCode = CuttingData(value, 2)
                ToriNName = CuttingData(value, 50)
                FuriDate = CuttingData(value, 8)
                Baitai = CuttingData(value, 2)
                Syubetu = CuttingData(value, 4)
                TekiyouSyubetu = CuttingData(value, 8)
                HonbuTenCode = CuttingData(value, 3)
                HonbuTenname = CuttingData(value, 15)
                KeiyakuName = CuttingData(value, 40)
                FurikomiKinCode = CuttingData(value, 4)
                FurikomiKinName = CuttingData(value, 15)
                FurikomiSitCode = CuttingData(value, 3)
                FurikomiSitName = CuttingData(value, 15)
                Kamoku = CuttingData(value, 1)
                KouzaNo = CuttingData(value, 7)
                FurikomiKIN = CuttingData(value, 15)
                Bikou1 = CuttingData(value, 48)
                Bikou2 = CuttingData(value, 48)
                TukekaeKinCode = CuttingData(value, 4)
                TukekaeKinName = CuttingData(value, 15)
                TukekaeSitCode = CuttingData(value, 4)
                TukekaeSitName = CuttingData(value, 15)

            End Set

        End Property
    End Structure
#End Region
    '
    ' 機能　 ： 文字列から，指定の長さを切り取る
    '
    ' 引数　 ： ARG1 - 文字列
    ' 　　　 　 ARG2 - 長さ
    '           ARG3 - ０：左詰，１：右詰
    '           ARG4 - 埋め文字
    '
    ' 戻り値 ： 切り取った後の残りの文字列
    '
    ' 備考　 ：
    '
    Protected Friend Shared Function SubData(ByVal value As String, ByVal len As Integer, _
                    Optional ByVal align As Integer = 0, Optional ByVal pad As Char = " "c) As String
        Try
            If len = 0 Then
                Return ""
            End If

            ' 切り取る文字列
            If align = 0 Then
                ' 左詰
                value = value.PadRight(len, pad)
            Else
                ' 右詰
                value = value.PadLeft(len, pad)
            End If

            ' 切り取る文字列
            Dim bt() As Byte = EncdJ.GetBytes(value)
            Return EncdJ.GetString(bt, 0, len)
        Catch ex As Exception
            Return New String(" "c, len)
        End Try
    End Function

    '
    ' 機能　 ： 文字列から，指定の長さを切り取る
    '
    ' 引数　 ： ARG1 - 文字列
    ' 　　　 　 ARG2 - 長さ
    '
    ' 戻り値 ： 切り取った後の残りの文字列
    '
    ' 備考　 ：
    '
    Protected Friend Shared Function CuttingData(ByRef value As String, ByVal len As Integer) As String
        Try
            ' 切り取る文字列
            Dim ret As String
            Dim bt() As Byte = EncdJ.GetBytes(value)
            ret = EncdJ.GetString(bt, 0, len)
            ' 切り取った後の残りの文字列
            value = value.Substring(ret.Length())
            Return ret
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class


