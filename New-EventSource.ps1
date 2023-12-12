        
        
        $PolicyModuleName = "TameMyCerts.WSTEP"
        If ([System.Diagnostics.EventLog]::SourceExists($PolicyModuleName) -eq $false) {
            [System.Diagnostics.EventLog]::CreateEventSource("$PolicyModuleName", 'Application')
        }