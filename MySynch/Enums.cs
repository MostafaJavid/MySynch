// -----------------------------------------------------------------------
// <copyright file="Enums.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MySynch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public enum OperationType
    {
        Equal,
        CreateInSource,
        CreateInDestination,
        CopyToSource,
        CopyToDestination,
        DeleteFromSource,
        DeleteFromDestination,
        Error,
    }

    public enum ItemMode
    {
        File,
        Directory,
    }

    public enum OperationResultMode
    {
        None = 0,
        Successful,
        Error,
    }
}
