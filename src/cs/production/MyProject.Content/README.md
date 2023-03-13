# MyProject.Content

This C# project contains the _**code**_ to load or unload assets into the game.

To add more assets:
1. Copy the files to be added into the folder structure `assets/` at the root of the repository. Do this _**outside of VisualStudio or Rider**_. Using VisualStudio or Rider will muck up the `.csproj` file. The files will appear in VisualStudio or Rider automatically under `assets` linked folder using a glob pattern, see the `.csproj` file for more details. These linked files are purely for reference and sanity check. 
2. Add and or edit the C# code in this project to load or unload the assets from disk.