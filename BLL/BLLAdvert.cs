
using System;
using System.Collections.Generic;
using System.Data;
using MPP;
using BE;

namespace BLL
{
    public class BLLAdvert
    {
        private readonly MPPAdvert _mpp = new MPPAdvert();

        public List<BEAdvert> ListAll()
        {
            var dt = _mpp.ListAll();
            var list = new List<BEAdvert>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new BEAdvert
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Title = Convert.ToString(r["Title"]),
                    Body = r["Body"] as string,
                    ImageUrl = r["ImageUrl"] as string,
                    LinkUrl = r["LinkUrl"] as string,
                    IsActive = Convert.ToBoolean(r["IsActive"]),
                    Weight = Convert.ToInt32(r["Weight"]),
                    StartUtc = r["StartUtc"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["StartUtc"]),
                    EndUtc = r["EndUtc"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["EndUtc"])
                });
            }
            return list;
        }

        public int Save(BEAdvert a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (string.IsNullOrWhiteSpace(a.Title)) throw new ArgumentException("Title requerido.");
            if (string.IsNullOrWhiteSpace(a.ImageUrl) && string.IsNullOrWhiteSpace(a.Body))
                throw new ArgumentException("Debe tener imagen o texto.");
            if (a.Weight < 1) a.Weight = 1;
            return _mpp.Save(a.Id == 0 ? (int?)null : a.Id, a.Title, a.Body, a.ImageUrl, a.LinkUrl,
                             a.IsActive, a.Weight, a.StartUtc, a.EndUtc);
        }

        public void Delete(int id) => _mpp.Delete(id);

        public BEAdvert GetRandomActive()
        {
            var dt = _mpp.GetRandomActive();
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new BEAdvert
            {
                Id = Convert.ToInt32(r["Id"]),
                Title = Convert.ToString(r["Title"]),
                Body = r["Body"] as string,
                ImageUrl = r["ImageUrl"] as string,
                LinkUrl = r["LinkUrl"] as string
            };
        }
    }
}
