# Simple AccessControl emulator

This repo contains emulator of AccessControl. This emulator is necessary to demonstrate [AccessControlAdapterSample](https://github.com/A-H-Software-House-Inc/AccessControlAdapterSample) work.

Emulator creates random doors, zones, partitions, outputs, users, and events. 

It also switch between doors, zones, partitions, and output states triggering state changes and events from the different emulated sources at a random moment.

## Run simple AccessControl emulator

Execute: `SimpleAccessControlEmulator.exe HttpPort`

AccessControl emulator runs on the localhost port **HttpPort**. 

If **HttpPort** isn't specified the default port **8095** will be used.