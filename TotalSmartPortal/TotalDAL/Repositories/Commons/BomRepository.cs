﻿using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

using TotalModel.Models;
using TotalCore.Repositories.Commons;

namespace TotalDAL.Repositories.Commons
{
    public class BomRepository : GenericWithDetailRepository<Bom, BomDetail>, IBomRepository
    {
        public BomRepository(TotalSmartPortalEntities totalSmartPortalEntities)
            : base(totalSmartPortalEntities, "BomEditable", null, "BomDeletable")
        {
        }
    }



    public class BomAPIRepository : GenericAPIRepository, IBomAPIRepository
    {
        public BomAPIRepository(TotalSmartPortalEntities totalSmartPortalEntities)
            : base(totalSmartPortalEntities, "GetBomIndexes")
        {
        }

        public IList<BomBase> GetBomBases(string searchText, int commodityID, int commodityTypeID, int commodityCategoryID, int commodityClassID, int commodityLineID)
        {
            return this.TotalSmartPortalEntities.GetBomBases(searchText, commodityID, commodityTypeID, commodityCategoryID, commodityClassID, commodityLineID).ToList();
        }

        public IList<BomValue> GetBomValues(int bomID, decimal quantity, bool overStockOnly)
        {
            IList<BomValue> bomValues = this.TotalSmartPortalEntities.GetBomValues(bomID, quantity).ToList();
            if (overStockOnly) bomValues = bomValues.Where(w => w.QuantityAvailables == null || w.Quantity > w.QuantityAvailables).ToList();
            return bomValues;
        }

        public IList<CommodityBom> GetCommodityBoms(int? bomID, int? commodityID)
        {
            return this.TotalSmartPortalEntities.GetCommodityBoms(bomID, commodityID).ToList();
        }

        public void AddCommodityBom(int? bomID, int? commodityID)
        {
            ObjectParameter[] parameters = new ObjectParameter[] { new ObjectParameter("BomID", bomID), new ObjectParameter("CommodityID", commodityID) };
            this.ExecuteFunction("AddCommodityBom", parameters);
        }

        public void RemoveCommodityBom(int? commodityBomID)
        {
            ObjectParameter[] parameters = new ObjectParameter[] { new ObjectParameter("CommodityBomID", commodityBomID) };
            this.ExecuteFunction("RemoveCommodityBom", parameters);
        }

        public void UpdateCommodityBom(int? commodityBomID, int commodityID, decimal blockUnit, decimal blockQuantity, string remarks, bool? isDefault)
        {
            ObjectParameter[] parameters = new ObjectParameter[] { new ObjectParameter("CommodityBomID", commodityBomID), new ObjectParameter("CommodityID", commodityID), new ObjectParameter("BlockUnit", blockUnit), new ObjectParameter("BlockQuantity", blockQuantity), new ObjectParameter("Remarks", remarks != null ? remarks : ""), new ObjectParameter("IsDefault", isDefault) };
            this.ExecuteFunction("UpdateCommodityBom", parameters);
        }
    }

}