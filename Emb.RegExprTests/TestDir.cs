using System;
using System.Collections.Generic;
using System.IO;

namespace Emb.RegExprTests
{
    public static class TestDir
    {
        public static IEnumerable<string> EnumerateFiles(string path)
        {
            return Directory.Exists(path) ? Directory.EnumerateFiles(path) : Array.Empty<string>();
        }
    }
}