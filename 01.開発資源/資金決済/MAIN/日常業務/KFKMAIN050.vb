Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKMAIN050

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKMAIN050", "�������σf�[�^�쐬���")
    Private Const msgTitle As String = "�������σf�[�^�쐬���(KFKMAIN050)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private Const ThisModuleName As String = "KFKMAIN050.vb"

    Private KESSAI_DATE As String             '���ϓ�

    Private MainDB As CASTCommon.MyOracle   '�p�u���b�N�f�[�^�[�x�[�X
    '�N���b�N������̔ԍ�
    Dim ClickedColumn As Integer
    '�\�[�g�I�[�_�[�t���O
    Dim SortOrderFlag As Boolean = True

    Private BATCHSV As String            '2010.01.27 �ǉ�
    Private RIENTASV As String           '2010.01.27 �ǉ�

    Structure KeyInfo

        Dim TORIS_CODE As String            '������R�[�h
        Dim TORIF_CODE As String            '����敛�R�[�h
        Dim FURI_DATE As String             '�U�֓�
        Dim KESSAI_YDATE As String          '���ϗ\���
        Dim TESUU_YDATE As String           '�萔�������\���
        Dim TESUU_KIN As String             '�萔�����z�@�F�萔�����v
        Dim TESUU_KIN1 As String            '�萔�����z�P�F�����萔��
        Dim TESUU_KIN2 As String            '�萔�����z�Q�F����
        Dim TESUU_KIN3 As String            '�萔�����z�R�F�U���萔��
        Dim FURI_KEN As String              '�U���ό���
        Dim FURI_KIN As String              '�U���ϋ��z
        Dim KIGYO_CODE As String            '��ƃR�[�h
        Dim BAITAI_CODE As String           '�}�̃R�[�h
        Dim SYUBETU_CODE As String          '��ʃR�[�h
        Dim ITAKU_CODE As String            '�ϑ��҃R�[�h
        Dim ITAKU_NNAME As String           '�ϑ��Җ�����
        Dim ITAKU_KNAME As String           '�ϑ��Җ��J�i
        Dim FURI_CODE As String             '�U�փR�[�h
        Dim NS_KBN As String                '���o���敪
        Dim TESUUTYO_PATN As String         '�萔���������@
        Dim TESUUTYO_KBN As String          '�萔�������敪
        Dim KESSAI_KBN As String            '���ϋ敪
        Dim TORIMATOME_SIT_NO As String     '�Ƃ�܂ƂߓX
        Dim HONBU_KOUZA As String           '�{���ʒi�����ԍ�
        Dim TUKEKIN_NO As String            '���ϋ��Z�@��
        Dim TUKESIT_NO As String            '���ώx�X
        Dim TUKEKAMOKU As String            '���ωȖ�
        Dim TUKEKOUZA As String             '���ό����ԍ�
        Dim TUKEMEIGI As String             '���ϖ��`�l�i�J�i�j
        Dim BIKOU1 As String                '���l�P
        Dim BIKOU2 As String                '���l�Q
        Dim TSUUTYOSIT_NO As String         '�萔�������x�X
        Dim TSUUTYOKAMOKU As String         '�萔�������Ȗ�
        Dim TSUUTYOKOUZA As String          '�萔�����������ԍ�

        Dim KESSAI_TORI_KBN As String       '0�F�������ςƎ萔�������̗���
        '                                    1�F�������ς̂�
        '                                    2:�萔�������̂�
        Dim TESUUTYO_FLG As String          '�萔�������σt���O
        Dim RECTUUBAN As Long               '���R�[�h�ԍ�

        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
        End Sub

        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_SV1").PadRight(7)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_SV1").PadRight(2)
            FURI_DATE = oraReader.GetString("FURI_DATE_SV1").PadRight(8)
            KESSAI_YDATE = oraReader.GetString("KESSAI_YDATE_SV1")
            TESUU_YDATE = oraReader.GetString("TESUU_YDATE_SV1")
            TESUU_KIN = oraReader.GetString("TESUU_KIN_SV1")
            TESUU_KIN1 = oraReader.GetString("TESUU_KIN1_SV1")
            TESUU_KIN2 = oraReader.GetString("TESUU_KIN2_SV1")
            TESUU_KIN3 = oraReader.GetString("TESUU_KIN3_SV1")
            FURI_KEN = oraReader.GetString("FURI_KEN_SV1")
            FURI_KIN = oraReader.GetString("FURI_KIN_SV1")
            KIGYO_CODE = oraReader.GetString("KIGYO_CODE_TV1")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_TV1")
            SYUBETU_CODE = oraReader.GetString("SYUBETU_CODE_TV1")
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_TV1")
            ITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_TV1")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_TV1")
            FURI_CODE = oraReader.GetString("FURI_CODE_TV1")
            NS_KBN = oraReader.GetString("NS_KBN_TV1")
            TESUUTYO_PATN = oraReader.GetString("TESUUTYO_PATN_TV1")
            TESUUTYO_KBN = oraReader.GetString("TESUUTYO_KBN_TV1")
            KESSAI_KBN = oraReader.GetString("KESSAI_KBN_TV1")
            TORIMATOME_SIT_NO = oraReader.GetString("TORIMATOME_SIT_NO_TV1")
            HONBU_KOUZA = oraReader.GetString("HONBU_KOUZA_TV1")
            TUKEKIN_NO = oraReader.GetString("TUKEKIN_NO_TV1")
            TUKESIT_NO = oraReader.GetString("TUKESIT_NO_TV1")
            TUKEKAMOKU = oraReader.GetString("TUKEKAMOKU_TV1")
            TUKEKOUZA = oraReader.GetString("TUKEKOUZA_TV1")
            TUKEMEIGI = oraReader.GetString("TUKEMEIGI_TV1")
            BIKOU1 = oraReader.GetString("BIKOU1_TV1")
            BIKOU2 = oraReader.GetString("BIKOU2_TV1")
            TSUUTYOSIT_NO = oraReader.GetString("TSUUTYOSIT_NO_TV1")
            TSUUTYOKAMOKU = oraReader.GetString("TSUUTYOKAMOKU_TV1")
            TSUUTYOKOUZA = oraReader.GetString("TSUUTYOKOUZA_TV1")
            KESSAI_TORI_KBN = oraReader.GetString("KESSAI_TORI_KBN")

        End Sub

    End Structure
    '

