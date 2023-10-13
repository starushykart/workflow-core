using System;
using System.Threading.Tasks;
using OpenTelemetry.Trace;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Tracing
{
    public class StepTracingMiddleware : IWorkflowStepMiddleware
    {
        public async Task<ExecutionResult> HandleAsync(IStepExecutionContext context, IStepBody body, WorkflowStepDelegate next)
        {
            using (var stepActivity = WorkflowActivityV2.StartStepActivity(context))
            {
                try
                {
                    var result = await next();
                    return result;
                }
                catch (Exception ex)
                {
                    stepActivity?.RecordException(ex);
                    throw;
                }
            }
        }
    }
}