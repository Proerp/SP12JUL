﻿using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;

using TotalBase.Enums;
using TotalModel.Models;
using TotalCore.Repositories.Purchases;

namespace TotalDAL.Repositories.Purchases
{
    public class GoodsArrivalRepository : GenericWithDetailRepository<GoodsArrival, GoodsArrivalDetail>, IGoodsArrivalRepository
    {
        public GoodsArrivalRepository(TotalSmartPortalEntities totalSmartPortalEntities)
            : base(totalSmartPortalEntities, "GoodsArrivalEditable", "GoodsArrivalApproved")
        {
        }

        public List<BarcodeBase> GetBarcodeBases(int? goodsArrivalID)
        {
            this.TotalSmartPortalEntities.Configuration.ProxyCreationEnabled = false;
            List<BarcodeBase> barcodeBases = base.TotalSmartPortalEntities.GetBarcodeBases(goodsArrivalID).ToList();
            this.TotalSmartPortalEntities.Configuration.ProxyCreationEnabled = true;

            return barcodeBases;
        }

        public void SetBarcodeSymbologies(int? barcodeID, string symbologies)
        {
            base.TotalSmartPortalEntities.SetBarcodeSymbologies(barcodeID, symbologies);
        }

        public bool ChangeExpiryDate(int? goodsArrivalID, int? goodsArrivalDetailID, DateTime? expiryDate, string Remarks)
        {
            return base.TotalSmartPortalEntities.GoodsArrivalChangeExpiryDate(goodsArrivalID, goodsArrivalDetailID, expiryDate, Remarks) > 0;
        }
    }








    public class GoodsArrivalAPIRepository : GenericAPIRepository, IGoodsArrivalAPIRepository
    {
        public GoodsArrivalAPIRepository(TotalSmartPortalEntities totalSmartPortalEntities)
            : base(totalSmartPortalEntities, "GetGoodsArrivalIndexes")
        {
        }

        protected override ObjectParameter[] GetEntityIndexParameters(string aspUserID, DateTime fromDate, DateTime toDate)
        {
            ObjectParameter[] baseParameters = base.GetEntityIndexParameters(aspUserID, fromDate, toDate);
            ObjectParameter[] objectParameters = new ObjectParameter[] { new ObjectParameter("NMVNTaskID", this.RepositoryBag.ContainsKey("NMVNTaskID") && this.RepositoryBag["NMVNTaskID"] != null ? this.RepositoryBag["NMVNTaskID"] : 0), baseParameters[0], baseParameters[1], baseParameters[2], new ObjectParameter("PendingOnly", this.RepositoryBag.ContainsKey("PendingOnly") && this.RepositoryBag["PendingOnly"] != null ? this.RepositoryBag["PendingOnly"] : false) };

            this.RepositoryBag.Remove("NMVNTaskID");
            this.RepositoryBag.Remove("PendingOnly");

            return objectParameters;
        }

        public string GetBarcodeSymbologies(int barcodeID)
        {
            return base.TotalSmartPortalEntities.GetBarcodeSymbologies(barcodeID).FirstOrDefault();
        }

        public IEnumerable<GoodsArrivalPendingCustomer> GetCustomers(int? locationID, int? nmvnTaskID)
        {
            this.TotalSmartPortalEntities.Configuration.ProxyCreationEnabled = false;
            IEnumerable<GoodsArrivalPendingCustomer> pendingPurchaseOrderCustomers = base.TotalSmartPortalEntities.GetGoodsArrivalPendingCustomers(locationID, nmvnTaskID).ToList();
            this.TotalSmartPortalEntities.Configuration.ProxyCreationEnabled = true;

            return pendingPurchaseOrderCustomers;
        }

        public IEnumerable<GoodsArrivalPendingPurchaseOrder> GetPurchaseOrders(int? locationID, int? nmvnTaskID)
        {
            this.TotalSmartPortalEntities.Configuration.ProxyCreationEnabled = false;
            IEnumerable<GoodsArrivalPendingPurchaseOrder> pendingPurchaseOrders = base.TotalSmartPortalEntities.GetGoodsArrivalPendingPurchaseOrders(locationID, nmvnTaskID).ToList();
            this.TotalSmartPortalEntities.Configuration.ProxyCreationEnabled = true;

            return pendingPurchaseOrders;
        }

        public IEnumerable<GoodsArrivalPendingPurchaseOrderDetail> GetPendingPurchaseOrderDetails(int? locationID, int? nmvnTaskID, int? goodsArrivalID, int? purchaseOrderID, int? customerID, int? transporterID, string purchaseOrderDetailIDs)
        {
            this.TotalSmartPortalEntities.Configuration.ProxyCreationEnabled = false;
            IEnumerable<GoodsArrivalPendingPurchaseOrderDetail> pendingPurchaseOrderDetails = base.TotalSmartPortalEntities.GetGoodsArrivalPendingPurchaseOrderDetails(locationID, nmvnTaskID, goodsArrivalID, purchaseOrderID, customerID, transporterID, purchaseOrderDetailIDs).ToList();
            this.TotalSmartPortalEntities.Configuration.ProxyCreationEnabled = true;

            return pendingPurchaseOrderDetails;
        }
    }


}

