SPOOL c:\rskj\bat\��Ǝ��U�V�X�e���^���[�U�ꗗ�\.LOG

select trim(loginid_u) as ���O�C���h�c,
PASSWORD_DATE_U as �p�X���[�h�ύX��,
decode(kengen_u,'0','���','�Ǘ���') as ����
from uidmast 
order by kengen_u desc,loginid_u;
SPOOL OFF

QUIT;
