
'共通 リテラル定義モジュール
'オラクル定義
Public Class DB
    Public Shared ReadOnly USER As String = "KZFMAST"               'ユーザ
    Public Shared ReadOnly PASSWORD As String = "KZFMAST"           'パスワード
    Public Shared ReadOnly SOURCE As String = "RSKJ_LSNR"           'データソース名
    '接続文字列
    Public Shared ReadOnly CONNECT As String = String.Format("User={0};Password={1};Data Source={2};Pooling=false;", _
                                                USER, _
                                                PASSWORD, _
                                                SOURCE)

End Class
