set CURRENT_DIR=%CD%
cd APP_BIN
mono Node.Cs.Cmd.exe -config "%CURRENT_DIR%\node.config" 2> error.log
