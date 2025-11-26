$GreenLumaLock = New-Object System.Diagnostics.ProcessStartInfo
$GreenLumaLock.FileName = "<GreenLumaLock full path>"
$GreenLumaLock.WorkingDirectory = "<GreenLumaLock dir path>"
$GreenLumaLock.UseShellExecute = $true
$GreenLumaLock.Arguments = "noninteractive"
$p = New-Object System.Diagnostics.Process
$p.StartInfo = $GreenLumaLock
$p.Start()
$p.WaitForExit()    

switch ($p.ExitCode) {
    15 {throw "Invalid GreenLuma state! Check status and lock file manually."}
    32 {exit}
    63 {throw "Internal Error! Check starter status" }
}
