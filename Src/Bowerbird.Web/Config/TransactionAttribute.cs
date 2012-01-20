﻿/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Web.Mvc;
using Raven.Client;
using Microsoft.Practices.ServiceLocation;

namespace Bowerbird.Web.Config
{
    public class TransactionAttribute : ActionFilterAttribute
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (ShouldCommitChanges(filterContext))
            {
                ServiceLocator.Current.GetInstance<IDocumentSession>().SaveChanges();
            }
        }

        private bool ShouldCommitChanges(ResultExecutedContext filterContext)
        {
            // Commit changes if:
            // 1: there are no unhandled exceptions and 
            // 2: rollback on model state is enabled and the model state is not valid 
            return (filterContext.Exception != null ? filterContext.ExceptionHandled : true) && 
                (filterContext.Controller.ViewData.Model != null ? filterContext.Controller.ViewData.ModelState.IsValid : true);
        }

        #endregion

    }
}
