﻿using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Emb.Core.Models
{
    public class ApplicationSettings
    {
        public IList<DataFlow> DataFlows { get; set; }
        public LogLevel LogLevel { get; set; }
        public string LogDirectoryName { get; set; }
    }
}
