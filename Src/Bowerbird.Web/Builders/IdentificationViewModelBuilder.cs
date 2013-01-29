﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using System;

namespace Bowerbird.Web.Builders
{
    public class IdentificationViewModelBuilder : IIdentificationViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IIdentificationViewFactory _identificationViewFactory;

        #endregion

        #region Constructors

        public IdentificationViewModelBuilder(
            IDocumentSession documentSession,
            IIdentificationViewFactory identificationViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(identificationViewFactory, "identificationViewFactory");

            _documentSession = documentSession;
            _identificationViewFactory = identificationViewFactory;
        }

        #endregion

        #region Methods

        public object BuildCreateIdentification(string sightingId)
        {
            return _identificationViewFactory.MakeCreateIdentification(sightingId);
        }

        public object BuildUpdateIdentification(string sightingId, int identificationId)
        {
            var result = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ParentContributionId == sightingId && x.SubContributionId == identificationId.ToString())
                .First();

            return _identificationViewFactory.MakeUpdateIdentification(result.Observation, result.User, identificationId);
        }

        #endregion
    }
}