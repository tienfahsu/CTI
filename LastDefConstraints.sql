select schema_name(t.schema_id) + '.' + t.[name] AS SCHNAME,
        'Table' AS TTYPE,
        'Default constraint' AS TYPENAME,
        con.[name],
        col.[name] + ' = ' + con.[definition] AS NAMEVALUE,
		'DF_'+t.[name]+ '_'+col.[name] AS SPNAME
		,'EXEC sp_rename N'''+con.[name]+''',N''DF_'+t.[name]+ '_'+col.[name]+'''' AS RENAMESP
    from sys.default_constraints con
        left outer join sys.objects t
            on con.parent_object_id = t.object_id
        left outer join sys.all_columns col
            on con.parent_column_id = col.column_id
            and con.parent_object_id = col.object_id
	WHERE con.[name] <> 'DF_'+t.[name]+ '_'+col.[name] AND t.[name] NOT LIKE 'XXX%'