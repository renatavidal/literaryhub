using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Abstracciones;

namespace Servicios
{
    public class SmtpEmailService : IEmailService
    {
        public void Send(string to, string subject, string htmlBody, string plainTextBody)
        {
            using (var message = new MailMessage())
            {
                // Usa <system.net><mailSettings> del web.config
                // El From se toma de mailSettings/smtp@from si no lo seteás.
                message.To.Add(new MailAddress(to));
                message.Subject = subject;

                // Alternativas: texto plano + HTML
                var plainView = AlternateView.CreateAlternateViewFromString(
                    plainTextBody ?? StripHtml(htmlBody),
                    Encoding.UTF8,
                    "text/plain");

                var htmlView = AlternateView.CreateAlternateViewFromString(
                    htmlBody ?? "",
                    Encoding.UTF8,
                    "text/html");

                message.AlternateViews.Add(plainView);
                message.AlternateViews.Add(htmlView);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    smtp.Send(message);
                }
            }
        }

        public void SendTemplate(string to, string subject, string templatePath, Dictionary<string, string> tokens)
        {
            string html = File.ReadAllText(templatePath, Encoding.UTF8);
            html = MailTemplate.RenderTemplate(html, tokens);

            string plain = BuildPlainFromTokens(tokens);

            Send(to, subject, html, plain);
        }

        private static string RenderTemplate(string template, Dictionary<string, string> tokens)
        {
            if (tokens == null) tokens = new Dictionary<string, string>();

            // Manejar bloque CTA {{#CTA}}...{{/CTA}}: eliminar si no hay ButtonUrl o ButtonText
            bool hasCta = tokens.ContainsKey("ButtonUrl") && !string.IsNullOrEmpty(tokens["ButtonUrl"]);
            var ctaRegex = new Regex(@"\{\{#CTA\}\}([\s\S]*?)\{\{\/CTA\}\}", RegexOptions.IgnoreCase);
            if (hasCta)
            {
                template = ctaRegex.Replace(template, "$1");
            }
            else
            {
                template = ctaRegex.Replace(template, string.Empty);
            }

            // Reemplazo simple de {{Token}}
            foreach (var kv in tokens)
            {
                string key = "{{" + kv.Key + "}}";
                template = template.Replace(key, kv.Value ?? "");
            }

            // Quitar tokens que queden sin reemplazar
            template = Regex.Replace(template, @"\{\{[A-Za-z0-9_]+\}\}", string.Empty);

            return template;
        }

        private static string BuildPlainFromTokens(Dictionary<string, string> tokens)
        {
            var sb = new StringBuilder();
            string title = tokens.ContainsKey("Title") ? tokens["Title"] : "";
            string body = tokens.ContainsKey("BodyHtml") ? StripHtml(tokens["BodyHtml"]) : "";
            sb.AppendLine(title);
            sb.AppendLine();
            sb.AppendLine(body);

            if (tokens.ContainsKey("ButtonUrl") && !string.IsNullOrEmpty(tokens["ButtonUrl"]))
            {
                sb.AppendLine();
                sb.AppendLine("Enlace: " + tokens["ButtonUrl"]);
            }

            string footer = tokens.ContainsKey("FooterText") ? tokens["FooterText"] : "";
            if (!string.IsNullOrEmpty(footer))
            {
                sb.AppendLine();
                sb.AppendLine(footer);
            }

            return sb.ToString();
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return "";
            // Quitar etiquetas simples
            string text = Regex.Replace(html, "<[^>]+>", " ");
            // Normalizar espacios
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text;
        }
    }
}
