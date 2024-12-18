# ea-modelkit
An Enterprise Architect plugin that supports import and export of model data

## Installation
A MSI installer is provided on each release to ease the installation. 

This MSI have to be run with __Elevated Rights__. 

## How To
### Generic Export

The generic export feature provides an customizable data export to Excel. It allows exports of the Name, Alias, Notes, all available TaggedValues and all Connectors information, for any kind of Element present on the current selection.

#### Steps

  1. Select, in the project browser, Elements to be exported. If a package is selected, all Elements contained into the package and sub-package(s) are also part of the selection. Multi-selection is also supported.
  2. On the __Publish__ entry of the Ribbon, click on the __EA ModelKit__ menu entry and select __Generic Export__
  3. A pop-up dialog window provides customization of the export. You can, for each kind of Element, select if Elements have to be exported and which kind of TaggedValue/Connector should be part of the export. ![image](https://github.com/user-attachments/assets/c9f62ee9-48ce-4d2f-a1cc-71a0efc2d936)

  4. Select the target file
  5. Press __Export__

