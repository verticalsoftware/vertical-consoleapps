namespace Vertical.ConsoleApplications.Commands
{
    internal static class TemplateBuilder
    {
        internal static string Combine(string? baseTemplate, string? template)
        {
            return !string.IsNullOrWhiteSpace(baseTemplate) && !string.IsNullOrWhiteSpace(template)
                ? $"{baseTemplate} {template}"
                : baseTemplate?.Trim() ?? template?.Trim() ?? string.Empty;
        }
    }
}