#Region "�֐�"
    ''' <summary>
    ''' ���σf�[�^�̒��o
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function KessaiDataList() As Integer

        Dim OraKesReader As New CASTCommon.MyOracleReader(MainDB)
        Dim ROW As Integer = 0
        Dim Data(7) As String


        Try

            Dim SQL As StringBuilder
            Dim CommonSQL As StringBuilder          '���o�����i���ʁj
            Dim TudoWhereSQL As StringBuilder       '���o�����i���ρE�萔�������j
            Dim KessaiOnlyWhereSQL As StringBuilder '���o�����i���ς̂݁j
            Dim TesuuOnlyWhereSQL As StringBuilder  '���o�����i�萔���̂݁j
            Dim IkkatuWhereSQL As StringBuilder     '���o�����i���ρE�萔������(�ꊇ)�j

            SQL = New StringBuilder(128)                    '������������N���XStringBuilder�i�����ɕ�����̒ǉ��A�}�����s���j
            CommonSQL = New StringBuilder(128)
            TudoWhereSQL = New StringBuilder(128)
            KessaiOnlyWhereSQL = New StringBuilder(128)
            TesuuOnlyWhereSQL = New StringBuilder(128)
            IkkatuWhereSQL = New StringBuilder(128)

            CommonSQL.Append(" SELECT")
            CommonSQL.Append(" TORIS_CODE_SV1")
            CommonSQL.Append(", TORIF_CODE_SV1")
            CommonSQL.Append(", FURI_DATE_SV1")
            CommonSQL.Append(", KESSAI_DATE_SV1")
            CommonSQL.Append(", KESSAI_YDATE_SV1")
            CommonSQL.Append(", TESUU_DATE_SV1")
            CommonSQL.Append(", TESUU_YDATE_SV1")
            CommonSQL.Append(", TESUUKEI_FLG_SV1")
            CommonSQL.Append(", KESSAI_FLG_SV1")
            CommonSQL.Append(", TESUUTYO_FLG_SV1")
            CommonSQL.Append(", TYUUDAN_FLG_SV1")
            CommonSQL.Append(", FURI_KIN_SV1")
            CommonSQL.Append(", TESUUTYO_KBN_TV1")
            CommonSQL.Append(", ITAKU_NNAME_TV1")
            CommonSQL.Append(", KIGYO_CODE_SV1")
            CommonSQL.Append(", FURI_CODE_SV1")
            CommonSQL.Append(", ' ' TEIKEI_KBN")
            CommonSQL.Append(", KESSAI_KBN_TV1")

            ' 2017/07/25 �^�X�N�j���� ADD (RSV2�W���Ή� No.8(�w�Z����� �����o���ΏۊO)) -------------------- START
            CommonSQL.Append(", BAITAI_CODE_TV1")
            ' 2017/07/25 �^�X�N�j���� ADD (RSV2�W���Ή� No.8(�w�Z����� �����o���ΏۊO)) -------------------- END

            ' ���������ςƎ萔�������𓯎��ɍs������SQL
            TudoWhereSQL.Append(" FROM V1_KESMAST")
            TudoWhereSQL.Append(" WHERE KESSAI_YDATE_SV1 = " & SQ(KESSAI_DATE))                                         '���ϗ\����i=���͓��j
            TudoWhereSQL.Append(" AND TESUU_YDATE_SV1 = " & SQ(KESSAI_DATE))                                             '�萔���\����i=���͓��j
            TudoWhereSQL.Append(" AND TESUUKEI_FLG_SV1 = '1'")                                                          '�萔���v�Z�t���O�i�萔���v�Z�ς݁j
            TudoWhereSQL.Append(" AND ( (KESSAI_FLG_SV1 = '0' AND TESUUTYO_FLG_SV1 = '0') ")                            '���σt���O�i�����ρj
            TudoWhereSQL.Append(" OR (KESSAI_FLG_SV1 = '2' AND TESUUTYO_FLG_SV1 = '2') )")                              '�萔�������t���O�i�萔���������j
            TudoWhereSQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")                                                           '���f�t���O�i���f�Ȃ��j
            TudoWhereSQL.Append(" AND FURI_KIN_SV1 > 0")                                                                '�U�֍ς݋��z�i�U�֍ϋ��z����j
            TudoWhereSQL.Append(" AND KESSAI_KBN_TV1 <> '99'")                                                          '���ϑΏۊO�ł͂Ȃ� 2009.10.28

            ' ���������ς������s������SQL
            KessaiOnlyWhereSQL.Append(" FROM V1_KESMAST")
            KessaiOnlyWhereSQL.Append(" WHERE KESSAI_YDATE_SV1 = " & SQ(KESSAI_DATE))                                    '���ϗ\����i=���͓��j
            'KessaiOnlyWhereSQL.Append(" AND (TESUU_YDATE_SV1 != " & SQ(KESSAI_DATE))
            KessaiOnlyWhereSQL.Append(" AND (TESUUTYO_KBN_TV1 = '1' OR TESUUTYO_KBN_TV1 = '2' OR TESUUTYO_KBN_TV1 = '3') ")
            KessaiOnlyWhereSQL.Append(" AND TESUUKEI_FLG_SV1 = '1'")                                                    '�萔���v�Z�t���O�i�萔���v�Z�ς݁j
            KessaiOnlyWhereSQL.Append(" AND ((KESSAI_FLG_SV1 = '0' AND TESUUTYO_FLG_SV1 = '0') ")                      '���σt���O�i�����ρj
            KessaiOnlyWhereSQL.Append(" OR (KESSAI_FLG_SV1 = '2' AND TESUUTYO_FLG_SV1 = '0') )")                        '���σt���O�i�萔���͂Ȃ��j 
            KessaiOnlyWhereSQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")                                                     '���f�t���O�i���f�Ȃ��j
            KessaiOnlyWhereSQL.Append(" AND FURI_KIN_SV1 > 0")                                                          '�U�֍ς݋��z�i�U�֍ϋ��z����j
            KessaiOnlyWhereSQL.Append(" AND KESSAI_KBN_TV1 <> '99'")                                                     '���ϑΏۊO�ł͂Ȃ� 2009.10.28

            '***�萔���݂̂͒��o���Ȃ� 2009.10.28 start
            '' ���萔�������������s������SQL
            'TesuuOnlyWhereSQL.Append(" FROM V1_KESMAST")
            'TesuuOnlyWhereSQL.Append(" WHERE KESSAI_YDATE_SV1 != " & SQ(KESSAI_DATE))                                   '���ϗ\����i=���͓��j
            'TesuuOnlyWhereSQL.Append(" AND TESUU_YDATE_SV1 = " & SQ(KESSAI_DATE))                                       '�萔���\����i�����͓��j
            'TesuuOnlyWhereSQL.Append(" AND TESUUKEI_FLG_SV1 = '1'")                                                    '�萔���v�Z�t���O�i�萔���v�Z�ς݁j
            'TesuuOnlyWhereSQL.Append(" AND ( ( KESSAI_FLG_SV1 = '1'")                                                   '���σt���O�i�����ρj            
            'TesuuOnlyWhereSQL.Append(" AND TESUUTYO_FLG_SV1 = '0') ")                                                   '�萔�������t���O�i�萔���������j
            'TesuuOnlyWhereSQL.Append(" OR (KESSAI_FLG_SV1 = '1' AND TESUUTYO_FLG_SV1 = '2') )")                         '���σt���O�i�萔���͂Ȃ��j '2008/08/08 �l���M���@tesuutyo_FLG='2'�ǉ�
            'TesuuOnlyWhereSQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")                                                     '���f�t���O�i���f�Ȃ��j
            'TesuuOnlyWhereSQL.Append(" AND FURI_KIN_SV1 > 0")                                                          '�U�֍ς݋��z�i�U�֍ϋ��z����j
            '***�萔���݂̂͒��o���Ȃ� 2009.10.28 end

            ' ���萔�������敪��1:�ꊇ�����̏ꍇ�A�������ςƎ萔�������𓯎��ɍs������SQL
            'IkkatuWhereSQL.Append(" FROM V1_KESMAST")
            'IkkatuWhereSQL.Append(" WHERE KESSAI_YDATE_SV1 = " & SQ(KESSAI_DATE))                                       '���ϗ\����i=���͓��j
            'IkkatuWhereSQL.Append(" AND TESUU_YDATE_SV1 = " & SQ(KESSAI_DATE))                                          '�萔���\����i=���͓��j
            'IkkatuWhereSQL.Append(" AND TESUUKEI_FLG_SV1 = '1'")                                                        '�萔���v�Z�t���O�i�萔���v�Z�ς݁j
            'IkkatuWhereSQL.Append(" AND ( (KESSAI_FLG_SV1 = '0' AND TESUUTYO_FLG_SV1 = '0') ")                          '���σt���O�i�����ρj
            'IkkatuWhereSQL.Append(" OR (KESSAI_FLG_SV1 = '2' AND TESUUTYO_FLG_SV1 = '2') ) ")                           '�萔�������t���O�i�萔���������j
            'IkkatuWhereSQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")                                                         '���f�t���O�i���f�Ȃ��j
            'IkkatuWhereSQL.Append(" AND FURI_KIN_SV1 > 0")                                                              '�U�֍ς݋��z�i�U�֍ϋ��z����j
            'IkkatuWhereSQL.Append(" AND TESUUTYO_KBN_TV1 = '1'")                                                        '�萔�������敪�i�ꊇ�����j
            'IkkatuWhereSQL.Append(" AND KESSAI_KBN_TV1 <> '99'")                                                        '���ϑΏۊO�ł͂Ȃ� 2009.10.28

            'SQL���̍쐬
            SQL.Append(CommonSQL)
            SQL.Append(TudoWhereSQL)
            SQL.Append(" UNION")
            SQL.Append(CommonSQL)
            SQL.Append(KessaiOnlyWhereSQL)
            '***�萔���݂̂͒��o���Ȃ� 2009.10.28 start
            'SQL.Append(" UNION")
            'SQL.Append(CommonSQL)
            'SQL.Append(TesuuOnlyWhereSQL)
            'SQL.Append(" UNION")
            'SQL.Append(CommonSQL)
            'SQL.Append(IkkatuWhereSQL)
            '***�萔���݂̂͒��o���Ȃ� 2009.10.28 end

            SQL.Append(" ORDER BY KIGYO_CODE_SV1 ASC, FURI_CODE_SV1 ASC")

            If OraKesReader.DataReader(SQL) = True Then

                Do While OraKesReader.EOF = False

                    ' 2017/07/25 �^�X�N�j���� ADD (RSV2�W���Ή� No.8(�w�Z����� �����o���ΏۊO)) -------------------- START
                    If GCom.NzStr(OraKesReader.GetItem("BAITAI_CODE_TV1")) = "07" And
                       GCom.NzDec(OraKesReader.GetItem("TORIF_CODE_SV1"), 0).ToString("00") = "04" Then
                        '--------------------------------------------------------------------------------
                        ' �w�Z�����́u�����o���v�͎������ϑΏۊO�̂��߁A�����R�[�h��
                        '--------------------------------------------------------------------------------
                    Else
                        ' 2017/07/25 �^�X�N�j���� ADD (RSV2�W���Ή� No.8(�w�Z����� �����o���ΏۊO)) -------------------- END

                        Dim Temp As String

                        ROW += 1

                        'ListView�̒l�ݒ�
                        Dim strTESUUTYO_KBN_TV1 As String   '�萔�������敪
                        Dim strKESSAI_FLG_SV1 As String     '���σt���O
                        Dim strTESUU_YDATE_SV1 As String    '�萔���\���

                        strTESUUTYO_KBN_TV1 = GCom.NzStr(OraKesReader.GetItem("TESUUTYO_KBN_TV1")).Trim '�萔�������敪
                        strKESSAI_FLG_SV1 = GCom.NzStr(OraKesReader.GetItem("KESSAI_FLG_SV1")).Trim
                        strTESUU_YDATE_SV1 = GCom.NzStr(OraKesReader.GetItem("TESUU_YDATE_SV1")).Trim

                        If strTESUUTYO_KBN_TV1 <> "0" Then
                            Data(1) = "2"           '���o�����Q(���ς̂�)
                        Else
                            Data(1) = "1"           '���o�����P(���ρA�萔������)
                        End If
                        '================================================================================================================================================================================

                        '����於
                        Temp = GCom.NzStr(OraKesReader.GetItem("ITAKU_NNAME_TV1")).Trim
                        Data(2) = GCom.GetLimitString(Temp, 40)
                        '�����R�[�h�i��R�[�h + "-" + ���R�[�h�j
                        Data(3) = GCom.NzDec(OraKesReader.GetItem("TORIS_CODE_SV1"), 0).ToString("0000000000")
                        Data(3) &= "-"
                        Data(3) &= GCom.NzDec(OraKesReader.GetItem("TORIF_CODE_SV1"), 0).ToString("00")
                        '2014/04/15 saitou �W���C�� UPD -------------------------------------------------->>>>
                        '���M�f�[�^�쐬��ʂƓ��t�̕\�����@�����킹��
                        '�U�֓�
                        Dim strTempFuriDate As String = GCom.NzDec(OraKesReader.GetItem("FURI_DATE_SV1"), "")
                        Data(4) = strTempFuriDate.Substring(0, 4) & "/" & strTempFuriDate.Substring(4, 2) & "/" & strTempFuriDate.Substring(6, 2)
                        '���ϗ\���
                        Dim strTempKessaiYDate As String = GCom.NzStr(OraKesReader.GetItem("KESSAI_YDATE_SV1")).Trim.Substring(0, 8)
                        Data(5) = strTempKessaiYDate.Substring(0, 4) & "/" & strTempKessaiYDate.Substring(4, 2) & "/" & strTempKessaiYDate.Substring(6, 2)
                        '�萔�������\���
                        Dim strTempTesuuYDate As String = GCom.NzStr(OraKesReader.GetItem("TESUU_YDATE_SV1")).Trim.Substring(0, 8)
                        Data(6) = strTempTesuuYDate.Substring(0, 4) & "/" & strTempTesuuYDate.Substring(4, 2) & "/" & strTempTesuuYDate.Substring(6, 2)

                        ''�U�֓�
                        'Data(4) = GCom.NzDec(OraKesReader.GetItem("FURI_DATE_SV1"), "")
                        ''���ϗ\���
                        'Data(5) = GCom.NzStr(OraKesReader.GetItem("KESSAI_YDATE_SV1")).Trim.Substring(0, 8)
                        ''�萔�����ϗ\���
                        'Data(6) = GCom.NzStr(OraKesReader.GetItem("TESUU_YDATE_SV1")).Trim.Substring(0, 8)
                        '2014/04/15 saitou �W���C�� UPD --------------------------------------------------<<<<

                        '���ϋ敪�擾 ���ϋ敪
                        Select Case GCom.NzStr(OraKesReader.GetItem("KESSAI_KBN_TV1"))
                            Case "00"
                                Data(7) = "�a����"
                            Case "01"
                                Data(7) = "��������"
                            Case "02"
                                Data(7) = "�ב֐U��"
                            Case "03"
                                Data(7) = "�ב֕t��"
                            Case "04"
                                Data(7) = "�ʒi�o���̂�"
                            Case "05"
                                Data(7) = "���ʊ��"
                            Case "99"
                                Data(7) = "���ϑΏۊO"
                        End Select

                        Dim vLstItem As New ListViewItem(Data, -1, Color.Black, Color.White, Nothing)
                        ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                        ' 2017/07/25 �^�X�N�j���� ADD (RSV2�W���Ή� No.8(�w�Z����� �����o���ΏۊO)) -------------------- START
                    End If
                    ' 2017/07/25 �^�X�N�j���� ADD (RSV2�W���Ή� No.8(�w�Z����� �����o���ΏۊO)) -------------------- END

                    OraKesReader.NextRead()
                Loop
            Else
                Return 0    '0��
            End If

            Return ROW      '���� ����

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^����)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return -1

        Finally
            If Not OraKesReader Is Nothing Then OraKesReader.Close()
        End Try

    End Function
