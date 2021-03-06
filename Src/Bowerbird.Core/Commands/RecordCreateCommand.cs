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

using System;
using System.Collections.Generic;

namespace Bowerbird.Core.Commands
{
    public class RecordCreateCommand : ICommand
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Key { get; set; }

        public DateTime ObservedOn { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public bool AnonymiseLocation { get; set; }

        public string Category { get; set; }

        public IEnumerable<string> Projects { get; set; }

        public string UserId { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}
