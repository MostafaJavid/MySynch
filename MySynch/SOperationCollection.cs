// -----------------------------------------------------------------------
// <copyright file="SOperationCollection.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MySynch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SOperationCollection
    {
        public string LeftRootPath { get; private set; }
        public string RightRootPath { get; private set; }
        public IEnumerable<SOperation> OperationList { get { return _OperationList; } }
        List<SOperation> _OperationList;
        public void Operate()
        {
            var trashRelatedPath = "\\" + MainWindow.TrashFolderName + "\\" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
            foreach (var o in OperationList)
            {
                o.Operate(LeftRootPath, RightRootPath, trashRelatedPath);
            }
            SaveOldDirectory(SynchHelper.GetDirectory(LeftRootPath, LeftRootPath));
            SaveOldDirectory(SynchHelper.GetDirectory(RightRootPath, RightRootPath));
        }

        public SOperationCollection(string leftRoot, string rightRoot, IEnumerable<SOperation> operations)
        {
            this.LeftRootPath = leftRoot;
            this.RightRootPath = rightRoot;
            this._OperationList = operations == null ? new List<SOperation>() : operations.OrderBy(a => a.RightPath).OrderBy(a => a.LeftPath).ToList();
        }

        private void SaveOldDirectory(SDirectory directory)
        {
            var filePath = directory.RootPath + "\\" + MainWindow.RepositoryFileName;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            var file = File.CreateText(filePath);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(directory);
            file.Write(jsonString);
            file.Flush();
            file.Dispose();
        }
    }
}
