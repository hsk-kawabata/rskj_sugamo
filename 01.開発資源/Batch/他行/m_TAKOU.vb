Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CAstReports.ClsReportBase
Imports System.Data
Imports Microsoft.VisualBasic
Imports System.Windows.Forms
Imports CASTCommon

Module m_TAKOU

#Region "���W���[���ϐ�"

    Private clsFusion As New clsFUSION.clsMain

    '�p�����[�^�p
    Public strTORI_CODE As String
    Public strTORIS_CODE As String
    Public strTORIF_CODE As String
    Public strFURI_DATE As String
    Public strTUUBAN As String

    ' �n�����������c�a
    Private MainDB As CASTCommon.MyOracle

    '�w�b�_�[���R�[�h�p
    Public strHEDDA_FURI_DATA As String

    '���[�N�t�@�C����
    Public strWRK_FILE_NAME As String
    '�t�@�C���̃��R�[�h��
    Public intREC_LENGTH As Integer
    '�����E���z�v�Z�p
    Private dblGOUKEI_KEN As Decimal
    Private dblGOUKEI_KIN As Decimal
    '�f�[�^�쐬�����Z�@�֒P�ʂŃ��[�v����邽�߁A�t���O�ŊǗ�����
    Dim blnMakeMATOME_DATA As Boolean
    '****************************************************

    Dim strIBMP_FILE As String

    Public strParaFURI_DATE As String
    Public strParaTORI_CODE As String
    Public nJIFURI_SEQ As Long

    Private BLOG As New CASTCommon.BatchLOG("KFJ020", "���s�f�[�^�쐬")
    Private Const msgTitle As String = "���s�f�[�^�쐬(KFJ020)"

    '2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
    Public vbDLL As New CMTCTRL.ClsFSKJ
    'Public vbDLL As New FSKJDLL.ClsFSKJ
    '2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END

    Private gstrP_FILE As String

    Private JobMessage As String = ""

    Private GCOM As New MenteCommon.clsCommon

    Private Structure strcTakoumastInfo
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
        Dim TKIN_NO As String
        Dim TSIT_NO As String
        Dim ITAKU_CODE As String
        Dim KAMOKU As String
        Dim KOUZA As String
        Dim BAITAI_CODE As String
        Dim CODE_KBN As String
        Dim SFILE_NAME As String
        Dim RFILE_NAME As String

        Public Sub Init()
            TORIS_CODE = String.Empty
            TORIF_CODE = String.Empty
            ITAKU_CODE = String.Empty
            TKIN_NO = String.Empty
            TSIT_NO = String.Empty
            KAMOKU = String.Empty
            KOUZA = String.Empty
            BAITAI_CODE = String.Empty
            CODE_KBN = String.Empty
            SFILE_NAME = String.Empty
            RFILE_NAME = String.Empty
        End Sub

        Friend Sub SetOraData(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_V")
            TORIF_CODE = oraReader.GetString("TORIF_CODE_V")
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_V")
            TKIN_NO = oraReader.GetString("TKIN_NO_V")
            TSIT_NO = oraReader.GetString("TSIT_NO_V")
            KAMOKU = oraReader.GetString("KAMOKU_V")
            KOUZA = oraReader.GetString("KOUZA_V")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_V")
            CODE_KBN = oraReader.GetString("CODE_KBN_V")
            SFILE_NAME = oraReader.GetString("SFILE_NAME_V")
            RFILE_NAME = oraReader.GetString("RFILE_NAME_V")
        End Sub
    End Structure
    Private TakouInfo As strcTakoumastInfo

    Private Structure strcTorimastInfo
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
        Dim ITAKU_KNAME As String
        Dim ITAKU_NNAME As String
        Dim KIGYO_CODE As String
        Dim FURI_CODE As String
        Dim BAITAI_CODE As String
        Dim FMT_KBN As String
        Dim NS_KBN As String
        Dim LABEL_KBN As String
        Dim SYOUHI_KBN As String

        Public Sub Init()
            TORIS_CODE = String.Empty
            TORIF_CODE = String.Empty
            ITAKU_KNAME = String.Empty
            ITAKU_NNAME = String.Empty
            KIGYO_CODE = String.Empty
            FURI_CODE = String.Empty
            BAITAI_CODE = String.Empty
            FMT_KBN = String.Empty
            NS_KBN = String.Empty
            LABEL_KBN = String.Empty
            SYOUHI_KBN = String.Empty
        End Sub

        Friend Sub SetOraData(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_T")
            TORIF_CODE = oraReader.GetString("TORIF_CODE_T")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_T")
            ITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_T")
            KIGYO_CODE = oraReader.GetString("KIGYO_CODE_T")
            FURI_CODE = oraReader.GetString("FURI_CODE_T")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_T")
            FMT_KBN = oraReader.GetString("FMT_KBN_T")
            NS_KBN = oraReader.GetString("NS_KBN_T")
            LABEL_KBN = oraReader.GetString("LABEL_KBN_T")
            SYOUHI_KBN = oraReader.GetString("SYOUHI_KBN_T")
        End Sub
    End Structure
    Private ToriInfo As strcTorimastInfo

    Private Structure strcIniInfo
        Dim DEN As String
        Dim TAK As String
        Dim KINKOCD As String
        Dim MT As String
        Dim CMT As String
        Dim TAKO_SEQ As String
        Dim NOUKYOMATOME As String
        Dim NOUKYOFROM As String
        Dim NOUKYOTO As String
        ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
        Dim RSV2_EDITION As String        ' RSV2�@�\�ݒ�
        Dim COMMON_BAITAIWRITE As String  ' �}�̏����p�t�H���_
        ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
    End Structure
    Private IniInfo As strcIniInfo

    Private strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

#End Region

#Region "�p�u���b�N���\�b�h"

    Public Sub Main()
        '============================================================================
        'NAME           :Main
        'Parameter      :
        'Description    :���s�f�[�^�쐬�����又��
        'Return         :
        'Create         :2004/08/19
        'Update         :
        '============================================================================

        '-----------------------------------------
        '�p�����^�`�F�b�N
        '-----------------------------------------
        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            sw = BLOG.Write_Enter1("", "0000000000-00", "00000000", "���s�f�[�^�쐬�����又��(�J�n)", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            Dim strCommand As String = Microsoft.VisualBasic.Command()
            '2017/01/16 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
            Dim SSSFlg As Boolean = False
            If strCommand.Split(","c).Length <> 3 AndAlso strCommand.Split(","c).Length <> 4 Then
                'If strCommand.Split(","c).Length <> 3 Then
                '2017/01/16 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
                BLOG.Write("�p�����^�擾", "���s", "�����F" & strCommand)

                Exit Sub
            Else
                Dim arr() As String = strCommand.Split(","c)
                '2017/01/16 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                If arr.Length = 3 Then
                    strTORI_CODE = arr(0)                           '�����R�[�h
                    strTORIS_CODE = strTORI_CODE.Substring(0, 10)   '������R�[�h
                    strTORIF_CODE = strTORI_CODE.Substring(10, 2)   '����敛�R�[�h
                    strFURI_DATE = arr(1)                           '�U�֓�
                    strTUUBAN = arr(2)                              '�ʔ�
                ElseIf arr.Length = 4 Then
                    strTORI_CODE = arr(0)                           '�����R�[�h
                    strTORIS_CODE = strTORI_CODE.Substring(0, 10)   '������R�[�h
                    strTORIF_CODE = strTORI_CODE.Substring(10, 2)   '����敛�R�[�h
                    strFURI_DATE = arr(1)                           '�U�֓�
                    strTUUBAN = arr(3)                              '�ʔ�
                    SSSFlg = True
                End If
                'strTORI_CODE = arr(0)                           '�����R�[�h
                'strTORIS_CODE = strTORI_CODE.Substring(0, 10)   '������R�[�h
                'strTORIF_CODE = strTORI_CODE.Substring(10, 2)   '����敛�R�[�h
                'strFURI_DATE = arr(1)                           '�U�֓�
                'strTUUBAN = arr(2)                              '�ʔ�
                '2017/01/16 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END

                strParaTORI_CODE = strTORI_CODE
                strParaFURI_DATE = strFURI_DATE

                BLOG.Write("�p�����^�擾", "����")

            End If

            BLOG.ToriCode = strTORI_CODE
            BLOG.FuriDate = strFURI_DATE
            BLOG.JobTuuban = CASTCommon.CAInt32(strTUUBAN.Trim)

            '------------------------------------------
            'INI�t�@�C���̓ǂݍ���
            '------------------------------------------
            If fn_INI_READ() = False Then
                BLOG.UpdateJOBMASTbyErr("�f�B���N�g���擾���s")
                Return
            End If

            '-----------------------------------------
            '���s�f�[�^�쐬���C������
            '-----------------------------------------
            '2017/01/16 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
            If SSSFlg = False Then
                If fn_CREATE_DATA_MAIN() = False Then
                    BLOG.UpdateJOBMASTbyErr(JobMessage)
                    BLOG.Write("���s�f�[�^�쐬 �ُ�I��", "���s")

                Else
                    BLOG.UpdateJOBMASTbyOK(JobMessage)
                    BLOG.Write("���s�f�[�^�쐬 ����I��", "����")

                End If
            Else
                Dim SSS As New ClsSSS
                SSS.FURI_DATE = strParaFURI_DATE
                If SSS.fn_CREATE_DATA_MAIN() = False Then
                    BLOG.Write("SSS���s�f�[�^�쐬 �ُ�I��", "���s")
                    '�z�M�҂��t���O�̖߂�
                    SSS.ReturnFlg()
                Else
                    BLOG.Write("SSS���s�f�[�^�쐬 ����I��", "����")
                    '�z�M�҂��t���O�̖߂�
                    SSS.ReturnFlg()
                    BLOG.UpdateJOBMASTbyOK("")
                End If
            End If

            'If fn_CREATE_DATA_MAIN() = False Then
            '    BLOG.UpdateJOBMASTbyErr(JobMessage)
            '    BLOG.Write("���s�f�[�^�쐬 �ُ�I��", "���s")

            'Else
            '    BLOG.UpdateJOBMASTbyOK(JobMessage)
            '    BLOG.Write("���s�f�[�^�쐬 ����I��", "����")

            'End If
            '2017/01/16 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END

            If Not MainDB Is Nothing Then
                MainDB.Close()
            End If

        Catch ex As Exception
            BLOG.Write("0000000000-00", "00000000", "���s�f�[�^�쐬�����又��", "���s", ex.Message)
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            BLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "���s�f�[�^�쐬�����又��(�I��)", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
        End Try
    End Sub

#End Region

#Region "�v���C�x�[�g���\�b�h"

    ''' <summary>
    ''' ���s�f�[�^�쐬���C������
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_CREATE_DATA_MAIN() As Boolean

        Dim TakouMeisaiList As New ArrayList(128)       '���s���ו\����p
        Dim FurikaeDataInvoice As New ArrayList(128)    '�����U�֐����f�[�^���t�[����p

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 600
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 600
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

        MainDB = New CASTCommon.MyOracle
        Dim oraSchReader As New CASTCommon.MyOracleReader(MainDB)
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        Try
            BLOG.Write("(���s�f�[�^�쐬)�J�n", "����")


            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s���b�N
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                BLOG.Write_Err("���s�f�[�^�쐬����", "���s", "���s�f�[�^�쐬�����Ŏ��s�҂��^�C���A�E�g")
                JobMessage = "���s�f�[�^�쐬�����Ŏ��s�҂��^�C���A�E�g"
                Return False
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            '----------------------------------------
            '�X�P�W���[������
            '----------------------------------------
            If oraSchReader.DataReader(CreateGetSchmastSQL) = True Then

                '�ΏۃX�P�W���[�������݂�����������s�X�P�W���[���̍폜
                If DeleteTakoschmast() = False Then
                    BLOG.Write("���s�f�[�^�쐬", "���s", "���s�X�P�W���[���}�X�^�폜���s")
                    JobMessage = "���s�X�P�W���[���}�X�^�폜���s�@�U�֓��F" & strFURI_DATE
                    Return False
                End If

                '���U�V�[�P���X�̎擾
                nJIFURI_SEQ = GetMaxJifuriSEQPlusOne()

                '���׌������[�_�[
                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)

                While oraSchReader.EOF = False

                    Dim strKEIYAKUKIN_OLD As String = ""

                    '----------------------------------------
                    '���O�p�̎����R�[�h�ݒ�
                    '----------------------------------------
                    BLOG.ToriCode = oraSchReader.GetString("TORIS_CODE_S") & oraSchReader.GetString("TORIF_CODE_S")
                    BLOG.FuriDate = oraSchReader.GetString("FURI_DATE_S")

                    '----------------------------------------
                    '�f�[�^���o(�w�b�_�[��)
                    '----------------------------------------
                    If oraMeiReader.DataReader(CreateGetMeimastHeaderRecord(oraSchReader)) = True Then
                        strHEDDA_FURI_DATA = oraMeiReader.GetItem("FURI_DATA_K")
                    Else
                        BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "���s�f�[�^�쐬", "���s", "���s�f�[�^�w�b�_���R�[�h�������s")
                        JobMessage = "���s�f�[�^�w�b�_���R�[�h�������s"
                        Return False
                    End If

                    oraMeiReader.Close()

                    '----------------------------------------
                    '�f�[�^���o(�f�[�^��)
                    '----------------------------------------
                    Dim intRECORD_COUNT As Integer = 0
                    If oraMeiReader.DataReader(CreateGetMeimastKeiyakuKin(oraSchReader)) = True Then
                        While oraMeiReader.EOF = False
                            intRECORD_COUNT += 1

                            '-------------------------------------
                            '���s�}�X�^����
                            '-------------------------------------
                            TakouInfo.Init()
                            If GetTakoumast(oraSchReader.GetString("TORIS_CODE_S"), oraSchReader.GetString("TORIF_CODE_S"), oraMeiReader.GetString("KEIYAKU_KIN_K")) = False Then
                                '���s�}�X�^���o�^
                                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "���s�f�[�^�쐬", "���s", "���s�}�X�^�������s�@���Z�@�փR�[�h�F" & oraMeiReader.GetString("KEIYAKU_KIN_K"))
                                JobMessage = "���s�}�X�^�������s�@�����R�[�h�F" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S") & " ���Z�@�փR�[�h�F" & oraMeiReader.GetString("KEIYAKU_KIN_K")
                            End If

                            '-------------------------------------
                            '���Z�@�փ}�X�^����
                            '-------------------------------------
                            If CheckTenmast(TakouInfo.TKIN_NO, TakouInfo.TSIT_NO) = False Then
                                '���Z�@�փ}�X�^���o�^
                                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "���s�f�[�^�쐬", "���s", "���Z�@�փ}�X�^�������s�@���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & "-" & TakouInfo.TSIT_NO)
                                JobMessage = "���Z�@�փ}�X�^�������s�@���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & "-" & TakouInfo.TSIT_NO
                                Return False
                            End If

                            '--------------------------------------------------------
                            '�_�������Ή��i���Z�@�փR�[�h��5874�`5939�͂܂Ƃ߂Ĕ_�������j
                            '--------------------------------------------------------
                            '2013/03/29 saitou �\�[�X���P DEL -------------------------------------------------->>>>
                            '�p�r�s���Ȃ̂ō폜
                            'If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                            '    strKIN_NNAME = "�_��"
                            'End If
                            '2013/03/29 saitou �\�[�X���P DEL --------------------------------------------------<<<<

                            '-------------------------------------
                            '�����}�X�^����
                            '-------------------------------------
                            ToriInfo.Init()
                            If GetTorimast(oraSchReader.GetString("TORIS_CODE_S"), oraSchReader.GetString("TORIF_CODE_S")) = False Then
                                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "���s�f�[�^�쐬", "���s", "�����R�[�h�F" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S"))
                                JobMessage = "�����}�X�^�������s �����R�[�h�F" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S")
                                Return False
                            End If

                            '-------------------------------------
                            '�t�H���_�̑��݊m�F�ƍ쐬
                            '-------------------------------------
                            Dim strFOLDER As String
                            If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                                '*** �C�� mitsu 2008/08/11 �_����\�R�[�h�Ńt�H���_�쐬 ***
                                strFOLDER = Path.Combine(IniInfo.TAK, IniInfo.NOUKYOMATOME)
                                '**********************************************************
                            Else
                                strFOLDER = Path.Combine(IniInfo.TAK, TakouInfo.TKIN_NO)
                            End If
                            strWRK_FILE_NAME = Path.Combine(strFOLDER, strFURI_DATE & "_" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & ".dat")
                            '�t�H���_�̑��݊m�F
                            If Directory.Exists(strFOLDER) = False Then
                                '�t�H���_�̍쐬
                                Directory.CreateDirectory(strFOLDER)
                            End If

                            '-------------------------------------
                            '�t�@�C���̍쐬
                            '-------------------------------------
                            ' 2016/01/22 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
                            Select Case ToriInfo.FMT_KBN
                                Case "04", "05"
                                    ToriInfo.FMT_KBN = "00"
                                Case Else
                                    If CInt(ToriInfo.FMT_KBN.Trim) >= 50 Then
                                        ToriInfo.FMT_KBN = "00"
                                    End If
                            End Select
                            ' 2016/01/22 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

                            Select Case ToriInfo.FMT_KBN
                                Case "00"   '�S��t�H�[�}�b�g
                                    Select Case TakouInfo.BAITAI_CODE
                                        Case "04", "09"
                                            If fn_CREATE_FILE_ZENGIN(False) = False Then
                                                Return False
                                            End If
                                        Case Else
                                            If fn_CREATE_FILE_ZENGIN(True) = False Then
                                                Return False
                                            End If
                                    End Select

                                Case "01"   'NHK�t�H�[�}�b�g
                                    Select Case TakouInfo.BAITAI_CODE
                                        Case "04", "09"
                                            If fn_CREATE_FILE_NHK(False) = False Then
                                                Return False
                                            End If
                                        Case Else
                                            If fn_CREATE_FILE_NHK(True) = False Then
                                                Return False
                                            End If
                                    End Select

                                Case "02"

                            End Select

                            '-------------------------------------
                            '�}�̏�����
                            '-------------------------------------
                            Select Case ToriInfo.FMT_KBN
                                Case "00", "01"
                                    Select Case TakouInfo.CODE_KBN
                                        Case "0" : gstrP_FILE = "120JIS��JIS.P"
                                        Case "1" : gstrP_FILE = "120JIS��JIS��.P"
                                        Case "2" : gstrP_FILE = "119JIS��JIS��.P"
                                        Case "3" : gstrP_FILE = "118JIS��JIS��.P"
                                        Case "4"
                                            gstrP_FILE = "120.P"
                                            strIBMP_FILE = "120IBM.P"
                                    End Select

                                    intREC_LENGTH = 120
                            End Select
                            '2016/02/05 �^�X�N�j�֓� RSV2�Ή� UPD ---------------------------------------- START
                            '��K�͔Ŏg�p���̓��b�Z�[�W�o�͂��Ȃ�
                            Select Case IniInfo.RSV2_EDITION
                                Case "2"
                                    'NOP
                                Case Else
                                    '2010/02/19 �ǂ���MT/CMT��v������Ă��邩�킩��Ȃ��̂Œʒm
                                    Dim KIN_NAME As String = GCOM.GetBKBRName(TakouInfo.TKIN_NO, "")
                                    If (TakouInfo.BAITAI_CODE = "05" And IniInfo.MT = "0") Then
                                        MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "MT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                                    ElseIf (TakouInfo.BAITAI_CODE = "06" And IniInfo.CMT = "0") Then
                                        MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "CMT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                                    End If
                                    '===========================================================

                                    If TakouInfo.BAITAI_CODE = "01" Then
                                        MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "FD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                                    End If
                            End Select

                            ''2010/02/19 �ǂ���MT/CMT��v������Ă��邩�킩��Ȃ��̂Œʒm
                            'Dim KIN_NAME As String = GCOM.GetBKBRName(TakouInfo.TKIN_NO, "")
                            'If (TakouInfo.BAITAI_CODE = "05" And IniInfo.MT = "0") Then
                            '    MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "MT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                            'ElseIf (TakouInfo.BAITAI_CODE = "06" And IniInfo.CMT = "0") Then
                            '    MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "CMT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                            'End If
                            ''===========================================================

                            'If TakouInfo.BAITAI_CODE = "01" Then
                            '    MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "FD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                            'End If
                            '2016/02/05 �^�X�N�j�֓� RSV2�Ή� UPD ---------------------------------------- END

                            If fn_BAITAI_WRITE() = False Then
                                Return False
                            End If

                            '2010/02/19 �������ݒǉ�
                            ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                            'Select Case TakouInfo.BAITAI_CODE
                            '    Case "01", "05", "06"
                            '        If TakouInfo.BAITAI_CODE = "05" And IniInfo.MT = "1" Then
                            '            Exit Select
                            '        ElseIf TakouInfo.BAITAI_CODE = "06" And IniInfo.CMT = "1" Then
                            '            Exit Select
                            '        End If
                            '        If MessageBox.Show(MSG0061I, _
                            '                           msgTitle, _
                            '                           MessageBoxButtons.YesNo, _
                            '                           MessageBoxIcon.Information) = DialogResult.Yes Then

                            '            If fn_BAITAI_WRITE() = False Then
                            '                Exit Function
                            '            End If
                            '        End If
                            'End Select
                            If TakouInfo.BAITAI_CODE = "05" And IniInfo.MT = "1" Then
                                ' NOP
                            ElseIf TakouInfo.BAITAI_CODE = "06" And IniInfo.CMT = "1" Then
                                ' NOP
                            Else
                                Select Case IniInfo.RSV2_EDITION
                                    Case "2"
                                        ' NOP
                                    Case Else
                                        Select Case TakouInfo.BAITAI_CODE
                                            Case "01"
                                                If MessageBox.Show(MSG0061I, _
                                                                       msgTitle, _
                                                                       MessageBoxButtons.YesNo, _
                                                                       MessageBoxIcon.Information) = DialogResult.Yes Then

                                                    If fn_BAITAI_WRITE() = False Then
                                                        Exit Function
                                                    End If
                                                End If
                                        End Select
                                End Select
                            End If
                            ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
                            '==========================

                            '���s���ו\�p�̔z��Ɋi�[
                            If TakouInfo.TKIN_NO <> strKEIYAKUKIN_OLD Then
                                strKEIYAKUKIN_OLD = TakouInfo.TKIN_NO

                                Select Case TakouInfo.BAITAI_CODE
                                    Case "04"
                                        Dim KeyCode() As String = New String() {ToriInfo.TORIS_CODE, _
                                                                                ToriInfo.TORIF_CODE, _
                                                                                strFURI_DATE, _
                                                                                TakouInfo.TKIN_NO}
                                        '���s���ו\
                                        TakouMeisaiList.Add(KeyCode)
                                End Select

                                Dim KeyCodeDataSoufu() As String = New String() {ToriInfo.TORIS_CODE, _
                                                                                 ToriInfo.TORIF_CODE, _
                                                                                 strFURI_DATE, _
                                                                                 TakouInfo.TKIN_NO}
                                '�����U�֐����f�[�^���t�[
                                FurikaeDataInvoice.Add(KeyCodeDataSoufu)

                            End If


                            '-------------------------------------
                            '���s�X�P�W���[���̍쐬
                            '-------------------------------------
                            If InsertTakoschmast(oraSchReader) = False Then
                                Return False
                            End If

                            oraMeiReader.NextRead()
                        End While
                    End If

                    oraMeiReader.Close()


                    oraSchReader.NextRead()

                End While
            Else
                BLOG.Write("���s�f�[�^�쐬", "���s", "�쐬�ΏۃX�P�W���[���Ȃ� �U�֓��F" & strFURI_DATE)
                JobMessage = "���s�f�[�^�쐬�ΏۃX�P�W���[���Ȃ��@�U�֓��F" & strFURI_DATE
                Return False
            End If

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s�A�����b�N
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            '�����܂Ŗ����Ȃ�R�~�b�g
            MainDB.Commit()
            MainDB = Nothing

            '' ���s���Ă����̏ꍇ�̏���
            For intN As Integer = 0 To TakouMeisaiList.Count - 1
                Dim KeyCodeList() As String = CType(TakouMeisaiList(intN), String())

                Dim strTorisCode As String = KeyCodeList(0)
                Dim strTorifCode As String = KeyCodeList(1)
                Dim strFuriDate As String = KeyCodeList(2)
                Dim strKeiyakuKin As String = KeyCodeList(3)

                BLOG.ToriCode = strTorisCode & strTorifCode
                BLOG.FuriDate = strFURI_DATE

                '���s���ו\���
                BLOG.Write("���s���ו\���(�J�n)", "����", "���Z�@�փR�[�h�F" & strKeiyakuKin)
                If PrnTakoumeisaiList(strTorisCode, strTorifCode, strFuriDate, strKeiyakuKin) = False Then
                    BLOG.Write("���s���ו\���", "���s", "���Z�@�փR�[�h�F" & strKeiyakuKin)

                    JobMessage = "���ו\������s ���Z�@�փR�[�h�F" & strKeiyakuKin
                    Return False
                End If
                BLOG.Write("���s���ו\���(�I��)", "����", "���Z�@�փR�[�h�F" & strKeiyakuKin)
            Next

            For intM As Integer = 0 To FurikaeDataInvoice.Count - 1
                Dim KeyCodeList() As String = CType(FurikaeDataInvoice(intM), String())

                Dim strTorisCode As String = KeyCodeList(0)
                Dim strTorifCode As String = KeyCodeList(1)
                Dim strFuriDate As String = KeyCodeList(2)
                Dim strKeiyakuKin As String = KeyCodeList(3)

                BLOG.ToriCode = strTorisCode & strTorifCode
                BLOG.FuriDate = strFuriDate

                '�����U�֐����f�[�^���t�[
                BLOG.Write("�����U�֐����f�[�^���t�[(�J�n)", "����", "���Z�@�փR�[�h�F" & strKeiyakuKin)
                If PrnKouzafurikaeSeikyuDataInvoice(strTorisCode, strTorifCode, strFuriDate, strKeiyakuKin) = False Then
                    BLOG.Write("�����U�֐����f�[�^���t�[���", "���s", "���Z�@�փR�[�h�F" & strKeiyakuKin)
                    JobMessage = "�����U�֐����f�[�^���t�[������s ���Z�@�փR�[�h�F" & strKeiyakuKin
                    Return False
                End If
                BLOG.Write("�����U�֐����f�[�^���t�[(�I��)", "����", "���Z�@�փR�[�h�F" & strKeiyakuKin)

            Next

            Return True

        Catch ex As Exception
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "���s�f�[�^�쐬", "���s", ex.Message)
            JobMessage = "���s�f�[�^�쐬�i���C���j���s �����R�[�h�F" & BLOG.ToriCode
            Return False
        Finally
            If Not oraSchReader Is Nothing Then
                oraSchReader.Close()
                oraSchReader = Nothing
            End If

            If Not oraMeiReader Is Nothing Then
                oraMeiReader.Close()
                oraMeiReader = Nothing
            End If

            '�Ō�܂Ŏc���Ă����烍�[���o�b�N
            If Not MainDB Is Nothing Then
                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                ' �W���u���s�A�����b�N
                dblock.Job_UnLock(MainDB)
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If


        End Try

    End Function

    ''' <summary>
    ''' NHK�t�H�[�}�b�g�̑��s�f�[�^���쐬���܂��B(120�o�C�g)
    ''' </summary>
    ''' <param name="createFlag">���t���O</param>
    ''' <returns>True or False</returns>
    ''' <remarks>���S�M�� �n�[�h�X���Ή��ɂĉ��C</remarks>
    Private Function fn_CREATE_FILE_NHK(ByVal createFlag As Boolean) As Boolean
        fn_CREATE_FILE_NHK = False
        Dim intRECORD_COUNT As Integer

        '---------------------------------------------
        '�쐬�Ώۋ��Z�@�ւ̃f�[�^�𒊏o
        '---------------------------------------------
        Dim SQL As StringBuilder
        SQL = New StringBuilder(128)
        SQL.Append("SELECT * FROM MEIMAST")
        If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then    '�_���Ή��P�̃t�@�C���ɂ܂Ƃ߂č쐬
            SQL.Append(" WHERE TORIS_CODE_K '" & ToriInfo.TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_K = '" & ToriInfo.TORIF_CODE & "'")
            SQL.Append(" AND FURI_DATE_K = '" & strFURI_DATE & "'")
            'SQL.Append(" AND KEIYAKU_KIN_K BETWEEN  '5874' AND '5939'")
            SQL.Append(" AND KEIYAKU_KIN_K BETWEEN  " & SQ(IniInfo.NOUKYOFROM) & " AND " & SQ(IniInfo.NOUKYOTO)) 'INI�t�@�C���̐ݒ�𔽉f
            SQL.Append(" AND DATA_KBN_K = '2'")

            '*** �C�� mitsu 2008/08/12 �܂Ƃ߃f�[�^�쐬�t���O ***
            blnMakeMATOME_DATA = True
            '****************************************************
        Else
            SQL.Append(" WHERE TORIS_CODE_K = '" & ToriInfo.TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_K = '" & ToriInfo.TORIF_CODE & "'")
            SQL.Append(" AND FURI_DATE_K = '" & strFURI_DATE & "'")
            SQL.Append(" AND KEIYAKU_KIN_K = '" & TakouInfo.TKIN_NO & "'")
            SQL.Append(" AND DATA_KBN_K = '2'")
        End If
        '*** �C�� mitsu 2008/08/12 ���Z�@�փ\�[�g�ǉ� ***
        'SQL.Append(" ORDER BY KEIYAKU_SIT_K,RECORD_NO_K ASC")
        SQL.Append(" ORDER BY KEIYAKU_KIN_K, KEIYAKU_SIT_K, RECORD_NO_K")
        '************************************************

        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        '*** �C�� mitsu 2008/08/11 strKEIYAKU_KIN�̌����E���z ***
        Dim lngTKIN_KEN As Long = 0
        Dim lngTKIN_KIN As Long = 0
        '********************************************************
        Dim dblYUUKIN_KEN As Long = 0
        Dim dblZEROKIN_KEN As Long = 0

        '  ����NHK�t�@�C�������Ȃ� >>
        If createFlag = False Then
            dblGOUKEI_KEN = 0
            dblGOUKEI_KIN = 0

            '*** �C�� mitsu 2008/08/12 ��O�����ǉ� ***
            Try
                BLOG.Write("���ׂ̌���(�J�n)", "����", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE)
                '**************************************
                If OraReader.DataReader(SQL) = True Then
                    While (OraReader.EOF = False)
                        dblGOUKEI_KEN += 1
                        dblGOUKEI_KIN += GCOM.NzDec(OraReader.GetItem("FURIKIN_K"))

                        '*** �C�� mitsu 2008/08/11 strKEIYAKU_KIN�̌����E���z ***
                        If OraReader.GetItem("KEIYAKU_KIN_K") = TakouInfo.TKIN_NO Then
                            lngTKIN_KEN += 1
                            lngTKIN_KIN += GCOM.NzLong(OraReader.GetItem("FURIKIN_K"))
                        End If
                        '********************************************************

                        OraReader.NextRead()
                    End While

                    '*** �C�� mitsu 2008/08/11 strKEIYAKU_KIN�̌����E���z ***
                    'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
                    If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                        dblGOUKEI_KEN = lngTKIN_KEN
                        dblGOUKEI_KIN = lngTKIN_KIN
                    End If
                    '********************************************************

                    Return True
                End If

                '*** �C�� mitsu 2008/08/12 ��O�����ǉ� ***
            Catch EX As Exception
                BLOG.Write("���ׂ̌���", "���s", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE & " " & EX.Message)
               JobMessage = "���ׂ̌��� ���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE
                Return False
            Finally
                OraReader.Close()
            End Try
            '**********************************************
            BLOG.Write("���ׂ̌���(�I��)", "����", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE)
            End If
        ' 2008.04.09 ADD <<

        intRECORD_COUNT = 1
        dblGOUKEI_KEN = 0
        dblGOUKEI_KIN = 0

        '--------------------------------------------
        '�t�@�C���̃I�[�v��
        '--------------------------------------------
        If File.Exists(strWRK_FILE_NAME) = True Then
            File.Delete(strWRK_FILE_NAME)
        End If

        Dim oFusion As New clsFUSION.clsMain

        Dim nhkFormat As New CAstFormat.CFormatNHK
        Dim nhkStreamWriter As New StreamWriter(strWRK_FILE_NAME, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

        '--------------------------------------------
        '�w�b�_�[���R�[�h�̏�����
        '--------------------------------------------
        nhkFormat.NHK_REC1.NH01 = "1"                                   '�f�[�^�敪
        Select Case ToriInfo.NS_KBN
            Case "1"
                nhkFormat.NHK_REC1.NH02 = "21"                          '��ʃR�[�h
            Case "9"
                nhkFormat.NHK_REC1.NH02 = "91"
        End Select
        nhkFormat.NHK_REC1.NH03 = "1"                                   '�R�[�h�敪
        nhkFormat.NHK_REC1.NH04 = TakouInfo.ITAKU_CODE                   '���s�ϑ��҃R�[�h
        nhkFormat.NHK_REC1.NH05 = strHEDDA_FURI_DATA.Substring(14, 40)  '�ϑ��Җ�
        nhkFormat.NHK_REC1.NH06 = strFURI_DATE.Substring(4, 4)          '�U�֓�
        nhkFormat.NHK_REC1.NH07 = TakouInfo.TKIN_NO                        '���Z�@�փR�[�h
        nhkFormat.NHK_REC1.NH08 = ""
        nhkFormat.NHK_REC1.NH09 = ""
        nhkFormat.NHK_REC1.NH10 = ""
        nhkFormat.NHK_REC1.NH11 = ""
        nhkFormat.NHK_REC1.NH12 = ""
        nhkFormat.NHK_REC1.NH13 = ""
        nhkFormat.NHK_REC1.NH14 = TakouInfo.TKIN_NO                    '�������Z�@��
        nhkFormat.NHK_REC1.NH15 = "*"

        nhkStreamWriter.Write(nhkFormat.NHK_REC1.Data)
        intRECORD_COUNT += 1

        Try
            BLOG.Write("�f�[�^���R�[�h�̏���(�J�n)", "����", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE)
            If OraReader.DataReader(SQL) = True Then
                While (OraReader.EOF = False)
                    '--------------------------------------------
                    '�f�[�^���R�[�h�̏�����
                    '--------------------------------------------
                    nhkFormat.RecordData = OraReader.GetItem("FURI_DATA_K")
                    dblGOUKEI_KEN += 1
                    dblGOUKEI_KIN += GCOM.NzDec(OraReader.GetItem("FURIKIN_K"))

                    '*** �C�� mitsu 2008/08/11 strKEIYAKU_KIN�̌����E���z ***
                    If OraReader.GetItem("KEIYAKU_KIN_K") = TakouInfo.TKIN_NO Then
                        lngTKIN_KEN += 1
                        lngTKIN_KIN += GCOM.NzLong(OraReader.GetItem("FURIKIN_K"))
                    End If
                    '********************************************************
                    If GCOM.NzLong(OraReader.GetItem("FURIKIN_K")) = 0 Then
                        dblZEROKIN_KEN += 1
                    Else
                        dblYUUKIN_KEN += 1
                    End If

                    '    nhkFormat.RecordData = nhkFormat.RecordData.Substring(0, 112) & " " & nJIFURI_SEQ.ToString("00000000")
                    '
                    '    Try
                    '        SQL = New StringBuilder(128)
                    '        Sql.Append ("UPDATE MEIMAST SET KIGYO_SEQ_K = '" & nJIFURI_SEQ.ToString & "'")
                    '        Sql.Append (" WHERE TORIS_CODE_K ='" & strTORIS_CODE & "'")
                    '        Sql.Append ("   AND TORIF_CODE_K ='" & strTORIF_CODE & "'")
                    '        Sql.Append ("   AND FURI_DATE_K ='" & strFURI_DATE & "'")
                    '        Sql.Append ("   AND RECORD_NO_K = " & OraReader.GetItem("RECORD_NO_K"))
                    '
                    '        MainDB.ExecuteNonQuery (Sql)
                    '
                    '    Catch EX As Exception
                    '        '*** �C�� mitsu 2008/08/12 �G���[���b�Z�[�W���ǉ� ***
                    '        BLOG.Write("��ƃV�[�P���X�̓o�^", "���s", "���Z�@�փR�[�h�F" & strKEIYAKU_KIN & " �����R�[�h�F" & strTORIS_CODE & "-" & strTORIF_CODE & " �U�֓��F" & strFURI_DATE & " " & EX.Message)
                    '        JobMessage = "��ƃV�[�P���X�̓o�^���s ���Z�@�փR�[�h�F" & strKEIYAKU_KIN & " �����R�[�h�F" & strTORIS_CODE & "-" & strTORIF_CODE & " �U�֓��F" & strFURI_DATE
                    '        '******************************************************
                    '        Return False
                    '    End Try

                    '   nJIFURI_SEQ += 1

                    nhkStreamWriter.Write(nhkFormat.RecordData)
                    intRECORD_COUNT += 1

                    OraReader.NextRead()
                End While
            End If
            BLOG.Write("�f�[�^���R�[�h�̏���(�I��)", "����", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE)
            
        Catch EX As Exception
            '*** �C�� mitsu 2008/08/12 �G���[���b�Z�[�W���ǉ� ***
            BLOG.Write("�t�@�C���̍쐬", "���s", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE & " " & EX.Message)
            JobMessage = "�t�@�C���̍쐬 ���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE
            '******************************************************
            Return False
        Finally
            OraReader.Close()
        End Try

        '--------------------------------------------
        '�g���[�����R�[�h�̏�����
        '--------------------------------------------
        nhkFormat.NHK_REC8.NH01 = "8"
        nhkFormat.NHK_REC8.NH02 = dblGOUKEI_KEN.ToString("000000")              '��������
        nhkFormat.NHK_REC8.NH03 = dblGOUKEI_KIN.ToString("000000000000")        '�������z
        nhkFormat.NHK_REC8.NH04 = New String("0"c, 6)                           '�U�֍ό���
        nhkFormat.NHK_REC8.NH05 = New String("0"c, 12)                          '�U�֍ϋ��z
        nhkFormat.NHK_REC8.NH06 = New String("0"c, 6)                           '�U�֕s�\����
        nhkFormat.NHK_REC8.NH07 = New String("0"c, 12)                          '�U�֕s�\���z
        nhkFormat.NHK_REC8.NH08 = New String(" "c, 12)                          '�U�֎萔��
        nhkFormat.NHK_REC8.NH09 = New String(" "c, 12)                          '�����z
        nhkFormat.NHK_REC8.NH10 = dblGOUKEI_KEN.ToString("000000")              '���R�[�h��
        nhkFormat.NHK_REC8.NH11 = dblYUUKIN_KEN.ToString("000000")              '�L���z
        nhkFormat.NHK_REC8.NH12 = dblZEROKIN_KEN.ToString("000000")             '���z�u�O�v����
        nhkFormat.NHK_REC8.NH13 = New String(" "c, 15)                          '�_�~�[
        nhkFormat.NHK_REC8.NH14 = New String("0"c, 3)                           '�g���[������
        nhkFormat.NHK_REC8.NH15 = New String(" "c, 4)                           '�\��
        nhkFormat.NHK_REC8.NH16 = "*"
        nhkStreamWriter.Write(nhkFormat.NHK_REC8.Data)
        intRECORD_COUNT += 1

        '*** �C�� mitsu 2008/08/11 strKEIYAKU_KIN�̌����E���z ***
        'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
        If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
            dblGOUKEI_KEN = lngTKIN_KEN
            dblGOUKEI_KIN = lngTKIN_KIN
        End If
        '********************************************************

        '--------------------------------------------
        '�G���h���R�[�h�̏�����
        '--------------------------------------------
        nhkFormat.NHK_REC9.NH01 = "9"
        nhkFormat.NHK_REC9.NH02 = New String(" "c, 111)
        nhkFormat.NHK_REC9.NH03 = New String(" "c, 3)
        nhkFormat.NHK_REC9.NH04 = TakouInfo.TKIN_NO
        nhkFormat.NHK_REC9.NH05 = "*"
        nhkStreamWriter.Write(nhkFormat.NHK_REC9.Data)
        nhkStreamWriter.Close()

        fn_CREATE_FILE_NHK = True
    End Function

    ''' <summary>
    ''' �S��t�H�[�}�b�g�̑��s�f�[�^���쐬���܂��B(120�o�C�g)
    ''' </summary>
    ''' <param name="createFlag">���t���O</param>
    ''' <returns>True or False</returns>
    ''' <remarks>���S�M�� �n�[�h�X���Ή��ɂĉ��C</remarks>
    Private Function fn_CREATE_FILE_ZENGIN(ByVal createFlag As Boolean) As Boolean
        '============================================================================
        'NAME           :fn_CREATE_FILE_120
        'Parameter      :
        'Description    :120�o�C�g�̃t�@�C��(JIS)�쐬
        'Return         :True=OK(����),False=NG�i���s�j
        'Create         :2004/08/19
        'Update         :
        '============================================================================
        fn_CREATE_FILE_ZENGIN = False
        Dim intRECORD_COUNT As Integer

        '---------------------------------------------
        '�쐬�Ώۋ��Z�@�ւ̃f�[�^�𒊏o
        '---------------------------------------------
        Dim SQL As StringBuilder
        SQL = New StringBuilder(128)
        SQL.Append("SELECT * FROM MEIMAST")
        If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then    '�_���Ή��P�̃t�@�C���ɂ܂Ƃ߂č쐬
            SQL.Append(" WHERE TORIS_CODE_K = '" & ToriInfo.TORIS_CODE & _
                                   "' AND TORIF_CODE_K = '" & ToriInfo.TORIF_CODE & _
                                   "' AND FURI_DATE_K = '" & strFURI_DATE & _
                                   "' AND KEIYAKU_KIN_K BETWEEN  " & SQ(IniInfo.NOUKYOFROM) & " AND " & SQ(IniInfo.NOUKYOTO) & " AND DATA_KBN_K = '2'") 'INI�t�@�C���̐ݒ�𔽉f
            '*** �C�� mitsu 2008/08/12 �܂Ƃ߃f�[�^�쐬�t���O ***
            blnMakeMATOME_DATA = True
            '****************************************************
        Else
            SQL.Append(" WHERE TORIS_CODE_K = '" & ToriInfo.TORIS_CODE & _
                                   "' AND TORIF_CODE_K = '" & ToriInfo.TORIF_CODE & _
                                   "' AND FURI_DATE_K = '" & strFURI_DATE & _
                                   "' AND KEIYAKU_KIN_K = '" & TakouInfo.TKIN_NO & _
                                   "' AND DATA_KBN_K = '2'")
        End If
        'If strParaTORI_CODE = New String("9", 7) & "88" And strTEIKEI_KBN <> "2" Then
        '    SQL.Append(" AND EXISTS (")
        '    SQL.Append(" SELECT BAITAI_CODE_V")
        '    SQL.Append("   FROM TAKOUMAST")
        '    SQL.Append("  WHERE TORIS_CODE_V = TORIS_CODE_K")
        '    SQL.Append("    AND TORIF_CODE_V = TORIF_CODE_K")
        '    If strTEIKEI_KBN = "1" Then
        '        ' �`��
        '        SQL.Append(" AND BAITAI_CODE_V  = '00'")
        '    Else
        '        ' �`���ȊO
        '        SQL.Append(" AND BAITAI_CODE_V  <> '00'")
        '    End If
        '    SQL.Append(" )")
        'End If
        '*** �C�� mitsu 2008/08/12 ���Z�@�փ\�[�g�ǉ� ***
        'SQL.Append(" ORDER BY KEIYAKU_SIT_K,RECORD_NO_K ASC")
        SQL.Append(" ORDER BY KEIYAKU_KIN_K, KEIYAKU_SIT_K, RECORD_NO_K")
        '************************************************

        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        '*** �C�� mitsu 2008/08/11 strKEIYAKU_KIN�̌����E���z ***
        Dim lngTKIN_KEN As Long = 0
        Dim lngTKIN_KIN As Long = 0
        '********************************************************

        ' 2008.04.09 ADD ���͑S��t�@�C�������Ȃ� >>
        If createFlag = False Then
            dblGOUKEI_KEN = 0
            dblGOUKEI_KIN = 0

            '*** �C�� mitsu 2008/08/12 ��O�����ǉ� ***
            Try
                BLOG.Write("���ׂ̌���(�J�n)", "����", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE)
                '**************************************
                If OraReader.DataReader(SQL) = True Then
                    While (OraReader.EOF = False)
                        dblGOUKEI_KEN += 1
                        dblGOUKEI_KIN += GCOM.NzDec(OraReader.GetItem("FURIKIN_K"))

                        '*** �C�� mitsu 2008/08/11 strKEIYAKU_KIN�̌����E���z ***
                        If OraReader.GetItem("KEIYAKU_KIN_K") = TakouInfo.TKIN_NO Then
                            lngTKIN_KEN += 1
                            lngTKIN_KIN += GCOM.NzLong(OraReader.GetItem("FURIKIN_K"))
                        End If
                        '********************************************************

                        OraReader.NextRead()
                    End While

                    '*** �C�� mitsu 2008/08/11 strKEIYAKU_KIN�̌����E���z ***
                    'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
                    If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                        dblGOUKEI_KEN = lngTKIN_KEN
                        dblGOUKEI_KIN = lngTKIN_KIN
                    End If
                    '********************************************************

                    Return True
                End If

                '*** �C�� mitsu 2008/08/12 ��O�����ǉ� ***
            Catch EX As Exception
                BLOG.Write("���ׂ̌���", "���s", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE & " " & EX.Message)
                JobMessage = "���ׂ̌��� ���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE
                Return False
            Finally
                OraReader.Close()
            End Try
            '**********************************************
            BLOG.Write("���ׂ̌���(�I��)", "����", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE)
           End If
        ' 2008.04.09 ADD <<

        intRECORD_COUNT = 1
        dblGOUKEI_KEN = 0
        dblGOUKEI_KIN = 0

        '--------------------------------------------
        '�t�@�C���̃I�[�v��
        '--------------------------------------------
        If File.Exists(strWRK_FILE_NAME) = True Then
            File.Delete(strWRK_FILE_NAME)
        End If

        Dim oFusion As New clsFUSION.clsMain

        ' 2008.01.17 >>
        'intFILE_NO_1 = FreeFile()
        'FileOpen(intFILE_NO_1, strWRK_FILE_NAME, OpenMode.Random, , , 120)    '���[�N�t�@�C��
        Dim ZenFmt As New CAstFormat.CFormatZengin
        Dim ZenSWri As New StreamWriter(strWRK_FILE_NAME, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
        ' 2008.01.17 <<
        'FileOpen(intFILE_NO_1, strWRK_FILE_NAME, OpenMode.Random)    '���[�N�t�@�C��

        '--------------------------------------------
        '�w�b�_�[���R�[�h�̏�����
        '--------------------------------------------
        ZenFmt.ZENGIN_REC1.ZG1 = "1"
        Select Case ToriInfo.NS_KBN
            Case "1"
                ZenFmt.ZENGIN_REC1.ZG2 = "21"
            Case "9"
                ZenFmt.ZENGIN_REC1.ZG2 = "91"
        End Select
        ZenFmt.ZENGIN_REC1.ZG3 = "1"
        ZenFmt.ZENGIN_REC1.ZG4 = TakouInfo.ITAKU_CODE
        ZenFmt.ZENGIN_REC1.ZG5 = strHEDDA_FURI_DATA.Substring(14, 40)
        ZenFmt.ZENGIN_REC1.ZG6 = strFURI_DATE.Substring(4, 4)
        ZenFmt.ZENGIN_REC1.ZG7 = TakouInfo.TKIN_NO
        '���Z�@�֖��擾
        ZenFmt.ZENGIN_REC1.ZG8 = GCOM.GetBKBRKName(TakouInfo.TKIN_NO, "").PadRight(30).Substring(0, 15)
        ZenFmt.ZENGIN_REC1.ZG10 = GCOM.GetBKBRKName(TakouInfo.TKIN_NO, TakouInfo.TSIT_NO).PadRight(30).Substring(0, 15)
        ZenFmt.ZENGIN_REC1.ZG9 = TakouInfo.TSIT_NO
        '2013/07/22 saitou ���S�M�� UPD -------------------------------------------------->>>>
        '�ʒm�a���ǉ����Ăяo���֐��ύX
        ZenFmt.ZENGIN_REC1.ZG11 = CASTCommon.ConvertKamoku2TO1(TakouInfo.KAMOKU)
        'ZenFmt.ZENGIN_REC1.ZG11 = oFusion.fn_CHG_KAMOKU2TO1(TakouInfo.KAMOKU)
        '2013/07/22 saitou ���S�M�� UPD --------------------------------------------------<<<<
        ZenFmt.ZENGIN_REC1.ZG12 = TakouInfo.KOUZA
        ZenFmt.ZENGIN_REC1.ZG13 = New String(" "c, 17)

        ZenSWri.Write(ZenFmt.ZENGIN_REC1.Data)

        intRECORD_COUNT += 1

        Try
            BLOG.Write("�f�[�^���R�[�h�̏���(�J�n)", "����", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE)
             If OraReader.DataReader(SQL) = True Then
                While (OraReader.EOF = False)
                    '--------------------------------------------
                    '�f�[�^���R�[�h�̏�����
                    '--------------------------------------------
                    'Dim strFURI_DATA As String
                    ZenFmt.RecordData = OraReader.GetItem("FURI_DATA_K")
                    dblGOUKEI_KEN += 1
                    dblGOUKEI_KIN += GCOM.NzDec(OraReader.GetItem("FURIKIN_K"))

                    ''2010/01/05 ���ʍX�V�Ŗ������Ȃ��悤�ɂ��邽�߁A�}�X�^�̒l��ݒ肷�� > �s�v(2010/01/12)
                    'ZenFmt.ZENGIN_REC2.Data = ZenFmt.RecordData
                    'ZenFmt.ZENGIN_REC2.ZG2 = OraReader.GetItem("KEIYAKU_KIN_K")                                     '���Z�@�փR�[�h
                    'ZenFmt.ZENGIN_REC2.ZG4 = OraReader.GetItem("KEIYAKU_SIT_K")                                     '�x�X�R�[�h
                    'ZenFmt.ZENGIN_REC2.ZG7 = CASTCommon.ConvertKamoku2TO1(OraReader.GetItem("KEIYAKU_KAMOKU_K"))    '�Ȗ�
                    'ZenFmt.ZENGIN_REC2.ZG8 = OraReader.GetItem("KEIYAKU_KOUZA_K")                                   '�����ԍ�
                    'ZenFmt.ZENGIN_REC2.ZG9 = GCOM.NzStr(OraReader.GetItem("KEIYAKU_KNAME_K"))                       '�_��Җ�
                    'ZenFmt.ZENGIN_REC2.ZG10 = GCOM.NzStr(OraReader.GetItem("FURIKIN_K")).PadLeft(10, "0"c)          '�U�֋��z
                    'ZenFmt.ZENGIN_REC2.ZG12 = Mid(GCOM.NzStr(OraReader.GetItem("JYUYOUKA_NO_K")), 1, 10)            '���v�Ɣԍ�
                    'ZenFmt.ZENGIN_REC2.ZG13 = Mid(GCOM.NzStr(OraReader.GetItem("JYUYOUKA_NO_K")), 11, 10)           '���v�Ɣԍ�
                    'ZenFmt.RecordData = ZenFmt.ZENGIN_REC2.Data
                    ''=================================================

                    '*** �C�� mitsu 2008/08/11 strKEIYAKU_KIN�̌����E���z ***
                    If OraReader.GetItem("KEIYAKU_KIN_K") = TakouInfo.TKIN_NO Then
                        lngTKIN_KEN += 1
                        lngTKIN_KIN += GCOM.NzLong(OraReader.GetItem("FURIKIN_K"))
                    End If
                    '********************************************************
                    '2010/01/04 ��ƃV�[�P���X�����邩���t���O�Ŕ���
                    If IniInfo.TAKO_SEQ = "1" Then

                        '���ʍX�V�ŕK�v�Ȃ��ߕ��� ==================================
                        ZenFmt.RecordData = ZenFmt.RecordData.Substring(0, 112) & nJIFURI_SEQ.ToString("00000000")

                        Try
                            SQL = New StringBuilder(128)
                            SQL.Append("UPDATE MEIMAST SET KIGYO_SEQ_K = '" & nJIFURI_SEQ.ToString & "'")
                            SQL.Append(" WHERE TORIS_CODE_K ='" & ToriInfo.TORIS_CODE & "'")
                            SQL.Append("   AND TORIF_CODE_K ='" & ToriInfo.TORIF_CODE & "'")
                            SQL.Append("   AND FURI_DATE_K ='" & strFURI_DATE & "'")
                            SQL.Append("   AND RECORD_NO_K = " & OraReader.GetItem("RECORD_NO_K"))

                            MainDB.ExecuteNonQuery(SQL)

                        Catch EX As Exception
                            '*** �C�� mitsu 2008/08/12 �G���[���b�Z�[�W���ǉ� ***
                            BLOG.Write("��ƃV�[�P���X�̓o�^", "���s", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE & " " & EX.Message)
                            JobMessage = "��ƃV�[�P���X�̓o�^���s ���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE
                            '******************************************************
                            Return False
                        End Try

                        nJIFURI_SEQ += 1
                        '============================================================
                    End If
                    ' 2008.01.17 >>
                    'FilePut(intFILE_NO_1, gstrDATA, intRECORD_COUNT)
                    ZenSWri.Write(ZenFmt.RecordData)
                    ' 2008.01.17 <<
                    intRECORD_COUNT += 1

                    OraReader.NextRead()
                End While
            End If

            BLOG.Write("�f�[�^���R�[�h�̏���(�I��)", "����", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE)
            
        Catch EX As Exception
            '*** �C�� mitsu 2008/08/12 �G���[���b�Z�[�W���ǉ� ***
            BLOG.Write("�t�@�C���̍쐬", "���s", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE & " " & EX.Message)
            JobMessage = "�t�@�C���̍쐬 ���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " �����R�[�h�F" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " �U�֓��F" & strFURI_DATE
            '******************************************************
            Return False
        Finally
            OraReader.Close()
        End Try

        '--------------------------------------------
        '�g���[�����R�[�h�̏�����
        '--------------------------------------------
        ZenFmt.ZENGIN_REC8.ZG1 = "8"
        ZenFmt.ZENGIN_REC8.ZG2 = dblGOUKEI_KEN.ToString("000000")
        ZenFmt.ZENGIN_REC8.ZG3 = dblGOUKEI_KIN.ToString("000000000000")
        '*** �C�� mitsu 2008/08/11 �U�֍ρE�s�\����0���߂��� ***
        'ZenFmt.ZENGIN_REC8.ZG4 = New String(" "c, 101)
        ZenFmt.ZENGIN_REC8.ZG4 = New String("0"c, 6)
        ZenFmt.ZENGIN_REC8.ZG5 = New String("0"c, 12)
        ZenFmt.ZENGIN_REC8.ZG6 = New String("0"c, 6)
        ZenFmt.ZENGIN_REC8.ZG7 = New String("0"c, 12)
        ZenFmt.ZENGIN_REC8.ZG8 = New String(" "c, 65)
        '*******************************************************
        ' 2008.01.17 >>
        'FilePut(intFILE_NO_1, gZENGIN_REC8, intRECORD_COUNT)
        ZenSWri.Write(ZenFmt.ZENGIN_REC8.Data)
        ' 2008.01.17 <<
        intRECORD_COUNT += 1

        '*** �C�� mitsu 2008/08/11 strKEIYAKU_KIN�̌����E���z ***
        'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
        If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
            dblGOUKEI_KEN = lngTKIN_KEN
            dblGOUKEI_KIN = lngTKIN_KIN
        End If
        '********************************************************

        '--------------------------------------------
        '�G���h���R�[�h�̏�����
        '--------------------------------------------
        ZenFmt.ZENGIN_REC9.ZG1 = "9"
        ZenFmt.ZENGIN_REC9.ZG2 = New String(" "c, 119)
        ' 2008.01.17 >>
        'FilePut(intFILE_NO_1, gZENGIN_REC9, intRECORD_COUNT)

        'FileClose(intFILE_NO_1)
        ZenSWri.Write(ZenFmt.ZENGIN_REC9.Data)
        ZenSWri.Close()
        ' 2008.01.17 <<

        fn_CREATE_FILE_ZENGIN = True
    End Function

    ''' <summary>
    ''' �X�P�W���[���}�X�^���Q�Ƃ���SQL���쐬���܂��B
    ''' </summary>
    ''' <returns>�쐬SQL</returns>
    ''' <remarks>���S�M�� �n�[�h�X���Ή��ɂĐV�K�ǉ�</remarks>
    Private Function CreateGetSchmastSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            '���s�쐬�Ώۂ̏W����s�ȊO�������Ƃ���
            .Append("select * from SCHMAST")
            .Append(" inner join TORIMAST")
            .Append(" on TORIS_CODE_S = TORIS_CODE_T")
            .Append(" and TORIF_CODE_S = TORIF_CODE_T")
            .Append(" where FURI_DATE_S = " & SQ(strFURI_DATE))
            .Append(" and TORIS_CODE_S = " & SQ(strTORIS_CODE))
            .Append(" and TORIF_CODE_S = " & SQ(strTORIF_CODE))
            .Append(" and TOUROKU_FLG_S = '1'")
            .Append(" and HAISIN_FLG_S = '0'")
            .Append(" and TYUUDAN_FLG_S = '0'")
            .Append(" and TAKO_KBN_T = '1'")
            '2017/01/18 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
            '�W����s��20(��)��21(���O)
            .Append(" and FMT_KBN_T not in ('20', '21')")
            '.Append(" and FMT_KBN_T <> '20'")
            '2017/01/18 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' ���s�X�P�W���[���}�X�^���폜���܂��B
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function DeleteTakoschmast() As Boolean
        Dim SQL As New StringBuilder
        With SQL
            .Append("delete from TAKOSCHMAST")
            .Append(" where FURI_DATE_U = " & SQ(strFURI_DATE))
            .Append(" and TORIS_CODE_U = " & SQ(strTORIS_CODE))
            .Append(" and TORIF_CODE_U = " & SQ(strTORIF_CODE))
        End With

        Try
            BLOG.Write("(���s�X�P�W���[���}�X�^�폜)�J�n", "����", "�����R�[�h�F" & strTORI_CODE & " �U�֓��F" & strFURI_DATE)
            '�Ώۂ����݂��Ȃ��Ă������A�G���[�ɂȂ�Ȃ���΂���
            If MainDB.ExecuteNonQuery(SQL) < 0 Then
                BLOG.Write("���s�X�P�W���[���}�X�^�폜", "���s", "�����R�[�h�F" & strTORI_CODE & " �U�֓��F" & strFURI_DATE)
                Return False
            End If
            Return True
        Catch ex As Exception
            BLOG.Write("���s�X�P�W���[���}�X�^�폜", "���s", "�����R�[�h�F" & strTORI_CODE & " �U�֓��F" & strFURI_DATE & " " & ex.Message)
            Return False
        Finally
            BLOG.Write("(���s�X�P�W���[���}�X�^�폜)�I��", "����", "�����R�[�h�F" & strTORI_CODE & " �U�֓��F" & strFURI_DATE)
             End Try
    End Function

    ''' <summary>
    ''' ���s�X�P�W���[���}�X�^��o�^���܂��B
    ''' </summary>
    ''' <param name="oraReader">�X�P�W���[���I���N�����[�_�[</param>
    ''' <returns>True or False</returns>
    ''' <remarks>���S�M�� �n�[�h�X���Ή��ɂĐV�K�ǉ�</remarks>
    Private Function InsertTakoschmast(ByVal oraReader As CASTCommon.MyOracleReader) As Boolean
        Dim SQL As New StringBuilder
        With SQL
            .Append("insert into TAKOSCHMAST values (")
            .Append(" " & SQ(oraReader.GetString("TORIS_CODE_S")))
            .Append("," & SQ(oraReader.GetString("TORIF_CODE_S")))
            .Append("," & SQ(oraReader.GetString("FURI_DATE_S")))
            .Append("," & SQ(oraReader.GetString("FUNOU_YDATE_S")))
            .Append("," & SQ(oraReader.GetString("FMT_KBN_T")))
            .Append("," & SQ(TakouInfo.BAITAI_CODE))
            .Append("," & SQ(oraReader.GetString("LABEL_KBN_T")))
            .Append("," & SQ(TakouInfo.CODE_KBN))
            .Append("," & SQ(TakouInfo.TKIN_NO))
            .Append("," & SQ("0"))
            .Append("," & dblGOUKEI_KEN.ToString)
            .Append("," & dblGOUKEI_KIN.ToString)
            .Append(",0")
            .Append(",0")
            .Append(",0")
            .Append(",0")
            .Append("," & SQ(strDate))
            .Append(")")
        End With

        Dim DELSQL As New StringBuilder
        With DELSQL
            .Append("delete from TAKOSCHMAST")
            .Append(" where TORIS_CODE_U = " & SQ(oraReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_U = " & SQ(oraReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_U = " & SQ(oraReader.GetString("FURI_DATE_S")))
            .Append(" and TKIN_NO_U = " & SQ(TakouInfo.TKIN_NO))
        End With

        Try
            Dim nRet As Integer = MainDB.ExecuteNonQuery(DELSQL)
            If nRet > 0 Then
                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "���s�X�P�W���[���쐬", "����", "��s���R�[�h���폜")
            End If

            nRet = MainDB.ExecuteNonQuery(SQL)
            If nRet > 0 Then
                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "���s�X�P�W���[���쐬", "����")
            End If

            Return True

        Catch ex As Exception
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "���s�X�P�W���[���쐬", "���s", "���Z�@�փR�[�h�F" & TakouInfo.TKIN_NO & " " & ex.Message)
            JobMessage = "���s�X�P�W���[���쐬���s ��O����"
            Return False
        End Try
    End Function

    ''' <summary>
    ''' ���U�V�[�P���X�̍ő�l�{�P���擾���܂��B
    ''' </summary>
    ''' <returns>�ő厩�U�V�[�P���X�{�P</returns>
    ''' <remarks>���S�M�� �n�[�h�X���Ή��ɂĐV�K�ǉ�</remarks>
    Private Function GetMaxJifuriSEQPlusOne() As Long
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder
        With SQL
            .Append("select nvl(max(KIGYO_SEQ_K), 79999999) as JIFURI_SEQ_MAX")
            .Append(", count(KIGYO_SEQ_K) as JIFURI_SEQ_CNT")
            .Append(" from MEIMAST")
            .Append(" where FURI_DATE_K = " & SQ(strFURI_DATE))
            .Append(" and KIGYO_SEQ_K between '80000000' and '89999999'")
        End With

        Try
            If oraReader.DataReader(SQL) = True Then
                Return oraReader.GetInt64("JIFURI_SEQ_MAX") + 1
            Else
                Return 80000000
            End If
        Catch ex As Exception
            Return 80000000
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' ���׃}�X�^����w�b�_���R�[�h���擾����SQL���쐬���܂��B
    ''' </summary>
    ''' <param name="oraReader">�X�P�W���[���I���N�����[�_�[</param>
    ''' <returns>�쐬SQL</returns>
    ''' <remarks>���S�M�� �n�[�h�X���Ή��ɂĐV�K�ǉ�</remarks>
    Private Function CreateGetMeimastHeaderRecord(ByVal oraReader As CASTCommon.MyOracleReader) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select FURI_DATA_K from MEIMAST")
            .Append(" where TORIS_CODE_K = " & SQ(oraReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_K = " & SQ(oraReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_K = " & SQ(oraReader.GetString("FURI_DATE_S")))
            .Append(" and DATA_KBN_K = '1'")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' ���׃}�X�^���瑼�s�f�[�^�쐬�Ώۂ̋��Z�@�ւ��擾���܂��B
    ''' </summary>
    ''' <param name="oraReader">�X�P�W���[���I���N�����[�_�[</param>
    ''' <returns>�쐬SQL</returns>
    ''' <remarks>���S�M�� �n�[�h�X���Ή��ɂĐV�K�ǉ�</remarks>
    Private Function CreateGetMeimastKeiyakuKin(ByVal oraReader As CASTCommon.MyOracleReader) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select distinct KEIYAKU_KIN_K from MEIMAST")
            .Append(" where TORIS_CODE_K = " & SQ(oraReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_K = " & SQ(oraReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_K = " & SQ(oraReader.GetString("FURI_DATE_S")))
            .Append(" and DATA_KBN_K = '2'")
            .Append(" and KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
            .Append(" and FURIKETU_CODE_K = 0")
            .Append(" order by KEIYAKU_KIN_K")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' ���s�}�X�^���Q�Ƃ��܂��B
    ''' </summary>
    ''' <param name="TORIS_CODE">������R�[�h</param>
    ''' <param name="TORIF_CODE">����敛�R�[�h</param>
    ''' <param name="TKIN_NO">���Z�@�փR�[�h</param>
    ''' <returns>True or False</returns>
    ''' <remarks>���S�M�� �n�[�h�X���ɂĐV�K�ǉ�</remarks>
    Private Function GetTakoumast(ByVal TORIS_CODE As String, ByVal TORIF_CODE As String, ByVal TKIN_NO As String) As Boolean
        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            SQL.Append("SELECT * FROM TAKOUMAST")
            SQL.Append(" WHERE TORIS_CODE_V = " & SQ(TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_V = " & SQ(TORIF_CODE))
            SQL.Append(" AND TKIN_NO_V = " & SQ(TKIN_NO))

            If oraReader.DataReader(SQL) = False Then
                Return False
            Else
                TakouInfo.SetOraData(oraReader)
            End If
            Return True
        Catch ex As Exception
            BLOG.Write("���s�}�X�^����", "���s", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' �����}�X�^���Q�Ƃ��܂��B
    ''' </summary>
    ''' <param name="TORIS_CODE">������R�[�h</param>
    ''' <param name="TORIF_CODE">����敛�R�[�h</param>
    ''' <returns>True or False</returns>
    ''' <remarks>���S�M�� �n�[�h�X���ɂĐV�K�ǉ�</remarks>
    Private Function GetTorimast(ByVal TORIS_CODE As String, ByVal TORIF_CODE As String) As Boolean
        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            SQL.Append("SELECT * FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(TORIF_CODE))

            If oraReader.DataReader(SQL) = False Then
                Return False
            Else
                ToriInfo.SetOraData(oraReader)
            End If
            Return True
        Catch ex As Exception
            BLOG.Write("�����}�X�^����", "���s", ex.Message)

            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' ���s���ו\��������܂��B
    ''' </summary>
    ''' <param name="strTorisCode">������R�[�h</param>
    ''' <param name="strTorifCode">����敛�R�[�h</param>
    ''' <param name="strFuriDate">�U�֓�</param>
    ''' <param name="strKeiyakuKin">���Z�@�փR�[�h</param>
    ''' <returns>True or False</returns>
    ''' <remarks>���S�M�� �n�[�h�X���ɂĐV�K�ǉ�</remarks>
    Private Function PrnTakoumeisaiList(ByVal strTorisCode As String, _
                                        ByVal strTorifCode As String, _
                                        ByVal strFuriDate As String, _
                                        ByVal strKeiyakuKin As String) As Boolean
        Try
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim ParamRepo As String = ""
            Dim DQ As String = """"

            ParamRepo = strTorisCode & strTorifCode & _
                        "," & strFuriDate & _
                        "," & strKeiyakuKin
            BLOG.Write("���s���ו\���(�J�n)", "����", ParamRepo)

            Dim nRet As Integer = ExeRepo.ExecReport("KFJP006.EXE", ParamRepo)
            If nRet = 0 Then
                BLOG.Write("���s���ו\���(�I��)", "����")
            Else
                BLOG.Write("���s���ו\���(�I��)", "���s")
            End If

            Return True

        Catch ex As Exception
            BLOG.Write("���s���ו\���", "���s", ex.Message)
            JobMessage = "���s���ו\������s"
            Return False
        End Try

    End Function

    ''' <summary>
    ''' �����U�֐����f�[�^���t�[��������܂��B
    ''' </summary>
    ''' <param name="strTorisCode">������R�[�h</param>
    ''' <param name="strTorifCode">����敛�R�[�h</param>
    ''' <param name="strFuriDate">�U�֓�</param>
    ''' <param name="strKeiyakuKin">���Z�@�փR�[�h</param>
    ''' <returns>True or False</returns>
    ''' <remarks>���S�M�� �n�[�h�X���ɂĐV�K�ǉ�</remarks>
    Private Function PrnKouzafurikaeSeikyuDataInvoice(ByVal strTorisCode As String, _
                                                      ByVal strTorifCode As String, _
                                                      ByVal strFuriDate As String, _
                                                      ByVal strKeiyakuKin As String) As Boolean
        Try
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim ParamRepo As String = ""
            Dim DQ As String = """"

            ParamRepo = strTorisCode & strTorifCode & _
                        "," & strFuriDate & _
                        "," & strKeiyakuKin
            BLOG.Write("�����U�֐����f�[�^���t�[(�J�n)", "����", ParamRepo)

            Dim nRet As Integer = ExeRepo.ExecReport("KFJP007.EXE", ParamRepo)
            If nRet = 0 Then
                BLOG.Write("�����U�֐����f�[�^���t�[(�I��)", "����")
            Else
                BLOG.Write("�����U�֐����f�[�^���t�[(�I��)", "���s")
            End If

            Return True

        Catch ex As Exception
            BLOG.Write("�����U�֐����f�[�^���t�[���", "���s", ex.Message)
            JobMessage = "�����U�֐����f�[�^���t�[������s"
            Return False
        End Try

    End Function

    ''' <summary>
    ''' �}�̏����݂��s���܂��B
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks>���S�M�� �n�[�h�X���ɂĉ��C</remarks>
    Private Function fn_BAITAI_WRITE() As Boolean
        '============================================================================
        'NAME           :fn_BAITAI_WRITE
        'Parameter      :
        'Description    :�}�̏����ݏ���
        'Return         :True=OK(����),False=NG�i���s�j
        'Create         :2004/08/20
        'Update         :
        '============================================================================
        fn_BAITAI_WRITE = False
        Dim strTAKOU_SEND_FILE As String
        Dim intKEKKA As Integer
        Dim strKEKKA As String
        Dim strKAKUTYOUSI As String

        Select Case TakouInfo.BAITAI_CODE
            Case "00"        '�`��
                Select Case ToriInfo.FMT_KBN
                    Case "00"                   '�S��
                        strKAKUTYOUSI = ".120"
                    Case "01"                   '�m�g�j
                        strKAKUTYOUSI = ".120"
                    Case "02"
                        strKAKUTYOUSI = ".390"  '����
                    Case "03"
                        strKAKUTYOUSI = ".130"  '�N��
                    Case Else
                        strKAKUTYOUSI = ".120"
                End Select
                If TakouInfo.SFILE_NAME.Trim = "" Then  '���s�}�X�^�ɑ��M�t�@�C�������ݒ肳��Ă��Ȃ����ꊇ���M�t�@�C���쐬�Ώ�
                    '�t�@�C�����̃Z�b�g
                    'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
                    If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                        strTAKOU_SEND_FILE = Path.Combine(IniInfo.DEN & IniInfo.NOUKYOMATOME, "T" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & strKAKUTYOUSI)
                    Else
                        strTAKOU_SEND_FILE = Path.Combine(IniInfo.DEN & TakouInfo.TKIN_NO, "T" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & strKAKUTYOUSI)
                    End If
                    '�t�H���_�̑��݊m�F
                    Dim strFOLDER As String
                    'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
                    If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                        strFOLDER = IniInfo.DEN & IniInfo.NOUKYOMATOME
                        If Not Directory.Exists(strFOLDER) Then
                            '�t�H���_�쐬
                            Directory.CreateDirectory(strFOLDER)
                        End If
                    Else
                        strFOLDER = IniInfo.DEN & TakouInfo.TKIN_NO
                        If Not Directory.Exists(strFOLDER) Then
                            '�t�H���_�쐬
                            Directory.CreateDirectory(strFOLDER)
                        End If
                    End If
                Else
                    strTAKOU_SEND_FILE = IniInfo.DEN & TakouInfo.SFILE_NAME     '�o�̓t�@�C��
                End If
                intKEKKA = clsFusion.fn_DISK_CPYTO_DEN(strTORI_CODE, _
                                                       strWRK_FILE_NAME, _
                                                       strTAKOU_SEND_FILE, _
                                                       intREC_LENGTH, _
                                                       TakouInfo.CODE_KBN, _
                                                       gstrP_FILE)

                Select Case intKEKKA
                    Case 0
                        fn_BAITAI_WRITE = True
                        BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "����", "�t�@�C���ϊ�")
                    Case 100
                        fn_BAITAI_WRITE = False
                        'Return         :0=�����A100=�R�[�h�ϊ����s
                        BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "���s", "�t�@�C���ϊ��i�R�[�h�ϊ��j")
                        JobMessage = "�t�@�C���ϊ��i�R�[�h�ϊ��j���s"
                        Return False
                End Select

            Case "01"    '�R�D�T�e�c
                ' 2016/01/18 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                'strTAKOU_SEND_FILE = TakouInfo.SFILE_NAME.Trim
                'Select Case TakouInfo.CODE_KBN
                '    Case "0", "1", "2", "3"   'DOS�`��
                '        intKEKKA = clsFusion.fn_DISK_CPYTO_FD(strTORI_CODE, _
                '                                              strWRK_FILE_NAME, _
                '                                              strTAKOU_SEND_FILE, _
                '                                              intREC_LENGTH, _
                '                                              TakouInfo.CODE_KBN, _
                '                                              gstrP_FILE, _
                '                                              True, msgTitle)
                '    Case "4"        'IBM�`��
                '        intKEKKA = clsFusion.fn_DISK_CPYTO_FD(strTORI_CODE, _
                '                                              strWRK_FILE_NAME, _
                '                                              strTAKOU_SEND_FILE, _
                '                                              intREC_LENGTH, _
                '                                              TakouInfo.CODE_KBN, _
                '                                              strIBMP_FILE, _
                '                                              True, msgTitle)
                'End Select

                'Select Case intKEKKA
                '    Case 0
                '        fn_BAITAI_WRITE = True
                '        BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "����", "�e�c������")
                '    Case 100
                '        fn_BAITAI_WRITE = False
                '        'Return         :0=�����A100=�e�c�����ݎ��s�iIBM�`���j�A200=�e�c�����ݎ��s�iDOS�`���j
                '        BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "���s", "�e�c�����݁iIBM�`���j")
                '        JobMessage = "�e�c�����ݎ��s�iIBM�`���j"
                '        Return False
                '    Case 200
                '        fn_BAITAI_WRITE = False
                '        BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "���s", "�e�c�����ݎ��s�iIBM�`���j")
                '        JobMessage = "�e�c�����ݎ��s�iDOS�`���j"
                '        Return False
                'End Select
                Select Case IniInfo.RSV2_EDITION
                    Case "2"
                        '-------------------------------------------------
                        ' �o�̓t�@�C�����\�z
                        '-------------------------------------------------
                        strTAKOU_SEND_FILE = IniInfo.COMMON_BAITAIWRITE & "TAK_FD_" & strTORI_CODE & "_" & strFURI_DATE & "_" & TakouInfo.TKIN_NO


                        '-------------------------------------------------
                        ' �t�@�C���o��
                        '-------------------------------------------------
                        '2016/02/08 �^�X�N�j��� RSV2�Ή� MODIFY ------------------------------------- START
                        '�R�[�h�ϊ�����Ɣ}�̕ϊ������̃R�[�h�ϊ��ł���ɃR�[�h�ϊ����Ă��܂����߁A�Ԋ҃f�[�^�쐬�ł̓R�[�h�ϊ����s��Ȃ��B
                        If Dir(strTAKOU_SEND_FILE) <> "" Then
                            Kill(strTAKOU_SEND_FILE)
                        End If
                        File.Copy(strWRK_FILE_NAME, strTAKOU_SEND_FILE)
                        fn_BAITAI_WRITE = True
                        BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "����", "�t�@�C���ϊ�")
                        'intKEKKA = clsFusion.fn_DISK_CPYTO_DEN(strTORI_CODE, _
                        '                               strWRK_FILE_NAME, _
                        '                               strTAKOU_SEND_FILE, _
                        '                               intREC_LENGTH, _
                        '                               TakouInfo.CODE_KBN, _
                        '                               gstrP_FILE)

                        'Select Case intKEKKA
                        '    Case 0
                        '        fn_BAITAI_WRITE = True
                        '        BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "����", "�t�@�C���ϊ�")
                        '    Case 100
                        '        fn_BAITAI_WRITE = False
                        '        BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "���s", "�t�@�C���ϊ��i�R�[�h�ϊ��j")
                        '        JobMessage = "�t�@�C���ϊ��i�R�[�h�ϊ��j���s"
                        '        Return False
                        'End Select
                        '2016/02/08 �^�X�N�j��� RSV2�Ή� MODIFY ------------------------------------- END

                    Case Else
                        strTAKOU_SEND_FILE = TakouInfo.SFILE_NAME.Trim
                        Select Case TakouInfo.CODE_KBN
                            Case "0", "1", "2", "3"   'DOS�`��
                                intKEKKA = clsFusion.fn_DISK_CPYTO_FD(strTORI_CODE, _
                                                                      strWRK_FILE_NAME, _
                                                                      strTAKOU_SEND_FILE, _
                                                                      intREC_LENGTH, _
                                                                      TakouInfo.CODE_KBN, _
                                                                      gstrP_FILE, _
                                                                      True, msgTitle)
                            Case "4"        'IBM�`��
                                intKEKKA = clsFusion.fn_DISK_CPYTO_FD(strTORI_CODE, _
                                                                      strWRK_FILE_NAME, _
                                                                      strTAKOU_SEND_FILE, _
                                                                      intREC_LENGTH, _
                                                                      TakouInfo.CODE_KBN, _
                                                                      strIBMP_FILE, _
                                                                      True, msgTitle)
                        End Select

                        Select Case intKEKKA
                            Case 0
                                fn_BAITAI_WRITE = True
                                BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "����", "�e�c������")
                            Case 100
                                fn_BAITAI_WRITE = False
                                'Return         :0=�����A100=�e�c�����ݎ��s�iIBM�`���j�A200=�e�c�����ݎ��s�iDOS�`���j
                                BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "���s", "�e�c�����݁iIBM�`���j")
                                JobMessage = "�e�c�����ݎ��s�iIBM�`���j"
                                Return False
                            Case 200
                                fn_BAITAI_WRITE = False
                                BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "���s", "�e�c�����ݎ��s�iIBM�`���j")
                                JobMessage = "�e�c�����ݎ��s�iDOS�`���j"
                                Return False
                        End Select
                End Select
                ' 2016/01/18 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
            Case "04"        '�˗���
                '�㏈���ő��s���ו\���

            Case "05"        '�l�s
                Select Case IniInfo.MT
                    Case "0"     '�l�s�����ڎ��U�T�[�o�ɐڑ����Ă���ꍇ
                        Dim lngErrStatus As Long
                        Dim intBlkSize As Integer
                        Select Case intREC_LENGTH
                            Case 120
                                intBlkSize = 1800
                            Case 220
                                intBlkSize = 2200
                            Case 300
                                intBlkSize = 3000
                        End Select

                        '2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
                        strKEKKA = vbDLL.mtCPYtoMT(intBlkSize, intREC_LENGTH, 1, "SMLT", 1, 4, strWRK_FILE_NAME, " ", 0, lngErrStatus)
                        'strKEKKA = vbDLL.mtCPYtoMT(CShort(intBlkSize), CShort(intREC_LENGTH), 1, "SMLT", 1, 4, strWRK_FILE_NAME, " ", 0, CInt(lngErrStatus))
                        '2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END

                        If strKEKKA <> "" Then
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "���s", "�l�s������")
                            JobMessage = "�l�s�����ݎ��s"
                            Return False
                        Else
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "����", "�l�s������")
                        End If
                    Case "1"      '�l�s�����U�T�[�o�ɐڑ����Ă��Ȃ��ꍇ
                        '2010/01/25 DEN������TAKOU�t�H���_�ɋ��Z�@�փR�[�h�ŕۑ�����
                        'If INI_COMMON_MTTAKOUFILE = "" Then
                        '    BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "���s", "�l�s������(�t�@�C�����擾)���s")
                        '    JobMessage = "�l�s������(�t�@�C�����擾)���s"
                        '    Return False
                        'End If
                        'If Dir(INI_COMMON_MTTAKOUFILE) <> "" Then
                        '    Kill(INI_COMMON_MTTAKOUFILE)
                        'End If
                        'FileCopy(strWRK_FILE_NAME, INI_COMMON_MTTAKOUFILE)
                        If Directory.Exists(Path.Combine(IniInfo.DEN, "TAKOU")) = False Then
                            Directory.CreateDirectory(Path.Combine(IniInfo.DEN, "TAKOU"))
                        End If
                        'FileCopy(strWRK_FILE_NAME, Path.Combine(Path.Combine(INI_COMMON_DEN, "TAKOU"), strKEIYAKU_KIN))
                        intKEKKA = clsFusion.fn_DISK_CPYTO_DEN(strTORI_CODE, _
                                                               strWRK_FILE_NAME, _
                                                               Path.Combine(Path.Combine(IniInfo.DEN, "TAKOU"), TakouInfo.TKIN_NO), _
                                                               intREC_LENGTH, _
                                                               TakouInfo.CODE_KBN, _
                                                               gstrP_FILE)
                        '==========================================================
                        If Err.Number = 0 Then
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "����", "�l�s������(�t�@�C���R�s�[)")
                        Else
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "����", "�l�s������(�t�@�C���R�s�[)���s")
                            JobMessage = "�l�s������(�t�@�C���R�s�[)���s"
                            Return False
                        End If

                End Select
            Case "06"        '�b�l�s
                Select Case IniInfo.CMT
                    Case "0"    '�b�l�s�����ڎ��U�T�[�o�ɐڑ����Ă���ꍇ
                        Dim lngErrStatus As Long
                        Dim intBlkSize As Integer
                        Select Case intREC_LENGTH
                            Case 120
                                intBlkSize = 1800
                            Case 220
                                intBlkSize = 2200
                            Case 300
                                intBlkSize = 3000
                        End Select

                        '2018/02/19 saitou �L���M��(RSV2�W��) UPD �T�[�o�[�����Ή�(64�r�b�g�Ή�) -------------------- START
                        strKEKKA = vbDLL.cmtCPYtoMT(intBlkSize, intREC_LENGTH, 1, "SMLT", 1, 4, strWRK_FILE_NAME, " ", 0, lngErrStatus)
                        'strKEKKA = vbDLL.cmtCPYtoMT(CShort(intBlkSize), CShort(intREC_LENGTH), 1, "SMLT", 1, 4, strWRK_FILE_NAME, " ", 0, CInt(lngErrStatus))
                        '2018/02/19 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------- END

                        If strKEKKA <> "" Then
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "���s", "�b�l�s������")
                            JobMessage = "�b�l�s�����ݎ��s"
                            Return False
                        Else
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "����", "�b�l�s������")
                        End If
                    Case "1"    '�b�l�s�����U�T�[�o�ɐڑ����Ă��Ȃ��ꍇ
                        '2010/01/25 DEN������TAKOU�t�H���_�ɋ��Z�@�փR�[�h�ŕۑ�����
                        'If INI_COMMON_CMTTAKOUFILE = "" Then
                        '    BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "���s", "�b�l�s������(�t�@�C�����擾)���s")
                        '    JobMessage = "�b�l�s������(�t�@�C�����擾)���s"
                        '    Return False
                        'End If
                        'If Dir(INI_COMMON_CMTTAKOUFILE) <> "" Then
                        '    Kill(INI_COMMON_CMTTAKOUFILE)
                        'End If
                        'FileCopy(strWRK_FILE_NAME, INI_COMMON_CMTTAKOUFILE)
                        If Directory.Exists(Path.Combine(IniInfo.DEN, "TAKOU")) = False Then
                            Directory.CreateDirectory(Path.Combine(IniInfo.DEN, "TAKOU"))
                        End If
                        'FileCopy(strWRK_FILE_NAME, Path.Combine(Path.Combine(INI_COMMON_DEN, "TAKOU"), strKEIYAKU_KIN))
                        intKEKKA = clsFusion.fn_DISK_CPYTO_DEN(strTORI_CODE, _
                                                               strWRK_FILE_NAME, _
                                                               Path.Combine(Path.Combine(IniInfo.DEN, "TAKOU"), TakouInfo.TKIN_NO), _
                                                               intREC_LENGTH, _
                                                               TakouInfo.CODE_KBN, _
                                                               gstrP_FILE)
                        '==========================================================
                        If Err.Number = 0 Then
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "����", "�b�l�s������(�t�@�C���R�s�[)")
                        Else
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "���s�f�[�^�쐬", "���s", "�b�l�s������(�t�@�C���R�s�[)���s")
                            JobMessage = "�b�l�s������(�t�@�C���R�s�[)���s"
                            Return False
                        End If
                End Select
        End Select

        fn_BAITAI_WRITE = True
    End Function

    ''' <summary>
    ''' ���Z�@�փ}�X�^�ɑ��݂��邩�`�F�b�N���܂��B
    ''' </summary>
    ''' <param name="strKinCd">���Z�@�փR�[�h</param>
    ''' <param name="strSitCd">�x�X�R�[�h</param>
    ''' <returns>True or False</returns>
    ''' <remarks>���S�M�� �n�[�h�X���ɂĐV�K�ǉ�</remarks>
    Private Function CheckTenmast(ByVal strKinCd As String, _
                                  ByVal strSitCd As String) As Boolean
        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        SQL.Append("select * from TENMAST where KIN_NO_N = " & SQ(strKinCd) & " and SIT_NO_N = " & SQ(strSitCd))

        Try
            Return oraReader.DataReader(SQL)
        Catch ex As Exception
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' INI�t�@�C����ǂݍ��݂܂��B
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks>���S�M�� �n�[�h�X���ɂĉ��C</remarks>
    Private Function fn_INI_READ() As Boolean
        Try
            BLOG.Write("INI�t�@�C���Ǎ�(�J�n)", "����")

            IniInfo.DEN = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If IniInfo.DEN.ToUpper.Equals("ERR") = True OrElse IniInfo.DEN = Nothing Then
                BLOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[COMMON].DEN �ݒ�Ȃ�")
                BLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [COMMON].DEN �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.TAK = CASTCommon.GetFSKJIni("COMMON", "TAK")
            If IniInfo.TAK.ToUpper.Equals("ERR") = True OrElse IniInfo.TAK = Nothing Then
                BLOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[COMMON].TAK �ݒ�Ȃ�")
                BLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [COMMON].TAK �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.KINKOCD = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If IniInfo.KINKOCD.ToUpper.Equals("ERR") = True OrElse IniInfo.KINKOCD = Nothing Then
                BLOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[COMMON].KINKOCD �ݒ�Ȃ�")
                BLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [COMMON].KINKOCD �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.MT = CASTCommon.GetFSKJIni("COMMON", "MT")
            If IniInfo.MT.ToUpper.Equals("ERR") = True OrElse IniInfo.MT = Nothing Then
                BLOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[COMMON].MT �ݒ�Ȃ�")
                BLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [COMMON].MT �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.CMT = CASTCommon.GetFSKJIni("COMMON", "CMT")
            If IniInfo.CMT.ToUpper.Equals("ERR") = True OrElse IniInfo.CMT = Nothing Then
                BLOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[COMMON].CMT �ݒ�Ȃ�")
                BLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [COMMON].CMT �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.TAKO_SEQ = CASTCommon.GetFSKJIni("JIFURI", "TAKO_SEQ")
            If IniInfo.TAKO_SEQ.ToUpper.Equals("ERR") = True OrElse IniInfo.TAKO_SEQ = Nothing Then
                BLOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[JIFURI].TAKO_SEQ �ݒ�Ȃ�")
                BLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [JIFURI].TAKO_SEQ �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.NOUKYOMATOME = CASTCommon.GetFSKJIni("TAKO", "NOUKYOMATOME")
            If IniInfo.NOUKYOMATOME.ToUpper.Equals("ERR") = True OrElse IniInfo.NOUKYOMATOME = Nothing Then
                BLOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[TAKO].NOUKYOMATOME �ݒ�Ȃ�")
                BLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [TAKO].NOUKYOMATOME �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.NOUKYOFROM = CASTCommon.GetFSKJIni("TAKO", "NOUKYOFROM")
            If IniInfo.NOUKYOFROM.ToUpper.Equals("ERR") = True OrElse IniInfo.NOUKYOFROM = Nothing Then
                BLOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[TAKO].NOUKYOFROM �ݒ�Ȃ�")
                BLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [TAKO].NOUKYOFROM �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.NOUKYOTO = CASTCommon.GetFSKJIni("TAKO", "NOUKYOTO")
            If IniInfo.NOUKYOTO.ToUpper.Equals("ERR") = True OrElse IniInfo.NOUKYOTO = Nothing Then
                BLOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[TAKO].NOUKYOTO �ݒ�Ȃ�")
                BLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [TAKO].NOUKYOTO �ݒ�Ȃ�")
                Return False
            End If

            ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
            IniInfo.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            If IniInfo.RSV2_EDITION.ToUpper.Equals("ERR") = True OrElse IniInfo.RSV2_EDITION = Nothing Then
                BLOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[RSV2_V1.0.0].EDITION �ݒ�Ȃ�")
                BLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [RSV2_V1.0.0].EDITION �ݒ�Ȃ�")
                Return False
            End If

            IniInfo.COMMON_BAITAIWRITE = CASTCommon.GetFSKJIni("COMMON", "BAITAIWRITE")
            If IniInfo.COMMON_BAITAIWRITE.ToUpper.Equals("ERR") = True OrElse IniInfo.COMMON_BAITAIWRITE = Nothing Then
                BLOG.Write("(�ݒ�t�@�C���ǂݍ���)", "���s", "[COMMON].BAITAIWRITE �ݒ�Ȃ�")
                BLOG.UpdateJOBMASTbyErr("�ݒ�t�@�C���ǂݍ��� [COMMON].BAITAIWRITE �ݒ�Ȃ�")
                Return False
            End If
            ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

            BLOG.Write("INI�t�@�C���Ǎ�(�I��)", "����")

        Catch ex As Exception
            BLOG.Write("INI�t�@�C���Ǎ�", "���s", ex.Message)
            Return False
        End Try

        '*** �C�� mitsu 2008/08/27 ���s�t�H���_�����݂��Ȃ��ꍇ ***
        Try
            If Directory.Exists(IniInfo.TAK) = False Then
                Directory.CreateDirectory(IniInfo.TAK)
            End If
        Catch ex As Exception
            BLOG.Write("���s�t�H���_�쐬", "���s", ex.Message)
            Return False
        End Try
        '**********************************************************
        Return True
    End Function

#End Region

    Private Class mySortClass
        Implements IComparer

        ' Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
           Implements IComparer.Compare

            Dim strX() As String = CType(x, String())
            Dim strY() As String = CType(y, String())

            Dim splitX() As String = strX(0).Split("."c)     ' ���Z�@��,�U�֓�
            Dim splitY() As String = strY(0).Split("."c)     ' ���Z�@��,�U�֓�

            Dim nRet As Integer = 0
            ' ���Z�@�ւ��ɔ�r
            nRet = String.Compare(splitX(0), splitY(0))
            If nRet = 0 Then
                ' �U�֓����r
                nRet = String.Compare(splitX(1), splitY(1))
            End If
            If nRet = 0 Then
                ' �t�H���_�����r
                Return New CaseInsensitiveComparer().Compare(strX(0), strY(0))
            End If
            Return nRet
        End Function 'IComparer.Compare

    End Class 'myReverserClass

End Module
