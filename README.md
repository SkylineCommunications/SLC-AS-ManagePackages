# ManageInstallPackages

## About
This Automation script is an interactive automation script that can be triggered from other scripts to manage the packages on a Dataminer system.
All packages are located under the General Documents (C:\Skyline DataMiner\Documents\DMA_COMMON_DOCUMENTS\InstallPackages).

## Input Parameter

### Filter
The Filter input parameter allows you to filter down the packages that will be displayed in the overview screen. If you don't want to filter, you can set it to any text that is not a valid filter (e.g. N/A).

> ** Warning **<br>
Ensure you provide a valid semantic version format, otherwise the filter parsing will fail and all packages will be returned. When the filter parsing fails an information event will be generated with the exception.

An example of a filter that will show all packages starting with 'cassandra' and greater or equal than version 0.0.0:
```json
{"name":"cassandra*","version":"0.0.0"}
```
or
```json
{"arch":"x86_64","name":"reaper","system":"Debian"}
```