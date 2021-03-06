﻿/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationListView
// --------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/organisationitemview'],
function ($, _, Backbone, app, ich, OrganisationItemView) {

    var OrganisationListView = Backbone.Marionette.CompositeView.extend({
        template: 'OrganisationList',

        itemView: OrganisationItemView,

        events: {
            'click #stream-load-more-button': 'onLoadMoreClicked',
            'click .sort-button': 'showSortMenu',
            'click .sort-button a': 'changeSort',
            'click .search-button': 'showHideSearch'
        },

        initialize: function (options) {
            _.bindAll(this, 'appendHtml', 'clearListAnPrepareShowLoading');

            this.collection.on('fetching', this.onLoadingStart, this);
            this.collection.on('fetched', this.onLoadingComplete, this);
            this.collection.on('reset', this.onLoadingComplete, this);
            this.collection.on('search-reset', this.clearListAnPrepareShowLoading, this);
        },

        serializeData: function () {
            return {
                Model: {
                    Query: {
                        Id: this.collection.organisationId,
                        Page: this.collection.pageSize,
                        PageSize: this.collection.page,
                        View: this.collection.viewType,
                        Sort: this.collection.sortByType
                    }
                }
            };
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            var els = this.$el.find('.organisation-item');
            _.each(this.collection.models, function (item, index) {
                var childView = new OrganisationItemView({ model: item, tagName: 'li' });
                childView.$el = $(els[index]);
                childView.showBootstrappedDetails();
                childView.delegateEvents();
                this.storeChild(childView);
            }, this);
            this._showDetails();
        },

        _showDetails: function () {
            this.$el.find('.search-button').tipsy({ gravity: 'se' });
            this.$el.find('.sort-button').tipsy({ gravity: 'se' });

            this.$el.find('h3 a').on('click', function (e) {
                e.preventDefault();
                Backbone.history.navigate($(this).attr('href'), { trigger: true });
                return false;
            });

            this.onLoadingComplete(this.collection);
            this.changeSortLabel(this.collection.sortByType);

            var that = this;
            this.$el.find('.close-call-to-action').on('click', function (e) {
                e.preventDefault();
                that.$el.find('.call-to-action').slideUp('fast', function () {
                    that.$el.find('.call-to-action').remove();
                });
                app.vent.trigger('close-call-to-action', 'organisation-explore-welcome');
                return false;
            });

            this.collection.on('criteria-changed', this.clearListAnPrepareShowLoading);

            this.enableInfiniteScroll();

            this.refresh();
        },

        changeSortLabel: function (value) {
            var label = '';
            switch (value) {
                case 'oldest':
                    label = 'Oldest';
                    break;
                case 'a-z':
                    label = 'Alphabetical (A-Z)';
                    break;
                case 'z-a':
                    label = 'Alphabetical (Z-A)';
                    break;
                case 'newest':
                    label = 'Newest';
                    break;
                default:
                    label = "Most Popular";
                    break;
            }

            this.$el.find('.sort-button .tab-list-selection').empty().text(label);
            this.$el.find('.tabs li a, .tabs .tab-list-button').tipsy.revalidate();
        },

        refresh: function () {
            _.each(this.children, function (childView) {
                childView.refresh();
            });
        },

        buildItemView: function (item, ItemView) {
            var view = new ItemView({
                model: item,
                tagName: 'li',
                className: 'organisation-item tile-organisation-details'
            });
            return view;
        },

        enableInfiniteScroll: function () {
            // Infinite scroll
            var that = this;
            this.$el.find('.organisation-list').infinitescroll({
                navSelector: '.stream-load-more', // selector for the paged navigation (it will be hidden)
                nextSelector: ".next-page", // selector for the NEXT link (to page 2)
                itemSelector: '.organisation-item', // selector for all items you'll retrieve                
                dataType: 'json',
                appendCallback: false,
                binder: $(window), // used to cache the selector for the element that will be scrolling
                maxPage: that.collection.total > 0 ? (Math.floor(that.collection.total / that.collection.pageSize)) + 1 : 0, // Total number of navigable pages
                animate: false,
                pixelsFromNavToBottom: 30,
                path: function (currentPage) {
                    //return that.buildPagingUrl(true);
                    return that.collection.searchUrl(true, currentPage);
                },
                loading: {
                    msg: $(ich.StreamLoading()),
                    speed: 1
                }
            }, function (json, opts) {
                // Get current page
                //var page = opts.state.currPage;

                for (var x = 0; x < json.Model.Organisations.PagedListItems.length; x++) {
                    that.collection.add(json.Model.Organisations.PagedListItems[x]);
                }

                var maxPage = that.collection.total > 0 ? (Math.floor(that.collection.total / that.collection.pageSize)) + 1 : 0;
                if (json.Model.Organisations.Page === maxPage) {
                    that.$el.find('.organisation-list').append('<div class="no-more-items">Hooray! You\'ve reached the end of the list.</div>');
                    opts.state.isDone = true;
                }
            });

            this.$el.find('.stream-load-more').hide();
        },

        appendHtml: function (collectionView, itemView) {
            var items = this.collection.pluck('Id');
            var index = _.indexOf(items, itemView.model.id);

            var $li = collectionView.$el.find('.organisation-items > li:eq(' + (index) + ')');

            if ($li.length === 0) {
                collectionView.$el.find('.organisation-items').append(itemView.el);
            } else {
                $li.before(itemView.el);
            }

            itemView.refresh();
        },

        onLoadMoreClicked: function () {
            this.$el.find('.stream-message, .stream-load-more').remove();
            this.collection.fetchNextPage();
        },

        onLoadNewClicked: function () {
            this.$el.find('.stream-message, .stream-load-new').remove();
            this.collection.add(this.newStreamItemsCache);
            this.newStreamItemsCache = [];
            this.newItemsCount = 0;
        },

        onLoadingStart: function (collection) {
            this.$el.find('.organisation-list').append(ich.StreamLoading());
        },

        onLoadingComplete: function (collection) {
            this.$el.find('.stream-message, .stream-loading').remove();
            if (collection.length === 0) {
                this.$el.find('.organisation-list').append(ich.StreamMessage());
            }
            if (collection.pageInfo().next) {
                var pagingInfo = {
                    NextUrl: this.collection.searchUrl(true, this.collection.page + 1)
                };

                if (this.collection.page > 1) {
                    pagingInfo['PreviousUrl'] = this.collection.searchUrl(true, this.collection.page - 1);
                }

                this.$el.find('.organisation-list').append(ich.StreamLoadMore(pagingInfo));
            }

            this.enableInfiniteScroll();
        },

        showSortMenu: function (e) {
            app.vent.trigger('close-sub-menus');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        changeSort: function (e) {
            e.preventDefault();
            this.$el.find('.sort-button .tab-list-selection').empty().text($(e.currentTarget).text());
            app.vent.trigger('close-sub-menus');
            this.clearListAnPrepareShowLoading();
            this.collection.changeSort($(e.currentTarget).data('sort'));
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: false });
            this.$el.find('.tabs li a, .tabs .tab-list-button').tipsy.revalidate();
            return false;
        },

        clearListAnPrepareShowLoading: function () {
            this.$el.find('.stream-message, .stream-load-more, .stream-load-new, .stream-loading, .no-more-items').remove();
            this.$el.find('.organisation-items').empty();
        },

        showLoading: function () {
            this.$el.find('.stream-message, .stream-load-new, .stream-load-more, .no-more-items').remove();
            this.$el.find('.organisation-items').hide();
        },

        showHideSearch: function () {
            this.trigger('toggle-search');
        }
    });

    return OrganisationListView;

});