﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;

using AutoMapper;

using TotalBase;
using TotalModel;
using TotalDTO;
using TotalModel.Models;
using TotalDTO.Productions;
using TotalCore.Repositories.Productions;
using TotalCore.Services.Productions;

namespace TotalService.Productions
{
    public class RecyclateService<TDto, TPrimitiveDto, TDtoDetail> : GenericWithViewDetailService<Recyclate, RecyclateDetail, RecyclateViewDetail, TDto, TPrimitiveDto, TDtoDetail>, IRecyclateService<TDto, TPrimitiveDto, TDtoDetail>
        where TDto : TPrimitiveDto, IBaseDetailEntity<TDtoDetail>, IRecyclateDTO
        where TPrimitiveDto : BaseDTO, IPrimitiveEntity, IPrimitiveDTO, new()
        where TDtoDetail : class, IPrimitiveEntity
    {
        private IRecyclateRepository recyclateRepository;
        public RecyclateService(IRecyclateRepository recyclateRepository)
            : base(recyclateRepository, "RecyclatePostSaveValidate", "RecyclateSaveRelative", "RecyclateToggleApproved", null, null, null, "GetRecyclateViewDetails")
        {
            this.recyclateRepository = recyclateRepository;
        }

        public override ICollection<RecyclateViewDetail> GetViewDetails(int recyclateID)
        {
            throw new System.ArgumentException("Invalid call GetViewDetails(id). Use GetRecyclateViewDetails instead.", "FinishProduct Service");
        }

        public ICollection<RecyclateViewDetail> GetRecyclateViewDetails(int? nmvnTaskID, int recyclateID, int locationID, int workshiftID, bool isReadonly)
        {
            ObjectParameter[] parameters = new ObjectParameter[] { new ObjectParameter("nmvnTaskID", nmvnTaskID), new ObjectParameter("RecyclateID", recyclateID), new ObjectParameter("LocationID", locationID), new ObjectParameter("WorkshiftID", workshiftID), new ObjectParameter("IsReadonly", isReadonly) };
            return this.GetViewDetails(parameters);
        }

        public List<RecyclateViewPackage> GetRecyclateViewPackages(int? nmvnTaskID, int? recyclateID)
        {
            return this.recyclateRepository.GetRecyclateViewPackages(nmvnTaskID, recyclateID);
        }

        protected override void UpdateDetail(TDto dto, Recyclate entity)
        {
            base.UpdateDetail(dto, entity);

            if (dto.RecyclatePackages != null && dto.RecyclatePackages.Count > 0)
                dto.RecyclatePackages.Each(detailDTO =>
                {
                    RecyclatePackage recyclatePackage;

                    if (detailDTO.RecyclatePackageID <= 0 || (recyclatePackage = entity.RecyclatePackages.First(detailModel => detailModel.RecyclatePackageID == detailDTO.RecyclatePackageID)) == null)
                    {
                        recyclatePackage = new RecyclatePackage();
                        entity.RecyclatePackages.Add(recyclatePackage);
                    }

                    Mapper.Map<RecyclatePackageDTO, RecyclatePackage>(detailDTO, recyclatePackage);
                });
        }

        protected override void UndoDetail(TDto dto, Recyclate entity, bool isDelete)
        {
            base.UndoDetail(dto, entity, isDelete);

            if (entity.GetID() > 0 && entity.RecyclatePackages.Count > 0)
                if (isDelete || dto.RecyclatePackages == null || dto.RecyclatePackages.Count == 0)
                    this.recyclateRepository.TotalSmartPortalEntities.RecyclatePackages.RemoveRange(entity.RecyclatePackages);
                else
                    entity.RecyclatePackages.ToList()//Have to use .ToList(): to convert enumerable to List before do remove. To correct this error: Collection was modified; enumeration operation may not execute. 
                            .Where(detailModel => !dto.RecyclatePackages.Any(detailDTO => detailDTO.RecyclatePackageID == detailModel.RecyclatePackageID))
                            .Each(deleted => this.recyclateRepository.TotalSmartPortalEntities.RecyclatePackages.Remove(deleted)); //remove deleted details
        }
    }


    public class SemifinishedProductRecyclateService : RecyclateService<RecyclateDTO<SemifinishedProductRecyclateOption>, RecyclatePrimitiveDTO<SemifinishedProductRecyclateOption>, RecyclateDetailDTO>, ISemifinishedProductRecyclateService
    {
        public SemifinishedProductRecyclateService(IRecyclateRepository recyclateRepository)
            : base(recyclateRepository) { }
    }

    public class FinishedProductRecyclateService : RecyclateService<RecyclateDTO<FinishedProductRecyclateOption>, RecyclatePrimitiveDTO<FinishedProductRecyclateOption>, RecyclateDetailDTO>, IFinishedProductRecyclateService
    {
        public FinishedProductRecyclateService(IRecyclateRepository recyclateRepository)
            : base(recyclateRepository) { }
    }

    public class FinishedItemRecyclateService : RecyclateService<RecyclateDTO<FinishedItemRecyclateOption>, RecyclatePrimitiveDTO<FinishedItemRecyclateOption>, RecyclateDetailDTO>, IFinishedItemRecyclateService
    {
        public FinishedItemRecyclateService(IRecyclateRepository recyclateRepository)
            : base(recyclateRepository) { }
    }
}
