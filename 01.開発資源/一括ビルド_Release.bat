@ECHO OFF
ECHO ���S�Ẵ\�����[�V�������r���h���A���s���֏㏑�����܂��B
PAUSE
SET PATH="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\"
IF EXIST build.log DEL build.log

ECHO �o�b�`�A�N���X���r���h���ł��D�D�D
devenv Batch/Batch.sln /Rebuild Release /out build.log
devenv Batch���U\Batch���U.sln /Rebuild Release /out build.log

ECHO RS��Ǝ��U_V2 �ǉ��@�\���r���h���ł��D�D�D
devenv Common/CLS/ClsAstExternal/CAstExternal.sln /Rebuild Release /out build.log
devenv Common/CLS/ClsAstExternalPrint/CAstExternalPrint.sln /Rebuild Release /out build.log

ECHO �g��������r���h���ł��D�D�D
devenv Common/ExtendPrint/ExtendPrint.sln /Rebuild Release /out build.log

COPY DLL\*.dll EXE\ /Y >> build.log

ECHO �����U�ւ��r���h���ł��D�D�D
devenv ���U�Ǘ�\JIFURI\JIFURI.sln /Rebuild Release /out build.log

ECHO �������ς��r���h���ł��D�D�D
devenv ��������\KESSAI.sln /Rebuild Release /out build.log

ECHO �^�p�Ǘ����r���h���ł��D�D�D
devenv JOB\�W���u�Ď�.sln /Rebuild Release /out build.log
devenv ���C��\���C��.sln /Rebuild Release /out build.log
devenv �^�p�Ǘ�\Management.sln /Rebuild Release /out build.log

ECHO �����U�����r���h���ł��D�D�D
devenv �����U�Ǘ�\SOUFURI.sln /Rebuild Release /out build.log

ECHO �w�Z�������r���h���ł��D�D�D
devenv �w�Z���U\GAKKOU\GAKKOU.sln /Rebuild Release /out build.log

ECHO �}�̕ϊ����r���h���ł��D�D�D
devenv �}�̕ϊ�\�}�̕ϊ�.sln /Rebuild Release /out build.log

ECHO WEB�`�����r���h���ł��D�D�D
devenv WEB�`��\WEB_DENSO.sln /Rebuild Release /out build.log

ECHO �W����s���r���h���ł��D�D�D
devenv �W����s\SSS.sln /Rebuild Release /out build.log

ECHO �ʏ������r���h���ł��D�D�D
devenv �ʏ���\DelDB\DelDB.sln /Rebuild Release /out build.log
devenv �ʏ���\DelFile\DelFile.sln /Rebuild Release /out build.log
devenv �ʏ���\�`���A�g\�`���A�g.sln /Rebuild Release /out build.log

ECHO ���[���(�����U��)���r���h���ł��D�D�D
devenv ���[���\��Ǝ��U\KFJP001.�������ʊm�F�\(����)\KFJP001_�������ʊm�F�\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP002.�������ʊm�F�\(�Z���^�[���ڎ���)\KFJP002_�������ʊm�F�\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP003.��t���ו\\KFJP003_��t���ו\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP006.���s���ו\\KFJP006.���s���ו\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP007.�����U�֐����f�[�^���t�[\KFJP007.�����U�֐����f�[�^���t�[.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP008.�������ʊm�F�\(�z�M�f�[�^�쐬)\KFJP008.�������ʊm�F�\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP009.�Z���^�[�J�b�g�f�[�^�쐬�Ώۈꗗ\KFJP009.�Z���^�[�J�b�g�f�[�^�쐬�Ώۈꗗ.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP010.�����U�֖��ו\\KFJP010.�����U�֖��ו\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP012.�a�������U�֕ύX�ʒm��\�a�������U�֕ύX�ʒm��.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP013.�������ʊm�F�\(�s�\���ʍX�V)\KFJP013_�������ʊm�F�\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP013_2.�������ʊm�F�\(�s�\���ʍX�V�E��Ǝ���)\KFJP013_�������ʊm�F�\(�s�\���ʍX�V�E��Ǝ���).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP014.�X�P�W���[���G���[���X�g\�X�P�W���[���G���[���X�g.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP015.�����}�X�^�G���[���X�g\�����}�X�^�G���[���X�g.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP016.�Ԋ҃f�[�^�쐬�ꗗ\KFJP016.�Ԋ҃f�[�^�쐬�ꗗ.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP017.�U�֕s�\���ו\\KFJP017.�U�֕s�\���ו\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP018.�U�֌��ʖ��ו\\KFJP018.�U�֌��ʖ��ו\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP019.�����U�֓X�ʏW�v�\\KFJP019.�����U�֓X�ʏW�v�\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP020.�������ʊm�F�\(�Ԋ҃f�[�^�쐬)\KFJP020_�������ʊm�F�\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP021.�������ʊm�F�\(�ĐU�f�[�^�쐬)\KFJP021_�������ʊm�F�\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP022.�����}�X�^�����e�i�o�^�j\�����}�X�^�����e(�o�^).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP023.�����}�X�^�����e�i�ύX�j\�����}�X�^�����e(�ύX).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP024.�����}�X�^�����e�i�폜�j\�����}�X�^�����e(�폜).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP025.���s�}�X�^�ꗗ�\\KFJP025.���s�}�X�^�ꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP026.�������ʊm�F�\(�������)\KFJP026.�������ʊm�F�\(�������).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP027.�X�P�W���[���i���Ǘ��\\KFJP027.�X�P�W���[���i���Ǘ��\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP028.�����U�ֈ˗���\�����U�ֈ˗���.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP029_30.�����U�֓��̓`�F�b�N���X�g\�����U�֓��̓`�F�b�N���X�g.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP031.�_��҈ꗗ�\\�_��҈ꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP032.�����}�X�^�`�F�b�N���X�g\�����}�X�^�`�F�b�N���X�g.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP033_34.�X�P�W���[���\\�X�P�W���[���\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP035.�����}�X�^������\�����}�X�^������.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP036.�U�֌��ʕύX�`�F�b�N���X�g\�U�֌��ʕύX�`�F�b�N���X�g.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP037.�������ꗗ�\(����)\�������ꗗ�\(����).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP038.�����U�փf�[�^���M�񍐏�\�����U�փf�[�^���M�񍐏�.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP039.�N�����l�ʐU�����ו\\KFJP039.�N�����l�ʐU�����ו\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP040.�N���U���x�X�R�[�h�`�F�b�N���X�g\KFJP040.�N���U���x�X�R�[�h�`�F�b�N���X�g.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP041.�����`�F�b�N�m�F�\(������t��)\KFJP041.�����`�F�b�N�m�F�\(������t��).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP042.�����`�F�b�N�m�F�\(�˗���)\KFJP042.�����`�F�b�N�m�F�\(�˗���).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP043.�������ʊm�F�\(���U�_��)\KFJP043.�������ʊm�F�\(���U�_��).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP044.�������ʊm�F�\(���U�_�񌋉�)\KFJP044.�������ʊm�F�\(���U�_�񌋉�).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP045.�̎��؏�\KFJP045.�̎��؏�.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP048.�̎��T\KFJP048.�̎��T.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP049.�����U�֗p�[�t�����t��\KFJP049.�����U�֗p�[�t�����t��.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP050.�����U�֏������ʌ����\\KFJP050.�����U�֏������ʌ����\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP051_�n��`���f�[�^���M�A���[\KFJP051_�n��`���f�[�^���M�A���[.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP052.�U�֕s�\���R�ʏW�v�\\KFJP052.�U�֕s�\���R�ʏW�v�\.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP053.���U�Ǘ����X�g\���U�Ǘ����X�g.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP054.�������ʊm�F�\(�����ꗗ)\KFJP054_�������ʊm�F�\(�����ꗗ).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP055.�������ʊm�F�\(�Ԋ҈ꗗ)\KFJP055_�������ʊm�F�\(�Ԋ҈ꗗ).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP056.�U���萔���}�X�^�o�^���X�g\�U���萔���}�X�^�o�^���X�g.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP057.�萔�������t���O�ꊇ�X�V�ꗗ\KFJP057.�萔�������t���O�ꊇ�X�V�ꗗ.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP058.�f�[�^�`���ʒm��\�f�[�^�`���ʒm��.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP059.�����}�X�^�����e(�o�^)\�����}�X�^�����e(�o�^).sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP060.�ĐU�Ώې�`�F�b�N���X�g\�ĐU�Ώې�`�F�b�N���X�g.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP062.�����}�X�^���ڊm�F�[\KFJP062.�����}�X�^���ڊm�F�[.sln /Rebuild Release /out build.log
devenv ���[���\��Ǝ��U\KFJP063.�a�������U�֕ύX�ʒm��\KFJP063.�a�������U�֕ύX�ʒm��.sln /Rebuild Release /out build.log

ECHO ���[���(��������)���r���h���ł��D�D�D
devenv ���[���\��������\KFKP001.�������ʊm�F�\(��������)\�������ʊm�F�\(��������).sln /Rebuild Release /out build.log
devenv ���[���\��������\KFKP002.�������ʊm�F�\(�������ώ��)\�������ʊm�F�\(�������ώ��).sln /Rebuild Release /out build.log
devenv ���[���\��������\KFKP003.�������ʊm�F�\(�������ό���)\�������ʊm�F�\(�������ό���).sln /Rebuild Release /out build.log
devenv ���[���\��������\KFKP004.���U������ƈꗗ�\\���U������ƈꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\��������\KFKP005_06.�������ϊ�ƈꗗ�\\�������ϊ�ƈꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\��������\KFKP007.�萔����������ƈꗗ�\\�萔����������ƈꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\��������\KFKP008.�萔���ꊇ�������ו\\�萔���ꊇ�������ו\.sln /Rebuild Release /out build.log
devenv ���[���\��������\KFKP009.�a�������U�֓���[�萔��������\�a�������U�֓���[�萔��������.sln /Rebuild Release /out build.log

ECHO ���[���(�^�p�Ǘ�)���r���h���ł��D�D�D
devenv ���[���\�^�p�Ǘ�\KFUP001.�f�[�^�`�����O�ꗗ\KFUP001.�f�[�^�`�����O�ꗗ.sln /Rebuild Release /out build.log
devenv ���[���\�^�p�Ǘ�\KFUP002.�W���u�Ď��󋵈ꗗ�\\�W���u�Ď��󋵊m�F�ꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\�^�p�Ǘ�\KFUP003.�o�^���[�U�ꗗ�\\�o�^���[�U�ꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\�^�p�Ǘ�\KFUP004.�x�����ꗗ�\\�x�����ꗗ�\.sln /Rebuild Release /out build.log

ECHO ���[���(�����U��)���r���h���ł��D�D�D
devenv ���[���\�����U��\KFSP001.�������ʊm�F�\(����)\KFSP001_�������ʊm�F�\.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP009.�U�����M���G���^�Ώۈꗗ\�U�����M���G���^�Ώۈꗗ.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP010.�ב֐U�����ו\(�{�x�X�ב�)\�ב֐U�����ו\.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP011.�ב֐U�����ו\(���s�ב�)\�ב֐U�����ו\.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP012.�ב֐U�����ו\(���U���M���O�o�^)\�ב֐U�����ו\.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP013.�����U�����ו\\�����U�����ו\.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP017.�����}�X�^�����e�i�o�^�j\�����}�X�^�����e(�o�^).sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP018.�����}�X�^�����e�i�ύX�j\�����}�X�^�����e(�ύX).sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP019.�����}�X�^�����e�i�폜�j\�����}�X�^�����e(�폜).sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP020.�������ʊm�F�\(�������)\KFSP020.�������ʊm�F�\(�������).sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP021.�X�P�W���[���i���Ǘ��\\KFSP021.�X�P�W���[���i���Ǘ��\.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP022.�����U���˗���\�����U���˗���.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP023_24.�����U�����̓`�F�b�N���X�g\�����U�����̓`�F�b�N���X�g.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP025.�_��҈ꗗ�\\�_��҈ꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP026.�����}�X�^�`�F�b�N���X�g\�����}�X�^�`�F�b�N���X�g.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP027_28.�X�P�W���[���\\�X�P�W���[���\.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP029.�����}�X�^������\�����}�X�^������.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP031.�����}�X�^�����e(�o�^)\�����}�X�^�����e(�o�^).sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP032.�������ʊm�F�\(CSV���G���^)\�������ʊm�F�\(CSV���G���^).sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP033.�����U�����ו\(CSV���G���^)\�����U�����ו\(CSV���G���^).sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP034.FB�^���L���O�f�[�^�U���X�ʏW�v�\\FB�^���L���O�f�[�^�U���X�ʏW�v�\.sln /Rebuild Release /out build.log
devenv ���[���\�����U��\KFSP036.�����}�X�^���ڊm�F�[\KFSP036.�����}�X�^���ڊm�F�[.sln /Rebuild Release /out build.log

ECHO ���[���(�w�Z�����)���r���h���ł��D�D�D
devenv ���[���\�w�Z�����\KFGP001.�C���v�b�g�G���[���X�g\�C���v�b�g�G���[���X�g.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP003_4.�����U�֗\�薾�ו\\�����U�֗\�薾�ו\.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP005.�����U�֗\��W�v�\\�����U�֗\��W�v�\.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP006.�w�Z�}�X�^�����e�i�o�^�j\�w�Z�}�X�^�����e�i�o�^�j.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP007.�w�Z�}�X�^�����e�i�X�V�j\�w�Z�}�X�^�����e�i�X�V�j.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP008.�w�Z�}�X�^�����e�i�폜�j\�w�Z�}�X�^�����e�i�폜�j.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP009.�w�Z���s�}�X�^�ꗗ\�w�Z���s�}�X�^�ꗗ.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP010.��ڃ}�X�^�ꗗ\��ڃ}�X�^�ꗗ.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP011.�i�����X�g\�i�����X�g.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP012.���ԃX�P�W���[���\\���ԃX�P�W���[���\.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP013.�����U�֗\��ꗗ�\\�����U�֗\��ꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP014_15.�����U�֗\��ꗗ�\(�����f�[�^)\�����U�֗\��ꗗ�\(�����f�[�^).sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP016_17.�����U�֌��ʈꗗ�\\�����U�֌��ʈꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP018.���[�񍐏�\���[�񍐏�.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP019.�����U�֓X�ʏW�v�\\�����U�֓X�ʏW�v�\.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP020.�����U�֖��[�̂��m�点\�����U�֖��[�̂��m�点.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP021.���ʗa�������`�[\���ʗa�������`�[.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP022.�������X�g\�������X�g.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP023.���[�󋵈ꗗ�\\���[�󋵈ꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP024.���[�󋵈ꗗ�\(��ڕʍ��v)\���[�󋵈ꗗ�\(��ڕʍ��v).sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP025.�w�Z�}�X�^������\�w�Z�}�X�^������.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP026.�w�Z���k����\�w�Z���k����.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP027_28.���k�}�X�^�o�^�`�F�b�N���X�g\���k�}�X�^�o�^�`�F�b�N���X�g.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP029.���q�`�F�b�N���X�g���ו\\���q�`�F�b�N���X�g���ו\.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP030.���k���ד��̓`�F�b�N���X�g\���k���ד��̓`�F�b�N���X�g.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP031.�w�Z���U�����`�F�b�N���X�g\�w�Z���U�����`�F�b�N���X�g.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP032.�����U�ֈ˗���\�����U�ֈ˗���.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP033.���k�}�X�^�Ǘ��\\���k�}�X�^�Ǘ��\.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP035.�w�Z�}�X�^�����e�i�o�^�j\�w�Z�}�X�^�����e�i�o�^�j.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP036.���k�}�X�^�������`�F�b�N���X�g\���k�}�X�^�������`�F�b�N���X�g.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP037.�N�ԃX�P�W���[���\\�N�ԃX�P�W���[���\.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP038.�w�Z�}�X�^���ڊm�F�[\KFGP038.�w�Z�}�X�^���ڊm�F�[.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP039.���k�o�^���ꗗ\���k�o�^���ꗗ.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP106.���k�}�X�^�����e�i�o�^�j\���k�}�X�^�����e�i�o�^�j.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP107.���k�}�X�^�����e�i�X�V�j\���k�}�X�^�����e�i�X�V�j.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP108.���k�}�X�^�����e�i�폜�j\���k�}�X�^�����e�i�폜�j.sln /Rebuild Release /out build.log
devenv ���[���\�w�Z�����\KFGP501.���k�f�[�^�捞�o�^���X�g\KFGP501_���k�f�[�^�捞�o�^���X�g.sln /Rebuild Release /out build.log

ECHO ���[���(WEB�`��)���r���h���ł��D�D�D
devenv ���[���\WEB�`��\KFWP001.WEB�`�����O�ꗗ\WEB�`�����O�ꗗ.sln /Rebuild Release /out build.log

ECHO ���[���(�W����s)���r���h���ł��D�D�D
devenv ���[���\�W����s\KF3SP001.SSS�������ʍ��v�\\SSS�������ʍ��v�\.sln /Rebuild Release /out build.log
devenv ���[���\�W����s\KF3SP002.SSS�ϑ��ҕʌ��ϊz�ꗗ�\\SSS�ϑ��ҕʌ��ϊz�ꗗ�\.sln /Rebuild Release /out build.log
devenv ���[���\�W����s\KF3SP003.SSS��s�ʐU�֌��ʍ��v�\\SSS��s�ʐU�֌��ʍ��v�\.sln /Rebuild Release /out build.log
devenv ���[���\�W����s\KF3SP004.SSS��ƕʌ��ʕ\\SSS��ƕʌ��ʕ\.sln /Rebuild Release /out build.log

ECHO RS��Ǝ��U_V2 �ǉ��@�\���r���h���ł��D�D�D
devenv Common/CLS/ClsAstExternal/CAstExternal.sln /Rebuild Release /out build.log
devenv Common/CLS/ClsAstExternalPrint/CAstExternalPrint.sln /Rebuild Release /out build.log

ECHO �s�v�t�@�C�����폜���ł��E�E�E
FOR /F %%A IN ('DIR /B /S /A:D') DO (RMDIR /S /Q "%%A\obj" & RMDIR /S /Q "%%A\bin")
DEL  /S /Q *.pdb
DEL  /S /Q *.xml
DEL  /S /Q *.vshost.exe

