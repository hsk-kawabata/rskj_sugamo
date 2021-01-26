Imports System
Imports System.IO
Imports System.Text

' 資金決済データフォーマット
Public Class ClsFormKes

    ' 固定長構造体用インターフェース
    Protected Interface IFormat
        ' データ
        Sub Init()
        Property Data() As String
    End Interface

    ' SHIT-JISエンコーディング
    Protected Friend Shared EncdJ As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")

#Region "資金決済／手数料徴求データ 共通部"
    '　資金決済／手数料徴求データ 共通部
    Public Structure KessaiData
        Implements ClsFormKes.IFormat

        '--------詳細部----------
        Dim record320 As String             'オペコードごとのデータ

        Dim OpeCode As String           '取引コード
        '--------ヘッダ--------
        Dim KessaiDate As String         '決済日
        '--------グループ------
        Dim TorisCode As String          '取引先主コード
        Dim TorifCode As String          '取引先副コード
        Dim ToriKName As String          '取引先名
        Dim ToriNName As String          '取引先名(日本語)
        Dim FuriCode As String           '振替コード
        Dim KigyoCode As String          '企業コード
        Dim FuriDate As String           '振替日
        Dim KessaiKbn As String          '決済区分
        Dim KesKinCode As String         '決済金融機関コード
        Dim KesSitCode As String         '決済支店コード
        Dim KesKamoku As String          '決済科目
        Dim KesKouza As String           '決済口座番号
        Dim TesTyoKbn As String          '手数料徴求区分
        Dim TesTyohh As String           '手数料徴求方法
        Dim TorimatomeSit As String      'とりまとめ店コード
        Dim SyoriKen As String              '請求件数
        Dim Syorikin As String              '請求金額
        Dim FunouKen As String              '不能件数
        Dim FunouKin As String              '不能金額
        Dim FuriKen As String               '振替件数
        Dim FuriKin As String               '振替金額
        Dim TesuuKin As String              '手数料
        Dim JifutiTesuuKin As String        '自振手数料
        Dim FurikomiTesuukin As String      '振込手数料
        Dim SonotaTesuuKin As String        'その他手数料
        Dim NyukinKen As String             '入金件数
        Dim NyukinKin As String             '入金金額
        Dim ToriKbn As String               '資金決済と手数料、資金決済のみ、手数料のみ
        Dim TesuuTyoFlg As String           '手数料徴求済区分
        Dim Tesuukin1 As String             '手数料金額１
        Dim tesuukin2 As String            '手数料金額２
        Dim tesuukin3 As String            '手数料金額３
        Dim ope_nyukin As String            '入金金額オペごと
        Dim ope_tesuu As String            '手数料金額オペごと
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
        Dim HonbuKouza As String            '本部別段口座番号
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

        Public Sub Init() Implements IFormat.Init
            record320 = ""             'オペコードごとのデータ
            OpeCode = ""
            KessaiDate = ""         '決済日
            TorisCode = ""          '取引先主コード
            TorifCode = ""          '取引先副コード
            ToriKName = ""          '取引先名
            ToriNName = ""          '取引先名(日本語)
            FuriCode = ""           '振替コード
            KigyoCode = ""          '企業コード
            FuriDate = ""           '振替日
            KessaiKbn = ""          '決済区分
            KesKinCode = ""         '決済金融機関コード
            KesSitCode = ""         '決済支店コード
            KesKamoku = ""          '決済科目
            KesKouza = ""           '決済口座番号
            TesTyoKbn = ""          '手数料徴求区分
            TesTyohh = ""           '手数料徴求方法
            TorimatomeSit = ""      'とりまとめ店コード
            SyoriKen = ""              '請求件数
            Syorikin = ""              '請求金額
            FunouKen = ""              '不能件数
            FunouKin = ""              '不能金額
            FuriKen = ""               '振替件数
            FuriKin = ""               '振替金額
            TesuuKin = ""              '手数料
            JifutiTesuuKin = ""        '自振手数料
            FurikomiTesuukin = ""      '振込手数料
            SonotaTesuuKin = ""        'その他手数料
            NyukinKen = ""             '入金件数
            NyukinKin = ""             '入金金額
            ToriKbn = ""                 '
            TesuuTyoFlg = ""
            Tesuukin1 = ""              '手数料１
            tesuukin2 = ""              '手数料２
            tesuukin3 = ""              '手数料３
            ope_nyukin = ""             '入金金額オペごと
            ope_tesuu = ""            '手数料金額オペごと
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
            HonbuKouza = ""             '本部別段口座番号
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
        End Sub

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements IFormat.Data
            Get
                Dim record As String = ""

                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                '本部別段口座番号追加
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
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
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
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                HonbuKouza = CuttingData(value, 7)
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
            End Set

        End Property
    End Structure

    '　自振契約リエンタデータ 共通部
    Public Structure JifkeiData
        Implements ClsFormKes.IFormat

        '--------詳細部----------
        Dim record320 As String             'オペコードごとのデータ

        Dim OpeCode As String           '取引コード
        Dim SyoriDate As String         '処理日
        Dim TorisCode As String          '取引先主コード
        Dim TorifCode As String          '取引先副コード
        Dim FuriDate As String           '振替日
        Dim MeiRecordNo As String           'レコード番号
        Dim ToriKName As String          '取引先名
        Dim ToriNName As String          '取引先名(日本語)
        Dim FuriCode As String           '振替コード
        Dim KigyoCode As String          '企業コード
        Dim KeiyakuKname As String      '契約者カナ

        Public Sub Init() Implements IFormat.Init
            record320 = ""          'オペコードごとのデータ
            OpeCode = ""
            SyoriDate = ""          '決済日
            TorisCode = ""          '取引先主コード
            TorifCode = ""          '取引先副コード
            FuriDate = ""           '振替日
            MeiRecordNo = ""           'レコード番号
            ToriKName = ""          '取引先名
            ToriNName = ""          '取引先名(日本語)
            FuriCode = ""           '振替コード
            KigyoCode = ""          '企業コード
            KeiyakuKname = ""       '契約者カナ
        End Sub

        '固定長データ処理用プロパティ
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
    '　資金確保リエンタデータ 共通部
    Public Structure KakuhoData
        Implements ClsFormKes.IFormat

        '--------詳細部----------
        Dim record320 As String         'オペコードごとのデータ

        Dim OpeCode As String           '取引コード
        '--------ヘッダ--------
        Dim KessaiDate As String        '決済日
        '--------グループ------
        Dim TorisCode As String         '取引先主コード
        Dim TorifCode As String         '取引先副コード
        Dim ToriKName As String         '取引先名
        Dim ToriNName As String         '取引先名(日本語)
        Dim FuriCode As String          '振替コード
        Dim KigyoCode As String         '企業コード
        Dim FuriDate As String          '振替日
        Dim KessaiKbn As String         '決済区分
        Dim KesKinCode As String        '決済金融機関コード
        Dim KesSitCode As String        '決済支店コード
        Dim KesKamoku As String         '決済科目
        Dim KesKouza As String          '決済口座番号
        Dim KesMeigi As String          '決済名義人
        Dim TesTyoKbn As String         '手数料徴求区分
        Dim TesTyohh As String          '手数料徴求方法
        Dim TesKinCode As String        '手数料徴求金融機関コード
        Dim TesSitCode As String        '手数料徴求支店コード
        Dim TesKamoku As String         '手数料徴求科目
        Dim TesKouza As String          '手数料徴求口座番号
        Dim TorimatomeSit As String     'とりまとめ店コード
        Dim SyoriKen As String          '請求件数
        Dim Syorikin As String          '請求金額
        Dim FunouKen As String          '不能件数
        Dim FunouKin As String          '不能金額
        Dim FuriKen As String           '振替件数
        Dim FuriKin As String           '振替金額
        Dim TesuuKin As String          '手数料
        Dim JifutiTesuuKin As String    '自振手数料
        Dim FurikomiTesuukin As String  '振込手数料
        Dim SonotaTesuuKin As String    'その他手数料
        Dim NyukinKen As String         '入金件数
        Dim NyukinKin As String         '入金金額
        Dim ToriKbn As String           '資金決済と手数料、資金決済のみ、手数料のみ
        Dim TesuuTyoFlg As String       '手数料徴求済区分
        Dim Tesuukin1 As String         '手数料金額１
        Dim tesuukin2 As String         '手数料金額２
        Dim tesuukin3 As String         '手数料金額３
        Dim ope_nyukin As String        '入金金額オペごと
        Dim ope_tesuu As String         '手数料金額オペごと

        Public Sub Init() Implements IFormat.Init
            record320 = ""              'オペコードごとのデータ
            OpeCode = ""
            KessaiDate = ""             '決済日
            TorisCode = ""              '取引先主コード
            TorifCode = ""              '取引先副コード
            ToriKName = ""              '取引先名
            ToriNName = ""              '取引先名(日本語)
            FuriCode = ""               '振替コード
            KigyoCode = ""              '企業コード
            FuriDate = ""               '振替日
            KessaiKbn = ""              '決済区分
            KesKinCode = ""             '決済金融機関コード
            KesSitCode = ""             '決済支店コード
            KesKamoku = ""              '決済科目
            KesKouza = ""               '決済口座番号
            KesMeigi = ""               '決済名義人
            TesTyoKbn = ""              '手数料徴求区分
            TesTyohh = ""               '手数料徴求方法
            TesKinCode = ""             '手数料徴求金融機関コード
            TesSitCode = ""             '手数料徴求支店コード
            TesKamoku = ""              '手数料徴求科目
            TesKouza = ""               '手数料徴求口座番号
            TorimatomeSit = ""          'とりまとめ店コード
            SyoriKen = ""               '請求件数
            Syorikin = ""               '請求金額
            FunouKen = ""               '不能件数
            FunouKin = ""               '不能金額
            FuriKen = ""                '振替件数
            FuriKin = ""                '振替金額
            TesuuKin = ""               '手数料
            JifutiTesuuKin = ""         '自振手数料
            FurikomiTesuukin = ""       '振込手数料
            SonotaTesuuKin = ""         'その他手数料
            NyukinKen = ""              '入金件数
            NyukinKin = ""              '入金金額
            ToriKbn = ""                     '
            TesuuTyoFlg = ""
            Tesuukin1 = ""              '手数料１
            tesuukin2 = ""              '手数料２
            tesuukin3 = ""              '手数料３
            ope_nyukin = ""             '入金金額オペごと
            ope_tesuu = ""              '手数料金額オペごと
        End Sub

        '固定長データ処理用プロパティ
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

    '　資金決済／手数料徴求データ 共通部
    Public Structure KessaiDataKinBatch
        Implements ClsFormKes.IFormat

        '--------詳細部----------
        Dim record320 As String             'オペコードごとのデータ

        Dim OpeCode As String           '取引コード
        '--------ヘッダ--------
        Dim KessaiDate As String         '決済日
        '--------グループ------
        Dim TorisCode As String          '取引先主コード
        Dim TorifCode As String          '取引先副コード
        Dim ToriKName As String          '取引先名
        Dim ToriNName As String          '取引先名(日本語)
        Dim FuriCode As String           '振替コード
        Dim KigyoCode As String          '企業コード
        Dim FuriDate As String           '振替日
        Dim KessaiKbn As String          '決済区分
        Dim KesKinCode As String         '決済金融機関コード
        Dim KesSitCode As String         '決済支店コード
        Dim KesKamoku As String          '決済科目
        Dim KesKouza As String           '決済口座番号
        Dim TesTyoKbn As String          '手数料徴求区分
        Dim TesTyohh As String           '手数料徴求方法
        Dim TorimatomeSit As String      'とりまとめ店コード
        Dim SyoriKen As String              '請求件数
        Dim Syorikin As String              '請求金額
        Dim FunouKen As String              '不能件数
        Dim FunouKin As String              '不能金額
        Dim FuriKen As String               '振替件数
        Dim FuriKin As String               '振替金額
        Dim TesuuKin As String              '手数料
        Dim JifutiTesuuKin As String        '自振手数料
        Dim FurikomiTesuukin As String      '振込手数料
        Dim SonotaTesuuKin As String        'その他手数料
        Dim NyukinKen As String             '入金件数
        Dim NyukinKin As String             '入金金額
        Dim ToriKbn As String               '資金決済と手数料、資金決済のみ、手数料のみ
        Dim TesuuTyoFlg As String           '手数料徴求済区分
        Dim Tesuukin1 As String             '手数料金額１
        Dim tesuukin2 As String            '手数料金額２
        Dim tesuukin3 As String            '手数料金額３
        Dim ope_nyukin As String            '入金金額オペごと
        Dim ope_tesuu As String            '手数料金額オペごと
        Dim ItakuCode As String             '委託者コード
        Dim Syubetu As String               '種別コード
        Dim BaitaiCode As String            '媒体コード
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
        Dim HonbuKouza As String            '本部別段口座番号
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

        Public Sub Init() Implements IFormat.Init
            record320 = ""             'オペコードごとのデータ
            OpeCode = ""
            KessaiDate = ""         '決済日
            TorisCode = ""          '取引先主コード
            TorifCode = ""          '取引先副コード
            ToriKName = ""          '取引先名
            ToriNName = ""          '取引先名(日本語)
            FuriCode = ""           '振替コード
            KigyoCode = ""          '企業コード
            FuriDate = ""           '振替日
            KessaiKbn = ""          '決済区分
            KesKinCode = ""         '決済金融機関コード
            KesSitCode = ""         '決済支店コード
            KesKamoku = ""          '決済科目
            KesKouza = ""           '決済口座番号
            TesTyoKbn = ""          '手数料徴求区分
            TesTyohh = ""           '手数料徴求方法
            TorimatomeSit = ""      'とりまとめ店コード
            SyoriKen = ""              '請求件数
            Syorikin = ""              '請求金額
            FunouKen = ""              '不能件数
            FunouKin = ""              '不能金額
            FuriKen = ""               '振替件数
            FuriKin = ""               '振替金額
            TesuuKin = ""              '手数料
            JifutiTesuuKin = ""        '自振手数料
            FurikomiTesuukin = ""      '振込手数料
            SonotaTesuuKin = ""        'その他手数料
            NyukinKen = ""             '入金件数
            NyukinKin = ""             '入金金額
            ToriKbn = ""                 '
            TesuuTyoFlg = ""
            Tesuukin1 = ""              '手数料１
            tesuukin2 = ""              '手数料２
            tesuukin3 = ""              '手数料３
            ope_nyukin = ""             '入金金額オペごと
            ope_tesuu = ""            '手数料金額オペごと
            ItakuCode = ""          '委託者コード
            Syubetu = ""            '種別コード
            BaitaiCode = ""         '媒体コード
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
            HonbuKouza = ""             '本部別段口座番号
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
        End Sub

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements IFormat.Data
            Get
                Dim record As String = ""

                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                '本部別段口座番号追加
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
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
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
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                HonbuKouza = CuttingData(value, 7)
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
            End Set

        End Property
    End Structure

