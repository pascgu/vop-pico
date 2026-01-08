using System;
using System.Collections.Generic;

namespace VopPico.App.Models
{
    public class VopFlowNode
    {
        public required string id { get; set; }
        public required string type { get; set; }
        public Dictionary<string, object> position { get; set; } = new();
        public Dictionary<string, object> data { get; set; } = new();
        public Dictionary<string, object> properties { get; set; } = new();
    }

    public class VopFlowEdge
    {
        public required string id { get; set; }
        public required string source { get; set; }
        public required string target { get; set; }
        public bool animated { get; set; } = false;
        public required string type { get; set; }
        public string label { get; set; } = "";
        public Dictionary<string, object> metadata { get; set; } = new();
    }

    public class VopFlowMetadata
    {
        public string author { get; set; } = "";
        public string createdAt { get; set; } = "";
        public string updatedAt { get; set; } = "";
    }

    public class VopFlow
    {
        public string version { get; set; } = "";
        public required string name { get; set; }
        public List<VopFlowNode> nodes { get; set; } = new();
        public List<VopFlowEdge> edges { get; set; } = new();
        public VopFlowMetadata metadata { get; set; } = new();
    }
}
