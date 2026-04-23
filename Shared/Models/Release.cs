using System;
using System.Collections.Generic;

namespace Shared.Models;

public class Release
{
    public string tag_name { get; set; }
    public string name { get; set; }
    public DateTime published_at { get; set; }
    public List<Asset> assets { get; set; }
}
