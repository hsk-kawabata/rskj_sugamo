SPOOL c:\rskj\bat\企業自振システム／ユーザ一覧表.LOG

select trim(loginid_u) as ログインＩＤ,
PASSWORD_DATE_U as パスワード変更日,
decode(kengen_u,'0','一般','管理者') as 権限
from uidmast 
order by kengen_u desc,loginid_u;
SPOOL OFF

QUIT;
