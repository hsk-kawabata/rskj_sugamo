Imports System
Imports System.IO
Imports System.Text
Imports System.Collections

' タンキングヘッダフォーマット
Public Class ClsT_RIENT77

    ' SHIT-JISエンコーディング
    Protected Shared EncdJ As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")

    ' 固定長構造体用インターフェース
    Protected Interface IFormat
        ' データ
        Sub Init()
        ReadOnly Property Data() As Byte()
    End Interface

#Region "タンキングヘッダ部"
    '　タンキングヘッダ部
    Public Structure T_RIENT77T
        Implements ClsT_RIENT77.IFormat

        Private bytHDR() As Byte                'HDR(2)
        Private bytHEAD_KBN As Byte             'ヘッダ区分
        Private bytTDATA_FLG As Byte            'Tデータフラグ
        Private bytRSIJI_FLG As Byte            'R指示フラグ
        Private bytRSUMI_FLG As Byte            'R済フラグ         
        Private bytYOBI1 As Byte                '予備1
        Public strT_HIDUKE As String            'T日付
        Private bytRTYUFD_ADD() As Byte         'R中FDアドレス(3)
        Public bytZENTENPO_TKEN() As Byte       '全店舗T件数(1)  
        Private bytZENTENPO_RKEN() As Byte      '全店舗R件数(1) 
        Private bytZENTENPO_ERR_RKEN() As Byte  '全店舗Rエラー件数(1)
        Private bytYOBI2() As Byte              '予備2(1)

        Public Sub Init() Implements IFormat.Init
            bytHDR = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytHDR(0) = 72
            bytHDR(1) = 68
            bytHDR(2) = 82

            bytHEAD_KBN = 67
            bytTDATA_FLG = 3
            bytRSIJI_FLG = 0
            bytRSUMI_FLG = 0
            bytYOBI1 = 0

            strT_HIDUKE = New String(" "c, 8)

            bytRTYUFD_ADD = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytRTYUFD_ADD(0) = 255
            bytRTYUFD_ADD(1) = 255
            bytRTYUFD_ADD(2) = 255
            bytRTYUFD_ADD(3) = 255

            bytZENTENPO_TKEN = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytZENTENPO_TKEN(0) = 0
            bytZENTENPO_TKEN(1) = 0

            '***修正　前田　20080930**************************
            '全店舗リエンタ件数 'FFFF→0000に変更
            bytZENTENPO_RKEN = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytZENTENPO_RKEN(0) = 0
            bytZENTENPO_RKEN(1) = 0
            '*************************************************

            bytZENTENPO_ERR_RKEN = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytZENTENPO_ERR_RKEN(0) = 0
            bytZENTENPO_ERR_RKEN(1) = 0

            bytYOBI2 = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytYOBI2(0) = 0
            bytYOBI2(1) = 0
        End Sub

        '固定長データ処理用プロパティ
        Public ReadOnly Property Data() As Byte() Implements IFormat.Data
            Get
                Dim ret(27) As Byte

                Array.Copy(bytHDR, 0, ret, 0, 3)
                ret(3) = bytHEAD_KBN
                ret(4) = bytTDATA_FLG
                ret(5) = bytRSIJI_FLG
                ret(6) = bytRSUMI_FLG
                ret(7) = bytYOBI1

                Array.Copy(EncdJ.GetBytes(SubData(strT_HIDUKE, 8)), 0, ret, 8, 8)
                Array.Copy(bytRTYUFD_ADD, 0, ret, 16, 4)
                Array.Copy(bytZENTENPO_TKEN, 0, ret, 20, 2)
                Array.Copy(bytZENTENPO_RKEN, 0, ret, 22, 2)
                Array.Copy(bytZENTENPO_ERR_RKEN, 0, ret, 24, 2)
                Array.Copy(bytYOBI2, 0, ret, 26, 2)

                Return ret
            End Get
        End Property
    End Structure
#End Region

