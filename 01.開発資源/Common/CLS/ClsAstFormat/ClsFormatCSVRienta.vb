''' <summary>
''' 振込発信CSVリエンタ　フォーマット定義クラス
''' </summary>
''' <remarks>
''' 2016/10/19 saitou RSV2 added for 信組対応
''' 近畿産業信組のFSV2のソースを解析し新規作成
''' </remarks>
Public Class ClsFormatCSVRienta
    Inherits CFormat

    Public Structure CSVRNTHDRECORD1
        Implements CFormat.IFormat

        Dim CR1 As String
        Dim CR2 As String

        Public Property Data As String Implements IFormat.Data
            Get
                Return String.Concat(New String() { _
                                     CR1, _
                                     ",", _
                                     CR2 _
                                 })
            End Get
            Set(value As String)
                Dim SplitValue() As String = value.Split(","c)
                CR1 = SplitValue(0)
                CR2 = SplitValue(1)
            End Set
        End Property
    End Structure
    Public CSVRNT_HDREC1 As CSVRNTHDRECORD1

    Public Structure CSVRNTHDRECORD2
        Implements CFormat.IFormat

        Dim CR1 As String
        Dim CR2 As String
        Dim CR3 As String
        Dim CR4 As String
        Dim CR5 As String
        Dim CR6 As String

        Public Property Data As String Implements IFormat.Data
            Get
                Return String.Concat(New String() { _
                                     CR1, _
                                     ",", _
                                     CR2, _
                                     ",", _
                                     CR3, _
                                     ",", _
                                     CR4, _
                                     ",", _
                                     CR5, _
                                     ",", _
                                     CR6 _
                                 })
            End Get
            Set(value As String)
                Dim SplitValue() As String = value.Split(","c)
                CR1 = SplitValue(0)
                CR2 = SplitValue(1)
                CR3 = SplitValue(2)
                CR4 = SplitValue(3)
                CR5 = SplitValue(4)
                CR6 = SplitValue(5)
            End Set
        End Property
    End Structure
    Public CSVRNT_HDREC2 As CSVRNTHDRECORD2

    Public Structure CSVRNTDTRECORD
        Implements CFormat.IFormat

        Dim CR1 As String
        Dim CR2 As String
        Dim CR3 As String
        Dim CR4 As String
        Dim CR5 As String
        Dim CR6 As String
        Dim CR7 As String
        Dim CR8 As String
        Dim CR9 As String
        Dim CR10 As String
        Dim CR11 As String
        Dim CR12 As String
        Dim CR13 As String
        Dim CR14 As String
        Dim CR15 As String
        Dim CR16 As String
        Dim CR17 As String
        Dim CR18 As String

        Public Property Data As String Implements IFormat.Data
            Get
                Return String.Concat(New String() { _
                                     CR1, _
                                     ",", _
                                     CR2, _
                                     ",", _
                                     CR3, _
                                     ",", _
                                     CR4, _
                                     ",", _
                                     CR5, _
                                     ",", _
                                     CR6, _
                                     ",", _
                                     CR7, _
                                     ",", _
                                     CR8, _
                                     ",", _
                                     CR9, _
                                     ",", _
                                     CR10, _
                                     ",", _
                                     CR11, _
                                     ",", _
                                     CR12, _
                                     ",", _
                                     CR13, _
                                     ",", _
                                     CR14, _
                                     ",", _
                                     CR15, _
                                     ",", _
                                     CR16, _
                                     ",", _
                                     CR17, _
                                     ",", _
                                     CR18 _
                                 })
            End Get
            Set(value As String)
                Dim SplitValue() As String = value.Split(","c)
                CR1 = SplitValue(0)
                CR2 = SplitValue(1)
                CR3 = SplitValue(2)
                CR4 = SplitValue(3)
                CR5 = SplitValue(4)
                CR6 = SplitValue(5)
                CR7 = SplitValue(6)
                CR8 = SplitValue(7)
                CR9 = SplitValue(8)
                CR10 = SplitValue(9)
                CR11 = SplitValue(10)
                CR12 = SplitValue(11)
                CR13 = SplitValue(12)
                CR14 = SplitValue(13)
                CR15 = SplitValue(14)
                CR16 = SplitValue(15)
                CR17 = SplitValue(16)
                CR18 = SplitValue(17)
            End Set
        End Property
    End Structure
    Public CSVRNT_DTREC As CSVRNTDTRECORD

End Class
