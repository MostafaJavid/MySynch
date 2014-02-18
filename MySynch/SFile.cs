// -----------------------------------------------------------------------
// <copyright file="SFile.cs" company="">
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
    public class SFile : SBase , IComparable<SFile>
    {
        public override string ToString()
        {
            var w = new StringBuilder();
            w.Append("FilePath=").Append(this.Path);
            return w.ToString();
            
        }

        public int CompareTo(SFile other)
        {
            if (this.Path.Equals(other.Path))
            {
                if (this.LastWriteTime == other.LastWriteTime)
                {
                    return 0;
                }
                else if (this.LastWriteTime > other.LastWriteTime)
                {
                    return 1;
                }
                else if (this.LastWriteTime < other.LastWriteTime)
                {
                    return -1;
                }
            }
            throw new InvalidComparisionException();
        }
    }
}
