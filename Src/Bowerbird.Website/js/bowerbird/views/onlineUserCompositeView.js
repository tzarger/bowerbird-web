﻿ /// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OnlineUsersCompositeView
// ----------------------------

define(['jquery','underscore','backbone','app','views/useritemview'],
function ($, _, Backbone, app, UserItemView) 
{
    var OnlineUserCompositeView = Backbone.Marionette.CompositeView.extend({

        itemView: UserItemView,

        template: 'OnlineUserList',

        className: 'onlineusers',

        regions: {
            summary: '#online-user-summary',
            users: '#online-users'
        },

        serializeData: function () {
            return {
                Count: this.collection.length
            };
        }
    });

    app.addInitializer(function (options) {
        $(function () {
            var onlineUserCompositeView = new OnlineUserCompositeView({ model: app.onlineUsers, collection: app.onlineUsers });

            onlineUserCompositeView.on('show', function () {
                app.vent.trigger('onlineUsers:rendered');
            });

            app.usersonline.show(onlineUserCompositeView);
        });
    });

    return OnlineUserCompositeView;

});