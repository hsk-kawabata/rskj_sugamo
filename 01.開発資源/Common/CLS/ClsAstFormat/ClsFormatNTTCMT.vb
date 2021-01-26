Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' ＮＴＴ口座振替データフォーマットクラス
Public Class CFormatNTTCMT
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 180

    '------------------------------------------
    'ＮＴＴフォーマット
    '------------------------------------------
    'ヘッダレコード
    Public Structure NTRECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NT1 As String     ' データ区分
        <VBFixedString(2)> Public NT2 As String     ' 種別コード
        <VBFixedString(4)> Public NT3 As String     ' 引落銀行番号
        <VBFixedString(4)> Public NT4 As String     ' 請求年月
        <VBFixedString(1)> Public NT5 As String     ' 群
        <VBFixedString(6)> Public NT6 As String     ' お支払期日
        <VBFixedString(6)> Public NT7 As String     ' 再振替日
        <VBFixedString(3)> Public NT8 As String     ' 料金センタコード
        <VBFixedString(40)> Public NT9 As String    ' 料金センタ名
        <VBFixedString(4)> Public NT10 As String    ' 取引銀行番号
        <VBFixedString(3)> Public NT11 As String    ' 取引店舗番号
        <VBFixedString(15)> Public NT12 As String   ' 取引銀行名
        <VBFixedString(1)> Public NT13 As String    ' 預金種目（委託者）
        <VBFixedString(7)> Public NT14 As String    ' 口座番号（委託者）
        <VBFixedString(6)> Public NT15 As String    ' 振替日
        <VBFixedString(77)> Public NT16 As String   ' ＮＴＴ使用欄

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {NT1, NT2, NT3, NT4, NT5, NT6, NT7, _
                            NT8, NT9, NT10, NT11, NT12, NT13, NT14, _
                            NT15, NT16})
            End Get
            Set(ByVal value As String)
                NT1 = CuttingData(value, 1)
                NT2 = CuttingData(value, 2)
                NT3 = CuttingData(value, 4)
                NT4 = CuttingData(value, 4)
                NT5 = CuttingData(value, 1)
                NT6 = CuttingData(value, 6)
                NT7 = CuttingData(value, 6)
                NT8 = CuttingData(value, 3)
                NT9 = CuttingData(value, 40)
                NT10 = CuttingData(value, 4)
                NT11 = CuttingData(value, 3)
                NT12 = CuttingData(value, 15)
                NT13 = CuttingData(value, 1)
                NT14 = CuttingData(value, 7)
                NT15 = CuttingData(value, 6)
                NT16 = CuttingData(value, 77)
            End Set
        End Property
    End Structure
    Public NTT_REC1 As NTRECORD1

    'データレコード
    Structure NTRECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NT1 As String      ' データ区分
        <VBFixedString(4)> Public NT2 As String      ' 入金通知コード
        <VBFixedString(4)> Public NT3 As String      ' 振替通知コード
        <VBFixedString(5)> Public NT4 As String      ' 市外局番
        <VBFixedString(4)> Public NT5 As String      ' 市内局番
        <VBFixedString(4)> Public NT6 As String      ' 電話番号
        <VBFixedString(1)> Public NT7 As String      ' 分割番号
        <VBFixedString(1)> Public NT8 As String      ' 消込対象印
        <VBFixedString(2)> Public NT9 As String      ' 送付区分
        <VBFixedString(4)> Public NT10 As String     ' 銀行番号
        <VBFixedString(3)> Public NT11 As String     ' 店舗番号
        <VBFixedString(1)> Public NT12 As String     ' 預金種目
        <VBFixedString(7)> Public NT13 As String     ' 口座番号
        <VBFixedString(2)> Public NT14 As String     ' 振替要否コード
        <VBFixedString(2)> Public NT15 As String     ' 異動情報		
        <VBFixedString(1)> Public NT16 As String     ' 早期領収書希望者
        <VBFixedString(1)> Public NT17 As String     ' 振替結果印		
        <VBFixedString(10)> Public NT18 As String    ' 請求額			
        <VBFixedString(13)> Public NT19 As String    ' 旧電話番号		
        <VBFixedString(40)> Public NT20 As String    ' 口座名義		
        <VBFixedString(1)> Public NT21 As String     ' NTT使用欄		
        <VBFixedString(16)> Public NT22 As String    ' 局名			
        <VBFixedString(16)> Public NT23 As String    ' 電話取扱局名	
        <VBFixedString(1)> Public NT24 As String     ' 疑似局番印		
        <VBFixedString(32)> Public NT25 As String    ' 空白			
        <VBFixedString(4)> Public NT26 As String     ' 入金通知コード	

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {NT1, NT2, NT3, NT4, NT5, NT6, NT7, _
                            NT8, NT9, NT10, NT11, NT12, NT13, NT14, _
                            NT15, NT16, NT17, NT18, NT19, NT20, NT21, _
                            NT22, NT23, NT24, NT25, NT26})
            End Get
            Set(ByVal Value As String)
                NT1 = CuttingData(Value, 1)
                NT2 = CuttingData(Value, 4)
                NT3 = CuttingData(Value, 4)
                NT4 = CuttingData(Value, 5)
                NT5 = CuttingData(Value, 4)
                NT6 = CuttingData(Value, 4)
                NT7 = CuttingData(Value, 1)
                NT8 = CuttingData(Value, 1)
                NT9 = CuttingData(Value, 2)
                NT10 = CuttingData(Value, 4)
                NT11 = CuttingData(Value, 3)
                NT12 = CuttingData(Value, 1)
                NT13 = CuttingData(Value, 7)
                NT14 = CuttingData(Value, 2)
                NT15 = CuttingData(Value, 2)
                NT16 = CuttingData(Value, 1)
                NT17 = CuttingData(Value, 1)
                NT18 = CuttingData(Value, 10)
                NT19 = CuttingData(Value, 13)
                NT20 = CuttingData(Value, 40)
                NT21 = CuttingData(Value, 1)
                NT22 = CuttingData(Value, 16)
                NT23 = CuttingData(Value, 16)
                NT24 = CuttingData(Value, 1)
                NT25 = CuttingData(Value, 32)
                NT26 = CuttingData(Value, 4)
            End Set
        End Property
    End Structure
    Public NTT_REC2 As NTRECORD2

    'トレーラレコード
    Structure NTRECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NT1 As String     ' データ区分				
        <VBFixedString(6)> Public NT2 As String     ' 振替分件数				
        <VBFixedString(12)> Public NT3 As String    ' 振替分金額				
        <VBFixedString(6)> Public NT4 As String     ' 振替不能分件数			
        <VBFixedString(12)> Public NT5 As String    ' 振替不能分金額			
        <VBFixedString(6)> Public NT6 As String     ' 振替保留分件数			
        <VBFixedString(12)> Public NT7 As String    ' 振替保留分金額			
        <VBFixedString(6)> Public NT8 As String     ' 保留分の内振替分件数	
        <VBFixedString(12)> Public NT9 As String    ' 保留分の内振替分金額	
        <VBFixedString(6)> Public NT10 As String    ' 保留分の内不能分件数
        <VBFixedString(12)> Public NT11 As String   ' 保留分の内不能分金額
        <VBFixedString(6)> Public NT12 As String    ' 振替依頼分の合計件数
        <VBFixedString(12)> Public NT13 As String   ' 振替依頼分の合計金額
        <VBFixedString(4)> Public NT14 As String    ' 振替否件数			
        <VBFixedString(6)> Public NT15 As String    ' 早期領収書希望件数	
        <VBFixedString(61)> Public NT16 As String   ' ＮＴＴ使用欄空白	

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {NT1, NT2, NT3, NT4, NT5, NT6, NT7, _
                            NT8, NT9, NT10, NT11, NT12, NT13, NT14, _
                            NT15, NT16})
            End Get
            Set(ByVal Value As String)
                NT1 = CuttingData(Value, 1)
                NT2 = CuttingData(Value, 6)
                NT3 = CuttingData(Value, 12)
                NT4 = CuttingData(Value, 6)
                NT5 = CuttingData(Value, 12)
                NT6 = CuttingData(Value, 6)
                NT7 = CuttingData(Value, 12)
                NT8 = CuttingData(Value, 6)
                NT9 = CuttingData(Value, 12)
                NT10 = CuttingData(Value, 6)
                NT11 = CuttingData(Value, 12)
                NT12 = CuttingData(Value, 6)
                NT13 = CuttingData(Value, 12)
                NT14 = CuttingData(Value, 4)
                NT15 = CuttingData(Value, 6)
                NT16 = CuttingData(Value, 61)
            End Set
        End Property
    End Structure
    Public NTT_REC8 As NTRECORD8

    'エンドレコード
    Structure NTRECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NT1 As String    ' データ区分		
        <VBFixedString(90)> Public NT2 As String    ' ＮＴＴ使用欄	
        <VBFixedString(3)> Public NT3 As String    ' 取引銀行		
        <VBFixedString(86)> Public NT4 As String    ' ＮＴＴ使用欄空白

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {NT1, NT2, NT3, NT4})
            End Get
            Set(ByVal Value As String)
                NT1 = CuttingData(Value, 1)
                NT2 = CuttingData(Value, 90)
                NT3 = CuttingData(Value, 3)
                NT4 = CuttingData(Value, 86)
            End Set
        End Property
    End Structure
    Public NTT_REC9 As NTRECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"1", "8", "9"}

        FtranPfile = "NTT.P"

        HeaderKubun = New String() {"1"}
        DataKubun = New String() {"2"}
        TrailerKubun = New String() {"8"}
    End Sub

    '
    ' 機能　 ： 規定文字チェック ＆　文字置換処理
    '
    ' 戻り値 ： 不正文字の位置
    '
    ' 備考　 ： RepaceString()関数にて文字置換を実施
    '           置換対象文字は，不正文字にはならないはず
    '
    Public Overrides Function CheckRegularString() As Long
        Dim buff As New StringBuilder(RecordLen)

        Select Case RecordData.Substring(0, 1)
            Case "1"        ' ヘッダレコード
            Case "2"        ' データレコード
            Case "8"        ' トレーラ
            Case "9"        ' エンド
        End Select

        Return MyBase.CheckRegularString()
    End Function

End Class
