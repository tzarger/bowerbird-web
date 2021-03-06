﻿/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// IdentificationDetailsView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'moment', 'voter', 'timeago', 'tipsy'],
function ($, _, Backbone, ich, app, moment, Voter) {

    var IdentificationDetailsView = Backbone.Marionette.ItemView.extend({

        template: 'ActivityItemIdentificationAdded',

        events: {
            'click h3 a': 'showItem',
            'click .vote-panel .vote-up': 'voteUp',
            'click .vote-panel .vote-down': 'voteDown',
            'click .edit-button': 'showItem'
        },

        initialize: function (options) {
            this.sighting = options.sighting;
        },

        serializeData: function () {
            var viewModel = {
                Identification: this.model.toJSON(),
                Sighting: this.sighting.toJSON(),
                Model: { AuthenticatedUser: app.authenticatedUser != undefined }
            };
            return viewModel;
        },

        currentObservationMedia: null,

        onRender: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            if (app.authenticatedUser) {
                //this.$el.find('.edit-panel').append(ich.Buttons({ EditIdentification: true, SightingId: this.model.get('SightingId'), Id: this.model.id }));

                this.$el.find('.vote-up, .vote-down, .add-comment-button').tipsy({ gravity: 'n', html: true });
                this.$el.find('.edit-button').tipsy({ gravity: 's', html: true });                
            }
        },

        showItem: function (e) {
            e.preventDefault();
            this.$el.find('.actions a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        voteUp: function (e) {
            Voter.voteUp(this.model);
            this.updateVotePanel('up');
        },

        voteDown: function (e) {
            Voter.voteDown(this.model);
            this.updateVotePanel('down');
        },

        updateVotePanel: function (direction) {
            if (direction === 'up') {
                this.$el.find('.vote-panel .vote-down').removeClass().addClass('vote-down button');
                this.$el.find('.vote-panel .vote-up').removeClass().addClass('vote-up button user-vote-score' + this.model.get('UserVoteScore'));
                this.$el.find('.vote-panel .vote-score').removeClass().addClass('vote-score user-vote-score' + this.model.get('UserVoteScore')).text(this.model.get('TotalVoteScore'));
            } else {
                this.$el.find('.vote-panel .vote-up').removeClass().addClass('vote-up button');
                this.$el.find('.vote-panel .vote-down').removeClass().addClass('vote-down button user-vote-score' + this.model.get('UserVoteScore'));
                this.$el.find('.vote-panel .vote-score').removeClass().addClass('vote-score user-vote-score' + this.model.get('UserVoteScore')).text(this.model.get('TotalVoteScore'));
            }
        }
    });

    return IdentificationDetailsView;

});