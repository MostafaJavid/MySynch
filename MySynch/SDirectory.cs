// -----------------------------------------------------------------------
// <copyright file="SDirectory.cs" company="">
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
    public class SDirectory : SBase ,IEquatable<SDirectory>
    {
        public IEnumerable<SDirectory> InnerDirectories { get { return _InnerDirectories;}  }
        public IEnumerable<SFile> Files { get { return _Files;}}
        private List<SFile> _Files;
        private List<SDirectory> _InnerDirectories;

        public SDirectory()
        {
            this._InnerDirectories = new List<SDirectory>();
            this._Files = new List<SFile>();
        }

        public void AddFiles(IEnumerable<SFile> files)
        {
             this._Files.AddRange(files);
        }

        public void AddDirectories(IEnumerable<SDirectory> directories)
        {
            this._InnerDirectories.AddRange(directories);
        }

        public void AddDirectory(SDirectory directory)
        {
            this._InnerDirectories.Add(directory);
        }

        public override string ToString()
        {
            var w = new StringBuilder();
            w.Append("Path=").Append(this.Path);
            return w.ToString();
        }

        public bool Equals(SDirectory other)
        {
            return this.Path.Equals(other.Path);
        }
    }
}
