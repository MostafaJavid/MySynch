// -----------------------------------------------------------------------
// <copyright file="SBase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MySynch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SBase 
    {
        public string Path { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string RootPath { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
