﻿using System.Collections.Generic;

using TotalBase.Enums;
using TotalCore.Repositories;
using TotalCore.Services;
using TotalModel.Models;

namespace TotalService
{
    public abstract class BaseService : IBaseService
    {
        public Dictionary<string, object> ServiceBag { get; set; }

        private int userID;
        public int LocationID { get; protected set; }

        private readonly IBaseRepository baseRepository;

        public BaseService(IBaseRepository baseRepository)
        {
            this.baseRepository = baseRepository;
            this.ServiceBag = new Dictionary<string, object>();
        }

        public virtual int UserID
        {
            get { return this.userID; }
            set
            {
                if (this.UserID != value)
                {
                    this.userID = value;
                    if (this.UserID != 0)
                    {
                        OrganizationalUnitUser organizationalUnitUser = this.baseRepository.GetEntity<OrganizationalUnitUser>(w => w.UserID == this.UserID && !w.InActive, i => i.OrganizationalUnit);
                        if (organizationalUnitUser != null) this.LocationID = organizationalUnitUser.OrganizationalUnit.LocationID;
                        else throw new System.ArgumentException("Get user location", "Can not get current user location. Please check the current user organizational unit");
                    }
                    else this.LocationID = 0;
                }
            }
        }

        public virtual GlobalEnums.NmvnTaskID NmvnTaskID { get { return GlobalEnums.NmvnTaskID.UnKnown; } }
        public int GetModuleID(GlobalEnums.NmvnTaskID moduleDetailID) { return this.baseRepository.GetModuleID(moduleDetailID); }


        public virtual GlobalEnums.AccessLevel GetAccessLevel()
        { return this.GetAccessLevel(0); }
        public virtual GlobalEnums.AccessLevel GetAccessLevel(int? organizationalUnitID)
        { return GlobalEnums.AccessLevel.Deny; }


        public virtual bool GetApprovalPermitted()
        { return this.GetApprovalPermitted(0); }
        public virtual bool GetApprovalPermitted(int? organizationalUnitID)
        { return false; }


        public virtual bool GetUnApprovalPermitted()
        { return this.GetUnApprovalPermitted(0); }
        public virtual bool GetUnApprovalPermitted(int? organizationalUnitID)
        { return false; }



        public virtual bool GetVoidablePermitted()
        { return this.GetVoidablePermitted(0); }
        public virtual bool GetVoidablePermitted(int? organizationalUnitID)
        { return false; }


        public virtual bool GetUnVoidablePermitted()
        { return this.GetUnVoidablePermitted(0); }
        public virtual bool GetUnVoidablePermitted(int? organizationalUnitID)
        { return false; }


        public virtual bool GetShowDiscount()
        { return false; }

        public virtual bool GetShowListedPrice(int? priceCategoryID)
        { return false; }

        public virtual bool GetShowListedGrossPrice(int? priceCategoryID)
        { return false; }


        public string GetMatrixSymbologies(string barcode)
        { return this.baseRepository.GetMatrixSymbologies(barcode); }
    }
}
