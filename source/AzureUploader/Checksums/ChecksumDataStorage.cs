using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureUploader.Checksums
{
    internal class ChecksumDataStorage
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, string> _storage = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public string GetChecksum(string path)
        {
            lock (_lock)
            {
                return _storage.TryGetValue(path, out var checksum) ? checksum : null;
            }
        }

        public void Store(string path, string checksum)
        {
            lock (_lock)
            {
                if (_storage.ContainsKey(path))
                {
                    _storage[path] = checksum;
                }
                else
                {
                    _storage.Add(path, checksum);
                }
            }
        }

        public string GetStorageDump()
        {
            lock (_lock)
            {
                var keys = _storage.Keys.ToArray();
                Array.Sort(keys);

                var lines = keys.Select(key => $"{key};{_storage[key]}").ToArray();
                var text = string.Join(Environment.NewLine, lines);
                return text;
            }
        }

        public void RestoreFromDump(string dump)
        {
            lock (_lock)
            {
                _storage.Clear();
                foreach (var line in dump.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var cells = line.Split(';');
                    _storage.Add(cells[0], cells[1]);
                }
            }
        }
    }
}
