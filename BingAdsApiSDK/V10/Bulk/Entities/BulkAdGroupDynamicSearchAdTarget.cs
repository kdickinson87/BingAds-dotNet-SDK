﻿//=====================================================================================================================================================
// Bing Ads .NET SDK ver. 11.5
// 
// Copyright (c) Microsoft Corporation
// 
// All rights reserved. 
// 
// MS-PL License
// 
// This license governs use of the accompanying software. If you use the software, you accept this license. 
//  If you do not accept the license, do not use the software.
// 
// 1. Definitions
// 
// The terms reproduce, reproduction, derivative works, and distribution have the same meaning here as under U.S. copyright law. 
//  A contribution is the original software, or any additions or changes to the software. 
//  A contributor is any person that distributes its contribution under this license. 
//  Licensed patents  are a contributor's patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// 
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
//  each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, 
//  prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// 
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
//  each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, 
//  sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// 
// (A) No Trademark License - This license does not grant you rights to use any contributors' name, logo, or trademarks.
// 
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
//  your patent license from such contributor to the software ends automatically.
// 
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, 
//  and attribution notices that are present in the software.
// 
// (D) If you distribute any portion of the software in source code form, 
//  you may do so only under this license by including a complete copy of this license with your distribution. 
//  If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
// 
// (E) The software is licensed *as-is.* You bear the risk of using it. The contributors give no express warranties, guarantees or conditions.
//  You may have additional consumer rights under your local laws which this license cannot change. 
//  To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, 
//  fitness for a particular purpose and non-infringement.
//=====================================================================================================================================================

using System;
using System.Collections.Generic;
using Microsoft.BingAds.V10.CampaignManagement;
using Microsoft.BingAds.V10.Internal.Bulk;
using Microsoft.BingAds.V10.Internal.Bulk.Entities;
using Microsoft.BingAds.V10.Internal.Bulk.Mappings;

namespace Microsoft.BingAds.V10.Bulk.Entities
{
    /// <summary>
    /// <para>
    /// Represents an Ad Group Dynamic Search Ad Target that can be read or written in a bulk file. 
    /// </para>
    /// <para>For more information, see <see href="http://go.microsoft.com/fwlink/?LinkId=836837">Ad Group Dynamic Search Ad Target</see> </para>
    /// </summary>
    /// <seealso cref="BulkServiceManager"/>
    /// <seealso cref="BulkOperation{TStatus}"/>
    /// <seealso cref="BulkFileReader"/>
    /// <seealso cref="BulkFileWriter"/>
    public class BulkAdGroupDynamicSearchAdTarget : SingleRecordBulkEntity
    {
        /// <summary>
        /// Defines an Ad Group Criterion.
        /// </summary>
        public AdGroupCriterion AdGroupCriterion { get; set; }

        /// <summary>
        /// The name of the campaign that contains the ad group.
        /// Corresponds to the 'Campaign' field in the bulk file. 
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// The AdGroup Data Object of the Campaign Management Service. A subset of AdGroup properties are available 
        /// in the Ad Group record. For more information, see <see href="http://go.microsoft.com/fwlink/?LinkID=620252">Ad Group</see>.
        /// </summary>
        public string AdGroupName { get; set; }

        /// <summary>
        /// The historical performance data for the ad group criterion.
        /// </summary>
        public PerformanceData PerformanceData { get; private set; }

