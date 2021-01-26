'Imports System.IO
'Imports CrystalDecisions
'Imports CrystalDecisions.Shared
'Imports System.Runtime.InteropServices

'' クリスタルレポート制御クラス（.netバージョン）
'Public Class CR
'    Protected mReportPath As String                 ' レポートパス
'    Protected mReportName As String                 ' レポート名
'    Protected mRecordSelectionFormula As String     ' レポート選択式
'    Protected mErrorMessage As String               ' エラー情報
'    Protected mFormulaFieldDefinition As String     ' フィールド値

'    Protected mCsvPath As String                    ' ＣＳＶ出力パス
'    Protected mCsvName As String                    ' ＣＳＶ名

'    '' クリスタルレポート ドキュメントオブジェクト
'    'Private crReportDocument As CrystalReports.Engine.ReportDocument

'    '' プレビュー用
'    'Protected crPreview As CrystalDecisions.Windows.Forms.CrystalReportViewer

'    '' レポート接続情報
'    'Protected mConnInfo As CrystalDecisions.Shared.ConnectionInfo

'    Public Sub New()
'        'mConnInfo = New CrystalDecisions.Shared.ConnectionInfo
'        'mConnInfo.ServerName = CASTCommon.DB.SOURCE
'        'mConnInfo.UserID = CASTCommon.DB.USER
'        'mConnInfo.Password = CASTCommon.DB.PASSWORD

'        'レポートパスデフォルト指定
'        mReportPath = Path.GetDirectoryName(ReportName)
'        If mReportPath = "" Then
'            CASTCommon.GetFSKJIni("COMMON", "LST", mReportPath)
'            mReportPath = Path.GetDirectoryName(mReportPath)
'        End If

'        mReportName = ""
'        mRecordSelectionFormula = ""
'        mErrorMessage = ""
'        mCsvPath = ""
'        mCsvName = ""
'    End Sub

'    ' reportname : レポート名 フルパス
'    Public Sub New(ByVal reportname As String)
'        MyClass.New()

'        mReportName = Path.GetFileName(reportname)

'        Call SetupReport()
'    End Sub

'    ' reportpath : パス名
'    ' reportname : レポート名
'    Public Sub New(ByVal reportpath As String, ByVal reportname As String)
'        MyClass.New()

'        mReportPath = reportpath
'        mReportName = reportname

'        Call SetupReport()
'    End Sub

'    ' レポートパス
'    Public Property ReportPath() As String
'        Get
'            Return mReportPath
'        End Get
'        Set(ByVal Value As String)
'            mReportPath = Value
'        End Set
'    End Property

'    ' レポート名
'    Public Overridable Property ReportName() As String
'        Get
'            Return mReportName
'        End Get
'        Set(ByVal Value As String)
'            If mReportPath = "" Then
'                mReportPath = Path.GetDirectoryName(Value)
'            End If
'            mReportName = Path.GetFileName(Value)

'            If File.Exists(Path.Combine(mReportPath, mReportName)) = True Then
'                Call SetupReport()
'            End If
'        End Set
'    End Property

'    ' CSVパス
'    Public Property CsvPath() As String
'        Get
'            Return mCsvPath
'        End Get
'        Set(ByVal Value As String)
'            mCsvPath = Value
'        End Set
'    End Property

'    ' CSV名
'    Public Property CsvName() As String
'        Get
'            Return mCsvName
'        End Get
'        Set(ByVal Value As String)
'            If mCsvPath = "" Then
'                mCsvPath = Path.GetDirectoryName(Value)
'            End If
'            mCsvName = Path.GetFileName(Value)
'        End Set
'    End Property

'    ' レポート選択式
'    Public Overridable Property RecordSelectionFormula() As String
'        Get
'            Return mRecordSelectionFormula
'        End Get
'        Set(ByVal Value As String)
'            mRecordSelectionFormula = Value
'            If Not crReportDocument Is Nothing Then
'                If Not mRecordSelectionFormula Is Nothing AndAlso mRecordSelectionFormula.Equals("") = False Then
'                    crReportDocument.RecordSelectionFormula = mRecordSelectionFormula
'                End If
'            End If
'        End Set
'    End Property

'    Public Overridable ReadOnly Property FormulaFieldsCount() As Integer
'        Get
'            Return crReportDocument.DataDefinition.FormulaFields.Count
'        End Get
'    End Property

'    Public Overridable ReadOnly Property FormulaFieldsItem(ByVal num As Integer) As String
'        Get
'            Return crReportDocument.DataDefinition.FormulaFields(num).FormulaName
'        End Get
'    End Property

'    Public Overridable Property FormulaFieldsText(ByVal num As Integer) As String
'        Get
'            Return crReportDocument.DataDefinition.FormulaFields(num).Text
'        End Get
'        Set(ByVal Value As String)
'            crReportDocument.DataDefinition.FormulaFields(num).Text = Value
'        End Set
'    End Property

'    Public Overridable WriteOnly Property SortOrder(ByVal tableName As String, ByVal index As Integer) As String
'        Set(ByVal fieldName As String)
'            Dim CRField As CrystalDecisions.CrystalReports.Engine.DatabaseFieldDefinition
'            CRField = crReportDocument.Database.Tables(tableName).Fields(fieldName)

'            crReportDocument.DataDefinition.SortFields.Item(index).Field = CRField
'        End Set
'    End Property


'    '
'    ' 機能　 ： ＤＢ接続情報設定
'    '
'    ' 引数　 ： ARG1 - データソース名
'    ' 　　　 　 ARG2 - ユーザＩＤ
'    ' 　　　 　 ARG3 - パスワード
'    '
'    ' 戻り値 ： 
'    '
'    ' 備考　 ： 
'    '
'    Public Sub SetConnectionInfo(ByVal servername As String, ByVal userid As String, ByVal password As String)
'        mConnInfo = New CrystalDecisions.Shared.ConnectionInfo
'        mConnInfo.ServerName = servername
'        mConnInfo.UserID = userid
'        mConnInfo.Password = password
'    End Sub

'    '
'    ' 機能　 ： 印刷
'    '
'    ' 引数　 ： ARG1 - 
'    '
'    ' 戻り値 ： True-成功，False-失敗
'    '
'    ' 備考　 ： 
'    '
'    Public Overridable Overloads Function PrintOut() As Boolean
'        Try
'            ' 印刷
'            crReportDocument.PrintToPrinter(1, False, 0, 0)
'        Catch ex As Exception
'            mErrorMessage = ex.Message
'            Return False
'        End Try

'        Return True
'    End Function

'    '
'    ' 機能　 ： 印刷
'    '
'    ' 引数　 ： ARG1 - プリンタ名
'    '
'    ' 戻り値 ： True-成功，False-失敗
'    '
'    ' 備考　 ： 
'    '
'    Public Overridable Overloads Function PrintOut(ByVal printername As String) As Boolean
'        Try
'            ' 出力プリンタ指定
'            crReportDocument.PrintOptions.PrinterName = printername

'            ' 印刷
'            crReportDocument.PrintToPrinter(1, True, 0, 0)
'        Catch ex As Exception
'            mErrorMessage = ex.Message
'            Return False
'        End Try

'        Return True
'    End Function

'    '
'    ' 機能　 ： プレビュー
'    '
'    ' 引数　 ： ARG1 - 
'    '
'    ' 戻り値 ： True-成功，False-失敗
'    '
'    ' 備考　 ： 
'    '
'    Public Overridable Function Preview() As Boolean
'        Try
'            ' プレビュー
'            Dim frm As New FrmViewer
'            frm.CrystalReportViewer1.ReportSource = crReportDocument
'            With frm.CrystalReportViewer1
'                .Zoom(1)
'            End With
'            frm.ShowDialog()
'            frm.Dispose()
'        Catch ex As Exception
'            mErrorMessage = ex.Message
'            Return False
'        End Try

'        Return True
'    End Function

'    '
'    ' 機能　 ： ＰＤＦ出力
'    '
'    ' 引数　 ： ARG1 - 
'    '
'    ' 戻り値 ： True-成功，False-失敗
'    '
'    ' 備考　 ： 
'    '
'    Public Overridable Function ExportPDF(ByVal diskfilename As String) As Boolean
'        Try
'            ' PDF出力
'            Dim exportOpts As New ExportOptions
'            Dim diskOpts As New DiskFileDestinationOptions
'            exportOpts = crReportDocument.ExportOptions

