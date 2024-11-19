// DevTool 1.1 
// Copyright (C) 2024, Adaruru

/// <summary>
/// 
/// </summary>
public class Schema
{
    /// <summary>
    /// 
    /// </summary>
    public string SchemaId { get; set; } = "";
    public string SchemaName { get; set; } = "";
    public List<Table>? Tables { get; set; } = new List<Table>();
}

public class Table
{
    public List<Column> Columns { get; set; } = new List<Column>();
    public string TableDescription { get; set; } = "";
    public string TableName { get; set; } = "";
}

public class Column
{
    public string ColumnDescription { get; set; } = "";
    public string ColumnName { get; set; } = "";
    public string DataType { get; set; } = "";
    public string DefaultValue { get; set; } = "";
    public string Identity { get; set; } = "";
    public string Length { get; set; } = "";
    public string NotNull { get; set; } = "";
    public string Precision { get; set; } = "";
    public string PrimaryKey { get; set; } = "";
    public string Scale { get; set; } = "";
    public string Sort { get; set; } = "";
}