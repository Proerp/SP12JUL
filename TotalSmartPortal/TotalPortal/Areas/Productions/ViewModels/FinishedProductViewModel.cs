﻿using System;
using System.Web.Mvc;
using System.Collections.Generic;

using TotalBase.Enums;
using TotalDTO.Productions;
using TotalPortal.Builders;
using TotalPortal.ViewModels.Helpers;
using TotalPortal.Areas.Commons.ViewModels.Helpers;


namespace TotalPortal.Areas.Productions.ViewModels
{
    public class FinishedProductViewModel : FinishedProductDTO, IViewDetailViewModel<FinishedProductDetailDTO>, IPreparedPersonDropDownViewModel, IApproverDropDownViewModel, IA01SimpleViewModel, IShiftDropDownViewModel
    {
        public IEnumerable<SelectListItem> AspNetUserSelectList { get; set; }
        public IEnumerable<SelectListItem> ShiftSelectList { get; set; }
    }
}