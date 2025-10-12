using System;
using System.Collections.Generic;
using System.Linq;

public static class PageDirectory
{
    public class PageItem
    {
        public string Title { get; set; }
        public string Url { get; set; }   
        public string[] Roles { get; set; }  // null o [] => pública; si tiene valores => requiere alguno
        public string[] Keywords { get; set; }
    }

    // ⚠ Ajustá Roles/Keywords según tu app (yo puse valores razonables).
    public static readonly List<PageItem> All = new List<PageItem>
    {
        // Públicas / generales
        new PageItem{ Title="Home",        Url="~/Home.aspx",        Roles=null, Keywords=new[]{"inicio"} },
        new PageItem{ Title="Landing",     Url="~/Landing.aspx",     Roles=null },
        new PageItem{ Title="About",       Url="~/About.aspx",       Roles=null, Keywords=new[]{"quienes somos"} },
        new PageItem{ Title="Privacy",     Url="~/Privacy.aspx",     Roles=null, Keywords=new[]{"privacidad"} },
        new PageItem{ Title="Terms",       Url="~/Terms.aspx",       Roles=null, Keywords=new[]{"términos"} },
        new PageItem{ Title="Security",    Url="~/Security.aspx",    Roles=null, Keywords=new[]{"seguridad"} },
        new PageItem{ Title="Help",        Url="~/Help.aspx",        Roles=null, Keywords=new[]{"ayuda","faq"} },
        new PageItem{ Title="Contact",     Url="~/Contact.aspx",     Roles=null, Keywords=new[]{"contacto"} },
        new PageItem{ Title="Login",       Url="~/Login.aspx",       Roles=null },
        new PageItem{ Title="Signup",      Url="~/Signup.aspx",      Roles=null, Keywords=new[]{"registrarse"} },
        new PageItem{ Title="Signup Cliente", Url="~/SignupCliente.aspx", Roles=null, Keywords=new[]{"registrarse cliente"} },

        // Navegación / contenido
        new PageItem{ Title="Suscripciones", Url="~/Suscripciones.aspx", Roles=null, Keywords=new[]{"planes"} },
        new PageItem{ Title="Newsletter",    Url="~/Newsletter.aspx",   Roles=null },

        // Requieren login (lectores/clientes/admins)
        new PageItem{ Title="Mi Cuenta",   Url="~/MyAccount.aspx",  Roles=new[]{"reader","cliente","admin"} },
        new PageItem{ Title="Mis Libros",  Url="~/MyBooks.aspx",    Roles=new[]{"reader","cliente","admin"} },
        new PageItem{ Title="Soporte (chat cliente)", Url="~/SupportChat.aspx", Roles=new[]{"reader","cliente","admin"}, Keywords=new[]{"soporte","chat"} },
        new PageItem{ Title="Logout",      Url="~/Logout.aspx",     Roles=new[]{"reader","cliente","admin"} },

        // Admin
        new PageItem{ Title="Admin Usuarios/Clientes", Url="~/AdministrarUsuarios.aspx", Roles=new[]{"admin"}, Keywords=new[]{"usuarios","clientes"} },
        new PageItem{ Title="Admin Finanzas",     Url="~/AdminFinanzas.aspx",     Roles=new[]{"admin","finance"}, Keywords=new[]{"cuentas","notas crédito/débito"} },
        new PageItem{ Title="Admin Permisos/Roles", Url="~/AdminPermisos.aspx",   Roles=new[]{"admin"}, Keywords=new[]{"roles","permisos"} },
        new PageItem{ Title="Admin Reportes",     Url="~/AdminReports.aspx",     Roles=new[]{"admin"}, Keywords=new[]{"estadísticas","gráficos"} },
        new PageItem{ Title="Admin Publicidad",   Url="~/AdsAdmin.aspx",         Roles=new[]{"admin","marketing"}, Keywords=new[]{"ads","anuncios","publicidad"} },
        new PageItem{ Title="Backups (Admin)",    Url="~/BackupsAdmin.aspx",     Roles=new[]{"admin"}, Keywords=new[]{"copias","restore"} },
        new PageItem{ Title="Encuestas (Admin)",  Url="~/EncuestasAdmin.aspx",   Roles=new[]{"admin"}, Keywords=new[]{"surveys"} },
        new PageItem{ Title="Newsletter (Admin)", Url="~/NewsletterAdmin.aspx",  Roles=new[]{"admin"} },
        new PageItem{ Title="Bitácora",           Url="~/Bitacora.aspx",         Roles=new[]{"admin"} },
        new PageItem{ Title="Chats (Admin)",      Url="~/ChatsAdmin.aspx",       Roles=new[]{"admin","soporte"} },

        // Público técnico para anuncios random (no listarlo si no querés)
        new PageItem{ Title="Ads Public (API)",    Url="~/AdsPublic.aspx",       Roles=new[]{"admin"}, Keywords=new[]{"api"} },
    };

    public static IEnumerable<PageItem> Search(string query, IEnumerable<string> userRoles)
    {
        string q = (query ?? "").Trim();

        var roles = (userRoles ?? Enumerable.Empty<string>())
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Select(r => r.Trim().ToLowerInvariant())
                    .ToHashSet();

        Func<PageItem, bool> allowed = p =>
        {
            if (p.Roles == null || p.Roles.Length == 0) return true;
            return p.Roles.Any(r => roles.Contains((r ?? "").ToLowerInvariant()));
        };

        var src = All.Where(allowed);

        if (q == "") return src.OrderBy(p => p.Title).Take(12);

        return src.Where(p =>
                    p.Title.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    p.Url.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (p.Keywords != null && p.Keywords.Any(k => k.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0))
               )
               .OrderBy(p => p.Title)
               .Take(20);
    }
}
