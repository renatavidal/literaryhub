using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones
{
    public interface IEmailService
    {
        void Send(string to, string subject, string htmlBody, string plainTextBody);
        void SendTemplate(string to, string subject, string templatePath, System.Collections.Generic.Dictionary<string, string> tokens);
    }
}
