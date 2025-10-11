// BE/BEBackup.cs
using System;

namespace BE
{
    public class BEBackup
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Label { get; set; }
        public DateTime CreatedUtc { get; set; }
        public long? SizeBytes { get; set; }
    }
}
