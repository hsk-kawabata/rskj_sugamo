Imports System
Imports System.IO
Imports System.Text
Imports System.Diagnostics
Imports System.Collections

Imports CAstBatch
Imports CAstSystem
Imports CASTCommon.ModPublic

' �`���A�g����
Public Class ClsDensouRenkei

    ' ���O�����N���X
    Private LOG As CASTCommon.BatchLOG

    Private mDenParam As CAstSystem.ClsDensou.DENSOUPARAM

    ' �N���p�����[�^ ���ʏ��
    Private mArgumentData As CommData

    ' �˗��f�[�^�t�@�C����
    Private mDataFileName As String

    ' �p�u���b�N�c�a
    Private MainDB As CASTCommon.MyOracle

    ' ���b�Z�[�W
    Public Message As String = ""

    ' FSKJ.INI �Z�N�V������
    Private ReadOnly AppTOUROKU As String = "OTHERSYS"

    ' New
    Public Sub New()
    End Sub

    ' �@�\�@ �F ���C��
    '
    ' �߂�l �F 0 - ���� �C 0�ȊO - �ُ�
    '
    ' ���l�@ �F 
    Public Function Main(ByVal command As String) As Integer
        Dim oDenso As New CAstSystem.ClsDensou  ' �Ɩ��w�b�_

        ' �W���u�ʔԐݒ�
        LOG = New CASTCommon.BatchLOG("���V�X�A�g", AppTOUROKU)
        LOG.ToriCode = ""

        ' �Ɩ��w�b�_
        oDenso.GyoumuHeadName = command

        ' �Ɩ��w�b�_�t�@�C���Ǎ�
        Try
            ' �p�����[�^����
            mDenParam.Data = oDenso.ReadHeader
        Catch ex As Exception
            ' �Ɩ��w�b�_�ǂݍ��ݎ��s
            Message = "�`���Ɩ��w�b�_�ǂݍ��� ���s"
            Call LOG.Write("�`���Ɩ��w�b�_�ǂݍ���", "���s", ex.Message & ":" & ex.StackTrace.ToString)
            Return 1
        End Try

        ' �I���N��
        MainDB = New CASTCommon.MyOracle

        ' �N���p�����[�^���ʏ��
        mArgumentData = New CommData(MainDB)

        ' �p�����[�^����ݒ�
        Dim InfoParam As CommData.stPARAMETER
        InfoParam.TORI_CODE = ""
        InfoParam.BAITAI_CODE = ""
        ' ���R�[�h������t�H�[�}�b�g�敪�𔻒肷��
        Select Case CASTCommon.CAInt32(mDenParam.RecordLen)
            Case 120    ' �S��
                InfoParam.FMT_KBN = "00"
            Case 350    ' �n���̂R�T�O
                InfoParam.FMT_KBN = "01"
            Case 390    ' ����
                InfoParam.FMT_KBN = "02"
            Case 130    ' �N��
                InfoParam.FMT_KBN = "03"
            Case 300    ' �n���̂R�O�O
                InfoParam.FMT_KBN = "06"
            Case 165    ' �Z���^�Ԋ҃t�@�C��
                InfoParam.FMT_KBN = "MT"
            Case 210    ' �r�r�b���ʃf�[�^
                InfoParam.FMT_KBN = "SC"
            Case Else
                InfoParam.FMT_KBN = "00"
        End Select

        LOG.Write("���R�[�h���擾", "����", "���R�[�h���F" & mDenParam.RecordLen & " �t�H�[�}�b�g�敪�F" & InfoParam.FMT_KBN)

        InfoParam.FURI_DATE = ""
        InfoParam.CODE_KBN = ""
        InfoParam.FSYORI_KBN = ""
        InfoParam.JOBTUUBAN = 0
        InfoParam.RENKEI_KBN = "00"     ' �`���Œ�
        InfoParam.RENKEI_FILENAME = mDenParam.FileName
        InfoParam.ENC_KBN = ""
        InfoParam.ENC_KEY1 = ""
        InfoParam.ENC_KEY2 = ""
        InfoParam.ENC_OPT1 = ""
        InfoParam.CYCLENO = ""
        mArgumentData.INFOParameter = InfoParam

        ' �A�g�p�N���X�쐬
        Dim oRenkei As New CAstSystem.ClsRenkei(mArgumentData)

        Dim bRet As Boolean
        ' �A�g����
        bRet = MainDensou(oRenkei)
        If bRet = False Then
            ' ���[���o�b�N
            MainDB.Rollback()
        Else
            ' �R�~�b�g
            MainDB.Commit()
        End If

        MainDB.Close()

        If bRet = False Then
            Return 2
        End If
        Return 0

    End Function

    ' �@�\�@ �F �`��
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    ' encode.exe  -I "C:\ZEN01.dat" -l "120" -O "C:\ZEN01.dat.AES"    -a 8          -k "31323334353637383930313233343536" -v "31323334353637383930313233343536" -rl "120" -t 1 -b "120" -g 1 -m 0 -ak 0 -p 0 > "D:\WORK\AES.LOG"
    ' encode.exe  -I "C:\ZEN01.dat" -l "120" -O "C:\ZEN01.dat.�Í���" -a 2 -n "256" -k "3132333435363738"                 -v "3132333435363738"                           -t 0          -g 0                 > "D:\WORK\ENCODE.LOG"
    ' decode.exe  -I "C:\ZEN01.dat.AES"      -O "C:\ZEN01.dat.�Í���,������.DAT"    -k "31323334353637383930313233343536" -v "31323334353637383930313233343536" -lf 0                                        > "D:\WORK\DECODE.LOG"
    '
    Private Function MainDensou(ByVal renkei As ClsRenkei) As Boolean
        LOG.Write("�`���A�g�J�n", "����")

        Try
            Dim DensouPath As String = CASTCommon.GetFSKJIni(AppTOUROKU, "DENSOUREAD").Replace("%yyyyMMdd%", CASTCommon.Calendar.Now.ToString("yyyyMMdd"))
            renkei.InFileName = Path.Combine(DensouPath, mDenParam.FileName)
            LOG.Write("�`���t�@�C���擾", "����", renkei.InFileName)

            ' �`���f�[�^�擾����
            ' �i�n�a�o�^
            If TourokuFile(renkei) = True Then
                LOG.Write("�`���f�[�^�o�^", "����", "���̓t�@�C���F" & renkei.InFileName)
            Else
                If Message = "" Then
                    Message = "�`���f�[�^�o�^ ���s ���̓t�@�C���Ȃ��F" & renkei.InFileName
                    LOG.Write("�`���f�[�^�o�^", "���s", "���̓t�@�C���Ȃ��F" & renkei.InFileName)
                End If
                Return False
            End If
        Catch ex As Exception
            Message = "�`�� ���s"
            Call LOG.Write("�`��", "���s", ex.Message & ":" & ex.StackTrace)
            Return False
        Finally
        End Try

        LOG.Write("�`���A�g�I��", "����")

        Return True
    End Function

    ' �@�\�@ �F �t�@�C���������ǂݍ���łi�n�a�֓o�^����
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F ���f�B�A�R���o�[�^�C�w�Z�C�b�l�s 
    '
    Private Function TourokuFile(ByRef oRenkei As ClsRenkei) As Boolean

        '�}�̓ǂݍ���
        '�t�H�[�}�b�g�@
        Dim oReadFMT As CAstFormat.CFormat
        Dim FSyoriKbn As String = "1"
        Dim FormatKbn As String = mArgumentData.INFOParameter.FMT_KBN
        Dim FuriDate As String

        Try
            ' �t�H�[�}�b�g�敪����C�t�H�[�}�b�g����肷��
            oReadFMT = oReadFMT.GetFormat(mArgumentData.INFOParameter)
            oReadFMT.ToriData = mArgumentData

            ' �`���t�@�C�������[�J���ɃR�s�[����
            Dim sWorkFile As String = oRenkei.CopyToWork()
            If sWorkFile = oRenkei.InFileName Then
                ' �`���t�@�C���Ȃ�
                Return False
            End If
            If File.Exists(sWorkFile) = False Then
                sWorkFile = oRenkei.InFileName
            End If

            If mArgumentData.INFOParameter.FMT_KBN = "SC" Then
                ' �r�r�b���� �i�n�a�o�^
                Call InsertJOBMAST("E207", sWorkFile)
                oReadFMT.Close()
                oReadFMT = Nothing
                Return True
            End If

            ' ���̓t�@�C�����R�s�[����
            If mDenParam.EncodeKubun = "1" Then
                ' ���������ăR�s�[
                oRenkei.InFileName = sWorkFile
                Dim dmmyFile As String = oRenkei.FileDecodeMove(mDenParam.RecordLen, mDenParam.AES, mDenParam.EncodeKey, mDenParam.EncodeVIKey)

                If File.Exists(dmmyFile) = False Then
                    Message = "������ ���s " & oRenkei.Message
                    LOG.Write("������", "���s", "�������Ɏ��s���܂����B" & mDenParam.FileName)
                    Return False
                Else
                    sWorkFile = dmmyFile
                End If
            Else
                ' ���̂܂܎g�p
            End If

            Dim JobID As String
            Dim Param() As String
            ' 2008.04.21 ADD >>
            Dim OutFileName As String = ""
            ' 2008.04.21 ADD <<

            If FormatKbn = "MT" Then
                ' �Z���^�s�\���ׂl�s

                ' �U�֏����敪�C�t�H�[�}�b�g�敪�̃t�H���_�[�ֈړ�����
                Dim oKekkaRenkei As New ClsRenkei(oRenkei.InfoArg, 1)       ' ���ʗp�A�g�N���X 
                'Dim OutFileName As String
                OutFileName = oKekkaRenkei.MoveToGet(sWorkFile)
                LOG.Write("�`���t�@�C���R�s�[", "����", mDenParam.FileName & "->" & OutFileName)

                ' ���ʍX�V�p�p�����[�^
                Param = GetParam(OutFileName)

                ' ���ʍX�V����
                JobID = "K101"

            Else
                ' �˗��f�[�^�C����s�\�f�[�^

                '*** �C�� mitsu 2008/08/25 �ϑ��҃R�[�h�Ǒ֏��� ***
                '�S��t�H�[�}�b�g�̏ꍇ�̂�
                If FormatKbn = "00" AndAlso oRenkei.ConvertItakuCode(sWorkFile, LOG) = False Then
                    Message = oRenkei.Message
                    Return False
                End If
                '**************************************************

                ' �`���t�@�C���������ǂݍ���
                If oReadFMT.FirstRead(sWorkFile) = 1 Then
                    Call oReadFMT.CheckDataFormat()
                    ' 2008.04.21 ADD �ςȃf�[�^�Ή� >>
                    Call oReadFMT.CheckRecord1()
                    ' 2008.04.21 ADD �ςȃf�[�^�Ή� <<

                    ' �U�֓�����
                    FuriDate = oReadFMT.InfoMeisaiMast.FURIKAE_DATE
                    Dim dFuriDate As Date = ConvertDate(FuriDate)
                    Dim d10MaeDate As Date
                    Dim Fmt As New CAstFormat.CFormat
                    Fmt.Oracle = MainDB
                    d10MaeDate = Fmt.GetEigyobiFmt(dFuriDate, -10)
                    If dFuriDate.Subtract(d10MaeDate).Days >= 30 Then
                        ' �������ݏ���
                        JobID = "T101"
                    Else
                        If Not (FuriDate Is Nothing) And CASTCommon.Calendar.Now >= dFuriDate And dFuriDate >= d10MaeDate Then
                            ' ���ʍX�V����
                            JobID = "K101"
                        Else
                            ' �������ݏ���
                            JobID = "T101"
                        End If
                    End If

                    If JobID = "T101" Then
                        Select Case FormatKbn
                            Case "02"
                                ' ���ł̏ꍇ
                                Dim KokuzeiFmt As New CAstFormat.CFormatKokuzei
                                KokuzeiFmt.KOKUZEI_REC1.Data = oReadFMT.RecordData
                                Dim Kamoku As String = KokuzeiFmt.KOKUZEI_REC1.KZ4
                                KokuzeiFmt.Close()
                                Dim TORICODE As String
                                ' �ȖڃR�[�h�@020:�\��������, 300:����ŋy�n�������
                                TORICODE = CASTCommon.GetFSKJIni("TOUROKU", "KOKUZEI" & Kamoku)
                                If TORICODE = "err" Then
                                    TORICODE = ""
                                End If

                                Call oReadFMT.GetTorimastFromToriCode(TORICODE, MainDB)
                                If Not oReadFMT.ToriData Is Nothing Then
                                    oReadFMT.InfoMeisaiMast.ITAKU_CODE = oReadFMT.ToriData.INFOToriMast.ITAKU_CODE_T
                                End If
                            Case "03"
                                ' �N���̏ꍇ

                                Dim NenkinFmt As New CAstFormat.CFormatNenkin
                                NenkinFmt.NENKIN_REC1.Data = oReadFMT.RecordData
                                Dim Syubetu As String = NenkinFmt.NENKIN_REC1.NK2
                                NenkinFmt.Close()

                                Dim TORICODE As String
                                ' �N����ʂ��画�f 61:�������N��,62:���D���N��,63:�������N��,64:�J�ДN��,65:�V�����N���E�����N��,66:�V�D���N��,67:�������N���Z��
                                TORICODE = CASTCommon.GetFSKJIni("TOUROKU", "NENKIN" & Syubetu)
                                If TORICODE = "err" Then
                                    TORICODE = ""
                                    LOG.Write("�N���@�����R�[�h�擾", "���s", "�ȖځF" & Syubetu)
                                Else
                                    LOG.ToriCode = TORICODE
                                    LOG.Write("�N���@�����R�[�h�擾", "����", "�ȖځF" & Syubetu)
                                End If

                                Call oReadFMT.GetTorimastFromToriCode(TORICODE, MainDB)
                                If Not oReadFMT.ToriData Is Nothing Then
                                    oReadFMT.InfoMeisaiMast.ITAKU_CODE = oReadFMT.ToriData.INFOToriMast.ITAKU_CODE_T
                                End If
                            Case Else
                                ' �ϑ��҃R�[�h����C�����}�X�^�����擾����
                                Call oReadFMT.GetTorimastFromItakuCode(MainDB)
                        End Select
                        'Call oReadFMT.GetTorimastFromItakuCode(MainDB)
                        FSyoriKbn = oReadFMT.ToriData.INFOToriMast.FSYORI_KBN_T
                        FormatKbn = oReadFMT.ToriData.INFOToriMast.FMT_KBN_T
                    Else
                        ' ���ʍX�V�@����
                        ' �����ɃR�[�h
                        Dim Jikinko As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
                        If oReadFMT.InfoMeisaiMast.ITAKU_KIN = Jikinko Then
                            ' SSS�ϑ��Ҍ���
                            If oReadFMT.GetTorimastFromItakuCodeSSS(MainDB) = False Then
                                ' ����悪������Ȃ��̂ŁC�������ݏ��������s
                                JobID = "T101"
                                Call oReadFMT.GetTorimastFromItakuCode(MainDB)
                            End If
                        Else
                            If oReadFMT.GetTorimastFromItakuCodeTAKO(MainDB) = False Then
                                ' ����悪������Ȃ��̂ŁC�������ݏ��������s
                                JobID = "T101"
                                Call oReadFMT.GetTorimastFromItakuCode(MainDB)
                            End If
                        End If
                        FSyoriKbn = oReadFMT.ToriData.INFOToriMast.FSYORI_KBN_T
                        FormatKbn = oReadFMT.ToriData.INFOToriMast.FMT_KBN_T
                    End If

                Else
                    ' �������ݏ���
                    JobID = "T101"
                End If
                oReadFMT.Close()

                Dim InfoParam As CommData.stPARAMETER = mArgumentData.INFOParameter
                InfoParam.FSYORI_KBN = FSyoriKbn
                InfoParam.FMT_KBN = FormatKbn
                mArgumentData.INFOParameter = InfoParam

                ' �i�n�a�}�X�^����o�^
                If JobID = "T101" Then
                    ' �U�֏����敪�C�t�H�[�}�b�g�敪�̃t�H���_�[�ֈړ�����
                    ' 2008.04.21 DELETE >>
                    'Dim OutFileName As String
                    ' 2008.04.21 DELETE <<
                    OutFileName = oRenkei.MoveToGet(sWorkFile)
                    LOG.Write("�`���t�@�C���R�s�[", "����", mDenParam.FileName & "->" & OutFileName)

                    ' �������ݗp�p�����[�^
                    Param = GetParam(oReadFMT, OutFileName)
                Else
                    Dim oKekkaRenkei As New ClsRenkei(oRenkei.InfoArg, 1)       ' ���ʗp�A�g�N���X
                    ' 2008.04.21 DELETE >>
                    'Dim OutFileName As String
                    ' 2008.04.21 DELETE <<
                    OutFileName = oKekkaRenkei.MoveToGet(sWorkFile)
                    LOG.Write("�`���t�@�C���R�s�[", "����", sWorkFile & "->" & OutFileName)

                    ' ���ʍX�V�p�p�����[�^
                    Param = GetParamZenginKekka(oReadFMT, OutFileName)
                End If

                ' 2008.04.21 ADD �������擾���s���� >>
                If Param.Length >= 1 And Param(0).StartsWith("000000000") = True Then
                    If File.Exists(OutFileName) = True Then
                        File.Delete(OutFileName)
                    End If
                    OutFileName = oRenkei.InFileName

                    ' �������擾���s
                    Message = "�������擾���s �ϑ��҃R�[�h�F" & oReadFMT.InfoMeisaiMast.ITAKU_CODE & " �t�@�C�����F" & OutFileName
                    LOG.Write("�������擾", "���s", "�ϑ��҃R�[�h�F" & oReadFMT.InfoMeisaiMast.ITAKU_CODE & " �t�@�C����:" & OutFileName)

                    Dim Prn As New ClsPrnSyorikekkaKakuninhyo
                    If Prn.OutputCSVKekkaSysError(oReadFMT.InfoMeisaiMast.ITAKU_CODE, mArgumentData.INFOParameter.FSYORI_KBN, OutFileName, MainDB) = False Then
                        LOG.Write("�������ʊm�F�\", "���s", "�t�@�C����:" & Prn.FileName & " ���b�Z�[�W:" & Prn.ReportMessage)
                    End If
                    Prn = Nothing
                    oReadFMT.Close()
                    Return False
                End If
                ' 2008.04.21 ADD �������擾���s���� <<
            End If

            If InsertJOBMAST(Param, JobID) = True Then
                If (oReadFMT.InfoMeisaiMast.ITAKU_CODE Is Nothing) Then
                    LOG.Write("�˗��f�[�^�ǂݎ��", "����")
                Else
                    LOG.Write("�˗��f�[�^�ǂݎ��", "����", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                End If
            Else
                Message = "�˗��f�[�^�ǂݎ�� ���s �ϑ��҃R�[�h�F" & oReadFMT.InfoMeisaiMast.ITAKU_CODE
                LOG.Write("�˗��f�[�^�ǂݎ��", "���s", oReadFMT.InfoMeisaiMast.ITAKU_CODE)
                Return False
            End If
            oReadFMT = Nothing
        Catch ex As Exception
            MainDB.Rollback()

            If Not oReadFMT Is Nothing Then
                oReadFMT.Close()
            End If
            Message = "�˗����f�[�^�W���u�o�^ ���s"
            LOG.Write("�˗��f�[�^�W���u�o�^", "���s", ex.Message & ":" & ex.StackTrace)
            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F �i�n�a�l�`�r�s�쐬
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function InsertJOBMAST(ByVal param() As String, Optional ByVal jobId As String = "T101") As Boolean
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
        SQL.Append(",DENSO_CNT_CODE_J")
        SQL.Append(",TOHO_CNT_CODE_J")
        SQL.Append(",ZENGIN_F_NAME_J")
        SQL.Append(",DENSO_F_NAME_J")
        SQL.Append(",DENSO_TIME_STAMP_J")
        SQL.Append(") VALUES (")
        SQL.Append(" TO_CHAR(SYSDATE,'YYYYMMDD')")                          ' TOUROKU_DATE_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' TOUROKU_TIME_J
        SQL.Append(",'00000000'")                                           ' STA_DATE_J
        SQL.Append(",'000000'")                                             ' STA_TIME_J
        SQL.Append(",'00000000'")                                           ' END_DATE_J
        SQL.Append(",'000000'")                                             ' END_TIME_J
        SQL.Append("," & SQ(jobId))                                         ' JOBID_J
        SQL.Append(",'0'")                                                  ' STATUS_J
        SQL.Append("," & SQ(LOG.UserID))                                      ' USERID_J
        Dim work() As String
        If param.Length = 18 Then
            ' ���Ƃ����ݗp
            '*** �C�� mitsu 2008/09/30 ���������� ***
            'work = CType(Array.CreateInstance(GetType(String), 13), String())
            work = New String(12) {}
            '****************************************
            Array.Copy(param, work, 13)
        Else
            ' ���ʍX�V�p
            '*** �C�� mitsu 2008/09/30 ���������� ***
            'work = CType(Array.CreateInstance(GetType(String), param.Length - 6), String())
            work = New String(param.Length - 7) {}
            '****************************************
            Array.Copy(param, work, param.Length - 6)
        End If
        Dim sJoin As String = String.Join(",", work)
        SQL.Append("," & SQ(sJoin))                                         ' PARAMETA_J
        SQL.Append(",' '")                                                  ' ERRMSG_J
        SQL.Append("," & SQ(param(param.Length - 5)))                       ' DENSO_CNT_CODE_J
        SQL.Append("," & SQ(param(param.Length - 4)))                       ' TOHO_CNT_CODE_J
        SQL.Append("," & SQ(param(param.Length - 3)))                       ' ZENGIN_F_NAME_J
        SQL.Append("," & SQ(param(param.Length - 2)))                       ' DENSO_F_NAME_J
        SQL.Append("," & SQ(param(param.Length - 1)))                       ' DENSO_TIME_STAMP_S
        SQL.Append(")")

        '*** �C�� mitsu 2008/08/04 ���s�����g���C���� ***
        'If MainDB.ExecuteNonQuery(SQL) <= 0 Then
        '    Message = "JOBMAST�o�^ ���s"
        '    LOG.Write("JOBMAST�o�^", "���s", MainDB.Message)
        '    Return False
        'End If
        Dim cnt As Integer = 0
        While True
            Try
                If MainDB.ExecuteNonQuery(SQL) <= 0 Then
                    Message = "JOBMAST�o�^ ���s"
                    LOG.Write("JOBMAST�o�^", "���s", MainDB.Message)
                    Return False
                Else
                    '�������͔�����
                    Exit While
                End If

            Catch ex As Exception
                cnt += 1
                '3��ȏ㎸�s���͋����I��
                If cnt >= 3 Then
                    Message = "JOBMAST�o�^ ���s " & ex.Message
                    LOG.Write("JOBMAST�o�^", "���s", MainDB.Message)
                    Return False
                End If
                '0.5�b�ҋ@
                Threading.Thread.Sleep(500)
            End Try
        End While
        '************************************************

        Return True
    End Function

    ' �@�\�@ �F �i�n�a�l�`�r�s�쐬
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function InsertJOBMAST(ByVal jobId As String, ByVal fileName As String) As Boolean
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
        SQL.Append(",DENSO_CNT_CODE_J")
        SQL.Append(",TOHO_CNT_CODE_J")
        SQL.Append(",ZENGIN_F_NAME_J")
        SQL.Append(",DENSO_F_NAME_J")
        SQL.Append(",DENSO_TIME_STAMP_J")
        SQL.Append(") VALUES (")
        SQL.Append(" TO_CHAR(SYSDATE,'YYYYMMDD')")                          ' TOUROKU_DATE_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' TOUROKU_TIME_J
        SQL.Append(",'00000000'")                                           ' STA_DATE_J
        SQL.Append(",'000000'")                                             ' STA_TIME_J
        SQL.Append(",'00000000'")                                           ' END_DATE_J
        SQL.Append(",'000000'")                                             ' END_TIME_J
        SQL.Append("," & SQ(jobId))                                         ' JOBID_J
        SQL.Append(",'0'")                                                  ' STATUS_J
        SQL.Append("," & SQ(LOG.UserID))                                      ' USERID_J
        SQL.Append("," & SQ("""" & fileName & """"))                        ' PARAMETA_J
        SQL.Append(",' '")                                                  ' ERRMSG_J
        SQL.Append("," & SQ(mDenParam.CenterCode))                          ' DENSO_CNT_CODE_J
        SQL.Append("," & SQ(mDenParam.TouhouCode))                          ' TOHO_CNT_CODE_zJ
        SQL.Append("," & SQ(mDenParam.ZenginName))                          ' ZENGIN_F_NAME_J
        SQL.Append("," & SQ(mDenParam.FileName))                            ' DENSO_F_NAME_J
        SQL.Append("," & SQ(mDenParam.DensouNitiji))                        ' DENSO_TIME_STAMP_S
        SQL.Append(")")

        '*** �C�� mitsu 2008/08/04 ���s�����g���C���� ***
        'If MainDB.ExecuteNonQuery(SQL) <= 0 Then
        '    Message = "JOBMAST�o�^ ���s"
        '    LOG.Write("JOBMAST�o�^", "���s", MainDB.Message)
        '    Return False
        'End If
        Dim cnt As Integer = 0
        While True
            Try
                If MainDB.ExecuteNonQuery(SQL) <= 0 Then
                    Message = "JOBMAST�o�^ ���s"
                    LOG.Write("JOBMAST�o�^", "���s", MainDB.Message)
                    Return False
                Else
                    '�������͔�����
                    Exit While
                End If

            Catch ex As Exception
                cnt += 1
                '3��ȏ㎸�s���͋����I��
                If cnt >= 3 Then
                    Message = "JOBMAST�o�^ ���s " & ex.Message
                    LOG.Write("JOBMAST�o�^", "���s", MainDB.Message)
                    Return False
                End If
                '0.5�b�ҋ@
                Threading.Thread.Sleep(500)
            End Try
        End While
        '************************************************

        Return True
    End Function

    ' �@�\�@ �F ���ʍX�V�p�p�����[�^�쐬�i�Z���^�s�\���ʁj
    '
    ' �߂�l �F �p�����[�^
    '
    ' ���l�@ �F 
    '
    Private Function GetParam(ByVal filename As String) As String()
        Dim Param As New ArrayList

        ' �t�H�[�}�b�g�敪
        Param.Add(mArgumentData.INFOParameter.FMT_KBN)

        ' �A�g�敪
        Param.Add(mArgumentData.INFOParameter.RENKEI_KBN)

        ' �X�V�L�[�F��ƃV�[�P���X
        Param.Add("0")

        ' �A�g�t�@�C����
        Param.Add(Path.GetFileName(filename).TrimEnd)

        ' ----- ��L�܂ł��p�����[�^

        '' �T�C�N����
        Param.Add(mDenParam.HostTuuban)

        ' ����Z���^�m�F�R�[�h
        Param.Add(mDenParam.CenterCode)

        ' �����Z���^�[�m�F�R�[�h
        Param.Add(mDenParam.TouhouCode)

        ' �S��t�@�C����
        Param.Add(mDenParam.ZenginName)

        ' �`���t�@�C����
        Param.Add(mDenParam.FileName)

        ' �`������
        Param.Add(mDenParam.DensouNitiji)

        Return CType(Param.ToArray(GetType(String)), String())
    End Function

    ' �@�\�@ �F ���ʍX�V�p�p�����[�^�쐬�i���s�C�r�r�r�j
    '
    ' �߂�l �F �p�����[�^
    '
    ' ���l�@ �F 
    '
    Private Function GetParamZenginKekka(ByVal readFmt As CAstFormat.CFormat, ByVal filename As String) As String()
        Dim Param As New ArrayList

        ' �����R�[�h
        If Not readFmt.ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            Param.Add(readFmt.ToriData.INFOToriMast.TORIS_CODE_T & readFmt.ToriData.INFOToriMast.TORIF_CODE_T)
        Else
            Param.Add(New String("0"c, 10))
        End If

        ' �U�֓�
        Param.Add(readFmt.InfoMeisaiMast.FURIKAE_DATE)

        If Not readFmt.ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            ' �t�H�[�}�b�g�敪
            Param.Add(readFmt.ToriData.INFOToriMast.FMT_KBN_T)
        Else
            Param.Add("")
        End If

        ' �A�g�敪
        Param.Add(mArgumentData.INFOParameter.RENKEI_KBN)

        ' �X�V�L�[�F��ƃV�[�P���X
        Param.Add("0")

        ' �A�g�t�@�C����
        Param.Add(Path.GetFileName(filename).TrimEnd)

        ' ----- ��L�܂ł��p�����[�^

        '' �T�C�N����
        Param.Add(mDenParam.HostTuuban)

        ' ����Z���^�m�F�R�[�h
        Param.Add(mDenParam.CenterCode)

        ' �����Z���^�[�m�F�R�[�h
        Param.Add(mDenParam.TouhouCode)

        ' �S��t�@�C����
        Param.Add(mDenParam.ZenginName)

        ' �`���t�@�C����
        Param.Add(mDenParam.FileName)

        ' �`������
        Param.Add(mDenParam.DensouNitiji)

        Return CType(Param.ToArray(GetType(String)), String())
    End Function

    ' �@�\�@ �F ���Ƃ����ݗp�p�����[�^�쐬
    '
    ' �߂�l �F �p�����[�^
    '
    ' ���l�@ �F 
    '
    Private Function GetParam(ByVal readFmt As CAstFormat.CFormat, ByVal filename As String) As String()
        Dim Param As New ArrayList

        ' �ϑ��҃R�[�h����C�����}�X�^�����擾����
        Call readFmt.GetTorimastFromItakuCode(MainDB)

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

            ' �t�H�[�}�b�g�敪
            Param.Add(readFmt.ToriData.INFOToriMast.FMT_KBN_T)

            ' �}�̃R�[�h
            Param.Add(readFmt.ToriData.INFOToriMast.BAITAI_CODE_T)
        Else
            Param.Add("")
            Param.Add("")
            Param.Add("")
        End If

        ' ���x���敪
        Param.Add("0")

        ' �A�g�敪
        Param.Add(mArgumentData.INFOParameter.RENKEI_KBN)

        ' �A�g�t�@�C����
        Param.Add(Path.GetFileName(filename).TrimEnd)

        ' �Í����敪
        Param.Add(mDenParam.EncodeKubun)
        If mDenParam.EncodeKubun = "1" Then
            ' �Í����L�[
            Param.Add(mDenParam.EncodeKey.TrimEnd)
            ' �Í���IV�L�[
            Param.Add(mDenParam.EncodeVIKey.TrimEnd)
            ' �`�d�r
            Param.Add(mDenParam.AES.TrimEnd)
        Else
            ' �Í����L�[
            Param.Add("")
            ' �Í���IV�L�[
            Param.Add("")
            ' �`�d�r
            Param.Add("")
        End If

        '' �T�C�N����
        Param.Add(mDenParam.HostTuuban)

        ' ����Z���^�m�F�R�[�h
        Param.Add(mDenParam.CenterCode)

        ' �����Z���^�[�m�F�R�[�h
        Param.Add(mDenParam.TouhouCode)

        ' �S��t�@�C����
        Param.Add(mDenParam.ZenginName)

        ' �`���t�@�C����
        Param.Add(mDenParam.FileName)

        ' �`������
        Param.Add(mDenParam.DensouNitiji)

        Return CType(Param.ToArray(GetType(String)), String())
    End Function

    ' �@�\�@ �F �i�n�a�l�`�r�s�o�^
    '
    ' �߂�l �F ARG1 - �p�����[�^
    '           ARG2 - ���b�Z�[�W
    '
    ' ���l�@ �F 
    '
    Public Function InsertJOBMASTbyError(ByVal command As String) As Boolean
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
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' STA_DATE_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' STA_TIME_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' END_DATE_J
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' END_TIME_J
        SQL.Append(",'T102'")                                               ' JOBID_J
        SQL.Append(",'3'")          ' �ُ�I��                              ' STATUS_J
        SQL.Append("," & SQ("SYSTEM"))                                      ' USERID_J
        SQL.Append("," & SQ(command))                                       ' PARAMETA_J
        SQL.Append("," & SQ(Message))                                       ' ERRMSG_J
        SQL.Append(",' '")                                                  ' DENSO_F_NAME_J
        SQL.Append(",TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')")                  ' DENSO_TIME_STAMP_J
        SQL.Append(")")
        Dim DB As New CASTCommon.MyOracle
        If DB.ExecuteNonQuery(SQL) <= 0 Then
            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("���V�X�A�g�@���s", Diagnostics.EventLogEntryType.Error)
            Return False
        End If
        DB.Commit()
        DB.Close()

        Return True
    End Function
End Class
