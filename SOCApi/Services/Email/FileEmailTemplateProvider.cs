namespace SOCApi.Services.Email
{
    /// <summary>
    /// File-based email template provider that loads templates from the filesystem.
    /// </summary>
    public class FileEmailTemplateProvider : IEmailTemplateProvider
    {
        private readonly ILogger<FileEmailTemplateProvider> _logger;
        private readonly IWebHostEnvironment _environment;
        private const string TemplatesDirectory = "Templates/Email";

        public FileEmailTemplateProvider(ILogger<FileEmailTemplateProvider> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// Gets and renders an email template by replacing placeholders with provided variables.
        /// Placeholders should be in the format {{VariableName}}
        /// </summary>
        public async Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> variables)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(templateName))
                {
                    throw new ArgumentException("Template name cannot be empty.", nameof(templateName));
                }

                var templatePath = Path.Combine(
                    _environment.ContentRootPath,
                    TemplatesDirectory,
                    $"{templateName}.html"
                );

                if (!File.Exists(templatePath))
                {
                    throw new FileNotFoundException($"Email template not found: {templateName}");
                }

                var template = await File.ReadAllTextAsync(templatePath);

                // Replace all variables in the template
                if (variables != null && variables.Count > 0)
                {
                    foreach (var variable in variables)
                    {
                        template = template.Replace(
                            $"{{{{{variable.Key}}}}}",
                            variable.Value ?? string.Empty,
                            StringComparison.Ordinal
                        );
                    }
                }

                _logger.LogDebug("Email template '{TemplateName}' loaded and rendered successfully.", templateName);
                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load email template '{TemplateName}'", templateName);
                throw new InvalidOperationException($"Failed to load email template '{templateName}'", ex);
            }
        }
    }
}