#End Region

    '���LOAD��
    Private Sub KFKMAIN050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim strSysDate As String

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            '------------------------------------------------
            '�V�X�e�����t�ƃ��[�U����\��
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '�x���}�X�^��荞��
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�x�����擾)", "���s", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '2010.01.27 �ǉ� ********
            If Not SetIniFIle() Then
                Return
            End If
            '************************

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '��ʕ\�����ɃV�X�e�����t��\������
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            '���X�g��ʂ̏�����
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub

    '2010.01.27 �ǉ� ********
    Private Function SetIniFIle() As Boolean
        Try

            BATCHSV = CASTCommon.GetFSKJIni("COMMON", "BATCHSV")
            If BATCHSV.ToUpper = "ERR" OrElse BATCHSV = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�o�b�`�T�[�o", "COMMON", "BATCHSV"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�����ɃR�[�h ����:COMMON ����:BATCHSV")
                Return False
            End If

            RIENTASV = CASTCommon.GetFSKJIni("COMMON", "RIENTASV")
            If RIENTASV.ToUpper = "ERR" OrElse RIENTASV = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "���G���^�T�[�o", "COMMON", "RIENTASV"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�Z���^�[�敪 ����:COMMON ����:RIENTASV")
                Return False
            End If
 
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ݒ�t�@�C���擾)", "���s", ex.ToString)
        End Try
    End Function
    '*************************

    '���s�{�^��������
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�J�n", "����", "")

            Dim nSelectItems As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            '���X�g�ɂP�����\������Ă��Ȃ��Ƃ�
            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '���X�g�ɂP���ȏ�\������Ă��邪�A�`�F�b�N����Ă��Ȃ��Ƃ�
                If nSelectItems.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            Dim dRet As DialogResult

            If RIENTASV = BATCHSV Then  '�W���u�N��
                dRet = MessageBox.Show(MSG0023I.Replace("{0}", "�������σf�[�^�쐬"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            Else                        '�o�b�`�N��
                dRet = MessageBox.Show(MSG0015I.Replace("{0}", "�������σf�[�^�쐬"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            End If

            If Not dRet = DialogResult.OK Then
                Return
            End If

            MainDB = New CASTCommon.MyOracle
            Dim SQL As StringBuilder


            '***���ޭ��Ͻ��̍X�V
            Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items

            For Each item As ListViewItem In nItems

                Dim FURI_DATE As String = GCom.NzDec(item.SubItems(4).Text, "")
                Dim Temp As String = GCom.NzDec(item.SubItems(3).Text, "")
                Dim TORIS_CODE As String = Temp.Substring(0, 10)
                Dim TORIF_CODE As String = Temp.Substring(10)
                Dim Jyouken As String = GCom.NzDec(item.SubItems(1).Text, "")
                SQL = New StringBuilder(128)
                LW.ToriCode = TORIS_CODE & "-" & TORIF_CODE
                LW.FuriDate = FURI_DATE

                SQL.Append("UPDATE SCHMAST")

                If item.Checked = True Then
                    Select Case Jyouken
                        Case "1"                                    '(���ρA�萔������)
                            SQL.Append(" SET KESSAI_FLG_S = '2'")
                            SQL.Append(", TESUUTYO_FLG_S = '2'")
                        Case "2"                                    '(���ς̂�)
                            SQL.Append(" SET KESSAI_FLG_S = '2'")
                    End Select
                Else
                    Select Case Jyouken
                        Case "1"                                    '(���ρA�萔������)
                            SQL.Append(" SET KESSAI_FLG_S = '0'")
                            SQL.Append(", TESUUTYO_FLG_S = '0'")
                        Case "2"                                    '(���ς̂�)
                            SQL.Append(" SET KESSAI_FLG_S = '0'")
                    End Select
                End If

                SQL.Append(" WHERE TORIS_CODE_S = '" & TORIS_CODE & "'")
                SQL.Append(" AND TORIF_CODE_S = '" & TORIF_CODE & "'")
                SQL.Append(" AND FURI_DATE_S = '" & FURI_DATE & "'")

                Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
                If nRet = 0 Then
                    MainDB.Rollback()
                    Return
                ElseIf nRet < 0 Then
                    Throw New Exception(MSG0002E.Replace("{0}", "�o�^"))
                End If
            Next

            Dim jobid As String
            Dim para As String

            ''2010.01.27 �ǉ� *******************
            'ini�t�@�C����RIENTASV������
            '�W���u�N�����o�b�`�N�����؂�ւ���
            '************************************
            If RIENTASV = BATCHSV Then  '�W���u�N��

                '�W���u�}�X�^�ɓo�^
                jobid = "K030"                      '..\Batch\�������σf�[�^�쐬\
                para = KESSAI_DATE                  '���ϓ����p�����^�Ƃ��Đݒ�

                '#########################
                'job����
                '#########################
                Dim iRet As Integer
                iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                If iRet = 1 Then
                    MainDB.Rollback()
                    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                ElseIf iRet = -1 Then
                    Throw New Exception(MSG0002E.Replace("{0}", "����"))
                End If

                '#########################
                'job�o�^
                '#########################
                If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then
                    Throw New Exception(MSG0005E)
                End If

                MainDB.Commit()

                MessageBox.Show(MSG0021I.Replace("{0}", "�������σf�[�^�쐬"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Else                        '�o�b�`�N��

                MainDB.Commit()

                Dim ExePath As String = CASTCommon.GetFSKJIni("COMMON", "EXE")
                Dim l_exitcode As Long

                Dim myProcess As New Process
                Dim SInfo As New ProcessStartInfo(Path.Combine(ExePath, "KFK030.EXE"), KESSAI_DATE)

                SInfo.WorkingDirectory = ExePath
                SInfo.CreateNoWindow = True
                SInfo.UseShellExecute = False
                myProcess = Process.Start(SInfo)
                myProcess.WaitForExit()
                l_exitcode = myProcess.ExitCode

                If l_exitcode <> 0 Then

                    MessageBox.Show(MSG0034E.Replace("{0}", "�������σf�[�^�쐬"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", "�G���[�R�[�h�F " & l_exitcode)

                    Return
                Else

                    MessageBox.Show(MSG0016I.Replace("{0}", "�������σf�[�^�쐬"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                End If


            End If

            '���X�g��ʂ̏�����
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False

            Me.txtKijyunDateY.Enabled = True
            Me.txtKijyunDateM.Enabled = True
            Me.txtKijyunDateD.Enabled = True

        Catch ex As Exception

            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", ex.Message)

        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�I��", "����", "")
        End Try
    End Sub

    '�I���{�^��������
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")

            Me.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try

    End Sub

#Region "KFJMAIN030�p�֐�"

    Function fn_check_TEXT() As Boolean
        '============================================================================
        'NAME           :fn_check_TEXT
        'Parameter      :
        'Description    :���s�{�^�����������ɕK�{���ڂ̃e�L�X�g�{�b�N�X�̓��͒l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :
        'Update         :
        '============================================================================

        Try
            '------------------------------------------------
            '���ϔN�`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKijyunDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '���ό��`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKijyunDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtKijyunDateM.Text >= 1 And txtKijyunDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '���ϓ��`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKijyunDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtKijyunDateD.Text >= 1 And txtKijyunDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '���t�`�F�b�N
            '------------------------------------------------
            Dim WORK_DATE As String = txtKijyunDateY.Text & "/" & txtKijyunDateM.Text & "/" & txtKijyunDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(fn_check_TEXT)", "���s", ex.Message)
            Return False

        Finally

        End Try

        Return True

    End Function

    '�[������
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKijyunDateY.Validating, txtKijyunDateM.Validating, txtKijyunDateD.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub
    '�ꗗ�\���̈�̃\�[�g(2009/10/09)�ǉ�
    Private Sub LetSort_ListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
    Handles ListView1.ColumnClick

        Try

            With CType(sender, ListView)

                If ClickedColumn = e.Column Then
                    ' ��������N���b�N�����ꍇ�́C�t���ɂ��� 
                    SortOrderFlag = Not SortOrderFlag
                End If

                ' ��ԍ��ݒ�
                ClickedColumn = e.Column

                ' �񐅕������z�u
                Dim ColAlignment As HorizontalAlignment = .Columns(e.Column).TextAlign

                ' �\�[�g
                .ListViewItemSorter = New CASTCommon.ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)

                ' �\�[�g���s
                .Sort()

            End With

        Catch ex As Exception
            Throw
        End Try

    End Sub
#End Region

    Private Sub btnAllon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOn.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)�J�n", "����", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = True '((CType(sender, Button).Name.ToUpper = "BTNALLON")) OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)", "���s", ex.Message)
        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)�I��", "����", "")

        End Try

    End Sub

    Private Sub btnAlloff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOff.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)�J�n", "����", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = False 'OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)", "���s", ex.Message)
        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)�I��", "����", "")

        End Try

    End Sub

    Private Sub btnRef_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRef.Click

        Dim iRet As Integer

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�J�n", "����", "")


            '�e�L�X�g�{�b�N�X���̓`�F�b�N
            If fn_check_TEXT() = False Then
                Return
            End If

            '���X�g��ʂ̏�����
            Me.ListView1.Items.Clear()

            KESSAI_DATE = txtKijyunDateY.Text & txtKijyunDateM.Text & txtKijyunDateD.Text

            MainDB = New CASTCommon.MyOracle

            iRet = KessaiDataList()

            If iRet = 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return
            ElseIf iRet = -1 Then

                Return

            Else

                Me.btnRef.Enabled = False
                Me.btnClear.Enabled = True
                Me.btnAllOn.Enabled = True
                Me.btnAllOff.Enabled = True
                Me.btnAction.Enabled = True
                Me.txtKijyunDateY.Enabled = False
                Me.txtKijyunDateM.Enabled = False
                Me.txtKijyunDateD.Enabled = False
                Me.btnAction.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)", "���s", ex.Message)

        Finally

            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�I��", "����", "")

        End Try

    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���A)�J�n", "����", "")

            '����ɃV�X�e�����t��\��
            '�x���}�X�^��荞��
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '��ʕ\�����ɐU�֓��ɑO�c�Ɠ���\������
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            '���X�g��ʂ̏�����
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False
            Me.txtKijyunDateY.Enabled = True
            Me.txtKijyunDateM.Enabled = True
            Me.txtKijyunDateD.Enabled = True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���A)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���A)�I��", "����", "")
        End Try

    End Sub

End Class
