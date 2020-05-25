# DefenderBypassPoC
Bypassing Windows Defender by changing one line of code

A simple Poc to bypass Windows Defender while extracting google chrome passwords. It looks like that Windows Defender uses a string based detection.

By renaming "\Google\Chrome\User Data\Default\Login Data" to

dim a = "Data"
"\Google\Chrome\User Data\Default\Login " + a

Windows Defender will not detect it :)

I included a compiled detected file and an undetected file.

