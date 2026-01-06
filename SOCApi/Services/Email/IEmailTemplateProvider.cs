namespace SOCApi.Services.Email
{
    /// <summary>
    /// Interface for providing email templates with variable substitution.
    /// </summary>
    public interface IEmailTemplateProvider
    {
        /// <summary>
        /// Gets and renders an email template with the provided variables.
        /// </summary>
        /// <param name="templateName">Name of the template file (without extension)</param>
        /// <param name="variables">Dictionary of variables to substitute in the template</param>
        /// <returns>Rendered template content</returns>
        Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> variables);
    }
}
