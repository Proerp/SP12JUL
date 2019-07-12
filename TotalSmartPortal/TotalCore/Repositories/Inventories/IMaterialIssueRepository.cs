﻿using System;
using System.Collections.Generic;

using TotalModel.Models;

namespace TotalCore.Repositories.Inventories
{
    public interface IMaterialIssueRepository : IGenericWithDetailRepository<MaterialIssue, MaterialIssueDetail>
    {
    }

    public interface IMaterialIssueAPIRepository : IGenericAPIRepository
    {
        IEnumerable<MaterialIssuePendingFirmOrder> GetFirmOrders(int? locationID, int? nmvnTaskID, int? firmOrderID);

        IEnumerable<MaterialIssuePendingFirmOrderMaterial> GetPendingFirmOrderMaterials(int? locationID, int? materialIssueID, int? workOrderID, int? warehouseID, string goodsReceiptDetailIDs);
    }

}