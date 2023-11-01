namespace WorkflowCore.Tracing
{
    public interface ITraceableData
    {
        string TraceId { get; set; }
        
        string ParentSpanId { get; set; }
    }
}