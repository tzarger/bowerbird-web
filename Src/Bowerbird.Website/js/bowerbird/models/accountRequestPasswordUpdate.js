﻿/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../../libs/moment/moment.js" />

// AccountRequestPasswordUpdate
// ----------------------------

define(['jquery', 'underscore', 'backbone'],
function ($, _, Backbone) {

    var AccountRequestPasswordUpdate = Backbone.Model.extend({
        defaults: {
            Email: ''
        },

        url: function () {
            return '/account/requestpasswordupdate';
        },

        initialize: function () {
        },

        parse: function (resp, xhr) {
            return resp.Model.AccountRequestPasswordUpdate;
        }
    });

    return AccountRequestPasswordUpdate;

});