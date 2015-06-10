using System;

namespace ABus.Config.Pipeline
{
    public class PipelineConfiguration
    {
        internal PipelineTasks StartupPipelineTasks;
        internal PipelineTasks InboundMessagePipelineTasks;
        internal PipelineTasks OutboundMessagePipelineTasks;

        public PipelineConfiguration()
        {
            this.StartupPipelineTasks = new PipelineTasks();
            this.InboundMessagePipelineTasks = new PipelineTasks();
            this.OutboundMessagePipelineTasks = new PipelineTasks();

            this.InitializePipelineConfiguration();
        }

        void InitializePipelineConfiguration()
        {
            // Register the known startup stages
            this.StartupPipelineTasks.AddStage(StartupStages.Initialize);

            // Register the known inbound message stages
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.Security);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.TransactionManagement);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.TransformInboundRawMessage);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.Deserialize);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.TransformInboundMessage);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.ExecuteHandler);

            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.ValidateBestPractices);
            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.TransformOutboundMessage);
            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.CreateRawMessage);
            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.Serialize);
            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.TransformOutboundRawMessage);
            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.SendMessage);

           
        }

        PipelineConfiguration RegisterTask(PipelineTasks pipelineTasks, string stage, PipelineTask task)
        {
            if (!pipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            pipelineTasks.AddTask(stage, task);
            return this;
        }

        PipelineConfiguration RegisterTaskBefore(PipelineTasks pipelineTasks, string stage, PipelineTask existingTask, PipelineTask task)
        {
            if (!pipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            pipelineTasks.AddTaskBefore(stage, existingTask, task);
            return this;
        }

        PipelineConfiguration RegisterTaskAfter(PipelineTasks pipelineTasks, string stage, PipelineTask existingTask, PipelineTask task)
        {
            if (!pipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            pipelineTasks.AddTaskAfter(stage, existingTask, task);
            return this;
        }

        PipelineConfiguration RegisterReplaceTask(PipelineTasks pipelineTasks, string stage, PipelineTask existingTask, PipelineTask task)
        {
            if (!pipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            pipelineTasks.ReplaceTask(stage, existingTask, task);
            return this;
        }
        internal PipelineConfiguration Register(string pipeline, string stage, PipelineTask task)
        {
            PipelineTasks pipelineTasks;
            switch (pipeline)
            {
                case "Startup":
                    pipelineTasks = this.StartupPipelineTasks;
                    break;
                case "InboundMessage":
                    pipelineTasks = this.InboundMessagePipelineTasks;
                    break;
                case "OutboundMessage":
                    pipelineTasks = this.OutboundMessagePipelineTasks;
                    break;
                default:
                    throw new ArgumentException("Unknown pipeline " + pipeline);
            }

            return this.RegisterTask(pipelineTasks, stage, task);
        }

        internal PipelineConfiguration RegisterBefore(string pipeline, string stage, PipelineTask existingTask, PipelineTask task)
        {
            PipelineTasks pipelineTasks;
            switch (pipeline)
            {
                case "Startup":
                    pipelineTasks = this.StartupPipelineTasks;
                    break;
                case "InboundMessage":
                    pipelineTasks = this.InboundMessagePipelineTasks;
                    break;
                case "OutboundMessage":
                    pipelineTasks = this.OutboundMessagePipelineTasks;
                    break;
                default:
                    throw new ArgumentException("Unknown pipeline " + pipeline);
            }

            return this.RegisterTaskBefore(pipelineTasks, stage, existingTask, task);

        }

        internal PipelineConfiguration RegisterAfter(string pipeline, string stage, PipelineTask existingTask, PipelineTask task)
        {
            PipelineTasks pipelineTasks;
            switch (pipeline)
            {
                case "Startup":
                    pipelineTasks = this.StartupPipelineTasks;
                    break;
                case "InboundMessage":
                    pipelineTasks = this.InboundMessagePipelineTasks;
                    break;
                case "OutboundMessage":
                    pipelineTasks = this.OutboundMessagePipelineTasks;
                    break;
                default:
                    throw new ArgumentException("Unknown pipeline " + pipeline);
            }

            return this.RegisterTaskAfter(pipelineTasks, stage, existingTask, task);

        }
        internal PipelineConfiguration RegisterReplace(string pipeline, string stage, PipelineTask existingTask, PipelineTask task)
        {
            PipelineTasks pipelineTasks;
            switch (pipeline)
            {
                case "Startup":
                    pipelineTasks = this.StartupPipelineTasks;
                    break;
                case "InboundMessage":
                    pipelineTasks = this.InboundMessagePipelineTasks;
                    break;
                case "OutboundMessage":
                    pipelineTasks = this.OutboundMessagePipelineTasks;
                    break;
                default:
                    throw new ArgumentException("Unknown pipeline " + pipeline);
            }

            return this.RegisterReplaceTask(pipelineTasks, stage, existingTask, task);

        }
    }
}