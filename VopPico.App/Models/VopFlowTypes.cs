using System;
using System.Collections.Generic;

namespace VopPico.App.Models
{
    // v1.2: Input port definition
    public class VopFlowInputPort
    {
        public object? @default { get; set; }
    }

    // v1.2: Output binding
    public class VopFlowOutput
    {
        public required string name { get; set; }
    }

    // v1.2: Typed node data
    public class VopFlowNodeData
    {
        public string label { get; set; } = "";
        public string? target { get; set; }

        // Import-specific
        public string? module { get; set; }
        public string? alias { get; set; }

        // Function-call specific
        public string? function { get; set; }

        // Method-call specific
        public string? method { get; set; }
        public string? @object { get; set; }

        // Loop-specific
        public string? loopType { get; set; }
        public string? iterator { get; set; }
        public string? condition { get; set; }

        // Variable-specific
        public string? name { get; set; }
        public object? value { get; set; }

        // Literal-specific (v1.2)
        public string? valueType { get; set; }

        // v1.2: Structured inputs
        public Dictionary<string, VopFlowInputPort>? inputs { get; set; }

        // v1.2: Output variable binding
        public VopFlowOutput? output { get; set; }

        // Future: namespace for sub-flow organization
        public string? @namespace { get; set; }
    }

    public class VopFlowNode
    {
        public required string id { get; set; }
        public required string type { get; set; }
        public Dictionary<string, object> position { get; set; } = new();
        public VopFlowNodeData data { get; set; } = new();
        public Dictionary<string, object> properties { get; set; } = new();
    }

    public class VopFlowEdge
    {
        public required string id { get; set; }
        public required string source { get; set; }
        public required string target { get; set; }
        public string? sourceHandle { get; set; }
        public string? targetHandle { get; set; }
        public bool animated { get; set; } = false;
        public string? type { get; set; }
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
        public VopFlowMetadata metadata { get; set; } = new();
        public List<VopFlowNode> nodes { get; set; } = new();
        public List<VopFlowEdge> edges { get; set; } = new();
    }
}
