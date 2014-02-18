// -----------------------------------------------------------------------
// <copyright file="FileAndDirectoryContainer.cs" company="">
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
    public class SFileAndDirectoryContainer : IDictionary<string, SBase>
    {
        public string RootPath { get; set; }
        private Dictionary<string, SBase> dictionary;

        public SFileAndDirectoryContainer(string rootPath)
        {
            this.RootPath = rootPath;
            this.dictionary = new Dictionary<string, SBase>();
        }

        public SFileAndDirectoryContainer(SDirectory directory)
            : this(directory != null ? directory.RootPath : "")
        {
            AddContainerEntry(this, directory);
        }

        public static void AddContainerEntry(SFileAndDirectoryContainer container, SDirectory directory)
        {
            if (directory != null)
            {
                foreach (var f in directory.Files)
                {
                    container.Add(f.Path, f);
                }

                foreach (var d in directory.InnerDirectories)
                {
                    container.Add(d.Path, d);
                    AddContainerEntry(container, d);
                }
            }
        }

        #region Implementation
        public void Add(string key, SBase value)
        {
            dictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return dictionary.Keys; }
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out SBase value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public ICollection<SBase> Values
        {
            get { return dictionary.Values; }
        }

        public SBase this[string key]
        {
            get
            {
                return dictionary[key];
            }
            set
            {
                dictionary[key] = value;
            }
        }

        public void Add(KeyValuePair<string, SBase> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, SBase> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, SBase>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(KeyValuePair<string, SBase> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, SBase>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
        #endregion
    }
}
