@echo off

echo csv Generierung aus .json

Rem AppART
IssueColl.exe DATA\AppArt.json Export\CSV\AppArt workflow\workflow_Skyway.txt

Rem MyPorsche ART
rem IssueColl.exe "\DATA\PCOM.json" Export\PCOM "\workflow\workflow_PCOM.txt"


Rem MSF
IssueColl.exe DATA\msf.json Export\CSV\msf workflow\workflow_msf.txt


Rem AppART
IssueColl.exe "\DATA\STID.json" Export\CSV\STID "\workflow\workflow_STID.txt"

Rem Porsche.com
IssueColl.exe "\DATA\PCOM.json" Export\CSV\PCOM "\workflow\workflow_PCOM.txt"

Rem ProPo Porfolio
IssueColl.exe "\DATA\ProPo.json" Export\CSV\ProPo "\workflow\workflow_MyPorsche.txt"




REM MyPorsche ART
IssueColl.exe "\DATA\PCOM.json" Export\PCOM "\workflow\workflow_PCOM.txt"
R CMD BATCH "H:\Repositorys\toolstack\Win Portable\RScripts\PCOM\BuildCharts_ART.R" 

Rem MyPorsche ART
IssueColl.exe "\DATA\PCOM.json" Export\PCOM "\workflow\workflow_PCOM.txt"
R CMD BATCH "H:\Repositorys\toolstack\Win Portable\RScripts\PCOM\BuildCharts_ART.R" 

Rem MyPorsche ART
IssueColl.exe "\DATA\PCOM.json" Export\PCOM "\workflow\workflow_PCOM.txt"
R CMD BATCH "H:\Repositorys\toolstack\Win Portable\RScripts\PCOM\BuildCharts_ART.R" 

Rem MyPorsche ART
IssueColl.exe "\DATA\PCOM.json" Export\PCOM "\workflow\workflow_PCOM.txt"
R CMD BATCH "H:\Repositorys\toolstack\Win Portable\RScripts\PCOM\BuildCharts_ART.R" 