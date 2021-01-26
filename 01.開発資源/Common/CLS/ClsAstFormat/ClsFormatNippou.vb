Imports Microsoft.VisualBasic

Public Class CFormatNippou
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Public Shared Shadows ReadOnly RecordLen As Integer = 313

    '-------------------
    '日報データ取り込み用
    '-------------------
    Public Structure NIPPO_DATA_RECORD
        Implements CFormat.IFormat

        <VBFixedString(4)> Public NI1 As String      '金融機関コード
        <VBFixedString(3)> Public NI2 As String      '店舗コード
        <VBFixedString(2)> Public NI3 As String      'レコード種別
        <VBFixedString(3)> Public NI4 As String      '振替コード
        <VBFixedString(5)> Public NI5 As String      '企業コード
        <VBFixedString(3)> Public NI6 As String      '振替種別
        <VBFixedString(40)> Public NI7 As String     '企業名
        <VBFixedString(8)> Public NI8 As String      'データ作成日
        <VBFixedString(8)> Public NI9 As String      '振替指定日
        <VBFixedString(7)> Public NI10 As String     '普通件数
        <VBFixedString(13)> Public NI11 As String    '普通金額
        <VBFixedString(7)> Public NI12 As String     '当座件数
        <VBFixedString(13)> Public NI13 As String    '当座金額
        <VBFixedString(7)> Public NI14 As String     '企業完了件数
        <VBFixedString(13)> Public NI15 As String    '企業完了金額
        <VBFixedString(7)> Public NI16 As String     '金庫完了件数
        <VBFixedString(13)> Public NI17 As String    '金庫完了金額
        <VBFixedString(7)> Public NI18 As String     'ＷＭ完了件数
        <VBFixedString(13)> Public NI19 As String    'ＷＭ完了金額
        <VBFixedString(7)> Public NI20 As String     '企業不能件数
        <VBFixedString(13)> Public NI21 As String    '企業不能金額
        <VBFixedString(7)> Public NI22 As String     '金庫不能件数
        <VBFixedString(13)> Public NI23 As String    '金庫不能金額
        <VBFixedString(7)> Public NI24 As String     'ＷＭ不能件数
        <VBFixedString(13)> Public NI25 As String    'ＷＭ不能金額
        <VBFixedString(7)> Public NI26 As String     '依頼返却件数
        <VBFixedString(13)> Public NI27 As String    '依頼返却金額
        <VBFixedString(7)> Public NI28 As String     '事前照合正常件数
        <VBFixedString(7)> Public NI29 As String     '事前照合不能件数
        <VBFixedString(7)> Public NI30 As String     '企業持込件数
        <VBFixedString(13)> Public NI31 As String    '企業持込金額
        <VBFixedString(7)> Public NI32 As String     '金庫持込件数
        <VBFixedString(13)> Public NI33 As String    '金庫持込金額
        <VBFixedString(1)> Public NI34 As String     '予備

        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NI1, 4), _
                            SubData(NI2, 3), _
                            SubData(NI3, 2), _
                            SubData(NI4, 3), _
                            SubData(NI5, 5), _
                            SubData(NI6, 3), _
                            SubData(NI7, 40), _
                            SubData(NI8, 8), _
                            SubData(NI9, 8), _
                            SubData(NI10, 7), _
                            SubData(NI11, 13), _
                            SubData(NI12, 7), _
                            SubData(NI13, 13), _
                            SubData(NI14, 7), _
                            SubData(NI15, 13), _
                            SubData(NI16, 7), _
                            SubData(NI17, 13), _
                            SubData(NI18, 7), _
                            SubData(NI19, 13), _
                            SubData(NI20, 7), _
                            SubData(NI21, 13), _
                            SubData(NI22, 7), _
                            SubData(NI23, 13), _
                            SubData(NI24, 7), _
                            SubData(NI25, 13), _
                            SubData(NI26, 7), _
                            SubData(NI27, 13), _
                            SubData(NI28, 7), _
                            SubData(NI29, 7), _
                            SubData(NI30, 7), _
                            SubData(NI31, 13), _
                            SubData(NI32, 7), _
                            SubData(NI33, 13), _
                            SubData(NI34, 1) _
                    })
            End Get
            Set(ByVal value As String)
                NI1 = CuttingData(value, 4)
                NI2 = CuttingData(value, 3)
                NI3 = CuttingData(value, 2)
                NI4 = CuttingData(value, 3)
                NI5 = CuttingData(value, 5)
                NI6 = CuttingData(value, 3)
                NI7 = CuttingData(value, 40)
                NI8 = CuttingData(value, 8)
                NI9 = CuttingData(value, 8)
                NI10 = CuttingData(value, 7)
                NI11 = CuttingData(value, 13)
                NI12 = CuttingData(value, 7)
                NI13 = CuttingData(value, 13)
                NI14 = CuttingData(value, 7)
                NI15 = CuttingData(value, 13)
                NI16 = CuttingData(value, 7)
                NI17 = CuttingData(value, 13)
                NI18 = CuttingData(value, 7)
                NI19 = CuttingData(value, 13)
                NI20 = CuttingData(value, 7)
                NI21 = CuttingData(value, 13)
                NI22 = CuttingData(value, 7)
                NI23 = CuttingData(value, 13)
                NI24 = CuttingData(value, 7)
                NI25 = CuttingData(value, 13)
                NI26 = CuttingData(value, 7)
                NI27 = CuttingData(value, 13)
                NI28 = CuttingData(value, 7)
                NI29 = CuttingData(value, 7)
                NI30 = CuttingData(value, 7)
                NI31 = CuttingData(value, 13)
                NI32 = CuttingData(value, 7)
                NI33 = CuttingData(value, 13)
                NI34 = CuttingData(value, 1)
            End Set
        End Property
    End Structure
    Public NIPPO_DATA As NIPPO_DATA_RECORD

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        FtranPfile = "NIPPOU.P"
        FtranIBMPfile = ""
    End Sub

    Public Overrides Function CheckDataFormat() As String
        ' 基本クラス チェック
        Dim sRet As String = MyBase.CheckKekkaFormat()

        If RecordData.Length = 0 Then
            DataInfo.Message = "ファイル異常"
            mnErrorNumber = 1
            Return "ERR"
        End If

        NIPPO_DATA.Data = RecordData

        Return sRet
    End Function

End Class
