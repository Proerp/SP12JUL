﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;

using TotalModel.Models;
using TotalDTO.Productions;
using TotalCore.Repositories.Productions;
using TotalCore.Services.Productions;

namespace TotalService.Productions
{
    public class BlendingInstructionService : GenericWithViewDetailService<BlendingInstruction, BlendingInstructionDetail, BlendingInstructionViewDetail, BlendingInstructionDTO, BlendingInstructionPrimitiveDTO, BlendingInstructionDetailDTO>, IBlendingInstructionService
    {
        private IBlendingInstructionRepository blendingInstructionRepository;
        public BlendingInstructionService(IBlendingInstructionRepository blendingInstructionRepository)
            : base(blendingInstructionRepository, "BlendingInstructionPostSaveValidate", "BlendingInstructionSaveRelative", "BlendingInstructionToggleApproved", "BlendingInstructionToggleVoid", "BlendingInstructionToggleVoidDetail", "BlendingInstructionSaveRemarkDetail", "GetBlendingInstructionViewDetails")
        {
            this.blendingInstructionRepository = blendingInstructionRepository;
        }

        public override ICollection<BlendingInstructionViewDetail> GetViewDetails(int blendingInstructionID)
        {
            ObjectParameter[] parameters = new ObjectParameter[] { new ObjectParameter("BlendingInstructionID", blendingInstructionID) };
            return this.GetViewDetails(parameters);
        }

        public override bool Save(BlendingInstructionDTO blendingInstructionDTO)
        {
            blendingInstructionDTO.BlendingInstructionViewDetails.RemoveAll(x => x.Quantity == 0);
            return base.Save(blendingInstructionDTO);
        }

        protected override BlendingInstruction SaveThis(BlendingInstructionDTO dto)
        {
            BlendingInstruction blendingInstruction = base.SaveThis(dto);
            this.blendingInstructionRepository.SetBlendingInstructionSymbologies(blendingInstruction.BlendingInstructionID, blendingInstruction.Code, this.blendingInstructionRepository.GetMatrixSymbologies("B" + blendingInstruction.Code));
            return blendingInstruction;
        }
    }
}