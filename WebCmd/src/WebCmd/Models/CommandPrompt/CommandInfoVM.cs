using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebCmd.Models.CommandPrompt
{
    public class CommandInfoVM
    {
        [Required]
        public string Command { get; set; }
        public string Path { get; set; }
    }
}
