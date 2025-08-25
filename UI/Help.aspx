<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Help.aspx.cs" Inherits="Help" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Servicio de Ayuda - LiteraryHub</title>
    <style>
        body {
            font-family: Arial, Helvetica, sans-serif;
            margin: 0; padding: 0;
            background: #faf7f4;
            color: #3b2f2a;
        }
        .container {
            max-width: 900px;
            margin: 40px auto;
            background: #ffffff;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.1);
        }
        h1 {
            text-align: center;
            margin-bottom: 30px;
            font-family: Georgia, serif;
            color: #6b4226;
        }
        .faq {
            margin-bottom: 20px;
        }
        .faq h3 {
            margin: 0;
            font-size: 18px;
            cursor: pointer;
            padding: 10px;
            background: #f5f0ea;
            border-radius: 6px;
        }
        .faq p {
            margin: 0;
            padding: 10px;
            display: none;
            border-left: 3px solid #a47148;
            background: #fbf8f4;
        }
        .faq.active p {
            display: block;
        }
        .back-link {
            display: block;
            margin-top: 30px;
            text-align: center;
        }
        .back-link a {
            text-decoration: none;
            color: #a47148;
            font-weight: bold;
        }
    </style>
    <script>
        window.onload = function () {
            var items = document.querySelectorAll(".faq h3");
            items.forEach(function (q) {
                q.addEventListener("click", function () {
                    var parent = q.parentElement;
                    parent.classList.toggle("active");
                });
            });
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Servicio de Ayuda (FAQs)</h1>

            <div class="faq">
                <h3>¿Cómo creo una cuenta en LiteraryHub?</h3>
                <p>Hacé clic en <strong>Sign up</strong>, completá tus datos y confirmá tu email. ¡Listo!</p>
            </div>

            <div class="faq">
                <h3>¿Qué hago si no recibí el correo de verificación?</h3>
                <p>Revisá tu carpeta de spam. Si no está, desde la página de <strong>Verificación Pendiente</strong> podés reenviar el correo.</p>
            </div>

            <div class="faq">
                <h3>¿Cómo cambio mi contraseña?</h3>
                <p>Ingresá en tu perfil, seleccioná <em>Configuración de cuenta</em> y hacé clic en <strong>Cambiar contraseña</strong>.</p>
            </div>

            <div class="faq">
                <h3>¿Puedo participar en grupos de lectura sin suscripción paga?</h3>
                <p>Sí, hay grupos gratuitos disponibles. Con una suscripción Premium accedés a grupos exclusivos.</p>
            </div>

            <div class="faq">
                <h3>¿Cómo contacto al soporte técnico?</h3>
                <p>Podés escribirnos a <a href="mailto:literary.hub.contact@gmail.com">literary.hub.contact@gmail.com</a> o desde la sección <strong>Contáctenos</strong> en la web.</p>
            </div>

            <div class="back-link">
                <a href="/Landing.aspx">← Volver al inicio</a>
            </div>
        </div>
    </form>
</body>
</html>
