using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Wokiwoki.Application.Common.Interfaces.Services;
using Wokiwoki.Application.Common.Models;

namespace Wokiwoki.Infrastructure.Services
{
	public class EmailService : IEmailService
	{
		private readonly IFluentEmail _fluentEmail;
		private readonly ILogger<EmailService> _logger;
		private const string VerifyCodeTemplate = "VerifyEmail.cshtml";

		public EmailService(IFluentEmail fluentEmail, ILogger<EmailService> logger)
		{
			_fluentEmail = fluentEmail;
			_logger = logger;
		}

		public async Task SendEmailVerificationCode(EmailVerificationRequest message)
		{
			if (string.IsNullOrWhiteSpace(message.To) || !IsValidEmail(message.To))
				throw new ArgumentException("Invalid recipient email.", nameof(message.To));
			
			if (string.IsNullOrWhiteSpace(message.Code))
				throw new ArgumentException("Verification code cannot be null or empty.", nameof(message.Code));

			var templatePath = Path.Combine(AppContext.BaseDirectory, "Data", "Templates", VerifyCodeTemplate);
			if (!File.Exists(templatePath))
			{
				_logger.LogError("Template file not found: {Path}", templatePath);
				throw new ArgumentException("Template file not found."); 
			}

			var templateData = new EmailVerificationResponse
			{
				Email = message.To,
				VerificationCode = message.Code,
			};

			var email = _fluentEmail
				.To(message.To)
				.Subject(message.Subject)
				.UsingTemplateFromFile(templatePath, templateData, isHtml: true);

			var result = await email.SendAsync();

			if (result.Successful)
			{
				_logger.LogInformation("Verify code email sent successfully "); 
			}
			else
			{
				_logger.LogError("Failed to send verify code to {Email}. Errors: {Errors}",
		   message.To,
		   result.ErrorMessages != null ? string.Join("; ", result.ErrorMessages) : "(no message)");
				throw new InvalidOperationException($"Failed to send email: {string.Join("; ", result.ErrorMessages ?? new[] { "unknown" })}");
			}
		}

		private bool IsValidEmail(string email)
		{
			if (string.IsNullOrEmpty(email))
				return false;

			try
			{
				var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
				return regex.IsMatch(email);
			}
			catch
			{
				return false;
			}
		}
	}
}
