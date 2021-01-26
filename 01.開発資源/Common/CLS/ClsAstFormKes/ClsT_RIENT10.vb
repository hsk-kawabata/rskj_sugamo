Imports System
Imports System.IO
Imports System.Text
Imports System.Collections

' タンキングヘッダフォーマット
Public Class ClsT_RIENT10

    ' SHIT-JISエンコーディング
    Protected Shared EncdJ As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")

    ' セパレータ
    Protected Shared SEPARATE As String = Microsoft.VisualBasic.ChrW(&H1E)

    ' 固定長構造体用インターフェース
    Protected Interface IFormat
        ' データ
        Sub Init_10()
        ReadOnly Property Data_10() As Byte()
        Sub Init_48()
        ReadOnly Property Data_48() As Byte()
    End Interface

#Region "タンキングヘッダ部"
    '　タンキングヘッダ部
    Public Structure T_RIENT10T
        Implements ClsT_RIENT10.IFormat

        Dim bytDATA_LENGTH() As Byte     'データ長(1)
        Dim bytDATA_SIKIBETU As Byte     'データ識別 
        Dim bytDATA_KBN As Byte          'データ区分
        Dim bytKINGAKU_SEPA() As Byte     '金額セパレータ(1)
        Dim strSOFT_NO1 As String        'ソフト機番1
        Dim bytTANMATU As Byte           '端末識別   
        Dim bytKINKO_CODE() As Byte      '金庫コード(3)
        Dim bytTENPO_CODE() As Byte      '店舗コード(2)  
        Dim bytYOBI1 As Byte             '予備1 
        Dim bytTANKING_NO() As Byte       'タンキング連番(1) 
        Dim strOPE_CODE As String        'オペコード   
        Dim bytYOBI2 As Byte             '予備2    
        Dim bytNEXT_DATA_ADD() As Byte   '次データアドレス(3)
        Dim bytFIN_FLG As Byte           '終了フラグ 
        Dim bytYOBI3() As Byte           '予備3(2)  
        'テキストヘッダ
        Dim bytDENBUN_JOKEN As Byte      '電文条件 
        Dim bytTUUBAN() As Byte          '通番(3) 
        Dim bytNYURYOKU() As Byte        '入力条件(2)   
        Dim bytOPERATE_KEY As Byte       'オペレータキー  
        Dim bytBAITAI_SET As Byte        '媒体セット情報  
        Dim bytYAKUSEKI As Byte          '役席キー   
        Dim bytTORIHIKI_MODE As Byte     '取引モードキー 
        Dim strKAMOKU_CODE As String     '科目モード
        Dim strTORIHIKI_CODE As String   '取引コード
        Dim strKAWASESOFTNO As String    'ソフト機番（為替TEXTのみ）
        Dim bytKAWASETUUBAN() As Byte     '為替通番（為替TEXTのみ）
        Dim strSEPARATE_1 As String      '第1セパレータ  
        Dim strKINTEN_CD As String        '金店コード     
        Dim strSEPARATE_2 As String      '第2セパレータ    
        '口座移管読替データ
        Dim strTANKING_DATA As String    'タンキングデータ 

        Public Sub Init_10() Implements IFormat.Init_10
            bytDATA_LENGTH = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytDATA_LENGTH(0) = 0
            bytDATA_LENGTH(1) = 224

            bytDATA_SIKIBETU = 1
            bytDATA_KBN = 0

            bytKINGAKU_SEPA = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytKINGAKU_SEPA(0) = 0
            bytKINGAKU_SEPA(1) = 0

            strSOFT_NO1 = "V"
            bytTANMATU = 128

            bytKINKO_CODE = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytKINKO_CODE(0) = 0
            bytKINKO_CODE(1) = 0
            bytKINKO_CODE(2) = 0
            bytKINKO_CODE(3) = 0

            bytTENPO_CODE = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytTENPO_CODE(0) = 0
            bytTENPO_CODE(1) = 0
            bytTENPO_CODE(2) = 0

            bytYOBI1 = 0

            bytTANKING_NO = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytTANKING_NO(0) = 0
            bytTANKING_NO(1) = 0

            bytYOBI2 = 0

            bytNEXT_DATA_ADD = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytNEXT_DATA_ADD(0) = 0
            bytNEXT_DATA_ADD(1) = 0
            bytNEXT_DATA_ADD(2) = 0
            bytNEXT_DATA_ADD(3) = 0

            bytFIN_FLG = 0

            bytYOBI3 = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytYOBI3(0) = 0
            bytYOBI3(1) = 0
            bytYOBI3(2) = 0

            bytDENBUN_JOKEN = 1

            bytTUUBAN = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytTUUBAN(0) = 0
            bytTUUBAN(1) = 0
            bytTUUBAN(2) = 0
            bytTUUBAN(3) = 0

            bytNYURYOKU = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytNYURYOKU(0) = 0
            bytNYURYOKU(1) = 0
            bytNYURYOKU(2) = 0

            bytOPERATE_KEY = 1
            bytBAITAI_SET = 3
            bytYAKUSEKI = 0
            bytTORIHIKI_MODE = 0
            strSEPARATE_1 = SEPARATE
            strKINTEN_CD = New String(" "c, 7)
            strSEPARATE_2 = SEPARATE

            strTANKING_DATA = New String(" "c, 198)
        End Sub

        '固定長データ処理用プロパティ
        Public ReadOnly Property Data_10() As Byte() Implements IFormat.Data_10
            Get
                Dim ret(511) As Byte

                Array.Copy(bytDATA_LENGTH, 0, ret, 0, 2)
                ret(2) = bytDATA_SIKIBETU
                ret(3) = bytDATA_KBN
                Array.Copy(bytKINGAKU_SEPA, 0, ret, 4, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strSOFT_NO1, 1)), 0, ret, 6, 1)
                ret(7) = bytTANMATU
                Array.Copy(bytKINKO_CODE, 0, ret, 8, 4)
                Array.Copy(bytTENPO_CODE, 0, ret, 12, 3)
                ret(15) = bytYOBI1
                Array.Copy(bytTANKING_NO, 0, ret, 16, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strOPE_CODE, 5)), 0, ret, 18, 5)
                ret(23) = bytYOBI2
                Array.Copy(bytNEXT_DATA_ADD, 0, ret, 24, 4)
                ret(28) = bytFIN_FLG
                Array.Copy(bytYOBI3, 0, ret, 29, 3)
                ret(32) = bytDENBUN_JOKEN
                Array.Copy(bytTUUBAN, 0, ret, 33, 4)
                Array.Copy(bytNYURYOKU, 0, ret, 37, 3)
                ret(40) = bytOPERATE_KEY
                ret(41) = bytBAITAI_SET
                ret(42) = bytYAKUSEKI
                ret(43) = bytTORIHIKI_MODE
                Array.Copy(EncdJ.GetBytes(SubData(strKAMOKU_CODE, 2)), 0, ret, 44, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strTORIHIKI_CODE, 3)), 0, ret, 46, 3)
                Array.Copy(EncdJ.GetBytes(SubData(strSEPARATE_1, 1)), 0, ret, 49, 1)
                Array.Copy(EncdJ.GetBytes(SubData(strKINTEN_CD, 7)), 0, ret, 50, 7)
                Array.Copy(EncdJ.GetBytes(SubData(strSEPARATE_2, 1)), 0, ret, 57, 1)
                Array.Copy(EncdJ.GetBytes(SubData(strTANKING_DATA, 198)), 0, ret, 58, 198)

                '***修正　20080926************************
                '113バイト目以降をNULLで埋める→口番訂正は138
                '58(タンキングデータヘッダ長) + タンキングデータ長
                For i As Integer = 58 + strTANKING_DATA.Length To ret.Length - 1
                    ret(i) = 0
                Next i
                '*****************************************

                Return ret
            End Get
        End Property

        Public Sub Init_48() Implements IFormat.Init_48
            bytDATA_LENGTH = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytDATA_LENGTH(0) = 0
            bytDATA_LENGTH(1) = 224

            bytDATA_SIKIBETU = 2
            bytDATA_KBN = 0

            bytKINGAKU_SEPA = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytKINGAKU_SEPA(0) = 0
            bytKINGAKU_SEPA(1) = 0

            strSOFT_NO1 = "V"
            bytTANMATU = 128

            bytKINKO_CODE = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytKINKO_CODE(0) = 0
            bytKINKO_CODE(1) = 0
            bytKINKO_CODE(2) = 0
            bytKINKO_CODE(3) = 0

            bytTENPO_CODE = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytTENPO_CODE(0) = 0
            bytTENPO_CODE(1) = 0
            bytTENPO_CODE(2) = 0

            bytYOBI1 = 0

            bytTANKING_NO = CType(Array.CreateInstance(GetType(Byte), 2), Byte())
            bytTANKING_NO(0) = 0
            bytTANKING_NO(1) = 0

            bytYOBI2 = 0

            bytNEXT_DATA_ADD = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytNEXT_DATA_ADD(0) = 0
            bytNEXT_DATA_ADD(1) = 0
            bytNEXT_DATA_ADD(2) = 0
            bytNEXT_DATA_ADD(3) = 0

            bytFIN_FLG = 0

            bytYOBI3 = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytYOBI3(0) = 0
            bytYOBI3(1) = 0
            bytYOBI3(2) = 0

            bytDENBUN_JOKEN = 2

            bytTUUBAN = CType(Array.CreateInstance(GetType(Byte), 4), Byte())
            bytTUUBAN(0) = 0
            bytTUUBAN(1) = 0
            bytTUUBAN(2) = 0
            bytTUUBAN(3) = 0

            bytNYURYOKU = CType(Array.CreateInstance(GetType(Byte), 3), Byte())
            bytNYURYOKU(0) = 0
            bytNYURYOKU(1) = 0
            bytNYURYOKU(2) = 0

            bytOPERATE_KEY = 1
            bytBAITAI_SET = 3
            bytYAKUSEKI = 0
            bytTORIHIKI_MODE = 0

            strKAWASESOFTNO = "V"   'ソフト機番（為替TEXTのみ）
            bytKAWASETUUBAN = CType(Array.CreateInstance(GetType(Byte), 5), Byte()) '為替通番（為替TEXTのみ）
            bytKAWASETUUBAN(0) = 0
            bytKAWASETUUBAN(1) = 0
            bytKAWASETUUBAN(2) = 0
            bytKAWASETUUBAN(3) = 0
            bytKAWASETUUBAN(4) = 0

            strSEPARATE_1 = SEPARATE
            strKINTEN_CD = New String(" "c, 7)
            strSEPARATE_2 = SEPARATE

            strTANKING_DATA = New String(" "c, 198)
        End Sub

        '固定長データ処理用プロパティ
        Public ReadOnly Property Data_48() As Byte() Implements IFormat.Data_48
            Get
                Dim ret(511) As Byte

                Array.Copy(bytDATA_LENGTH, 0, ret, 0, 2)
                ret(2) = bytDATA_SIKIBETU
                ret(3) = bytDATA_KBN
                Array.Copy(bytKINGAKU_SEPA, 0, ret, 4, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strSOFT_NO1, 1)), 0, ret, 6, 1)
                ret(7) = bytTANMATU
                Array.Copy(bytKINKO_CODE, 0, ret, 8, 4)
                Array.Copy(bytTENPO_CODE, 0, ret, 12, 3)
                ret(15) = bytYOBI1
                Array.Copy(bytTANKING_NO, 0, ret, 16, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strOPE_CODE, 5)), 0, ret, 18, 5)
                ret(23) = bytYOBI2
                Array.Copy(bytNEXT_DATA_ADD, 0, ret, 24, 4)
                ret(28) = bytFIN_FLG
                Array.Copy(bytYOBI3, 0, ret, 29, 3)
                ret(32) = bytDENBUN_JOKEN
                Array.Copy(bytTUUBAN, 0, ret, 33, 4)
                Array.Copy(bytNYURYOKU, 0, ret, 37, 3)
                ret(40) = bytOPERATE_KEY
                ret(41) = bytBAITAI_SET
                ret(42) = bytYAKUSEKI
                ret(43) = bytTORIHIKI_MODE
                Array.Copy(EncdJ.GetBytes(SubData(strKAMOKU_CODE, 2)), 0, ret, 44, 2)
                Array.Copy(EncdJ.GetBytes(SubData(strTORIHIKI_CODE, 3)), 0, ret, 46, 3)
                Array.Copy(EncdJ.GetBytes(SubData(strSOFT_NO1, 1)), 0, ret, 49, 1)
                Array.Copy(bytKAWASETUUBAN, 0, ret, 50, 5)
                Array.Copy(EncdJ.GetBytes(SubData(strSEPARATE_1, 1)), 0, ret, 55, 1)
                Array.Copy(EncdJ.GetBytes(SubData(strKINTEN_CD, 7)), 0, ret, 56, 7)
                Array.Copy(EncdJ.GetBytes(SubData(strSEPARATE_2, 1)), 0, ret, 63, 1)
                Array.Copy(EncdJ.GetBytes(SubData(strTANKING_DATA, 198)), 0, ret, 64, 198)

                '***修正　20080926************************
                For i As Integer = 64 + strTANKING_DATA.Length To ret.Length - 1
                    ret(i) = 0
                Next i
                '*****************************************

                Return ret
            End Get
        End Property

    End Structure
#End Region

#Region "最終レコード"
    '　最終レコード
    Public Structure T_RIENT10L
        Implements ClsT_RIENT10.IFormat

        Private bytBYTE_DATA() As Byte           'バイトデータ(511)

        Public Sub Init() Implements IFormat.Init_10, IFormat.Init_48
            bytBYTE_DATA = CType(Array.CreateInstance(GetType(Byte), 512), Byte())
            bytBYTE_DATA.Initialize()

            bytBYTE_DATA(0) = 255
            bytBYTE_DATA(1) = 255
            'bytBYTE_DATA(48) = 30
            'bytBYTE_DATA(56) = 30
        End Sub

        '固定長データ処理用プロパティ
        Public ReadOnly Property Data() As Byte() Implements IFormat.Data_10, IFormat.Data_48
            Get
                Dim ret(511) As Byte

                Array.Copy(bytBYTE_DATA, 0, ret, 0, 512)

                Return ret
            End Get
        End Property
    End Structure
#End Region

    ' タンキングデータ
    Public TANKING_DATA As T_RIENT10T

    ' タンキング最終
    Public TANKING_LAST As T_RIENT10L

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

End Class
