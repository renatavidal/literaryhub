using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BE;
using DAL;

namespace MPP
{
    public class MPPNewsletter
    {
        private readonly Acceso _db = new Acceso();

        private static BENewsletter MapRow(DataRow r) => new BENewsletter
        {
            Id = Convert.ToInt32(r["Id"]),
            Title = Convert.ToString(r["Title"]),
            ShortDescription = Convert.ToString(r["ShortDescription"]),
            FullDescription = Convert.ToString(r["FullDescription"]),
            ImageUrl = r["ImageUrl"] == DBNull.Value ? null : Convert.ToString(r["ImageUrl"]),
            CreatedUtc = Convert.ToDateTime(r["CreatedUtc"]),
            CreatedByUser = r["CreatedByUser"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["CreatedByUser"]),
            IsPublished = Convert.ToBoolean(r["IsPublished"])
        };

        public int Insert(BENewsletter n)
        {
            var h = new Hashtable
            {
                { "@Title",            (object)n.Title            ?? DBNull.Value },
                { "@ShortDescription", (object)n.ShortDescription ?? DBNull.Value },
                { "@FullDescription",  (object)n.FullDescription  ?? DBNull.Value },
                { "@ImageUrl",         (object)n.ImageUrl         ?? DBNull.Value },
                { "@CreatedByUser",    (object)n.CreatedByUser    ?? DBNull.Value },
                { "@IsPublished",      n.IsPublished }
            };

            // usp_Newsletter_Insert hace SELECT @NewId;  -> ExecuteScalar
            int newId = _db.LeerCantidad("dbo.usp_Newsletter_Insert", h);
            return newId;
        }
        public void SubscribeUpsert(int? userId, string email,
                                     bool prefReco, bool prefLaunch,
                                     bool prefEvtPres, bool prefEvtVirt)
        {
            var h = new Hashtable {
                {"@UserId", (object)userId ?? DBNull.Value},
                {"@Email", email},
                {"@PrefReco", prefReco},
                {"@PrefLaunch", prefLaunch},
                {"@PrefEvtPres", prefEvtPres},
                {"@PrefEvtVirt", prefEvtVirt}
            };
            _db.Escribir("sp_Newsletter_SubscribeUpsert", h);
        }
        public class SubStatus
        {
            public bool IsActive { get; set; }
        }

        public SubStatus GetStatus(int? userId, string email)
        {
            var h = new Hashtable {
                {"@UserId", (object)userId ?? DBNull.Value},
                {"@Email",  (object)email  ?? DBNull.Value}
            };
            var dt = _db.Leer("sp_Newsletter_GetStatus", h);
            if (dt.Rows.Count == 0) return new SubStatus { IsActive = false };
            return new SubStatus { IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]) };
        }

        public void Unsubscribe(int? userId, string email)
        {
            var h = new Hashtable {
                {"@UserId", (object)userId ?? DBNull.Value},
                {"@Email",  (object)email  ?? DBNull.Value}
            };
            _db.Escribir("sp_Newsletter_Unsubscribe", h);
        }
        public void Delete(int id)
        {
            var h = new Hashtable { { "@Id", id } };
            _db.Escribir("dbo.usp_Newsletter_Delete", h);
        }

        // Admin
        public IList<BENewsletter> ListAll()
        {
            var dt = _db.Leer("dbo.usp_Newsletter_ListAll", null);
            var list = new List<BENewsletter>();
            foreach (DataRow r in dt.Rows) list.Add(MapRow(r));
            return list;
        }

        // Público (paginado)
        public IList<BENewsletter> ListPublished(int pageIndex, int pageSize, out int total)
        {
            var h = new Hashtable
            {
                { "@PageIndex", pageIndex },
                { "@PageSize",  pageSize  }
            };

            var ds = _db.LeerDataSet("dbo.usp_Newsletter_ListPublishedPaged", h);

            var list = new List<BENewsletter>();
            if (ds.Tables.Count > 0)
                foreach (DataRow r in ds.Tables[0].Rows)
                    list.Add(MapRow(r));

            total = 0;
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                total = Convert.ToInt32(ds.Tables[1].Rows[0][0]);

            return list;
        }
    }
}
