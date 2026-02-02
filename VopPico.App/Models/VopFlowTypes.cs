using System;
using System.Collections.Generic;

namespace VopPico.App.Models
{
    // Node Types
    public enum VopFlowNodeType
    {
        Import,
        Start,
        FunctionCall,
        MethodCall,
        Loop,
        Conditional,
        Variable,
        Comment
    }

    public class VopFlowNodeInput
    {
        public required string id { get; set; }
        public required string type { get; set; }
        public object? value { get; set; }
    }

    public class VopFlowNodeOutput
    {
        public required string id { get; set; }
        public required string type { get; set; }
    }

    public class VopFlowNodeData
    {
        public string? module { get; set; }
        public List<string>? symbols { get; set; }
        public string? alias { get; set; }
        public string? prototype { get; set; }
        public string? target { get; set; }
        public string? method { get; set; }
        public string? loop_type { get; set; } // "for" or "while"
        public List<VopFlowNodeInput>? inputs { get; set; }
        public List<VopFlowNodeOutput>? outputs { get; set; }
        public Dictionary<string, object>? properties { get; set; }
    }

    public class VopFlowNode
    {
        public required string id { get; set; }
        public required VopFlowNodeType type { get; set; }
        public Dictionary<string, object> position { get; set; } = new();
        public required VopFlowNodeData data { get; set; } = new();
    }

    // Edge Types
    public enum VopFlowEdgeType
    {
        Flow,
        Data
    }

    public class VopFlowEdgeData
    {
        public string? label { get; set; }
        public Dictionary<string, object>? metadata { get; set; }
    }

    public class VopFlowEdge
    {
        public required string id { get; set; }
        public required string source { get; set; }
        public required string target { get; set; }
        public required VopFlowEdgeType type { get; set; }
        public bool animated { get; set; } = false;
        public VopFlowEdgeData? data { get; set; }
    }

    public class VopFlowMetadata
    {
        public required string author { get; set; }
        public required string createdAt { get; set; }
        public required string updatedAt { get; set; }
    }

    public class VopFlow
    {
        public required string version { get; set; }
        public required string name { get; set; }
        public required VopFlowMetadata metadata { get; set; }
        public required List<VopFlowNode> nodes { get; set; }
        public required List<VopFlowEdge> edges { get; set; }
    }
}
