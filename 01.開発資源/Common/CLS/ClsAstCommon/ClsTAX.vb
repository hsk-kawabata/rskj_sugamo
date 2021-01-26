Imports System
Imports System.Collections
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic

''' <summary>
''' 消費税管理メインクラス
''' </summary>
''' <remarks></remarks>
Public Class ClsTAX

#Region "クラス定数"

#End Region

#Region "クラス変数"

    Private MainDB As CASTCommon.MyOracle

    Private Structure strcTaxmastInfo
        Dim TAX_ID As String
        Dim TAX As String
        Dim START_DATE As String
        Dim END_DATE As String

        Friend Sub SetOraData(ByVal oraReader As CASTCommon.MyOracleReader)
            TAX_ID = oraReader.GetString("TAX_ID_Z")
            TAX = oraReader.GetString("TAX_Z")
            START_DATE = oraReader.GetString("START_DATE_Z")
            END_DATE = oraReader.GetString("END_DATE_Z")
        End Sub
    End Structure
    Private TAXMAST As strcTaxmastInfo

    Private lstTax As ArrayList

    Private strZEIRITSU As String
    Public ReadOnly Property ZEIRITSU() As String
        Get
            Return Me.strZEIRITSU
        End Get
    End Property

    Private strZEIRITSU_ID As String
    Public ReadOnly Property ZEIRITSU_ID() As String
        Get
            Return Me.strZEIRITSU_ID
        End Get
    End Property

    '2013/12/27 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
    Private Structure strcInshizaimastInfo
        Dim INSHIZEI_ID As String
        Dim INSHIZEI1 As Integer
        Dim INSHIZEI2 As Integer
        Dim START_DATE As String
        Dim END_DATE As String

        Friend Sub SetOraData(ByVal oraReader As CASTCommon.MyOracleReader)
            INSHIZEI_ID = oraReader.GetString("INSHIZEI_ID_I")
            INSHIZEI1 = oraReader.GetInt("INSHIZEI1_I")
            INSHIZEI2 = oraReader.GetInt("INSHIZEI2_I")
            START_DATE = oraReader.GetString("START_DATE_I")
            END_DATE = oraReader.GetString("END_DATE_I")
        End Sub
    End Structure
    Private INSHIZEIMAST As strcInshizaimastInfo

    Private lstInshizei As ArrayList

    Private strINSHIZEI_ID As String
    Public ReadOnly Property INSHIZEI_ID() As String
        Get
            Return Me.strINSHIZEI_ID
        End Get
    End Property

    Private intINSHIZEI1 As Integer
    Public ReadOnly Property INSHIZEI1() As Integer
        Get
            Return intINSHIZEI1
        End Get
    End Property

    Private intINSHIZEI2 As Integer
    Public ReadOnly Property INSHIZEI2() As Integer
        Get
            Return Me.intINSHIZEI2
        End Get
    End Property
    '2013/12/27 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<

#End Region

#Region "コンストラクタ生成"

    ''' <summary>
    ''' コンストラクタ生成
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            '消費税マスタ読み込み
            Me.MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(Me.MainDB)

            With SQL
                .Length = 0
                .Append("select * from TAXMAST")
                .Append(" order by TAX_ID_Z")
            End With

            Me.lstTax = New ArrayList

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    Me.TAXMAST.SetOraData(oraReader)
                    Me.lstTax.Add(Me.TAXMAST)
                    oraReader.NextRead()
                End While
            End If

            '2013/12/27 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
            oraReader.Close()

            With SQL
                .Length = 0
                .Append("select * from INSHIZEIMAST")
                .Append(" order by INSHIZEI_ID_I")
            End With

            Me.lstInshizei = New ArrayList

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    Me.INSHIZEIMAST.SetOraData(oraReader)
                    Me.lstInshizei.Add(Me.INSHIZEIMAST)
                    oraReader.NextRead()
                End While
            End If
            '2013/12/27 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<

            oraReader.Close()
            oraReader = Nothing

            Me.MainDB.Close()
            Me.MainDB = Nothing

        Catch ex As Exception
            Throw
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If

            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If
        End Try
    End Sub

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 税率を取得します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetZeiritsu()
        Call GetZeiritsu(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))
    End Sub

    ''' <summary>
    ''' 税率を取得します。
    ''' </summary>
    ''' <param name="strDate">基準日</param>
    ''' <remarks></remarks>
    Public Sub GetZeiritsu(ByVal strDate As String)
        Try
            '初期値設定
            Me.strZEIRITSU_ID = "err"
            Me.strZEIRITSU = "err"

            For i As Integer = 0 To Me.lstTax.Count - 1
                Me.TAXMAST = Me.lstTax.Item(i)

                '引数で渡されてきた基準日が開始日と終了日に存在する場合に税率を設定する
                If Me.TAXMAST.START_DATE <= strDate AndAlso strDate <= Me.TAXMAST.END_DATE Then
                    Me.strZEIRITSU_ID = Me.TAXMAST.TAX_ID
                    Me.strZEIRITSU = Me.TAXMAST.TAX
                    Exit For
                End If
            Next

        Catch ex As Exception
            '異常終了した場合はerrを設定
            Me.strZEIRITSU_ID = "err"
            Me.strZEIRITSU = "err"
        End Try
    End Sub

    ''' <summary>
    ''' 印紙税を取得します。
    ''' </summary>
    ''' <remarks>2013/12/27 saitou 印紙税対応 add</remarks>
    Public Sub GetInshizei()
        Call Me.GetInshizei(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))
    End Sub

    ''' <summary>
    ''' 印紙税を取得します。
    ''' </summary>
    ''' <param name="strDate">基準日</param>
    ''' <remarks>2013/12/27 saitou 印紙税対応 add</remarks>
    Public Sub GetInshizei(ByVal strDate As String)
        Try
            '初期値設定
            Me.strINSHIZEI_ID = "err"
            Me.intINSHIZEI1 = 0
            Me.intINSHIZEI2 = 0

            For i As Integer = 0 To Me.lstInshizei.Count - 1
                Me.INSHIZEIMAST = Me.lstInshizei.Item(i)

                '引数で渡されてきた基準日が開始日と終了日に存在する場合に税率を設定する
                If Me.INSHIZEIMAST.START_DATE <= strDate AndAlso strDate <= Me.INSHIZEIMAST.END_DATE Then
                    Me.strINSHIZEI_ID = Me.INSHIZEIMAST.INSHIZEI_ID
                    Me.intINSHIZEI1 = Me.INSHIZEIMAST.INSHIZEI1
                    Me.intINSHIZEI2 = Me.INSHIZEIMAST.INSHIZEI2
                    Exit For
                End If
            Next

        Catch ex As Exception
            '異常終了した場合はerrを設定
            Me.strINSHIZEI_ID = "err"
        End Try
    End Sub

#End Region

End Class
