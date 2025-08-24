using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Servicios
{
    public static class MailTemplate
    {
        public static string RenderTemplate(string template, Dictionary<string, string> tokens)
        {
            if (template == null) template = "";
            if (tokens == null) tokens = new Dictionary<string, string>();

            // 1) BLOQUES CONDICIONALES
            // CTA: se muestra si hay ButtonUrl (y opcionalmente ButtonText)
            bool hasCta = tokens.ContainsKey("ButtonUrl") && !string.IsNullOrEmpty(tokens["ButtonUrl"]);
            template = RenderOrRemoveBlock(template, "CTA", hasCta);

            // Secondary: si hay SecondaryHtml o SecondaryTitle
            bool hasSecondary = (tokens.ContainsKey("SecondaryHtml") && !string.IsNullOrEmpty(tokens["SecondaryHtml"])) ||
                                (tokens.ContainsKey("SecondaryTitle") && !string.IsNullOrEmpty(tokens["SecondaryTitle"]));
            template = RenderOrRemoveBlock(template, "Secondary", hasSecondary);

            // Social: si hay al menos una red
            bool hasSocial = (tokens.ContainsKey("Instagram") && !string.IsNullOrEmpty(tokens["Instagram"])) ||
                             (tokens.ContainsKey("Twitter") && !string.IsNullOrEmpty(tokens["Twitter"])) ||
                             (tokens.ContainsKey("Facebook") && !string.IsNullOrEmpty(tokens["Facebook"]));
            template = RenderOrRemoveBlock(template, "Social", hasSocial);

            // Prefs: si hay al menos una URL
            bool hasPrefs = (tokens.ContainsKey("ManagePrefsUrl") && !string.IsNullOrEmpty(tokens["ManagePrefsUrl"])) ||
                            (tokens.ContainsKey("UnsubscribeUrl") && !string.IsNullOrEmpty(tokens["UnsubscribeUrl"]));
            template = RenderOrRemoveBlock(template, "Prefs", hasPrefs);

            // 2) REEMPLAZO SIMPLE DE TOKENS {{Key}}
            foreach (var kv in tokens)
            {
                string key = "{{" + kv.Key + "}}";
                template = template.Replace(key, kv.Value ?? "");
            }

            // 3) LIMPIEZA: remover cualquier {{Algo}} que haya quedado
            template = Regex.Replace(template, @"\{\{[A-Za-z0-9_]+\}\}", string.Empty);

            return template;
        }

        private static string RenderOrRemoveBlock(string template, string blockName, bool render)
        {
            // Coincide con {{#Block}} ... {{/Block}} (multilínea)
            var rx = new Regex(@"\{\{#" + blockName + @"\}\}([\s\S]*?)\{\{\/" + blockName + @"\}\}",
                               RegexOptions.IgnoreCase);
            if (render)
            {
                // Dejar solo el contenido
                return rx.Replace(template, "$1");
            }
            else
            {
                // Eliminar por completo el bloque (para que no quede el recuadro vacío)
                return rx.Replace(template, string.Empty);
            }
        }
    }
}