#Region "店舗情報レコード"
    '　店舗情報レコード部
    Public Structure T_RIENT77M
        Implements ClsT_RIENT77.IFormat

        Public strKINKO_CD As String            '金庫コード
        Public strSIT_CD As String              '店舗コード
        Private bytYOBI1 As Byte                '予備1
        Public bytTENPO_TKEN() As Byte          '店舗T件数(1) 
        Private bytTENPO_RKEN() As Byte         '店舗R件数(1)
        Private bytTENPO_ERR_RKEN() As Byte     '店舗Rエラー件数(1)
        Private bytRKEKKA_SYOKAI As Byte        'R結果照会フラグ
        Private bytYOBI2 As Byte                '予備2 
        Private bytTENPO_TSTART_FDADD() As Byte  '店舗T開始FDアドレス(3)
        Public bytTENPO_TFINISH_FDADD() As Byte '店舗T終了FDアドレス(3)
        Private bytTENPO_RKEKKA_FDADD() As Byte '店舗R結果照会FDアドレス(3)

        Public Sub Init() Implements IFormat.Init
            strKINKO_CD = New String(" "c, 4)
            strSIT_CD = New String(" "c, 3)

            bytYOBI1 = 0

            bytTENPO_TKEN = CType(Array.CreateInstance(GetType(Byte), 2), Byte())

            bytTENPO_RKEN = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytTENPO_RKEN.Initialize()

            bytTENPO_ERR_RKEN = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytTENPO_ERR_RKEN.Initialize()

            bytRKEKKA_SYOKAI = 0
            bytYOBI2 = 0

            bytTENPO_TSTART_FDADD = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytTENPO_TSTART_FDADD(0) = 0
            bytTENPO_TSTART_FDADD(1) = 0
            bytTENPO_TSTART_FDADD(2) = 4
            bytTENPO_TSTART_FDADD(3) = 0

            '***修正　前田　20080930**********************************
            '終了アドレスの初期値を開始アドレスと同一に設定
            bytTENPO_TFINISH_FDADD = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytTENPO_TFINISH_FDADD(0) = 0
            bytTENPO_TFINISH_FDADD(1) = 0
            bytTENPO_TFINISH_FDADD(2) = 4
            bytTENPO_TFINISH_FDADD(3) = 0
            '*********************************************************

            bytTENPO_RKEKKA_FDADD = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytTENPO_RKEKKA_FDADD(0) = 255
            bytTENPO_RKEKKA_FDADD(1) = 255
            bytTENPO_RKEKKA_FDADD(2) = 255
            bytTENPO_RKEKKA_FDADD(3) = 255
        End Sub

        Public Sub Init2_32()
            strKINKO_CD = New String(" "c, 4)
            strSIT_CD = New String(" "c, 3)

            bytYOBI1 = 0

            bytTENPO_TKEN = CType(Array.CreateInstance(GetType(Byte), 2), Byte())

            bytTENPO_RKEN = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytTENPO_RKEN(0) = 0
            bytTENPO_RKEN(1) = 0

            bytTENPO_ERR_RKEN = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytTENPO_ERR_RKEN(0) = 0
            bytTENPO_ERR_RKEN(1) = 0

            bytRKEKKA_SYOKAI = 0
            bytYOBI2 = 0

            bytTENPO_TSTART_FDADD = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytTENPO_TSTART_FDADD(0) = 255
            bytTENPO_TSTART_FDADD(1) = 255
            bytTENPO_TSTART_FDADD(2) = 255
            bytTENPO_TSTART_FDADD(3) = 255

            bytTENPO_TFINISH_FDADD = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytTENPO_TFINISH_FDADD(0) = 255
            bytTENPO_TFINISH_FDADD(1) = 255
            bytTENPO_TFINISH_FDADD(2) = 255
            bytTENPO_TFINISH_FDADD(3) = 255

            bytTENPO_RKEKKA_FDADD = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytTENPO_RKEKKA_FDADD(0) = 255
            bytTENPO_RKEKKA_FDADD(1) = 255
            bytTENPO_RKEKKA_FDADD(2) = 255
            bytTENPO_RKEKKA_FDADD(3) = 255
        End Sub

        '固定長データ処理用プロパティ
        Public ReadOnly Property Data() As Byte() Implements IFormat.Data
            Get
                Dim ret(27) As Byte

                Array.Copy(EncdJ.GetBytes(SubData(strKINKO_CD, 4)), 0, ret, 0, 4)
                Array.Copy(EncdJ.GetBytes(SubData(strSIT_CD, 3)), 0, ret, 4, 3)
                ret(7) = bytYOBI1

                Array.Copy(bytTENPO_TKEN, 0, ret, 8, 2)
                Array.Copy(bytTENPO_RKEN, 0, ret, 10, 2)
                Array.Copy(bytTENPO_ERR_RKEN, 0, ret, 12, 2)
                ret(14) = bytRKEKKA_SYOKAI
                ret(15) = bytYOBI2

                Array.Copy(bytTENPO_TSTART_FDADD, 0, ret, 16, 4)
                Array.Copy(bytTENPO_TFINISH_FDADD, 0, ret, 20, 4)
                Array.Copy(bytTENPO_RKEKKA_FDADD, 0, ret, 24, 4)

                Return ret
            End Get
        End Property
    End Structure
#End Region

#Region "予備"
    '　予備
    Public Structure T_RIENT77E
        Implements ClsT_RIENT77.IFormat

        Private bytDATA_SIKIBETU() As Byte      '予備1

        Public Sub Init() Implements IFormat.Init
            bytDATA_SIKIBETU = CType(Array.CreateInstance(GetType(Byte), 84), Byte())
            bytDATA_SIKIBETU.Initialize()
        End Sub

        '固定長データ処理用プロパティ
        Public ReadOnly Property Data() As Byte() Implements IFormat.Data
            Get
                Dim ret(83) As Byte

                Array.Copy(bytDATA_SIKIBETU, 0, ret, 0, 84)

                Return ret
            End Get
        End Property
    End Structure
#End Region

    ' タンキングヘッダ
    Public TANKING_HEAD As T_RIENT77T

    ' 店舗情報レコード
    Public TENPO_INFOREC(31) As T_RIENT77M

    ' 予備３
    Public DATA_SIKIBETU As T_RIENT77E

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
    Protected Shared Function SubData(ByVal value As String, ByVal len As Integer, _
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

    Public Sub New()

    End Sub
End Class
