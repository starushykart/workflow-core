using System;
using System.Diagnostics;
using WorkflowCore.Interface;
using Activity = System.Diagnostics.Activity;

namespace WorkflowCore.Tracing
{
    public static class WorkflowActivityV2
    {
        public const string SourceName = "WorkflowCoreV2";
        
        private static readonly ActivitySource ActivitySource = new ActivitySource(SourceName);

        internal static Activity StartStepActivity(IStepExecutionContext context)
        {
            if (!(context.Workflow.Data is ITraceableData data))
                return null;
            
            if (!TryGetTraceId(data.TraceId, out var traceId))
                return null;
            
            if (!TryGetParentSpanId(data.ParentSpanId, out var parentSpanId))
                return null;
            
            var isInline = string.IsNullOrEmpty(context.Step.Name);
            var stepName = isInline ? "inline" : context.Step.Name;

            var activity = ActivitySource.StartActivity(stepName, ActivityKind.Internal,
                new ActivityContext(traceId, parentSpanId, ActivityTraceFlags.Recorded));
            
            if (activity != null)
            {
                activity.ActivityTraceFlags = isInline ? ActivityTraceFlags.None : ActivityTraceFlags.Recorded;
                activity.SetTag("workflow.name", context.Workflow.WorkflowDefinitionId);
                activity.SetTag("workflow.step.id", context.Step.Id);
                activity.SetTag("workflow.step.name", stepName);
            }

            return activity;
        }

        private static bool TryGetTraceId(string traceIdString, out ActivityTraceId traceId)
        {
            traceId = default;

            try
            {
                if (string.IsNullOrEmpty(traceIdString))
                    return false;
                
                traceId = ActivityTraceId.CreateFromString(traceIdString.ToCharArray());
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryGetParentSpanId(string spanIdString, out ActivitySpanId spanId)
        {
            spanId = new ActivitySpanId();

            try
            {
                if(!string.IsNullOrEmpty(spanIdString)) 
                    spanId = ActivitySpanId.CreateFromString(spanIdString.ToCharArray());
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}