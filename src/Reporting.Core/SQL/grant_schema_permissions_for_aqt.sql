-- Grant Permissions
BEGIN TRY
    BEGIN TRANSACTION;

    GRANT UPDATE ON SCHEMA::[Reporting] TO [aqt_users];
    GRANT SELECT ON SCHEMA::[Reporting] TO [aqt_users];
    GRANT INSERT ON SCHEMA::[Reporting] TO [aqt_users];
    GRANT EXECUTE ON SCHEMA::[Reporting] TO [aqt_users];
    GRANT DELETE ON SCHEMA::[Reporting] TO [aqt_users];
    GRANT ALTER ON SCHEMA::[Reporting] TO [aqt_users];

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO