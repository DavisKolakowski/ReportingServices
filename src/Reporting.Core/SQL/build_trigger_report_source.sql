CREATE OR ALTER TRIGGER [Trigger_ReportSource]
ON DATABASE
FOR CREATE_PROCEDURE, ALTER_PROCEDURE, DROP_PROCEDURE, 
    CREATE_VIEW, ALTER_VIEW, DROP_VIEW
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @EventType NVARCHAR(128),
        @SchemaName NVARCHAR(128),
        @SourceName NVARCHAR(128),
        @SourceType NVARCHAR(32),
        @SourceId INT,
        @HasParameters BIT,
        @CreatedAt DATETIME = GETDATE(),
        @CreatedBy NVARCHAR(128) = SUSER_NAME(),
        @MappedActivityType NVARCHAR(50);

    SELECT 
        @EventType = EVENTDATA().value('(/EVENT_INSTANCE/EventType)[1]', 'NVARCHAR(128)'),
        @SchemaName = EVENTDATA().value('(/EVENT_INSTANCE/SchemaName)[1]', 'NVARCHAR(128)'),
        @SourceName = EVENTDATA().value('(/EVENT_INSTANCE/ObjectName)[1]', 'NVARCHAR(128)');

    IF @SchemaName = 'Reporting'
    BEGIN
        SET @SourceType = 
            CASE 
                WHEN @EventType IN ('CREATE_PROCEDURE', 'ALTER_PROCEDURE') THEN 'Procedure'
                WHEN @EventType IN ('CREATE_VIEW', 'ALTER_VIEW') THEN 'View'
                ELSE NULL
            END;

        SET @MappedActivityType = 
            CASE 
                WHEN @EventType IN ('CREATE_PROCEDURE', 'CREATE_VIEW') THEN 'Created'
                WHEN @EventType IN ('ALTER_PROCEDURE', 'ALTER_VIEW') THEN 'Updated'
                WHEN @EventType IN ('DROP_PROCEDURE', 'DROP_VIEW') THEN 'Deleted'
                ELSE NULL
            END;

        DECLARE @IsValid BIT = 1;

        IF @SourceType = 'Procedure'
        BEGIN
            DECLARE @ProcedureName NVARCHAR(128) = @SchemaName + '.' + @SourceName;
            IF EXISTS (
                SELECT 1
                FROM sys.dm_exec_describe_first_result_set_for_object(OBJECT_ID(@ProcedureName), NULL)
                WHERE name = 'InternalId'
            )
            BEGIN
                PRINT 'Procedure ' + @ProcedureName + ' is valid for reporting.';
            END
            ELSE
            BEGIN
                SET @IsValid = 0;
                PRINT 'Procedure ' + @ProcedureName + ' does not meet the reporting rules.';
            END;
        END
        ELSE IF @SourceType = 'View'
        BEGIN
            DECLARE @ViewName NVARCHAR(128) = @SchemaName + '.' + @SourceName;
            IF EXISTS (
                SELECT 1
                FROM sys.columns c
                JOIN sys.objects o ON c.object_id = o.object_id
                WHERE o.schema_id = SCHEMA_ID(@SchemaName)
                  AND o.name = @SourceName
                  AND c.name = 'InternalId'
            )
            BEGIN
                PRINT 'View ' + @ViewName + ' is valid for reporting.';
            END
            ELSE
            BEGIN
                SET @IsValid = 0;
                PRINT 'View ' + @ViewName + ' does not meet the reporting rules.';
            END;
        END;

        IF @IsValid = 1
        BEGIN
            IF @EventType IN ('CREATE_PROCEDURE', 'CREATE_VIEW', 'ALTER_PROCEDURE', 'ALTER_VIEW')
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM [Reporting].[ReportSources]
                    WHERE [Schema] = @SchemaName AND [Name] = @SourceName
                )
                BEGIN
                    UPDATE [Reporting].[ReportSources]
                    SET
                        [Type] = @SourceType,
                        [LastActivityType] = @MappedActivityType,
                        [LastActivityByUser] = @CreatedBy,
                        [LastActivityDate] = @CreatedAt
                    WHERE [Schema] = @SchemaName AND [Name] = @SourceName;

                    PRINT 'Updated ' + @SourceType + ' ' + @SourceName + ' in the reports.';
                END
                ELSE
                BEGIN
                    INSERT INTO [Reporting].[ReportSources] (
                        [Type], [Schema], [Name], [LastActivityType], [LastActivityByUser], [LastActivityDate]
                    )
                    VALUES (
                        @SourceType, @SchemaName, @SourceName, @MappedActivityType, @CreatedBy, @CreatedAt
                    );

                    PRINT 'Added ' + @SourceType + ' ' + @SourceName + ' to the reports.';
                END;
            END

            INSERT INTO [Reporting].[ReportSourceHistory] (
                [Type], [SqlObjectName], [ActivityType], [ActivityByUser], [ActivityDate]
            )
            VALUES (
                @SourceType, @SchemaName + '.' + @SourceName, @MappedActivityType, @CreatedBy, @CreatedAt
            );
        END
        ELSE
        BEGIN
            SET @SourceId = (
                SELECT [Id]
                FROM [Reporting].[ReportSources]
                WHERE [Schema] = @SchemaName AND [Name] = @SourceName
            );

            IF EXISTS (
                SELECT 1
                FROM [Reporting].[Reports]
                WHERE [ReportSourceId] = @SourceId
            )
            BEGIN
                DELETE FROM [Reporting].[Reports]
                WHERE [ReportSourceId] = @SourceId;
            END;

            DELETE FROM [Reporting].[ReportSources]
            WHERE [Id] = @SourceId;

            PRINT 'Removed invalid ' + @SourceType + ' ' + @SourceName + ' from the reports.';
        END;

        IF @EventType IN ('DROP_PROCEDURE', 'DROP_VIEW')
        BEGIN
            SET @SourceId = (
                SELECT [Id]
                FROM [Reporting].[ReportSources]
                WHERE [Schema] = @SchemaName AND [Name] = @SourceName
            );

            IF EXISTS (
                SELECT 1
                FROM [Reporting].[Reports]
                WHERE [ReportSourceId] = @SourceId
            )
            BEGIN
                DELETE FROM [Reporting].[Reports]
                WHERE [ReportSourceId] = @SourceId;
            END;

            DELETE FROM [Reporting].[ReportSources]
            WHERE [Id] = @SourceId;

            INSERT INTO [Reporting].[ReportSourceHistory] (
                [Type], [SqlObjectName], [ActivityType], [ActivityByUser], [ActivityDate]
            )
            VALUES (
                @SourceType, @SchemaName + '.' + @SourceName, 'Deleted', @CreatedBy, @CreatedAt
            );

            PRINT 'Deleted ' + @SourceType + ' ' + @SourceName + ' from the reports.';
        END;
    END
END;
GO
