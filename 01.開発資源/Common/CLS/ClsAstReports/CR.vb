'Imports System.IO
'Imports CrystalDecisions
'Imports CrystalDecisions.Shared
'Imports System.Runtime.InteropServices

'' �N���X�^�����|�[�g����N���X�i.net�o�[�W�����j
'Public Class CR
'    Protected mReportPath As String                 ' ���|�[�g�p�X
'    Protected mReportName As String                 ' ���|�[�g��
'    Protected mRecordSelectionFormula As String     ' ���|�[�g�I����
'    Protected mErrorMessage As String               ' �G���[���
'    Protected mFormulaFieldDefinition As String     ' �t�B�[���h�l

'    Protected mCsvPath As String                    ' �b�r�u�o�̓p�X
'    Protected mCsvName As String                    ' �b�r�u��

'    '' �N���X�^�����|�[�g �h�L�������g�I�u�W�F�N�g
'    'Private crReportDocument As CrystalReports.Engine.ReportDocument

'    '' �v���r���[�p
'    'Protected crPreview As CrystalDecisions.Windows.Forms.CrystalReportViewer

'    '' ���|�[�g�ڑ����
'    'Protected mConnInfo As CrystalDecisions.Shared.ConnectionInfo

'    Public Sub New()
'        'mConnInfo = New CrystalDecisions.Shared.ConnectionInfo
'        'mConnInfo.ServerName = CASTCommon.DB.SOURCE
'        'mConnInfo.UserID = CASTCommon.DB.USER
'        'mConnInfo.Password = CASTCommon.DB.PASSWORD

'        '���|�[�g�p�X�f�t�H���g�w��
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

'    ' reportname : ���|�[�g�� �t���p�X
'    Public Sub New(ByVal reportname As String)
'        MyClass.New()

'        mReportName = Path.GetFileName(reportname)

'        Call SetupReport()
'    End Sub

'    ' reportpath : �p�X��
'    ' reportname : ���|�[�g��
'    Public Sub New(ByVal reportpath As String, ByVal reportname As String)
'        MyClass.New()

'        mReportPath = reportpath
'        mReportName = reportname

'        Call SetupReport()
'    End Sub

'    ' ���|�[�g�p�X
'    Public Property ReportPath() As String
'        Get
'            Return mReportPath
'        End Get
'        Set(ByVal Value As String)
'            mReportPath = Value
'        End Set
'    End Property

'    ' ���|�[�g��
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

'    ' CSV�p�X
'    Public Property CsvPath() As String
'        Get
'            Return mCsvPath
'        End Get
'        Set(ByVal Value As String)
'            mCsvPath = Value
'        End Set
'    End Property

'    ' CSV��
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

'    ' ���|�[�g�I����
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
'    ' �@�\�@ �F �c�a�ڑ����ݒ�
'    '
'    ' �����@ �F ARG1 - �f�[�^�\�[�X��
'    ' �@�@�@ �@ ARG2 - ���[�U�h�c
'    ' �@�@�@ �@ ARG3 - �p�X���[�h
'    '
'    ' �߂�l �F 
'    '
'    ' ���l�@ �F 
'    '
'    Public Sub SetConnectionInfo(ByVal servername As String, ByVal userid As String, ByVal password As String)
'        mConnInfo = New CrystalDecisions.Shared.ConnectionInfo
'        mConnInfo.ServerName = servername
'        mConnInfo.UserID = userid
'        mConnInfo.Password = password
'    End Sub

'    '
'    ' �@�\�@ �F ���
'    '
'    ' �����@ �F ARG1 - 
'    '
'    ' �߂�l �F True-�����CFalse-���s
'    '
'    ' ���l�@ �F 
'    '
'    Public Overridable Overloads Function PrintOut() As Boolean
'        Try
'            ' ���
'            crReportDocument.PrintToPrinter(1, False, 0, 0)
'        Catch ex As Exception
'            mErrorMessage = ex.Message
'            Return False
'        End Try

'        Return True
'    End Function

'    '
'    ' �@�\�@ �F ���
'    '
'    ' �����@ �F ARG1 - �v�����^��
'    '
'    ' �߂�l �F True-�����CFalse-���s
'    '
'    ' ���l�@ �F 
'    '
'    Public Overridable Overloads Function PrintOut(ByVal printername As String) As Boolean
'        Try
'            ' �o�̓v�����^�w��
'            crReportDocument.PrintOptions.PrinterName = printername

'            ' ���
'            crReportDocument.PrintToPrinter(1, True, 0, 0)
'        Catch ex As Exception
'            mErrorMessage = ex.Message
'            Return False
'        End Try

'        Return True
'    End Function

'    '
'    ' �@�\�@ �F �v���r���[
'    '
'    ' �����@ �F ARG1 - 
'    '
'    ' �߂�l �F True-�����CFalse-���s
'    '
'    ' ���l�@ �F 
'    '
'    Public Overridable Function Preview() As Boolean
'        Try
'            ' �v���r���[
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
'    ' �@�\�@ �F �o�c�e�o��
'    '
'    ' �����@ �F ARG1 - 
'    '
'    ' �߂�l �F True-�����CFalse-���s
'    '
'    ' ���l�@ �F 
'    '
'    Public Overridable Function ExportPDF(ByVal diskfilename As String) As Boolean
'        Try
'            ' PDF�o��
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
'    ' �@�\�@ �F �b�r�u�o��
'    '
'    ' �����@ �F ARG1 - 
'    '
'    ' �߂�l �F True-�����CFalse-���s
'    '
'    ' ���l�@ �F 
'    '
'    Public Overridable Function ExportCSV(ByVal diskfilename As String) As Boolean
'        mErrorMessage = "�b�r�u�o�͋@�\�͂���܂���"
'        Return False
'    End Function

'    '
'    ' �@�\�@ �F ���|�[�g�ݒ�
'    '
'    ' �����@ �F ARG1 - 
'    '
'    ' �߂�l �F 
'    '
'    ' ���l�@ �F 
'    '
'    Protected Overridable Sub SetupReport()
'        ' �N���X�^�����|�[�g �ϐ�
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
'            ' ���O�I�����ݒ�
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
'    ' �@�\�@ �F �G���[���N���A
'    '
'    ' �����@ �F ARG1 - 
'    '
'    ' �߂�l �F 
'    '
'    ' ���l�@ �F 
'    '
'    Public Sub ErrClear()
'        mErrorMessage = ""
'    End Sub

'    '
'    ' �v���p�e�B�F ���b�Z�[�W
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
'    ' �@�\�@ �F �g�p�����b�r�u�t�@�C�����ǂ����փR�s�[����
'    '
'    ' �����@ �F ARG1 - destination �R�s�[��t�@�C�����i�t���p�X�w��j 
'    '
'    ' �߂�l �F 
'    '
'    ' ���l�@ �F 
'    '
'    Public Function CopyCSVFile(ByVal destination As String) As Boolean
'        If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
'            mErrorMessage = "�R�s�[���̃t�@�C����������܂���"
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

'    ' �t�@�C�i���C�Y
'    Protected Overrides Sub Finalize()
'        MyBase.Finalize()

'        Try
'            crReportDocument.Close()
'            crReportDocument.Dispose()
'        Catch ex As Exception

'        End Try
'    End Sub
'End Class
