using System;

public class AuditService
{
    public async Task LogActionAsync(string username, string action, string details)
    {
        var utcNow = DateTime.UtcNow;
        var centralZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        var centralTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, centralZone);

        var auditLog = new AuditLog
        {
            Username = username,
            Action = action,
            Details = details,
            Timestamp = centralTime
        };

        Console.WriteLine($"Audit Log: {auditLog.Timestamp} - {auditLog.Username} - {auditLog.Action} - {auditLog.Details}");
    }
}