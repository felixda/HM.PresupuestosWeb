# 02-update-tfm: Actualizar TargetFramework en todos los proyectos

Cambiar TargetFramework de net8.0 a net10.0 en los seis .csproj: Domain, Application, Infrastructure, Web, UnitTest, E2ETest.

**Done when**: Los seis proyectos referencian net10.0; dotnet restore sin errores.
