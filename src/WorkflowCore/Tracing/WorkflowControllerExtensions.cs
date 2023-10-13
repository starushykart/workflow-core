using System.Diagnostics;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.Tracing
{
    public static class WorkflowControllerExtensions
    {
        public static Task<string> StartTraceableWorkflow<TData>(this IWorkflowController controller,
            string workflowId,
            TData data,
            string reference = null)
            where TData : class, ITraceableData, new()
        {
            return controller.StartTraceableWorkflow(workflowId, null, data, reference);
        }
        
        public static Task<string> StartTraceableWorkflow<TData>(this IWorkflowController controller,
            string workflowId,
            int? version,
            TData data,
            string reference = null)
            where TData : class, ITraceableData, new()
        {
            data.TraceId = Activity.Current?.TraceId.ToString() ?? ActivityTraceId.CreateRandom().ToString();
            data.ParentSpanId = Activity.Current?.SpanId.ToString();

            return controller.StartWorkflow(workflowId, version, data, reference);
        }
    }
}