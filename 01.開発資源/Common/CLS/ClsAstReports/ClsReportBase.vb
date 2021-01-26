Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon

' ���|�[�g��{�N���X
Public Class ClsReportBase
    Protected Structure PARAREPORT
        Dim Tenmei As String        ' �X��  
        Dim ReportName As String    ' ���[��
        Dim Itakusyamei As String   ' �ϑ��Җ�
        Dim Hiduke As String        ' ���t
        Dim HostCsvName As String   ' �z�X�g�ۑ��p�b�r�u�t�@�C��
        ReadOnly Property CsvName() As String
            Get
                Dim sBuff As String = ""
                If Not Tenmei Is Nothing AndAlso Tenmei <> "" Then
                    sBuff &= Tenmei & "_"
                End If
                If Not ReportName Is Nothing AndAlso ReportName <> "" Then
                    sBuff &= ReportName & "_"
                End If
                If Not Itakusyamei Is Nothing AndAlso Itakusyamei <> "" Then
                    sBuff &= Itakusyamei & "_"
                End If
                If Not Hiduke Is Nothing AndAlso Hiduke <> "" Then
                    sBuff &= Hiduke & "_"
                Else
                    ' sBuff &= CASTCommon.Calendar.Now.ToString("MMdd") & "_" 
                    sBuff &= CASTCommon.Calendar.Now.ToString("yyyyMMddHHmmss") & "_" & Process.GetCurrentProcess.Id & "_"
                End If

                Return ValidPath(sBuff)
            End Get
        End Property
    End Structure
    Protected InfoReport As PARAREPORT
    ' �b�r�u��
    Protected CsvName As String
    Public ReadOnly Property FileName() As String
        Get
            Return CsvName
        End Get
    End Property
    ' �b�r�u�p�X
    Protected CsvPath As String = GetFSKJIni("COMMON", "PRT")

    ' ���[��`��
    Protected ReportBaseName As String

    ' �b�r�u�p���J�N���X
    Public CSVObject As CSV

    ' ������
    Public MEMObject As MEM

    ' ���|�[�g���J���b�Z�[�W
    Public ReportMessage As String

    ' �������
    Public Copies As Integer = 1
    ' �����R�[�h
    Public DataCode As Integer = 0
    Public Enum DataCodeType
        SHIFT_JIS = 0
        UTF8 = 1
        UTF16 = 2
    End Enum

    ' �X���v���p�e�B
    Public Property Tenmei() As String
        Get
            Return InfoReport.Tenmei
        End Get
        Set(ByVal Value As String)
            InfoReport.Tenmei = Value
        End Set
    End Property

    ' �ϑ��Җ��v���p�e�B
    Public Property Itakusyamei() As String
        Get
            Return InfoReport.Itakusyamei
        End Get
        Set(ByVal Value As String)
            InfoReport.Itakusyamei = Value
        End Set
    End Property

    ' ���t�v���p�e�B
    Public Property Hiduke() As String
        Get
            Return InfoReport.Hiduke
        End Get
        Set(ByVal Value As String)
            InfoReport.Hiduke = Value
        End Set
    End Property

    ' �z�X�g�ۑ��p�b�r�u�t�@�C��
    Public Property HostCsvName() As String
        Get
            Return InfoReport.HostCsvName
        End Get
        Set(ByVal Value As String)
            InfoReport.HostCsvName = Value
        End Set
    End Property

    ' �n�q�`�b�k�d�N���X
    Protected MainDB As MyOracle
    Public WriteOnly Property OraDB() As MyOracle
        Set(ByVal Value As CASTCommon.MyOracle)
            MainDB = Value
        End Set
    End Property

    ' �v�����^��
    Private Shared mPrinter(4) As String

    ' �v�����^���̎擾
    Private Shared Sub GetPrinterName()
        For i As Integer = 0 To 4
            mPrinter(i) = GetFSKJIni("COMMON", "PRINTER_" & (i + 1).ToString)
            If mPrinter(i) = "err" Then
                mPrinter(i) = ""
            End If
        Next i
    End Sub

    Public Enum PrinterType As Integer
        Printer3F = 0
        Printer5F = 1
    End Enum

    ' �v�����^���̎擾
    ' PRINTER_1 �` PRINTER_5
    ' nIndex 1 �` 5 ���w�肷��
    Public Shared ReadOnly Property PrinterName(ByVal prnType As PrinterType) As String
        Get
            Try
                If mPrinter(0) Is Nothing Then
                    Call ClsReportBase.GetPrinterName()
                End If
                Return ClsReportBase.mPrinter(prnType)
            Catch ex As Exception
                Return ""
            End Try
        End Get
    End Property

    '
    ' �@�\�@ �F ����i�v�����^�w��j
    '
    ' �����@ �F ARG1 - �v�����^��
    '
    ' �߂�l �F True-�����CFalse-���s
    '
    ' ���l�@ �F 
    '
    Public Shared Function DetectPrinter(ByVal printerName As String) As Boolean
        '�v�����^�����擾����
        Try
            Dim pinfo As PRINTER_INFO_2 = GetPrinterInfo(printerName)
            If GetString(pinfo.pDriverName).Trim = "" Then
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    ' New
    Sub New()
        ' �v�����^���̎擾
        Call GetPrinterName()
    End Sub

    '
    ' �@�\�@ �F �b�r�u�t�@�C���쐬
    '
    ' �߂�l �F �b�r�u�t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Overridable Function CreateCsvFile() As String
        ' �t�@�C��������
        Dim sWork As String = Path.Combine(CsvPath, InfoReport.CsvName & "{0}.csv")
        For nKaiji As Integer = 1 To Integer.MaxValue - 1
            Dim fl As New FileInfo(String.Format(sWork, nKaiji))
            If fl.Exists() = False Then
                Try
                    Dim fs As FileStream = fl.Create()
                    ' �t�@�C���쐬�����������̂Ŕ�����
                    CsvName = fl.FullName
                    fs.Close()
                    Exit For
                Catch ex As Exception
                    Exit For
                End Try
            End If
        Next nKaiji

        Call OpenCsv()

        Return CsvName
    End Function

    '
    ' �@�\�@ �F �������X�g���[���쐬
    '
    ' ���l�@ �F 
    '
    Public Overridable Sub CreateMemFile()
        MEMObject = New MEM
        MEMObject.Open()
    End Sub

    '
    ' �@�\�@ �F ������s
    '
    ' ���l�@ �F 
    '
    Public Overridable Overloads Function ReportExecute() As Boolean
        Return ReportExecute("")
    End Function

    '
    ' �@�\�@ �F ������s
    '
    ' ���l�@ �F 
    '
    Public Overridable Overloads Function ReportExecute(ByVal prnType As PrinterType) As Boolean
        Return ReportExecute(PrinterName(prnType))
    End Function

    '
    ' �@�\�@ �F ������s
    '
    ' ���l�@ �F 
    '
    Public Overridable Overloads Function ReportExecute(ByVal printerName As String) As Boolean
        Dim repo As RAX
        Try
            Call CloseCsv()

            repo = New RAX(ReportBaseName)
            repo.CsvName = CsvName
            repo.Copies = Copies
            repo.DataCode = DataCode

            ' 2016/06/08 �^�X�N�j���� CHG �yRD�zUI_B-14-99(RSV2�Ή�) -------------------- START
            '' ������s
            'repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
            'If Directory.Exists(repo.LogPath) = False Then
            '    Directory.CreateDirectory(repo.LogPath)
            'End If
            'repo.LogName = repo.ReportName & ".LOG"
            'If printerName Is Nothing OrElse printerName.Trim = "" Then
            '    repo.PrintOut()
            'Else
            '    repo.PrintOut(printerName)
            'End If

            'ReportMessage = repo.Message
            'If Not ReportMessage Is Nothing And ReportMessage <> "" Then
            '    Return False
            'End If
            Dim GetIniReturn As String = String.Empty
            Dim PrinterSetNum As Integer = -1

            If printerName = "" Then
                GetIniReturn = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "SET_NUM")
                Select Case GetIniReturn
                    Case "err", ""
                        PrinterSetNum = -1
                    Case Else
                        PrinterSetNum = CInt(Trim(GetIniReturn))
                End Select
            End If

            Select Case PrinterSetNum
                Case -1
                    '================================================================
                    ' ������s(RSV1����)
                    '================================================================
                    repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
                    If Directory.Exists(repo.LogPath) = False Then
                        Directory.CreateDirectory(repo.LogPath)
                    End If
                    repo.LogName = repo.ReportName & ".LOG"
                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                        repo.PrintOut()
                    Else
                        repo.PrintOut(printerName)
                    End If

                    ReportMessage = repo.Message
                    If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                        Return False
                    End If
                Case Else
                    '================================================================
                    ' ������s(RSV2����)
                    '================================================================
                    For i As Integer = 1 To PrinterSetNum Step 1
                        printerName = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "PRINTER" & i.ToString)
                        Dim PrnIni_PrintBusuu As Integer = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "BUSUU" & i.ToString)
                        Dim PrnIni_SpoolName As String = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "SPOOL" & i.ToString)

                        ' 2017/04/17 �^�X�N�j���� CHG �yME�zUI_99-99(RSV2�Ή� �@�\�g��) -------------------- START
                        Dim PrnIni_ReportBaseName As String = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "RPDNAME" & i.ToString)
                        Select Case PrnIni_ReportBaseName
                            Case "err", ""
                            Case Else
                                repo = New RAX(PrnIni_ReportBaseName)
                                repo.CsvName = CsvName
                                repo.Copies = Copies
                                repo.DataCode = DataCode
                        End Select
                        ' 2017/04/17 �^�X�N�j���� CHG �yME�zUI_99-99(RSV2�Ή� �@�\�g��) -------------------- END

                        For j As Integer = 1 To PrnIni_PrintBusuu Step 1
                            repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
                            If Directory.Exists(repo.LogPath) = False Then
                                Directory.CreateDirectory(repo.LogPath)
                            End If
                            repo.LogName = repo.ReportName & ".LOG"
                            Select Case PrnIni_SpoolName
                                Case ""
                                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                                        repo.PrintOut()
                                    Else
                                        repo.PrintOut(printerName)
                                    End If
                                Case Else
                                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                                        repo.PrintOut("", PrnIni_SpoolName)
                                    Else
                                        repo.PrintOut(printerName, PrnIni_SpoolName)
                                    End If
                            End Select

                            ReportMessage = repo.Message
                            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                                Return False
                            End If
                        Next
                    Next
            End Select
            ' 2016/06/08 �^�X�N�j���� CHG �yRD�zUI_B-14-99(RSV2�Ή�) -------------------- END

        Catch ex As Exception
            ReportMessage = ex.ToString    '2010/02/25 �G���[����ԋp
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function

    '
    ' �@�\�@ �F ������s(�w�Z�p)
    '
    ' ���l�@ �F �w�Z�p����֐�(2010/02/25)
    '
    Public Overridable Overloads Function G_ReportExecute(ByVal printerName As String) As Boolean
        Dim repo As CAstReports.RAX
        Try
            Call CloseCsv()

            repo = New CAstReports.RAX(CASTCommon.GetFSKJIni("GCOMMON", "LST"), ReportBaseName)
            repo.CsvName = CsvName
            repo.Copies = Copies
            repo.DataCode = DataCode

            ' 2016/06/08 �^�X�N�j���� CHG �yRD�zUI_B-14-99(RSV2�Ή�) -------------------- START
            '' ������s
            'repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
            'If Directory.Exists(repo.LogPath) = False Then
            '    Directory.CreateDirectory(repo.LogPath)
            'End If
            'repo.LogName = repo.ReportName & ".LOG"
            'If printerName Is Nothing OrElse printerName.Trim = "" Then
            '    repo.PrintOut()
            'Else
            '    repo.PrintOut(printerName)
            'End If

            'ReportMessage = repo.Message
            'If Not ReportMessage Is Nothing And ReportMessage <> "" Then
            '    Return False
            'End If
            Dim GetIniReturn As String = String.Empty
            Dim PrinterSetNum As Integer = -1

            If printerName = "" Then
                GetIniReturn = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "SET_NUM")
                Select Case GetIniReturn
                    Case "err", ""
                        PrinterSetNum = -1
                    Case Else
                        PrinterSetNum = CInt(Trim(GetIniReturn))
                End Select
            End If

            Select Case PrinterSetNum
                Case -1
                    '================================================================
                    ' ������s(RSV1����)
                    '================================================================
                    repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
                    If Directory.Exists(repo.LogPath) = False Then
                        Directory.CreateDirectory(repo.LogPath)
                    End If
                    repo.LogName = repo.ReportName & ".LOG"
                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                        repo.PrintOut()
                    Else
                        repo.PrintOut(printerName)
                    End If

                    ReportMessage = repo.Message
                    If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                        Return False
                    End If
                Case Else
                    '================================================================
                    ' ������s(RSV2����)
                    '================================================================
                    For i As Integer = 1 To PrinterSetNum Step 1
                        printerName = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "PRINTER" & i.ToString)
                        Dim PrnIni_PrintBusuu As Integer = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "BUSUU" & i.ToString)
                        Dim PrnIni_SpoolName As String = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "SPOOL" & i.ToString)

                        ' 2017/04/17 �^�X�N�j���� CHG �yME�zUI_99-99(RSV2�Ή� �@�\�g��) -------------------- START
                        Dim PrnIni_ReportBaseName As String = CASTCommon.GetIni("PRINT_INFO.INI", Path.GetFileNameWithoutExtension(ReportBaseName), "RPDNAME" & i.ToString)
                        Select Case PrnIni_ReportBaseName
                            Case "err", ""
                            Case Else
                                repo = New CAstReports.RAX(CASTCommon.GetFSKJIni("GCOMMON", "LST"), PrnIni_ReportBaseName)
                                repo.CsvName = CsvName
                                repo.Copies = Copies
                                repo.DataCode = DataCode
                        End Select
                        ' 2017/04/17 �^�X�N�j���� CHG �yME�zUI_99-99(RSV2�Ή� �@�\�g��) -------------------- END

                        For j As Integer = 1 To PrnIni_PrintBusuu Step 1
                            repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
                            If Directory.Exists(repo.LogPath) = False Then
                                Directory.CreateDirectory(repo.LogPath)
                            End If
                            repo.LogName = repo.ReportName & ".LOG"
                            Select Case PrnIni_SpoolName
                                Case ""
                                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                                        repo.PrintOut()
                                    Else
                                        repo.PrintOut(printerName)
                                    End If
                                Case Else
                                    If printerName Is Nothing OrElse printerName.Trim = "" Then
                                        repo.PrintOut("", PrnIni_SpoolName)
                                    Else
                                        repo.PrintOut(printerName, PrnIni_SpoolName)
                                    End If
                            End Select

                            ReportMessage = repo.Message
                            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                                Return False
                            End If
                        Next
                    Next
            End Select
            ' 2016/06/08 �^�X�N�j���� CHG �yRD�zUI_B-14-99(RSV2�Ή�) -------------------- END

        Catch ex As Exception
            ReportMessage = ex.ToString
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function

    '
    ' �@�\�@ �F ������s(WEB�`���p)
    '
    ' ���l�@ �F WEB�`���Ή�(2012/06/30) ���W����
    '
    Public Overridable Overloads Function ReportExecute(ByVal printerName As String, ByVal USER_ID As String, ByVal ITAKU_CODE As String) As Boolean
        Dim repo As RAX
        
        Try
            Call CloseCsv()

            repo = New RAX(ReportBaseName)
            repo.CsvName = CsvName
            repo.Copies = Copies
            repo.DataCode = DataCode

            ' ������s
            repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
            If Directory.Exists(repo.LogPath) = False Then
                Directory.CreateDirectory(repo.LogPath)
            End If
            repo.LogName = repo.ReportName & ".LOG"
            If printerName Is Nothing OrElse printerName.Trim = "" Then
                repo.PrintOut()
            Else
                repo.PrintOut(printerName, USER_ID, ITAKU_CODE)
            End If

            ReportMessage = repo.Message
            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                Return False
            End If

        Catch ex As Exception
            ReportMessage = ex.ToString    '2010/02/25 �G���[����ԋp
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function

    '2017/04/25 �^�X�N�j���� ADD �W���ŏC���i�w�ZWEB�`���Ή��j------------------------------------ START
    '
    ' �@�\�@ �F ������s
    '
    ' ���l�@ �F 2014/07/02 �ޗǐM���@WEB�`���Ή�
    '
    Public Overridable Overloads Function G_ReportExecute(ByVal printerName As String, ByVal USER_ID As String, ByVal ITAKU_CODE As String) As Boolean
        Dim repo As RAX

        Try
            Call CloseCsv()

            repo = New CAstReports.RAX(CASTCommon.GetFSKJIni("GCOMMON", "LST"), ReportBaseName)
            repo.CsvName = CsvName
            repo.Copies = Copies
            repo.DataCode = DataCode

            ' ������s
            repo.LogPath = Path.Combine(repo.ReportPath, "LOG")
            If Directory.Exists(repo.LogPath) = False Then
                Directory.CreateDirectory(repo.LogPath)
            End If
            repo.LogName = repo.ReportName & ".LOG"
            If printerName Is Nothing OrElse printerName.Trim = "" Then
                repo.PrintOut()
            Else
                repo.PrintOut(printerName, USER_ID, ITAKU_CODE)
            End If

            ReportMessage = repo.Message
            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                Return False
            End If

        Catch ex As Exception
            ReportMessage = ex.ToString    '2010/02/25 �G���[����ԋp
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function
    '2017/04/25 �^�X�N�j���� ADD �W���ŏC���i�w�ZWEB�`���Ή��j------------------------------------ END

    '*** Str Add 2015/12/04 SO)���� for �g������Ή� ***

    ' �@�\   �F �w�肳�ꂽ�v�����^���z��̏��ԂŊg��������s��
    ' ����   �F repo  RepoAgent
    '           prtDspName  ����� �i��F ���[ID_���[���j
    '           printerArray  �v�����^���z��i�ʏ�g���v�����^�݂̂̏ꍇ��Nothing�A�z�񒆂̒ʏ�g���v�����^���͋󕶎��j
    ' �߂�l :  True-����, False-���s
    '
    Public Function Extend_ReportExecute(ByVal repo As RAX, ByVal prtDspName As String, _
                                         ByVal printerArray As String()) As Boolean

        Dim prtRet As Boolean = False

        Dim LOG As CASTCommon.BatchLOG = New CASTCommon.BatchLOG("CAstReports", "ClsReportBase")

        ' �����J�n���O�o��
        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("ClsReportBase.Extend_ReportExecute")

        Try
            ' ������s
            If printerArray Is Nothing Then
                ' �ʏ�g���v�����^�݂̂̏ꍇ
                printerArray = New String(0) {}
                printerArray(0) = ""
            End If

            For printerNo As Integer = 0 To printerArray.Length - 1
                prtRet = repo.ExtendPrintOut(printerArray(printerNo), prtDspName)

               If prtRet = False Then
                   ReportMessage = repo.Message
                   Return prtRet
               End If
            Next

            Return prtRet

        Catch ex As Exception
            ReportMessage = ex.ToString
            Return prtRet

        Finally
            ' �����I�����O�o��
            LOG.Write_Exit3(sw, "ClsReportBase.Extend_ReportExecute", "���A�l=" & prtRet)
        End Try

    End Function

    '*** End Add 2015/12/04 SO)���� for �g������Ή� ***

    '===================================================================

    '
    ' �@�\�@ �F �v���r���[���s
    '
    ' ���l�@ �F 
    '
    Public Function PreviewExecute() As Boolean
        Dim repo As RAX
        Try
            Call CloseCsv()

            repo = New RAX(ReportBaseName)
            repo.CsvName = CsvName

            ' ������s
            repo.LogPath = repo.ReportPath
            repo.LogName = repo.ReportName & ".LOG"
            repo.Preview()

            ReportMessage = repo.Message
            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function

    '2018/03/09 saitou �L���M��(RSV2�W��) ADD �w�Z�p�v���r���[��� -------------------- START
    Public Function G_PreviewExecute() As Boolean
        Dim repo As RAX
        Try
            Call CloseCsv()

            repo = New CAstReports.RAX(CASTCommon.GetFSKJIni("GCOMMON", "LST"), ReportBaseName)
            repo.CsvName = CsvName

            ' ������s
            repo.LogPath = repo.ReportPath
            repo.LogName = repo.ReportName & ".LOG"
            repo.Preview()

            ReportMessage = repo.Message
            If Not ReportMessage Is Nothing And ReportMessage <> "" Then
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            repo = Nothing
        End Try

        Return True
    End Function
    '2018/03/09 saitou �L���M��(RSV2�W��) ADD ----------------------------------------- END

    '
    ' �@�\�@ �F �b�r�u�t�@�C�����J��
    '
    ' ���l�@ �F 
    '
    Public Sub OpenCsv()
        CSVObject = New CSV
        CSVObject.Open(CsvName, True)
    End Sub

    '
    ' �@�\�@ �F �b�r�u�t�@�C�������
    '
    ' ���l�@ �F 
    '
    Public Sub CloseCsv()
        If Not CSVObject Is Nothing Then
            CSVObject.Close()
        End If
        CSVObject = Nothing
    End Sub

    '�@�\   :   �\�[�g���s
    '
    '���l   :   
    '
    Public Sub SortFile(ByVal keyItem As String, Optional ByVal SkipRec As String = "")
        ' �z��ɓǍ�
        Dim arraySort As New ArrayList(2048)        ' �\�[�g�p�z��
        Try
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START
            Dim PowerSort As New clsPowerSORT
            PowerSort.DisposalNumber = 0        ' �\�[�g�@�\
            PowerSort.FieldDefinition = 0       ' �����t�B�[���h
            PowerSort.FieldDelimiter = ","      ' �t�B�[���h��������
            PowerSort.KeyCmdStr = keyItem       ' �\�[�g�L�[
            If SkipRec <> "" Then
                PowerSort.InputFilesSkiprec = SkipRec
            End If
            PowerSort.InputFiles = CsvName      ' �t�@�C����
            PowerSort.InputFileType = 0         ' �e�L�X�g�t�@�C��
            PowerSort.OutputFile = CsvName & ".sort.csv"
            PowerSort.OutputFileType = 0
            PowerSort.MaxRecordLength = 1000
            PowerSort.Action()

            If PowerSort.ErrorCode <> 0 Then
                Return
            End If

            'Dim FrmSort As New FrmPowerSort
            'Dim PowerSort As AxPowerSORT_Lib.AxPowerSORT = FrmSort.objAxPowerSORT
            'PowerSort.DispMessage = False
            'PowerSort.DisposalNumber = 0        ' �\�[�g�@�\
            'PowerSort.FieldDefinition = 0       ' �����t�B�[���h
            'PowerSort.FieldDelimiter = "','"    ' �t�B�[���h��������
            'PowerSort.KeyCmdStr = keyItem       ' "0.9sjia"     ' �\�[�g�L�[
            'If SkipRec <> "" Then
            '    PowerSort.InputFilesSkiprec = SkipRec
            'End If
            'PowerSort.InputFiles = CsvName      ' �t�@�C����
            'PowerSort.InputFileType = 0         ' �e�L�X�g�t�@�C��
            'PowerSort.OutputFile = CsvName & ".sort.csv"
            'PowerSort.OutputFileType = 0
            'PowerSort.MaxRecordLength = 1000
            'PowerSort.Action()

            'If PowerSort.ErrorCode <> 0 Then
            '    Return
            'End If
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------------------ END

            If File.Exists(CsvName) = True Then
                File.Delete(CsvName)
            End If
            File.Move(PowerSort.OutputFile, CsvName)
        Catch ex As Exception
            Return
        End Try

    End Sub

    '�d�q���[���̂��ߒǉ�
    Private Function CreateFTPFile(ByVal linedata() As String) As Boolean
        '==========================================================================
        '�T�v-------FTP���M�̂��߂�CSV�t�@�C�������t�@�C���E�_�~�[�t�@�C�����쐬��
        '           FTP���M���s��
        '����-------FTP_Data TXT��1�s�f�[�^
        '�߂�l-----���s����
        '==========================================================================
        Dim FTPFileName As String = linedata(3)
        Dim FTPFileNameFull As String
        Dim WorkDir As String = Path.Combine(CsvPath, "WrkFTP" & Process.GetCurrentProcess.Id)

        Try
            '�t�@�C�����ݒ�
            FTPFileName = FTPFileName.Replace("YYYYMMDDHHMMSS", Now.ToString("yyyyMMddHHmmss"))
            FTPFileName = FTPFileName.Replace("99999", Process.GetCurrentProcess.Id.ToString("00000"))
            FTPFileNameFull = Path.Combine(WorkDir, FTPFileName)

            '�t�@�C�������[�N�t�H���_�ɃR�s�[
            Directory.CreateDirectory(WorkDir)
            File.Copy(CsvName, FTPFileNameFull, True)

            '����t�@�C���̍쐬
            Dim sw As StreamWriter = New StreamWriter(FTPFileNameFull & ".flw", False, Encoding.GetEncoding(932))
            sw.WriteLine("fm-printerentry = ReportViewer")              '�Œ�
            sw.WriteLine("fm-printer = " & FTPFileName.Split("@"c)(0))  'RV�o�^�̃z�X�g�]���t�@�C����(FTP�t�@�C������@����O��ݒ肷��)
            sw.WriteLine("fm-formentry = dummy")                        '�Œ�
            sw.WriteLine("fm-formfilename = " & linedata(8))            'FMEditor�ō쐬�����t�H�[����
            sw.Close()

            '�_�~�[�t�@�C���쐬
            Dim fs As FileStream = File.Create(FTPFileNameFull & ".dmy")
            fs.Close()

            'FTP���M
            If FTPExecute(FTPFileNameFull, linedata) = False Then
                Return False
            End If

            '����I������PRT/FTPBK�t�H���_�Ƀt�@�C���ړ�
            Dim FtpDir As String = Path.Combine(CsvPath, "FTPBK")
            If Directory.Exists(FtpDir) = False Then
                Directory.CreateDirectory(FtpDir)
            End If

            File.Move(FTPFileNameFull, Path.Combine(FtpDir, FTPFileName))
            File.Move(FTPFileNameFull & ".flw", Path.Combine(FtpDir, FTPFileName) & ".flw")
            File.Move(FTPFileNameFull & ".dmy", Path.Combine(FtpDir, FTPFileName) & ".dmy")

        Catch ex As Exception
            ReportMessage = "FTP���M�t�@�C���̍쐬�Ɏ��s���܂����B"
            Return False
        Finally
            Try
                Directory.Delete(WorkDir, True)
            Catch ex As Exception
            End Try
        End Try

        Return True
    End Function

    Private Function FTPExecute(ByVal FTPFileName As String, ByVal linedata() As String) As Boolean
        '==========================================================================
        '�T�v-------FTP���M���s��
        '����-------FTP_FileName �t�@�C����,FTP_Data() TXT��1�s�f�[�^
        '�߂�l-----���s����
        '==========================================================================

        Dim FTPHOST As String = linedata(6)
        Dim FTPPATH As String = linedata(7)

        Dim strUSERID As String = linedata(4)
        Dim strPASSWORD As String = linedata(5)

        Try
            Dim WorkDir As String = Path.GetDirectoryName(FTPFileName)
            Dim FTPText As String = Path.Combine(WorkDir, "ftpcmd.txt")

            'FTP.EXE�ɑ���R�}���h���e�L�X�g�o�͂���
            Dim sw As StreamWriter = New StreamWriter(FTPText, False, Encoding.GetEncoding(932))
            sw.AutoFlush = False
            sw.WriteLine("OPEN " & FTPHOST)
            sw.WriteLine(strUSERID)
            sw.WriteLine(strPASSWORD)
            sw.WriteLine("BINARY")
            If Not FTPPATH = "" Then
                sw.WriteLine("CD " & FTPPATH)
            End If
            sw.WriteLine("PUT " & Path.GetFileName(FTPFileName))
            sw.WriteLine("PUT " & Path.GetFileName(FTPFileName) & ".flw")
            sw.WriteLine("PUT " & Path.GetFileName(FTPFileName) & ".dmy")
            sw.WriteLine("QUIT")
            sw.Close()

            Dim psInfo As New ProcessStartInfo
            psInfo.FileName = "ftp"
            psInfo.Arguments = "-s:" & FTPText
            psInfo.WorkingDirectory = WorkDir
            psInfo.UseShellExecute = False
            psInfo.CreateNoWindow = True '2010/05/18�ǉ�

            'FTP���s
            Dim p As Process = Process.Start(psInfo)
            p.WaitForExit()

            If Not p.ExitCode = 0 Then
                ReportMessage = "�e�s�o�������s�F���^�[���R�[�h" & p.ExitCode
                Return False
            End If

        Catch ex As Exception
            ReportMessage = "�e�s�o�������s�F" & ex.Message
            Return False
        End Try

        Return True
    End Function

End Class
'�t�@�C���\�[�g�N���X
Public Class ClsFileSort
    Implements IComparer

    Private mKey(,) As Integer

    Sub New(ByVal keyitem(,) As Integer)
        mKey = keyitem
    End Sub

    '-------------------------------------------------------------------
    '�@�\   :   ��r�p�֐�
    '����   :   x ��r�Ώۂ̑� 1 �I�u�W�F�N�g
    '           y ��r�Ώۂ̑� 2 �I�u�W�F�N�g
    '�߂�l :   Integer 0 ��菬�����l x �� y ��菬�����B 
    '                   0 x �� y �͓������B 
    '                   0 ���傫���l x �� y ���傫���B 
    '���l   :   
    '-------------------------------------------------------------------
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
        Implements IComparer.Compare

        Dim nRet As Integer = 0
        '���[�N�z��
        Dim DataX As String
        Dim DataY As String

        '�����ɕϊ�
        For i As Integer = 0 To CType((mKey.Length / 2), Integer) - 1
            DataX = CType(x, String).Substring(mKey(i, 0), mKey(i, 1))
            DataY = CType(y, String).Substring(mKey(i, 0), mKey(i, 1))

            nRet = New CaseInsensitiveComparer().Compare(DataX, DataY)
            If nRet <> 0 Then
                Exit For
            End If
        Next i

        Return nRet
    End Function
End Class

'2017/12/11 saitou �L���M��(RSV2�W��) ADD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START

''' <summary>
''' �p���[�\�[�g�N���X
''' </summary>
''' <remarks>2017/12/11 saitou �L���M��(RSV2�W��) added for �T�[�o�[����Ή�</remarks>
Public Class clsPowerSORT
    Protected iDisposalNumber As Integer
    Protected iFieldDefinition As Integer
    Protected mFieldDelimiter As String
    Protected mKeyCmdStr As String
    Protected mInputFilesSkiprec As String
    Protected mSelCmdStr As String
    Protected mInputFiles As String
    Protected iInputFileType As Integer
    Protected mOutputFile As String
    Protected iOutputFileType As Integer
    Protected iMaxRecordLength As Integer
    Protected iErrorCode As Integer
    Protected mErrorDetail As String

    ''' <summary>
    ''' �R���X�g���N�^
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        iDisposalNumber = 0
        iFieldDefinition = 0
        mFieldDelimiter = ""
        mKeyCmdStr = ""
        mInputFilesSkiprec = ""
        mSelCmdStr = ""
        mInputFiles = ""
        iInputFileType = 0
        mOutputFile = ""
        iOutputFileType = 0
        iMaxRecordLength = 0
        iErrorCode = 0
        mErrorDetail = ""
    End Sub

#Region "�v���p�e�B"

    Public Property DisposalNumber As Integer
        Get
            Return iDisposalNumber
        End Get
        Set(value As Integer)
            iDisposalNumber = value
        End Set
    End Property

    Public Property FieldDefinition As Integer
        Get
            Return iFieldDefinition
        End Get
        Set(value As Integer)
            iFieldDefinition = value
        End Set
    End Property

    Public Property FieldDelimiter As String
        Get
            Return mFieldDelimiter
        End Get
        Set(value As String)
            mFieldDelimiter = value
        End Set
    End Property

    Public Property KeyCmdStr As String
        Get
            Return mKeyCmdStr
        End Get
        Set(value As String)
            mKeyCmdStr = value
        End Set
    End Property

    Public Property InputFilesSkiprec As String
        Get
            Return mInputFilesSkiprec
        End Get
        Set(value As String)
            mInputFilesSkiprec = value
        End Set
    End Property

    Public Property SelCmdStr As String
        Get
            Return mSelCmdStr
        End Get
        Set(value As String)
            mSelCmdStr = value
        End Set
    End Property

    Public Property InputFiles As String
        Get
            Return mInputFiles
        End Get
        Set(value As String)
            mInputFiles = value
        End Set
    End Property

    Public Property InputFileType As Integer
        Get
            Return iInputFileType
        End Get
        Set(value As Integer)
            iInputFileType = value
        End Set
    End Property

    Public Property OutputFile As String
        Get
            Return mOutputFile
        End Get
        Set(value As String)
            mOutputFile = value
        End Set
    End Property

    Public Property OutputFileType As Integer
        Get
            Return iOutputFileType
        End Get
        Set(value As Integer)
            iOutputFileType = value
        End Set
    End Property

    Public Property MaxRecordLength As Integer
        Get
            Return iMaxRecordLength
        End Get
        Set(value As Integer)
            iMaxRecordLength = value
        End Set
    End Property

    Public Property ErrorCode As Integer
        Get
            Return iErrorCode
        End Get
        Set(value As Integer)
            iErrorCode = value
        End Set
    End Property

    Public Property ErrorDetail As String
        Get
            Return mErrorDetail
        End Get
        Set(value As String)
            mErrorDetail = value
        End Set
    End Property

#End Region

#Region "���\�b�h"

    Public Sub Action()
        Try
            Dim ProcSort As Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(GetRSKJIni("RSV2_V1.0.0", "POWERSORT_PATH"), "bsort.exe")
            If File.Exists(ProcInfo.FileName) = False Then
                ' �p���[�\�[�g���Ȃ��̂ŁA�\�[�g�����ɏ����𑱍s����
                Return
            End If

            '���o�̓t�@�C�����`�F�b�N
            If InputFiles.Trim = "" OrElse OutputFile.Trim = "" Then
                Return
            End If

            '�����敪
            Dim SyoriKbn As String = String.Empty
            Select Case DisposalNumber
                Case 0 : SyoriKbn = "-s"    '�\�[�g
                Case 1 : SyoriKbn = "-m"    '�}�[�W
                Case 2 : SyoriKbn = "-c"    '�R�s�[
                Case Else : Return
            End Select

            '�e�L�X�g�t�@�C���I�v�V����
            Dim TxtOpt As String = String.Empty
            Select Case FieldDefinition
                Case 0 : TxtOpt = "-Tflt"   '����
                Case 1 : TxtOpt = "-Tfix"   '�Œ�
                Case Else : Return
            End Select

            '�t�B�[���h����������
            Dim FieldSep As String = String.Empty
            If FieldDefinition = 0 AndAlso FieldDelimiter.Trim <> "" Then
                FieldSep = "-t " & FieldDelimiter
            End If

            '�ő僌�R�[�h�T�C�Y
            Dim RecSize As String = String.Empty
            If MaxRecordLength <> 0 Then
                RecSize = "-z" & MaxRecordLength.ToString
            End If

            '�X�L�b�v���R�[�h�ԍ�
            Dim SkipRec As String = String.Empty
            If InputFilesSkiprec.Trim <> "" Then
                SkipRec = "-R " & InputFilesSkiprec
            End If

            '�I���t�B�[���h�I�v�V����
            Dim SelCmd As String = String.Empty
            If SelCmdStr.Trim <> "" Then
                SelCmd = "-p " & SelCmdStr
            End If

            ProcInfo.Arguments = String.Format("{0} {1} -""{2}"" {3} {4} {5} {6} -o {7} {8}", SyoriKbn, RecSize, KeyCmdStr, TxtOpt, FieldSep, SkipRec, SelCmd, OutputFile, InputFiles)

            ProcInfo.WorkingDirectory = Path.GetDirectoryName(ProcInfo.FileName)
            ProcInfo.UseShellExecute = False
            ProcInfo.RedirectStandardOutput = True
            ProcSort = Process.Start(ProcInfo)
            ProcSort.WaitForExit()

            '�G���[�R�[�h�ݒ�
            ErrorCode = ProcSort.ExitCode

            If ProcSort.ExitCode <> 0 Then
                ErrorDetail = ProcSort.StandardOutput.ReadToEnd()
                Return
            End If

        Catch ex As Exception
            Return
        End Try
    End Sub

#End Region

End Class
'2017/12/11 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------------------ END
