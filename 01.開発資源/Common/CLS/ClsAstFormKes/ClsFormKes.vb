Imports System
Imports System.IO
Imports System.Text

' �������σf�[�^�t�H�[�}�b�g
Public Class ClsFormKes

    ' �Œ蒷�\���̗p�C���^�[�t�F�[�X
    Protected Interface IFormat
        ' �f�[�^
        Sub Init()
        Property Data() As String
    End Interface

    ' SHIT-JIS�G���R�[�f�B���O
    Protected Friend Shared EncdJ As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")

#Region "�������ρ^�萔�������f�[�^ ���ʕ�"
    '�@�������ρ^�萔�������f�[�^ ���ʕ�
    Public Structure KessaiData
        Implements ClsFormKes.IFormat

        '--------�ڍו�----------
        Dim record320 As String             '�I�y�R�[�h���Ƃ̃f�[�^

        Dim OpeCode As String           '����R�[�h
        '--------�w�b�_--------
        Dim KessaiDate As String         '���ϓ�
        '--------�O���[�v------
        Dim TorisCode As String          '������R�[�h
        Dim TorifCode As String          '����敛�R�[�h
        Dim ToriKName As String          '����於
        Dim ToriNName As String          '����於(���{��)
        Dim FuriCode As String           '�U�փR�[�h
        Dim KigyoCode As String          '��ƃR�[�h
        Dim FuriDate As String           '�U�֓�
        Dim KessaiKbn As String          '���ϋ敪
        Dim KesKinCode As String         '���ϋ��Z�@�փR�[�h
        Dim KesSitCode As String         '���ώx�X�R�[�h
        Dim KesKamoku As String          '���ωȖ�
        Dim KesKouza As String           '���ό����ԍ�
        Dim TesTyoKbn As String          '�萔�������敪
        Dim TesTyohh As String           '�萔���������@
        Dim TorimatomeSit As String      '�Ƃ�܂ƂߓX�R�[�h
        Dim SyoriKen As String              '��������
        Dim Syorikin As String              '�������z
        Dim FunouKen As String              '�s�\����
        Dim FunouKin As String              '�s�\���z
        Dim FuriKen As String               '�U�֌���
        Dim FuriKin As String               '�U�֋��z
        Dim TesuuKin As String              '�萔��
        Dim JifutiTesuuKin As String        '���U�萔��
        Dim FurikomiTesuukin As String      '�U���萔��
        Dim SonotaTesuuKin As String        '���̑��萔��
        Dim NyukinKen As String             '��������
        Dim NyukinKin As String             '�������z
        Dim ToriKbn As String               '�������ςƎ萔���A�������ς̂݁A�萔���̂�
        Dim TesuuTyoFlg As String           '�萔�������ϋ敪
        Dim Tesuukin1 As String             '�萔�����z�P
        Dim tesuukin2 As String            '�萔�����z�Q
        Dim tesuukin3 As String            '�萔�����z�R
        Dim ope_nyukin As String            '�������z�I�y����
        Dim ope_tesuu As String            '�萔�����z�I�y����
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
        Dim HonbuKouza As String            '�{���ʒi�����ԍ�
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

        Public Sub Init() Implements IFormat.Init
            record320 = ""             '�I�y�R�[�h���Ƃ̃f�[�^
            OpeCode = ""
            KessaiDate = ""         '���ϓ�
            TorisCode = ""          '������R�[�h
            TorifCode = ""          '����敛�R�[�h
            ToriKName = ""          '����於
            ToriNName = ""          '����於(���{��)
            FuriCode = ""           '�U�փR�[�h
            KigyoCode = ""          '��ƃR�[�h
            FuriDate = ""           '�U�֓�
            KessaiKbn = ""          '���ϋ敪
            KesKinCode = ""         '���ϋ��Z�@�փR�[�h
            KesSitCode = ""         '���ώx�X�R�[�h
            KesKamoku = ""          '���ωȖ�
            KesKouza = ""           '���ό����ԍ�
            TesTyoKbn = ""          '�萔�������敪
            TesTyohh = ""           '�萔���������@
            TorimatomeSit = ""      '�Ƃ�܂ƂߓX�R�[�h
            SyoriKen = ""              '��������
            Syorikin = ""              '�������z
            FunouKen = ""              '�s�\����
            FunouKin = ""              '�s�\���z
            FuriKen = ""               '�U�֌���
            FuriKin = ""               '�U�֋��z
            TesuuKin = ""              '�萔��
            JifutiTesuuKin = ""        '���U�萔��
            FurikomiTesuukin = ""      '�U���萔��
            SonotaTesuuKin = ""        '���̑��萔��
            NyukinKen = ""             '��������
            NyukinKin = ""             '�������z
            ToriKbn = ""                 '
            TesuuTyoFlg = ""
            Tesuukin1 = ""              '�萔���P
            tesuukin2 = ""              '�萔���Q
            tesuukin3 = ""              '�萔���R
            ope_nyukin = ""             '�������z�I�y����
            ope_tesuu = ""            '�萔�����z�I�y����
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            HonbuKouza = ""             '�{���ʒi�����ԍ�
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
        End Sub

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements IFormat.Data
            Get
                Dim record As String = ""

                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                '�{���ʒi�����ԍ��ǉ�
                record = String.Concat(New String() _
                                 { _
                                 SubData(record320, 320), _
                                 SubData(OpeCode, 5), _
                                 SubData(KessaiDate, 8), _
                                 SubData(TorisCode, 10), _
                                 SubData(TorifCode, 2), _
                                 SubData(ToriKName, 40), _
                                 SubData(ToriNName, 50), _
                                 SubData(FuriCode, 3), _
                                 SubData(KigyoCode, 5), _
                                 SubData(FuriDate, 8), _
                                 SubData(KessaiKbn, 2), _
                                 SubData(KesKinCode, 4), _
                                 SubData(KesSitCode, 3), _
                                 SubData(KesKamoku, 2), _
                                 SubData(KesKouza, 7), _
                                 SubData(TesTyoKbn, 1), _
                                 SubData(TesTyohh, 1), _
                                 SubData(TorimatomeSit, 3), _
                                 SubData(SyoriKen, 6), _
                                 SubData(Syorikin, 13), _
                                 SubData(FunouKen, 6), _
                                 SubData(FunouKin, 13), _
                                 SubData(FuriKen, 6), _
                                 SubData(FuriKin, 13), _
                                 SubData(TesuuKin, 13), _
                                 SubData(JifutiTesuuKin, 13), _
                                 SubData(FurikomiTesuukin, 13), _
                                 SubData(SonotaTesuuKin, 13), _
                                 SubData(NyukinKen, 6), _
                                 SubData(NyukinKin, 13), _
                                 SubData(ToriKbn, 1), _
                                 SubData(TesuuTyoFlg, 1), _
                                 SubData(Tesuukin1, 13), _
                                 SubData(tesuukin2, 13), _
                                 SubData(tesuukin3, 13), _
                                 SubData(ope_nyukin, 13), _
                                 SubData(ope_tesuu, 13), _
                                 SubData(HonbuKouza, 7) _
                                 })
                'record = String.Concat(New String() _
                '                             { _
                '                             SubData(record320, 320), _
                '                             SubData(OpeCode, 5), _
                '                             SubData(KessaiDate, 8), _
                '                             SubData(TorisCode, 10), _
                '                             SubData(TorifCode, 2), _
                '                             SubData(ToriKName, 40), _
                '                             SubData(ToriNName, 50), _
                '                             SubData(FuriCode, 3), _
                '                             SubData(KigyoCode, 5), _
                '                             SubData(FuriDate, 8), _
                '                             SubData(KessaiKbn, 2), _
                '                             SubData(KesKinCode, 4), _
                '                             SubData(KesSitCode, 3), _
                '                             SubData(KesKamoku, 2), _
                '                             SubData(KesKouza, 7), _
                '                             SubData(TesTyoKbn, 1), _
                '                             SubData(TesTyohh, 1), _
                '                             SubData(TorimatomeSit, 3), _
                '                             SubData(SyoriKen, 6), _
                '                             SubData(Syorikin, 13), _
                '                             SubData(FunouKen, 6), _
                '                             SubData(FunouKin, 13), _
                '                             SubData(FuriKen, 6), _
                '                             SubData(FuriKin, 13), _
                '                             SubData(TesuuKin, 13), _
                '                             SubData(JifutiTesuuKin, 13), _
                '                             SubData(FurikomiTesuukin, 13), _
                '                             SubData(SonotaTesuuKin, 13), _
                '                             SubData(NyukinKen, 6), _
                '                             SubData(NyukinKin, 13), _
                '                             SubData(ToriKbn, 1), _
                '                             SubData(TesuuTyoFlg, 1), _
                '                             SubData(Tesuukin1, 13), _
                '                             SubData(tesuukin2, 13), _
                '                             SubData(tesuukin3, 13), _
                '                             SubData(ope_nyukin, 13), _
                '                             SubData(ope_tesuu, 13) _
                '                             })
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                Return record
            End Get
            Set(ByVal value As String)
                record320 = CuttingData(value, 320)
                OpeCode = CuttingData(value, 5)
                KessaiDate = CuttingData(value, 8)
                TorisCode = CuttingData(value, 10)
                TorifCode = CuttingData(value, 2)
                ToriKName = CuttingData(value, 40)
                ToriNName = CuttingData(value, 50)
                FuriCode = CuttingData(value, 3)
                KigyoCode = CuttingData(value, 5)
                FuriDate = CuttingData(value, 8)
                KessaiKbn = CuttingData(value, 2)
                KesKinCode = CuttingData(value, 4)
                KesSitCode = CuttingData(value, 3)
                KesKamoku = CuttingData(value, 2)
                KesKouza = CuttingData(value, 7)
                TesTyoKbn = CuttingData(value, 1)
                TesTyohh = CuttingData(value, 1)
                TorimatomeSit = CuttingData(value, 3)
                SyoriKen = CuttingData(value, 6)
                Syorikin = CuttingData(value, 13)
                FunouKen = CuttingData(value, 6)
                FunouKin = CuttingData(value, 13)
                FuriKen = CuttingData(value, 6)
                FuriKin = CuttingData(value, 13)
                TesuuKin = CuttingData(value, 13)
                JifutiTesuuKin = CuttingData(value, 13)
                FurikomiTesuukin = CuttingData(value, 13)
                SonotaTesuuKin = CuttingData(value, 13)
                NyukinKen = CuttingData(value, 6)
                NyukinKin = CuttingData(value, 13)
                ToriKbn = CuttingData(value, 1)
                TesuuTyoFlg = CuttingData(value, 1)
                Tesuukin1 = CuttingData(value, 13)
                tesuukin2 = CuttingData(value, 13)
                tesuukin3 = CuttingData(value, 13)
                ope_nyukin = CuttingData(value, 13)
                ope_tesuu = CuttingData(value, 13)
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                HonbuKouza = CuttingData(value, 7)
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
            End Set

        End Property
    End Structure

    '�@���U�_�񃊃G���^�f�[�^ ���ʕ�
    Public Structure JifkeiData
        Implements ClsFormKes.IFormat

        '--------�ڍו�----------
        Dim record320 As String             '�I�y�R�[�h���Ƃ̃f�[�^

        Dim OpeCode As String           '����R�[�h
        Dim SyoriDate As String         '������
        Dim TorisCode As String          '������R�[�h
        Dim TorifCode As String          '����敛�R�[�h
        Dim FuriDate As String           '�U�֓�
        Dim MeiRecordNo As String           '���R�[�h�ԍ�
        Dim ToriKName As String          '����於
        Dim ToriNName As String          '����於(���{��)
        Dim FuriCode As String           '�U�փR�[�h
        Dim KigyoCode As String          '��ƃR�[�h
        Dim KeiyakuKname As String      '�_��҃J�i

        Public Sub Init() Implements IFormat.Init
            record320 = ""          '�I�y�R�[�h���Ƃ̃f�[�^
            OpeCode = ""
            SyoriDate = ""          '���ϓ�
            TorisCode = ""          '������R�[�h
            TorifCode = ""          '����敛�R�[�h
            FuriDate = ""           '�U�֓�
            MeiRecordNo = ""           '���R�[�h�ԍ�
            ToriKName = ""          '����於
            ToriNName = ""          '����於(���{��)
            FuriCode = ""           '�U�փR�[�h
            KigyoCode = ""          '��ƃR�[�h
            KeiyakuKname = ""       '�_��҃J�i
        End Sub

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements IFormat.Data
            Get
                Dim record As String = ""

                record = String.Concat(New String() _
                            { _
                            SubData(record320, 320), _
                            SubData(OpeCode, 5), _
                            SubData(SyoriDate, 8), _
                            SubData(TorisCode, 10), _
                            SubData(TorifCode, 2), _
                            SubData(FuriDate, 8), _
                            SubData(MeiRecordNo, 8), _
                            SubData(ToriKName, 40), _
                            SubData(ToriNName, 50), _
                            SubData(FuriCode, 3), _
                            SubData(KigyoCode, 5), _
                            SubData(KeiyakuKname, 40) _
                            })
                Return record
            End Get
            Set(ByVal value As String)
                record320 = CuttingData(value, 320)
                OpeCode = CuttingData(value, 5)
                SyoriDate = CuttingData(value, 8)
                TorisCode = CuttingData(value, 10)
                TorifCode = CuttingData(value, 2)
                FuriDate = CuttingData(value, 8)
                MeiRecordNo = CuttingData(value, 8)
                ToriKName = CuttingData(value, 40)
                ToriNName = CuttingData(value, 50)
                FuriCode = CuttingData(value, 3)
                KigyoCode = CuttingData(value, 5)
                KeiyakuKname = CuttingData(value, 40)
            End Set

        End Property
    End Structure
    '�@�����m�ۃ��G���^�f�[�^ ���ʕ�
    Public Structure KakuhoData
        Implements ClsFormKes.IFormat

        '--------�ڍו�----------
        Dim record320 As String         '�I�y�R�[�h���Ƃ̃f�[�^

        Dim OpeCode As String           '����R�[�h
        '--------�w�b�_--------
        Dim KessaiDate As String        '���ϓ�
        '--------�O���[�v------
        Dim TorisCode As String         '������R�[�h
        Dim TorifCode As String         '����敛�R�[�h
        Dim ToriKName As String         '����於
        Dim ToriNName As String         '����於(���{��)
        Dim FuriCode As String          '�U�փR�[�h
        Dim KigyoCode As String         '��ƃR�[�h
        Dim FuriDate As String          '�U�֓�
        Dim KessaiKbn As String         '���ϋ敪
        Dim KesKinCode As String        '���ϋ��Z�@�փR�[�h
        Dim KesSitCode As String        '���ώx�X�R�[�h
        Dim KesKamoku As String         '���ωȖ�
        Dim KesKouza As String          '���ό����ԍ�
        Dim KesMeigi As String          '���ϖ��`�l
        Dim TesTyoKbn As String         '�萔�������敪
        Dim TesTyohh As String          '�萔���������@
        Dim TesKinCode As String        '�萔���������Z�@�փR�[�h
        Dim TesSitCode As String        '�萔�������x�X�R�[�h
        Dim TesKamoku As String         '�萔�������Ȗ�
        Dim TesKouza As String          '�萔�����������ԍ�
        Dim TorimatomeSit As String     '�Ƃ�܂ƂߓX�R�[�h
        Dim SyoriKen As String          '��������
        Dim Syorikin As String          '�������z
        Dim FunouKen As String          '�s�\����
        Dim FunouKin As String          '�s�\���z
        Dim FuriKen As String           '�U�֌���
        Dim FuriKin As String           '�U�֋��z
        Dim TesuuKin As String          '�萔��
        Dim JifutiTesuuKin As String    '���U�萔��
        Dim FurikomiTesuukin As String  '�U���萔��
        Dim SonotaTesuuKin As String    '���̑��萔��
        Dim NyukinKen As String         '��������
        Dim NyukinKin As String         '�������z
        Dim ToriKbn As String           '�������ςƎ萔���A�������ς̂݁A�萔���̂�
        Dim TesuuTyoFlg As String       '�萔�������ϋ敪
        Dim Tesuukin1 As String         '�萔�����z�P
        Dim tesuukin2 As String         '�萔�����z�Q
        Dim tesuukin3 As String         '�萔�����z�R
        Dim ope_nyukin As String        '�������z�I�y����
        Dim ope_tesuu As String         '�萔�����z�I�y����

        Public Sub Init() Implements IFormat.Init
            record320 = ""              '�I�y�R�[�h���Ƃ̃f�[�^
            OpeCode = ""
            KessaiDate = ""             '���ϓ�
            TorisCode = ""              '������R�[�h
            TorifCode = ""              '����敛�R�[�h
            ToriKName = ""              '����於
            ToriNName = ""              '����於(���{��)
            FuriCode = ""               '�U�փR�[�h
            KigyoCode = ""              '��ƃR�[�h
            FuriDate = ""               '�U�֓�
            KessaiKbn = ""              '���ϋ敪
            KesKinCode = ""             '���ϋ��Z�@�փR�[�h
            KesSitCode = ""             '���ώx�X�R�[�h
            KesKamoku = ""              '���ωȖ�
            KesKouza = ""               '���ό����ԍ�
            KesMeigi = ""               '���ϖ��`�l
            TesTyoKbn = ""              '�萔�������敪
            TesTyohh = ""               '�萔���������@
            TesKinCode = ""             '�萔���������Z�@�փR�[�h
            TesSitCode = ""             '�萔�������x�X�R�[�h
            TesKamoku = ""              '�萔�������Ȗ�
            TesKouza = ""               '�萔�����������ԍ�
            TorimatomeSit = ""          '�Ƃ�܂ƂߓX�R�[�h
            SyoriKen = ""               '��������
            Syorikin = ""               '�������z
            FunouKen = ""               '�s�\����
            FunouKin = ""               '�s�\���z
            FuriKen = ""                '�U�֌���
            FuriKin = ""                '�U�֋��z
            TesuuKin = ""               '�萔��
            JifutiTesuuKin = ""         '���U�萔��
            FurikomiTesuukin = ""       '�U���萔��
            SonotaTesuuKin = ""         '���̑��萔��
            NyukinKen = ""              '��������
            NyukinKin = ""              '�������z
            ToriKbn = ""                     '
            TesuuTyoFlg = ""
            Tesuukin1 = ""              '�萔���P
            tesuukin2 = ""              '�萔���Q
            tesuukin3 = ""              '�萔���R
            ope_nyukin = ""             '�������z�I�y����
            ope_tesuu = ""              '�萔�����z�I�y����
        End Sub

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements IFormat.Data
            Get
                Dim record As String = ""

                record = String.Concat(New String() _
                            { _
                            SubData(record320, 320), _
                            SubData(OpeCode, 5), _
                            SubData(KessaiDate, 8), _
                            SubData(TorisCode, 10), _
                            SubData(TorifCode, 2), _
                            SubData(ToriKName, 40), _
                            SubData(ToriNName, 50), _
                            SubData(FuriCode, 3), _
                            SubData(KigyoCode, 5), _
                            SubData(FuriDate, 8), _
                            SubData(KessaiKbn, 2), _
                            SubData(KesKinCode, 4), _
                            SubData(KesSitCode, 3), _
                            SubData(KesKamoku, 2), _
                            SubData(KesKouza, 7), _
                            SubData(KesMeigi, 40), _
                            SubData(TesTyoKbn, 1), _
                            SubData(TesTyohh, 1), _
                            SubData(TesKinCode, 4), _
                            SubData(TesSitCode, 3), _
                            SubData(TesKamoku, 2), _
                            SubData(TesKouza, 7), _
                            SubData(TorimatomeSit, 3), _
                            SubData(SyoriKen, 6), _
                            SubData(Syorikin, 13), _
                            SubData(FunouKen, 6), _
                            SubData(FunouKin, 13), _
                            SubData(FuriKen, 6), _
                            SubData(FuriKin, 13), _
                            SubData(TesuuKin, 13), _
                            SubData(JifutiTesuuKin, 13), _
                            SubData(FurikomiTesuukin, 13), _
                            SubData(SonotaTesuuKin, 13), _
                            SubData(NyukinKen, 6), _
                            SubData(NyukinKin, 13), _
                            SubData(ToriKbn, 1), _
                            SubData(TesuuTyoFlg, 1), _
                            SubData(Tesuukin1, 13), _
                            SubData(tesuukin2, 13), _
                            SubData(tesuukin3, 13), _
                            SubData(ope_nyukin, 13), _
                            SubData(ope_tesuu, 13) _
                            })
                Return record
            End Get
            Set(ByVal value As String)
                record320 = CuttingData(value, 320)
                OpeCode = CuttingData(value, 5)
                KessaiDate = CuttingData(value, 8)
                TorisCode = CuttingData(value, 10)
                TorifCode = CuttingData(value, 2)
                ToriKName = CuttingData(value, 40)
                ToriNName = CuttingData(value, 50)
                FuriCode = CuttingData(value, 3)
                KigyoCode = CuttingData(value, 5)
                FuriDate = CuttingData(value, 8)
                KessaiKbn = CuttingData(value, 2)
                KesKinCode = CuttingData(value, 4)
                KesSitCode = CuttingData(value, 3)
                KesKamoku = CuttingData(value, 2)
                KesKouza = CuttingData(value, 7)
                KesMeigi = CuttingData(value, 40)
                TesTyoKbn = CuttingData(value, 1)
                TesTyohh = CuttingData(value, 1)
                TesKinCode = CuttingData(value, 4)
                TesSitCode = CuttingData(value, 3)
                TesKamoku = CuttingData(value, 2)
                TesKouza = CuttingData(value, 7)
                TorimatomeSit = CuttingData(value, 3)
                SyoriKen = CuttingData(value, 6)
                Syorikin = CuttingData(value, 13)
                FunouKen = CuttingData(value, 6)
                FunouKin = CuttingData(value, 13)
                FuriKen = CuttingData(value, 6)
                FuriKin = CuttingData(value, 13)
                TesuuKin = CuttingData(value, 13)
                JifutiTesuuKin = CuttingData(value, 13)
                FurikomiTesuukin = CuttingData(value, 13)
                SonotaTesuuKin = CuttingData(value, 13)
                NyukinKen = CuttingData(value, 6)
                NyukinKin = CuttingData(value, 13)
                ToriKbn = CuttingData(value, 1)
                TesuuTyoFlg = CuttingData(value, 1)
                Tesuukin1 = CuttingData(value, 13)
                tesuukin2 = CuttingData(value, 13)
                tesuukin3 = CuttingData(value, 13)
                ope_nyukin = CuttingData(value, 13)
                ope_tesuu = CuttingData(value, 13)
            End Set

        End Property

    End Structure

    '�@�������ρ^�萔�������f�[�^ ���ʕ�
    Public Structure KessaiDataKinBatch
        Implements ClsFormKes.IFormat

        '--------�ڍו�----------
        Dim record320 As String             '�I�y�R�[�h���Ƃ̃f�[�^

        Dim OpeCode As String           '����R�[�h
        '--------�w�b�_--------
        Dim KessaiDate As String         '���ϓ�
        '--------�O���[�v------
        Dim TorisCode As String          '������R�[�h
        Dim TorifCode As String          '����敛�R�[�h
        Dim ToriKName As String          '����於
        Dim ToriNName As String          '����於(���{��)
        Dim FuriCode As String           '�U�փR�[�h
        Dim KigyoCode As String          '��ƃR�[�h
        Dim FuriDate As String           '�U�֓�
        Dim KessaiKbn As String          '���ϋ敪
        Dim KesKinCode As String         '���ϋ��Z�@�փR�[�h
        Dim KesSitCode As String         '���ώx�X�R�[�h
        Dim KesKamoku As String          '���ωȖ�
        Dim KesKouza As String           '���ό����ԍ�
        Dim TesTyoKbn As String          '�萔�������敪
        Dim TesTyohh As String           '�萔���������@
        Dim TorimatomeSit As String      '�Ƃ�܂ƂߓX�R�[�h
        Dim SyoriKen As String              '��������
        Dim Syorikin As String              '�������z
        Dim FunouKen As String              '�s�\����
        Dim FunouKin As String              '�s�\���z
        Dim FuriKen As String               '�U�֌���
        Dim FuriKin As String               '�U�֋��z
        Dim TesuuKin As String              '�萔��
        Dim JifutiTesuuKin As String        '���U�萔��
        Dim FurikomiTesuukin As String      '�U���萔��
        Dim SonotaTesuuKin As String        '���̑��萔��
        Dim NyukinKen As String             '��������
        Dim NyukinKin As String             '�������z
        Dim ToriKbn As String               '�������ςƎ萔���A�������ς̂݁A�萔���̂�
        Dim TesuuTyoFlg As String           '�萔�������ϋ敪
        Dim Tesuukin1 As String             '�萔�����z�P
        Dim tesuukin2 As String            '�萔�����z�Q
        Dim tesuukin3 As String            '�萔�����z�R
        Dim ope_nyukin As String            '�������z�I�y����
        Dim ope_tesuu As String            '�萔�����z�I�y����
        Dim ItakuCode As String             '�ϑ��҃R�[�h
        Dim Syubetu As String               '��ʃR�[�h
        Dim BaitaiCode As String            '�}�̃R�[�h
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
        Dim HonbuKouza As String            '�{���ʒi�����ԍ�
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

        Public Sub Init() Implements IFormat.Init
            record320 = ""             '�I�y�R�[�h���Ƃ̃f�[�^
            OpeCode = ""
            KessaiDate = ""         '���ϓ�
            TorisCode = ""          '������R�[�h
            TorifCode = ""          '����敛�R�[�h
            ToriKName = ""          '����於
            ToriNName = ""          '����於(���{��)
            FuriCode = ""           '�U�փR�[�h
            KigyoCode = ""          '��ƃR�[�h
            FuriDate = ""           '�U�֓�
            KessaiKbn = ""          '���ϋ敪
            KesKinCode = ""         '���ϋ��Z�@�փR�[�h
            KesSitCode = ""         '���ώx�X�R�[�h
            KesKamoku = ""          '���ωȖ�
            KesKouza = ""           '���ό����ԍ�
            TesTyoKbn = ""          '�萔�������敪
            TesTyohh = ""           '�萔���������@
            TorimatomeSit = ""      '�Ƃ�܂ƂߓX�R�[�h
            SyoriKen = ""              '��������
            Syorikin = ""              '�������z
            FunouKen = ""              '�s�\����
            FunouKin = ""              '�s�\���z
            FuriKen = ""               '�U�֌���
            FuriKin = ""               '�U�֋��z
            TesuuKin = ""              '�萔��
            JifutiTesuuKin = ""        '���U�萔��
            FurikomiTesuukin = ""      '�U���萔��
            SonotaTesuuKin = ""        '���̑��萔��
            NyukinKen = ""             '��������
            NyukinKin = ""             '�������z
            ToriKbn = ""                 '
            TesuuTyoFlg = ""
            Tesuukin1 = ""              '�萔���P
            tesuukin2 = ""              '�萔���Q
            tesuukin3 = ""              '�萔���R
            ope_nyukin = ""             '�������z�I�y����
            ope_tesuu = ""            '�萔�����z�I�y����
            ItakuCode = ""          '�ϑ��҃R�[�h
            Syubetu = ""            '��ʃR�[�h
            BaitaiCode = ""         '�}�̃R�[�h
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            HonbuKouza = ""             '�{���ʒi�����ԍ�
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
        End Sub

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements IFormat.Data
            Get
                Dim record As String = ""

                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                '�{���ʒi�����ԍ��ǉ�
                record = String.Concat(New String() _
                            { _
                            SubData(record320, 320), _
                            SubData(OpeCode, 5), _
                            SubData(KessaiDate, 8), _
                            SubData(TorisCode, 10), _
                            SubData(TorifCode, 2), _
                            SubData(ToriKName, 40), _
                            SubData(ToriNName, 50), _
                            SubData(FuriCode, 3), _
                            SubData(KigyoCode, 5), _
                            SubData(FuriDate, 8), _
                            SubData(KessaiKbn, 2), _
                            SubData(KesKinCode, 4), _
                            SubData(KesSitCode, 3), _
                            SubData(KesKamoku, 2), _
                            SubData(KesKouza, 7), _
                            SubData(TesTyoKbn, 1), _
                            SubData(TesTyohh, 1), _
                            SubData(TorimatomeSit, 3), _
                            SubData(SyoriKen, 6), _
                            SubData(Syorikin, 13), _
                            SubData(FunouKen, 6), _
                            SubData(FunouKin, 13), _
                            SubData(FuriKen, 6), _
                            SubData(FuriKin, 13), _
                            SubData(TesuuKin, 13), _
                            SubData(JifutiTesuuKin, 13), _
                            SubData(FurikomiTesuukin, 13), _
                            SubData(SonotaTesuuKin, 13), _
                            SubData(NyukinKen, 6), _
                            SubData(NyukinKin, 13), _
                            SubData(ToriKbn, 1), _
                            SubData(TesuuTyoFlg, 1), _
                            SubData(Tesuukin1, 13), _
                            SubData(tesuukin2, 13), _
                            SubData(tesuukin3, 13), _
                            SubData(ope_nyukin, 13), _
                            SubData(ope_tesuu, 13), _
                            SubData(ItakuCode, 10), _
                            SubData(Syubetu, 2), _
                            SubData(BaitaiCode, 2), _
                            SubData(HonbuKouza, 7) _
                            })
                'record = String.Concat(New String() _
                '            { _
                '            SubData(record320, 320), _
                '            SubData(OpeCode, 5), _
                '            SubData(KessaiDate, 8), _
                '            SubData(TorisCode, 10), _
                '            SubData(TorifCode, 2), _
                '            SubData(ToriKName, 40), _
                '            SubData(ToriNName, 50), _
                '            SubData(FuriCode, 3), _
                '            SubData(KigyoCode, 5), _
                '            SubData(FuriDate, 8), _
                '            SubData(KessaiKbn, 2), _
                '            SubData(KesKinCode, 4), _
                '            SubData(KesSitCode, 3), _
                '            SubData(KesKamoku, 2), _
                '            SubData(KesKouza, 7), _
                '            SubData(TesTyoKbn, 1), _
                '            SubData(TesTyohh, 1), _
                '            SubData(TorimatomeSit, 3), _
                '            SubData(SyoriKen, 6), _
                '            SubData(Syorikin, 13), _
                '            SubData(FunouKen, 6), _
                '            SubData(FunouKin, 13), _
                '            SubData(FuriKen, 6), _
                '            SubData(FuriKin, 13), _
                '            SubData(TesuuKin, 13), _
                '            SubData(JifutiTesuuKin, 13), _
                '            SubData(FurikomiTesuukin, 13), _
                '            SubData(SonotaTesuuKin, 13), _
                '            SubData(NyukinKen, 6), _
                '            SubData(NyukinKin, 13), _
                '            SubData(ToriKbn, 1), _
                '            SubData(TesuuTyoFlg, 1), _
                '            SubData(Tesuukin1, 13), _
                '            SubData(tesuukin2, 13), _
                '            SubData(tesuukin3, 13), _
                '            SubData(ope_nyukin, 13), _
                '            SubData(ope_tesuu, 13), _
                '            SubData(ItakuCode, 10), _
                '            SubData(Syubetu, 2), _
                '            SubData(BaitaiCode, 2) _
                '            })
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                Return record
            End Get
            Set(ByVal value As String)
                record320 = CuttingData(value, 320)
                OpeCode = CuttingData(value, 5)
                KessaiDate = CuttingData(value, 8)
                TorisCode = CuttingData(value, 10)
                TorifCode = CuttingData(value, 2)
                ToriKName = CuttingData(value, 40)
                ToriNName = CuttingData(value, 50)
                FuriCode = CuttingData(value, 3)
                KigyoCode = CuttingData(value, 5)
                FuriDate = CuttingData(value, 8)
                KessaiKbn = CuttingData(value, 2)
                KesKinCode = CuttingData(value, 4)
                KesSitCode = CuttingData(value, 3)
                KesKamoku = CuttingData(value, 2)
                KesKouza = CuttingData(value, 7)
                TesTyoKbn = CuttingData(value, 1)
                TesTyohh = CuttingData(value, 1)
                TorimatomeSit = CuttingData(value, 3)
                SyoriKen = CuttingData(value, 6)
                Syorikin = CuttingData(value, 13)
                FunouKen = CuttingData(value, 6)
                FunouKin = CuttingData(value, 13)
                FuriKen = CuttingData(value, 6)
                FuriKin = CuttingData(value, 13)
                TesuuKin = CuttingData(value, 13)
                JifutiTesuuKin = CuttingData(value, 13)
                FurikomiTesuukin = CuttingData(value, 13)
                SonotaTesuuKin = CuttingData(value, 13)
                NyukinKen = CuttingData(value, 6)
                NyukinKin = CuttingData(value, 13)
                ToriKbn = CuttingData(value, 1)
                TesuuTyoFlg = CuttingData(value, 1)
                Tesuukin1 = CuttingData(value, 13)
                tesuukin2 = CuttingData(value, 13)
                tesuukin3 = CuttingData(value, 13)
                ope_nyukin = CuttingData(value, 13)
                ope_tesuu = CuttingData(value, 13)
                ItakuCode = CuttingData(value, 10)
                Syubetu = CuttingData(value, 2)
                BaitaiCode = CuttingData(value, 2)
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                HonbuKouza = CuttingData(value, 7)
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
            End Set

        End Property
    End Structure

