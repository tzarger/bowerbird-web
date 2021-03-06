﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Core.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.Queries
{
    public class SpeciesViewModelQuery : ISpeciesViewModelQuery
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public SpeciesViewModelQuery(
            IDocumentSession documentSession
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildSpeciesList(SpeciesQueryInput speciesQueryInput, PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            var field = string.IsNullOrWhiteSpace(speciesQueryInput.Field) ? string.Empty : speciesQueryInput.Field;
            var queryText = string.IsNullOrWhiteSpace(speciesQueryInput.Query) ? string.Empty : speciesQueryInput.Query;
            var category = string.IsNullOrWhiteSpace(speciesQueryInput.Category) ? string.Empty : speciesQueryInput.Category;

            if (field.ToLower() == "allranks")
            {
                // Eg: query=Animalia: Chordata: Amphibia&field=rankall to get all browsable ranks for each rank level of the specified taxonomic classification
                var ranks = System.Web.HttpUtility.HtmlDecode(queryText).Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
                var queryResults = new List<object>();

                queryResults.Add(
                    _documentSession
                    .Advanced
                    .LuceneQuery<All_Species.Result, All_Species>()
                    .SelectFields<All_Species.Result>("Taxonomy", "Name", "RankPosition", "RankName", "RankType", "ParentRankName", "Ranks", "Category", "SpeciesCount", "CommonGroupNames", "CommonNames", "Synonyms", "SightingCount")
                    .Statistics(out stats)
                    .WhereEquals("RankPosition", 1)
                    .OrderBy(x => x.Name)
                    .Take(1024)
                    .ToList()
                    .Select(x => MakeSpecies(x, speciesQueryInput.LimitCommonNames))
                    .ToPagedList(
                        pagingInput.GetPage(),
                        pagingInput.GetPageSize(),
                        stats.TotalResults
                    ));

                // Get all the ranks of the specified taxonomy, plus the next rank, so as the user can browse it
                var rankCountToGet = ranks.Count() + 1 > 8 ? 8 : ranks.Count() + 1;
                
                for (var rankIndex = 1; rankIndex < rankCountToGet; rankIndex++)
                {
                    queryResults.Add(
                        _documentSession
                        .Advanced
                        .LuceneQuery<All_Species.Result, All_Species>()
                        .SelectFields<All_Species.Result>("Taxonomy", "Name", "RankPosition", "RankName", "RankType", "ParentRankName", "Ranks", "Category", "SpeciesCount", "CommonGroupNames", "CommonNames", "Synonyms", "SightingCount")
                        .Statistics(out stats)
                        .WhereEquals("ParentRankName", ranks.ElementAt(rankIndex - 1))
                        .AndAlso()
                        .WhereEquals("RankPosition", rankIndex + 1)
                        .OrderBy(x => x.Name)
                        .Take(1024)
                        .ToList()
                        .Select(x => MakeSpecies(x, speciesQueryInput.LimitCommonNames))
                        .ToPagedList(
                            pagingInput.GetPage(),
                            pagingInput.GetPageSize(),
                            stats.TotalResults
                        ));
                }

                return queryResults;
            }

            var getAllPages = false;

            var query = _documentSession
                .Advanced
                .LuceneQuery<All_Species.Result, All_Species>()
                .SelectFields<All_Species.Result>("Taxonomy", "Name", "RankPosition", "RankName", "RankType", "ParentRankName", "Ranks", "Category", "SpeciesCount", "CommonGroupNames", "CommonNames", "Synonyms", "SightingCount")
                .Statistics(out stats);

            if (field.ToLower() == "taxonomy")
            {
                query.WhereEquals("Taxonomy", queryText);
            }
            else if(field.ToLower() == "scientific")
            {
                query.WhereStartsWith("AllScientificNames", queryText);
            }
            else if(field.ToLower() == "common")
            {
                query.WhereStartsWith("AllCommonNames", queryText);
            }
            else if (field.ToLower() == "rankposition")
            {
                // Eg: query=1&field=rankposition to get all rank values in position 1
                query.WhereEquals("RankPosition", queryText);
            }
            else if (field.ToLower() == "ranktype")
            {
                // Eg: query=kingdom&field=ranktype to get all kingdoms
                query.WhereEquals("RankType", queryText);
            }
            else if(field.ToLower() == "rank2" ||
                field.ToLower() == "rank3" ||
                field.ToLower() == "rank4" ||
                field.ToLower() == "rank5" ||
                field.ToLower() == "rank6" ||
                field.ToLower() == "rank7" ||
                field.ToLower() == "rank8")
            {
                // Eg: query=animalia&field=rank2 to get all ranks in rank 2 that have a parent rank named "animalia"
                query
                    .WhereEquals("ParentRankName", queryText)
                    .AndAlso()
                    .WhereEquals("RankPosition", field.ToLower().Replace("rank", string.Empty));

                getAllPages = true;
            }
            else
            {
                query.WhereStartsWith("AllNames", queryText);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                // Additionally restrict to a single category
                query.AndAlso().WhereEquals("Category", category);
            }

            return query
                .OrderBy(x => x.Name)
                .Skip(pagingInput.GetSkipIndex())
                .Take(getAllPages ? 1024 : pagingInput.GetPageSize())
                .ToList()
                .Select(x => MakeSpecies(x, speciesQueryInput.LimitCommonNames, queryText))
                .ToPagedList(
                    pagingInput.GetPage(),
                    pagingInput.GetPageSize(),
                    stats.TotalResults
                );
        }

        private static object MakeSpecies(All_Species.Result result, bool limitCommonNames = false, string query = "")
        {
            IEnumerable<string> commonGroupNames = new string[] {};
            IEnumerable<string> commonNames = new string[] { };
            IEnumerable<string> synonyms = new string[] { };

            if (limitCommonNames)
            {
                commonGroupNames = result.CommonGroupNames.Where(x => x.ToLower().StartsWith(query.ToLower()));
                commonNames = result.CommonNames.Where(x => x.ToLower().StartsWith(query.ToLower()));
                synonyms = result.Synonyms.Where(x => x.ToLower().StartsWith(query.ToLower()));
            }
            else
            {
                commonGroupNames = result.CommonGroupNames;
                commonNames = result.CommonNames;
                synonyms = result.Synonyms;
            }

            return new
            {
                result.Taxonomy,
                result.Name,
                result.RankPosition,
                result.RankName,
                result.RankType,
                result.ParentRankName,
                result.Ranks,
                result.Category,
                result.SpeciesCount,
                result.SightingCount,
                CommonGroupNames = commonGroupNames,
                CommonNames = commonNames,
                Synonyms = synonyms,
                AllCommonNames = string.Join(", ", commonGroupNames.Concat(commonNames))
            };
        }

        //private static object MakeSpecies(Species species)
        //{
        //    return new
        //    {
        //        species.Id,
        //        species.CommonGroupNames,
        //        species.CommonNames,
        //        species.KingdomName,
        //        species.PhylumName,
        //        species.ClassName,
        //        species.OrderName,
        //        species.FamilyName,
        //        species.GenusName,
        //        species.SpeciesName
        //    };
        //}

        #endregion
    }
}