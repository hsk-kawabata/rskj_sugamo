Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' �r�r�r���s�f�[�^�쐬�N���X
''' </summary>
''' <remarks>2017/01/16 saitou ���t�M��(RSV2) added for �X���[�G�X�Ή�</remarks>
Public Class ClsSSS

#Region "�N���X�ϐ�"

    '���O
    Private LOG As New CASTCommon.BatchLOG("KFJ020", "���s�f�[�^�쐬(SSS)")

    '�ݒ�t�@�C��
    Private Structure strcIniInfo
        Dim DATBK As String
        Dim DEN As String
        Dim FTR As String
        Dim FTRANP As String
        Dim KINKOCD As String
        Dim SSS_SOUFU As String
        Dim SSS_SOUFU_TEL As String
        Dim SSS_SOUFU_FAX As String
        '2017/01/19 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
        Dim FUNOU_SSS_1 As String
        Dim FUNOU_SSS_2 As String
        '2017/01/19 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END
        '2017/02/27 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
        Dim SSS_ITAKUCODE_PATN As String
        '2017/02/27 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END
    End Structure
    Private IniInfo As strcIniInfo

    '��������
    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    Private MainDB As CASTCommon.MyOracle

    Private strOUT_FILE As String
    Private strSOUSIN_FILE As String
    Private strJIFURI_SEQ As String

    '�U�֓�
    Public Property FURI_DATE As String
        Get
            Return strFURI_DATE
        End Get
        Set(value As String)
            strFURI_DATE = value
        End Set
    End Property
    Private strFURI_DATE As String

    Private bDataExistsFlg As Boolean = False

    '2017/01/19 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
    Private FmtComm As New CAstFormat.CFormat
    '2017/01/19 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END

#End Region

#Region "�p�u���b�N���\�b�h"

    ''' <summary>
    ''' �r�r�r���s�f�[�^�쐬���C������
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Public Function fn_CREATE_DATA_MAIN() As Boolean
        LOG.ToriCode = m_TAKOU.strTORI_CODE
        LOG.FuriDate = m_TAKOU.strFURI_DATE
        LOG.JobTuuban = CASTCommon.CAInt32(m_TAKOU.strTUUBAN)
        LOG.Write(LOG.ToriCode, LOG.FuriDate, "(SSS�f�[�^�쐬���C������)�J�n", "����")

        '------------------------------------------------
        '�ݒ�t�@�C���ǂݍ���
        '------------------------------------------------
        If fn_INI_READ() = False Then
            Return False
        End If

        '------------------------------------------------
        '�c�a�ڑ� �g�����U�N�V�����̊J�n
        '------------------------------------------------
        MainDB = New CASTCommon.MyOracle
        MainDB.BeginTrans()
        '2017/01/19 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
        FmtComm.Oracle = MainDB
        '2017/01/19 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END

        Try
            '------------------------------------------------
            '���M�t�@�C���̍쐬(���[�N�t�@�C��)
            '------------------------------------------------
            '�����O�Ƀ��[�N�t�@�C���폜
            For i As Integer = 1 To 2
                If File.Exists(Path.Combine(IniInfo.DATBK, "SYUKIN_DAIKOU" & i.ToString & ".DAT")) = True Then
                    File.Delete(Path.Combine(IniInfo.DATBK, "SYUKIN_DAIKOU" & i.ToString & ".DAT"))
                End If
            Next

            If fn_CREATE_FILE() = False Then
                MainDB.Rollback()
                MainDB.Close()
                Return False
            End If

            '------------------------------------------------
            '���M�t�@�C���̃R�[�h�ϊ�
            '------------------------------------------------
            '��g���ƒ�g�O�ƂŃt�@�C�����قȂ邽�߁A�R�[�h�ϊ���2��s��
            For i As Integer = 1 To 2
                Me.strOUT_FILE = Path.Combine(IniInfo.DATBK, "SYUKIN_DAIKOU" & i.ToString & ".DAT")
                If File.Exists(Me.strOUT_FILE) = True Then
                    Me.strSOUSIN_FILE = Path.Combine(IniInfo.DEN, "SYUKIN_DAIKOU" & i.ToString & ".DAT")
                    If File.Exists(Me.strSOUSIN_FILE) = True Then
                        File.Delete(Me.strSOUSIN_FILE)
                    End If

                    Dim strPFilePath As String = Path.Combine(IniInfo.FTR, "120.P")
                    Dim intKEKKA As Integer = ConvertFileFtranP("PUTRAND", strOUT_FILE, strSOUSIN_FILE, strPFilePath)
                    Select Case intKEKKA
                        Case 0
                            LOG.Write("(�R�[�h�ϊ�)", "����")
                        Case 100
                            LOG.Write("(�R�[�h�ϊ�)", "���s")
                            LOG.UpdateJOBMASTbyErr("�R�[�h�ϊ����s")
                            Return False
                    End Select
                End If
            Next

            '------------------------------------------------
            '���t�[�̈��
            '------------------------------------------------
            If bDataExistsFlg = True Then
                If fn_PRINT_SOUFUHYOU() = False Then
                    LOG.UpdateJOBMASTbyErr("���t�[�o�͎��s")
                    Return False
                End If
            Else
                LOG.Write("(���t�[���)", "����", "����ΏۂȂ�")
            End If

            '------------------------------------------------
            '�c�a�̊J��
            '------------------------------------------------
            MainDB.Commit()
            MainDB = Nothing

            Return True

        Catch ex As Exception
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "(SSS�f�[�^�쐬���C������)", "���s", ex.Message)
            LOG.UpdateJOBMASTbyErr("SSS�f�[�^�쐬�@��O")
            MainDB.Rollback()
            Return False
        Finally
            '�Ō�܂Ŏc���Ă����烍�[���o�b�N
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "(SSS�f�[�^�쐬���C������)�I��", "����")
        End Try
    End Function

    ''' <summary>
    ''' �z�M�҂��̃t���O��S�Č��ɖ߂�
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReturnFlg() As Integer
        Dim SQL As New StringBuilder
        Dim Ret As Integer = 0
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Try
            '�O�̂��߁A��[�N���[�Y
            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If
            Me.MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)
            If OraReader.DataReader(Me.GetSSSFileCreateSQL) = True Then
                While OraReader.EOF = False
                    SQL.Length = 0
                    SQL.Append("UPDATE SCHMAST SET")
                    '2017/01/17 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                    '�W����(�X���[�G�X���ϖ���)�Ŏ���
                    SQL.Append(" TAKOU_FLG_S = '0'")
                    'If OraReader.GetString("SORTKEY") = "1" Then
                    '    SQL.Append(" HAISIN_T1FLG_S = '0'")
                    'Else
                    '    SQL.Append(" HAISIN_T2FLG_S = '0'")
                    'End If
                    '2017/01/17 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
                    SQL.Append(" WHERE TORIS_CODE_S = " & SQ(OraReader.GetString("TORIS_CODE_S")))
                    SQL.Append(" AND TORIF_CODE_S = " & SQ(OraReader.GetString("TORIF_CODE_S")))
                    SQL.Append(" AND FURI_DATE_S = " & SQ(OraReader.GetString("FURI_DATE_S")))

                    Ret = Me.MainDB.ExecuteNonQuery(SQL)
                    LOG.Write("�z�M�҂����", "����", Ret & "��")

                    OraReader.NextRead()
                End While
            End If

            Me.MainDB.Commit()
            Return 0

        Catch ex As Exception
            MainDB.Rollback()
            LOG.Write("�z�M�҂����", "���s", ex.ToString)
            Return -1
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If
        End Try
    End Function

#End Region

#Region "�v���C�x�[�g���\�b�h"

    ''' <summary>
    ''' SSS���[�N�t�@�C�����쐬���܂��B
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_CREATE_FILE() As Boolean

        Dim oraSchReader As CASTCommon.MyOracleReader = Nothing
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        Dim ZenStream As StreamWriter = Nothing
        Dim ZenFmt As New CAstFormat.CFormatZengin

        Dim dblFURI_KEN As Double
        Dim dblFURI_KIN As Double
        Dim bDataRecordExistsFlg As Boolean = False
        Dim bMeisaiExistsFlg As Boolean = False

        Dim OldSortKey As String = ""
        Dim bTeikeinaiOpenFlg As Boolean = False
        Dim bTeikeigaiOpenFlg As Boolean = False

        Try
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "(SSS�f�[�^�쐬)�J�n", "����")

            '------------------------------------------------
            '�ΏۃX�P�W���[���̌���
            '------------------------------------------------
            oraSchReader = New CASTCommon.MyOracleReader(MainDB)
            If oraSchReader.DataReader(GetSSSFileCreateSQL) = True Then

                '���חp�̃��[�_�[�I�[�v��
                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)

                While oraSchReader.EOF = False

                    '�f�[�^���R�[�h���݃t���O������
                    bDataRecordExistsFlg = False

                    '�w�b�_���R�[�h�̎擾
                    If oraMeiReader.DataReader(GetMeimastSQL("1", oraSchReader)) = True Then
                        '2017/02/27 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                        '�w�b�_�̈ϑ��҃R�[�h�̃p�^�[����ݒ肷��B
                        Select Case IniInfo.SSS_ITAKUCODE_PATN
                            Case "0"
                                '0:�U�ֈ˗��f�[�^�ϑ��҃R�[�h�����̂܂܎g�p
                                ZenFmt.ZENGIN_REC1.Data = oraMeiReader.GetItem("FURI_DATA_K")
                            Case "1"
                                '1:�ϑ��҃R�[�h�̉�1���ɒ�g��ʂ�ݒ肷��
                                ZenFmt.ZENGIN_REC1.Data = oraMeiReader.GetItem("FURI_DATA_K")
                                ZenFmt.ZENGIN_REC1.ZG4 = ZenFmt.ZENGIN_REC1.ZG4.Substring(0, 9) & oraSchReader.GetString("SORTKEY")
                            Case "2"
                                '2:����("03" + �����ɃR�[�h��3�� + �ϑ��҃R�[�h��4�� + ��g���)
                                ZenFmt.ZENGIN_REC1.Data = oraMeiReader.GetItem("FURI_DATA_K")
                                ZenFmt.ZENGIN_REC1.ZG4 = "03" & IniInfo.KINKOCD.Substring(1, 3) & ZenFmt.ZENGIN_REC1.ZG4.Substring(6, 4) & oraSchReader.GetString("SORTKEY")
                            Case Else
                                '��L�ȊO�͐U�փf�[�^���g�p����
                                ZenFmt.ZENGIN_REC1.Data = oraMeiReader.GetItem("FURI_DATA_K")
                        End Select
                        'ZenFmt.ZENGIN_REC1.Data = oraMeiReader.GetItem("FURI_DATA_K")
                        '2017/02/27 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
                    Else
                        LOG.Write("(SSS�f�[�^�쐬)", "���s", "�w�b�_���R�[�h�擾���s")
                        LOG.UpdateJOBMASTbyErr("SSS�f�[�^�쐬���s �w�b�_���R�[�h�擾���s")
                        Return False
                    End If

                    oraMeiReader.Close()

                    '�V�[�P���X�ԍ��̎擾
                    strJIFURI_SEQ = fn_SELECT_SEQ(oraSchReader.GetString("FURI_DATE_S"))

                    '�f�[�^���R�[�h�̏�������
                    If oraMeiReader.DataReader(GetMeimastSQL("2", oraSchReader)) = True Then
                        '�����Ƌ��z������
                        dblFURI_KEN = 0
                        dblFURI_KIN = 0
                        '�f�[�^���R�[�h���݃t���O�X�V
                        bDataRecordExistsFlg = True
                        '���ב��݃t���O�X�V
                        bMeisaiExistsFlg = True

                        If oraSchReader.GetString("SORTKEY") = "1" AndAlso bTeikeinaiOpenFlg = False Then
                            '��g���̃f�[�^
                            Dim strTeikeinaiWorkFileName As String = Path.Combine(IniInfo.DATBK, "SYUKIN_DAIKOU1.DAT")
                            If File.Exists(strTeikeinaiWorkFileName) = True Then
                                File.Delete(strTeikeinaiWorkFileName)
                            End If

                            ZenStream = New StreamWriter(strTeikeinaiWorkFileName, False, Encoding.GetEncoding("SHIFT-JIS"))
                            bTeikeinaiOpenFlg = True
                        ElseIf oraSchReader.GetString("SORTKEY") = "2" AndAlso bTeikeigaiOpenFlg = False Then
                            '��g�O�̃f�[�^
                            If bTeikeinaiOpenFlg = True Then
                                '���łɒ�g���̃f�[�^���쐬���Ă���ꍇ�̓G���h���R�[�h��������ŃN���[�Y
                                ZenFmt.ZENGIN_REC9.ZG1 = "9"
                                ZenStream.Write(ZenFmt.ZENGIN_REC9.Data)
                                If Not ZenStream Is Nothing Then
                                    ZenStream.Close()
                                    ZenStream = Nothing
                                End If
                            End If

                            Dim strTeikeigaiWorkFileName As String = Path.Combine(IniInfo.DATBK, "SYUKIN_DAIKOU2.DAT")
                            If File.Exists(strTeikeigaiWorkFileName) = True Then
                                File.Delete(strTeikeigaiWorkFileName)
                            End If

                            ZenStream = New StreamWriter(strTeikeigaiWorkFileName, False, Encoding.GetEncoding("SHIFT-JIS"))
                            bTeikeigaiOpenFlg = True
                        End If

                        '�w�b�_���R�[�h�̏�������
                        ZenStream.Write(ZenFmt.ZENGIN_REC1.Data)


                        While oraMeiReader.EOF = False
                            dblFURI_KEN += 1
                            dblFURI_KIN += oraMeiReader.GetInt64("FURIKIN_K")
                            ZenFmt.ZENGIN_REC2.Data = oraMeiReader.GetItem("FURI_DATA_K")

                            If oraMeiReader.GetString("TEKIYO_KBN_K") <> "1" Then
                                If oraMeiReader.GetString("KTEKIYO_K") = String.Empty Then
                                    '�X�ܖ��͋�
                                    ZenFmt.ZENGIN_REC2.ZG5 = String.Empty
                                Else
                                    '�X�ܖ��ɃJ�i�E�v
                                    ZenFmt.ZENGIN_REC2.ZG5 = oraMeiReader.GetString("KTEKIYO_K").PadRight(15, " "c)
                                End If
                            Else
                                '�X�ܖ��͋�
                                ZenFmt.ZENGIN_REC2.ZG5 = String.Empty
                            End If

                            '�ڋq�ԍ�
                            Dim strKokyakuNo As String = String.Concat(New String() {oraSchReader.GetString("ITAKU_CODE_T").Substring(5, 4), "0", IniInfo.KINKOCD, "00000000000"})
                            ZenFmt.ZENGIN_REC2.ZG12 = strKokyakuNo.Substring(0, 10)
                            ZenFmt.ZENGIN_REC2.ZG13 = strKokyakuNo.Substring(10, 10)

                            '�_�~�[��(�V�[�P���X)
                            ZenFmt.ZENGIN_REC2.ZG15 = strJIFURI_SEQ

                            ZenStream.Write(ZenFmt.ZENGIN_REC2.Data)


                            '------------------------------------------------
                            '���׃}�X�^�̃V�[�P���X�ԍ��̍X�V
                            '------------------------------------------------
                            If UpdateMeimastJifuriSEQ(oraMeiReader) = False Then
                                Return False
                            End If

                            strJIFURI_SEQ = CStr(CLng(strJIFURI_SEQ) + 1)

                            oraMeiReader.NextRead()
                        End While
                    End If

                    oraMeiReader.Close()


                    '�g���[�����R�[�h�̏�������
                    If bDataRecordExistsFlg = True Then
                        ZenFmt.ZENGIN_REC8.ZG1 = "8"
                        ZenFmt.ZENGIN_REC8.ZG2 = dblFURI_KEN.ToString.PadLeft(6, "0"c)
                        ZenFmt.ZENGIN_REC8.ZG3 = dblFURI_KIN.ToString.PadLeft(12, "0"c)
                        ZenFmt.ZENGIN_REC8.ZG4 = "".PadLeft(6, "0"c)
                        ZenFmt.ZENGIN_REC8.ZG5 = "".PadLeft(12, "0"c)
                        ZenFmt.ZENGIN_REC8.ZG6 = "".PadLeft(6, "0"c)
                        ZenFmt.ZENGIN_REC8.ZG7 = "".PadLeft(12, "0"c)

                        ZenStream.Write(ZenFmt.ZENGIN_REC8.Data)
                    End If

                    '���s�X�P�W���[���}�X�^�쐬
                    If Me.fn_INSERT_TAKOSCHMAST(oraSchReader) = False Then
                        Return False
                    End If

                    '------------------------------------------------
                    '�X�P�W���[���}�X�^�X�V
                    '�����s���f�[�^���쐬���Ă��Ȃ��Ă��X�V
                    '------------------------------------------------
                    If fn_UPDATE_SCHMAST(oraSchReader) = False Then
                        Return False
                    End If

                    oraSchReader.NextRead()
                End While

            Else
                '�X�P�W���[���Ȃ�
                LOG.Write("(SSS�f�[�^�쐬)", "���s", "�ΏۃX�P�W���[���Ȃ�")
                LOG.UpdateJOBMASTbyErr("SSS�f�[�^�쐬���s �ΏۃX�P�W���[���Ȃ�")
                Return False
            End If

            If bMeisaiExistsFlg = True Then
                '�G���h���R�[�h�̏�������
                ZenFmt.ZENGIN_REC9.ZG1 = "9"
                ZenStream.Write(ZenFmt.ZENGIN_REC9.Data)

                bDataExistsFlg = True
            End If

            If Not ZenStream Is Nothing Then ZenStream.Close()

            Return True

        Catch ex As Exception
            LOG.Write("(SSS�f�[�^�쐬)", "���s", ex.Message)
            LOG.UpdateJOBMASTbyErr("SSS�f�[�^�쐬���s ��O����")
            Return False
        Finally
            If Not ZenStream Is Nothing Then ZenStream.Close()
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "(SSS�f�[�^�쐬)�I��", "����")
        End Try
    End Function

    ''' <summary>
    ''' �V�[�P���X�ԍ��̌������s���܂��B
    ''' </summary>
    ''' <param name="strFuriDate">�U�֓�</param>
    ''' <returns>MAX�V�[�P���X�ԍ�</returns>
    ''' <remarks></remarks>
    Private Function fn_SELECT_SEQ(ByVal strFuriDate As String) As String
        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        SQL.Append("SELECT NVL(MAX(KIGYO_SEQ_K), 69999999) AS JIFURI_SEQ_MAX, COUNT(KIGYO_SEQ_K) AS JIFURI_SEQ_CNT")
        SQL.Append(" FROM MEIMAST")
        SQL.Append(" WHERE FURI_DATE_K = " & SQ(strFuriDate))
        SQL.Append(" AND KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
        SQL.Append(" AND KIGYO_SEQ_K BETWEEN '70000000' AND '79999999'")

        Try
            If oraReader.DataReader(SQL) = True Then
                Return (oraReader.GetInt64("JIFURI_SEQ_MAX") + 1).ToString
            Else
                Return "70000000"
            End If
        Catch ex As Exception
            Return "70000000"
        Finally
            oraReader.Close()
        End Try

    End Function

    ''' <summary>
    ''' �X�P�W���[���}�X�^���X�V���܂��B
    ''' </summary>
    ''' <param name="oraSchReader">�X�P�W���[���I���N�����[�_�[</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_UPDATE_SCHMAST(ByVal oraSchReader As CASTCommon.MyOracleReader) As Boolean
        Dim SQL As New StringBuilder
        With SQL
            .Append("update SCHMAST set ")
            '2017/01/17 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
            '�W����(�X���[�G�X���ϖ���)�Ŏ���
            .Append(" TAKOU_FLG_S = '1'")
            .Append(",YOBI1_S = " & SQ(strDate & strTime))
            'If oraSchReader.GetString("SORTKEY") = "1" Then
            '    .Append(" HAISIN_T1DATE_S = " & SQ(strDate))
            '    .Append(",HAISIN_T1FLG_S = '1'")
            '    .Append(",JIFURI_T1TIME_STAMP_S = " & SQ(strDate & strTime))
            'Else
            '    .Append(" HAISIN_T2DATE_S = " & SQ(strDate))
            '    .Append(",HAISIN_T2FLG_S = '1'")
            '    .Append(",JIFURI_T2TIME_STAMP_S = " & SQ(strDate & strTime))
            'End If
            '2017/01/17 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
            .Append(" where TORIS_CODE_S = " & SQ(oraSchReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_S = " & SQ(oraSchReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_S = " & SQ(oraSchReader.GetString("FURI_DATE_S")))
        End With

        Try
            Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If nRet < 1 Then
                LOG.Write("(�X�P�W���[���}�X�^�X�V)", "���s", _
                          "�����R�[�h�F" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIS_CODE_S") & _
                          "�U�֓��F" & oraSchReader.GetString("FURI_DATE_S"))
                LOG.UpdateJOBMASTbyErr("�X�P�W���[���}�X�^�X�V���s �����R�[�h�F" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIS_CODE_S") & _
                          "�U�֓��F" & oraSchReader.GetString("FURI_DATE_S"))
                Return False
            End If

            Return True
        Catch ex As Exception
            LOG.Write("(�X�P�W���[���}�X�^�X�V)", "���s", ex.Message)
            LOG.UpdateJOBMASTbyErr("�X�P�W���[���}�X�^�X�V���s ��O����")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' �ݒ�t�@�C����ǂݍ��݂܂��B
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_INI_READ() As Boolean
        Try
            IniInfo.DATBK = CASTCommon.GetFSKJIni("COMMON", "DATBK")
            If IniInfo.DATBK.ToUpper.Equals("ERR") = True OrElse IniInfo.DATBK = Nothing Then
                LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[COMMON].DATBK �ݒ�Ȃ�")
                LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [COMMON].DATBK �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.DEN = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If IniInfo.DEN.ToUpper.Equals("ERR") = True OrElse IniInfo.DEN = Nothing Then
                LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[COMMON].DEN �ݒ�Ȃ�")
                LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [COMMON].DEN �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.FTR = CASTCommon.GetFSKJIni("COMMON", "FTR")
            If IniInfo.FTR.ToUpper.Equals("ERR") = True OrElse IniInfo.FTR = Nothing Then
                LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[COMMON].FTR �ݒ�Ȃ�")
                LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [COMMON].FTR �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.FTRANP = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
            If IniInfo.FTRANP.ToUpper.Equals("ERR") = True OrElse IniInfo.FTRANP = Nothing Then
                LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[COMMON].FTRANP �ݒ�Ȃ�")
                LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [COMMON].FTRANP �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.KINKOCD = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If IniInfo.KINKOCD.ToUpper.Equals("ERR") = True OrElse IniInfo.KINKOCD = Nothing Then
                LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[COMMON].KINKOCD �ݒ�Ȃ�")
                LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [COMMON].KINKOCD �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.SSS_SOUFU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SSS_SOUFU")
            If IniInfo.SSS_SOUFU.ToUpper.Equals("ERR") = True OrElse IniInfo.SSS_SOUFU = Nothing Then
                LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[RSV2_V1.0.0].SSS_SOUFU �ݒ�Ȃ�")
                LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [RSV2_V1.0.0].SSS_SOUFU �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.SSS_SOUFU_TEL = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SSS_SOUFU_TEL")
            If IniInfo.SSS_SOUFU_TEL.ToUpper.Equals("ERR") = True OrElse IniInfo.SSS_SOUFU_TEL = Nothing Then
                LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[RSV2_V1.0.0].SSS_SOUFU_TEL �ݒ�Ȃ�")
                LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [RSV2_V1.0.0].SSS_SOUFU_TEL �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.SSS_SOUFU_FAX = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SSS_SOUFU_FAX")
            If IniInfo.SSS_SOUFU_FAX.ToUpper.Equals("ERR") = True OrElse IniInfo.SSS_SOUFU_FAX = Nothing Then
                LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[RSV2_V1.0.0].SSS_SOUFU_FAX �ݒ�Ȃ�")
                LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [RSV2_V1.0.0].SSS_SOUFU_FAX �ݒ�Ȃ�")
                Return False
            End If

            '2017/01/19 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
            IniInfo.FUNOU_SSS_1 = CASTCommon.GetFSKJIni("JIFURI", "FUNOU_SSS_1")
            If IniInfo.FUNOU_SSS_1.ToUpper.Equals("ERR") = True OrElse IniInfo.FUNOU_SSS_1 = Nothing Then
                LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[JIFURI].FUNOU_SSS_1 �ݒ�Ȃ�")
                LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [JIFURI].FUNOU_SSS_1 �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.FUNOU_SSS_2 = CASTCommon.GetFSKJIni("JIFURI", "FUNOU_SSS_2")
            If IniInfo.FUNOU_SSS_2.ToUpper.Equals("ERR") = True OrElse IniInfo.FUNOU_SSS_2 = Nothing Then
                LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[JIFURI].FUNOU_SSS_2 �ݒ�Ȃ�")
                LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [JIFURI].FUNOU_SSS_2 �ݒ�Ȃ�")
                Return False
            End If
            '2017/01/19 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END

            '2017/02/27 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
            IniInfo.SSS_ITAKUCODE_PATN = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SSS_ITAKUCODE_PATN")
            If IniInfo.SSS_ITAKUCODE_PATN.ToUpper.Equals("ERR") = True OrElse IniInfo.SSS_ITAKUCODE_PATN = Nothing Then
                LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[RSV2_V1.0.0].SSS_ITAKUCODE_PATN �ݒ�Ȃ�")
                LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [RSV2_V1.0.0].SSS_ITAKUCODE_PATN �ݒ�Ȃ�")
                Return False
            End If
            '2017/02/27 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END

            Return True
        Catch ex As Exception
            LOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", ex.Message)
            LOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� ��O����")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' SSS�t�@�C���쐬�Ώۂ̃X�P�W���[������������SQL���쐬���܂��B
    ''' </summary>
    ''' <returns>�쐬SQL</returns>
    ''' <remarks></remarks>
    Private Function GetSSSFileCreateSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            '��g��
            .Append("SELECT ")
            .Append(" '1' SORTKEY")
            .Append(",TORIS_CODE_S KEY1")
            .Append(",TORIF_CODE_S KEY2")
            .Append(",SCHMAST.*")
            .Append(",TORIMAST.*")
            .Append(" FROM SCHMAST,TORIMAST")
            .Append(" WHERE ")
            '2017/01/17 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
            '�W����(�X���[�G�X���ϖ���)�Ŏ���
            .Append("     FURI_DATE_S = " & SQ(FURI_DATE))
            .Append(" AND TAKOU_FLG_S = '2'")
            '.Append("     HAISIN_T1FLG_S = '2'")
            '.Append(" AND HAISIN_T1YDATE_S = " & SQ(HAISIN_YDATE))
            '2017/01/17 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
            .Append(" AND TOUROKU_FLG_S = '1'")
            .Append(" AND TYUUDAN_FLG_S = '0'")
            .Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            .Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            .Append(" AND FMT_KBN_T IN ('20', '21')")
            .Append(" AND MOTIKOMI_KBN_T = '0'")
            '2017/01/17 saitou ���t�M��(RSV2�W��) DEL �X���[�G�X�Ή� ---------------------------------------- START
            '�W����(�X���[�G�X���ϖ���)�Ŏ���
            '.Append(" UNION ALL ")
            ''��g�O
            '.Append("SELECT ")
            '.Append(" '2' SORTKEY")
            '.Append(",TORIS_CODE_S KEY1")
            '.Append(",TORIF_CODE_S KEY2")
            '.Append(",SCHMAST.*")
            '.Append(",TORIMAST.*")
            '.Append(" FROM SCHMAST,TORIMAST")
            '.Append(" WHERE ")
            '.Append("     HAISIN_T2FLG_S = '2'")
            '.Append(" AND HAISIN_T2YDATE_S = " & SQ(HAISIN_YDATE))
            '.Append(" AND TOUROKU_FLG_S = '1'")
            '.Append(" AND TYUUDAN_FLG_S = '0'")
            '.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            '.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            '.Append(" AND FMT_KBN_T = '21'")
            '.Append(" AND MOTIKOMI_KBN_T = '0'")
            '2017/01/17 saitou ���t�M��(RSV2�W��) DEL ------------------------------------------------------- END
            .Append(" ORDER BY SORTKEY ASC, KEY1 ASC, KEY2 ASC")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' SSS�t�@�C���쐬�Ώۂ̖��ׂ��擾����SQL���쐬���܂��B
    ''' </summary>
    ''' <param name="strDataKbn">�f�[�^�敪</param>
    ''' <param name="oraSchReader">�X�P�W���[���I���N�����[�_�[</param>
    ''' <returns>�쐬SQL</returns>
    ''' <remarks></remarks>
    Private Function GetMeimastSQL(ByVal strDataKbn As String, _
                                   ByVal oraSchReader As CASTCommon.MyOracleReader) As StringBuilder
        Dim SQL As New StringBuilder
        SQL.Append("select * from MEIMAST")
        SQL.Append(" where TORIS_CODE_K = " & SQ(oraSchReader.GetString("TORIS_CODE_S")))
        SQL.Append(" and TORIF_CODE_K = " & SQ(oraSchReader.GetString("TORIF_CODE_S")))
        SQL.Append(" and FURI_DATE_K = " & SQ(oraSchReader.GetString("FURI_DATE_S")))
        Select Case strDataKbn
            Case "1"
                SQL.Append(" and DATA_KBN_K = '1'")
            Case "2"
                SQL.Append(" and DATA_KBN_K = '2'")
                SQL.Append(" and KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
                SQL.Append(" and FURIKETU_CODE_K = '0'")
                SQL.Append(" and exists (")
                SQL.Append(" select TEIKEI_KBN_N")
                SQL.Append("    from TENMAST")
                SQL.Append("    where KIN_NO_N = KEIYAKU_KIN_K")
                If oraSchReader.GetString("SORTKEY") = "1" Then
                    '��g��
                    SQL.Append("    and TEIKEI_KBN_N = '1'")
                Else
                    '��g�O
                    SQL.Append("    and TEIKEI_KBN_N = '0'")
                End If
                SQL.Append(" )")
                '�\�[�g���͑�K�͂ɍ��킹��
                SQL.Append(" order by KEIYAKU_KIN_K, KEIYAKU_SIT_K")
                SQL.Append(",decode(KEIYAKU_KAMOKU_K,'02',1,'01',2,'05',3,'37',4,9)")
                SQL.Append(",KEIYAKU_KOUZA_K, RECORD_NO_K")
            Case "8"
            Case "9"
        End Select
        Return SQL
    End Function

    ''' <summary>
    ''' ���׃}�X�^�̎��U�V�[�P���X���X�V���܂��B
    ''' </summary>
    ''' <param name="oraMeiReader">���׃I���N�����[�_�[</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function UpdateMeimastJifuriSEQ(ByVal oraMeiReader As CASTCommon.MyOracleReader) As Boolean
        Dim SQL As New StringBuilder
        With SQL
            .Append("update MEIMAST set ")
            .Append(" KIGYO_SEQ_K = " & SQ(strJIFURI_SEQ))
            .Append(" where TORIS_CODE_K = " & SQ(oraMeiReader.GetString("TORIS_CODE_K")))
            .Append(" and TORIF_CODE_K = " & SQ(oraMeiReader.GetString("TORIF_CODE_K")))
            .Append(" and FURI_DATE_K = " & SQ(oraMeiReader.GetString("FURI_DATE_K")))
            .Append(" and RECORD_NO_K = " & oraMeiReader.GetInt64("RECORD_NO_K"))
        End With

        Try
            Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If nRet < 1 Then
                LOG.Write("(���׃}�X�^�X�V)", "���s", _
                          "�����R�[�h�F" & oraMeiReader.GetString("TORIS_CODE_K") & "-" & oraMeiReader.GetString("TORIF_CODE_K") & _
                          " �U�֓��F" & oraMeiReader.GetString("FURI_DATE_K") & " ���R�[�h�ԍ��F" & oraMeiReader.GetInt64("RECORD_NO_K"))
                LOG.UpdateJOBMASTbyErr("���׃}�X�^�X�V���s �����R�[�h�F" & oraMeiReader.GetString("TORIS_CODE_K") & "-" & oraMeiReader.GetString("TORIF_CODE_K") & _
                          " �U�֓��F" & oraMeiReader.GetString("FURI_DATE_K") & " ���R�[�h�ԍ��F" & oraMeiReader.GetInt64("RECORD_NO_K"))
                Return False
            End If

            Return True
        Catch ex As Exception
            LOG.Write("(���׃}�X�^�X�V)", "���s", ex.Message)
            LOG.UpdateJOBMASTbyErr("���׃}�X�^�X�V���s ��O����")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' FTRANP��p���ăR�[�h�ϊ����s���܂��B
    ''' </summary>
    ''' <param name="strGetOrPut">GETRAND,GETDATA,PUTRAND,PUTDATA</param>
    ''' <param name="strInFileName">���̓t�@�C���p�X</param>
    ''' <param name="strOutFileName">�o�̓t�@�C���p�X</param>
    ''' <param name="strPFileName">FTRANP�p�����[�^�t�@�C����</param>
    ''' <returns>0:���� 100:�ُ�</returns>
    ''' <remarks></remarks>
    Private Function ConvertFileFtranP(ByVal strGetOrPut As String, _
                                       ByVal strInFileName As String, _
                                       ByVal strOutFileName As String, _
                                       ByVal strPFileName As String) As Integer
        Try
            '�ϊ��R�}���h�g�ݗ���
            Dim Command As New StringBuilder
            With Command
                .Append(" /nwd/ cload ")
                .Append("""" & IniInfo.FTR & "FUSION" & """")
                .Append(" ; kanji 83_jis")
                .Append(" " & strGetOrPut & " ")
                .Append("""" & strInFileName & """" & " ")
                .Append("""" & strOutFileName & """" & " ")
                .Append(" ++" & """" & strPFileName & """")
            End With

            Dim Proc As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(IniInfo.FTRANP, "FP.EXE")
            ProcInfo.WorkingDirectory = IniInfo.FTRANP
            ProcInfo.Arguments = Command.ToString
            Proc = Process.Start(ProcInfo)
            Proc.WaitForExit()
            If Proc.ExitCode = 0 Then
                Return 0
            Else
                LOG.Write("(FTRANP�R�[�h�ϊ�)", "���s", "�I���R�[�h�F" & Proc.ExitCode)
                Return 100
            End If
        Catch ex As Exception
            LOG.Write("(FTRANP�R�[�h�ϊ�)", "���s", ex.Message)
            Return 100
        End Try
    End Function

    ''' <summary>
    ''' ���t�[���o�͂��܂��B
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_PRINT_SOUFUHYOU() As Boolean

        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        Try
            LOG.Write("(���t�[�o��)�J�n", "����")

            Dim List As New KF3SP005
            Dim Name As String = List.CreateCsvFile()

            '����Ώۂ̃X�P�W���[��������
            If oraReader.DataReader(GetSchPrintSOUFUHYOU) = True Then

                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)

                While oraReader.EOF = False
                    If oraMeiReader.DataReader(GetMeiPrintSOUFUHYOU(oraReader)) = True Then
                        If oraMeiReader.GetInt("FURIKEN") = 0 Then
                            '������0���̓f�[�^�ł��쐬���Ă��Ȃ����߁A�r��
                        Else
                            List.OutputCsvData(oraReader.GetString("FURI_DATE_S"), True)
                            List.OutputCsvData(IniInfo.SSS_SOUFU, True)
                            List.OutputCsvData(IniInfo.SSS_SOUFU_TEL, True)
                            List.OutputCsvData(IniInfo.SSS_SOUFU_FAX, True)
                            '2017/02/27 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                            Dim strItakuCode As String = String.Empty
                            Select Case IniInfo.SSS_ITAKUCODE_PATN
                                Case "0"
                                    strItakuCode = oraReader.GetString("ITAKU_CODE_T")
                                Case "1"
                                    strItakuCode = oraReader.GetString("ITAKU_CODE_T").Substring(0, 9) & oraReader.GetString("SORTKEY")
                                Case "2"
                                    strItakuCode = "03" & IniInfo.KINKOCD.Substring(1, 3) & oraReader.GetString("ITAKU_CODE_T").Substring(6, 4) & oraReader.GetString("SORTKEY")
                                Case Else
                                    strItakuCode = oraReader.GetString("ITAKU_CODE_T")
                            End Select
                            ''�ϑ��҃R�[�h��SSS�d�l
                            'Dim strItakuCode As String
                            'strItakuCode = "03" & IniInfo.KINKOCD.Substring(1, 3) & oraReader.GetString("ITAKU_CODE_T").Substring(5, 4) & "1"
                            '2017/02/27 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
                            List.OutputCsvData(strItakuCode, True)
                            List.OutputCsvData(oraReader.GetString("ITAKU_NNAME_T"), True)
                            List.OutputCsvData(oraMeiReader.GetInt("FURIKEN").ToString)
                            List.OutputCsvData(oraMeiReader.GetInt64("FURIKIN").ToString)
                            If oraReader.GetString("SORTKEY") = "1" Then
                                List.OutputCsvData("1", True, True)
                            Else
                                List.OutputCsvData("2", True, True)
                            End If
                        End If
                    End If

                    oraMeiReader.Close()


                    oraReader.NextRead()
                End While

            Else
                LOG.Write("(���t�[�o��)", "���s", "�X�P�W���[���Ȃ�")
                Return False
            End If

            List.CloseCsv()
            If List.ReportExecute = True Then
                LOG.Write("(���t�[�o��)", "����")
            Else
                LOG.Write("(���t�[�o��)", "���s", List.ReportMessage)
                Return False
            End If

            Return True
        Catch ex As Exception
            LOG.Write("(���t�[�o��)", "���s", ex.Message)
            Return False
        Finally
            LOG.Write("(���t�[�o��)�I��", "����")
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not oraMeiReader Is Nothing Then oraMeiReader.Close()
        End Try
    End Function

    ''' <summary>
    ''' ���t�[����Ώۂ̃X�P�W���[�����擾����SQL���쐬���܂��B
    ''' </summary>
    ''' <returns>�쐬SQL</returns>
    ''' <remarks></remarks>
    Private Function GetSchPrintSOUFUHYOU() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            '��g��
            .Append("SELECT ")
            .Append(" '1' SORTKEY")
            .Append(",TORIS_CODE_S KEY1")
            .Append(",TORIF_CODE_S KEY2")
            .Append(",SCHMAST.*")
            .Append(",TORIMAST.*")
            .Append(" FROM SCHMAST,TORIMAST")
            .Append(" WHERE")
            '2017/01/17 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
            '�W����(�X���[�G�X���ϖ���)�Ŏ���
            .Append("     TAKOU_FLG_S = '1'")
            .Append(" AND YOBI1_S = " & SQ(strDate & strTime))
            '.Append("     HAISIN_T1FLG_S = '1'")
            '.Append(" AND HAISIN_T1DATE_S = " & SQ(strDate))
            '.Append(" AND JIFURI_T1TIME_STAMP_S = " & SQ(strDate & strTime))
            '2017/01/17 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
            .Append(" AND TOUROKU_FLG_S = '1'")
            .Append(" AND TYUUDAN_FLG_S = '0'")
            .Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            .Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            .Append(" AND FMT_KBN_T IN ('20', '21')")
            .Append(" AND MOTIKOMI_KBN_T = '0'")
            '2017/01/17 saitou ���t�M��(RSV2�W��) DEL �X���[�G�X�Ή� ---------------------------------------- START
            '�W����(�X���[�G�X���ϖ���)�Ŏ���
            '.Append(" UNION ALL ")
            ''��g�O
            '.Append("SELECT ")
            '.Append(" '2' SORTKEY")
            '.Append(",TORIS_CODE_S KEY1")
            '.Append(",TORIF_CODE_S KEY2")
            '.Append(",SCHMAST.*")
            '.Append(",TORIMAST.*")
            '.Append(" FROM SCHMAST,TORIMAST")
            '.Append(" WHERE HAISIN_T2FLG_S = '1'")
            '.Append(" AND HAISIN_T2DATE_S = " & SQ(strDate))
            '.Append(" AND JIFURI_T2TIME_STAMP_S = " & SQ(strDate & strTime))
            '.Append(" AND TOUROKU_FLG_S = '1'")
            '.Append(" AND TYUUDAN_FLG_S = '0'")
            '.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            '.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            '.Append(" AND FMT_KBN_T = '21'")
            '.Append(" AND MOTIKOMI_KBN_T = '0'")
            '2017/01/17 saitou ���t�M��(RSV2�W��) DEL ------------------------------------------------------- END
            .Append(" ORDER BY SORTKEY ASC, KEY1 ASC, KEY2 ASC")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' ���t�[����Ώۂ̖��ׂ��擾����SQL���쐬���܂��B
    ''' </summary>
    ''' <param name="oraSchReader">�X�P�W���[���I���N�����[�_�[</param>
    ''' <returns>�쐬SQL</returns>
    ''' <remarks></remarks>
    Private Function GetMeiPrintSOUFUHYOU(ByVal oraSchReader As CASTCommon.MyOracleReader) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select ")
            .Append("  count(FURIKIN_K) as FURIKEN")
            .Append(", sum(FURIKIN_K) as FURIKIN")
            .Append(" from MEIMAST")
            .Append(" where TORIS_CODE_K = " & SQ(oraSchReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_K = " & SQ(oraSchReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_K = " & SQ(oraSchReader.GetString("FURI_DATE_S")))
            .Append(" and KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
            .Append(" and DATA_KBN_K = '2'")
            .Append(" and FURIKETU_CODE_K = 0")
            .Append(" and exists (")
            .Append(" select TEIKEI_KBN_N from TENMAST")
            .Append(" where KIN_NO_N = KEIYAKU_KIN_K")
            If oraSchReader.GetString("SORTKEY") = "1" Then
                '��g��
                .Append(" and TEIKEI_KBN_N = '1'")
            Else
                '��g�O
                .Append(" and TEIKEI_KBN_N = '0'")
            End If
            .Append(" )")
        End With
        Return SQL
    End Function


    ''' <summary>
    ''' ���s�X�P�W���[���}�X�^���쐬���܂��B
    ''' </summary>
    ''' <param name="OraSchReader"></param>
    ''' <returns>True or False</returns>
    ''' <remarks>
    ''' 2017/01/19 saitou ���t�M��(RSV2�W��) added for �X���[�G�X�Ή�
    ''' �W����(�X���[�G�X���ϖ���)�͍��܂ő��s�X�P�W���[���}�X�^������Ă��Ȃ��������A
    ''' �W���g�ݍ��݂ő��s�X�P�W���[�������悤�ɂ��Ă݂�B
    ''' </remarks>
    Private Function fn_INSERT_TAKOSCHMAST(ByVal OraSchReader As CASTCommon.MyOracleReader) As Boolean
        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            '2017/01/19 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
            '�s�\���ʍX�V�\������Z�o
            '�W����(RSV2�X���[�G�X���ϖ���)�Ȃ̂ŁA��g��������OK
            Dim FUNOU_T1YDATE As String = String.Empty
            FUNOU_T1YDATE = CASTCommon.GetEigyobi(CASTCommon.ConvertDate(OraSchReader.GetString("FURI_DATE_S")), _
                                                  CInt(Me.IniInfo.FUNOU_SSS_1), _
                                                  FmtComm.HolidayList).ToString("yyyyMMdd")
            '2017/01/19 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END

            '--------------------------------------------------
            '�G���[�����A���z�̒��o
            '--------------------------------------------------
            Dim inputErrCnt As Long = 0
            Dim inputErrKin As Long = 0

            With SQL
                .Length = 0
                .Append("SELECT COUNT(FURIKIN_K) AS CNT,NVL(SUM(FURIKIN_K),0) AS KIN FROM MEIMAST ")
                .Append(" WHERE TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                .Append(" AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                .Append(" AND FURI_DATE_K = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                .Append(" AND DATA_KBN_K = '2'")
                .Append(" AND KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
                .Append(" AND FURIKETU_CODE_K <> 0")
                .Append(" AND EXISTS (")
                .Append(" SELECT TEIKEI_KBN_N FROM TENMAST")
                .Append(" WHERE KIN_NO_N = KEIYAKU_KIN_K")
                If OraSchReader.GetString("SORTKEY") = "1" Then
                    .Append(" AND TEIKEI_KBN_N = '1'")
                Else
                    .Append(" AND TEIKEI_KBN_N = '0'")
                End If
                .Append(" )")
            End With

            If oraReader.DataReader(SQL) = True Then
                inputErrCnt = oraReader.GetInt64("CNT")
                inputErrKin = oraReader.GetInt64("KIN")
            End If

            oraReader.Close()

            '--------------------------------------------------
            '�G���[�łȂ������A���z�̒��o
            '--------------------------------------------------
            Dim normalCnt As Long = 0
            Dim normalKin As Long = 0

            With SQL
                .Length = 0
                .Append("SELECT COUNT(FURIKIN_K) AS CNT,NVL(SUM(FURIKIN_K),0) AS KIN FROM MEIMAST ")
                .Append(" WHERE TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                .Append(" AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                .Append(" AND FURI_DATE_K = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                .Append(" AND DATA_KBN_K = '2'")
                .Append(" AND KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
                .Append(" AND FURIKETU_CODE_K = 0")
                .Append(" AND EXISTS (")
                .Append(" SELECT TEIKEI_KBN_N FROM TENMAST")
                .Append(" WHERE KIN_NO_N = KEIYAKU_KIN_K")
                If OraSchReader.GetString("SORTKEY") = "1" Then
                    .Append(" AND TEIKEI_KBN_N = '1'")
                Else
                    .Append(" AND TEIKEI_KBN_N = '0'")
                End If
                .Append(" )")
            End With

            If oraReader.DataReader(SQL) = True Then
                normalCnt = oraReader.GetInt64("CNT")
                normalKin = oraReader.GetInt64("KIN")
            End If

            oraReader.Close()

            '--------------------------------------------------
            '���s�X�P�W���[���}�X�^�쐬
            '--------------------------------------------------
            '2017/01/19 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
            '�W����(�X���[�G�X���ϖ���)�p�ɕύX
            With SQL
                .Length = 0
                .Append("INSERT INTO TAKOSCHMAST (")
                .Append(" TORIS_CODE_U")
                .Append(",TORIF_CODE_U")
                .Append(",FURI_DATE_U")
                .Append(",FUNOU_YDATE_U")
                .Append(",FMT_KBN_U")
                .Append(",BAITAI_CODE_U")
                .Append(",LABEL_CODE_U")
                .Append(",CODE_KBN_U")
                .Append(",TKIN_NO_U")
                .Append(",FUNOU_FLG_U")
                .Append(",SYORI_KEN_U")
                .Append(",SYORI_KIN_U")
                .Append(",FURI_KEN_U")
                .Append(",FURI_KIN_U")
                .Append(",FUNOU_KEN_U")
                .Append(",FUNOU_KIN_U")
                .Append(",SAKUSEI_DATE_U")
                .Append(") VALUES (")
                .Append(" " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                .Append("," & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                .Append("," & SQ(OraSchReader.GetString("FURI_DATE_S")))
                .Append("," & SQ(FUNOU_T1YDATE))
                .Append("," & SQ(OraSchReader.GetString("FMT_KBN_T")))
                .Append("," & SQ("00"))
                .Append("," & SQ(OraSchReader.GetString("LABEL_KBN_T")))
                .Append("," & SQ("4"))
                .Append("," & SQ(IniInfo.KINKOCD))
                .Append("," & SQ("0"))
                .Append("," & normalCnt.ToString)
                .Append("," & normalKin.ToString)
                .Append(",0")
                .Append(",0")
                .Append(",0")
                .Append(",0")
                .Append("," & SQ(strDate))
                .Append(")")
            End With
            'With SQL
            '    .Length = 0
            '    .Append("INSERT INTO TAKOSCHMAST (")
            '    .Append(" TORIS_CODE_U")
            '    .Append(",TORIF_CODE_U")
            '    .Append(",FURI_DATE_U")
            '    .Append(",FUNOU_YDATE_U")
            '    .Append(",FMT_KBN_U")
            '    .Append(",BAITAI_CODE_U")
            '    .Append(",LABEL_CODE_U")
            '    .Append(",CODE_KBN_U")
            '    .Append(",TKIN_NO_U")
            '    .Append(",TEIKEI_KBN_U")
            '    .Append(",FUNOU_FLG_U")
            '    .Append(",SYORI_KEN_U")
            '    .Append(",SYORI_KIN_U")
            '    .Append(",FURI_KEN_U")
            '    .Append(",FURI_KIN_U")
            '    .Append(",FUNOU_KEN_U")
            '    .Append(",FUNOU_KIN_U")
            '    .Append(",SAKUSEI_DATE_U")
            '    .Append(",HAISIN_YDATE_U")
            '    .Append(",HAISIN_DATE_U")
            '    .Append(",KESSAI_YDATE_U")
            '    .Append(",KESSAI_DATE_U")
            '    .Append(",TESUU_YDATE_U")
            '    .Append(",TESUU_DATE_U")
            '    .Append(",HENKAN_YDATE_U")
            '    .Append(",HENKAN_DATE_U")
            '    .Append(",TESUUKEI_FLG_U")
            '    .Append(",TESUUTYO_FLG_U")
            '    .Append(",KESSAI_FLG_U")
            '    .Append(",HENKAN_FLG_U")
            '    .Append(",TESUU_KIN_U")
            '    .Append(",TESUU_KIN1_U")
            '    .Append(",TESUU_KIN2_U")
            '    .Append(",TESUU_KIN3_U")
            '    .Append(",FUNOU_DATE_U")
            '    .Append(",ERR_KEN_U")
            '    .Append(",ERR_KIN_U")
            '    .Append(",SEND_KEN_U")
            '    .Append(",SEND_KIN_U")
            '    .Append(",JIFURI_TIME_STAMP_U")
            '    .Append(",KESSAI_TIME_STAMP_U")
            '    .Append(",TESUU_TIME_STAMP_U")
            '    .Append(") VALUES (")
            '    .Append(" " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
            '    .Append("," & SQ(OraSchReader.GetString("TORIF_CODE_S")))
            '    .Append("," & SQ(OraSchReader.GetString("FURI_DATE_S")))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ(OraSchReader.GetString("FUNOU_T1YDATE_S")))
            '    Else
            '        .Append("," & SQ(OraSchReader.GetString("FUNOU_T2YDATE_S")))
            '    End If
            '    .Append("," & SQ(OraSchReader.GetString("FMT_KBN_T")))
            '    .Append("," & SQ("00"))
            '    .Append("," & SQ(OraSchReader.GetString("LABEL_KBN_T")))
            '    .Append("," & SQ("3"))
            '    .Append("," & SQ(IniInfo.KINKOCD))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ("1"))
            '    Else
            '        .Append("," & SQ("2"))
            '    End If
            '    .Append("," & SQ("0"))
            '    .Append("," & (normalCnt + inputErrCnt).ToString)
            '    .Append("," & (normalKin + inputErrKin).ToString)
            '    .Append(",0")
            '    .Append(",0")
            '    .Append(",0")
            '    .Append(",0")
            '    .Append("," & SQ(strDate))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ(OraSchReader.GetString("HAISIN_T1YDATE_S")))
            '    Else
            '        .Append("," & SQ(OraSchReader.GetString("HAISIN_T2YDATE_S")))
            '    End If
            '    .Append("," & SQ(strDate))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ(OraSchReader.GetString("KESSAI_T1YDATE_S")))
            '    Else
            '        .Append("," & SQ(OraSchReader.GetString("KESSAI_T2YDATE_S")))
            '    End If
            '    .Append("," & SQ(New String("0"c, 8)))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ(OraSchReader.GetString("TESUU_T1YDATE_S")))
            '    Else
            '        .Append("," & SQ(OraSchReader.GetString("TESUU_T2YDATE_S")))
            '    End If
            '    .Append("," & SQ(New String("0"c, 8)))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ(OraSchReader.GetString("HENKAN_T1YDATE_S")))
            '    Else
            '        .Append("," & SQ(OraSchReader.GetString("HENKAN_T2YDATE_S")))
            '    End If
            '    .Append("," & SQ(New String("0"c, 8)))
            '    .Append("," & SQ("0"))
            '    .Append("," & SQ("0"))
            '    .Append("," & SQ("0"))
            '    .Append("," & SQ("0"))
            '    .Append(",0")
            '    .Append(",0")
            '    .Append(",0")
            '    .Append(",0")
            '    .Append("," & SQ(New String("0"c, 8)))
            '    .Append("," & inputErrCnt.ToString)
            '    .Append("," & inputErrKin.ToString)
            '    .Append("," & normalCnt.ToString)
            '    .Append("," & normalKin.ToString)
            '    .Append("," & SQ(strDate & strTime))
            '    .Append("," & SQ(New String("0"c, 14)))
            '    .Append("," & SQ(New String("0"c, 14)))
            '    .Append(")")
            'End With
            '2017/01/19 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END

            Dim SQLDel As New StringBuilder
            SQLDel.Append("DELETE FROM TAKOSCHMAST")
            SQLDel.Append(" WHERE TORIS_CODE_U = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
            SQLDel.Append(" AND TORIF_CODE_U = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
            SQLDel.Append(" AND FURI_DATE_U = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
            SQLDel.Append(" AND TKIN_NO_U = " & SQ(IniInfo.KINKOCD))
            '2017/01/19 saitou ���t�M��(RSV2�W��) DEL �X���[�G�X�Ή� ---------------------------------------- START
            '�W����(�X���[�G�X���ϖ���)�͒�g���݂̂̈����Ȃ̂ŁA���s�X�P�W���[���}�X�^�ɒ�g�敪�͎����Ȃ��B
            'If OraSchReader.GetString("SORTKEY") = "1" Then
            '    SQLDel.Append(" AND TEIKEI_KBN_U = '1'")
            'Else
            '    SQLDel.Append(" AND TEIKEI_KBN_U = '2'")
            'End If
            '2017/01/19 saitou ���t�M��(RSV2�W��) DEL ------------------------------------------------------- END

            Dim nRet As Integer = MainDB.ExecuteNonQuery(SQLDel)
            If nRet >= 0 Then
                LOG.Write("���s�X�P�W���[���쐬", "����", "��s���R�[�h���폜")
            End If

            nRet = MainDB.ExecuteNonQuery(SQL)
            If nRet > 0 Then
                LOG.Write("���s�X�P�W���[���쐬", "����", "")
            End If

        Catch ex As Exception
            LOG.Write("���s�X�P�W���[���쐬", "���s", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try

        Return True

    End Function

#End Region

#Region "�v���C�x�[�g�N���X"

    ''' <summary>
    ''' ���t�[����N���X
    ''' </summary>
    ''' <remarks></remarks>
    Private Class KF3SP005
        Inherits CAstReports.ClsReportBase
        Public Sub New()
            'CSV�t�@�C���Z�b�g
            InfoReport.ReportName = "KF3SP005"
            '��`�̖��Z�b�g
            ReportBaseName = "KF3SP005_SSS���t�[.rpd"
        End Sub

        Public Overrides Function CreateCsvFile() As String
            Return MyBase.CreateCsvFile()
        End Function

        Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
            CSVObject.Output(data, dq, crlf)
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

    End Class

#End Region

End Class
