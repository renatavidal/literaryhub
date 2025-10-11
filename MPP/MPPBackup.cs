// MPP/MPPBackup.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DAL;
using BE;

namespace MPP
{
    public class MPPBackup
    {
        private readonly Acceso _datos = new Acceso();

        public BEBackup Create(string dbName, string folder, string label)
        {
            var h = new Hashtable {
                {"@DbName", dbName},
                {"@Folder", folder},
                {"@Label",  (object)label ?? DBNull.Value}
            };
            var dt = _datos.Leer("usp_AppBackup_Create", h);
            if (dt.Rows.Count == 0) throw new Exception("No se pudo crear el backup.");

            return new BEBackup
            {
                Id = 0,
                FilePath = Convert.ToString(dt.Rows[0]["FilePath"]),
                SizeBytes = dt.Rows[0]["SizeBytes"] == DBNull.Value ? (long?)null : Convert.ToInt64(dt.Rows[0]["SizeBytes"]),
                Label = label,
                CreatedUtc = DateTime.UtcNow
            };
        }

        public List<BEBackup> List()
        {
            var dt = _datos.Leer("usp_AppBackup_List", new Hashtable());
            var list = new List<BEBackup>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new BEBackup
                {
                    Id = Convert.ToInt32(r["Id"]),
                    FilePath = Convert.ToString(r["FilePath"]),
                    Label = r["Label"] as string,
                    CreatedUtc = Convert.ToDateTime(r["CreatedUtc"]),
                    SizeBytes = r["SizeBytes"] == DBNull.Value ? (long?)null : Convert.ToInt64(r["SizeBytes"])
                });
            }
            return list;
        }

        public BEBackup GetById(int id)
        {
            var h = new Hashtable { { "@Id", id } };
            var dt = _datos.Leer("usp_AppBackup_GetById", h);
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new BEBackup
            {
                Id = Convert.ToInt32(r["Id"]),
                FilePath = Convert.ToString(r["FilePath"]),
                Label = r["Label"] as string,
                CreatedUtc = Convert.ToDateTime(r["CreatedUtc"]),
                SizeBytes = r["SizeBytes"] == DBNull.Value ? (long?)null : Convert.ToInt64(r["SizeBytes"])
            };
        }

        public void Restore(string dbName, string filePath)
        {
            var h = new Hashtable { { "@DbName", dbName }, { "@FilePath", filePath } };
            // 👇 llamado por nombre de 3 partes => se ejecuta en master, pero usando DAL
            _datos.Escribir("[master].dbo.usp_AppBackup_Restore", h);
        }
    }
}
