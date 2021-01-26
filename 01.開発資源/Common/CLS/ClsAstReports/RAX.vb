Imports System.IO

' RepoAgent����N���X�iActiveX�o�[�W�����j
Public Class RAX
    'Inherits CR
    Protected mReportPath As String                 ' ���|�[�g�p�X
    Protected mReportName As String                 ' ���|�[�g��
    Protected mRecordSelectionFormula As String     ' ���|�[�g�I����
    Protected mErrorMessage As String               ' �G���[���
    Protected mFormulaFieldDefinition As String     ' �t�B�[���h�l

    Protected mCsvPath As String                    ' �b�r�u�o�̓p�X
    Protected mCsvName As String                    ' �b�r�u��


    Protected mLogPath As String                    ' ���O�o�̓p�X
    Protected mLogName As String                    ' ���O�t�@�C����

    '2018/02/15 saitou �L���M��(RSV2�W��) ADD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START
    '�����ŃC���X�^���X�̐����͍s��Ȃ��B
    Private raReport As raocxlib.RepoAgent
    'Private raReport As New raocxlib.RepoAgent
    '2018/02/15 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------------------ END

    Public Copies As Integer = 1                    '�������
    Public DataCode As Integer = 0                  '�����R�[�h

    '2017/12/11 saitou �L���M��(RSV2�W��) ADD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START
    Private raReport64 As RA64
    Private FLG64 As String = "0"
    '2017/12/11 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------------------ END

    Public Sub New()
        '���|�[�g�p�X�f�t�H���g�w��
        mReportPath = Path.GetDirectoryName(ReportName)
        If mReportPath = "" Then
            CASTCommon.GetFSKJIni("COMMON", "LST", mReportPath)
            mReportPath = Path.GetDirectoryName(mReportPath)
        End If

        mReportName = ""
        mRecordSelectionFormula = ""
        mErrorMessage = ""
        mCsvPath = ""
        mCsvName = ""
    End Sub

    ' reportname : ���|�[�g�� �t���p�X
    Public Sub New(ByVal reportname As String)

        MyClass.New()

        mReportName = Path.GetFileName(reportname)

        Call SetupReport()

    End Sub

    ' reportpath : �p�X��
    ' reportname : ���|�[�g��
    Public Sub New(ByVal reportpath As String, ByVal reportname As String)

        MyClass.New()

        mReportPath = reportpath
        mReportName = reportname

        Call SetupReport()

    End Sub

    ' ���|�[�g�p�X
    Public Property ReportPath() As String
        Get
            Return mReportPath
        End Get
        Set(ByVal Value As String)
            mReportPath = Value
        End Set
    End Property

    ' ���|�[�g��
    Public Overridable Property ReportName() As String
        Get
            Return mReportName
        End Get
        Set(ByVal Value As String)
            If mReportPath = "" Then
                mReportPath = Path.GetDirectoryName(Value)
            End If
            mReportName = Path.GetFileName(Value)

            If File.Exists(Path.Combine(mReportPath, mReportName)) = True Then
                Call SetupReport()
            End If
        End Set
    End Property

    ' CSV�p�X
    Public Property CsvPath() As String
        Get
            Return mCsvPath
        End Get
        Set(ByVal Value As String)
            mCsvPath = Value
        End Set
    End Property

    ' CSV��
    Public Property CsvName() As String
        Get
            Return mCsvName
        End Get
        Set(ByVal Value As String)
            If mCsvPath = "" Then
                mCsvPath = Path.GetDirectoryName(Value)
            End If
            mCsvName = Path.GetFileName(Value)
        End Set
    End Property

    '
    ' �v���p�e�B�F ���b�Z�[�W
    '
    Public Property Message() As String
        Get
            Return mErrorMessage
        End Get
        Set(ByVal Value As String)
            mErrorMessage = Value
        End Set
    End Property


    ' LOG�p�X
    Public Property LogPath() As String
        Get
            Return mLogPath
        End Get
        Set(ByVal Value As String)
            mLogPath = Value
        End Set
    End Property

    ' LOG��
    Public Property LogName() As String
        Get
            Return mLogName
        End Get
        Set(ByVal Value As String)
            If mLogPath = "" Then
                mLogPath = Path.GetDirectoryName(Value)
            End If
            mLogName = Path.GetFileName(Value)
        End Set
    End Property

    '
    ' �@�\�@ �F ���
    '
    ' �����@ �F ARG1 - 
    '
    ' �߂�l �F True-�����CFalse-���s
    '
    ' ���l�@ �F 
    '
    Public Overloads Function PrintOut() As Boolean
        Dim ret As Boolean
        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "�b�r�u�t�@�C����������܂���B" & Path.Combine(mCsvPath, mCsvName)
                Return False
            End If

            ' ���|�[�g�ݒ�
            Call SetupReport()

            ' ���
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START
            If FLG64 = "1" Then
                raReport64.ReportName = Path.GetFileNameWithoutExtension(raReport64.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")
                ret = raReport64.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If
            Else
                raReport.ReportName = Path.GetFileNameWithoutExtension(raReport.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")
                ret = raReport.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If
            End If
            'raReport.ReportName = Path.GetFileNameWithoutExtension(raReport.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")
            'ret = raReport.ReportPrint()
            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------------------ END
        Catch ex As Exception
            mErrorMessage = ex.Message
            Return False
        End Try
        Return ret
    End Function

    '
    ' �@�\�@ �F ���
    '
    ' �����@ �F ARG1 - �v�����^��
    '
    ' �߂�l �F True-�����CFalse-���s
    '
    ' ���l�@ �F 
    '
    Public Overloads Function PrintOut(ByVal printername As String) As Boolean
        Dim ret As Boolean
        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "�b�r�u�t�@�C����������܂���B" & Path.Combine(mCsvPath, mCsvName)
                Return False
            End If

            ' ���|�[�g�ݒ�
            Call SetupReport()

            ' ���
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START
            If FLG64 = "1" Then
                raReport64.PrtName = printername

                raReport64.ReportName = Path.GetFileNameWithoutExtension(raReport64.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")

                ret = raReport64.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If
            Else
                raReport.PrtName = printername

                raReport.ReportName = Path.GetFileNameWithoutExtension(raReport.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")

                ret = raReport.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If
            End If
            'raReport.PrtName = printername

            'raReport.ReportName = Path.GetFileNameWithoutExtension(raReport.RpdFileName) & CASTCommon.Calendar.Now.ToString("HHmmss") & "_" & CASTCommon.Calendar.Now.ToString("%fffffff")

            'ret = raReport.ReportPrint()
            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------------------ END
        Catch ex As Exception
            mErrorMessage = ex.Message
            ret = False
        End Try
        Return ret
    End Function

    ' 2016/06/08 �^�X�N�j���� ADD �yRD�zUI_B-14-99(RSV2�Ή�) -------------------- START
    '
    ' �@�\�@ �F ���
    '
    ' �����@ �F ARG1 - �v�����^��
    '
    ' �߂�l �F True-�����CFalse-���s
    '
    ' ���l�@ �F 
    '
    Public Overloads Function PrintOut(ByVal printername As String, ByVal ReportName As String) As Boolean
        Dim ret As Boolean
        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "�b�r�u�t�@�C����������܂���B" & Path.Combine(mCsvPath, mCsvName)
                Return False
            End If

            ' ���|�[�g�ݒ�
            Call SetupReport()

            ' ���
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START
            If FLG64 = "1" Then
                If printername <> "" Then
                    raReport64.PrtName = printername
                End If

                raReport64.ReportName = ReportName

                ret = raReport64.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If
            Else
                If printername <> "" Then
                    raReport.PrtName = printername
                End If

                raReport.ReportName = ReportName

                ret = raReport.ReportPrint()
                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If
            End If
            'If printername <> "" Then
            '    raReport.PrtName = printername
            'End If

            'raReport.ReportName = ReportName

            'ret = raReport.ReportPrint()
            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------------------ END
        Catch ex As Exception
            mErrorMessage = ex.Message
            ret = False
        End Try
        Return ret
    End Function
    ' 2016/06/08 �^�X�N�j���� ADD �yRD�zUI_B-14-99(RSV2�Ή�) -------------------- END

    '
    ' �@�\�@ �F ���
    '
    ' �����@ �F ARG1 - �v�����^��
    '
    ' �߂�l �F True-�����CFalse-���s
    '
    ' ���l�@ �F 2012/06/30 �W���Ł@WEB�`���Ή�
    '
    Public Overloads Function PrintOut(ByVal printername As String, ByVal USER_ID As String, ByVal ITAKU_CODE As String) As Boolean
        Dim ret As Boolean
        Dim SrcPath As String = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_TMP")
        Dim DstPath As String = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_SED")
        '2017/04/26 �^�X�N�j���� ADD �W���ŏC���i�w�ZWEB�`���Ή��j------------------------------------ START
        Dim FNAME_MODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "WEBDEN_FNAME_MODE")
        '2017/04/26 �^�X�N�j���� ADD �W���ŏC���i�w�ZWEB�`���Ή��j------------------------------------ END
        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "�b�r�u�t�@�C����������܂���B" & Path.Combine(mCsvPath, mCsvName)
                Return False
            End If

            ' ���|�[�g�ݒ�
            Call SetupReport()

            ' ���
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START
            If FLG64 = "1" Then
                raReport64.PrtName = printername

                '2017/04/26 �^�X�N�j���� CHG �W���ŏC���i�w�ZWEB�`���Ή��j------------------------------------ START
                Dim RPD_NAME As String = Path.GetFileNameWithoutExtension(raReport64.RpdFileName)

                'WEB�`���̃t�@�C������u������i���[��`�̕����j
                If FNAME_MODE = "1" Then
                    Dim RET_RPD_NAME As String = CASTCommon.GetIni("WEBDEN_RPDNAME.INI", RPD_NAME, "RPDNAME")
                    Select Case RET_RPD_NAME
                        Case "err", ""
                            '�������Ȃ��i�W���d�l�j
                        Case Else
                            RPD_NAME = RET_RPD_NAME
                    End Select
                End If
                raReport64.ReportName = USER_ID & "_" & RPD_NAME & "_" & ITAKU_CODE
                'raReport64.ReportName = USER_ID & "_" & Path.GetFileNameWithoutExtension(raReport64.RpdFileName) & "_" & ITAKU_CODE
                '2017/04/26 �^�X�N�j���� CHG �W���ŏC���i�w�ZWEB�`���Ή��j------------------------------------ END

                ret = raReport64.ReportPrint()

                '��ʈ���Ή��@�ҋ@���Ԃ��w��
                Dim start As Object
                start = CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss"))
                Do While CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss")) > start + 60
                    Application.DoEvents()
                Loop

                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If

                'PDF�t�@�C�����쐬��������܂őҋ@����
                For cnt As Integer = 1 To 300 Step 1
                    Try
                        If File.Exists(SrcPath & raReport64.ReportName & ".pdf") = True Then
                            File.Copy(SrcPath & raReport64.ReportName & ".pdf", DstPath & raReport64.ReportName & ".pdf", True)
                            File.Delete(SrcPath & raReport64.ReportName & ".pdf")
                            Exit For
                        End If
                    Catch ex As Exception
                        'System.Threading.Thread.Sleep(1000) '2010/05/18 �R�����g�A�E�g
                    End Try
                    '2010/05/18 ��O�ɂȂ�Ȃ��ꍇ������̂Ń��[�v�ɑҋ@������
                    System.Threading.Thread.Sleep(1000)
                Next

            Else
                raReport.PrtName = printername

                '2017/04/26 �^�X�N�j���� CHG �W���ŏC���i�w�ZWEB�`���Ή��j------------------------------------ START
                Dim RPD_NAME As String = Path.GetFileNameWithoutExtension(raReport.RpdFileName)

                'WEB�`���̃t�@�C������u������i���[��`�̕����j
                If FNAME_MODE = "1" Then
                    Dim RET_RPD_NAME As String = CASTCommon.GetIni("WEBDEN_RPDNAME.INI", RPD_NAME, "RPDNAME")
                    Select Case RET_RPD_NAME
                        Case "err", ""
                            '�������Ȃ��i�W���d�l�j
                        Case Else
                            RPD_NAME = RET_RPD_NAME
                    End Select
                End If
                raReport.ReportName = USER_ID & "_" & RPD_NAME & "_" & ITAKU_CODE
                'raReport.ReportName = USER_ID & "_" & Path.GetFileNameWithoutExtension(raReport.RpdFileName) & "_" & ITAKU_CODE
                '2017/04/26 �^�X�N�j���� CHG �W���ŏC���i�w�ZWEB�`���Ή��j------------------------------------ END

                ret = raReport.ReportPrint()

                '��ʈ���Ή��@�ҋ@���Ԃ��w��
                Dim start As Object
                start = CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss"))
                Do While CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss")) > start + 60
                    Application.DoEvents()
                Loop

                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If

                'PDF�t�@�C�����쐬��������܂őҋ@����
                For cnt As Integer = 1 To 300 Step 1
                    Try
                        If File.Exists(SrcPath & raReport.ReportName & ".pdf") = True Then
                            File.Copy(SrcPath & raReport.ReportName & ".pdf", DstPath & raReport.ReportName & ".pdf", True)
                            File.Delete(SrcPath & raReport.ReportName & ".pdf")
                            Exit For
                        End If
                    Catch ex As Exception
                        'System.Threading.Thread.Sleep(1000) '2010/05/18 �R�����g�A�E�g
                    End Try
                    '2010/05/18 ��O�ɂȂ�Ȃ��ꍇ������̂Ń��[�v�ɑҋ@������
                    System.Threading.Thread.Sleep(1000)
                Next
            End If
            'raReport.PrtName = printername

            ''2017/04/26 �^�X�N�j���� CHG �W���ŏC���i�w�ZWEB�`���Ή��j------------------------------------ START
            'Dim RPD_NAME As String = Path.GetFileNameWithoutExtension(raReport.RpdFileName)

            ''WEB�`���̃t�@�C������u������i���[��`�̕����j
            'If FNAME_MODE = "1" Then
            '    Dim RET_RPD_NAME As String = CASTCommon.GetIni("WEBDEN_RPDNAME.INI", RPD_NAME, "RPDNAME")
            '    Select Case RET_RPD_NAME
            '        Case "err", ""
            '            '�������Ȃ��i�W���d�l�j
            '        Case Else
            '            RPD_NAME = RET_RPD_NAME
            '    End Select
            'End If
            'raReport.ReportName = USER_ID & "_" & RPD_NAME & "_" & ITAKU_CODE
            ''raReport.ReportName = USER_ID & "_" & Path.GetFileNameWithoutExtension(raReport.RpdFileName) & "_" & ITAKU_CODE
            ''2017/04/26 �^�X�N�j���� CHG �W���ŏC���i�w�ZWEB�`���Ή��j------------------------------------ END

            'ret = raReport.ReportPrint()

            ''��ʈ���Ή��@�ҋ@���Ԃ��w��
            'Dim start As Object
            'start = CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss"))
            'Do While CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss")) > start + 60
            '    Application.DoEvents()
            'Loop

            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If

            ''PDF�t�@�C�����쐬��������܂őҋ@����
            'For cnt As Integer = 1 To 300 Step 1
            '    Try
            '        If File.Exists(SrcPath & raReport.ReportName & ".pdf") = True Then
            '            File.Copy(SrcPath & raReport.ReportName & ".pdf", DstPath & raReport.ReportName & ".pdf", True)
            '            File.Delete(SrcPath & raReport.ReportName & ".pdf")
            '            Exit For
            '        End If
            '    Catch ex As Exception
            '        'System.Threading.Thread.Sleep(1000) '2010/05/18 �R�����g�A�E�g
            '    End Try
            '    '2010/05/18 ��O�ɂȂ�Ȃ��ꍇ������̂Ń��[�v�ɑҋ@������
            '    System.Threading.Thread.Sleep(1000)
            'Next
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------------------ END

        Catch ex As Exception
            mErrorMessage = ex.Message
            ret = False
        End Try
        Return ret
    End Function

    '*** Str Add 2015/12/04 SO)���� for �g������Ή� ***

    ' �@�\   �F ����i�g������Ή��j
    ' ����   �F printername  �v�����^��
    '           prtDspName  �����
    ' �߂�l �F True-�����CFalse-���s
    '
    Public Function ExtendPrintOut(ByVal printername As String, ByVal prtDspName As String) As Boolean

        Dim ret As Boolean = False

        Dim LOG As CASTCommon.BatchLOG = New CASTCommon.BatchLOG("CAstReports", "RAX")

        ' �����J�n���O�o��
        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("RAX.ExtendPrintOut")

        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "�b�r�u�t�@�C����������܂���B" & Path.Combine(mCsvPath, mCsvName)
                Return ret
            End If

            ' ���|�[�g�ݒ�
            Call SetupReport()

            '2017/12/11 saitou �L���M��(RSV2�W��) UPD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START
            If FLG64 = "1" Then
                If Not printername = "" Then
                    ' �ʏ�g���v�����^�ȊO�̏ꍇ�A�v�����^����ݒ�
                    raReport64.PrtName = printername
                End If

                ' �������ݒ�
                raReport64.ReportName = prtDspName

                ' ���
                ret = raReport64.ReportPrint()

                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If
            Else
                If Not printername = "" Then
                    ' �ʏ�g���v�����^�ȊO�̏ꍇ�A�v�����^����ݒ�
                    raReport.PrtName = printername
                End If

                ' �������ݒ�
                raReport.ReportName = prtDspName

                ' ���
                ret = raReport.ReportPrint()

                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If
            End If
            'If Not printername = "" Then
            '    ' �ʏ�g���v�����^�ȊO�̏ꍇ�A�v�����^����ݒ�
            '    raReport.PrtName = printername
            'End If

            '' �������ݒ�
            'raReport.ReportName = prtDspName

            '' ���
            'ret = raReport.ReportPrint()

            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------------------ END

            Return ret

        Catch ex As Exception
            mErrorMessage = ex.ToString
            Return ret

        Finally
            ' �����I�����O�o��
            LOG.Write_Exit3(sw, "RAX.ExtendPrintOut", "���A�l=" & ret)
        End Try

    End Function

    '*** End Add 2015/12/04 SO)���� for �g������Ή� ***

    '
    ' �@�\�@ �F �v���r���[
    '
    ' �����@ �F ARG1 - 
    '
    ' �߂�l �F True-�����CFalse-���s
    '
    ' ���l�@ �F 
    '
    Public Function Preview() As Boolean
        Dim ret As Boolean
        Try
            If File.Exists(Path.Combine(mCsvPath, mCsvName)) = False Then
                mErrorMessage = "�b�r�u�t�@�C����������܂���B" & Path.Combine(mCsvPath, mCsvName)
                Return False
            End If

            ' ���|�[�g�ݒ�
            Call SetupReport()

            ' �v���r���[
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START
            If FLG64 = "1" Then
                ret = raReport64.ReportView()
                If ret = False Then
                    mErrorMessage = raReport64.ErrorDetail
                End If
            Else
                ret = raReport.ReportView()
                If ret = False Then
                    mErrorMessage = raReport.ErrorDetail
                End If
            End If
            'ret = raReport.ReportView()
            'If ret = False Then
            '    mErrorMessage = raReport.ErrorDetail
            'End If
            '2017/12/11 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------------------ END
        Catch ex As Exception
            mErrorMessage = ex.Message
            ret = False
        End Try

        Return ret
    End Function

    '
    ' �@�\�@ �F ���|�[�g�ݒ�
    '
    ' �����@ �F ARG1 - 
    '
    ' �߂�l �F 
    '
    ' ���l�@ �F 
    '
    Protected Overloads Sub SetupReport()
        '2017/12/11 saitou �L���M��(RSV2�W��) UPD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START
        '��������s���Ă���[����64�r�b�g���ǂ������f
        If System.Environment.Is64BitProcess Then
            FLG64 = "1"
        Else
            FLG64 = "0"
        End If

        If FLG64 = "1" Then
            ' ���|�[�g�쐬
            raReport64 = New RA64

            ' ���|�[�g�p�X�w��
            raReport64.DataFileInputPath = mReportPath & ";"
            raReport64.CustomDataFileInputPath = mReportPath & ";"
            ' ���|�[�g���w��
            raReport64.RpdFileName = Path.Combine(mReportPath, mReportName)
            ' �C���v�b�g�b�r�u�t�@�C���w��
            raReport64.InputDataTextName = Path.Combine(mCsvPath, mCsvName)

            ' �������
            raReport64.Copies = Copies
            ' �����R�[�h
            raReport64.DataCode = DataCode

            If Not mLogPath Is Nothing AndAlso Not mLogName Is Nothing Then
                If mLogName = mReportName Then
                    raReport64.LogFileName = Path.Combine(mLogPath, mLogName & ".LOG")
                Else
                    raReport64.LogFileName = Path.Combine(mLogPath, mLogName)
                End If
                raReport64.LogSize = 1024 * 1024
            End If
        Else
            ' ���|�[�g�쐬
            raReport = New raocxlib.RepoAgent

            ' ���|�[�g�p�X�w��
            raReport.DataFileInputPath = mReportPath & ";"
            raReport.CustomDataFileInputPath = mReportPath & ";"
            ' ���|�[�g���w��
            raReport.RpdFileName = Path.Combine(mReportPath, mReportName)
            ' �C���v�b�g�b�r�u�t�@�C���w��
            raReport.InputDataTextName = Path.Combine(mCsvPath, mCsvName)

            ' �������
            raReport.Copies = Copies
            ' �����R�[�h
            raReport.DataCode = DataCode

            If Not mLogPath Is Nothing AndAlso Not mLogName Is Nothing Then
                If mLogName = mReportName Then
                    raReport.LogFileName = Path.Combine(mLogPath, mLogName & ".LOG")
                Else
                    raReport.LogFileName = Path.Combine(mLogPath, mLogName)
                End If
                raReport.LogSize = 1024 * 1024
            End If
        End If
        '' ���|�[�g�쐬
        'raReport = New raocxlib.RepoAgent

        '' ���|�[�g�p�X�w��
        'raReport.DataFileInputPath = mReportPath & ";"
        'raReport.CustomDataFileInputPath = mReportPath & ";"
        '' ���|�[�g���w��
        'raReport.RpdFileName = Path.Combine(mReportPath, mReportName)
        '' �C���v�b�g�b�r�u�t�@�C���w��
        'raReport.InputDataTextName = Path.Combine(mCsvPath, mCsvName)

        '' �������
        'raReport.Copies = Copies
        '' �����R�[�h
        'raReport.DataCode = DataCode

        'If Not mLogPath Is Nothing AndAlso Not mLogName Is Nothing Then
        '    If mLogName = mReportName Then
        '        raReport.LogFileName = Path.Combine(mLogPath, mLogName & ".LOG")
        '    Else
        '        raReport.LogFileName = Path.Combine(mLogPath, mLogName)
        '    End If
        '    raReport.LogSize = 1024 * 1024
        'End If
        '2017/12/11 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------------------ END
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

'2017/12/11 saitou �L���M��(RSV2�W��) ADD �T�[�o�[����Ή�(RepoAgent64�r�b�g�Ή�) -------------------- START

''' <summary>
''' RepoAgent 64�r�b�g������N���X
''' </summary>
''' <remarks>2017/12/11 saitou �L���M��(RSV2�W��) added for �T�[�o�[����Ή�</remarks>
Public Class RA64
    Protected mPrtName As String                    '�v�����^��
    Protected mReportName As String                 '������
    Protected mRpdFileName As String                '���|�[�g��`�t�@�C����
    Protected mDataFileInputPath As String          '�f�[�^�\�[�X�t�@�C�������p�X
    Protected mInputDataTextName As String          '�f�[�^�\�[�X�t�@�C����
    Protected mCustomDataFileInputPath As String    '�J�X�^���f�[�^�\�[�X�t�@�C�������p�X
    Protected mCopies As Integer                    '�R�s�[����
    Protected mErrorDetail As String                '�G���[���e������
    Protected mDataCode As String                   '�����R�[�h�̌n
    Protected mLogFileName As String                '���O�̎�t�@�C����
    Protected mLogSize As String                    '�ő働�O�̎��
    '2018/03/07 saitou �L���M��(RSV2�W��) ADD �T�[�o�[����Ή� -------------------- START
    Private htErrorMessage As Hashtable             '�G���[���b�Z�[�W���X�g
    '2018/03/07 saitou �L���M��(RSV2�W��) ADD ------------------------------------- END

    ''' <summary>
    ''' �R���X�g���N�^
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        mPrtName = ""
        mReportName = ""
        mRpdFileName = ""
        mErrorDetail = ""
        mDataFileInputPath = ""
        mInputDataTextName = ""
        mCustomDataFileInputPath = ""
        mCopies = 1
        mDataCode = ""

        '2018/03/07 saitou �L���M��(RSV2�W��) ADD �T�[�o�[����Ή� -------------------- START
        'RepoAgent�̃G���[���b�Z�[�W���X�g�̍쐬
        htErrorMessage = New Hashtable
        Dim sr As StreamReader = Nothing

        Try
            Dim ErrTxt As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "����G���[�R�[�h.TXT")
            If File.Exists(ErrTxt) Then
                sr = New StreamReader(ErrTxt, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
                While sr.Peek > -1
                    Dim s() As String = sr.ReadLine.Split("="c)
                    If s.Length >= 2 Then
                        If htErrorMessage.ContainsKey(s(0)) = False Then
                            Dim ErrStr As String = String.Empty
                            '�G���[���b�Z�[�W���̃C�R�[������؂蕶���Ƃ��Ĕ��f����A
                            '������Ƃ��ĕ\������Ȃ��̂ŁA�C�R�[����ʓr�ǉ�����
                            For i As Integer = 1 To s.Length - 1
                                ErrStr &= s(i)
                                If i <> s.Length - 1 Then
                                    ErrStr &= "="
                                End If
                            Next
                            htErrorMessage.Add(s(0), s(1))
                        End If
                    End If
                End While
                sr.Close()
                sr = Nothing
            End If
        Catch ex As Exception
            Throw
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
        End Try
        '2018/03/07 saitou �L���M��(RSV2�W��) ADD ------------------------------------- END
    End Sub

#Region "�v���p�e�B"
    ''' <summary>
    ''' �v�����^��
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PrtName() As String
        Get
            Return mPrtName
        End Get
        Set(value As String)
            mPrtName = value
        End Set
    End Property

    ''' <summary>
    ''' ������
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReportName() As String
        Get
            Return mReportName
        End Get
        Set(value As String)
            mReportName = value
        End Set
    End Property

    ''' <summary>
    ''' ���|�[�g��`�t�@�C����
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RpdFileName As String
        Get
            Return mRpdFileName
        End Get
        Set(value As String)
            mRpdFileName = value
        End Set
    End Property

    ''' <summary>
    ''' �f�[�^�\�[�X�t�@�C�������p�X
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataFileInputPath() As String
        Get
            Return mDataFileInputPath
        End Get
        Set(ByVal Value As String)
            mDataFileInputPath = Value
        End Set
    End Property

    ''' <summary>
    ''' �f�[�^�\�[�X�t�@�C����
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property InputDataTextName() As String
        Get
            Return mInputDataTextName
        End Get
        Set(ByVal Value As String)
            mInputDataTextName = Value
        End Set
    End Property

    ''' <summary>
    ''' �J�X�^���f�[�^�\�[�X�t�@�C�������p�X
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CustomDataFileInputPath() As String
        Get
            Return mCustomDataFileInputPath
        End Get
        Set(ByVal Value As String)
            mCustomDataFileInputPath = Value
        End Set
    End Property

    ''' <summary>
    ''' �R�s�[����
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Copies() As String
        Get
            Return mCopies.ToString
        End Get
        Set(ByVal Value As String)
            mCopies = CInt(Value)
        End Set
    End Property

    ''' <summary>
    ''' �G���[���e������
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ErrorDetail() As String
        Get
            Return mErrorDetail
        End Get
        Set(ByVal Value As String)
            mErrorDetail = Value
        End Set
    End Property

    ''' <summary>
    ''' �����R�[�h�n
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataCode() As String
        Get
            Return mDataCode
        End Get
        Set(ByVal Value As String)
            mDataCode = Value
        End Set
    End Property

    ''' <summary>
    ''' ���O�̎�t�@�C����
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LogFileName() As String
        Get
            Return mLogFileName
        End Get
        Set(ByVal Value As String)
            mLogFileName = Value
        End Set
    End Property

    ''' <summary>
    ''' �ő働�O�̎��
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LogSize() As String
        Get
            Return mLogSize
        End Get
        Set(ByVal Value As String)
            mLogSize = Value
        End Set
    End Property
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' ���[�o��
    ''' </summary>
    ''' <returns>True - ���� , False - �ُ�</returns>
    ''' <remarks></remarks>
    Public Function ReportPrint() As Boolean
        Dim CmdArg As String = ""

        CmdArg = RpdFileName & " -id " & InputDataTextName

        '���O�t�@�C����
        If Not LogFileName Is Nothing Then
            CmdArg &= " -lf " & LogFileName
        End If

        '����
        CmdArg &= " -cp " & Copies
        '�v�����^��
        CmdArg &= " -p """ & PrtName & """"
        '�h�L�������g��
        CmdArg &= " -n " & ReportName

        Try
            Dim ProcReport As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(CASTCommon.GetRSKJIni("RSV2_V1.0.0", "REPOAGENT_PATH"), "raprint.exe")
            'ProcInfo.FileName = "raprint.exe"
            If CmdArg.Length > 0 Then
                ProcInfo.Arguments = CmdArg
            End If
            ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "EXE")
            ProcInfo.UseShellExecute = False
            ProcInfo.CreateNoWindow = True
            ProcInfo.RedirectStandardOutput = True
            ProcReport = Process.Start(ProcInfo)

            Do While True
                If ProcReport.HasExited = True Then
                    Exit Do
                Else
                    Dim NowProc As Process
                    Try
                        NowProc = Diagnostics.Process.GetProcessById(ProcReport.Id)
                        If NowProc Is Nothing Then
                            Exit Do
                        End If
                    Catch ex As Exception
                        Exit Do
                    End Try
                End If

                Application.DoEvents()

                Threading.Thread.Sleep(100)

            Loop

            If ProcReport.ExitCode <> 0 Then
                '2018/03/07 saitou �L���M��(RSV2�W��) ADD �T�[�o�[����Ή� -------------------- START
                '�G���[���b�Z�[�W��ݒ肵�A�ُ�ŕԂ��B
                ErrorDetail = GetErrMessageReport(ProcReport.ExitCode.ToString)
                Return False
                '2018/03/07 saitou �L���M��(RSV2�W��) ADD ------------------------------------- END
            End If

            Return True
        Catch ex As Exception
            '2018/03/07 saitou �L���M��(RSV2�W��) ADD �T�[�o�[����Ή� -------------------- START
            '��O���������G���[���b�Z�[�W��ݒ肷��B
            ErrorDetail = ex.ToString
            '2018/03/07 saitou �L���M��(RSV2�W��) ADD ------------------------------------- END
            Return False
        End Try
    End Function

    ''' <summary>
    ''' ���[�v���r���[
    ''' </summary>
    ''' <returns>True - ���� , False - �ُ�</returns>
    ''' <remarks></remarks>
    Public Function ReportView() As Boolean
        Dim CmdArg As String = ""

        CmdArg = RpdFileName & " -id " & InputDataTextName

        '���O�t�@�C����
        If Not LogFileName Is Nothing Then
            CmdArg &= " -lf " & LogFileName
        End If

        Try
            Dim ProcReport As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(CASTCommon.GetRSKJIni("RSV2_V1.0.0", "REPOAGENT_PATH"), "raview.exe")
            'ProcInfo.FileName = "raview.exe"

            If CmdArg.Length > 0 Then
                ProcInfo.Arguments = CmdArg
            End If
            ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "EXE")
            ProcInfo.UseShellExecute = False
            ProcInfo.CreateNoWindow = True
            ProcInfo.RedirectStandardOutput = True
            ProcReport = Process.Start(ProcInfo)

            Do While True
                If ProcReport.HasExited = True Then
                    Exit Do
                Else
                    Dim NowProc As Process
                    Try
                        NowProc = Diagnostics.Process.GetProcessById(ProcReport.Id)
                        If NowProc Is Nothing Then
                            Exit Do
                        End If
                    Catch ex As Exception
                        Exit Do
                    End Try
                End If

                Application.DoEvents()

                Threading.Thread.Sleep(100)

            Loop

            If ProcReport.ExitCode <> 0 Then
                '2018/03/07 saitou �L���M��(RSV2�W��) ADD �T�[�o�[����Ή� -------------------- START
                '�G���[���b�Z�[�W��ݒ肵�A�ُ�ŕԂ��B
                ErrorDetail = GetErrMessageReport(ProcReport.ExitCode.ToString)
                Return False
                '2018/03/07 saitou �L���M��(RSV2�W��) ADD ------------------------------------- END
            End If

            Return True
        Catch ex As Exception
            '2018/03/07 saitou �L���M��(RSV2�W��) ADD �T�[�o�[����Ή� -------------------- START
            '��O���������G���[���b�Z�[�W��ݒ肷��B
            ErrorDetail = ex.ToString
            '2018/03/07 saitou �L���M��(RSV2�W��) ADD ------------------------------------- END
            Return False
        End Try
    End Function

    '2018/03/07 saitou �L���M��(RSV2�W��) ADD �T�[�o�[����Ή� -------------------- START
    Private Function GetErrMessageReport(ByVal errCode As String) As String
        If htErrorMessage.ContainsKey(errCode) = True Then
            Return htErrorMessage.Item(errCode).ToString
        Else
            Return "����G���[�R�[�h�ɐݒ肳��Ă��Ȃ��G���[���������܂����B�G���[�R�[�h�F" & errCode
        End If
    End Function
    '2018/03/07 saitou �L���M��(RSV2�W��) ADD ------------------------------------- END
#End Region

End Class
'2017/12/11 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------------------ END
