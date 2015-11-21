
# Welcome #
This is a project aiming at creating a simple wrapper for the minecraft server so it can run as a windows service.
this has not a graphical server monitor and config panel yet but is is in development.

**_NEW! 1.8-pre support_**

## Installing the service ##
  1. Download the latest minecraft-service-_releasenumber_.zip (or build from source if you have visual studio 2010)
  1. You probably need InstallUtil so download installutil.zip
  1. extract everything in the same directory
  1. doubleclick server.jar (if windows asks for a program to open it with: c:/windows/system32/java.exe)
  1. now the normal server GUI should start. close the server
  1. open Ops.txt and enter your username
  1. Rightclick installServer.bat and choose "Run as Administrator"

now the minecraft server is installed.

to start the server run "services.msc" from the windows startmenu, find minecraftservice in the list and click start.