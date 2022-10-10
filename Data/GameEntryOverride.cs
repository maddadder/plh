using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using plhhoa;

namespace Lib
{
    public partial class GameEntryOverride : GameEntry
    {
        public List<GameEntryDescription> Descriptions {get;set;} = new List<GameEntryDescription>();
    }
    public class GameEntryDescription
    {
        [Required]
        public string Value{get;set;}
    }
}
