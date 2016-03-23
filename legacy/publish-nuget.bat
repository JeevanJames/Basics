@ECHO OFF

CALL install-nuget.bat Basics
CALL install-nuget.bat Basics.AspNet.WebApi
CALL install-nuget.bat Basics.Containers.Autofac
CALL install-nuget.bat Basics.Containers.ServiceLocator
CALL install-nuget.bat Basics.Data.Dapper
CALL install-nuget.bat Basics.Data.Dapper.SqlServer
CALL install-nuget.bat Basics.Testing.Xunit
