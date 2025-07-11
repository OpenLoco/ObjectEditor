// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/aspnetcore/blob/c9027726579d953496ce3c43546d1b69f77385a5/src/Identity/Core/src/EmailSender.cs
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace ObjectService.Identity;

public class EmailSender : IEmailSender
{
	readonly IConfiguration _configuration;

	public EmailSender(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		// Get SMTP settings from appsettings.json
		var smtpHost = _configuration["SmtpSettings:Host"];
		var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
		var smtpUsername = _configuration["SmtpSettings:Username"];
		var smtpPassword = _configuration["SmtpSettings:Password"];
		var senderEmail = _configuration["SmtpSettings:SenderEmail"];

		var client = new SmtpClient(smtpHost, smtpPort)
		{
			Credentials = new NetworkCredential(smtpUsername, smtpPassword),
			EnableSsl = true // Use SSL/TLS for secure communication
		};

		var mailMessage = new MailMessage
		{
			From = new MailAddress(senderEmail),
			Subject = subject,
			Body = htmlMessage,
			IsBodyHtml = true
		};
		mailMessage.To.Add(email);

		return client.SendMailAsync(mailMessage);
	}
}
