using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixiv.Models
{
    public class ErrorModel
    {
        public bool Success { get { return false; } }
        public string Error { get; set; }
    }
}
