﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.Entities
{
    public class Comment : Entity
    {

        #region Members

        #endregion

        #region Constructors

        protected Comment() : base() { }

        public Comment(
            User createdByUser,
            string message)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(message, "message");

            SubmittedOn = DateTime.Now;

            SetDetails(
                message,
                SubmittedOn);

            EventProcessor.Raise(new EntityCreatedEvent<Comment>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime SubmittedOn { get; private set; }

        public DateTime EditedOn { get; private set; }

        public string Message { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string message, DateTime editedOn)
        {
            Message = message;
            EditedOn = editedOn;
        }

        private Comment UpdateDetails(User updatedByUser, string message)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                message,
                DateTime.Now);

            EventProcessor.Raise(new EntityUpdatedEvent<Comment>(this, updatedByUser));

            return this;
        }

        #endregion      
      
    }
}
