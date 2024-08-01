IF NOT EXISTS(SELECT 1 FROM dbo.AQTSecurable WHERE SecurableName = 'ActiveReportsPage')
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
 
        INSERT INTO dbo.AQTSecurable(SecurableName, SecurableDescription)
        VALUES ('ActiveReportsPage', 'View the list of active reports');
 
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO
 
IF NOT EXISTS(SELECT 1 FROM dbo.AQTSecurable WHERE SecurableName = 'ViewReportData')
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
 
        INSERT INTO dbo.AQTSecurable(SecurableName, SecurableDescription)
        VALUES ('ViewReportData', 'View and download the data for a specific report');
 
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO
 
IF NOT EXISTS(SELECT 1 FROM dbo.AQTSecurable WHERE SecurableName = 'ReportAdminPage')
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
 
        INSERT INTO dbo.AQTSecurable(SecurableName, SecurableDescription)
        VALUES ('ReportAdminPage', 'View all active and disabled reports. Create and Modify reports.');
 
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO