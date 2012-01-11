﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Core.Commands
{
    public class ProjectCreateCommand : CommandBase
    {
        #region Fields

        #endregion

        #region Constructors

        public ProjectCreateCommand()
        {
        }

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        #endregion

        #region Methods

        public override ICollection<ValidationResult> ValidationResults()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}