#End Region

    '
    ' �@�\�@ �F �����񂩂�C�w��̒�����؂���
    '
    ' �����@ �F ARG1 - ������
    ' �@�@�@ �@ ARG2 - ����
    '           ARG3 - �O�F���l�C�P�F�E�l
    '           ARG4 - ���ߕ���
    '
    ' �߂�l �F �؂�������̎c��̕�����
    '
    ' ���l�@ �F
    '
    Protected Friend Shared Function SubData(ByVal value As String, ByVal len As Integer, _
                    Optional ByVal align As Integer = 0, Optional ByVal pad As Char = " "c) As String
        Try
            If len = 0 Then
                Return ""
            End If

            ' �؂��镶����
            If align = 0 Then
                ' ���l
                value = value.PadRight(len, pad)
            Else
                ' �E�l
                value = value.PadLeft(len, pad)
            End If

            ' �؂��镶����
            Dim bt() As Byte = EncdJ.GetBytes(value)
            Return EncdJ.GetString(bt, 0, len)
        Catch ex As Exception
            Return New String(" "c, len)
        End Try
    End Function

    '
    ' �@�\�@ �F �����񂩂�C�w��̒�����؂���
    '
    ' �����@ �F ARG1 - ������
    ' �@�@�@ �@ ARG2 - ����
    '
    ' �߂�l �F �؂�������̎c��̕�����
    '
    ' ���l�@ �F
    '
    Protected Friend Shared Function CuttingData(ByRef value As String, ByVal len As Integer) As String
        Try
            ' �؂��镶����
            Dim ret As String
            Dim bt() As Byte = EncdJ.GetBytes(value)
            ret = EncdJ.GetString(bt, 0, len)
            ' �؂�������̎c��̕�����
            value = value.Substring(ret.Length())
            Return ret
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class
