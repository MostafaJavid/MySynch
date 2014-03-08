// -----------------------------------------------------------------------
// <copyright file="SOperation.cs" company="">
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
    using System.Windows.Forms;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SOperation
    {
        public string LeftPath { get; private set; }
        public string RightPath { get; private set; }
        public OperationType Operation { get; private set; }
        public string ErrorMessage { get; private set; }
        public ItemMode Mode { get; private set; }
        public OperationResultMode OperationResult { get; private set; }
        public string Path { get { return string.IsNullOrEmpty(LeftPath) ? RightPath : LeftPath; } }


        public SOperation(string leftPath, OperationType opType, string rightPath, ItemMode mode)
        {
            this.LeftPath = leftPath;
            this.Operation = opType;
            this.RightPath = rightPath;
            this.Mode = mode;
        }

        public SOperation(SBase left, SBase right)
        {
            this.LeftPath = left.Path;
            this.RightPath = right.Path;
            if (left is SFile && right is SBase)
            {
                this.Operation = GetOperationType(left as SFile, right as SFile);
                this.Mode = ItemMode.File;
            }
            else if (left is SDirectory && right is SDirectory)
            {
                this.Operation = (left as SDirectory).Equals((SDirectory)right) ? OperationType.Equal : OperationType.Error;
                this.Mode = ItemMode.Directory;
            }
        }

        public static OperationType GetOperationType(SFile leftFile, SFile rightFile)
        {
            try
            {
                var compareResult = leftFile.CompareTo(rightFile);
                if (compareResult == 0)
                {
                    return OperationType.Equal;
                }
                else if (compareResult > 0)
                {
                    return OperationType.CopyToDestination;
                }
                else if (compareResult < 0)
                {
                    return OperationType.CopyToSource;
                }
            }
            catch (InvalidComparisionException)
            {
                return OperationType.Error;
            }
            return OperationType.Error;
        }

        public bool Operate(string leftRootPath, string rightRootPath, string TrashRelatedPath)
        {
            try
            {
                var leftFullPath = GetFullPath(leftRootPath, this.LeftPath);
                var rightFullPath = GetFullPath(rightRootPath, this.RightPath);
                switch (Operation)
                {
                    case OperationType.Equal:
                        break;
                    case OperationType.CreateInSource:
                        CreateFile(rightFullPath, GetFullPath(leftRootPath, this.RightPath), Mode);
                        break;
                    case OperationType.CreateInDestination:
                        CreateFile(leftFullPath, GetFullPath(rightRootPath, this.LeftPath), Mode);
                        break;
                    case OperationType.CopyToSource:
                        Delete(LeftPath, Mode, leftRootPath, TrashRelatedPath);
                        CreateFile(rightFullPath, leftFullPath, Mode);
                        break;
                    case OperationType.CopyToDestination:
                        Delete(RightPath, Mode, rightRootPath, TrashRelatedPath);
                        CreateFile(leftFullPath, rightFullPath, Mode);
                        break;
                    case OperationType.DeleteFromSource:
                        Delete(LeftPath, Mode, leftRootPath, TrashRelatedPath);
                        break;
                    case OperationType.DeleteFromDestination:
                        Delete(RightPath, Mode, rightRootPath, TrashRelatedPath);
                        break;
                    case OperationType.Error:
                        return false;
                        break;
                    default:
                        break;
                }
                this.OperationResult = OperationResultMode.Successful;
                this.ErrorMessage = "";
                return true;
            }
            catch (Exception ex)
            {
                this.OperationResult = OperationResultMode.Error;
                this.ErrorMessage = ex.Message;
                return false;
            }
        }

        private static void CreateFile(string source, string destination, ItemMode Mode)
        {
            if (Mode == ItemMode.File)
            {
                File.Copy(source, destination);
            }
            else
            {
                Directory.CreateDirectory(destination);
            }
        }

        private static void Delete(string relatedPath, ItemMode Mode, string rootPath, string TrashRelatedPath)
        {
            var fullPath = rootPath + relatedPath;
            if (Mode == ItemMode.File)
            {
                if (File.Exists(fullPath))
                {
                    var tempPAth = TrashRelatedPath + relatedPath;
                    CheckAllFoldersExists(rootPath, tempPAth, true);
                    File.Copy(fullPath, rootPath + tempPAth);
                    File.Delete(fullPath);
                }
            }
            else
            {
                if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath);
                }
                else
                {
                    MessageBox.Show("");
                }
            }
        }

        private static void CheckAllFoldersExists(string RootPath, string RelatedPath, bool containsFileName)
        {
            var folders = RelatedPath.Trim('\\').Split('\\');
            var tempPath = RootPath;
            for (int i = 0; i < folders.Count() - (containsFileName ? 1 : 0); i++)
            {
                var folder = folders[i];
                if (!string.IsNullOrEmpty(folder))
                {
                    tempPath += "\\" + folder;
                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                }
            }
        }

        private string GetFullPath(string rootPath, string relatedPath)
        {
            return rootPath + relatedPath;
        }
    }
}
