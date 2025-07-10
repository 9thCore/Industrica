set mod=Industrica
set modpath=%SUBNAUTICA_PATH%\BepInEx\plugins\%mod%
mkdir "%modpath%"
copy "bin\Release\net472\%mod%.dll" "%modpath%\%mod%.dll"
copy "bin\Release\net472\%mod%.pdb" "%modpath%\%mod%.pdb"
robocopy "Localization" "%modpath%\Localization" /E
robocopy "Assets" "%modpath%\Assets" /E