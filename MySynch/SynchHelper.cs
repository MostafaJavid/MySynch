// -----------------------------------------------------------------------
// <copyright file="SynchHelper.cs" company="">
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
    public class SynchHelper
    {
        public static SDirectory GetDirectory(string fullPath, string rootPath)
        {
            var folder = new SDirectory();
            folder.Path = fullPath.Remove(0, rootPath.Length);
            folder.RootPath = rootPath;
            folder.LastAccessTime = Directory.GetLastAccessTimeUtc(fullPath);
            folder.LastWriteTime = Directory.GetLastWriteTimeUtc(fullPath);
            folder.AddFiles(GetFiles(fullPath, rootPath));

            var innerDirectories = Directory.GetDirectories(fullPath);
            var fullTrashName = rootPath + "\\" + MainWindow.TrashFolderName;
            foreach (var d in innerDirectories)
            {
                if (!d.Equals(fullTrashName))
                {
                    folder.AddDirectory(GetDirectory(d, rootPath));
                }
            }
            return folder;
        }

        private static IEnumerable<SFile> GetFiles(string directoryFullPath, string rootPath)
        {
            var files = Directory.GetFiles(directoryFullPath);
            var result = new List<SFile>();
            var fullRepositoryName = rootPath + "\\" + MainWindow.RepositoryFileName;
            foreach (var f in files)
            {
                if (!f.Equals(fullRepositoryName))
                {
                    var sfile = new SFile();
                    sfile.Path = f.Remove(0, rootPath.Length);
                    sfile.RootPath = rootPath;
                    sfile.LastAccessTime = File.GetLastAccessTimeUtc(f);
                    sfile.LastWriteTime = File.GetLastWriteTimeUtc(f);
                    sfile.CreationTime = File.GetCreationTimeUtc(f);
                    result.Add(sfile);
                }
            }
            return result;
        }

        private static List<SOperation> GetOperationsInternal(SFileAndDirectoryContainer leftContainer, SFileAndDirectoryContainer rightContainer, IEnumerable<string> LeftDeleted, IEnumerable<string> rightDeleted)
        {
            var result = new List<SOperation>();
            var Commons = from rf in rightContainer
                          from lf in leftContainer
                          where rf.Key == lf.Key && rf.GetType() == lf.GetType()
                          select new { left = lf, right = rf };
            foreach (var c in Commons)
            {
                result.Add(new SOperation(c.left.Value, c.right.Value));
            }
            var JustInLeft = leftContainer.Except(Commons.Select(a => a.left)).ToArray();
            foreach (var fd in JustInLeft)
            {
                if (!rightDeleted.Contains(fd.Key))
                {
                    result.Add(new SOperation(fd.Key, OperationType.CreateInDestination, "", fd.Value is SFile ? ItemMode.File : ItemMode.Directory));
                }
            }
            var JustInRight = rightContainer.Except(Commons.Select(a => a.right)).ToArray();
            foreach (var fd in JustInRight)
            {
                if (!LeftDeleted.Contains(fd.Key))
                {
                    result.Add(new SOperation("", OperationType.CreateInSource, fd.Key, fd.Value is SFile ? ItemMode.File : ItemMode.Directory));
                }
            }
            return result;
        }

        private static SDirectory LoadOldDirectory(string path)
        {
            var filePath = path + "\\" + MainWindow.RepositoryFileName;
            if (File.Exists(filePath))
            {
                var file = File.OpenText(filePath);
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SDirectory>(file.ReadToEnd());
                file.Dispose();
                return result;
            }
            return null;
        }

        private static IEnumerable<SOperation> GetDeletedFilesOperations(SFileAndDirectoryContainer currentDirectory, SFileAndDirectoryContainer oldDirectory, bool isLeft)
        {
            var result = GetOperationsInternal(currentDirectory, oldDirectory, new List<string>(), new List<string>());
            foreach (var item in result.Where(a => a.Operation == OperationType.CreateInSource))
            {
                if (isLeft)
                {
                    yield return new SOperation(item.LeftPath, OperationType.DeleteFromDestination, item.RightPath, item.Mode);
                }
                else
                {
                    yield return new SOperation(item.RightPath, OperationType.DeleteFromSource, item.LeftPath, item.Mode);
                }

            }
        }

        public static List<SOperation> GetOprations(string leftRootPath,string rightRootPath)
        {
            var leftDirectory = new SFileAndDirectoryContainer(SynchHelper.GetDirectory(leftRootPath, leftRootPath));
            var rightDirectory = new SFileAndDirectoryContainer(SynchHelper.GetDirectory(rightRootPath, rightRootPath));
            var oldLeftDirectory = new SFileAndDirectoryContainer(SynchHelper.LoadOldDirectory(leftRootPath));
            var oldRightDirectory = new SFileAndDirectoryContainer(SynchHelper.LoadOldDirectory(rightRootPath));
            var leftDeleted = SynchHelper.GetDeletedFilesOperations(leftDirectory, oldLeftDirectory, true).ToArray();
            var leftDeletedNames = (from t in leftDeleted select t.RightPath).ToArray();
            var rightDeleted = SynchHelper.GetDeletedFilesOperations(rightDirectory, oldRightDirectory, false).ToArray();
            var rightDeletedNames = (from t in rightDeleted select t.LeftPath).ToArray();
            var operationList = SynchHelper.GetOperationsInternal(leftDirectory, rightDirectory, leftDeletedNames, rightDeletedNames);
            operationList.AddRange(leftDeleted);
            operationList.AddRange(rightDeleted);
            return operationList;
        }
    }
}
