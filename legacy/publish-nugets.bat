@ECHO OFF

CALL install-nuget.bat Basics
ECHO ============================================================
CALL install-nuget.bat Basics.AspNet.WebApi
ECHO ============================================================
CALL install-nuget.bat Basics.Containers.Autofac
ECHO ============================================================
CALL install-nuget.bat Basics.Containers.ServiceLocator
ECHO ============================================================
CALL install-nuget.bat Basics.Data.Dapper
ECHO ============================================================
CALL install-nuget.bat Basics.Data.Dapper.SqlServer
ECHO ============================================================
CALL install-nuget.bat Basics.Domain
ECHO ============================================================
CALL install-nuget.bat Basics.Testing.Xunit
