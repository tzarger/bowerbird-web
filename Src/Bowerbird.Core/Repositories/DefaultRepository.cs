﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using Raven.Client;

namespace Bowerbird.Core.Repositories
{
    public class DefaultRepository<T> : RepositoryBase<T>
    {

        #region Members

        #endregion

        #region Constructors

        public DefaultRepository(IDocumentSession documentSession)
            : base(documentSession)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion      
      
    }
}
