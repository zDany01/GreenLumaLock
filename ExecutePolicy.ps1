$exitcode = (Start-Process -FilePath "<GreenLumaLock full path>" -ArgumentList "noninteractive" -WorkingDirectory "<GreenLumaLock directory path>" -PassThru -Wait).ExitCode
switch ($exitcode) {
    15 {throw "Invalid GreenLuma state\nCheck status and lock file manually."}
    32 {exit}
    63 {throw "Internal Error\nCheck starter status" }
}