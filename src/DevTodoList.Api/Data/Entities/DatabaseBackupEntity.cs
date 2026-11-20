using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

[Table("DatabaseBackups")]
public class DatabaseBackupEntity : EntityBase
{
    [Required, StringLength(500)]
    public string FileName { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    /// <summary>Manual / Auto</summary>
    [Required, StringLength(20)]
    public string BackupType { get; set; } = "Manual";

    [StringLength(500)]
    public string? Description { get; set; }

    [Required, StringLength(1000)]
    public string BackupPath { get; set; } = string.Empty;
}
