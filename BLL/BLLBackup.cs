// BLL/BLLBackup.cs
using System;
using System.Collections.Generic;
using System.Configuration;
using MPP;
using BE;

namespace BLL
{
    public class BLLBackup
    {
        private readonly MPPBackup _mpp = new MPPBackup();
        private string DbName => ConfigurationManager.AppSettings["DbName"] ?? "literaryhub";
        private string Folder => ConfigurationManager.AppSettings["BackupFolder"] ?? @"C:\SqlBackups\literaryhub";

        public BEBackup Create(string label) =>
            _mpp.Create(DbName, Folder, string.IsNullOrWhiteSpace(label) ? null : label.Trim());

        public List<BEBackup> List() => _mpp.List();

        public void Restore(int backupId)
        {
            var b = _mpp.GetById(backupId);
            if (b == null) throw new Exception("Backup inexistente.");
            _mpp.Restore(DbName, b.FilePath);
        }
    }
}
