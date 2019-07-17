using System.Collections.Generic;
using TotalModel.Models;

namespace TotalCore.Repositories.Productions
{
    public interface IBlendingInstructionRepository : IGenericWithDetailRepository<BlendingInstruction, BlendingInstructionDetail>
    {
        void SetBlendingInstructionSymbologies(int? blendingInstructionID, string code, string symbologies);
    }

    public interface IBlendingInstructionAPIRepository : IGenericAPIRepository
    {
        List<BlendingInstructionRunning> GetRunnings(int? locationID);
        List<BlendingInstruction> GetBlendingInstructions(string code);
        List<BlendingInstructionLog> GetBlendingInstructionLogs(int? blendingInstructionID);
    }
}