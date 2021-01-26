Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CAstSystem
Imports CASTCommon.ModPublic

' �ꊇ���Ƃ����ݘA�g����
Public Class ClsIkkatuRenkei

    ' ���O�����N���X
    Private LOG As CASTCommon.BatchLOG

    ' �ꊇ�N���p�����[�^ �\����
    Structure IKKATUPARAM
        Dim FSYORI_KBN As String        ' �����敪�i1:���U,2:�r�r�r,3:�U���j
        Dim RENKEI_KBN As String        ' �A�g�敪
        Dim FMT_KBN As String           ' �t�H�[�}�b�g�敪
        Dim JOBTUUBAN As Integer        ' �W���u�ʔ�
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public WriteOnly Property Data() As String
            Set(ByVal value As String)
                Dim para() As String = value.Split(","c)
                FSYORI_KBN = para(0)
                RENKEI_KBN = para(1).PadLeft(2, "0"c)
                FMT_KBN = para(2)
                JOBTUUBAN = Integer.Parse(para(3))
            End Set
        End Property
    End Structure
    Private mIkParam As IKKATUPARAM

    ' �N���p�����[�^ ���ʏ��
    Private mArgumentData As CommData

    ' �˗��f�[�^�t�@�C����
    Private mDataFileName As String

    ' �p�u���b�N�c�a
    Private MainDB As CASTCommon.MyOracle

    ' FSKJ.INI �Z�N�V������
    Private ReadOnly AppTOUROKU As String = "OTHERSYS"

    ' New
    Public Sub New()
    End Sub

    Public Function Main(ByVal command As String) As Integer

        Dim Comm() As String = command.Split(","c)

        ' �I���N��
        MainDB = New CASTCommon.MyOracle

        ' �A�g�敪����e�����𕪊򂷂�
        Select Case Comm(1)
            Case "02"
                ' �b�l�s����
            Case Else
                ' �������ݏ������Ăяo��
                mIkParam.JOBTUUBAN = CAInt32(Comm(3))
                LOG = New CASTCommon.BatchLOG("���V�X�A�g", AppTOUROKU, CType(mIkParam.JOBTUUBAN, String))
                'LOG.JobTuuban = mIkParam.JOBTUUBAN
                'LOG.Write("���V�X�A�g", "����", command)

                'Dim ProcInfo As New ProcessStartInfo
                'ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "EXE"), "KFJT101.EXE")
                'ProcInfo.Arguments = command
                'ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "EXE")
                'Call Process.Start(ProcInfo)

                Dim Syori As String = "��������"
                If mIkParam.FSYORI_KBN = "10" Then
                    Syori = "�Ԋ�"
                End If
                Dim Fmt As String = ""
                Select Case Comm(1)
                    Case "01"
                        Fmt = "���f�B�A�R���o�[�^"
                    Case "09"
                        Fmt = "�ėp�G���g���V�X�e��"
                    Case "02"
                        Fmt = "�b�l�s"
                    Case "07"
                        Fmt = "�w�Z���U�V�X�e��"
                End Select
                '*** �C�� mitsu 2008/09/30 ���������� ***
                'Dim Param() As String = CType(Array.CreateInstance(GetType(String), Comm.Length - 1), String())
                Dim Param() As String = New String(Comm.Length - 2) {}
                '****************************************
                Array.Copy(Comm, Param, Comm.Length - 1)
                If InsertJOBMAST(Param, "") = True Then
                    LOG.Write("�ꊇ" & Syori & "�A�g(" & Fmt & ")", "����")
                    LOG.UpdateJOBMASTbyOK("�ꊇ" & Syori & "�A�g(" & Fmt & ")")
                    ' �R�~�b�g
                    MainDB.Commit()
                    Return 0
                Else
                    LOG.Write("�ꊇ" & Syori & "�A�g(" & Fmt & ")", "���s")
                    LOG.UpdateJOBMASTbyErr("�ꊇ" & Syori & "�A�g(" & Fmt & ")")
                    ' �R�~�b�g
                    MainDB.Commit()
                    Return 3
                End If
        End Select

        ' �p�����[�^�`�F�b�N
        Try
            ' ���C�������ݒ�
            mIkParam.Data = command

            ' �W���u�ʔԐݒ�
            LOG = New CASTCommon.BatchLOG("���V�X�A�g", AppTOUROKU, CType(mIkParam.JOBTUUBAN, String))
            LOG.JobTuuban = mIkParam.JOBTUUBAN
            LOG.ToriCode = ""
        Catch ex As Exception
            LOG.Write("�J�n", "���s", "�p�����^�擾���s[" & command & "]")
            Return 1
        End Try

        ' �N���p�����[�^���ʏ��
        mArgumentData = New CommData(MainDB)

        ' �p�����[�^����ݒ�
        Dim InfoParam As CommData.stPARAMETER
        InfoParam.TORI_CODE = ""
        InfoParam.BAITAI_CODE = ""
        InfoParam.FMT_KBN = mIkParam.FMT_KBN
        InfoParam.FURI_DATE = ""
        InfoParam.CODE_KBN = ""

        Dim TakouCMT As Boolean = False
        If mIkParam.FSYORI_KBN = "10" Then
            TakouCMT = True
        End If
        InfoParam.FSYORI_KBN = mIkParam.FSYORI_KBN
        InfoParam.JOBTUUBAN = mIkParam.JOBTUUBAN
        InfoParam.RENKEI_KBN = mIkParam.RENKEI_KBN
        InfoParam.RENKEI_FILENAME = ""
        InfoParam.ENC_KBN = ""
        InfoParam.ENC_KEY1 = ""
        InfoParam.ENC_KEY2 = ""
        InfoParam.ENC_OPT1 = ""
        InfoParam.CYCLENO = ""
        mArgumentData.INFOParameter = InfoParam

        ' �A�g�p�N���X�쐬
        Dim oRenkei As CAstSystem.ClsRenkei

        Dim bRet As Boolean
        ' �A�g����
        If TakouCMT = True Then
            oRenkei = New CAstSystem.ClsRenkei(mArgumentData, 1)

            ' ���s���ʂb�l�s�ꊇ�X�V
            bRet = MainTakouCMT(oRenkei)
        Else
            oRenkei = New CAstSystem.ClsRenkei(mArgumentData)

            ' ��Ƃb�l�s�ꊇ��������
            bRet = MainCMT(oRenkei)
        End If
        If bRet = False Then
            Call LOG.UpdateJOBMASTbyErr(oRenkei.Message)

            ' ���[���o�b�N
            MainDB.Rollback()
        Else
            Call LOG.UpdateJOBMASTbyOK(oRenkei.Message)
            ' �R�~�b�g
            MainDB.Commit()
        End If

        MainDB.Close()

        If bRet = False Then
            Return 2
        End If
        Return 0

    End Function

    ' �@�\�@ �F �b�l�s 
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function MainCMT(ByRef renkei As ClsRenkei) As Boolean
        LOG.Write("�b�l�s�A�g�J�n", "����")

        Try
            Dim FileList() As String = renkei.GetCMTOtherFiles()
            LOG.Write("�b�l�s�t�@�C���擾:" & FileList.Length.ToString & "��", "����")

            Dim OutFileName As String
            For i As Integer = 0 To FileList.Length - 1
                renkei.InFileName = FileList(i)
                If Path.GetFileName(FileList(i)).ToUpper.StartsWith("C") = True Then
                    ' ���������ăR�s�[
                    OutFileName = renkei.FileDecodeMove()
                    If OutFileName = "" Then
                        Call LOG.Write("�b�l�s������", "���s", FileList(i) & ":" & renkei.Message)

                        ' ���s���������̂܂܎��֘A�g����
                        OutFileName = renkei.FileCopy()
                    End If
                Else
                    ' ���̂܂܃R�s�[
                    OutFileName = renkei.FileCopy()
                End If
                If OutFileName <> "" Then
                    ' �b�l�s�t�@�C���擾����
                    ' �i�n�a�o�^
                    If TourokuFile(renkei, OutFileName) = True Then
                        LOG.Write("�b�l�s�t�@�C���o�^", "����", OutFileName)
                        File.Delete(renkei.InFileName)

                        MainDB.Commit()

                        MainDB.BeginTrans()
                    Else
                        LOG.Write("�b�l�s�t�@�C���o�^", "���s", OutFileName)
                        renkei.Message = "�b�l�s�t�@�C���o�^���s " & OutFileName
                        Return False
                    End If
                End If
            Next i

            ' �O�O�F�S��  �O�P�F�n���́i�R�T�O�j  �O�Q�F����  �O�R�F�N���@�O�S�F�˗����@�O�T�F�`�[�@�O�U�F�n���́i�R�O�O�j�@�Q�O�F�r�r�r
            Dim sRenkei As String
            Select Case mArgumentData.INFOParameter.FMT_KBN
                Case "01"
                    sRenkei = "�n���́i�R�T�O�j"
                Case "02"
                    sRenkei = "����"
                Case "03"
                    sRenkei = "�N��"
                Case "06"
                    sRenkei = "�n���́i�R�O�O�j"
                Case "00"
                    sRenkei = "�S��"
                    '*** �C�� mitsu 2008/07/17 �Z���^�[���ڎ����t�H�[�}�b�g�ǉ� ***
                Case "TO"
                    sRenkei = "�Z���^�[���ڎ���"
                    '**************************************************************
                Case Else
                    sRenkei = "�Ȃ�"
            End Select

            If FileList.Length = 0 Then
                renkei.Message = "�b�l�s�Ώۃf�[�^�i" & sRenkei & "�j�F�O��"
            Else
                renkei.Message = "�b�l�s�����o�^ " & renkei.Message & " �t�H�[�}�b�g�敪�F" & sRenkei
            End If

        Catch ex As Exception
            Call LOG.Write("�b�l�s", "���s", ex.Message & ":" & ex.StackTrace)
            Call LOG.UpdateJOBMASTbyErr("�b�l�s�A�g ���s")
            Return False
        Finally
        End Try

        LOG.Write("�b�l�s�A�g�I��", "����")

        Return True
    End Function

    ' �@�\�@ �F �t�@�C���������ǂݍ���łi�n�a�֓o�^����
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F ���f�B�A�R���o�[�^�C�w�Z�C�b�l�s 
    '
    Private Function TourokuFile(ByRef oRenkei As ClsRenkei, ByRef filename As String) As Boolean

        '�}�̓ǂݍ���
        '�t�H�[�}�b�g�@
        Dim oReadFMT As CAstFormat.CFormat
        Try
            ' �t�H�[�}�b�g�敪����C�t�H�[�}�b�g����肷��
            oReadFMT = oReadFMT.GetFormat(mArgumentData.INFOParameter)

            '*** �C�� mitsu 2008/08/25 �ϑ��҃R�[�h�Ǒ֏��� ***
            '�S��t�H�[�}�b�g�̏ꍇ�̂�
            If mArgumentData.INFOParameter.FMT_KBN = "00" AndAlso oRenkei.ConvertItakuCode(filename, LOG) = False Then
                Return False
            End If
            '**************************************************

            If oReadFMT.FirstRead(filename) = 1 Then
                Call oReadFMT.CheckDataFormat()
                ' 2008.04.14 ADD �ςȃf�[�^�Ή� >>
                Call oReadFMT.CheckRecord1()
                ' 2008.04.14 ADD <<
            End If
            oReadFMT.ToriData = mArgumentData
            Dim Param() As String = GetParam(oReadFMT, filename)
            If Not oReadFMT.InfoMeisaiMast.ITAKU_CODE Is Nothing Then
                oRenkei.Message = "�ϑ��҃R�[�h�F" & oReadFMT.InfoMeisaiMast.ITAKU_CODE
            End If
            oReadFMT.Close()

            ' 2008.04.21 ADD �������擾���s���� >>
            If Param.Length >= 1 And Param(0).StartsWith("000000000") = True Then

                If File.Exists(filename) = True Then
                    File.Delete(filename)
                End If
                filename = oRenkei.InFileName
                LOG.Write("�������擾", "���s", "�ϑ��҃R�[�h�F" & oReadFMT.InfoMeisaiMast.ITAKU_CODE & " �t�@�C����:" & filename)

                ' �������擾���s
                Dim Prn As New ClsPrnSyorikekkaKakuninhyo
                If Prn.OutputCSVKekkaSysError(oReadFMT.InfoMeisaiMast.ITAKU_CODE, mArgumentData.INFOParameter.FSYORI_KBN, oRenkei.InFileName, MainDB) = False Then
                    LOG.Write("�������ʊm�F�\", "���s", "�t�@�C����:" & Prn.FileName & " ���b�Z�[�W:" & Prn.ReportMessage)
                End If
                Prn = Nothing
                oReadFMT.Close()
                Return False
            End If
            ' 2008.04.21 ADD �������擾���s���� <<

            ' �i�n�a�}�X�^����o�^
            If InsertJOBMAST(Param, oRenkei.InFileName) = True Then
                If (oReadFMT.InfoMeisaiMast.ITAKU_CODE Is Nothing) Then
                    LOG.Write("�˗��f�[�^�ǂݎ��", "����")
                Else
                    LOG.Write("�˗��f�[�^�ǂݎ��", "����", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                End If
            Else
                LOG.Write("�˗��f�[�^�ǂݎ��", "���s", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                Return False
            End If
            oReadFMT = Nothing
        Catch ex As Exception
            If Not oReadFMT Is Nothing Then
                oReadFMT.Close()
            End If
            LOG.Write("�˗��f�[�^�W���u�o�^", "���s", ex.Message & ":" & ex.StackTrace)
            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F �b�l�s (���s����)
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function MainTakouCMT(ByVal renkei As ClsRenkei) As Boolean
        LOG.Write("�b�l�s���s���ʘA�g�J�n", "����")

        Try
            Dim FileList() As String = renkei.GetCMTOtherFiles()
            LOG.Write("�b�l�s�t�@�C���擾:" & FileList.Length.ToString & "��", "����")

            Dim OutFileName As String
            For i As Integer = 0 To FileList.Length - 1
                renkei.InFileName = FileList(i)
                ' ���̂܂܃R�s�[
                OutFileName = renkei.FileCopy()
                If OutFileName <> "" Then
                    ' �b�l�s�t�@�C���擾����
                    ' �i�n�a�o�^
                    If TourokuTakouFile(renkei, OutFileName) = True Then
                        LOG.Write("�b�l�s�t�@�C���o�^", "����", OutFileName)
                        File.Delete(renkei.InFileName)

                        MainDB.Commit()

                        MainDB.BeginTrans()
                    Else
                        LOG.Write("�b�l�s�t�@�C���o�^", "���s", OutFileName)
                        renkei.Message = "�b�l�s�t�@�C���o�^���s " & OutFileName
                        Return False
                    End If
                End If
            Next i

            If FileList.Length = 0 Then
                Dim sRenkei As String
                sRenkei = "���s����"
                renkei.Message = "�b�l�s�Ώۃf�[�^�i" & sRenkei & "�j�F�O��"
            End If

        Catch ex As Exception
            Call LOG.Write("�b�l�s", "���s", ex.Message & ":" & ex.StackTrace)
            Call LOG.UpdateJOBMASTbyErr("�b�l�s�A�g ���s")
            Return False
        Finally
        End Try

        LOG.Write("�b�l�s���s���ʘA�g�I��", "����")

        Return True
    End Function

    ' �@�\�@ �F �t�@�C���������ǂݍ���łi�n�a�֓o�^����i���s���ʁj
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F ���f�B�A�R���o�[�^�C�w�Z�C�b�l�s 
    '
    Private Function TourokuTakouFile(ByRef oRenkei As ClsRenkei, ByVal filename As String) As Boolean

        '�}�̓ǂݍ���
        '�t�H�[�}�b�g�@
        Dim oReadFMT As CAstFormat.CFormat
        Try
            ' �t�H�[�}�b�g�敪����C�t�H�[�}�b�g����肷��
            oReadFMT = oReadFMT.GetFormat(mArgumentData.INFOParameter)
            If oReadFMT.FirstRead(filename) = 1 Then
                Call oReadFMT.CheckDataFormat()
                oReadFMT.ToriData = mArgumentData
                Dim Param() As String = GetTakouParam(oReadFMT, filename)
                oReadFMT.Close()

                ' �i�n�a�}�X�^����o�^
                If InsertJOBMAST(Param, oRenkei.InFileName) = True Then
                    If (oReadFMT.InfoMeisaiMast.ITAKU_CODE Is Nothing) Then
                        LOG.Write("���ʃf�[�^�ǂݎ��", "����")
                    Else
                        LOG.Write("���ʃf�[�^�ǂݎ��", "����", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                    End If
                Else
                    LOG.Write("���ʃf�[�^�ǂݎ��", "���s", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                    Return False
                End If
            Else
                LOG.Write("���ʃf�[�^�ǂݎ��", "���s", "�S��t�H�[�}�b�g�ȊO �t�@�C�����F" & filename)
                oReadFMT.Close()
            End If

            oReadFMT = Nothing
        Catch ex As Exception
            If Not oReadFMT Is Nothing Then
                oReadFMT.Close()
            End If
            LOG.Write("���ʃf�[�^�W���u�o�^", "���s", ex.Message & ":" & ex.StackTrace)
            Return False
        End Try

        Return True

    End Function

    Private Function InsertJOBMAST(ByVal param() As String, ByVal inFileName As String) As Boolean
        Dim SQL As New StringBuilder(128)

        SQL.Append("INSERT INTO JOBMAST(")
        SQL.Append(" TOUROKU_DATE_J")
        SQL.Append(",TOUROKU_TIME_J")
        SQL.Append(",STA_DATE_J")
        SQL.Append(",STA_TIME_J")
        SQL.Append(",END_DATE_J")
        SQL.Append(",END_TIME_J")
        SQL.Append(",JOBID_J")
        SQL.Append(",STATUS_J")
        SQL.Append(",USERID_J")
        SQL.Append(",PARAMETA_J")
        SQL.Append(",ERRMSG_J")
        SQL.Append(",DENSO_F_NAME_J")
        SQL.Append(",DENSO_TIME_STAMP_J")
        SQL.Append(") VALUES (")
        SQL.Append(" TO_CHAR(SYSDATE,'YYYYMMDD')")                          ' TOUROKU_DATE_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' TOUROKU_TIME_J
        SQL.Append(",'00000000'")                                           ' STA_DATE_J
        SQL.Append(",'000000'")                                             ' STA_TIME_J
        SQL.Append(",'00000000'")                                           ' END_DATE_J
        SQL.Append(",'000000'")                                             ' END_TIME_J
        If mIkParam.FSYORI_KBN = "10" Then
            SQL.Append(",'K101'")                                               ' JOBID_J
        Else
            SQL.Append(",'T101'")                                               ' JOBID_J
        End If
        SQL.Append(",'0'")                                                  ' STATUS_J
        SQL.Append("," & SQ(LOG.UserID))                                      ' USERID_J
        Dim sJoin As String = String.Join(",", param)
        SQL.Append("," & SQ(sJoin))                                         ' PARAMETA_J
        SQL.Append(",' '")                                                  ' ERRMSG_J
        SQL.Append("," & SQ(Path.GetFileName(inFileName)))                  ' DENSO_F_NAME_J
        SQL.Append(",TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')")                  ' DENSO_TIME_STAMP_J
        SQL.Append(")")
        If MainDB.ExecuteNonQuery(SQL) <= 0 Then
            LOG.Write("JOBMAST�o�^", "���s", MainDB.Message)
            Return False
        End If

        Return True
    End Function

    Private Function GetParam(ByVal readFmt As CAstFormat.CFormat, ByVal filename As String) As String()
        Dim Param As New ArrayList

        Dim ItakusyaCode As String
        Select Case mIkParam.FMT_KBN
            Case "02"
                ' ���ł̏ꍇ
                Dim TORICODE As String = GetKokuzeiTORIMAST(filename).PadRight(9, "0"c)

                Call readFmt.GetTorimastFromToriCode(TORICODE, MainDB)
            Case "03"
                ' �N���̏ꍇ
                Dim TORICODE As String = GetNenkinTORIMAST(filename).PadRight(9, "0"c)

                Call readFmt.GetTorimastFromToriCode(TORICODE, MainDB)

                '*** �C�� mitsu 2008/07/17 �Z���^�[���ڎ����̏ꍇ ***
            Case "TO"
                '�U�փR�[�h�E��ƃR�[�h��������}�X�^�����擾����
                Dim TORICODE As String = GetToKCenterTORIMAST(filename).PadRight(9, "0"c)

                Call readFmt.GetTorimastFromToriCode(TORICODE, MainDB)

                '�ϑ��҃R�[�h�𐳂����l�ɓǑ�
                readFmt.InfoMeisaiMast.ITAKU_CODE = readFmt.ToriData.INFOToriMast.ITAKU_CODE_T
                '************************************************
            Case Else
                ' �ϑ��҃R�[�h����C�����}�X�^�����擾����
                Call readFmt.GetTorimastFromItakuCode(MainDB)
        End Select

        ' �����R�[�h
        If Not readFmt.ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            Param.Add(readFmt.ToriData.INFOToriMast.TORIS_CODE_T & readFmt.ToriData.INFOToriMast.TORIF_CODE_T)
        Else
            Param.Add(New String("0"c, 10))
        End If

        ' �U�֓�
        Param.Add(readFmt.InfoMeisaiMast.FURIKAE_DATE)

        If Not readFmt.ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            ' �R�[�h�敪
            Param.Add(readFmt.ToriData.INFOToriMast.CODE_KBN_T)
        Else
            Param.Add("")
        End If

        ' �t�H�[�}�b�g�敪
        Param.Add(mIkParam.FMT_KBN)

        If Not readFmt.ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            ' �}�̃R�[�h
            Param.Add(readFmt.ToriData.INFOToriMast.BAITAI_CODE_T)
        Else
            Param.Add("")
        End If

        ' ���x���敪
        Param.Add("0")

        ' �A�g�敪
        Param.Add(mIkParam.RENKEI_KBN)

        ' �A�g�t�@�C����
        Param.Add("""" & Path.GetFileName(filename).TrimEnd & """")

        '' �Í����敪
        'Param.Add(readFmt.ToriData.INFOToriMast.ENC_KBN_T)
        'If readFmt.ToriData.INFOToriMast.ENC_KBN_T = "1" Then
        '    ' �Í����L�[
        '    Param.Add(readFmt.ToriData.INFOToriMast.ENC_KEY1_T)
        '    ' �Í���IV�L�[
        '    Param.Add(readFmt.ToriData.INFOToriMast.ENC_KEY2_T)
        '    ' �`�d�r
        '    Param.Add(readFmt.ToriData.INFOToriMast.ENC_OPT1_T)
        'Else
        '    ' �Í����L�[
        '    Param.Add("")
        '    ' �Í���IV�L�[
        '    Param.Add("")
        '    ' �`�d�r
        '    Param.Add("")
        'End If

        '' �T�C�N����
        'Param.Add("")

        Return CType(Param.ToArray(GetType(String)), String())
    End Function

    Private Function GetTakouParam(ByVal readFmt As CAstFormat.CFormat, ByVal filename As String) As String()
        Dim Param As New ArrayList

        ' �����R�[�h
        Param.Add("")

        ' �U�֓�
        Param.Add(readFmt.InfoMeisaiMast.FURIKAE_DATE)

        ' �t�H�[�}�b�g�敪
        Param.Add(mIkParam.FMT_KBN)

        ' �A�g�敪
        Param.Add(mIkParam.RENKEI_KBN)

        ' �X�V�L�[�F��ƃV�[�P���X
        Param.Add("0")

        ' �A�g�t�@�C����
        Param.Add(Path.GetFileName(filename).TrimEnd)

        Return CType(Param.ToArray(GetType(String)), String())
    End Function

    ' �@�\�@ �F ���Ł@�����擾
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Protected Friend Function GetKokuzeiTORIMAST(ByVal filename As String) As String
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader

        Dim KokuzeiFmt As New CAstFormat.CFormatKokuzei
        Call KokuzeiFmt.FirstRead(filename)
        KokuzeiFmt.GetFileData(KokuzeiFmt.KOKUZEI_REC1.Data)
        Dim Kamoku As String = KokuzeiFmt.KOKUZEI_REC1.KZ4
        KokuzeiFmt.Close()

        Dim ToriCode As String
        ' �ȖڃR�[�h�@020:�\��������, 300:����ŋy�n�������
        ToriCode = CASTCommon.GetFSKJIni("TOUROKU", "KOKUZEI" & Kamoku)
        If ToriCode <> "err" Then
            Return ToriCode
        End If

        SQL.Append("SELECT ")
        SQL.Append(" ITAKU_CODE_T,TORIS_CODE_T,TORIF_CODE_T")
        SQL.Append(" FROM TORIMAST")
        SQL.Append(" WHERE RENKEI_KBN_T = " & SQ(mIkParam.RENKEI_KBN))
        SQL.Append("   AND FMT_KBN_T = '02'")
        SQL.Append("   AND " & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & " BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")
        SQL.Append(" ORDER BY TORIS_CODE_T ASC, TORIF_CODE_T ASC")
        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                If Kamoku <> "020" Then
                    OraReader.NextRead()
                    If OraReader.EOF = True Then
                        OraReader.DataReader(SQL)
                    End If
                End If
                ToriCode = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")
                Return ToriCode.PadRight(9, " "c)
            End If
        Catch ex As Exception
            Return New String("0"c, 9)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            OraReader = Nothing
        End Try

        Return New String("0"c, 9)
    End Function

    ' �@�\�@ �F �N���@�����擾
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function GetNenkinTORIMAST(ByVal filename As String) As String
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader

        Dim NenkinFmt As New CAstFormat.CFormatNenkin
        Call NenkinFmt.FirstRead(filename)
        NenkinFmt.GetFileData(NenkinFmt.NENKIN_REC1.Data)
        Dim Kamoku As String = NenkinFmt.NENKIN_REC1.NK2
        NenkinFmt.Close()

        Dim ToriCode As String
        ' �N����ʂ��画�f 61:�������N��,62:���D���N��,63:�������N��,64:�J�ДN��,65:�V�����N���E�����N��,66:�V�D���N��,67:�������N���Z��
        ToriCode = CASTCommon.GetFSKJIni("TOUROKU", "NENKIN" & Kamoku)
        If ToriCode <> "err" Then
            Return ToriCode
        End If

        Return New String("0"c, 9)
    End Function

    '*** �C�� mitsu 2008/07/17 �Z���^�[���ڎ����t�H�[�}�b�g�̎����R�[�h�擾 ***
    ' �@�\�@ �F �Z���^�[���ڎ����@�����擾
    '
    ' �߂�l �F �����R�[�h
    '
    ' ���l�@ �F 
    '
    Private Function GetToKCenterTORIMAST(ByVal filename As String) As String
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader

        Dim CenterFmt As New CAstFormat.CFormatTokCenter
        Call CenterFmt.FirstRead(filename)
        CenterFmt.GetFileData(CenterFmt.TOKCENTER_REC1.Data)
        Dim FuriCode As String = CenterFmt.TOKCENTER_REC1.TC.TC5
        Dim KigyoCode As String = CenterFmt.TOKCENTER_REC1.TC.TC6
        CenterFmt.Close()

        Dim ToriCode As String

        SQL.Append("SELECT ")
        SQL.Append(" TORIS_CODE_T,TORIF_CODE_T")
        SQL.Append(" FROM TORIMAST")
        '*** �C�� mitsu 2008/10/27 �A�g�敪�e�s�o�Ή� �A�g�敪�𖳎����� ***
        'SQL.Append(" WHERE RENKEI_KBN_T = " & SQ(mIkParam.RENKEI_KBN))
        SQL.Append(" WHERE (RENKEI_KBN_T = " & SQ(mIkParam.RENKEI_KBN))
        SQL.Append("   OR RENKEI_KBN_T = '11')")
        '*******************************************************************
        SQL.Append("   AND MOTIKOMI_KBN_T = '1'")
        SQL.Append("   AND KIGYO_CODE_T = " & SQ(KigyoCode))
        SQL.Append("   AND FURI_CODE_T = " & SQ(FuriCode))
        SQL.Append("   AND " & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & " BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                ToriCode = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")
                Return ToriCode.PadRight(9, " "c)
            End If

        Catch ex As Exception
            Return New String("0"c, 9)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            OraReader = Nothing
        End Try

        Return New String("0"c, 9)
    End Function
    '************************************************************************

End Class