        private static readonly IBulkMapping<BulkAdGroupDynamicSearchAdTarget>[] Mappings =
        {
            new SimpleBulkMapping<BulkAdGroupDynamicSearchAdTarget>(StringTable.Status,
                c => c.AdGroupCriterion.Status.ToBulkString(),
                (v, c) => c.AdGroupCriterion.Status = v.ParseOptional<AdGroupCriterionStatus>()
            ),

            new SimpleBulkMapping<BulkAdGroupDynamicSearchAdTarget>(StringTable.Id,
                c => c.AdGroupCriterion.Id.ToBulkString(),
                (v, c) => c.AdGroupCriterion.Id = v.ParseOptional<long>()
            ),

            new SimpleBulkMapping<BulkAdGroupDynamicSearchAdTarget>(StringTable.ParentId,
                c => c.AdGroupCriterion.AdGroupId.ToBulkString(true),
                (v, c) => c.AdGroupCriterion.AdGroupId = v.Parse<long>()
            ),

            new SimpleBulkMapping<BulkAdGroupDynamicSearchAdTarget>(StringTable.Campaign,
                c => c.CampaignName,
                (v, c) => c.CampaignName = v
            ),

            new SimpleBulkMapping<BulkAdGroupDynamicSearchAdTarget>(StringTable.AdGroup,
                c => c.AdGroupName,
                (v, c) => c.AdGroupName = v
            ),
           
            new SimpleBulkMapping<BulkAdGroupDynamicSearchAdTarget>(StringTable.Bid,
                c =>
                {
                    var criterion = c.AdGroupCriterion as BiddableAdGroupCriterion;

                    if (criterion != null)
                    {
                        var fixedBid = criterion.CriterionBid as FixedBid;

                        if (fixedBid == null)
                        {
                            return null;
                        }

                        return fixedBid.Bid.ToAdGroupCriterionBidBulkString();
                    }

                    return null;
                },
                (v, c) =>
                {
                    var criterion = c.AdGroupCriterion as BiddableAdGroupCriterion;

                    if (criterion != null)
                    {
                        ((FixedBid) criterion.CriterionBid).Bid = v.ParseAdGroupCriterionBid();
                    }
                }
            ),

            new SimpleBulkMapping<BulkAdGroupDynamicSearchAdTarget>(StringTable.TrackingTemplate,
                c =>
                {
                    var criterion = c.AdGroupCriterion as BiddableAdGroupCriterion;

                    if (criterion != null)
                    {
                        return criterion.TrackingUrlTemplate.ToOptionalBulkString();
                    }

                    return null;
                },
                (v, c) =>
                {
                    var criterion = c.AdGroupCriterion as BiddableAdGroupCriterion;

                    if (criterion != null)
                    {
                        criterion.TrackingUrlTemplate = v.GetValueOrEmptyString();
                    }
                }
            ),

            new SimpleBulkMapping<BulkAdGroupDynamicSearchAdTarget>(StringTable.CustomParameter,
                c =>
                {
                    var criterion = c.AdGroupCriterion as BiddableAdGroupCriterion;

                    if (criterion != null)
                    {
                        return criterion.UrlCustomParameters.ToBulkString();
                    }

                    return null;
                },
                (v, c) =>
                {
                    var criterion = c.AdGroupCriterion as BiddableAdGroupCriterion;

                    if (criterion != null)
                    {
                        criterion.UrlCustomParameters = v.ParseCustomParameters();
                    }
                }
            ),

            new ComplexBulkMapping<BulkAdGroupDynamicSearchAdTarget>(
                (c, v) =>
                {
                    var webpage = c.AdGroupCriterion.Criterion as Webpage;

                    if (webpage == null)
                    {
                        return;
                    }

                    var webpageParameter = webpage.Parameter;

                    if (webpageParameter == null || webpageParameter.Conditions == null)
                    {
                        return;
                    }

                    WebpageConditionHelper.AddRowValuesFromConditions(webpageParameter.Conditions, v);
                },
                (v, c) =>
                {
                    var webpage = c.AdGroupCriterion.Criterion as Webpage;

                    if (webpage == null)
                    {
                        return;
                    }

                    if (webpage.Parameter != null)
                    {
                        webpage.Parameter.Conditions = new List<WebpageCondition>();
                        
                        WebpageConditionHelper.AddConditionsFromRowValues(v, webpage.Parameter.Conditions);
                    }                    
                }
                ),

            new SimpleBulkMapping<BulkAdGroupDynamicSearchAdTarget>(StringTable.Name,
                c =>
                {
                    var webpage = c.AdGroupCriterion.Criterion as Webpage;

                    return webpage != null ? webpage.Parameter.ToCriterionNameBulkString() : null;
                },
                (v, c) =>
                {
                    var webpage = c.AdGroupCriterion.Criterion as Webpage;

                    if (webpage != null && webpage.Parameter != null)
                    {
                        webpage.Parameter.CriterionName = v.ParseCriterionName();
                    }
                }
            ),
        };

        internal override void ProcessMappingsToRowValues(RowValues values, bool excludeReadonlyData)
        {
            ValidatePropertyNotNull(AdGroupCriterion, typeof(AdGroupCriterion).Name);

            this.ConvertToValues(values, Mappings);

            if (!excludeReadonlyData)
            {
                PerformanceData.WriteToRowValuesIfNotNull(PerformanceData, values);
            }
        }

        internal override void ProcessMappingsFromRowValues(RowValues values)
        {
            AdGroupCriterion = new BiddableAdGroupCriterion
            {
                Criterion = new Webpage()
                {
                    Parameter = new WebpageParameter(),
                    Type = typeof(Webpage).Name,
                },
                CriterionBid = new FixedBid
                {
                    Type = typeof(FixedBid).Name,
                },
                Type = typeof(BiddableAdGroupCriterion).Name
            };

            values.ConvertToEntity(this, Mappings);

            PerformanceData = PerformanceData.ReadFromRowValuesOrNull(values);
        }
    }
}