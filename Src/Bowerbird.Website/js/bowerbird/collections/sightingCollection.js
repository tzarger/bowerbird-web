﻿/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingCollection
// ------------------

define(['jquery', 'underscore', 'backbone', 'collections/paginatedcollection', 'models/sighting'],
function ($, _, Backbone, PaginatedCollection, Sighting) {
    
    var SightingCollection = PaginatedCollection.extend({

        model: Sighting,

        baseUrl: '/sightings',

        initialize: function () {
//            _.bindAll(this,
//            'onSuccess',
//            'onSuccessWithAddFix',
//            'getFetchOptions');
            PaginatedCollection.prototype.initialize.apply(this, arguments);
        },

//        comparator: function (streamItem1) {
//            //            log(streamItem1.get('CreatedDateTime').substr(6));
//            //            log(streamItem2.get('CreatedDateTime').substr(6));
//            //            var streamItem1CreateDate = new Date(parseInt(streamItem1.get('CreatedDateTimeOrder')));
//            //            var streamItem2CreateDate = new Date(parseInt(streamItem2.get('CreatedDateTimeOrder')));

//            //            if (streamItem1CreateDate.isAfter(streamItem2CreateDate)) {
//            //                return -1;
//            //            }

//            //            if (streamItem1CreateDate.isBefore(streamItem2CreateDate)) {
//            //                return 1;
//            //            }

//            //            return -1;
//            return -parseInt(streamItem1.get('CreatedDateTimeOrder'));
//        },

        fetchFirstPage: function () {
            this.firstPage();
        }

//        fetchNextPage: function () {
//            this.nextPage(this.getFetchOptions(true));
//        },

//        getFetchOptions: function (add) {
//            var options = {
//                data: {},
//                add: add,
//                success: null
//            };
//            if (add) {
//                options.success = this.onSuccess;
//            } else {
//                options.success = this.onSuccessWithAddFix;
//            }
//            if (this.groupOrUser) {
//                //if (this.groupOrUser instanceof Organisation || this.groupOrUser instanceof Team || this.groupOrUser instanceof Project) {
//                if (this.groupOrUser instanceof Project) {
//                    options.data.groupId = this.groupOrUser.id;
//                } else if (this.groupOrUser instanceof User) {
//                    options.data.userId = this.groupOrUser.id;
//                }
//            }
//            //            if (stream.get('Filter') != null) {
//            //                options.data.filter = stream.get('Filter');
//            //            }
//            return options;
//        },

//        onSuccess: function (collection, response) {
//            //app.stream.trigger('fetchingItemsComplete', app.stream, response);
//        },

//        onSuccessWithAddFix: function (collection, response) {
//            this.onSuccess(collection, response);
//            // Added the following manual triggering of 'add' event due to Backbone bug: https://github.com/documentcloud/backbone/issues/479
//            var self = this;
//            response.each(function (item, index) {
//                self.trigger('add', item, self, { Index: index });
//            });
//        }
    });

    return SightingCollection;

});