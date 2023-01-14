EXECUTE sp_addlinkedserver @server = N'loopback', @srvproduct = N' ', @provider = N'SQLNCLI', @datasrc = N'localhost';


GO
EXECUTE sp_serveroption @server = N'loopback', @optname = N'rpc out', @optvalue = N'TRUE';


GO
EXECUTE sp_serveroption @server = N'loopback', @optname = N'remote proc transaction promotion', @optvalue = N'FALSE';