'            With exportOpts
'                .ExportDestinationType = ExportDestinationType.DiskFile
'                .ExportFormatType = ExportFormatType.PortableDocFormat
'            End With
'            diskOpts.DiskFileName = diskfilename
'            exportOpts.DestinationOptions = diskOpts
'            crReportDocument.Export()
'        Catch ex As Exception
'            mErrorMessage = ex.Message
'            Return False
'        End Try

'        Return True
'    End Function

'    '
'    ' 機能　 ： ＣＳＶ出力
'    '
'    ' 引数　 ： ARG1 - 
'    '
'    ' 戻り値 ： True-成功，False-失敗
'    '
'    ' 備考　 ： 
'    '
'    Public Overridable Function ExportCSV(ByVal diskfilename As String) As Boolean
'        mErrorMessage = "ＣＳＶ出力機能はありません"
'        Return False
'    End Function

'    '
'    ' 機能　 ： レポート設定
'    '
'    ' 引数　 ： ARG1 - 
'    '
'    ' 戻り値 ： 
'    '
'    ' 備考　 ： 
'    '
'    Protected Overridable Sub SetupReport()
'        ' クリスタルレポート 変数
'        Dim crDatabase As CrystalReports.Engine.Database
'        Dim crTables As CrystalReports.Engine.Tables
'        Dim crTable As CrystalReports.Engine.Table
'        Dim crTableLogOnInfo As CrystalDecisions.Shared.TableLogOnInfo

'        If Not crReportDocument Is Nothing Then
'            crReportDocument.Close()
'            crReportDocument = Nothing
'        End If

'        crReportDocument = New CrystalReports.Engine.ReportDocument
'        Call crReportDocument.Load(Path.Combine(mReportPath, mReportName))
'        crDatabase = crReportDocument.Database
'        crTables = crDatabase.Tables
'        For Each crTable In crTables
'            ' ログオン情報設定
'            crTableLogOnInfo = crTable.LogOnInfo
'            crTableLogOnInfo.ConnectionInfo = mConnInfo
'            crTable.ApplyLogOnInfo(crTableLogOnInfo)
'            crTable.Location = CASTCommon.DB.USER & "." & crTable.Location
'        Next

'        'If Not mRecordSelectionFormula Is Nothing AndAlso mRecordSelectionFormula.Equals("") = False Then
'        '    crReportDocument.RecordSelectionFormula = mRecordSelectionFormula
'        'End If
'    End Sub

'    '
'    ' 機能　 ： エラー情報クリア
'    '
'    ' 引数　 ： ARG1 - 
'    '
'    ' 戻り値 ： 
'    '
'    ' 備考　 ： 
'    '
'    Public Sub ErrClear()
'        mErrorMessage = ""
'    End Sub

'    '
'    ' プロパティ： メッセージ
'    '
'    Public Property Message() As String
'        Get
'            Return mErrorMessage
'        End Get
'        Set(ByVal Value As String)
'            mErrorMessage = Value
'        End Set
'    End Property

'    '
'    ' 機能　 ： 使用したＣＳＶファイルをどこかへコピーする
'    '
'    ' 引数　 ： ARG1 - destination コピー先ファイル名（フルパス指定） 
'    '
'    ' 戻り値 ： 
'    '
'    ' 備考　 ： 
'    '
'    Public Function CopyCSVFile(ByVal destination As String) As Boolean
'        If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
'            mErrorMessage = "コピー元のファイルが見つかりません"
'            Return False
'        End If

'        Try
'            File.Copy(Path.Combine(mCsvPath, mCsvName), destination, False)
'        Catch ex As Exception
'            mErrorMessage = ex.Message
'            Return False
'        End Try

'        Return True
'    End Function

'    ' ファイナライズ
'    Protected Overrides Sub Finalize()
'        MyBase.Finalize()

'        Try
'            crReportDocument.Close()
'            crReportDocument.Dispose()
'        Catch ex As Exception

'        End Try
'    End Sub
'End Class
