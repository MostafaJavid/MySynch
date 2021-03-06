﻿// -----------------------------------------------------------------------
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
        public static SDirectory GetDirectory(string fullPath)
        {
            return GetDirectory(fullPath, fullPath);
        }

        private static SDirectory GetDirectory(string fullPath, string rootPath)
        {
            var folder = new SDirectory();
            folder.Path = fullPath.Remove(0, rootPath.Length);
            folder.RootPath = rootPath;
            folder.LastAccessTime = Directory.GetLastAccessTimeUtc(fullPath);
            folder.LastWriteTime = Directory.GetLastWriteTimeUtc(fullPath);
            folder.AddFiles(GetFiles(fullPath, rootPath));

            var innerDirectories = Directory.GetDirectories(fullPath);
            var fullTrashName = "\\" + MainWindow.TrashFolderName;
            foreach (var d in innerDirectories)
            {
                if (!d.EndsWith(fullTrashName))
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
            var repositoryName = "\\" + MainWindow.RepositoryFileName;
            foreach (var f in files)
            {
                if (!f.EndsWith(repositoryName))
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

        private static List<SOperation> GetOperationsInternal(SFileAndDirectoryContainer leftContainer, SFileAndDirectoryContainer rightContainer,
                                        IEnumerable<string> leftDeleted, IEnumerable<string> rightDeleted)
        {
            var commons = from rf in rightContainer
                          from lf in leftContainer
                          where rf.Key == lf.Key && rf.GetType() == lf.GetType()
                          select new { left = lf, right = rf };

            var result = commons.Select(c => new SOperation(c.left.Value, c.right.Value)).ToList();

            var justInLeft = leftContainer.Except(commons.Select(a => a.left)).Where(a => !rightDeleted.Contains(a.Key)).ToArray();
            result.AddRange(justInLeft.Select(fd => new SOperation(fd.Key, OperationType.CreateInDestination, "", fd.Value is SFile ? ItemMode.File : ItemMode.Directory)));

            var justInRight = rightContainer.Except(commons.Select(a => a.right)).Where(a => !leftDeleted.Contains(a.Key)).ToArray();
            result.AddRange(justInRight.Select(fd => new SOperation("", OperationType.CreateInSource, fd.Key, fd.Value is SFile ? ItemMode.File : ItemMode.Directory)));

            return result;
        }

        private static SDirectory LoadOldDirectory(string path)
        {
            var filePath = path + MainWindow.RepositoryFileName;
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

        public static List<SOperation> GetOprations(string leftRootPath, string rightRootPath)
        {
            leftRootPath = CheckFinalbackSlash(leftRootPath);
            rightRootPath = CheckFinalbackSlash(rightRootPath);
            var leftDirectory = new SFileAndDirectoryContainer(GetDirectory(leftRootPath));
            var rightDirectory = new SFileAndDirectoryContainer(GetDirectory(rightRootPath));

            var oldLeftDirectory = new SFileAndDirectoryContainer(LoadOldDirectory(leftRootPath));
            var oldRightDirectory = new SFileAndDirectoryContainer(LoadOldDirectory(rightRootPath));

            var leftDeleted = GetDeletedFilesOperations(leftDirectory, oldLeftDirectory, true).ToArray();
            var rightDeleted = GetDeletedFilesOperations(rightDirectory, oldRightDirectory, false).ToArray();

            var operationList = GetOperationsInternal(leftDirectory, rightDirectory, leftDeleted.Select(a => a.RightPath), rightDeleted.Select(a => a.LeftPath));
            operationList.AddRange(leftDeleted);
            operationList.AddRange(rightDeleted);
            return operationList;
        }

        public static string CheckFinalbackSlash(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path.Substring(path.Length - 1, 1) != "\\")
                {
                    return path + "\\";
                }
            }
            return path;
        }
    }
}