#End Region

    '
    ' 機能　 ： 文字列から，指定の長さを切り取る
    '
    ' 引数　 ： ARG1 - 文字列
    ' 　　　 　 ARG2 - 長さ
    '           ARG3 - ０：左詰，１：右詰
    '           ARG4 - 埋め文字
    '
    ' 戻り値 ： 切り取った後の残りの文字列
    '
    ' 備考　 ：
    '
    Protected Friend Shared Function SubData(ByVal value As String, ByVal len As Integer, _
                    Optional ByVal align As Integer = 0, Optional ByVal pad As Char = " "c) As String
        Try
            If len = 0 Then
                Return ""
            End If

            ' 切り取る文字列
            If align = 0 Then
                ' 左詰
                value = value.PadRight(len, pad)
            Else
                ' 右詰
                value = value.PadLeft(len, pad)
            End If

            ' 切り取る文字列
            Dim bt() As Byte = EncdJ.GetBytes(value)
            Return EncdJ.GetString(bt, 0, len)
        Catch ex As Exception
            Return New String(" "c, len)
        End Try
    End Function

    '
    ' 機能　 ： 文字列から，指定の長さを切り取る
    '
    ' 引数　 ： ARG1 - 文字列
    ' 　　　 　 ARG2 - 長さ
    '
    ' 戻り値 ： 切り取った後の残りの文字列
    '
    ' 備考　 ：
    '
    Protected Friend Shared Function CuttingData(ByRef value As String, ByVal len As Integer) As String
        Try
            ' 切り取る文字列
            Dim ret As String
            Dim bt() As Byte = EncdJ.GetBytes(value)
            ret = EncdJ.GetString(bt, 0, len)
            ' 切り取った後の残りの文字列
            value = value.Substring(ret.Length())
            Return ret
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class
