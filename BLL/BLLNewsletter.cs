using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BE;
using MPP;

namespace BLL
{
    public class BLLNewsletter
    {
        private readonly MPPNewsletter _dal = new MPPNewsletter();

        public int Crear(int userId, string title, string shortDesc, string fullDesc, string imageUrl, bool publicar = true)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Título requerido");
            if (string.IsNullOrWhiteSpace(shortDesc)) throw new ArgumentException("Short description requerida");
            if (string.IsNullOrWhiteSpace(fullDesc)) throw new ArgumentException("Full description requerida");

            title = title.Trim();
            shortDesc = shortDesc.Trim();
            fullDesc = fullDesc.Trim();
            if (title.Length > 200) title = title.Substring(0, 200);
            if (shortDesc.Length > 400) shortDesc = shortDesc.Substring(0, 400);

            // Sanitizado MUY básico (si querés permitir HTML, pasá por un sanitizer)
            fullDesc = Regex.Replace(fullDesc, @"<script[\s\S]*?</script>", "", RegexOptions.IgnoreCase);

            return _dal.Insert(new BENewsletter
            {
                Title = title,
                ShortDescription = shortDesc,
                FullDescription = fullDesc,
                ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim(),
                CreatedByUser = userId,
                IsPublished = publicar
            });
        }

        public void Eliminar(int id) => _dal.Delete(id);

        public IList<BENewsletter> ListarPublicados(int pageIndex, int pageSize, out int total)
            => _dal.ListPublished(pageIndex, pageSize, out total);

        public IList<BENewsletter> ListarTodo() => _dal.ListAll();
    }
}
